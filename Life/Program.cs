using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;
using System.Numerics;
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

        public static Figure[] get_figure(string name)
        {
            string fname = name;
            string str = File.ReadAllText(fname);
            Figure[] figs = JsonSerializer.Deserialize<Figure[]>(str);
            return figs;
        }

        public static int find_figure(Figure fig, Board boar)
        {
            int num = 0;
            int[,] matr = new int[fig.Width, fig.Height];
            int[,] fmatr = fig.readFigure();

            for (int ro = 0; ro < boar.Rows; ro++)
            {
                for (int co = 0; co < boar.Columns; co++)
                {
                    for (int ind = 0; ind < fig.Height; ind++)
                    {
                        for (int i = 0; i < fig.Width; i++)
                        {
                            int X = co + i < boar.Columns ? co + i : co + i - boar.Columns;
                            int Y = ro + ind < boar.Rows ? ro + ind : ro + ind - boar.Rows;

                            if (boar.Cells[X, Y].IsAlive)
                                matr[ind, i] = 1;
                            else
                                matr[ind, i] = 0;
                        }
                    }
                    num += compare_fig(matr, fmatr);
                }
            }
            return num;
        }

        static int compare_fig(int[,] mat_1, int[,] mat_2)
        {
            int resul = 1;
            int ro = mat_1.GetUpperBound(0) + 1;
            int co = mat_1.Length / ro;

            for (int ind = 0; ind < ro; ind++)
            {
                for (int i = 0; i < co; i++)
                {
                    if (mat_1[ind, i] != mat_2[ind, i])
                        resul = 0;
                }
            }
            return resul;
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
        public Cell[,] Cells;
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

        public void Upload(string fname)
        {
            string[] s = File.ReadAllLines(fname);
            Cell[,] newCell = new Cell[Columns, Rows];

            for (int ro = 0; ro < Rows; ro++)
            {
                for (int co = 0; co < Columns; co++)
                {
                    if (s[ro][co] == '1')
                        newCell[ro, co] = new Cell { IsAlive = true };
                    if (s[ro][co] == '0')
                        newCell[ro, co] = new Cell { IsAlive = false };
                }
            }
            Cells = newCell;
            ConnectNeighbors();
        }
    }

    public class Grafic
    {
        public static Dictionary<int, int> alive_in_generat(double d)
        {
            var resul = new Dictionary<int, int>();
            Board boar = new Board(100, 30, 1, d);

            while (true)
            {
                resul.Add(boar.poses.Count, boar.cellAliveCount());

                if (!boar.poses.Contains(boar.RecordPosition(boar)))
                {
                    boar.poses.Add(boar.RecordPosition(boar));
                }
                else
                {
                    break;
                }
                boar.Advance();
            }
            return resul;
        }

        public static List<Dictionary<int, int>> create_list(List<double> d, int num)
        {
            var lis = new List<Dictionary<int, int>>();

            for (int ind = 0; ind < num; ind++)
            {
                if (d[ind] < 0.3 || d[ind] > 0.5)
                    break;
                lis.Add(alive_in_generat(d[ind]));
            }
            lis.Sort((x, y) => x.Count - y.Count);
            return lis;
        }
        public static void grafic()
        {
            var p = new Plot();
            p.XLabel("generation");
            p.YLabel("alive cells");
            p.ShowLegend();
            Random rnd = new Random();
            List<double> density = new List<double>() { 0.3, 0.4, 0.5 };
            var lis = create_list(density, density.Count);
            int num = 0;
            foreach (var item in lis)
            {
                var scatter = p.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                scatter.Label = density[num].ToString();
                scatter.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                num++;
            }
            p.SavePng("plot.png", 1920, 1080);
        }
    }
    class Program
    {
        static Board boar;
        static private void Reset()
        {
            string fname = "configuration.json";
            string str = File.ReadAllText(fname);
            Setting set = JsonSerializer.Deserialize<Setting>(str);
            boar = new Board
                (
                width: set.Width,
                height: set.Height,
                cellSize: set.cellSize,
                liveDensity: set.liveDensity
                );
        }
        static void Render()
        {
            for (int ro = 0; ro < boar.Rows; ro++)
            {
                for (int co = 0; co < boar.Columns; co++)
                {
                    var cell = boar.Cells[co, ro];
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
            string fname = "saveBoard.txt";
            StreamWriter streamWr = new StreamWriter(fname);

            for (int ro = 0; ro < boar.Rows; ro++)
            {
                for (int co = 0; co < boar.Columns; co++)
                {
                    var cell = boar.Cells[ro, co];
                    if (cell.IsAlive)
                    {
                        streamWr.Write('1');
                    }
                    else
                    {
                        streamWr.Write('0');
                    }
                }
                streamWr.Write('\n');
            }
            streamWr.Close();
        }
        static void Main(string[] args)
        {
            Grafic.grafic();
            Reset();
            Figure[] fig = Figure.get_figure("figures.json");
            string name;
            int num = 0;
            int c = 0;
            int count = 0;
            bool fl = true;
            while (fl)
            {
                Render();
                c = boar.cellAliveCount();
                Console.WriteLine("Кол-во живых клеток: " + c);

                for (int i = 0; i < fig.Length; i++)
                {
                    name = fig[i].Name;
                    num = Figure.find_figure(fig[i], boar);
                    Console.WriteLine(name + " " + num);
                }

                if (!boar.poses.Contains(boar.RecordPosition(boar)))
                    boar.poses.Add(boar.RecordPosition(boar));
                else
                    fl = false;

                count = boar.poses.Count;
                Console.WriteLine("Кол-во поколений: " + count);
                boar.Advance();
                Thread.Sleep(1000);
            }
        }
    }
}
