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
            data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(path));
        }
        public void LoadState(Board board, String path)
        {
            String buf = File.ReadAllText(path);
            int i = 0;
            int j = -1;
            foreach (char c in buf)
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
            int schet = 0;
            Configuration configuration = new Configuration();
            configuration.LoadConfig("C:\\Programms\\GameLife\\Configuration.json");
            Reset(configuration);
            configuration.LoadState(board, "C:\\Programms\\GameLife\\board.txt");
            while ((true) && (schet < 10))
            {
                schet++;
                Console.Clear();
                Render();
                configuration.SaveState(board, "C:\\Programms\\GameLife\\board.txt");
                board.Advance();
                Thread.Sleep(1000);
            }
do{ 
            Console.WriteLine("Выберите номер теста: 1 - Мигалки, 2 - Ящик, 3 - Улей, 4 - Каравай, 5 - Корабль");
            int choice = 0;
            while (choice != 1 && choice != 2 && choice != 3 && choice != 4 && choice != 5)
            {
                choice = int.Parse(Console.ReadLine());
            }
            switch (choice)
            {
                case 1:
                    schet = 0;
                    configuration.LoadState(board, "C:\\Programms\\GameLife\\Tests\\test1.txt");
                    while ((true) && (schet < 5))
                    {
                        schet++;
                        Console.Clear();
                        Render();
                        configuration.SaveState(board, "C:\\Programms\\GameLife\\Tests\\test1.txt");
                        board.Advance();
                        Thread.Sleep(1000);
                    }
                    break;
                case 2:
                    schet = 0;
                    configuration.LoadState(board, "C:\\Programms\\GameLife\\Tests\\test2.txt");
                    while ((true) && (schet < 5))
                    {
                        schet++;
                        Console.Clear();
                        Render();
                        configuration.SaveState(board, "C:\\Programms\\GameLife\\Tests\\test2.txt");
                        board.Advance();
                        Thread.Sleep(1000);
                    }
                    break;
                case 3:
                    schet = 0;
                    configuration.LoadState(board, "C:\\Programms\\GameLife\\Tests\\test3.txt");
                    while ((true) && (schet < 5))
                    {
                        schet++;
                        Console.Clear();
                        Render();
                        configuration.SaveState(board, "C:\\Programms\\GameLife\\Tests\\test3.txt");
                        board.Advance();
                        Thread.Sleep(1000);
                    }
                    break;
                case 4:
                    schet = 0;
                    configuration.LoadState(board, "C:\\Programms\\GameLife\\Tests\\test4.txt");
                    while ((true) && (schet < 5))
                    {
                        schet++;
                        Console.Clear();
                        Render();
                        configuration.SaveState(board, "C:\\Programms\\GameLife\\Tests\\test4.txt");
                        board.Advance();
                        Thread.Sleep(1000);
                    }
                    break;
                case 5:
                    schet = 0;
                    configuration.LoadState(board, "C:\\Programms\\GameLife\\Tests\\test5.txt");
                    while ((true) && (schet < 5))
                    {
                        schet++;
                        Console.Clear();
                        Render();
                        configuration.SaveState(board, "C:\\Programms\\GameLife\\Tests\\test5.txt");
                        board.Advance();
                        Thread.Sleep(1000);
                    }
                    break;
            }
}while(true);

        }
    }
}
