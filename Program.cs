using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
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
        public Board(Set c)
        {
            CellSize = c.CellSize;

            Cells = new Cell[c.Width / c.CellSize, c.Height / c.CellSize];
            for (int i = 0; i < Columns; i++)
                for (int j = 0; j < Rows; j++)
                    Cells[i, j] = new Cell();

            ConnectNeighbors();
            Randomize(c.LiveDensity);
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
    public class Set
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
        public void Conf(String s)
        {
            data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(s));
        }
        public void LoadState(Board board, String s)
        {
            String b = File.ReadAllText(s);
            int i = 0;
            int j = -1;
            foreach (char c in b)
            {
                j += 1;
                if ((j == 50) && (i == 19))
                {
                    break;
                }
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
        public void SaveState(Board board, String s)
        {
            String b = "";
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        b += "*";
                    }
                    else
                    {
                        b += " ";
                    }
                }
                b += "\n";
            }
            File.WriteAllText(s, b);
        }
    }

    class Program
    {
        static Board board;
        static private void Reboot(Set c)
        {
            board = new Board(c);
        }
        static void Show()
        {
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    var c = board.Cells[j, i];
                    if (c.IsAlive)
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
            int num = 0;
            Set c = new Set();
            c.Conf("D:\\Life\\form.json");
            Reboot(c);
            c.LoadState(board, "D:\\Life\\data.txt");
            while ((true) && (num < 10))
            {
                num++;
                Console.Clear();
                Show();
                c.SaveState(board, "D:\\Life\\data.txt");
                board.Advance();
                Thread.Sleep(1000);
            }
            do
            {
                Console.WriteLine("Choose your number: 1 - Мигалки, 2 - Ящик, 3 - Улей, 4 - Каравай, 5 - Корабль");
                int choice = 0;
                while (choice != 1 && choice != 2 && choice != 3 && choice != 4 && choice != 5)
                {
                    choice = int.Parse(Console.ReadLine());
                }
                switch (choice)
                {
                    case 1:
                        num = 0;
                        c.LoadState(board, "D:\\Life\\Life.Test\\1.txt");
                        while ((true) && (num < 5))
                        {
                            num++;
                            Console.Clear();
                            Show();
                            c.SaveState(board, "D:\\Life\\Life.Test\\1.txt");
                            board.Advance();
                            Thread.Sleep(1000);
                        }
                        break;
                    case 2:
                        num = 0;
                        c.LoadState(board, "D:\\Life\\Life.Test\\2.txt");
                        while ((true) && (num < 5))
                        {
                            num++;
                            Console.Clear();
                            Show();
                            c.SaveState(board, "D:\\Life\\Life.Test\\2.txt");
                            board.Advance();
                            Thread.Sleep(1000);
                        }
                        break;
                    case 3:
                        num = 0;
                        c.LoadState(board, "D:\\Life\\Life.Test\\3.txt");
                        while ((true) && (num < 5))
                        {
                            num++;
                            Console.Clear();
                            Show();
                            c.SaveState(board, "D:\\Life\\Life.Test\\3.txt");
                            board.Advance();
                            Thread.Sleep(1000);
                        }
                        break;
                    case 4:
                        num = 0;
                        c.LoadState(board, "D:\\Life\\Life.Test\\4.txt");
                        while ((true) && (num < 5))
                        {
                            num++;
                            Console.Clear();
                            Show();
                            c.SaveState(board, "D:\\Life\\Life.Test\\4.txt");
                            board.Advance();
                            Thread.Sleep(1000);
                        }
                        break;
                    case 5:
                        num = 0;
                        c.LoadState(board, "D:\\Life\\Life.Test\\5.txt");
                        while ((true) && (num < 5))
                        {
                            num++;
                            Console.Clear();
                            Show();
                            c.SaveState(board, "D:\\Life\\Life.Test\\5.txt");
                            board.Advance();
                            Thread.Sleep(1000);
                        }
                        break;
                }
            } while (true);

        }
    }
}