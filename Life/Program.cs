using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        public bool IsAliveNext;
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
        public List<string> poses = new List<string>();

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1, bool useState = false)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();
            ConnectNeighbors();

            if (useState)
            {
                ReadFromFile("../../../state.txt");
            }
            else
            {
                Randomize(liveDensity);
            }
        }

        public int AliveCellsCount()
        {
            int result = 0;
            foreach (Cell c in Cells)
            {
                if (c.IsAlive)
                {
                    result++;
                }
            }
            return result;
        }

        public string RecordPosition(Board board)
        {
            string str = "";

            foreach (Cell cell in board.Cells)
            {
                if (cell.IsAlive)
                {
                    str += '1';
                }
                else
                {
                    str += '0';
                } 
            }
            return str;
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
        public void ReadFromFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
                for (int row = 0; row < Rows; row++)
                {
                    var line = reader.ReadLine();
                    var split = line.ToCharArray();
                    for (int col = 0; col < Columns; col++)
                    {
                        var cell = Cells[col, row];
                        cell.IsAlive = split[col].Equals('*');
                    }
                }
        }
    }
    public class Program
    {
        static Board board;
        static private void Reset(string filePath)
        {
            var settings = File.ReadAllText(filePath);
            board = CreateBoardWithSettings(settings);
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
        static void WriteToFile()
        {
            using (StreamWriter streamWriter = File.CreateText("../../../state.txt"))
            {
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
                    streamWriter.WriteLine();
                }
            }
        }
        public static Board CreateBoardWithSettings(string settings)
        {
            Board board = JsonConvert.DeserializeObject<Board>(settings);
            return board;
        }
        static void Main(string[] args)
        {
            Graph.Create();
            while (true)
            {
                Reset("../../../settings.json");
                Console.Clear();
                Render();
                board.Advance();
                WriteToFile();
                Thread.Sleep(1000);
            }
        }
    }

    public class Graph
    {

        public static void Create()
        {
            ScottPlot.Plot plot = new ScottPlot.Plot();
            plot.XLabel("Номер поколения");
            plot.YLabel("Количество живых клеток");
            plot.ShowLegend();

            Random rnd = new Random();
            List<double> liveDensities = new List<double>() { 0.1, 0.2, 0.3 };

            var list = CreateList(liveDensities);
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var scatter = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                scatter.LegendText = liveDensities[i].ToString();
                scatter.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            }
            plot.SavePng("../../../../plot.png", 1920, 1080);
        }

        public static List<Dictionary<int, int>> CreateList(List<double> liveDensities)
        {
            List<Dictionary<int,int>> list = new List<Dictionary<int, int>>();

            for (int i = 0; i < liveDensities.Count; i++)
            {
                list.Add(AliveCellsInGeneration(liveDensities[i]));
            }
            list.Sort((x, y) => x.Count - y.Count);
            return list;
        }

        public static Dictionary<int, int> AliveCellsInGeneration(double liveDensity)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            int width = 100;
            int height = 30;
            int cellSize = 1;
            Board board = new Board(width, height, cellSize, liveDensity);

            while (true)
            {
                result.Add(board.poses.Count, board.AliveCellsCount());

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
            return result;
        }
    }
}