using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Text.RegularExpressions;
using ScottPlot.Plottables;
using System.Runtime.CompilerServices;
using ScottPlot;

namespace cli_life
{
    public class Figure
    {
        // Свойства класса приватные по умолчанию и инициализируются нулями или пустыми строками.
        public string Name { get; set; } = string.Empty;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public string FigureString { get; set; } = string.Empty;

        // Главный конструктор класса
        public Figure(string name, int width, int height, string figureString)
        {
            Name = name;
            Width = width;
            Height = height;
            FigureString = figureString;
        }

        // Пустой конструктор вызывает главный конструктор с пустыми значенийми.
        public Figure() : this(string.Empty, 0, 0, string.Empty)
        {
        }

        // Переопределение метода Equals
        public override bool Equals(object obj)
        {
            return obj is Figure figure &&
                   Width == figure.Width &&
                   Height == figure.Height &&
                   FigureString == figure.FigureString;
        }
    }
    public class BoardSettings
    {
        // Приватно инициализируем свойства класса с начальными значениями 0 или 0.5 для LiveDensity.
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public int CellSize { get; set; } = 0;
        public double LiveDensity { get; set; } = 0.5;

        // Главный конструктор класса.
        public BoardSettings(int width, int height, int cellSize, double liveDensity = 0.5)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            LiveDensity = liveDensity;
        }
        // Пустой конструктор вызывает главный конструктор с начальными значениями
        public BoardSettings() : this(0, 0, 0, 0.5)
        {
        }
    }
    public class FileManager
    {
        public static BoardSettings LoadBoardSettings(string filePath)
        {
            var content = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<BoardSettings>(content);
        }

        public static void SaveBoardSettings(BoardSettings settings, string filePath)
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(settings));
        }

        public static void SaveBoardState(Board board, string filePath)
        {
            var state = new StringBuilder();
            for (var i = 0; i < board.Height; ++i)
            {
                for (var j = 0; j < board.Width; ++j)
                {
                    state.Append(board.Cells[i, j].IsAlive ? '*' : ' ');
                }
                state.Append('\n');
            }

            state.Append($"cellSize={board.CellSize}\n");
            state.Append($"generation={board.Generation}");

            File.WriteAllText(filePath, state.ToString());
        }

        public static Board LoadBoardState(string filePath)
        {
            var content = File.ReadAllLines(filePath);
            var length = content.Length;

            int.TryParse(content[length - 2].Substring(content[length - 2].IndexOf('=') + 1), out int cellSize);
            int.TryParse(content[length - 1].Substring(content[length - 1].IndexOf('=') + 1), out int generation);

            cellSize = Math.Max(1, cellSize);
            generation = Math.Max(0, generation);

            var width = content[0].Length;
            var height = length - 2;
            var cells = new Cell[height / cellSize, width / cellSize];

            for (var i = 0; i < height; i += cellSize)
            {
                for (var j = 0; j < width; j += cellSize)
                {
                    cells[i / cellSize, j / cellSize] = new Cell { IsAlive = (content[i][j] == '*') };
                }
            }
            return new Board(new BoardSettings(width, height, cellSize), generation, cells);
        }

        public static Figure[] LoadFigures(string filePath)
        {
            return JsonSerializer.Deserialize<Figure[]>(File.ReadAllText(filePath));
        }

        public static void SaveFigures(Figure[] figures, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(figures, options));
        }

        public static void SavePlotPng(Plot plot, string filePath, int width = 1920, int height = 1080)
        {
            plot.SavePng(filePath, width, height);
        }
    }
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbours = new();
        private bool IsAliveNext;

        public bool DetermineNextLiveState()
        {
            int liveNeighbors = neighbours.Where(x => x.IsAlive).Count();

            if (IsAlive)
            {
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            }
            else
            {
                IsAliveNext = liveNeighbors == 3;
            }

            return IsAliveNext;
        }

        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        private int _width;
        private int _heigth;
        public Cell[,] Cells;
        public int CellSize;
        public int Columns { get { return Cells.GetLength(1); } }
        public int Rows { get { return Cells.GetLength(0); } }

        public int Width
        {
            get
            {
                return _width;
            }
            private set
            {
                _width = value;
            }
        }

        public int Height
        {
            get
            {
                return _heigth;
            }
            private set
            {
                _heigth = value;
            }
        }
        public int Generation { get; set; }
        public Board(BoardSettings settings)
        {
            Generation = 0;

            CellSize = settings.CellSize;

            Cells = new Cell[settings.Height / CellSize, settings.Width / CellSize];

            Width = Columns * CellSize;
            Height = Rows * CellSize;

            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    Cells[row, col] = new Cell();
                }
            }

            ConnectNeighbours();
            Randomize(settings.LiveDensity);
        }
        public Board(BoardSettings settings, int generation, Cell[,] cells)
        {
            Generation = generation;

            CellSize = settings.CellSize;

            Cells = cells;

            Width = settings.Width;
            Height = settings.Height;

            ConnectNeighbours();
        }
        public Board(double density = 0.5) : this(new BoardSettings(50, 20, 1, density))
        {
        }
        public void Randomize(double liveDensity)
        {
            var rand = new Random();

            foreach (var cell in Cells)
            {
                cell.IsAlive = rand.NextDouble() < liveDensity;
            }
        }
        public void Advance()
        {
            foreach (var cell in Cells)
            {
                cell.DetermineNextLiveState();
            }

            foreach (var cell in Cells)
            {
                cell.Advance();
            }

            Generation++;
        }
        private void ConnectNeighbours()
        {
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    var rowBottom = (row > 0)
                        ? row - 1
                        : Rows - 1;
                    var rowTop = (row < Rows - 1)
                        ? row + 1
                        : 0;

                    var colLeft = (col > 0)
                        ? col - 1
                        : Columns - 1;
                    var colRight = (col < Columns - 1)
                        ? col + 1
                        : 0;

                    Cells[row, col].neighbours.Add(Cells[rowTop, colLeft]);
                    Cells[row, col].neighbours.Add(Cells[rowTop, col]);
                    Cells[row, col].neighbours.Add(Cells[rowTop, colRight]);
                    Cells[row, col].neighbours.Add(Cells[row, colLeft]);
                    Cells[row, col].neighbours.Add(Cells[row, colRight]);
                    Cells[row, col].neighbours.Add(Cells[rowBottom, colLeft]);
                    Cells[row, col].neighbours.Add(Cells[rowBottom, col]);
                    Cells[row, col].neighbours.Add(Cells[rowBottom, colRight]);
                }
            }
        }
    }
    public class BoardAnalytics
    {
        public static Figure[] Figures;
        private readonly Board _board;

        public BoardAnalytics(Board board) => _board = board;

        public int GetAliveCellsCount() => GetAliveCellsCount(_board);

        public static int GetAliveCellsCount(Board board)
        {
            int aliveCellsCount = 0;

            foreach (Cell cell in board.Cells)
            {
                if (cell != null && cell.IsAlive)
                {
                    aliveCellsCount++;
                }
            }

            return aliveCellsCount;
        }
        public Dictionary<string, int> GetFiguresCount()
        {
            var result = Figures.ToDictionary(figure => figure.Name, figure => 0);
            int rows = _board.Rows, cols = _board.Columns;

            for (var row = -1; row < rows; row++)
            {
                for (var col = -1; col < cols; col++)
                {
                    foreach (var figure in Figures)
                    {
                        if (figure.Equals(MakeFigure(row, col, figure.Width, figure.Height)))
                            result[figure.Name]++;
                    }
                }
            }
            return result;
        }

        private Figure MakeFigure(int row, int col, int width, int height)
        {
            var figureString = new StringBuilder();

            for (var rowIndex = row; rowIndex < row + height; rowIndex++)
            {
                for (var colIndex = col; colIndex < col + width; colIndex++)
                {
                    figureString.Append((rowIndex < 0 || rowIndex >= _board.Rows || colIndex < 0 || colIndex >= _board.Columns)
                    ? ' '
                    : (_board.Cells[rowIndex, colIndex].IsAlive ? '*' : ' '));
                }
            }
            return new Figure("figure", width, height, figureString.ToString());
        }
    }
    class Program
    {
        const string BoardSettingsPath = "../../../boardSettings.json";
        const string BoardStatePath = "../../../SaveDataGame.txt";
        const string FiguresPath = "../../../figures.json";
        const string PlotPath = "../../../plot.png";
        static int IterationsNum = 150;
        static bool LoadFromFile = false;
        static bool SaveToFile = false;
        public static Board SimulationBoard;
        public static BoardAnalytics BoardAnalytics;
        public static BoardSettings BoardSettings = new BoardSettings(50, 20, 1, 0.5);

        static private void Reset()
        {
            try
            {
                var settings = FileManager.LoadBoardSettings(BoardSettingsPath);

                SimulationBoard = new Board(settings);
            }
            catch
            {
                SimulationBoard = new Board(BoardSettings);
            }

            BoardAnalytics = new(SimulationBoard);
        }
        static void Render()
        {
            for (var row = 0; row < SimulationBoard.Rows; row++)
            {
                for (var col = 0; col < SimulationBoard.Columns; col++)
                {
                    var cell = SimulationBoard.Cells[row, col];

                    Console.Write(cell.IsAlive
                        ? '*'
                        : ' ');
                }

                Console.Write('\n');
            }
        }
        static void ConfigureSimulationStart()
        {
            Console.Write("Число итераций симуляции:\n>>");
            IterationsNum = int.Parse(Console.ReadLine());

            Console.Write("Загрузить из файла (1 - true, 0 - false):\n>>");
            LoadFromFile = Console.ReadLine() == "1";

            Console.Write("Выполнять сохранение результата в файл (1 - true, 0 - false):\n>>");
            SaveToFile = Console.ReadLine() == "1";
        }
        static void RunRegularSimulation()
        {
            ConfigureSimulationStart();
            Reset();

            BoardAnalytics.Figures = FileManager.LoadFigures(FiguresPath);

            if (LoadFromFile)
            {
                try
                {
                    SimulationBoard = FileManager.LoadBoardState(BoardStatePath);
                    BoardAnalytics = new(SimulationBoard);
                    Console.WriteLine("Успешное чтение данных из файла.");
                }
                catch
                {
                    Console.WriteLine("Ошибка при чтении данных из файла.");
                    Reset();
                }

                Thread.Sleep(2000);
            }

            for (var index = 0; index < IterationsNum; index++)
            {
                Console.Clear();
                Render();
                Console.WriteLine($"Текущее поколение: {SimulationBoard.Generation + 1}");
                Console.WriteLine($"Число живых клеток: {BoardAnalytics.GetAliveCellsCount()}");
                Console.WriteLine("Число каждой из фигур:");
                foreach (var figure in BoardAnalytics.GetFiguresCount())
                {
                    Console.WriteLine($"{figure.Key}: {figure.Value}");
                }
                SimulationBoard.Advance();
                Thread.Sleep(1000);
            }

            if (SaveToFile)
            {
                try
                {
                    FileManager.SaveBoardState(SimulationBoard, BoardStatePath);
                    Console.WriteLine("Успешное сохранение результата в файл.");
                }
                catch
                {
                    Console.WriteLine("Ошибка при сохранении результата в файл.");
                }

            }
        }
        static void RunResearchSimulation()
        {
            var rand = new Random();

            var plot = new Plot();
            plot.XLabel("Поколение");
            plot.YLabel("Живые клетки");
            plot.ShowLegend();

            var density = new double[]
            {
                0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9
            };

            var generations = Enumerable
                .Repeat(0, IterationsNum)
                .Select((index, generation) => generation + index)
                .ToList();
            List<int> aliveCells;

            int generation;
            Board board;
            BoardAnalytics boardAnalytics;
            var boardsCount = density.Count();
            var settings = new BoardSettings(BoardSettings.Width, BoardSettings.Height, BoardSettings.CellSize);

            Scatter scatter;

            for (int boardIndex = 0; boardIndex < boardsCount; boardIndex++)
            {
                aliveCells = new();
                generation = 0;

                settings.LiveDensity = density[boardIndex];
                board = new(density[boardIndex]);
                boardAnalytics = new BoardAnalytics(board);

                while (generation < IterationsNum)
                {
                    aliveCells.Add(boardAnalytics.GetAliveCellsCount());
                    board.Advance();
                    generation++;
                }

                scatter = plot.Add.Scatter(generations, aliveCells);
                scatter.LegendText = density[boardIndex].ToString();
                scatter.LineWidth = 3;
                scatter.Color = new(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
            }

            FileManager.SavePlotPng(plot, PlotPath);
        }
        static void GetSimulationType()
        {
            Console.Write("Симуляция: \n1. Обычная симуляция\n2. Анализ с графиком\n>>");
            if (!int.TryParse(Console.ReadLine(), out int userChoice))
            {
                throw new ArgumentException("Нужно вводить число!", nameof(userChoice));
            }

            switch (userChoice)
            {
                case 1:
                    RunRegularSimulation();
                    break;
                case 2:
                    RunResearchSimulation();
                    break;
                default:
                    throw new ArgumentException("Нужно выбрать 1 или 2!", nameof(userChoice));
            }
        }
        static void Main(string[] args)
        {
            GetSimulationType();
        }
    }
}