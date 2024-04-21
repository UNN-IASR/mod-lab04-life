using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using ScottPlot;
using ScottPlot.Plottables;

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

        public int gen;
        public List<string> states; 

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);

            states = new List<string>();
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
            gen++;
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
        public int Alive() {
            int count = 0;
            foreach (var cell in Cells)
            {
                if (cell.IsAlive) count++;
            }
            return count;
        }
        public bool IsStable() {
            string state = "";
            foreach (var cell in Cells)
            {
                if (cell.IsAlive) state += "*";
                else state += " ";
            }
            bool res = states.Contains(state);
            states.Add(state);
            if (states.Count > 5) states.RemoveAt(0);
            return res;
        }
    }
    public struct Params
    {
        public int width;
        public int height;
        public int cellSize;
        public double liveDensity;
    }
    public struct Figure
    {
        public string name;
        public int width;
        public int height;
        public int[] fig;
        public int count;
    }
    public class Program
    {
        public static Board board;
        public static void Reset()
        {
            string s = File.ReadAllText("params.json");
            var options = new JsonSerializerOptions { IncludeFields = true };
            Params p = JsonSerializer.Deserialize<Params>(s, options);
            board = new Board(p.width, p.height, p.cellSize, p.liveDensity);
        }
        public static void Render()
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
        public static void Save(string file)
        {
            using StreamWriter writer = new StreamWriter(file);
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                        writer.Write('*');
                    else
                    {
                        writer.Write(' ');
                    }
                }
                writer.Write('\n');
            }
        }
        public static Board Load(string file)
        {
            using StreamReader reader = new StreamReader(file);
            string text = reader.ReadToEnd();
            if (text.Length != board.Rows * (board.Columns + 1))
            {
                Console.WriteLine($"incorrect file {board.Rows} {board.Columns} {text.Length}");
                Thread.Sleep(2000);
                return board;
            }
            int i = 0;
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    char c = text[i++];
                    if (c == '\n') c = text[i++];
                    if (c == '*') 
                        board.Cells[col, row].IsAlive = true;
                    else
                        board.Cells[col, row].IsAlive = false;
                }
            }
            return board;
        }
        public static void FindFigures(Figure[] figures) {
            for (int f = 0; f < figures.Length; f++) figures[f].count = 0;
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    for (int f = 0; f < figures.Length; f++)
                    {
                        Figure fig = figures[f];
                        int ok = 0;
                        for (int w = 0; w < fig.width; w++)
                        {
                            for (int h = 0; h < fig.height; h++)
                            {
                                int c = col+w, r = row+h;
                                if (c >= board.Columns) c -= board.Columns;
                                if (r >= board.Rows) r -= board.Rows;
                                bool alive = board.Cells[c,r].IsAlive;
                                int num = fig.fig[h*fig.width+w];
                                if (alive && num == 1) ok++;
                                if (!alive && num == 0) ok++;
                            }
                        }
                        if (ok == fig.width * fig.height) figures[f].count++;
                    }
                }
            }
        }
        public static void Graph()
        {
            Plot graph = new Plot();
            graph.XLabel("Generation");
            graph.YLabel("Alive");
            graph.ShowLegend();
            double[] dens = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9];
            foreach (double d in dens)
            {
                Board b = new Board(100, 30, 1, d);
                Dictionary<int, int> dict = new Dictionary<int, int>();
                while (!b.IsStable())
                {
                    if (b.gen % 20 == 0)
                        dict.Add(b.gen, b.Alive());
                    b.Advance();
                }
                Scatter sc = graph.Add.Scatter(dict.Keys.ToArray(), dict.Values.ToArray());
                sc.Label = Convert.ToString(d);
                Random r = new Random();
                sc.Color = new Color(r.Next(255), r.Next(255), r.Next(255));
            }
            graph.SavePng("graph.png", 1920, 1080);
        }
        static void Main(string[] args)
        {
            Reset();

            Figure[] figures = new Figure[]
            {
                new Figure {name = "block", width = 4, height = 4, fig = [0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0]},
                new Figure {name = "beehive_h", width = 6, height = 5, fig = [0,0,0,0,0,0,0,0,1,1,0,0,0,1,0,0,1,0,0,0,1,1,0,0,0,0,0,0,0,0]},
                new Figure {name = "beehive_v", width = 5, height = 6, fig = [0,0,0,0,0,0,0,1,0,0,0,1,0,1,0,0,1,0,1,0,0,0,1,0,0,0,0,0,0,0]},
                new Figure {name = "ship_1", width = 5, height = 5, fig = [0,0,0,0,0,0,0,1,1,0,0,1,0,1,0,0,1,1,0,0,0,0,0,0,0]},
                new Figure {name = "ship_1", width = 5, height = 5, fig = [0,0,0,0,0,0,1,1,0,0,0,1,0,1,0,0,0,1,1,0,0,0,0,0,0]},
            };
            Figure[] loafs = new Figure[]
            {
                new Figure {name = "loaf_1", width = 6, height = 6, fig = [0,0,0,0,0,0,0,0,1,1,0,0,0,1,0,0,1,0,0,0,1,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0]},
                new Figure {name = "loaf_2", width = 6, height = 6, fig = [0,0,0,0,0,0,0,0,1,1,0,0,0,1,0,0,1,0,0,1,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0]},
                new Figure {name = "loaf_3", width = 6, height = 6, fig = [0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,1,0,0,1,0,0,1,0,0,0,1,1,0,0,0,0,0,0,0,0]},
                new Figure {name = "loaf_4", width = 6, height = 6, fig = [0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,1,0,0,0,1,0,0,1,0,0,0,1,1,0,0,0,0,0,0,0,0]},
            };
            Figure[] boats = new Figure[]
            {
                new Figure {name = "boat_1", width = 5, height = 5, fig = [0,0,0,0,0,0,0,1,0,0,0,1,0,1,0,0,1,1,0,0,0,0,0,0,0]},
                new Figure {name = "boat_2", width = 5, height = 5, fig = [0,0,0,0,0,0,1,1,0,0,0,1,0,1,0,0,0,1,0,0,0,0,0,0,0]},
                new Figure {name = "boat_3", width = 5, height = 5, fig = [0,0,0,0,0,0,0,1,1,0,0,1,0,1,0,0,0,1,0,0,0,0,0,0,0]},
                new Figure {name = "boat_4", width = 5, height = 5, fig = [0,0,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,1,1,0,0,0,0,0,0]},
            };

            // Load("board.txt");
            
            while(true)
            {
                FindFigures(figures);
                FindFigures(loafs);
                FindFigures(boats);
                Console.Clear();
                Render();
                Console.WriteLine("Generation " + board.gen);

                foreach (Figure fig in figures)
                    Console.Write(" " + fig.name + " " + fig.count);
                Console.WriteLine();
                foreach (Figure fig in loafs)
                    Console.Write(" " + fig.name + " " + fig.count);
                Console.WriteLine();
                foreach (Figure fig in boats)
                    Console.Write(" " + fig.name + " " + fig.count);
                Console.WriteLine();
                if (board.IsStable()) break;
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    char k = key.KeyChar;
                    if (k == 'q')
                        break;
                    else if (k == 's')
                        Save("board.txt");
                    else if (k == 'l')
                        Load("board.txt");
                    else if (k == 'r')
                        Reset();
                }
                board.Advance();
                Thread.Sleep(10);
            }
            Save("stableboard");
        }
    }
}