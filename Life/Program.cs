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

        public Board(double density = 0.5) : this(new BoardSettings(100, 50, 1, density))
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
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string FigureRepresentation { get; set; }

        public Figure(string name, int width, int height, string figureRepresentation)
        {
            Name = name;
            Width = width;
            Height = height;
            FigureRepresentation = figureRepresentation;
        }

        public Figure() : this(string.Empty, 0, 0, string.Empty) { }

        public override bool Equals(object obj)
        {
            if (obj is Figure figure)
            {
                return figure.Width == Width
                && figure.Height == Height
                && figure.FigureRepresentation == FigureRepresentation;
            }
            return false;
        }
    }

    public class BoardSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }

        public BoardSettings(int width, int height, int cellSize, double liveDensity = 0.5)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            LiveDensity = liveDensity;
        }

        public BoardSettings() : this(0, 0, 0, 0) { }
    }

    public class FileManager
    {
        private static T LoadJson<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static BoardSettings LoadBoardSettings(string filePath)
            => LoadJson<BoardSettings>(filePath);

        public static void SaveToJson<T>(T data, string filePath, bool writeIndented = false)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = writeIndented });
            File.WriteAllText(filePath, json);
        }

        public static void SaveBoardSettings(BoardSettings settings, string filePath)
            => SaveToJson(settings, filePath);

        public static void SaveBoardState(Board board, string filePath)
        {
            StringBuilder boardState = new StringBuilder();
            for (var row = 0; row < board.Height; row++)
            {
                for (var col = 0; col < board.Width; col++)
                {
                    boardState.Append(
                       board.Cells[row, col].IsAlive
                       ? '*'
                       : ' ');
                }
                boardState.Append('\n');
            }
            boardState.AppendLine($"cellSize={board.CellSize}")
                       .AppendLine($"generation={board.Generation}");

            File.WriteAllText(filePath, boardState.ToString());
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

            var settings = new BoardSettings(width, height, cellSize);

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
            => LoadJson<Figure[]>(filePath);

        public static void SaveFigures(Figure[] figures, string filePath)
            => SaveToJson(figures, filePath, true);

        public static void SavePlotPng(Plot plot, string filePath, int width = 1920, int heigth = 1080)
            => plot.SavePng(filePath, 1920, 1080);
    }
    public class BoardAnalysis
    {
        public static Figure[] Figures;

        private Board _board;

        public BoardAnalysis(Board board)
        {
            _board = board;
        }

        public int FindAliveCellsCount()
        {
            return FindAliveCellsCount(_board);
        }

        public static int FindAliveCellsCount(Board board)
        {
            var result = 0;

            foreach (var cell in board.Cells)
            {
                if (cell.IsAlive)
                {
                    result++;
                }
            }

            return result;
        }

        private Figure CreateFigure(int row, int col, int width, int height)
        {
            string figureString = GenerateFigureString(row, col, width, height);

            return new Figure("figure", width, height, figureString);
        }

        private string GenerateFigureString(int row, int col, int width, int height)
        {
            return string.Join("",
                from rowIndex in Enumerable.Range(row, height)
                from colIndex in Enumerable.Range(col, width)
                select GetFigureCell(rowIndex, colIndex));
        }

        private char GetFigureCell(int rowIndex, int colIndex)
        {
            return IsInsideBoard(rowIndex, colIndex)
                ? (_board.Cells[rowIndex, colIndex].IsAlive ? '*' : ' ')
                : ' ';
        }

        private bool IsInsideBoard(int rowIndex, int colIndex)
        {
            return rowIndex >= 0 && rowIndex < _board.Rows
                && colIndex >= 0 && colIndex < _board.Columns;
        }

        public Dictionary<string, int> GetFiguresCount()
        {
            Dictionary<string, int> figuresCount = InstantiateFiguresDictionary();

            for (int row = 0; row < _board.Rows; row++)
            {
                for (int col = 0; col < _board.Columns; col++)
                {
                    UpdateFiguresCount(figuresCount, row, col);
                }
            }

            return figuresCount;
        }

        private Dictionary<string, int> InstantiateFiguresDictionary()
        {
            return Figures.ToDictionary(figure => figure.Name, _ => 0);
        }

        private void UpdateFiguresCount(Dictionary<string, int> figuresCount, int row, int col)
        {
            var figureToIncrement = Figures
                .FirstOrDefault(figure => figure.Equals(CreateFigure(row, col, figure.Width, figure.Height)));

            if (figureToIncrement != null)
            {
                figuresCount[figureToIncrement.Name]++;
            }
        }
    }

    class Program
    {

        const string SettingsJsonPath = "../../../boardSettings.json";

        const string BoardStateFilePath = "../../../resultGameOfLife.txt";

        const string FiguresJsonPath = "../../../figures.json";

        const string PlotPngPath = "../../../plot.png";

        static int IterationsNum = 50;

        static bool LoadFromFile = false;

        static bool SaveToFile = false;

        static Board SimulationBoard;
        static BoardAnalysis BoardAnalysis;
        static BoardSettings Settings = new BoardSettings(50, 20, 1, 0.5);



        static void ConfigureSimulationStart()
        {
            Console.Write("The number of iterations of the simulation: ");
            IterationsNum = ParseNumericInput();

            Console.Write("Download from a file? (1 - yes, 0 - no): ");
            LoadFromFile = ParseYesNoInput();

            Console.Write("Save the result to a file? (1 - yes, 0 - no): ");
            SaveToFile = ParseYesNoInput();
        }

        static int ParseNumericInput()
        {
            int.TryParse(Console.ReadLine(), out int userResponse);
            return userResponse;
        }

        static bool ParseYesNoInput()
        {
            var userResponse = ParseNumericInput();
            return userResponse != 0;
        }

        static void RegularSimulation()
        {
            ConfigureSimulationStart();
            Reset();

            BoardAnalysis.Figures = FileManager.LoadFigures(FiguresJsonPath);

            LoadBoardState();

            RunGameLoop();

            SaveBoardState();
        }

        private static void LoadBoardState()
        {
            if (!LoadFromFile) return;

            try
            {
                SimulationBoard = FileManager.LoadBoardState(BoardStateFilePath);
                BoardAnalysis = new(SimulationBoard);
                Console.WriteLine("Successfully reading data from a file.");
            }
            catch
            {
                Console.WriteLine("Error when reading data from a file.");
                Reset();
            }

            Thread.Sleep(2000);
        }

        private static void RunGameLoop()
        {
            for (var index = 0; index < IterationsNum; index++)
            {
                Console.Clear();
                Render();
                DisplayGameStats();
                SimulationBoard.Advance();
                Thread.Sleep(1000);
            }
        }

        private static void DisplayGameStats()
        {
            Console.WriteLine($"Current generation: {SimulationBoard.Generation}");
            Console.WriteLine($"The number of living cells: {BoardAnalysis.FindAliveCellsCount()}");
            Console.WriteLine("The number of each of the figures:");
            foreach (var figure in BoardAnalysis.GetFiguresCount())
            {
                Console.WriteLine($"{figure.Key}: {figure.Value}");
            }
        }

        private static void SaveBoardState()
        {
            if (!SaveToFile) return;

            try
            {
                FileManager.SaveBoardState(SimulationBoard, BoardStateFilePath);
                Console.WriteLine("Successfully saving the result to a file.");
            }
            catch
            {
                Console.WriteLine("Error when saving the result to a file.");
            }
        }

        static void ResearchSimulation()
        {
            var plot = InitializePlot();

            double[] density = GenerateDensityArray();

            var generations = GenerateGenerationsList();
            var settings = new BoardSettings(Settings.Width, Settings.Height, Settings.CellSize);

            for (int boardIndex = 0; boardIndex < density.Length; boardIndex++)
            {
                Board board = CreateNewBoard(density, boardIndex, settings);
                BoardAnalysis boardAnalysis = new BoardAnalysis(board);

                List<int> aliveCells = CalculateAliveCells(board, boardAnalysis);

                AddScatterToPlot(plot, generations, aliveCells, density, boardIndex);
            }

            FileManager.SavePlotPng(plot, PlotPngPath);
        }

        private static Plot InitializePlot()
        {
            var plot = new Plot();
            plot.XLabel("Generation");
            plot.YLabel("Living cells");
            plot.ShowLegend();
            return plot;
        }

        private static double[] GenerateDensityArray() => new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };

        private static List<int> GenerateGenerationsList() => Enumerable.Repeat(0, IterationsNum).Select((index, generation) => generation + index).ToList();

        private static Board CreateNewBoard(double[] density, int boardIndex, BoardSettings settings)
        {
            settings.LiveDensity = density[boardIndex];
            return new Board(settings);
        }

        private static List<int> CalculateAliveCells(Board board, BoardAnalysis boardAnalysis)
        {
            List<int> aliveCells = new();
            int generation = 0;

            while (generation < IterationsNum)
            {
                aliveCells.Add(boardAnalysis.FindAliveCellsCount());
                board.Advance();
                generation++;
            }

            return aliveCells;
        }

        private static void AddScatterToPlot(Plot plot, List<int> generations, List<int> aliveCells, double[] density, int boardIndex)
        {
            var rand = new Random();
            Scatter scatter = plot.Add.Scatter(generations, aliveCells);
            scatter.LegendText = density[boardIndex].ToString();
            scatter.LineWidth = 3;
            scatter.Color = new(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
        }

        static void GetSimulationType()
        {
            Console.Write("To run: \n1. Simulation\n2. Plotting\nChoice: ");
            int userChoice = ParseNumericInput();

            switch (userChoice)
            {
                case 1:
                    RegularSimulation();
                    break;
                case 2:
                    ResearchSimulation();
                    break;
                default:
                    Console.WriteLine("A non-existent option is selected");
                    GetSimulationType(); // повторный запрос типа симуляции у пользователя при некорректном вводе
                    break;
            }
        }
        static private void Reset()
        {
            try
            {
                var settings = FileManager.LoadBoardSettings(SettingsJsonPath);

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

        static void Main(string[] args)
        {
            GetSimulationType();
        }
    }
}