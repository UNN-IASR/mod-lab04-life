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

namespace cli_life
{
    public class Cell
    {
        private readonly List<Cell> neighbors;
        private bool isAlive;
        private bool isAliveNext;

        public Cell(bool isAliveStart)
        {
            neighbors = [];
            isAlive = isAliveStart;
        }

        public bool IsAlive => isAlive;

        public void Load(char str) => isAlive = str == '*';
        public void AddNeighbor(Cell cell) => neighbors.Add(cell);
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                isAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                isAliveNext = liveNeighbors == 3;
        }
        public void Advance() => isAlive = isAliveNext;

    }

    public class Board
    {
        private readonly Cell[,] cells;
        private readonly List<string> states;
        private readonly Random rand = new();

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;
            states = [];
            cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    cells[x, y] = new Cell(rand.NextDouble() < liveDensity);

            ConnectNeighbors();
        }

        public int CellSize { get; }
        public int Columns => cells.GetLength(0);
        public int Rows => cells.GetLength(1);
        public int Width => Columns * CellSize;
        public int Height => Rows * CellSize;
        public double LiveDensity { get; }
        public Cell GetCell(int x, int y) => cells[x, y];

        public void Advance()
        {
            foreach (var cell in cells)
                cell.DetermineNextLiveState();
            foreach (var cell in cells)
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

                    cells[x, y].AddNeighbor(cells[xL, yT]);
                    cells[x, y].AddNeighbor(cells[x, yT]);
                    cells[x, y].AddNeighbor(cells[xR, yT]);
                    cells[x, y].AddNeighbor(cells[xL, y]);
                    cells[x, y].AddNeighbor(cells[xR, y]);
                    cells[x, y].AddNeighbor(cells[xL, yB]);
                    cells[x, y].AddNeighbor(cells[x, yB]);
                    cells[x, y].AddNeighbor(cells[xR, yB]);
                }
            }
        }
        public void Load(string path)
        {
            StreamReader reader = new(path);
            for (int row = 0; row < Rows; row++)
            {
                string line = reader.ReadLine();
                for (int col = 0; col < Columns; col++)
                {
                    cells[col, row].Load(line[col]);
                }
            }
        }
        public int FindPattern(Figure pattern)
        {
            int result = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (CheckPattern(pattern, row, col))
                    {
                        result++;
                    }
                }
            }
            return result;
        }
        public int CheckAlive()
        {
            int result = 0;
            foreach (Cell cell in cells)
            {
                result += cell.IsAlive ? 1 : 0;
            }
            return result;
        }
        public bool CheckStable()
        {
            string str = "";
            foreach (Cell cell in cells)
            {
                str += cell.IsAlive ? "*" : " ";
            }
            states.Add(str);
            if (states.Count > 3)
            {
                states.RemoveAt(0);
            }
            return states.Take(states.Count - 1).Contains(str);
        }
        private bool CheckPattern(Figure pattern, int row, int col)
        {
            for (int i = 0; i < pattern.Size; i++)
            {
                for (int j = 0; j < pattern.Size; j++)
                {
                    int x = col + j < Columns ? col + j : j - 1;
                    int y = row + i < Rows ? row + i : i - 1;
                    if ((cells[x, y].IsAlive && pattern.Image[i * pattern.Size + j] == '.') || (!cells[x, y].IsAlive && pattern.Image[i * pattern.Size + j] == '*'))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
    public class Figure
    {
        public Figure(string _image, int _size)
        {
            Image = _image;
            Size = _size;
        }
        public string Image { get; }
        public int Count { get; }
        public int Size { get; }
    }
    public class Graph
    {
        public static void CreateGraph()
        {
            Plot plot = new();
            plot.XLabel("Generation");
            plot.YLabel("Alive cells");
            plot.ShowLegend();
            List<double> density = [0.3, 0.4, 0.5];
            int count = 0;
            foreach (var item in CreateList(density, density.Count))
            {
                var scatter = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                scatter.Label = density[count].ToString();
                scatter.Color = new ScottPlot.Color(new Random().Next(256), new Random().Next(256), new Random().Next(256));
                count++;
            }
            plot.SavePng("plot.png", 1920, 1080);
        }
        private static List<Dictionary<int, int>> CreateList(List<double> density, int count)
        {
            List<Dictionary<int, int>> list = [];
            for (int i = 0; i < count; i++)
            {
                if (density[i] < 0.2 || density[i] > 0.8)
                {
                    break;
                }
                list.Add(CountAlive(density[i]));
            }
            list.Sort((x, y) => x.Count - y.Count);
            return list;
        }
        private static Dictionary<int, int> CountAlive(double density)
        {
            Dictionary<int, int> result = [];
            Board board = new(100, 20, 1, density);
            int countGen = 0;
            while (true)
            {
                if (countGen % 20 == 0)
                {
                    result.Add(countGen, board.CheckAlive());
                }
                if (board.CheckStable())
                {
                    break;
                }
                board.Advance();
                countGen++;
            }
            return result;
        }
    }
    public class Program
    {
        private static Board board;

        public Program()
        {
            board = Reset("../../../../Life/settings.json");

        }

        public static Board Reset(string filePath) => JsonSerializer.Deserialize<Board>(new FileStream(filePath, FileMode.OpenOrCreate));
        public static void Main(string[] args)
        {
        }
        private static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    if (board.GetCell(col, row).IsAlive)
                    {
                        Console.Write('*');
                        continue;
                    }
                    Console.Write(' ');
                }
                Console.Write('\n');
            }
        }
        private static void Save(int counter)
        {
            StreamWriter writer = new("state" + counter.ToString() + ".txt");
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    if (board.GetCell(col, row).IsAlive)
                    {
                        writer.Write('*');
                        continue;
                    }
                    writer.Write(' ');
                }
                writer.Write('\n');
            }
        }
    }
}
