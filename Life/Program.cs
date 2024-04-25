using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using ScottPlot;
using System.Dynamic;
using System.Data;

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
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
        public int RefreshRate { get; set; }
    }

    public class Board
    {
        public readonly Cell[,] Cells;
        public List<string> gens = new List<string>();
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

        public Board(int cellSize, Cell[,] cells)
        {
            CellSize = cellSize;
            Cells = cells;
            ConnectNeighbors();
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            gens.Add(this.RecordInGen(this));
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

        public void SaveToFile(string fileName)//
        {
            using (StreamWriter writer = new StreamWriter($"{fileName}"))
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        if (Cells[i, j].IsAlive)
                        {
                            writer.Write('*');
                        }
                        else
                        {
                            writer.Write(' ');
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        public static Board LoadFromFile(string fileName, int cellSize = 1)
        {
            string[] lines = File.ReadAllLines($"{fileName}");
            Cell[,] newCells = new Cell[lines.Length, lines[0].Length];

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    newCells[i, j] = new Cell { IsAlive = lines[i][j] == '*' };
                }
            }

            return new Board(cellSize, newCells);
        }

        public static Board ResetFromFile(string fileName)
        {
            using (StreamReader r = new StreamReader($"{fileName}"))
            {
                string json = r.ReadToEnd();
                Settings settings = JsonConvert.DeserializeObject<Settings>(json);
                return new Board(
                    width: settings.Width,
                    height: settings.Height,
                    cellSize: settings.CellSize,
                    liveDensity: settings.LiveDensity);
            }
        }

        public int living_cells() {
            int count = 0;
            foreach (Cell c in Cells) {
                if (c.IsAlive) count++;
            }
            return count;
        }

        public string RecordInGen(Board board)
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


    }

    public class Figure
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public int[] value { get; set; }

        public int[,] ReadFigure()
        {
            int[,] mas = new int[Width, Height];
            int n = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    mas[i, j] = value[n];
                    n++;
                }
            }
            return mas;
        }
        public static Figure[] GetFigure(string name)
        {
            string filename = name;
            string jsonString = File.ReadAllText(filename);
            Figure[] figure = JsonConvert.DeserializeObject<Figure[]>(jsonString);
            return figure;
        }
        public static int FindFigure(Figure figure, Board board)
        {
            int count = 0;
            int[,] matrix = new int[figure.Width, figure.Height];
            int[,] fmatrix = figure.ReadFigure();
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    for (int i = 0; i < figure.Height; i++)
                    {
                        for (int j = 0; j < figure.Width; j++)
                        {
                          int x = col + j < board.Columns ? col + j : col + j - board.Columns;
                          int y = row + i < board.Rows ? row + i : row + i - board.Rows;
                          if (board.Cells[x, y].IsAlive)
                                matrix[i, j] = 1;
                          else
                                matrix[i, j] = 0;
                        }
                    }
                    count += CompareFigure(matrix, fmatrix);
                }
            }
            return count;
        }
        static int CompareFigure(int[,] matr1, int[,] matr2)
        {
            int result = 1;
            int rows = matr1.GetUpperBound(0) + 1;
            int columns = matr1.Length / rows;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (matr1[i, j] != matr2[i, j])
                        result = 0;
                }
            }
            return result;
        }
    }

    public class Graphic
    {
        public static Dictionary<int, int> AliveInGeneration(double density)
        {
            var res = new Dictionary<int, int>();
            Board board = new Board(100, 30, 1, density);
            while (true)
            {
                res.Add(board.gens.Count, board.living_cells());
                if (!board.gens.Contains(board.RecordInGen(board)))
                {
                    board.gens.Add(board.RecordInGen(board));
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
                if (density[i] < 0.3 || density[i] > 0.5) break;
                list.Add(AliveInGeneration(density[i]));
            }
            list.Sort((x, y) => x.Count - y.Count);
            return list;
        }
        public static void MakePlot()
        {
            var plot = new Plot();
            plot.XLabel("generation");
            plot.YLabel("alive cells");
            plot.ShowLegend();
            Random rnd = new Random();
            List<double> density = new List<double>() { 0.3, 0.4, 0.5 };
            var list = CreateList(density, density.Count);
            int count = 0;
            foreach (var item in list)
            {
                var scatter = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                scatter.Label = density[count].ToString();
                scatter.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                count++;
            }
            plot.SavePng("plot.png", 1280, 720);
        }
    }
        class Program
        {
        static Board board;
        static private void Reset()
        {
            board = new Board(
                width: 50,
                height: 20,
                cellSize: 1,
                liveDensity: 0.5);
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

            int cells_alive, gen=0;
            bool in_process = true;
            Graphic.MakePlot();
            Reset();
            //boards to load and check ==>
            //board = Board.LoadFromFile("loadboard.txt"); 
            //board = Board.LoadFromFile("loadboard-2.txt");
            //board = Board.LoadFromFile("loadboard-3.txt");
            board = Board.LoadFromFile("research_board.txt");
            while (in_process)
            {
                Console.Clear();
                Render();
                gen++;
                cells_alive = board.living_cells();
                Console.WriteLine("Количество живых клеток: " + cells_alive);
                Console.WriteLine("Поколение: " + gen);
                board.Advance();
                Thread.Sleep(1000);
            }
        }
    }
}
