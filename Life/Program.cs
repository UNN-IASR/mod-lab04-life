using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        public bool IsAliveNext;

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

    public class Pattern
    {
        public string Name { get; }
        public string Classification { get; }
        public bool[,] Cells { get; }

        public Pattern(string name, string classification, bool[,] cells)
        {
            Name = name;
            Classification = classification;
            Cells = cells;
        }

        public static List<Pattern> GetStandardPatterns()
        {
            return new List<Pattern>
            {
                new Pattern("Block", "Still Life", new bool[,] { { true, true }, { true, true } }),
                new Pattern("Beehive", "Still Life", new bool[,] { { false, true, true, false }, { true, false, false, true }, { false, true, true, false } }),
                new Pattern("Loaf", "Still Life", new bool[,] { { false, true, true, false }, { true, false, false, true }, { false, true, false, true }, { false, false, true, false } }),
                new Pattern("Boat", "Still Life", new bool[,] { { true, true, false }, { true, false, true }, { false, true, false } }),
                new Pattern("Blinker", "Oscillator", new bool[,] { { true, true, true } }),
                new Pattern("Toad", "Oscillator", new bool[,] { { false, true, true, true }, { true, true, true, false } }),
                new Pattern("Beacon", "Oscillator", new bool[,] { { true, true, false, false }, { true, true, false, false }, { false, false, true, true }, { false, false, true, true } }),
                new Pattern("Glider", "Spaceship", new bool[,] { { false, true, false }, { false, false, true }, { true, true, true } }),
                new Pattern("Lightweight Spaceship", "Spaceship", new bool[,] { { true, false, false, true, false }, { false, false, false, false, true }, { true, false, false, false, true }, { false, true, true, true, true } })
            };
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

        private readonly List<string> stateHistory = new List<string>();

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
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
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
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

        public void SaveState(string filePath)
        {
            var state = Cells.Cast<Cell>().Select(cell => cell.IsAlive).ToArray();
            File.WriteAllText(filePath, JsonSerializer.Serialize(state));
        }

        public void LoadState(string filePath)
        {
            var state = JsonSerializer.Deserialize<bool[]>(File.ReadAllText(filePath));
            for (int i = 0; i < state.Length; i++)
            {
                Cells[i % Columns, i / Columns].IsAlive = state[i];
            }
        }

        public void LoadPattern(string patternFilePath)
        {
            var lines = File.ReadAllLines(patternFilePath);
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    Cells[x, y].IsAlive = lines[y][x] == '1';
                }
            }
        }

        public int CountAliveCells()
        {
            return Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        }

        public Dictionary<string, int> ClassifyElements()
        {
            var patterns = Pattern.GetStandardPatterns();
            var classifications = new Dictionary<string, int>();

            foreach (var pattern in patterns)
            {
                if (!classifications.ContainsKey(pattern.Classification))
                {
                    classifications[pattern.Classification] = 0;
                }
            }

            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    var classification = ClassifyElement(x, y, patterns);
                    if (classification != "Unknown")
                    {
                        classifications[classification]++;
                    }
                }
            }

            return classifications;
        }

        private string ClassifyElement(int x, int y, List<Pattern> patterns)
        {
            foreach (var pattern in patterns)
            {
                if (MatchPattern(x, y, pattern))
                {
                    return pattern.Classification;
                }
            }
            return "Unknown";
        }

        private bool MatchPattern(int x, int y, Pattern pattern)
        {
            for (int i = 0; i < pattern.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < pattern.Cells.GetLength(1); j++)
                {
                    int boardX = (x + i) % Columns;
                    int boardY = (y + j) % Rows;
                    if (Cells[boardX, boardY].IsAlive != pattern.Cells[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public int CalculateAverageStableTime(int maxGenerations)
        {
            int totalGenerations = 0;
            int stableCount = 0;

            for (int i = 0; i < maxGenerations; i++)
            {
                bool isStable = true;
                foreach (var cell in Cells)
                {
                    cell.DetermineNextLiveState();
                }
                foreach (var cell in Cells)
                {
                    if (cell.IsAlive != cell.IsAliveNext)
                    {
                        isStable = false;
                        break;
                    }
                }
                if (isStable)
                {
                    stableCount++;
                    totalGenerations += i;
                }
                foreach (var cell in Cells)
                {
                    cell.Advance();
                }
            }

            return stableCount > 0 ? totalGenerations / stableCount : maxGenerations;
        }

        public void PlotStablePhaseTransition(string filePath, int maxGenerations)
        {
            var densities = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5 };
            var results = new List<int>();

            foreach (var density in densities)
            {
                Randomize(density);
                int averageStableTime = CalculateAverageStableTime(maxGenerations);
                results.Add(averageStableTime);
            }
s
            for (int i = 0; i < densities.Length; i++)
            {
                Console.WriteLine($"Density: {densities[i]}, Average Stable Time: {results[i]}");
            }
        }

        private string GetCurrentState()
        {
            return string.Join(",", Cells.Cast<Cell>().Select(cell => cell.IsAlive ? "1" : "0"));
        }

        public bool IsStable()
        {
            var currentState = GetCurrentState();
            if (stateHistory.Contains(currentState))
            {
                return true;
            }
            stateHistory.Add(currentState);
            if (stateHistory.Count > 10) stateHistory.RemoveAt(0); // Keep history limited to the last 10 states to detect loops
            return false;
        }
    }

    public class Config
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }

        public static Config Load(string filePath)
        {
            return JsonSerializer.Deserialize<Config>(File.ReadAllText(filePath));
        }
    }

    class Program
    {
        static Board board;
        static Config config;
        static int generation = 0;
        static Dictionary<string, int> initialClassifications;
        static double initialDensity;

        static void Reset()
        {
            board = new Board(config.Width, config.Height, config.CellSize, config.LiveDensity);
            initialDensity = config.LiveDensity;
            initialClassifications = board.ClassifyElements();
        }

        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    Console.Write(cell.IsAlive ? '*' : ' ');
                }
                Console.Write('\n');
            }
            Console.WriteLine($"Generation: {generation}");
            Console.WriteLine($"Initial Density: {initialDensity}");
            Console.WriteLine("Initial Classifications:");
            foreach (var classification in initialClassifications)
            {
                Console.WriteLine($"{classification.Key}: {classification.Value}");
            }
            var currentClassifications = board.ClassifyElements();
            Console.WriteLine($"Current Density: {(double)board.CountAliveCells() / (board.Columns * board.Rows)}");
            Console.WriteLine("Current Classifications:");
            foreach (var classification in currentClassifications)
            {
                Console.WriteLine($"{classification.Key}: {classification.Value}");
            }
        }

        static void Main(string[] args)
        {
            if (args.Length > 0 && File.Exists(args[0]))
            {
                config = Config.Load(args[0]);
            }
            else
            {
                config = new Config { Width = 50, Height = 20, CellSize = 1, LiveDensity = 0.5 };
            }

            Reset();

            if (args.Length > 1 && File.Exists(args[1]))
            {
                board.LoadPattern(args[1]);
            }

            if (args.Length > 2 && File.Exists(args[2]))
            {
                board.LoadState(args[2]);
            }

            while (true)
            {
                Console.Clear();
                Render();
                if (board.IsStable())
                {
                    Console.WriteLine("System has reached a stable state. Simulation ending.");
                    break;
                }
                board.Advance();
                generation++;
                Thread.Sleep(1000);
            }
        }
    }
}
