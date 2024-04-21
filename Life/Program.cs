using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;

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
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        public readonly double LiveDensity;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;
            LiveDensity = liveDensity;

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
    }
    public class BoardSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }
    public class Program
    {
        public static Board board;
        static public void Reset(BoardSettings settings)
        {
            board = new Board(
                settings.Width,
                settings.Height,
                settings.CellSize,
                settings.LiveDensity);
        }
        static List<string> previousBoardStates = new List<string>();
        static int MaxPreviousStates = 10;
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
        static void SaveBoardState()
        {
            string dateTimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filePath = $"{dateTimeString}.txt";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Columns; col++)
                    {
                        var cell = board.Cells[col, row];
                        writer.Write(cell.IsAlive ? '*' : ' ');
                    }
                    writer.WriteLine();
                }
            }
            Console.WriteLine("Board state saved to " + filePath);
        }
        public static void LoadBoardState(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                int height = lines.Length;
                int width = lines[0].Length;

                if (width != board.Columns || height != board.Rows)
                {
                    Console.WriteLine("Cannot load board state. Dimensions do not match.");
                    return;
                }

                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        char symbol = lines[row][col];
                        board.Cells[col, row].IsAlive = (symbol == '*');
                    }
                }
            }
            else
            {
                Console.WriteLine("File not found: " + filePath + "\nPress any key to continue");
                Console.ReadKey();
            }
        }
        public static bool IsStable()
        {
            var currentBoardState = GetCurrentBoardState();

            foreach (var state in previousBoardStates)
            {
                if (state == currentBoardState)
                {
                    return true;
                }
            }

            previousBoardStates.Add(currentBoardState);

            if (previousBoardStates.Count > MaxPreviousStates)
            {
                previousBoardStates.RemoveAt(0);
            }

            return false;
        }

        static string GetCurrentBoardState()
        {
            string boardState = "";
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    boardState += board.Cells[col, row].IsAlive ? '*' : ' ';
                }
            }
            return boardState;
        }
        static void Main(string[] args)
        {
            var settings = LoadSettings("board_settings.json");
            Reset(settings);
            int steps = 0;
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.S)
                    {
                        SaveBoardState();
                    }
                    else if (key == ConsoleKey.L)
                    {
                        LoadBoardState("The R-pentomino.txt");
                    }
                }
                Console.Clear();
                Render();
                board.Advance();
                //Thread.Sleep(100);
                if (IsStable())
                {
                    Console.WriteLine(steps);
                    steps = 0;
                    break;
                }
                steps++;
            }
        }

        public static BoardSettings LoadSettings(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<BoardSettings>(json);
        }

        public static int CountAliveCells()
        {
            int count = 0;
            for (int row = 0; row < board.Rows; row++)
            {
                for(int col = 0;  col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
