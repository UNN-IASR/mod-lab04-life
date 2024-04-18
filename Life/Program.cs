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

namespace cli_life {
    public class Cell {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState() {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance() {
            IsAlive = IsAliveNext;
        }
    }
    public class Board {
        public readonly Cell[,] Cells;
        public int CellSize {get;}

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public List<string> states = new List<string>();
        public double LiveDensity {get;}

        public Board(int width, int height, int cellSize, double liveDensity = .1) {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity) {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance() {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors() {
            for (int x = 0; x < Columns; x++) {
                for (int y = 0; y < Rows; y++) {
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
        public void Load(string path) {
            using (var reader = new StreamReader(path)) {
                for(int row = 0; row < Rows; row++) {
                    string line = reader.ReadLine();
                    for(int col = 0; col < Columns; col++)
                        Cells[col, row].IsAlive = line[col] == '*';
                }
            }
        }

        public void FindPattern(Pattern pattern) {
            pattern.count = 0;
            for(int row = 0; row < Rows; row++) {
                for(int col = 0; col < Columns; col++) pattern.count += CheckPattern(pattern, row, col) ? 1 : 0;
            }
        }

        private bool CheckPattern(Pattern pattern, int row, int col) {
            for (int i = 0; i < pattern.size; i++) {
                for (int j = 0; j < pattern.size; j++) {
                    int x = col + j < Columns ? col+j : j-1;
                    int y = row + i < Rows ? row+i : i-1;
                    if ((Cells[x, y].IsAlive && pattern.image[i*pattern.size+j] == '.') || (!Cells[x, y].IsAlive && pattern.image[i*pattern.size+j] == '*')) return false;
                }
            }
            return true;
        }
        public int CheckAlive() {
            int count = 0;
            foreach(Cell cell in Cells) {
                count += cell.IsAlive ? 1 : 0;
            }
            return count;
        }
        public bool CheckStable() {
            string str = "";
            foreach(Cell cell in Cells) {
                str += cell.IsAlive ? "*" : " ";
            }
            states.Add(str);
            if (states.Count > 3) states.RemoveAt(0);
            return states.Take(states.Count-1).Contains(str);
        }
    }
    public class Pattern {
        public string name;
        public string image;
        public int count;
        public int size;
        public Pattern(string _name, string _image, int _size) {
            name = _name;
            image = _image;
            size = _size;
        }
    }
    public class Graph {
        private static Dictionary<int, int> CountAlive(double density) {
            var res = new Dictionary<int, int>();
            var board = new Board(100, 20, 1, density);
            int gen = 0;
            while (true) {
                if(gen % 20 == 0) res.Add(gen, board.CheckAlive());
                if(board.CheckStable()) break;
                board.Advance();
                gen++;
            }
            return res;
        }
        private static List<Dictionary<int, int>> CreateList(List<double> density, int count) {
            var list = new List<Dictionary<int,int>>();
            for(int i=0; i<count; i++) {
                if(density[i] < 0.2 || density[i] > 0.8) break;
                list.Add(CountAlive(density[i]));
            }
            list.Sort((x, y) => x.Count - y.Count);
            return list;
        }
        public static void CreateGraph() {
            var plot = new Plot();
            plot.XLabel("Generation");
            plot.YLabel("Alive cells");
            plot.ShowLegend();
            Random rnd = new Random();
            List<double> density = new List<double>() {0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8};
            var list = CreateList(density, density.Count);
            int c = 0;
            foreach(var item in list) {
                var scatter = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                scatter.Label = density[c].ToString();
                scatter.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                c++;
            }
            plot.SavePng("plot.png", 1920, 1080);
        }
    }
    public class Program {
        static public Board Reset(string filePath) {
            using FileStream fstream = new FileStream(filePath, FileMode.OpenOrCreate);
            return JsonSerializer.Deserialize<Board>(fstream);
        }
        static Board board = Reset("settings.json");
        static void Render() {
            for (int row = 0; row < board.Rows; row++) {
                for (int col = 0; col < board.Columns; col++) {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                        Console.Write('*');
                    else
                        Console.Write(' ');
                }
                Console.Write('\n');
            }
        }
        static void Save(int counter) {
            using (var writer = new StreamWriter("state" + counter.ToString() + ".txt")) {
                for (int row = 0; row < board.Rows; row++) {
                    for (int col = 0; col < board.Columns; col++) {
                        var cell = board.Cells[col, row];
                        if (cell.IsAlive)
                            writer.Write('*');
                        else
                            writer.Write(' ');
                    }
                    writer.Write('\n');
                }
            }
        }
        static bool exitRequested = false;
        static void Main(string[] args) {
            Graph.CreateGraph();
            //board.Load("state40.txt");
            Pattern boat = new Pattern("Boat", ".......*...*.*..**.......", 5);
            Pattern block = new Pattern("Block", ".....**..**.....", 4);
            Pattern loaf = new Pattern("Loaf", "........**...*..*...*.*....*........", 6);
            Pattern ship = new Pattern("Ship", ".......**..*.*..**.......", 5);
            Pattern tub = new Pattern("Tub", ".......*...*.*...*.......", 5);
            int counter = 1;
            while(!exitRequested) {
                Console.Clear();
                board.FindPattern(block);
                board.FindPattern(boat);
                board.FindPattern(loaf);
                board.FindPattern(ship);
                board.FindPattern(tub);
                Render();
                Console.WriteLine("Alive " + board.CheckAlive().ToString());
                Console.WriteLine("Block" + " " + block.count.ToString());
                Console.WriteLine("Boat" + " " + boat.count.ToString());
                Console.WriteLine("Loaf" + " " + loaf.count.ToString());
                Console.WriteLine("Ship" + " " + ship.count.ToString());
                Console.WriteLine("Tub" + " " + tub.count.ToString());
                if (board.CheckStable()) break;
                Thread.Sleep(1);
                if (Console.KeyAvailable) {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.KeyChar == 'q')
                        exitRequested = true;
                    else if (keyInfo.KeyChar == 's')
                        Save(counter);
                }
                counter++;
                board.Advance();
            }
        }
    }
}