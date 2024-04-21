using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
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
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Cell[,] Cells;
        public double liveDensity { get; }

        public int generation = 0;
        public int alive = 0;
        public bool stable = false;
        public List<string> states { get; }

        public Board(int width, int height, double liveDensity = .1)
        {
            Width = width;
            Height = height;
            Cells = new Cell[height, width];
            for (int r = 0; r < height; r++)
                for (int c = 0; c < width; c++)
                    Cells[r, c] = new Cell();
            ConnectNeighbors();
            states = new List<string>();
            Randomize(liveDensity);
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
            generation++;
            alive = Cells.Cast<Cell>().Where(x => x.IsAlive).Count();
            string state = "";
            foreach (var cell in Cells)
                state += (cell.IsAlive) ? "*" : " ";
            if (states.Count > 4) states.RemoveAt(0);
            stable = states.Contains(state);
            states.Add(state);
        }
        private void ConnectNeighbors()
        {
            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    int rT = (r > 0) ? r - 1 : Height - 1;
                    int rB = (r < Height - 1) ? r + 1 : 0;
                    int cL = (c > 0) ? c - 1 : Width - 1;
                    int cR = (c < Width - 1) ? c + 1 : 0;
                    Cells[r, c].neighbors.Add(Cells[rT, cL]);
                    Cells[r, c].neighbors.Add(Cells[rT, c]);
                    Cells[r, c].neighbors.Add(Cells[rT, cR]);
                    Cells[r, c].neighbors.Add(Cells[r, cL]);
                    Cells[r, c].neighbors.Add(Cells[r, cR]);
                    Cells[r, c].neighbors.Add(Cells[rB, cL]);
                    Cells[r, c].neighbors.Add(Cells[rB, c]);
                    Cells[r, c].neighbors.Add(Cells[rB, cR]);
                }
            }
        }
        public void SaveTo(string filename)
        {
            string board = "";
            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    board += (Cells[r, c].IsAlive) ? "*" : " ";
                }
                board += "\n";
            }
            board += generation + "\n";
            File.WriteAllText(filename, board);
        }
        public void LoadFrom(string filename)
        {
            string[] file = File.ReadAllLines(filename);
            Height = file.Length - 1;
            Width = file[0].Length;
            Cells = new Cell[Height, Width];
            for (int r = 0; r < Height; r++)
                for (int c = 0; c < Width; c++)
                {
                    Cells[r, c] = new Cell
                    {
                        IsAlive = (file[r][c] == '*')
                    };
                }
            ConnectNeighbors();
            generation = int.Parse(file[^1]);
            alive = Cells.Cast<Cell>().Where(x => x.IsAlive).Count();
        }
    }
    public struct Figure
    {
        public string name;
        public int height;
        public int width;
        public string shape;
    }
    public static class Tools
    {
        
        private static Figure[] figures = new Figure[]
        {
            new Figure { name = "block", height = 4, width = 4, shape = "     **  **     " },
            new Figure { name = "beehive", height = 5, width = 6, shape = "        **   *  *   **        " },
            new Figure { name = "beehive", height = 6, width = 5, shape = "       *   * *  * *   *       " },
            new Figure { name = "boat", height = 5, width = 5, shape = "       *   * *  **       " },
            new Figure { name = "boat", height = 5, width = 5, shape = "      **   * *   *       " },
            new Figure { name = "boat", height = 5, width = 5, shape = "       **  * *   *       " },
            new Figure { name = "boat", height = 5, width = 5, shape = "       *   * *   **      " },
            new Figure { name = "ship", height = 5, width = 5, shape = "       **  * *  **       " },
            new Figure { name = "ship", height = 5, width = 5, shape = "      **   * *   **      " },
            new Figure { name = "tub", height = 5, width = 5, shape = "       *   * *   *       " },
            new Figure { name = "blinker", height = 5, width = 5, shape = "       *    *    *       " },
            new Figure { name = "blinker", height = 5, width = 5, shape = "           ***           " },
            new Figure { name = "loaf", height = 6, width = 6, shape = "        **   *  *   * *    *        " },
            new Figure { name = "loaf", height = 6, width = 6, shape = "        **   *  *  * *    *         " },
            new Figure { name = "loaf", height = 6, width = 6, shape = "         *    * *  *  *   **        " },
            new Figure { name = "loaf", height = 6, width = 6, shape = "        *    * *   *  *   **        " },
            new Figure { name = "pond", height = 6, width = 6, shape = "        **   *  *  *  *   **        " }
        };

        public static Board LoadBoardFrom(string filename)
        {
            using FileStream fstream = new FileStream(filename, FileMode.Open);
            Board board = JsonSerializer.Deserialize<Board>(fstream);
            Console.WriteLine(board.Cells.Length);
            return board;
        }
        public static Dictionary<string, int> CountFigures(Board board)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (Figure f in figures)
            {
                if (!result.Keys.Contains(f.name))
                    result.Add(f.name, 0);
                for (int r = 0; r < board.Height; r++)
                {
                    for (int c = 0; c < board.Width; c++)
                    {
                        if (f.shape == MakeString(board, r, c, f.width, f.height))
                            result[f.name]++;
                    }
                }
            }
            return result;
        }
        public static string MakeString(Board board, int r, int c, int width, int height) {
            string result = "";
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int x = r + h, y = c + w; 
                    if (x >= board.Height)
                        x -= board.Height;
                    if (y >= board.Width)
                        y -= board.Width;
                    result += (board.Cells[x, y].IsAlive) ? "*" : " ";
                }
            }
            return result;
        }
        public static void AverageGraph(double[] densities, int count = 10)
        {
            Plot plt = new Plot();
            plt.Add.Palette = new ScottPlot.Palettes.Nord();
            Dictionary<double, double> result = new Dictionary<double, double>();
            foreach (double density in densities)
            {
                double alive = 0;
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine(density + " " + i);
                    Board board = new Board(100, 50, density);
                    alive += Run(board, every: 1).Count();
                }
                result.Add(density, alive / count);
            }
            foreach (var pair in result)
            {
                plt.Add.Bar(new () { Position=pair.Key, Value=pair.Value, Size=0.05 });
            }
            plt.Axes.Margins(bottom: 0);
            plt.SavePng("plot_average.png", 1000, 1000);
        }
        public static void Graph(double[] densities)
        {
            Plot plt = new Plot();
            plt.Add.Palette = new ScottPlot.Palettes.Nord();
            foreach (double density in densities)
            {
                Console.WriteLine(density);
                Board board = new Board(100, 50, density);
                Dictionary<int, int> result = Run(board);
                Scatter scatter = plt.Add.Scatter(result.Keys.ToArray(), result.Values.ToArray());
                scatter.Label = ""+density;
            }
            plt.ShowLegend(Alignment.UpperLeft);
            plt.Axes.Margins(bottom: 0);
            plt.SavePng("plot.png", 1000, 1000);
        }
        public static Dictionary<int, int> Run(Board board, int every = 15)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            while (!board.stable)
            {
                if (board.generation % every == 0)
                    result.Add(board.generation, board.alive);
                board.Advance();
            }
            return result;
        }
    }
    class Program
    {
        static Board board;
        static private void Reset()
        {
            board = new Board(
                width: 110,
                height: 50,
                liveDensity: 0.5);
        }
        static void Render()
        {
            for (int row = 0; row < board.Height; row++)
            {
                for (int col = 0; col < board.Width; col++)   
                {
                    var cell = board.Cells[row, col];
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
            // double[] densities = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9];
            // Tools.Graph(densities);
            // Tools.AverageGraph(densities);

            board = Tools.LoadBoardFrom("settings.json");
            while (!board.stable)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    char k = key.KeyChar;
                    if (k == 'q')
                        break;
                    else if (k == 's')
                        board.SaveTo("saved.txt");
                    else if (k == 'l')
                        board.LoadFrom("saved.txt");
                    else if (k == 'r')
                        Reset();
                }
                Dictionary<string, int> figures = Tools.CountFigures(board);
                Console.Clear();
                Render();
                Console.WriteLine("generation " + board.generation + " alive " + board.alive);
                foreach (var f in figures)
                    Console.Write(f.Key + " " + f.Value + " ");
                Console.WriteLine();
                board.Advance();
                Thread.Sleep(10);
            }
        }
    }
}