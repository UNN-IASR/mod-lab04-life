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
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string StableFigure { get; set; }

        public Figure(string name, int width, int height, string stableFigure)
        {
            Name = name;
            Width = width;
            Height = height;
            StableFigure = stableFigure;
        }

        public Figure()
            : this(string.Empty, 0, 0, string.Empty)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Figure figure))
            {
                return false;
            }

            return figure.Width == Width && figure.Height == Height &&
                   figure.StableFigure == StableFigure;
        }
    }
    public class DataHandler
    {
        public static ConfigurationPanel LoadBoardSettings(string filePath)
        {
            string jsonContent = System.IO.File.ReadAllText(filePath);
            ConfigurationPanel configPanel = System.Text.Json.JsonSerializer.Deserialize<ConfigurationPanel>(jsonContent);
            return configPanel;
        }

        public static void SaveBoardSettings(ConfigurationPanel settings, string filePath)
        {
            string jsonString = System.Text.Json.JsonSerializer.Serialize(settings);
            System.IO.File.WriteAllText(filePath, jsonString);
        }

        public static void SaveBoardState(Board board, string filePath)
        {
            StringBuilder boardRepresentation = new StringBuilder();

            for (int rowIndex = 0; rowIndex < board.Height; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < board.Width; columnIndex++)
                {
                    char cellIndicator = board.Cells[rowIndex, columnIndex].IsAlive ? '*' : ' ';
                    boardRepresentation.Append(cellIndicator);
                }
                boardRepresentation.AppendLine();
            }

            boardRepresentation.AppendLine($"unitBlockSize={board.CellSize}");
            boardRepresentation.AppendLine($"generation={board.Generation}");

            System.IO.File.WriteAllText(filePath, boardRepresentation.ToString());
        }

        public static Board LoadBoardState(string filePath)
        {
            string[] boardDataLines = System.IO.File.ReadAllLines(filePath);
            int.TryParse(boardDataLines[^2].Split('=').Last(), out int cellSize);
            int.TryParse(boardDataLines[^1].Split('=').Last(), out int generation);

            cellSize = Math.Max(1, cellSize);
            generation = Math.Max(0, generation);

            int boardHeight = boardDataLines.Length - 2;
            int boardWidth = boardDataLines[0].Length;

            var gridConfig = new ConfigurationPanel(boardWidth, boardHeight, cellSize);

            Cell[,] gridCells = new Cell[boardHeight / cellSize, boardWidth / cellSize];

            for (int y = 0; y < boardHeight; y += cellSize)
            {
                for (int x = 0; x < boardWidth; x += cellSize)
                {
                    bool isCellAlive = boardDataLines[y][x] == '*';
                    gridCells[y / cellSize, x / cellSize] = new Cell() { IsAlive = isCellAlive };
                }
            }

            return new Board(gridConfig, generation, gridCells);
        }

        public static Figure[] LoadFigures(string filePath)
        {
            string figuresJson = System.IO.File.ReadAllText(filePath);
            Figure[] figures = System.Text.Json.JsonSerializer.Deserialize<Figure[]>(figuresJson);
            return figures;
        }

        public static void SaveFigures(Figure[] figures, string filePath)
        {
            var options = new System.Text.Json.JsonSerializerOptions()
            {
                WriteIndented = true
            };
            string figuresJson = System.Text.Json.JsonSerializer.Serialize(figures, options);
            System.IO.File.WriteAllText(filePath, figuresJson);
        }

        public static void SavePlotPng(Plot plot, string filePath, int width = 1920, int height = 1080)
        {
            plot.SavePng(filePath, width, height);
        }
    }
    public class ConfigurationPanel
    {
        public int PanelWidth { get; set; }
        public int PanelHeight { get; set; }
        public int UnitBlockSize { get; set; }
        public double PopulationDensity { get; set; }

        public ConfigurationPanel(int panelWidth, int panelHeight, int unitBlockSize, double populationDensity = 0.5)
        {
            PanelWidth = panelWidth;
            PanelHeight = panelHeight;
            UnitBlockSize = unitBlockSize;
            PopulationDensity = populationDensity;
        }

        public ConfigurationPanel()
            : this(default, default, default, 0)
        {
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
        public Board(ConfigurationPanel settings)
        {
            Generation = 0;

            CellSize = settings.UnitBlockSize;

            Cells = new Cell[settings.PanelHeight / CellSize, settings.PanelWidth  / CellSize];

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
            Randomize(settings.PopulationDensity);
        }      
        public Board(ConfigurationPanel settings, int generation, Cell[,] cells)
        {
            Generation = generation;
            CellSize = settings.UnitBlockSize;
            Cells = cells;
            Width = settings.PanelWidth ;
            Height = settings.PanelHeight;
            ConnectNeighbours();
        }        
        public Board(double density = 0.5) : this(new ConfigurationPanel(50, 20, 1, density)){}       
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
    public class Analys
    {
        public static Figure[] Figures;
        private Board _board;

        public Analys(Board board)
        {
            _board = board;
        }

        public int GetAlive()
        {
            return GetAlive(_board);
        }

        public static int GetAlive(Board board)
        {
            int aliveCount = 0;
            foreach (var cell in board.Cells)
            {
                aliveCount += cell.IsAlive ? 1 : 0;
            }
            return aliveCount;
        }

        public Dictionary<string, int> GetFigures()
        {
            Dictionary<string, int> figureCounts = new Dictionary<string, int>();
            int boardRows = _board.Rows;
            int boardColumns = _board.Columns;

            foreach (Figure fig in Figures)
            {
                figureCounts[fig.Name] = 0;
            }

            for (int rowIdx = 0; rowIdx < boardRows; rowIdx++) 
            {
                for (int colIdx = 0; colIdx < boardColumns; colIdx++) 
                {
                    foreach (Figure fig in Figures)
                    {
                        Figure tempFigure = ConstructFigure(rowIdx, colIdx, fig.Width, fig.Height);
                        if (fig.Equals(tempFigure)) 
                        {
                            figureCounts[fig.Name]++;
                        }
                    }
                }
            }
            return figureCounts;
        }

        private Figure ConstructFigure(int startRow, int startCol, int figWidth, int figHeight)
        {
            StringBuilder figRepresentation = new StringBuilder();
            for (int row = startRow; row < startRow + figHeight; row++)
            {
                for (int col = startCol; col < startCol + figWidth; col++)
                {
                    char figChar = ' ';
                    if (row >= 0 && row < _board.Rows && col >= 0 && col < _board.Columns)
                    {
                        figChar = _board.Cells[row, col].IsAlive ? '*' : ' ';
                    }
                    figRepresentation.Append(figChar);
                }
            }
            return new Figure("figure", figWidth, figHeight, figRepresentation.ToString());
        }
    }
    class Program
    {
        static int IterationsNum = 100;

        static bool LoadFromFile = false;

        static bool SaveToFile = false;

        public static Board SimulationBoard;

        public static Analys BoardAnalysis;

        public static ConfigurationPanel Settings = new ConfigurationPanel(100, 40, 1, 0.5);

        const string SettingsJsonPath = "../../../Settings_board.json";
       
        const string BoardStateFilePath = "../../../Life_save1.txt";
        
        const string FiguresJsonPath = "../../../stableFigure.json";
   
        const string PlotPngPath = "../../../picture.png";      
       
        static private void Reset()
        {
            try
            {
                var settings = DataHandler.LoadBoardSettings(SettingsJsonPath);

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
        static void SimulationStart()
        {
            Console.Write("Введите количество итераций для симуляции: ");
            int.TryParse(Console.ReadLine(), out IterationsNum);

            Console.Write("Загружать стартовое состояние из файла? (да - 1, нет - 0): ");
            LoadFromFile = Console.ReadLine().Trim() == "1";

            Console.Write("Сохранять ли конечное состояние в файл? (да - 1, нет - 0): ");
            SaveToFile = Console.ReadLine().Trim() == "1";
        }
        static void RunRegularSimulation()
        {
            SimulationStart();
            Reset();

            Analys.Figures = DataHandler.LoadFigures(FiguresJsonPath);

            if (LoadFromFile)
            {
                try
                {
                    SimulationBoard = DataHandler.LoadBoardState(BoardStateFilePath);
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

            for (var index = 0; index < IterationsNum; index++)
            {
                Console.Clear();
                Render();
                Console.WriteLine($"Текущая итерация: {SimulationBoard.Generation}");
                Console.WriteLine($"Число живых клеток: {BoardAnalysis.GetAlive()}");
                Console.WriteLine("Число каждой из фигур:");
                foreach (var figure in BoardAnalysis.GetFigures())
                {
                    Console.WriteLine($"{figure.Key}: {figure.Value}");
                }
                SimulationBoard.Advance();
                Thread.Sleep(2000);
            }

            if (SaveToFile)
            {
                try
                {
                    DataHandler.SaveBoardState(SimulationBoard, BoardStateFilePath);
                    Console.WriteLine("Сохранение результата в файл.");
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
            plot.XLabel("Итерации");
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
            Analys boardAnalysis;
            var boardsCount = density.Count();
            var settings = new ConfigurationPanel(Settings.PanelWidth , Settings.PanelHeight, Settings.UnitBlockSize);

            Scatter chart;

            for (int boardIndex = 0; boardIndex < boardsCount; boardIndex++)
            {
                aliveCells = new();
                generation = 0;

                settings.PopulationDensity = density[boardIndex];
                board = new(density[boardIndex]);
                boardAnalysis = new Analys(board);

                while (generation < IterationsNum)
                {
                    aliveCells.Add(boardAnalysis.GetAlive());
                    board.Advance();
                    generation++;
                }

                chart = plot.Add.Scatter(generations, aliveCells);
                chart.LegendText = density[boardIndex].ToString();
                chart.LineWidth = 3;
                chart.Color = new(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
            }

            DataHandler.SavePlotPng(plot, PlotPngPath);
        }       
        static void GetSimulationType()
        {
            Console.Write("Выберите тип симуляции для запуска: \n1. Анализ \n2. Построение графика\nВыбор: ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                throw new ArgumentException("Ответ не является числом", nameof(choice));
            }
            switch (choice)
            {
                case 1:
                    RunRegularSimulation();
                    break;
                case 2:
                    RunResearchSimulation();
                    break;
                default:
                    throw new ArgumentException("Выберите корректный вариант", nameof(choice));
            }
        }     
        static void Main(string[] args)
        {
            GetSimulationType();
        }
    }
}