using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Numerics;
using ScottPlot;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using cli_life;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System;
using System.Linq;

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
    public class Settings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int cellSize { get; set; }
        public double liveDensity { get; set; }
    }
    public class Figure
    {
        public int size;
        public string figura;

        public Figure(int size_, string type_)
        {
            size = size_;
            figura = type_;
        }
        public int find_figures(Figure figure, Board board, string name_figure)
        {
            int count = 0;
            int c = 0;
            string str = "";
            string desired_figure = File.ReadAllText(name_figure);
            //Console.WriteLine(desired_figure);
            // Console.WriteLine('\n');
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    for (int i = 0; i < figure.size; i++)
                    {
                        for (int j = 0; j < figure.size; j++)
                        {
                            int x = col + j < board.Columns ? col + j : col + j - board.Columns;
                            int y = row + i < board.Rows ? row + i : row + i - board.Rows;
                            if (board.Cells[x, y].IsAlive)
                                str += 1;
                            else
                                str += 0;
                        }
                    }
                    //Console.WriteLine(str);
                    //Console.WriteLine('\n');
                    if (Equals(str, desired_figure))
                    {
                        c++;
                    }
                    str = "";

                }
            }
            return c;
        }
    }
    public class Board
    {
        public Cell[,] Cells;
        public List<string> positions = new List<string>();
        public readonly int CellSize;

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

        public void start_board(string name_file)
        {
            string[] string_ = File.ReadAllLines(name_file);
            Cell[,] new_cells = new Cell[Columns, Rows];
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (string_[row][col] == '*')
                    {
                        new_cells[col, row] = new Cell { IsAlive = true };
                    }
                    if (string_[row][col] == ' ')
                    {
                        new_cells[col, row] = new Cell { IsAlive = false };
                    }

                }
            }
            Cells = new_cells;
            ConnectNeighbors();
        }

        public int count_live_cell()
        {
            int count = 0;
            foreach (Cell cell in Cells)
            {
                if (cell.IsAlive)
                {
                    count++;
                }
            }
            return count;
        }
        public string save_board(Board board)
        {
            string str = "";
            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    if (Cells[i, j].IsAlive)
                        str += '*';
                    else
                        str += ' ';
                }
            }
            return str;

        }
    }
    public class CrateGraph
    {
        public static Dictionary<int, int> AliveInGeneration(double density)
        {
            var res = new Dictionary<int, int>();
            Board board = new Board(50, 20, 1, density);
            int gen = 0;
            while (true)
            {
                if (gen % 10 == 0)
                {
                    res.Add(board.positions.Count, board.count_live_cell());
                }
                if (!board.positions.Contains(board.save_board(board)))
                {
                    board.positions.Add(board.save_board(board));
                }
                else
                {
                    break;
                }
                board.Advance();
            }
            return res;
        }
        public static List<Dictionary<int, int>> CreateList(List<double> density, int count)
        {
            var list = new List<Dictionary<int, int>>();
            for (int i = 0; i < count; i++)
            {
                if (density[i] < 0.3 || density[i] > 0.7) break;
                list.Add(AliveInGeneration(density[i]));
            }
            list.Sort((x, y) => x.Count - y.Count);
            return list;
        }
        public static void GraphCrate()
        {
            var plot = new Plot();
            plot.XLabel("Generation");
            plot.YLabel("Count live cells");
            plot.ShowLegend();
            Random rnd = new Random();
            List<double> density = new List<double>() { 0.3, 0.4, 0.5, 0.6, 0.7 };
            var list = CreateList(density, density.Count);
            int count = 0;
            foreach (var item in list)
            {
                var scatter = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                scatter.Label = density[count].ToString();
                scatter.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                count++;
            }
            plot.SavePng("plot.png", 1920, 1080);
        }
    }
    class Program
    {
        static Board board;
        static private void Reset()
        {
            string filename = "config1.json";
            string jsonString = File.ReadAllText(filename);
            Settings settings = JsonSerializer.Deserialize<Settings>(jsonString);
            board = new Board(width: settings.Width, height: settings.Height, cellSize: settings.cellSize, liveDensity: settings.liveDensity);

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

        static void save(int number)
        {
            StreamWriter streamWriter = new StreamWriter("save_board" + number.ToString() + ".txt");
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        streamWriter.Write('*');
                    }
                    else
                    {
                        streamWriter.Write(' ');
                    }
                }
                streamWriter.Write('\n');
            }
            streamWriter.Close();
        }
        static void Main(string[] args)
        {
            CrateGraph.GraphCrate();
            Reset();
            int number = 1;
            bool stop = true;
            List<string> figures = new List<string>() { "Block.txt", "Tub.txt", "Loaf.txt", "Barge.txt" };
            List<int> sizes = new List<int>() { 4, 5, 6, 6 };
            Figure figure1 = new Figure(sizes[0], figures[0]);
            Figure figure2 = new Figure(sizes[1], figures[1]);
            Figure figure3 = new Figure(sizes[2], figures[2]);
            Figure figure4 = new Figure(sizes[3], figures[3]);
            while (stop)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.KeyChar == 'w')
                        stop = false;
                    else if (key.KeyChar == 'a')
                    {
                        save(number);
                        number++;
                    }
                    else if (key.KeyChar == 'd')
                        board.start_board("start_board.txt");

                }
                Console.Clear();
                Render();
                int count_live_cell = board.count_live_cell();
                Console.WriteLine("Количество живых клеток " + count_live_cell);

                int count1 = figure1.find_figures(figure1, board, figures[0]);
                Console.WriteLine("Block: " + count1);

                int count2 = figure2.find_figures(figure2, board, figures[1]);
                Console.WriteLine("Tub: " + count2);

                int count3 = figure3.find_figures(figure3, board, figures[2]);
                Console.WriteLine("Loaf: " + count3);

                int count4 = figure4.find_figures(figure4, board, figures[3]);
                Console.WriteLine("Barge: " + count4);
                count1 = 0;
                count2 = 0;
                count3 = 0;
                count4 = 0;

                if (!board.positions.Contains(board.save_board(board)))
                    board.positions.Add(board.save_board(board));
                else
                    stop = false;
                int CountPositions = 0;
                CountPositions = board.positions.Count;
                Console.WriteLine("Количество поколений " + CountPositions);

                board.Advance();
                Thread.Sleep(1);
            }

        }
    }

}