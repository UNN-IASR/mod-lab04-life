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
    public sealed class Shape(string label, int width, int height, string stringRepresentation)
    {
        public string Label { get; private set; } = label;
        public int Width { get; private set; } = width;
        public int Height { get; private set; } = height;
        public string StringRepresentation { get; private set; } = stringRepresentation;

        public override bool Equals(object obj) => obj is Shape shape &&
                   Width == shape.Width &&
                   Height == shape.Height &&
                   StringRepresentation == shape.StringRepresentation;
        public override int GetHashCode() => Width.GetHashCode() & Height.GetHashCode() & Label.GetHashCode() & StringRepresentation.GetHashCode();
    }
    public sealed record BoardSettings(int Width, int Height, int CellSize, double LiveDensity = 0.5)
    {
        public static BoardSettings Load(string filePath) => JsonSerializer.Deserialize<BoardSettings>(File.ReadAllText(filePath));

        public void Save(string filePath) => File.WriteAllText(filePath, JsonSerializer.Serialize(this));
    }
    public static class IOmanager
    {
        public static Shape[] LoadShapes(string filePath) => JsonSerializer.Deserialize<Shape[]>(File.ReadAllText(filePath));

        public static void SaveShapes(Shape[] shapes, string filePath) => File.WriteAllText(filePath, JsonSerializer.Serialize(shapes, new JsonSerializerOptions { WriteIndented = true }));

        public static void SavePlotPng(Plot plot, string filePath, int width = 1920, int height = 1080) => plot.SavePng(filePath, width, height);
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
        public void Save(string filePath)
        {
            var state = new StringBuilder();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    char cellIndicator = Cells[i, j].IsAlive ? '*' : ' ';
                    state.Append(cellIndicator);
                }
                state.AppendLine();
            }

            state.AppendLine($"cellSize={CellSize}");
            state.AppendLine($"generation={Generation}");

            string finalState = state.ToString();
            File.WriteAllText(filePath, finalState);
        }
        public static Board Load(string filePath)
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
    }
    public class BoardAnalytics
    {
        private static Shape[] _shapes;
        private readonly Board _board;

        public BoardAnalytics(Board board)
        {
            _board = board;
            _shapes ??= IOmanager.LoadShapes("../../../shapes.json");
        }

        public int GetAliveCellsCount() => GetAliveCellsCount(_board);
        private Shape CreateShape(int row, int col, int width, int height)
        {
            var shapeString = new StringBuilder();
            for (var rowIndex = row; rowIndex < row + height; rowIndex++)
            {
                for (var columnIndex = col; columnIndex < col + width; columnIndex++)
                {
                    shapeString.Append((rowIndex < 0 ||
                        rowIndex >= _board.Rows ||
                        columnIndex < 0 ||
                        columnIndex >= _board.Columns)
                    ? ' '
                    : (_board.Cells[rowIndex, columnIndex].IsAlive ? '*' : ' '));
                }
            }
            return new Shape("shape", width, height, shapeString.ToString());
        }
        public Dictionary<string, int> GetShapesCount()
        {
            var result = _shapes.ToDictionary(shape => shape.Label, shape => 0);

            for (var row = -1; row < _board.Rows; row++)
            {
                for (var col = -1; col < _board.Columns; col++)
                {
                    foreach (var shape in _shapes)
                    {
                        if (shape.Equals(CreateShape(row, col, shape.Width, shape.Height)))
                            result[shape.Label]++;
                    }
                }
            }
            return result;
        }
        public static int GetAliveCellsCount(Board board)
        {   
            return board.Cells.Cast<Cell>().Where(cell => cell != null && cell.IsAlive).Count();
        }


    }
    class Program
    {
        const string PathBoardSettings = "../../../boardSettings.json";
        const string PathBoardState = "../../../SaveDataGame.txt";
        const string PathPlotPath = "../../../plot.png";
        static int IterationsCount = 150;
        static bool LoadFromFile = false;
        static bool SaveToFile = false;
        public static Board LiveBoard;
        public static BoardAnalytics BoardAnalytics;
        public static BoardSettings BoardSettings = new BoardSettings(50, 20, 1, 0.5);



        static void View()
        {
            for (var row = 0; row < LiveBoard.Rows; row++)
            {
                for (var col = 0; col < LiveBoard.Columns; col++)
                {
                    Console.Write(LiveBoard.Cells[row, col].IsAlive ? '*' : ' ');
                }
                Console.WriteLine();
            }
        }

        static void Setup()
        {
            Console.Write("Count of itterations:\n>>");
            IterationsCount = int.Parse(Console.ReadLine());

            Console.Write("Load from file (1 - true, 0 - false):\n>>");
            LoadFromFile = Console.ReadLine() == "1";

            Console.Write("Save to file (1 - true, 0 - false):\n>>");
            SaveToFile = Console.ReadLine() == "1";
        }
        static void SimpleRun()
        {
            Setup();
            Reset();

            if (LoadFromFile)
            {
                try
                {
                    LiveBoard = Board.Load(PathBoardState);
                    BoardAnalytics = new(LiveBoard);
                    Console.WriteLine("Complete load!");
                }
                catch
                {
                    Console.WriteLine("Error loading!");
                    Reset();
                }
                Thread.Sleep(2000);
            }

            for (var index = 0; index < IterationsCount; index++)
            {
                Console.Clear();
                View();
                Console.WriteLine($"Current generation: {LiveBoard.Generation + 1}");
                Console.WriteLine($"Count of alive cells: {BoardAnalytics.GetAliveCellsCount()}");
                Console.WriteLine("Shape counts:");
                foreach (var shape in BoardAnalytics.GetShapesCount())
                {
                    Console.WriteLine($"{shape.Key}: {shape.Value}");
                }
                LiveBoard.Advance();
                Thread.Sleep(1000);
            }

            if (SaveToFile)
            {
                try
                {
                    LiveBoard.Save(PathBoardState);
                    Console.WriteLine("Comlete save!");
                }
                catch
                {
                    Console.WriteLine("Error saving boarstate!");
                }

            }
        }

        static void AnalysisRun()
        {
            var rand = new Random();

            var plot = new Plot();
            plot.XLabel("Generations");
            plot.YLabel("Allive cells");
            plot.ShowLegend();

            var density = new double[]
            {
                0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9
            };

            var generations = Enumerable
                .Repeat(0, IterationsCount)
                .Select((index, generation) => generation + index)
                .ToList();
            List<int> aliveCells;

            int generation;
            Board board;
            BoardAnalytics boardAnalytics;
            var boardsCount = density.Count();
            var baseSettings = new BoardSettings(BoardSettings.Width, BoardSettings.Height, BoardSettings.CellSize);

            Scatter scatter;

            for (int boardIndex = 0; boardIndex < boardsCount; boardIndex++)
            {
                aliveCells = new();
                generation = 0;

                var settings = baseSettings with { LiveDensity = density[boardIndex] };
                board = new(density[boardIndex]);
                boardAnalytics = new BoardAnalytics(board);

                while (generation < IterationsCount)
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

            IOmanager.SavePlotPng(plot, PathPlotPath);
        }

        static private void Reset()
        {
            try
            {
                LiveBoard = new Board(BoardSettings.Load(PathBoardSettings));
            }
            catch
            {
                LiveBoard = new Board(BoardSettings);
            }
            BoardAnalytics = new(LiveBoard);
        }
     
        static void Main()
        {
            Console.Write("View: \n1. Simple view\n2. Analys view\n>>");
            _ = int.TryParse(Console.ReadLine(), out int userChoice);

            switch (userChoice)
            {
                case 1:
                    SimpleRun();
                    break;
                case 2:
                    AnalysisRun();
                    break;
                default:
                    throw new ArgumentException("You must input 1 or 2 number!", nameof(userChoice));
            }
        }
    }
}