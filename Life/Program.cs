using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;

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
        public int Height
        {
            get
            {
                return Rows * CellSize;
            }
        }
        public int Width
        {
            get
            {
                return Columns * CellSize;
            }
        }
        public int Columns
        {
            get
            {
                return Cells.GetLength(0);
            }
        }
        public int Rows
        {
            get
            {
                return Cells.GetLength(1);
            }
        }
        public Board(Configuration configuration)
        {
            CellSize = configuration.CellSize;

            Cells = new Cell[configuration.Width / configuration.CellSize, configuration.Height / configuration.CellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(configuration.LiveDensity);
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
    public class Configuration
    {
        struct Data
        {
            public int Height { get; set; }
            public int Width { get; set; }
            public int CellSize { get; set; }
            public double LiveDensity { get; set; }
        }
        Data data;
        public int Height
        {
            get
            {
                return data.Height;
            }
        }
        public int Width
        {
            get
            {
                return data.Width;
            }
        }

        public int CellSize
        {
            get
            {
                return data.CellSize;
            }
        }
        public double LiveDensity
        {
            get
            {
                return data.LiveDensity;
            }
        }
        public void LoadConfig(String path)
        {
            data = JsonSerializer.Deserialize<Data>(File.ReadAllText(path));
        }
        public void LoadState(Board board, String path)
        {
            String buf = File.ReadAllText(path);
            int i = 0;
            int j = -1;
            foreach (char c in buf)
            {
                j += 1;
                if (c == '*')
                {
                    board.Cells[j, i].IsAlive = true;
                }
                if (c == ' ')
                {
                    board.Cells[j, i].IsAlive = false;
                }
                if (c == '\n')
                {
                    i += 1;
                    j = -1;
                }
            }
        }
        public void SaveState(Board board, String path)
        {
            String buf = "";
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        buf += "*";
                    }
                    else
                    {
                        buf += " ";
                    }
                }
                buf += "\n";
            }
            File.WriteAllText(path, buf);
        }
    }

    class Program
    {
        static Board board;
        static private void Reset(Configuration configuration)
        {
            board = new Board(configuration);
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
            Configuration configuration = new Configuration();
            configuration.LoadConfig("mod-task04-life\\Life\\Configuration.json");
            Reset(configuration);
            configuration.LoadState(board, "mod-task04-life\\Life\\save.txt");
            while (true)
            {
                Console.Clear();
                Render();
                configuration.SaveState(board, "C:\\LabsMipis\\mip4\\mod-task04-life\\Life\\save.txt");
                board.Advance();
                Thread.Sleep(1000);
            }
        }
    }
}