using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;
using ScottPlot.AxisPanels;

namespace cli_life
{
    public class StaticFigure
    {
        public string Name { get; set; }
        public string[] Figure { get; set; }

        public static StaticFigure CreateFigure(Cell[,] cells, int startRow, int startCol, int width, int height)
        {
            string[] result = new string[height];
            StringBuilder stringBuilder;

            int currentFigureRow = 0;

            for (int row = startRow; row < startRow + height; row++)
            {
                stringBuilder = new StringBuilder();

                for (int col = startCol; col < startCol + width; col++)
                {
                    if (col < 0 || col >= cells.GetLength(0)
                        || row < 0 || row >= cells.GetLength(1))
                    {
                        stringBuilder.Append(' ');
                        continue;
                    }

                    if (cells[col, row].IsAlive)
                    {
                        stringBuilder.Append('*');
                        continue;
                    }

                    stringBuilder.Append(' ');
                }

                result[currentFigureRow++] = stringBuilder.ToString();
            }

            return new StaticFigure
            {
                Name = "",
                Figure = result
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is not StaticFigure)
            {
                return false;
            }

            return Figure.SequenceEqual((obj as StaticFigure).Figure);
        }
    }

    public class BoardSettings()
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }

        public void SaveInFile(string path)
        {
            var json = JsonSerializer.Serialize(this);
            File.WriteAllText(path, json);
        }

        public void LoadFromFile(string path)
        {
            BoardSettings settings = JsonSerializer.Deserialize<BoardSettings>(File.ReadAllText(path));

            Width = settings.Width;
            Height = settings.Height;
            CellSize = settings.CellSize;
            LiveDensity = settings.LiveDensity;
        }
    }

    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public int PreviousAliveCells { get; private set; }
        public int AliveCells { get { return GetAliveCellsNumber(); } }
        public int StableStateGeneration { get; private set; }
        public int Generation { get; private set; }

        public Board(string path)
        {
            PreviousAliveCells = -1;
            StableStateGeneration = -1;
            Generation = 0;

            string[] fileStrings = File.ReadAllLines(path);
            Cells = new Cell[fileStrings[0].Length, fileStrings.Length];

            CellSize = 1;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Cells[col, row] = new Cell();

                    if (fileStrings[row].ElementAt(col) == '*')
                    {
                        Cells[col, row].IsAlive = true;
                    }
                }
            }

            ConnectNeighbors();
        }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            PreviousAliveCells = -1;
            StableStateGeneration = -1;
            Generation = 0;

            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            PreviousAliveCells = AliveCells;

            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();

            Generation++;

            if (PreviousAliveCells == AliveCells)
            {
                if (StableStateGeneration == -1)
                {
                    StableStateGeneration = Generation - 1;
                }

                return;
            }

            StableStateGeneration = -1;
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }

        public int GetAliveCellsNumber()
        {
            int result = 0;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Cells[col, row].IsAlive)
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public Dictionary<string, int> GetFiguresNumber(StaticFigure[] figures)
        {
            if (figures.Length == 0)
            {
                return [];
            }

            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (StaticFigure figure in figures)
            {
                result.Add(figure.Name, 0);
            }

            for (int row = -1; row <= Rows; row++)
            {
                for (int col = -1; col <= Columns; col++)
                {
                    foreach (StaticFigure figure in figures)
                    {
                        if (figure.Equals(StaticFigure.CreateFigure(
                            Cells, row, col, figure.Figure[0].Length, figure.Figure.GetLength(0))))
                        {
                            result[figure.Name]++;
                        }
                    }
                }
            }

            return result;
        }

        public void SaveInFile(string path)
        {
            StringBuilder fileString = new StringBuilder();

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Cells[col, row].IsAlive)
                    {
                        fileString.Append('*');
                        continue;
                    }

                    fileString.Append(' ');
                }

                fileString.Append('\n');
            }

            File.WriteAllText(path, fileString.ToString());
        }
    }
    class Program
    {
        static Board board;
        static StaticFigure[] figures;

        static private void Reset()
        {
            if (File.Exists("../../../figures.json"))
            {
                figures = JsonSerializer.Deserialize<StaticFigure[]>(File.ReadAllText("../../../figures.json"));
            }
            else
            {
                figures = [];
            }

            BoardSettings settings = new();
            settings.LoadFromFile("../../../settings.json");

            if (File.Exists("../../../board.txt"))
            {
                board = new Board("../../../board.txt");
                return;
            }

            board = new Board(
                width: settings.Width,
                height: settings.Height,
                cellSize: settings.CellSize,
                liveDensity: settings.LiveDensity);
        }
        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
        static void ShowInfo()
        {
            int elems = board.AliveCells;
            Dictionary<string, int> figureCount = board.GetFiguresNumber(figures);
            int stableState = board.StableStateGeneration;

            foreach (var figure in figureCount)
            {
                elems += figure.Value;
            }

            Console.WriteLine("Количество элементов: " + elems);
            Console.WriteLine("Количество фигур:");
            foreach (var figure in figureCount)
            {
                Console.WriteLine(figure.Key + " - " + figure.Value);
            }
            Console.WriteLine("Поколение перехода в стабильное состояние: " + stableState);
        }
        static private void SetUpBoardForAnalysis(double liveDensity = .1)
        {
            figures = [];

            BoardSettings settings = new();
            settings.LoadFromFile("../../../settings.json");
            settings.LiveDensity = liveDensity;

            board = new Board(
                width: settings.Width,
                height: settings.Height,
                cellSize: settings.CellSize,
                liveDensity: settings.LiveDensity);
        }
        static void UpdateConsole()
        {
            Console.Clear();
            Render();
            ShowInfo();
        }
        static void RunGameOfLife()
        {
            Reset();

            for (int iteration = 0; iteration < 100; iteration++)
            {
                UpdateConsole();
                board.Advance();
                Thread.Sleep(1000);
            }

            UpdateConsole();

            board.SaveInFile("../../../board.txt");
        }
        static void CreateGraph(int iterations = 1000)
        {
            Random rnd = new Random();

            ScottPlot.Plot plot = new ScottPlot.Plot();
            plot.XLabel("Поколение");
            plot.YLabel("Число живых клеток");
            plot.ShowLegend();

            double[] densities = new double[]
            {
                0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9
            };

            List<int> generations = Enumerable.Repeat(0, iterations).Select((i, e) => e + i).ToList();
            List<int> aliveCells;

            for (int boardIndex = 0; boardIndex < densities.Length; boardIndex++)
            {
                aliveCells = new List<int>();
                SetUpBoardForAnalysis(densities[boardIndex]);

                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    aliveCells.Add(board.AliveCells);
                    board.Advance();
                }

                var scat = plot.Add.Scatter(generations, aliveCells,
                    new ScottPlot.Color(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256)));
                scat.LineWidth = 2;
                scat.MarkerSize = 2;
                scat.LegendText = "Плотность жизни " + densities[boardIndex].ToString();
            }

            plot.SavePng("../../../plot.png", 1920, 1080);
        }
        static void GetLauchSimulationType()
        {
            Console.WriteLine("Выберите вариант запуска:\n1.Запуск игры \"Жизнь\"\n2.Построение графика");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    RunGameOfLife();
                    break;
                case "2":
                    CreateGraph();
                    break;
                default:
                    Console.WriteLine("Несуществующий вариант");
                    break;
            }
        }
        static void Main(string[] args)
        {
            GetLauchSimulationType();
        }
    }
}