using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;


namespace cli_life
{
    public class Data
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

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity)
        {
            CellSize = cellSize;

            Cells = new Cell[width / CellSize, height / CellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
        }

        readonly Random rand = new Random();

        public void GetCellsFromFile(string filename) 
        {
            string[] s = File.ReadAllLines(filename);
            char[][] arr = new char[Rows][];
            for (int i = 0; i < s.Length; i++)
            {
                arr[i] = new char[Columns];
                for (int j = 0; j < Rows; j++)
                {
                    arr[i][j] = s[i][j];
                }
            }
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (arr[i][j] == '*')
                    {
                        Cells[i,j].IsAlive = true;
                    }
                }
            }
        }

        public void Randomize(string filename)
        {
            string json = File.ReadAllText(filename);
            Data data = JsonSerializer.Deserialize<Data>(json);
            double liveDensity = data.liveDensity;
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

        public int HivesAmount()
        {
            int num = 0;
            for (int i = 0; i < Rows - 3; i++)
            {
                for (int j = 1; j < Columns - 1; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j - 1, i + 1].IsAlive && Cells[j - 1, i + 2].IsAlive 
                    && Cells[j, i + 3].IsAlive && Cells[j + 1, i + 1].IsAlive && Cells[j + 1, i + 2].IsAlive 
                    && !Cells[j, i + 1].IsAlive && !Cells[j, i + 2].IsAlive && !Cells[j - 1, i].IsAlive 
                    && !Cells[j + 1, i].IsAlive && !Cells[j - 1, i + 3].IsAlive && !Cells[j + 1, i + 3].IsAlive)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public int SymmetryFiguresAmount()
        {
            return BoxesAmount() + HivesAmount()+ BlocksAmount() ;
        }
    }

    public class LifeGame
    {
        static Board board;

        public int Reset(string filename, string read_from)
        {
            string json = File.ReadAllText(read_from);
            Data data = JsonSerializer.Deserialize<Data>(json);
            int width = data.width;
            int height = data.height;
            int cellSize = data.cellSize;
            double liveDensity = data.liveDensity;
            board = new Board(width, height, cellSize, liveDensity);
            board.GetCellsFromFile(filename);
            return board.Width * board.Height;
        }

        public int Render()
        {
            int count = 0;
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)   
                {
                    var cell = board.Cells[j, i];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                        count++;
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
            return count;
        }

        public void WriteToFile()
        {
            char[][] lines = new char[20][];
            for (int k = 0; k < 20; k++)
            {
                lines[k] = new char[50];
            }
            for (int i = 0; i < board.Rows; i++)
                {
                    for (int j = 0; j < board.Columns; j++)   
                    {
                        var cell = board.Cells[j, i];
                        if (cell.IsAlive)
                        {
                            lines[i][j] = '*';
                        }
                        else
                        {
                            lines[i][j] = ' ';
                        }
                    }
                }
            File.Create("mod-lab04-life/user_stuff/res.txt").Close();
            using (StreamWriter writer = new StreamWriter("mod-lab04-life/user_stuff/res.txt", true))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string str = new string(lines[i]);
                    writer.WriteLineAsync(str);
                }
            }
        }

        public (int allCells, int aliveCells, int Iters) Run(string filename, string read_from)
        {
            int[] list = {-1, -1, -1, -1, -1};
            int iters = 0;
            int alive_cells = 0;
            int all_cells = 0;

            all_cells = Reset(filename, read_from);

            while(true)
            {
                iters++;
                Console.Clear();
                alive_cells = Render();
                list[iters % 5] = alive_cells;
                if ((list[0] == list[1]) && (list[0] == list[2]) && (list[0] == list[3]) && (list[0] == list[4]))
                {
                    break;
                }
                board.Advance();
                Thread.Sleep(100);
            }

            Console.WriteLine("\n\tКоличество блоков: " + board.BlocksAmount());
            Console.WriteLine("\tКоличество ящиков: " + board.BoxesAmount());
            Console.WriteLine("\tКоличество ульев: " + board.HivesAmount());

            (int, int, int) cells = (all_cells, alive_cells, iters-2);
            return cells;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            LifeGame life = new LifeGame();
            var cells = life.Run("mod-lab04-life/user_stuff/example1.txt", "mod-lab04-life/user_stuff/user_settings.json");

            Console.Write("\n\tКоличество живых клеток: " + cells.aliveCells);
            Console.Write("\n\tКоличество мертвых клеток: " + (cells.allCells - cells.aliveCells));
            Console.Write("\n\tПлотность живых клеток: " + ((double)cells.aliveCells / cells.allCells));
            Console.Write("\n\tПлотность живых клеток: " + ((double)(cells.allCells - cells.aliveCells)/cells.allCells));

            life.WriteToFile();
            
            Console.Write("\n\n\tСтабильность на " + (cells.Iters) + " итерации.\n\n");
        }
    }
}
