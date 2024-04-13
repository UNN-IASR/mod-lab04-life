using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Collections.Immutable;
using ScottPlot;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> Neighbors = new List<Cell>();

        private bool isAliveNext;

        public void DetermineNextLiveState()
        {
            int liveNeighbors = Neighbors.Where(x => x.IsAlive).Count();

            if (IsAlive)
                isAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                isAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = isAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns => Cells.GetLength(0);
        public int Rows => Cells.GetLength(1);
        public int Width => Columns * CellSize;
        public int Height => Rows * CellSize;
        public int Generation { get; private set; }


        private const int cacheCapacity = 1000;
        private Queue<string> previousStates;
        public IReadOnlyList<string> PreviousStates => previousStates.ToList();

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            if (width <= 0 || height <= 0 || cellSize <= 0 || liveDensity <= 0)
            {
                throw new ArgumentException();
            }

            Generation = 1;

            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);

            previousStates = new Queue<string>(cacheCapacity);
            previousStates.Enqueue(CellsExtensions.ToString(Cells));
        }

        public Board(Cell[][] cells, int cellSize, int generation)
        {
            if (cells.Length <= 0 || cells[0].Length <= 0)
            {
                throw new ArgumentException();
            }

            Generation = generation;

            CellSize = cellSize;
            Cells = new Cell[cells[0].Length, cells.Length];
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell()
                    {
                        IsAlive = cells[y][x].IsAlive
                    };
                }
            }

            ConnectNeighbors();

            previousStates = new Queue<string>(cacheCapacity);
            previousStates.Enqueue(CellsExtensions.ToString(Cells));
        }

        public Board(Board source)
        {
            Generation = source.Generation;
            CellSize = source.CellSize;

            Cells = source.Cells.CloneWithoutNeighbors();
            ConnectNeighbors();
            previousStates = new Queue<string>(source.PreviousStates);
        }

        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = Random.Shared.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();

            Generation++;

            var state = CellsExtensions.ToString(Cells);

            previousStates.Enqueue(state);
            if (previousStates.Count == cacheCapacity)
            {
                previousStates.Dequeue();
            }
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

                    Cells[x, y].Neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].Neighbors.Add(Cells[x, yT]);
                    Cells[x, y].Neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].Neighbors.Add(Cells[xL, y]);
                    Cells[x, y].Neighbors.Add(Cells[xR, y]);
                    Cells[x, y].Neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].Neighbors.Add(Cells[x, yB]);
                    Cells[x, y].Neighbors.Add(Cells[xR, yB]);
                }
            }
        }
    }

    public static class BoardAnalyzer
    {
        public static async Task<ConcurrentDictionary<Pattern, int>> CountPatternsAsync(this Board src, IEnumerable<Pattern> patterns, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var tasks = new List<Task>();
            var result = new ConcurrentDictionary<Pattern, int>();

            foreach (var pattern in patterns)
            {
                tasks.Add(Task.Run(() => result[pattern] = CountPattern(src, pattern), cancellationToken));
            }

            await Task.WhenAll(tasks);
            return result;
        }

        private static int CountPattern(Board board, Pattern pattern)
        {
            var cells = board.Cells.CloneWithoutNeighbors();
            int count = 0;

            for (int y = 0; y < board.Rows; y++)
            {
                for (int x = 0; x < board.Columns; x++)
                {
                    string str = "";
                    for (int i = 0; i < pattern.Height && i < board.Rows; i++)
                    {
                        for (int j = 0; j < pattern.Width && j < board.Columns; j++)
                        {
                            int xCoord = x + j < board.Columns ? x + j : (x + j) - board.Columns;
                            int yCoord = y + i < board.Rows ? y + i : (i + y) - board.Rows;
                            str += cells[xCoord, yCoord].IsAlive ? '*' : '.';
                        }
                    }

                    if (str == pattern.Value)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static bool IsStable(this Board src)
        {
            var state = CellsExtensions.ToString(src.Cells);

            return src.PreviousStates.SkipLast(1).Contains(state);
        }

        public static int CountGenerationsToStableState(this Board src)
        {
            var copy = new Board(src);
            const int maxGenerations = 10000;
            
            if(copy.IsStable())
            {
                return copy.Generation;
            }

            for(int i = 0; i < maxGenerations; i++)
            {
                copy.Advance();
                
                if(copy.IsStable())
                {
                    return copy.Generation;
                }
            }
            
            var exception = new OverflowException("Too many generations");
            exception.Data.Add("MaxGenerations", maxGenerations);
            throw exception;
        }

        public static int CountAliveCells(this Board src)
        {
            int aliveCells = 0;
            for (int x = 0; x < src.Rows; x++)
            {
                for (int y = 0; y < src.Columns; y++)
                {
                    if (src.Cells[x, y].IsAlive)
                    {
                        aliveCells++;
                    }
                }
            }

            return aliveCells;
        }
    }

    public static class CellsExtensions
    {
        public static string ToString(this Cell[,] src)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int x = 0; x < src.GetLength(0); x++)
            {
                for (int y = 0; y < src.GetLength(1); y++)
                {
                    stringBuilder.Append(src[x, y].IsAlive ? '*' : '.');
                }
                //stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        public static Cell[,] CloneWithoutNeighbors(this Cell[,] src)
        {
            int Columns = src.GetLength(0);
            int Rows = src.GetLength(1);
            var cells = new Cell[Columns, Rows];
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    cells[x, y] = new Cell() { IsAlive = src[x, y].IsAlive };
                }
            }

            return cells;
        }
    }

    public class BoardSettings
    {
        public int CellSize { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public double LiveDensity { get; init; }
    }

    public record class Pattern
    {
        public required string Name { get; init; }
        public required string Value { get; init; }
        public required int Width { get; init; }
        public required int Height { get; init; }
    }

    public static class Helpers
    {
        public static BoardSettings LoadBoardSettings(string fileName)
        {
            var boardSettings = new BoardSettings();

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                boardSettings = JsonSerializer.Deserialize<BoardSettings>(fs);
            }

            return boardSettings;
        }

        public static Pattern[] LoadPatterns(string fileName)
        {
            var patterns = new List<Pattern>();

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                patterns = JsonSerializer.Deserialize<List<Pattern>>(fs);
            }

            return patterns.ToArray();
        }

        public static Board LoadBoardFromFile(string fileName)
        {
            var board = new List<Cell[]>();
            int cellSize = 1;
            int generation = 1;

            foreach (var line in File.ReadLines(fileName))
            {
                if (line.StartsWith("CellSize:"))
                {
                    cellSize = int.Parse(line.Substring("CellSize:".Length));
                }
                else if (line.StartsWith("Generation:"))
                {
                    generation = int.Parse(line.Substring("Generation:".Length));
                }
                else
                {
                    var cells = new List<Cell>(line.Length);
                    foreach (var cell in line)
                    {
                        cells.Add(new Cell() { IsAlive = cell == '*' });
                    }
                    board.Add(cells.ToArray());
                }
            }

            return new Board(board.ToArray(), cellSize, generation);
        }

        public static void SaveBoardToFile(Board board, string fileName)
        {
            using (var sw = new StreamWriter(fileName, false))
            {
                sw.WriteLine($"CellSize:{board.CellSize}");
                sw.WriteLine($"Generation:{board.Generation}");
                for (int x = 0; x < board.Columns; x++)
                {
                    string line = "";
                    for (int y = 0; y < board.Rows; y++)
                    {
                        line += board.Cells[x, y].IsAlive ? '*' : '.';
                    }

                    sw.WriteLine(line);
                }
            }
            Console.WriteLine("File saved");
        }
    }


    class Program
    {
        static Board board;
        static object boardLocker = new object();

        static async Task Main(string[] args)
        {
            //CreateGraph(new double[]
            //{
            //    0.1,
            //    0.4,
            //    0.7
            //}, 50, 50);


            //CancellationTokenSource cts = new CancellationTokenSource();
            //Reset();
            //board = LoadBoardFromFile("somePatterns.txt");
            //Render();
            //var patterns = LoadPatterns("patterns.json");
            //var result = (await board.CountPatternsAsync(patterns, cts.Token)).OrderBy(a => a.Key.Name);
            //foreach (var pc in result)
            //{
            //    Console.WriteLine($"{pc.Key.Name}: {pc.Value}");
            //}

            try
            {
                var cts = new CancellationTokenSource();

                Reset();

                var consoleInput = Task.Run(() => HandleConsoleInput(cts));
                var simulation = Task.Run(() => Simulate(cts.Token));

                await Task.WhenAll(consoleInput, simulation);
            }
            catch (OperationCanceledException ex)
            {

            }
        }

        static void CreateGraph(double[] densities, int width, int height)
        {
            var plot = new Plot();
            plot.XLabel("Generation");
            plot.YLabel("Alive cells");
            plot.ShowLegend();

            const int maxGenerations = 300;
            var dataPerDensity = GetData(densities, width, height, maxGenerations);
            foreach (var densityData in dataPerDensity)
            {
                var scatter = plot.Add.Scatter(densityData.data.Keys.ToArray(), densityData.data.Values.ToArray());
                scatter.Smooth = true;
                scatter.Label = $"{densityData.density}";
                scatter.Color = RandomColor();
            }

            plot.SavePng("plot.png", 1920, 1080);
        }

        static List<(double density, Dictionary<int, int> data)> GetData(double[] densities, int width, int height, int maxGenerations)
        {
            var dataPerDensity = new List<(double density, Dictionary<int, int> data)>();

            foreach (var density in densities)
            {
                var aliveCellsInGeneration = new Dictionary<int, int>();

                var board = new Board(width, height, 1, density);
                aliveCellsInGeneration[board.Generation] = board.CountAliveCells();

                for (int i = 0; i < maxGenerations; i++)
                {
                    board.Advance();
                    aliveCellsInGeneration[board.Generation] = board.CountAliveCells();

                    if (board.IsStable())
                    {
                        break;
                    }
                }

                dataPerDensity.Add((density, aliveCellsInGeneration));
            }

            return dataPerDensity;
        }

        static Color RandomColor()
        {
            int red = 100 + Random.Shared.Next() % 255;
            int green = 100 + Random.Shared.Next() % 255;
            int blue = 100 + Random.Shared.Next() % 255;

            return new Color(red, green, blue);
        }

        static async Task Simulate(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                Console.Clear();
                Render();
                lock (boardLocker)
                {
                    board.Advance();
                }
                await Task.Delay(125, cancellationToken);
            }
        }

        static void HandleConsoleInput(CancellationTokenSource cts)
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Escape)
                    {
                        cts.Cancel(true);
                        return;
                    }
                    if (key == ConsoleKey.Enter)
                    {
                        lock (boardLocker)
                        {
                            Helpers.SaveBoardToFile(board, $"gen-{board.Generation}.txt");
                        }
                    }
                }
            }
        }

        static private void Reset()
        {
            var settings = Helpers.LoadBoardSettings("boardSettings.json");
            board = new Board(settings.Width, settings.Height, settings.CellSize, settings.LiveDensity);
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
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
        }
    }
}