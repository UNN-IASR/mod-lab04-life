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

        public Board(SettingUpBorders settings)
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

        public Board(SettingUpBorders settings, int generation, Cell[,] cells)
        {
            Generation = generation;

            CellSize = settings.CellSize;

            Cells = cells;

            Width = settings.Width;
            Height = settings.Height;

            ConnectNeighbours();
        }

        public Board(double density = 0.5) : this(new SettingUpBorders(50, 20, 1, density))
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

    public class Figure
    {
        public string Name { get; set; } = string.Empty;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public string FormatFigure { get; set; } = string.Empty;

        public Figure() { }

        public Figure(string name, int width, int height, string formatFigure) : this()
        {
            Name = name;
            Width = width;
            Height = height;
            FormatFigure = formatFigure;
        }

        public override bool Equals(object obj) =>
            obj is Figure figure && figure.Width == Width && figure.Height == Height
                && figure.FormatFigure == FormatFigure;

        public override int GetHashCode() =>
            HashCode.Combine(Width, Height, FormatFigure);
    }

    public class SettingUpBorders
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }

        public SettingUpBorders(int width, int height, int cellSize, double liveDensity = 0.5)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            LiveDensity = liveDensity;
        }

        public SettingUpBorders() : this(0, 0, 0, 0) { }
    }

    public class WorkingWithFiles
    {
        public static SettingUpBorders LoadBoardSettings(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var settings = JsonSerializer.Deserialize<SettingUpBorders>(json);

            return settings;
        }

        public static void SaveBoardSettings(SettingUpBorders settings, string filePath)
        {
            var json = JsonSerializer.Serialize(settings);
            File.WriteAllText(filePath, json);
        }

        public static void SaveBoardState(Board board, string filePath)
        {
            var boardStringRepr = string.Empty;

            for (var row = 0; row < board.Height; row++)
            {
                for (var col = 0; col < board.Width; col++)
                {
                    boardStringRepr +=
                        (board.Cells[row, col].IsAlive)
                            ? '*'
                            : ' ';
                }
                boardStringRepr += '\n';
            }
            boardStringRepr += $"cellSize={board.CellSize}\n";
            boardStringRepr += $"generation={board.Generation}";

            File.WriteAllText(filePath, boardStringRepr);
        }

        public static Board LoadBoardState(string filePath)
        {
            var boardStringArrayRepr = File.ReadAllLines(filePath);

            int.TryParse(
                boardStringArrayRepr[boardStringArrayRepr.Length - 2]
                    .Substring(
                        boardStringArrayRepr[boardStringArrayRepr.Length - 2]
                        .IndexOf('=') + 1),
                out int cellSize);

            int.TryParse(
                boardStringArrayRepr[boardStringArrayRepr.Length - 1]
                    .Substring(
                        boardStringArrayRepr[boardStringArrayRepr.Length - 1]
                        .IndexOf('=') + 1),
                out int generation);

            cellSize = cellSize > 1
                ? cellSize
                : 1;

            generation = generation > 0
                ? generation
                : 0;

            var height = boardStringArrayRepr.Length - 2;
            var width = boardStringArrayRepr[0].Length;

            var settings = new SettingUpBorders(width, height, cellSize);

            var cells = new Cell[height / cellSize, width / cellSize];

            for (var row = 0; row < height; row += cellSize)
            {
                for (var col = 0; col < width; col += cellSize)
                {
                    cells[row / cellSize, col / cellSize] = new Cell
                    {
                        IsAlive = (boardStringArrayRepr[row][col] == '*')
                    };
                }
            }

            return new Board(settings, generation, cells);
        }

        public static Figure[] LoadFigures(string filePath)
        {
            var json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<Figure[]>(json);
        }

        public static void SaveFigures(Figure[] figures, string filePath)
        {
            var json = JsonSerializer.Serialize(figures, new
                JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }

        public static void SavePlotPng(Plot plot, string filePath, int width = 1920, int heigth = 1080)
        {
            plot.SavePng(filePath, 1920, 1080);
        }
    }

    public class AnalysisOfBorder
    {
        public static Figure[] Figures;
        private readonly Board _board;

        public AnalysisOfBorder(Board board) => _board = board;

        public int GetAliveCellsCount() => GetAliveCellsCount(_board);

        public static int GetAliveCellsCount(Board board)
        {
            int result = 0;
            for (int i = 0; i < board.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < board.Cells.GetLength(1); j++)
                {
                    if (board.Cells[i, j].IsAlive)
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        public Dictionary<string, int> GetFiguresCount()
        {
            return Figures.ToDictionary(figure => figure.Name, figure => CountFigures(figure.Name));
        }

        private int CountFigures(string figureName)
        {
            var count = 0;
            for (var row = 0; row < _board.Rows; row++)
            {
                for (var col = 0; col < _board.Columns; col++)
                {
                    if (Figures.Any(figure => figure.Name == figureName &&
                                              figure.Equals(MakeFigure(row, col, figure.Width, figure.Height))))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private Figure MakeFigure(int row, int col, int width, int height)
        {
            var figureString = new StringBuilder();

            for (var rowIndex = row; rowIndex < row + height; rowIndex++)
            {
                for (var colIndex = col; colIndex < col + width; colIndex++)
                {
                    figureString.Append(GetCellRepresentation(rowIndex, colIndex));
                }
            }

            return new Figure("figure", width, height, figureString.ToString());
        }

        private char GetCellRepresentation(int rowIndex, int colIndex)
        {
            if (IsOutOfBounds(rowIndex, colIndex))
                return ' ';

            return _board.Cells[rowIndex, colIndex].IsAlive ? '*' : ' ';
        }

        private bool IsOutOfBounds(int rowIndex, int colIndex) =>
            rowIndex < 0 || rowIndex >= _board.Rows || colIndex < 0 || colIndex >= _board.Columns;
    }

    class Program
    {
        const string SettingsJsonPath = "../../../boardSettings.json";

        const string BoardStateFilePath = "../../../savesFile.txt";

        const string FiguresJsonPath = "../../../figuresFile.json";

        const string PlotPngPath = "../../../plot.png";

        static int IterationsNum = 100;

        static bool LoadFromFile = false;

        static bool SaveToFile = false;

        public static Board SimulationBoard;

        public static AnalysisOfBorder BoardAnalysis;

        public static SettingUpBorders Settings = new SettingUpBorders(100, 40, 1, 0.5);

        static private void Reset()
        {
            try
            {
                var settings = WorkingWithFiles.LoadBoardSettings(SettingsJsonPath);

                SimulationBoard = new Board(settings);
            }
            catch
            {
                SimulationBoard = new Board(Settings);
            }

            BoardAnalysis = new(SimulationBoard);
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

        static void ProgramLaunchSettings()
        {
            Console.Write("Какое количество итераций требуется выполнить? ");
            if (!int.TryParse(Console.ReadLine(), out int iterationsNum))
            {
                Console.WriteLine("Введено недопустимое значение. Пожалуйста, введите число.");
            }
            IterationsNum = iterationsNum;

            Console.Write("Желаете ли вы загрузить данные из файла? (0. нет, 1. да): ");
            int.TryParse(Console.ReadLine(), out int loadFileResponse);
            LoadFromFile =
                loadFileResponse == 0
                    ? false
                    : true;


            Console.Write("Необходимо ли сохранить результаты в файл после завершения? (0. нет, 1. да): ");
            int.TryParse(Console.ReadLine(), out int saveFileResponse);
            SaveToFile =
                saveFileResponse == 0
                    ? false
                    : true;
        }

        static void RunningProgram()
        {
            InitializeProgram();
            LoadFigures();
            LoadBoardStateIfRequired();

            PerformIterations();

            SaveBoardStateIfRequired();
        }

        static void InitializeProgram()
        {
            ProgramLaunchSettings();
            Reset();
        }

        static void LoadFigures()
        {
            AnalysisOfBorder.Figures = WorkingWithFiles.LoadFigures(FiguresJsonPath);
        }

        static void LoadBoardStateIfRequired()
        {
            if (LoadFromFile)
            {
                try
                {
                    SimulationBoard = WorkingWithFiles.LoadBoardState(BoardStateFilePath);
                    BoardAnalysis = new(SimulationBoard);
                    Console.WriteLine("Успешное чтение данных из файла.");
                }
                catch
                {
                    Console.WriteLine("Ошибка при чтении данных из файла.");
                    Reset();
                }
                Thread.Sleep(2000);
            }
        }

        static void PerformIterations()
        {
            for (var index = 0; index < IterationsNum; index++)
            {
                Console.Clear();
                Render();
                DisplayGenerationInfo();
                SimulationBoard.Advance();
                Thread.Sleep(1000);
            }
        }

        static void DisplayGenerationInfo()
        {
            Console.WriteLine($"Текущее поколение: {SimulationBoard.Generation}");
            Console.WriteLine($"Число живых клеток: {BoardAnalysis.GetAliveCellsCount()}");
            Console.WriteLine("Число каждой из фигур:");
            foreach (var figure in BoardAnalysis.GetFiguresCount())
            {
                Console.WriteLine($"{figure.Key}: {figure.Value}");
            }
        }

        static void SaveBoardStateIfRequired()
        {
            if (SaveToFile)
            {
                try
                {
                    WorkingWithFiles.SaveBoardState(SimulationBoard, BoardStateFilePath);
                    Console.WriteLine("Успешное сохранение результата в файл.");
                }
                catch
                {
                    Console.WriteLine("Ошибка при сохранении результата в файл.");
                }
            }
        }

        static void PlotingGraphy()
        {
            Random random = new Random();

            Plot plot = new Plot();
            plot.XLabel("Поколение");
            plot.YLabel("Особи");
            plot.ShowLegend();

            double[] densities = {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};

            List<int> generations = Enumerable.Range(0, IterationsNum).ToList();
            List<int> aliveCells;

            Board board;
            AnalysisOfBorder boardAnalysis;
            int boardsCount = densities.Length;
            SettingUpBorders settings = new SettingUpBorders(Settings.Width, Settings.Height, Settings.CellSize);

            for (int i = 0; i < boardsCount; i++)
            {
                aliveCells = new List<int>();
                settings.LiveDensity = densities[i];
                board = new Board(densities[i]);
                boardAnalysis = new AnalysisOfBorder(board);

                for (int generation = 0; generation < IterationsNum; generation++)
                {
                    aliveCells.Add(boardAnalysis.GetAliveCellsCount());
                    board.Advance();
                }

                Scatter scatter = plot.Add.Scatter(generations, aliveCells);
                scatter.LegendText = densities[i].ToString();
                scatter.LineWidth = 3;
                scatter.Color = new Color(random.Next(256), random.Next(256), random.Next(256));
            }

            WorkingWithFiles.SavePlotPng(plot, PlotPngPath);
        }

        static void LaunchSelection()
        {
            Console.Write("Меню: \n1. Симуляция\n2. Анализ (plot)\nВыбор: ");
            if (!int.TryParse(Console.ReadLine(), out int userChoice))
            {
                throw new ArgumentException("Ответ не число", nameof(userChoice));
            }

            switch (userChoice)
            {
                case 1:
                    RunningProgram();
                    break;
                case 2:
                    PlotingGraphy();
                    break;
                default:
                    throw new ArgumentException("Неккоректный выбор", nameof(userChoice));
            }
        }

        static void Main(string[] args)
        {
            LaunchSelection();
        }
    }
}