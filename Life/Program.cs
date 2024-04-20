using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using System.Linq.Expressions;
using ScottPlot.AxisLimitCalculators;
using System.Diagnostics.Contracts;
using ScottPlot;
using System.Runtime.ExceptionServices;
using System.Diagnostics.CodeAnalysis;
using static System.Net.Mime.MediaTypeNames;

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

        public Board(Settings s)
        {
            CellSize = s.cellSize;
            Cells = new Cell[s.width / s.cellSize, s.height / s.cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(s.liveDensity);
        }

        public Board(Settings s, string file_name)
        {
            CellSize = s.cellSize;
            Cells = new Cell[s.width / s.cellSize, s.height / s.cellSize];

            if (!File.Exists(file_name))
            {
                for (int x = 0; x < Columns; x++)
                    for (int y = 0; y < Rows; y++)
                        Cells[x, y] = new Cell();

                ConnectNeighbors();
                Randomize(s.liveDensity);
                return;
            };
            string file_data = File.ReadAllText(file_name);
            string[] lines = file_data.Split('\n');
            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    Cells[i, j] = new();
                    Cells[i, j].IsAlive = lines[j][i] == '*';
                }
            }
            ConnectNeighbors();
        }

        public void Save(string file_name)
        {
            string[] output = new string[Columns];
            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    if (Cells[i, j].IsAlive) output[j] += '*';
                    else output[j] += ' ';
                }
            }
            File.WriteAllLines(file_name, output);
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            if (liveDensity > 1) throw new Exception("incorrect density");
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
    [Serializable()]
    public class Settings
    {
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }
        public float liveDensity { get; set; }
    }
    public struct BoardArchive
    {
        public int count;
        public bool[,] data;
        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if (((BoardArchive)obj).count != count) return false;
            for (int i = 0; i < data.GetLength(0); i++) 
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (((BoardArchive)obj).data[i, j] != data[i, j]) return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return count;
        }
    }
    public class Runner
    {
        public Board board;
        //public string file_name = "../../../input.txt";
        public List<BoardArchive> previous_boards = new();
        public bool show = false;
        public int step = 0;
        public bool overflow = false;
        public Runner(string file_name)
        {
            string fileName = "../../../settings.json";
            Settings s = new()
            {
                width = 100,
                height = 100,
                cellSize = 1,
                liveDensity = 1.0f,
            };
            if (File.Exists(fileName))
            {
                string jsonString;
                jsonString = File.ReadAllText(fileName);
                s = JsonSerializer.Deserialize<Settings>(jsonString);
            }
            board = new Board(s, file_name);
            previous_boards.Clear();
        }
        public Runner(Settings s, string file_name)
        {
            board = new Board(s, file_name);
            previous_boards.Clear();
        }
        bool RememberCompareState()
        {
            if (previous_boards.Count >= 50) previous_boards.RemoveAt(0);
            BoardArchive archive = new()
            {
                data = new bool[board.Columns, board.Rows]
            };
            for (int i = 0; i < board.Columns; i++)
            {
                for (int j = 0; j < board.Rows; j++)
                {
                    bool alive = board.Cells[i, j].IsAlive;
                    archive.data[i, j] = alive;
                    if (alive) archive.count++;
                }
            }
            foreach (BoardArchive prev in previous_boards)
            {
                if (prev.Equals(archive))
                {
                    return true;
                }
            }
            previous_boards.Add(archive);
            return false;
        }
        void Render()
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
        public void Run(int max_steps)
        {
            for (int iter = 0; iter < max_steps; iter++)
            {
                step++;
                board.Advance();
                if (show)
                {
                    Thread.Sleep(1000);
                    Render();
                }
                if (RememberCompareState())
                {
                    return;
                }
            }
            overflow = true;
        }
    }
    public class Program
    {
        static public Board board;
        static public string file_name = "../../../input.txt";
        static public List<BoardArchive> previous_boards = new();
        static public void Reset()
        {
            string fileName = "../../../settings.json";
            Settings s = new()
            {
                width = 100,
                height = 100,
                cellSize = 1,
                liveDensity = 1.0f,
            };
            if (File.Exists(fileName))
            {
                string jsonString;
                jsonString = File.ReadAllText(fileName);
                s = JsonSerializer.Deserialize<Settings>(jsonString);
            }
            board = new Board(s, file_name);
            previous_boards.Clear();
        }
        static public void Reset(Settings s)
        {
            board = new Board(s, "");
            previous_boards.Clear();
        }
        static public bool RememberCompareState()
        {
            if (previous_boards.Count >= 50) previous_boards.RemoveAt(0);
            BoardArchive archive = new()
            {
                data = new bool[board.Columns, board.Rows]
            };
            for (int i = 0; i < board.Columns; i++)
            {
                for (int j = 0; j < board.Rows; j++)
                {
                    bool alive = board.Cells[i, j].IsAlive;
                    archive.data[i, j] = alive;
                    if (alive) archive.count++;
                }
            }
            foreach (BoardArchive prev in previous_boards)
            {
                if (prev.Equals(archive))
                {
                    return true;
                }
            }
            previous_boards.Add(archive);
            return false;
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
            Console.Write("Launch avg test (Y/N)? ");
            if (Console.ReadLine() == "Y")
            {
                List<float> dataX = new();
                List<float> dataY = new();
                List<int> dataoverflows = new();
                int current_sum = 0;
                float previous = 0.0f;
                int overflow = 0;
                for (float i = 0.0001f; i < 1; i += 0.0001f)
                {
                    if (Math.Ceiling(i * 100.0f) != previous)
                    {
                        Console.WriteLine($"AVG iterations for {previous}% density is {current_sum / 100.0f} with {overflow} overflows.");
                        dataX.Add(previous);
                        dataY.Add(current_sum / 100.0f);
                        dataoverflows.Add(overflow);
                        current_sum = 0;
                        overflow = 0;
                    }
                    Settings s = new Settings()
                    {
                        width = 30,
                        height = 30,
                        cellSize = 1,
                        liveDensity = (float)Math.Ceiling(i * 100) / 100.0f
                    };
                    Runner runner = new(s, "");
                    const int max_iter = 10000;
                    runner.Run(max_iter);
                    if (runner.overflow) overflow++;
                    current_sum += runner.step;
                    previous = (float)Math.Ceiling(i * 100);
                }

                ScottPlot.Plot myPlot = new();
                myPlot.Add.Scatter(dataX, dataY);
                myPlot.SavePng("../../../chart.png", 1000, 800);
            }

            Console.Write("File name: ");
            string name = Console.ReadLine();
            if (name.Length != 0)
                file_name = "../../../" + name;

            Runner r = new(file_name);
            Console.Write("How many iterations do you want? ");
            int count = int.Parse(Console.ReadLine());
            r.show = true;
            r.Run(count);
            r.board.Save("../../../output.txt");
        }
    }
}