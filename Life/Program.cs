using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace cli_life
{
    public class Data
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
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

        public Board(Data data)
        {
            CellSize = data.CellSize;

            Cells = new Cell[data.Width / CellSize, data.Height / CellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(data.LiveDensity);
        }

        readonly Random rand = new Random();

        public void Randomize(double liveDensity)
        {        
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void GetCellsFromFile(string fileWay)
        {
            string[] rows = File.ReadAllLines(fileWay);
            char[][] cells = new char[Rows][];

            for (int i = 0; i < rows.Length; i++)
            {
                cells[i] = new char[Columns];
                for (int j = 0; j < Rows; j++)
                {
                    cells[i][j] = rows[i][j];
                }
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (cells[i][j] == '1')
                    {
                        Cells[i, j].IsAlive = true;
                    }
                    else
                    {
                        Cells[i, j].IsAlive = false;
                    }
                }
            }
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

        public int GetAliveCells()
        {
            int count = 0;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Cells[col, row].IsAlive) count++;
                }
            }

            return count;
        }

        public int BlocksAmount()
        {
            int num = 0;

            for (int i = 1; i < Rows - 2; i++)
            {
                for (int j = 1; j < Columns - 2; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j, i + 1].IsAlive && Cells[j + 1, i].IsAlive && Cells[j + 1, i + 1].IsAlive)
                    {
                        if (!Cells[j - 1, i - 1].IsAlive && !Cells[j, i - 1].IsAlive && !Cells[j + 1, i - 1].IsAlive && !Cells[j + 2, i - 1].IsAlive
                        && !Cells[j - 1, i + 2].IsAlive && !Cells[j, i + 2].IsAlive && !Cells[j + 1, i + 2].IsAlive && !Cells[j + 2, i + 2].IsAlive
                        && !Cells[j - 1, i].IsAlive && !Cells[j + 2, i].IsAlive && !Cells[j - 1, i + 2].IsAlive && !Cells[j + 2, i + 2].IsAlive)
                        {
                            num++;
                        }
                    }
                }
            }

            return num;
        }

        public int BoxesAmount()
        {
            int num = 0;
            for (int i = 0; i < Rows - 2; i++)
            {
                for (int j = 1; j < Columns - 1; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j - 1, i + 1].IsAlive && Cells[j + 1, i + 1].IsAlive && Cells[j, i + 2].IsAlive
                    && !Cells[j, i + 1].IsAlive && !Cells[j - 1, i].IsAlive && !Cells[j + 1, i].IsAlive && !Cells[j - 1, i + 2].IsAlive && !Cells[j + 1, i + 2].IsAlive)
                    {
                        num++;
                    }
                }
            }

            return num;
        }
    }

    public class Program
    {
        static Board board;

        static private void Reset()
        {
            //var fileWay = "";
            //string json = File.ReadAllText(fileWay);
            //var data = JsonSerializer.Deserialize<Data>(json);
            var data = new Data();
            data.CellSize = 1;
            data.Height = 5;
            data.Width = 5;
            data.LiveDensity = 0;
            board = new Board(data);
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

        static void WriteToFile()
        {
            var lines = new List<List<char>>();

            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    var cell = board.Cells[j, i];
                    if (cell.IsAlive)
                    {
                        lines[i].Add('*');
                    }
                    else
                    {
                        lines[i].Add(' ');
                    }
                }
            }

            File.Create(".//usersboard//res.txt").Close();

            using (StreamWriter writer = new StreamWriter(".//usersboard//res.txt", true))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string str = new string(lines[i].ToString());
                    writer.WriteLineAsync(str);
                }
            }
        }

        static void GetFullInfo(Board board)
        {
            Console.WriteLine("Number of alive cells:  " + board.GetAliveCells());
            Console.WriteLine("Number of blocks:  " + board.BlocksAmount());
            Console.WriteLine("Number of boxes:  " + board.BoxesAmount());            
        }

        static void Main(string[] args)
        {
            var count = 15;
            var step = 0;
            Reset();
            board.GetCellsFromFile(".//testboard//test1.txt");

            while(step<count)
            {
                step++;
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(1000);
            }

            GetFullInfo(board);
            WriteToFile();
        }
    }
}