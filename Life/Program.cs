using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Schema;

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

    public class FieldAnalyzer
    {
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

        public static int CountCombinations(Board board)
        {
            bool[,] visited = new bool[board.Columns, board.Rows];
            int combinationCount = 0;

            for (int x = 0; x < board.Columns; x++)
            {
                for (int y = 0; y < board.Rows; y++)
                {
                    if (board.Cells[x, y].IsAlive && !visited[x, y])
                    {
                        // new unvisited living cell has been found - we start a new combination
                        ExploreCombination(board, visited, x, y);
                        combinationCount++;
                    }
                }
            }

            return combinationCount;
        }

        private static void ExploreCombination(Board board, bool[,] visited, int startX, int startY)
        {
            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((startX, startY));

            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();

                if (x < 0 || x >= board.Columns || y < 0 || y >= board.Rows || visited[x, y] || !board.Cells[x, y].IsAlive)
                {
                    continue;
                }

                visited[x, y] = true;

                // Sorting through the neighbors
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (!(dx == 0 && dy == 0))
                        {
                            int neighborX = (x + dx + board.Columns) % board.Columns; // processing of cyclic boundaries horizontally
                            int neighborY = (y + dy + board.Rows) % board.Rows; // processing of cyclic boundaries vertically

                            stack.Push((neighborX, neighborY));
                        }
                    }
                }
            }
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
    }

    class Program
    {
        static Board board;
        static Settings settings;
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
            bool f = true;

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

            while (true)
            {
                Console.Clear();
                Render();
                Console.WriteLine($"Alive cells: {FieldAnalyzer.CountAliveCells(board)}");
                Console.WriteLine($"Сombinations: {FieldAnalyzer.CountCombinations(board)}");
                board.Advance();
                Thread.Sleep(1000);
            }
        }
    }
}