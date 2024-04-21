using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using System.Xml.Schema;
using ScottPlot;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using ScottPlot.Colormaps;

namespace cli_life
{
    public class Settings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
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
    public class Figure
    {
        public string name;
        public int length;
        public int width;
        public string type_figure;
        public Figure(string _name, int _length, int _width, string _type_figure)
        {
            name = _name;
            length = _length;
            width = _width;
            type_figure = _type_figure;
        }
        public int find_countfile(Figure figure, Board board, string name)
        {
            int count = 0;
            string str1 = "";
            string str2 = File.ReadAllText(name);
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    for (int i = 0; i < figure.length; i++)
                    {
                        for (int j = 0; j < figure.width; j++)
                        {
                            int x = col + j < board.Columns ? col + j : col + j - board.Columns;
                            int y = row + i < board.Rows ? row + i : row + i - board.Rows;
                            if (board.Cells[x, y].IsAlive)
                            {
                                str1 += "1";
                            }
                            else
                            {
                                str1 += "0";
                            }
                        }
                    }
                    if (str1 == str2)
                    {
                        count++;
                    }
                    //Console.WriteLine(str1);
                    str1 = "";
                }
            }
            return count;
        }
    }
    public class Board
    {
        public Cell[,] Cells;
        public int CellSize { get; }

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public double LiveDensity { get; }
        public List<string> gen1 = new List<string>();

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
        public void loadfromfile(string name)
        {
            StreamReader streamReader = new StreamReader(name);
            for (int row = 0; row < Rows; row++)
            {
                string line = streamReader.ReadLine();
                for (int col = 0; col < Columns; col++)
                {
                    if (line[col] == '*')
                    {
                        Cells[col, row].IsAlive = true;
                    }
                    if (line[col] == ' ')
                    {
                        Cells[col, row].IsAlive = false;
                    }
                }
            }
        }
        public int Check_count_live()
        {
            int count = 0;
            foreach (Cell cell in Cells)
            {
                if (cell.IsAlive)
                {
                    count += 1;
                }
            }
            return count;
        }

        public string WriteGen(Board board)
        {
            string str = "";
            foreach (Cell cell in board.Cells)
            {
                if (cell.IsAlive)
                    str += '*';
                else
                    str += ' ';
            }
            return str;
        }
    }
    public class Graph
    {
        private static Dictionary<int, int> CountAlive(double density)
        {
            var res = new Dictionary<int, int>();
            Board board = new Board(50, 20, 1, density);
            int gen = 0;
            while (true)
            {
                if (gen % 20 == 0) res.Add(gen, board.Check_count_live());
                if (!board.gen1.Contains(board.WriteGen(board)))
                {
                    board.gen1.Add(board.WriteGen(board));
                }
                else
                {
                    break;
                }
                board.Advance();
                gen++;
            }
            return res;
        }
        private static List<Dictionary<int, int>> CreateList(List<double> density, int count)
        {
            var list = new List<Dictionary<int, int>>();
            for (int i = 0; i < count; i++)
            {
                if (density[i] < 0.3 || density[i] > 0.7) break;
                list.Add(CountAlive(density[i]));
            }
            list.Sort((x, y) => x.Count - y.Count);
            return list;
        }
        public static void CreateGraph()
        {
            var plot = new Plot();
            plot.XLabel("Generation");
            plot.YLabel("Alive cells");
            plot.ShowLegend();
            Random rand = new Random();
            List<double> density = new List<double>() { 0.3, 0.4, 0.5, 0.6, 0.7 };
            var list = CreateList(density, density.Count);
            int count = 0;
            foreach (var item in list)
            {
                var scatter = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                scatter.Label = density[count].ToString();
                scatter.Color = new ScottPlot.Color(rand.Next(256), rand.Next(256), rand.Next(256));
                count++;
            }
            plot.SavePng("plot.png", 1920, 1080);
        }
    }
    class Program
    {

        static public Board Reset(string filePath)
        {

            using FileStream fstream = new FileStream(filePath, FileMode.OpenOrCreate);
            return JsonSerializer.Deserialize<Board>(fstream);
        }
        static Board board = Reset("file.json");
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
        static void Save()
        {
            string name = "res.txt";
            StreamWriter streamWriter = new StreamWriter(name);
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
            Graph.CreateGraph();
            Figure box = new Figure("Box", 4, 4, "0000011001100000");
            Figure hive = new Figure("Hive", 6, 5, "000000010001010010100010000000");
            Figure ship = new Figure("Ship", 5, 5, "0000001100010100011000000");
            Figure snake = new Figure("Snake", 4, 6, "000000010110011010000000");
            bool k = true;
            while (k)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.KeyChar == 'q')
                        k = false;
                    else if (key.KeyChar == 's')
                        Save();
                    else if (key.KeyChar == 'u')
                        board.loadfromfile("res.txt");
                    //board.loadfromfile("snake6.txt");
                    // board.loadfromfile("boxs.txt");
                    //board.loadfromfile("hives.txt");
                    // board.loadfromfile("ships.txt");

                }
                Console.Clear();
                Render();
                Console.WriteLine("A live-" + board.Check_count_live().ToString());
                Console.WriteLine("Box" + " " + box.find_countfile(box, board, "box.txt").ToString());
                Console.WriteLine("Hive" + " " + hive.find_countfile(hive, board, "hive.txt").ToString());
                Console.WriteLine("Ship" + " " + ship.find_countfile(ship, board, "ship.txt").ToString());
                Console.WriteLine("Snake" + " " + snake.find_countfile(snake, board, "snake.txt").ToString());
                if (!board.gen1.Contains(board.WriteGen(board)))
                {
                    board.gen1.Add(board.WriteGen(board));
                }
                board.Advance();
                Thread.Sleep(500);
            }
        }
    }
}