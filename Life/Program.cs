using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Schema;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Collections;

namespace cli_life
{
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
        public Board(int width, int height, int cellSize, bool[,] gameStateArray)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                    Cells[x, y].IsAlive = gameStateArray[x, y];
                }     

            ConnectNeighbors();
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
    }
    public class QueueDensity
    {
        public readonly int MaxSize;
        public readonly Queue<int> Densities;
        public QueueDensity(int maxSize)
        {
            MaxSize = maxSize;
            Densities = new Queue<int>();
        }

        public void AddDensity(int density)
        {
            Densities.Enqueue(density);
            if (Densities.Count >= MaxSize)
            {
                Densities.Dequeue();
            }            
        }
    }

    public class FieldAnalyzer
    {
        private int countStableState;
        public int CountStableState
        {
            get { return countStableState - 9; }
            private set { countStableState = value; }
        }
        public FieldAnalyzer()
        {
            countStableState = 0;
        }
        public void AddCountStableState()
        {
            countStableState++;
        }
        public static int CountAliveCells(Board board)
        {
            int aliveCount = 0;
            foreach (var cell in board.Cells)
            {
                if (cell.IsAlive)
                {
                    aliveCount++;
                }
            }
            return aliveCount;
        }
        private static Dictionary<string, Dictionary<string, int>> InitializeClassification()
        {
            Dictionary<string, Dictionary<string, int>> classification = new Dictionary<string, Dictionary<string, int>>
            {
                {
                    "Stable", new Dictionary<string, int>
                    {
                        { "Block", 0 },
                        { "Beehive", 0 },
                        { "Box", 0 },
                        { "Pond", 0 },
                        { "Snake", 0 }
                    }
                },
                //{
                //    "Periodic", new Dictionary<string, int>()
                //},
                //{
                //    "Moving", new Dictionary<string, int>()
                //},
                //{
                //    "Guns", new Dictionary<string, int>()
                //},
                //{
                //    "Locomotive", new Dictionary<string, int>()
                //},
                //{
                //    "Eater", new Dictionary<string, int>()
                //},
                {
                    "Other", new Dictionary<string, int>()
                    {
                        { "Unknown", 0 },
                    }
                }
            };

            return classification;
        }
        public static (int combinationCount, Dictionary<string, Dictionary<string, int>> classification) CountCombinations(Board board)
        {
            Dictionary<string, Dictionary<string, int>> classification = InitializeClassification();
            bool[,] visited = new bool[board.Columns, board.Rows];
            int combinationCount = 0;

            for (int x = 0; x < board.Columns; x++)
            {
                for (int y = 0; y < board.Rows; y++)
                {
                    if (board.Cells[x, y].IsAlive && !visited[x, y])
                    {
                        HashSet<Point> liveCellCoordinates = ExploreCombination(board, visited, x, y);
                        ClassifyFigures(board, liveCellCoordinates, classification);
                        combinationCount++;
                    }
                }
            }

            return (combinationCount, classification);
        }
        private static HashSet<Point> ExploreCombination(Board board, bool[,] visited, int startX, int startY)
        {
            HashSet<Point> liveCellCoordinates = new HashSet<Point>();
            Stack<Point> stack = new Stack<Point>();
            stack.Push(new Point(startX, startY));

            while (stack.Count > 0)
            {
                Point current = stack.Pop();

                int x = current.X;
                int y = current.Y;

                if (x < 0 || x >= board.Columns || y < 0 || y >= board.Rows || visited[x, y] || !board.Cells[x, y].IsAlive)
                {
                    continue;
                }

                visited[x, y] = true;
                liveCellCoordinates.Add(current);

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (!(dx == 0 && dy == 0))
                        {
                            int neighborX = (x + dx + board.Columns) % board.Columns;
                            int neighborY = (y + dy + board.Rows) % board.Rows;

                            stack.Push(new Point(neighborX, neighborY));
                        }
                    }
                }
            }

            return liveCellCoordinates;
        }
        private static void ClassifyFigures(Board board, HashSet<Point> liveCellCoordinates, Dictionary<string, Dictionary<string, int>> classification)
        {
            foreach (var key in classification.Keys)
            {
                foreach (var figure in classification[key].Keys) 
                {
                    if (Figures.IdentifyFigure(liveCellCoordinates, board.Columns, board.Rows) == figure)
                    {
                        classification[key][figure]++;
                    }
                }
            }
        }
        public static bool IsStable(QueueDensity queueDensity)
        {
            var array = queueDensity.Densities.ToArray();
            int length = array.Length;

            if (length < queueDensity.MaxSize - 1)
            {
                return false;
            }

            for (int len = 1; len <= length / 2; len++)
            {
                bool isRepeating = true;

                for (int start = 0; start <= length - 2 * len; start++)
                {
                    for (int i = 0; i < len; i++)
                    {
                        if (array[start + i] != array[start + len + i])
                        {
                            isRepeating = false;
                            break;
                        }
                    }

                    if (isRepeating)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class Figures()
    {
        public static string IdentifyFigure(HashSet<Point> liveCellCoordinates, int columns, int rows)
        {
            if (IsBlock(liveCellCoordinates, columns, rows))
                return "Block";
            else if (IsBeehive(liveCellCoordinates, columns, rows))
                return "Beehive";
            else if (IsBox(liveCellCoordinates, columns, rows))
                return "Box";
            else if (IsPond(liveCellCoordinates, columns, rows))
                return "Pond";
            else if (IsSnake(liveCellCoordinates, columns, rows))
                return "Snake";

            return "Unknown";
        }
        private static bool IsBlock(HashSet<Point> liveCellCoordinates, int columns, int rows)
        {
            foreach (var point in liveCellCoordinates)
            {
                int x = point.X;
                int y = point.Y;

                // Block
                // **
                // **
                if (liveCellCoordinates.Contains(new Point(x, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, y)) &&
                    liveCellCoordinates.Contains(new Point(x, (y + 1) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, (y + 1) % rows)))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsBeehive(HashSet<Point> liveCellCoordinates, int columns, int rows)
        {
            foreach (var point in liveCellCoordinates)
            {
                int x = point.X;
                int y = point.Y;

                // Beehive
                // - * -
                // * - *
                // * - *
                // - * -
                //
                //- * * -
                //* - - *
                //- * * -
                if ((liveCellCoordinates.Contains(new Point(x, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, (y - 1) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 2) % columns, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 2) % columns, (y + 1) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, (y + 2) % rows)) &&
                    liveCellCoordinates.Contains(new Point(x, (y + 1) % rows))) ||
                    (liveCellCoordinates.Contains(new Point(x, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 2) % columns, (y + 1) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, (y + 2) % rows)) &&
                    liveCellCoordinates.Contains(new Point(x, (y + 2) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x - 1) % columns, (y + 1) % rows))))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsBox(HashSet<Point> liveCellCoordinates, int columns, int rows)
        {
            foreach (var point in liveCellCoordinates)
            {
                int x = point.X;
                int y = point.Y;

                // Box
                // - * -
                // * - *
                // - * -
                if (liveCellCoordinates.Contains(new Point(x, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, (y +1) % rows)) &&
                    liveCellCoordinates.Contains(new Point(x, (y + 2) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x - 1) % columns, (y + 1) % rows)))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsPond(HashSet<Point> liveCellCoordinates, int columns, int rows)
        {
            foreach (var point in liveCellCoordinates)
            {
                int x = point.X;
                int y = point.Y;

                // Pond
                // - * * -
                // * - - *
                // * - - *
                // - * * -
                if (liveCellCoordinates.Contains(new Point(x, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, (y - 1) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 2) % columns, (y - 1) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 3) % columns, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 3) % columns, (y + 1) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 2) % columns, (y + 2) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, (y + 2) % rows)) &&
                    liveCellCoordinates.Contains(new Point(x, (y + 1) % rows)))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsSnake(HashSet<Point> liveCellCoordinates, int columns, int rows)
        {
            foreach (var point in liveCellCoordinates)
            {
                int x = point.X;
                int y = point.Y;

                // Snake
                // * - * *
                // * * - *
                //
                // * *
                // * -
                // - *
                // * *
                if ((liveCellCoordinates.Contains(new Point(x, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 2) % columns, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 3) % columns, y)) &&
                    liveCellCoordinates.Contains(new Point(x, (y + 1) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, (y + 1) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 3) % columns, (y + 1) % rows))) ||
                    (liveCellCoordinates.Contains(new Point(x, y)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, y)) &&
                    liveCellCoordinates.Contains(new Point(x, (y + 1) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, (y + 2) % rows)) &&
                    liveCellCoordinates.Contains(new Point((x + 1) % columns, (y + 3) % rows)) &&
                    liveCellCoordinates.Contains(new Point(x, (y + 3) % rows))))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class Settings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }

    public class FileHandler
    {
        public static void SaveToFile(string filePath, Board board, Settings settings)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"{settings.Width} {settings.Height} {settings.CellSize} {settings.LiveDensity}");

                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Columns; col++)
                    {
                        writer.Write(board.Cells[col, row].IsAlive ? '*' : '-');
                    }
                    writer.WriteLine();
                }
            }
        }
        public static void LoadFromFile(string filePath, out Board board, out Settings settings)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string[] settingsArray = reader.ReadLine().Split(' ');
                settings = new Settings
                {
                    Width = int.Parse(settingsArray[0]),
                    Height = int.Parse(settingsArray[1]),
                    CellSize = int.Parse(settingsArray[2]),
                    LiveDensity = double.Parse(settingsArray[3])
                };

                int rows = settings.Height / settings.CellSize;
                int columns = settings.Width / settings.CellSize;

                bool[,] gameStateArray = new bool[columns, rows];

                for (int row = 0; row < rows; row++)
                {
                    string line = reader.ReadLine();
                    for (int col = 0; col < columns; col++)
                    {
                        gameStateArray[col, row] = line[col] == '*';
                    }
                }

                board = new Board(
                width: settings.Width,
                height: settings.Height,
                cellSize: settings.CellSize,
                gameStateArray);
            }
        }
        public static void SaveCountAliveCell(string filePath, int densityAlive)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(densityAlive);
            }
        }
    }

    class Program
    {
        static Board board;
        static Settings settings;
        static QueueDensity queueDensity;
        static int densityAlive = 0;
        static int sizeStabilityDefinition = 10;
        static private void Reset(Settings settings)
        {
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

        static void Main(string[] args)
        {
            string file_state = "..\\..\\..\\game_state.txt";
            string file_settings = "..\\..\\..\\settings.json";
            string file_count = "..\\..\\..\\densityAlive.txt";
            FieldAnalyzer fieldAnalyzer = new FieldAnalyzer();
            bool f = true;

            queueDensity = new QueueDensity(sizeStabilityDefinition);

            if (File.Exists(file_state) && f)
            {
                FileHandler.LoadFromFile(file_state, out board, out settings);
            }
            else
            {
                string json = File.ReadAllText(file_settings);
                settings = JsonConvert.DeserializeObject<Settings>(json);
                Reset(settings);
            }

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                FileHandler.SaveToFile(file_state, board, settings);
            };

            if (File.Exists(file_count))
            {
                File.Delete(file_count);
            }

            while (true)
            {
                Console.Clear();
                Render();
                int countAliveCells = FieldAnalyzer.CountAliveCells(board);
                Console.WriteLine($"Alive cells: {countAliveCells}");
                var (combinationCount, classification) = FieldAnalyzer.CountCombinations(board);
                Console.WriteLine($"Combinations: {combinationCount}");
                foreach (var category in classification)
                {
                    Console.WriteLine($"Category: {category.Key}");
                    foreach (var figureType in category.Value)
                    {
                        Console.WriteLine($"  {figureType.Key}: {figureType.Value}");
                    }
                }

                FileHandler.SaveCountAliveCell(file_count, countAliveCells);
                if (FieldAnalyzer.IsStable(queueDensity))
                {
                    Console.WriteLine("Game is stable!");
                    Console.WriteLine($"Number generations of transition to a stable phase: {fieldAnalyzer.CountStableState}");
                }
                else
                {
                    fieldAnalyzer.AddCountStableState();
                    queueDensity.AddDensity(countAliveCells);
                }

                board.Advance();
                Thread.Sleep(1000);
            }
        }
    }
}