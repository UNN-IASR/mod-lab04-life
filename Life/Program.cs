using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;
using OxyPlot.Wpf;
using ScottPlot;

namespace cli_life
{
    public class Setting
    {
        public int Width 
        { 
            get; 
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public int cellSize
        {
            get;
            set;
        }

        public double liveDensity
        {
            get;
            set;
        }
    }

    public class Figure
    {
        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int[] value
        {
            get;
            set;
        }

        public int[,] readFigure()
        {
            int[,] a = new int[Width, Height];
            int n = 0;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    a[i, j] = value[n];
                    n++;
                }
            }
            return a;
        }

        public static Figure[] getFig(string name)
        {
            string filename = name;
            string jsonStr = File.ReadAllText(filename);
            Figure[] figures = JsonSerializer.Deserialize<Figure[]>(jsonStr);
            return figures;
        }

        public static int findFig(Figure fig, Board board)
        {
            int n = 0;
            int[,] mat = new int[fig.Width, fig.Height];
            int[,] fmat = fig.readFigure();

            for (int r = 0; r < board.Rows; r++)
            {
                for (int c = 0; c < board.Columns; c++)
                {
                    for (int i = 0; i < fig.Height; i++)
                    {
                        for (int j = 0; j < fig.Width; j++)
                        {
                            int X = c + j < board.Columns ? c + j : c + j - board.Columns;
                            int Y = r + i < board.Rows ? r + i : r + i - board.Rows;

                            if (board.Cells[X, Y].IsAlive)
                                mat[i, j] = 1;
                            else
                                mat[i, j] = 0;
                        }
                    }
                    n += compareFig(mat, fmat);
                }
            }
            return n;
        }

        static int compareFig(int [,] mat1, int[,] mat2)
        {
            int res = 1;
            int r = mat1.GetUpperBound(0) + 1;
            int c = mat1.Length / r;

            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    if (mat1[i, j] != mat2[i, j])
                        res = 0;
                }
            }
            return res;
        }
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
        public  Cell[,] Cells;
        public readonly int CellSize;
        public List<string> poses = new List<string>();

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

        public int cellAliveCount()
        {
            int n = 0;

            foreach (Cell c in Cells)
            {
                if (c.IsAlive)
                    n++;
            }
            return n;
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

        public string RecordPosition(Board board)
        {
            string str = "";

            foreach (Cell cell in board.Cells)
            {
                if (cell.IsAlive)
                    str += '1';
                else
                    str += '0';
            }
            return str;
        }

        public void Upload(string name)
        {
            string[] str = File.ReadAllLines(name);
            Cell[,] newCell = new Cell[Columns, Rows];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (str[i][j] == '1')
                        newCell[i, j] = new Cell { IsAlive = true };
                    if (str[i][j] == '0')
                        newCell[i, j] = new Cell { IsAlive = false };
                }
            }
            Cells = newCell;
            ConnectNeighbors();
        }
    }

    public class Grafic
    {
        public static Dictionary<int, int> aliveInGen(double den)
        {
            var res = new Dictionary<int, int>();
            Board board = new Board(100, 30, 1, den);
            
            while (true)
            {
                res.Add(board.poses.Count, board.cellAliveCount());

                if (!board.poses.Contains(board.RecordPosition(board)))
                {
                    board.poses.Add(board.RecordPosition(board));
                }
                else
                {
                    break;
                }
                board.Advance();
            }
            return res;
        }

        public static List<Dictionary<int, int>> createList(List<double> den, int n)
        {
            var list = new List<Dictionary<int, int>>();

            for (int i = 0; i < n; i++)
            {
                if (den[i] < 0.3 || den[i] > 0.5)
                    break;
                list.Add(aliveInGen(den[i]));
            }
            list.Sort((x, y) => x.Count - y.Count);
            return list;
        }

        public static void graf()
        {
            var p = new Plot();
            p.XLabel("gen");
            p.YLabel("alive cells");
            p.ShowLegend();

            Random random = new Random();
            List<double> den = new List<double>() { 0.3, 0.4, 0.5};
            var list = createList(den, den.Count);
            int n = 0;

            foreach (var i in list)
            {
                var scat = p.Add.Scatter(i.Keys.ToArray(), i.Values.ToArray());
                scat.Label = den[n].ToString();
                scat.Color = new ScottPlot.Color(random.Next(256), random.Next(256), random.Next(256));
                n++;
            }
            p.SavePng("plot.png", 1920, 1080);
        }
    }
    class Program
    {
        static Board board;
        static private void Reset()
        {
            string filename = "config.json";
            string jsonStr = File.ReadAllText(filename);
            Setting set = JsonSerializer.Deserialize<Setting>(jsonStr);
            board = new Board
                (
                width: set.Width,
                height: set.Height,
                cellSize: set.cellSize,
                liveDensity: set.liveDensity
                );
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

        static void save()
        {
            string filename = "saveBoard.txt";
            StreamWriter streamWriter = new StreamWriter(filename);

            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    var cell = board.Cells[i, j];
                    if (cell.IsAlive)
                    {
                        streamWriter.Write('1');
                    }
                    else
                    {
                        streamWriter.Write('0');
                    }
                }
                streamWriter.Write('\n');
            }
            streamWriter.Close();
        }
        static void Main(string[] args)
        {
            Grafic.graf();
            Reset();
            Figure[] figure = Figure.getFig("fig.json");
            string name;
            int n = 0;
            int a = 0;
            int countPoses = 0;
            bool f = true;
            while (f)
            {
                Render();
                a = board.cellAliveCount();
                Console.WriteLine("Кол-во живых клеток: " + a);

                for (int i = 0; i < figure.Length; i++)
                {
                    name = figure[i].Name;
                    n = Figure.findFig(figure[i], board);
                    Console.WriteLine(name + " " + n);
                }

                if (!board.poses.Contains(board.RecordPosition(board)))
                    board.poses.Add(board.RecordPosition(board));
                else
                    f = false;

                countPoses = board.poses.Count;
                Console.WriteLine("Кол-во поколений: " + countPoses);
                board.Advance();
                Thread.Sleep(1000);
            }
        }
    }
}
