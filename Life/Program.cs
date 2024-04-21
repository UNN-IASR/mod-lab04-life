using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text.Json;
using ScottPlot;
using ScottPlot.Plottables;

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
        public int Rows { get { return Cells.GetLength(0); } }
        public int Columns { get { return Cells.GetLength(1); } }
        public double LiveDensity { get; }
        private static List<int> hash_states = [];
        
        public Board(int rows, int columns, double liveDensity = 0) {
            Cells = new Cell[rows, columns];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < columns; c++)
                    Cells[r, c] = new Cell();
            LiveDensity = liveDensity;
            ConnectNeighbors();
            if (liveDensity > 0) Randomize();
        }

        readonly Random rand = new Random();
        public void Randomize() {
            foreach (Cell cell in Cells)
                cell.IsAlive = rand.NextDouble() < LiveDensity;
        }
        public void Advance() {
            foreach (Cell cell in Cells)
                cell.DetermineNextLiveState();
            foreach (Cell cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors() {
            for (int r = 0; r < Rows; r++) {
                for (int c = 0; c < Columns; c++) {
                    int rT = (r > 0) ? r - 1 : Rows - 1;
                    int rB = (r < Rows - 1) ? r + 1 : 0;
                    int cL = (c > 0) ? c - 1 : Columns - 1;
                    int cR = (c < Columns - 1) ? c + 1 : 0;
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
        public void Render() {
            for (int r = 0; r < Rows; r++) {
                string line = "";
                for (int c = 0; c < Columns; c++)
                    line += Cells[r, c].IsAlive ? '*' : ' ';
                Console.WriteLine(line);
            }
        }
        public bool CheckStable() {
            int hash = string.Join("", Cells.Cast<Cell>()
                               .Select(c => c.IsAlive? "*" : " "))
                               .GetHashCode();
            hash_states.Add(hash);
            if (hash_states.Count > 3) hash_states.RemoveAt(0);
            return hash_states.SkipLast(1).Contains(hash);
        }
    }
    public class Pattern {
        public string Name { get; init; }
        public int Height { get; init; }
        public int Width { get; init; }
        public string Body { get; init; }
    }
    public static class BoardUtils {
        public static List<Pattern> patterns;
        public static Board FromJSON(string fname) {
            using FileStream fstream = new FileStream(fname, FileMode.OpenOrCreate);
            return JsonSerializer.Deserialize<Board>(fstream);
        }
        public static Board FromTXT(string fname) {
            string[] lines = File.ReadAllLines(fname);
            (int rows, int columns) = (lines.Length, lines.Max(l => l.Length));
            Board board = new Board(rows, columns);
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < columns; c++)
                    board.Cells[r, c].IsAlive = lines[r][c] == '*';
            return board;
        }
        public static void Save(Board board, string fname) {
            using FileStream fstream = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write);
            using StreamWriter writer = new StreamWriter(fstream);
            TextWriter console = Console.Out;
            Console.SetOut(writer);
            board.Render();
            Console.SetOut(console);
        }
        public static int CountAlive(Board board) {
            int count = 0;
            foreach (Cell cell in board.Cells)
                count += cell.IsAlive ? 1 : 0;
            return count;
            // TODO: LINQ
        }
        public static void LoadPatterns(string fname) {
            using FileStream fstream = new FileStream(fname, FileMode.OpenOrCreate);
            patterns = JsonSerializer.Deserialize<List<Pattern>>(fstream);
        }
        public static int FindPattern(Board board, Pattern pattern) { 
            int count = 0;
            for (int r = 0; r < board.Rows; r++)
                for (int c = 0; c < board.Columns; c++) {
                    string slice = Slice(board, (r, c), (pattern.Height, pattern.Width));
                    if (pattern.Body == slice) count += 1;
                }
            return count;
        }
        private static string Slice(Board board, (int, int) pos, (int, int) size) {
            string res = "";
            for (int i = 0; i < size.Item1; i++) {
                for (int j = 0; j < size.Item2; j++) {
                    int r = (pos.Item1 + i < board.Rows) ? pos.Item1 + i : pos.Item1 + i - board.Rows;
                    int c = (pos.Item2 + j < board.Columns) ? pos.Item2 + j : pos.Item2 + j - board.Columns;
                    res += board.Cells[r, c].IsAlive ? '*' : ' ';
                }
            }
            return res;
        }
        public static Dictionary<string, int> FindPatterns(Board board) {
            return patterns.Select(p => new { name = p.Name, count = FindPattern(board, p)})
                            .GroupBy(p => p.name)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(p => p.count).Sum()
                            );
        }
    }
    static class GraphBuilder {
        private class DensityData : Dictionary<int, int> { }
        private static DensityData SimulateBoard(double density) {
            DensityData res = new DensityData();
            Board board = new Board(30, 100, density);
            int gen = 0;
            while (true) {
                if (gen % 10 == 0) {
                    int alive = BoardUtils.CountAlive(board);
                    res.Add(gen, alive > 1000 ? 1000 : alive);
                }
                if (board.CheckStable()) break;
                board.Advance();
                gen++;
            }
            return res;
        }
        private static List<DensityData> SimulateDensity(double density, int count) {
            List<DensityData> res = new();
            for (int i = 0; i < count; i++)
                res.Add(SimulateBoard(density));
            res.Sort((x, y) => x.Count - y.Count);
            return res;
        }
        public static void CreateGraph(List<double> densitites, int count) {
            Plot m_plot = new Plot();
            m_plot.XLabel("Generations (step: 10)");
            m_plot.YLabel("Alive Cells (max: 1000)");
            m_plot.ShowLegend();
            Random rand = new Random();
            Plot d_plot;
            foreach (double dens in densitites) {
                d_plot = new Plot();
                d_plot.XLabel("Generations (step: 10)");
                d_plot.YLabel("Alive Cells (max: 1000)");
                d_plot.ShowLegend();
                List<DensityData> res = SimulateDensity(dens, count);
                foreach (DensityData item in res) {
                    Scatter d_scatter = d_plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                    d_scatter.Smooth = true;
                    d_scatter.Color = new Color(rand.Next(256), rand.Next(256), rand.Next(256));
                }
                d_plot.SavePng($"graphs/{dens}.png", 1440, 900);

                DensityData median = res.ElementAt(count / 2);
                Scatter m_scatter = m_plot.Add.Scatter(median.Keys.ToArray(), median.Values.ToArray());
                m_scatter.Label = $"{dens}";
                m_scatter.Smooth = true;
                m_scatter.Color = new Color(rand.Next(256), rand.Next(256), rand.Next(256));
            }
            m_plot.SavePng("graph.png", 1440, 900);
        }
    }
    class Program {
        static Board board;
        static void Main(string[] args) {
            // GraphBuilder.CreateGraph(new List<double>{ 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 }, 11);
            // return;
            board = BoardUtils.FromJSON("settings.json");
            // board = BoardUtils.FromTXT("hehehehe.txt");
            // board = BoardUtils.FromTXT("glider_gun.txt");
            BoardUtils.LoadPatterns("patterns.json");
            int gen = 0;
            bool run = true;
            while (run) {
                Dictionary<string, int> patterns = BoardUtils.FindPatterns(board);
                Console.Clear();
                board.Render();
                Console.WriteLine($"Gen: {gen++}");
                Console.WriteLine($"Alive: {BoardUtils.CountAlive(board)}");
                foreach (KeyValuePair<string, int> rec in patterns)
                    Console.WriteLine($"{rec.Key}: {rec.Value}");
                if (board.CheckStable()) {
                    Console.WriteLine("Board is stable");
                    // BoardUtils.Save(board, $"stable_gen_{gen}.txt");
                    break;
                }
                board.Advance();
                if (Console.KeyAvailable) {
                    ConsoleKeyInfo key = Console.ReadKey();
                    switch (key.KeyChar) {
                    case 'q' :
                        run = false;
                        break;
                    case 's':
                        BoardUtils.Save(board, $"gen_{gen}.txt");
                        break;
                    case 'r':
                        board.Randomize();
                        gen = 0;
                        break;
                    }
                }
                Thread.Sleep(10);
            }
        }
    }
}