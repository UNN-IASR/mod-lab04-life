using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace cli_life
{
    public class Props
    {
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }
        public double liveDensity { get; set; }
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
        readonly Random rand = new Random();

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(Props props)
        {
            CellSize = props.cellSize;
            Cells = new Cell[props.width / CellSize, props.height / CellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();
            ConnectNeighbors();
            Randomize(props.liveDensity);
        }

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

        public void ReadFile(string path)
        {
            string[] rows = File.ReadAllLines(path);
            char[][] cells = new char[Rows][];
            for (int i = 0; i < rows.Length; i++)
            {
                cells[i] = new char[Columns];
                for (int j = 0; j < Rows; j++)
                    cells[i][j] = rows[i][j];
            }
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (cells[i][j] == '1')
                        Cells[i, j].IsAlive = true;
                    else
                        Cells[i, j].IsAlive = false;
                }
            }
        }
        public int AliveCellsCount()
        {
            int count = 0;
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                    if (Cells[col, row].IsAlive)
                        count++;
            return count;
        }
        public int BlocksCount()
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
                            num++;
                    }
                }
            }
            return num;
        }
        public int BoxesCount()
        {
            int num = 0;
            for (int i = 0; i < Rows - 2; i++)
            {
                for (int j = 1; j < Columns - 1; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j - 1, i + 1].IsAlive && Cells[j + 1, i + 1].IsAlive && Cells[j, i + 2].IsAlive
                    && !Cells[j, i + 1].IsAlive && !Cells[j - 1, i].IsAlive && !Cells[j + 1, i].IsAlive && !Cells[j - 1, i + 2].IsAlive && !Cells[j + 1, i + 2].IsAlive)
                        num++;
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
         
            Props props = new Props();
            props.cellSize = 1;
            props.height = 5;
            props.width = 5;
            props.liveDensity = 0;
            board = new Board(props);
        }
        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    Cell cell = board.Cells[col, row];
                    if (cell.IsAlive)
                        Console.Write('*');
                    else
                        Console.Write(' ');
                }
                Console.Write('\n');
            }
        }

        static void Info(Board board)
        {
            Console.WriteLine("количество живых клеток:  " + board.AliveCellsCount());
            Console.WriteLine("количество блоков:  " + board.BlocksCount());
            Console.WriteLine("количество боксов:  " + board.BoxesCount());
        }

        static void Main(string[] args)
        {
            var count = 15;
            var step = 0;
            Reset();
            board.ReadFile("input.txt");
            while (step < count)
            {
                step++;
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(1000);
            }
            Info(board);
        }
    }
}
