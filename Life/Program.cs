using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ScottPlot;
using System.Text.Json;
using System.Text.Json.Nodes;

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

        public int CellSize
        {
            get;
            set;
        }

        public double LiveDensity
        {
            get;
            set;
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
        public void LoadFromFile(string file)
        {
            var reader = new StreamReader(file);
            {
                for (int i = 0; i < Rows; i++)
                {
                    string line = reader.ReadLine();
                    for (int j = 0; j < Columns; j++)
                        Cells[j, i].IsAlive = line[j] == '*';
                }
            }
        }
        public int AliveCells()
        {
            int count = 0;
            foreach (Cell cell in Cells)
            {
                if (cell.IsAlive)
                    count++;
            }
            return count;
        }
    }
    public class Program
    {

        static Board board;
        static private void Reset()
        {
            string filename = "settings.json";
            string jsonStr = File.ReadAllText(filename);
            Setting set = JsonSerializer.Deserialize<Setting>(jsonStr);
            board = new Board
                (
                width: set.Width,
                height: set.Height,
                cellSize: set.CellSize,
                liveDensity: set.LiveDensity
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
        static void SaveInFile()
        {
            string filename = "save.txt";
            StreamWriter file = new StreamWriter(filename);

            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    var cell = board.Cells[j, i];
                    if (cell.IsAlive) file.Write('*');
                    else file.Write(' ');
                }
                file.Write('\n');
            }
            file.Close();
        }
        static public void Konstryct(Board boa)
        {
            board = boa;
        }
        static public int CountFigure(string str, string jso)
        {

            string jsonStr = File.ReadAllText(jso);
            Setting set = JsonSerializer.Deserialize<Setting>(jsonStr);
            Board figure = new Board
                (
                width: set.Width,
                height: set.Height,
                cellSize: set.CellSize,
                liveDensity: set.LiveDensity
                );
            int count = 0;
            figure.LoadFromFile(str);
            bool checkFigure = true;
            for (int rowBoard = 0; rowBoard <= board.Rows - figure.Rows; rowBoard++)
            {
                for (int colBoard = 0; colBoard <= board.Columns - figure.Columns; colBoard++)
                {
                    for (int rowFigure = 0; rowFigure < figure.Rows; rowFigure++)
                    {
                        for (int colFigure = 0; colFigure < figure.Columns; colFigure++)
                        {
                            if (board.Cells[colBoard + colFigure, rowBoard + rowFigure].IsAlive != figure.Cells[colFigure, rowFigure].IsAlive)
                            {
                                checkFigure = false;
                                break;
                            }
                        }
                        if (!checkFigure) break;
                    }
                    if (checkFigure)
                    {
                        count++;

                    }
                    checkFigure = true;
                }
            }
            return count;
        }
        static void Main(string[] args)
        {
            ScottPlot.Plot plot= new Plot();
            plot.XLabel("Поколения");
            plot.YLabel("Живые клетки");
            plot.ShowLegend();
            Random rnd = new Random();
            List<double> density = new List<double>() { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
            List<List<int>> X = new List<List<int>>();
            List<List<int>> Y = new List<List<int>>();
            int count = 1;
            for(int i = 0; i < density.Count; i++)
            {
                X.Add(new List<int>());
                Y.Add(new List<int>());
                Board graf = new Board(100, 100, 1, density[i]);
                while (count < 500)
                {
                    X[i].Add(count);
                    Y[i].Add(graf.AliveCells());
                    graf.Advance();
                    count++;
                }
                var scatter = plot.Add.Scatter(X[i], Y[i]);
                scatter.LegendText = density[i].ToString();
                scatter.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                count = 1;
            }
            plot.SavePng("plot.png", 1920, 1080);
            Reset();
            int counter = 1;
            while (true)
            {
                Console.Clear();

                Render();
                Console.WriteLine("Поколение: " + counter);
                Console.WriteLine("Количество живых клеток: " + board.AliveCells());
                Console.WriteLine("Количество Block:" + CountFigure("Block.txt", "setBlock.json"));
                Console.WriteLine("Количество Hive:" + CountFigure("Hive.txt", "setHive.json"));
                Console.WriteLine("Количество Karavai:" + CountFigure("Karavai.txt", "setKaravai.json"));
                Console.WriteLine("Количество Box:" + CountFigure("Box.txt", "setBox.json"));
                Console.WriteLine("Количество Boat:" + CountFigure("Boat.txt", "setBoat.json"));
                Console.WriteLine("Количество Ship:" + CountFigure("Ship.txt", "setShip.json"));
                Console.WriteLine("Количество Canoe:" + CountFigure("Canoe.txt", "setCanoe.json"));
                Console.WriteLine("Количество Integral:" + CountFigure("Integral.txt", "setIntegral.json"));
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.KeyChar == 'q') break;
                    else if (keyInfo.KeyChar == 's') SaveInFile();
                }
                counter++;
                board.Advance();
                Thread.Sleep(1000);
            }
        }
    }
}
