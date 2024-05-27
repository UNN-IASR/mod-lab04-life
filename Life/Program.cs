using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ScottPlot.AxisLimitManagers;
using System.Text.Json;
using System.IO;
using System.Data.SqlTypes;



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

    public class Settings {

        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }

    }
    public class Board
    {
        public Cell[,] Cells;
        public int CellSize;
        public double LiveDensity;
        public List<string> poses = new List<string>();

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;
            LiveDensity = liveDensity;


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

        public int cellAliveCount() {
            int n = 0;

            foreach (Cell c in Cells) {
                if (c.IsAlive)
                    n++;
            }
            return n;
        }

        public string Save(Board board) {
            string str = "";

            foreach (Cell cell in board.Cells) {
                if (cell.IsAlive)
                    str += '1';
                else
                    str += '0';
            }
            return str;
        }

        public bool IsStable() {

            if (poses.Contains(Save(this))) return true;
            else return false;

        }

        public void SaveBoardState() {
            string filePath = $"savedBoard.txt";

            using (StreamWriter writer = new StreamWriter(filePath)) {
                for (int row = 0; row < Rows; row++) {
                    for (int col = 0; col < Columns; col++) {
                        var cell = Cells[col, row];
                        writer.Write(cell.IsAlive ? '*' : ' ');
                    }
                    writer.WriteLine();
                }
            }
            Console.WriteLine("Board state saved to " + filePath);
        }
        public void LoadBoardState(string filePath) {
            if (File.Exists(filePath)) {
                string[] lines = File.ReadAllLines(filePath);
                int height = lines.Length;
                int width = lines[0].Length;

                if (width != Columns || height != Rows) {
                    Console.WriteLine("Cannot load board state. Dimensions do not match.");
                    return;
                }

                for (int row = 0; row < height; row++) {
                    for (int col = 0; col < width; col++) {
                        char symbol = lines[row][col];
                        Cells[col, row].IsAlive = (symbol == '*');
                    }
                }
            }
            else {
                Console.WriteLine("File not found: " + filePath + "\nPress any key to continue");
                Console.ReadKey();
            }
        }
    }



    public class Grafic {
        public static Dictionary<int, int> alive_in_generat(double d) {
            var resul = new Dictionary<int, int>();
            Board board = new Board(100, 30, 1, d);

            while (true) {
                resul.Add(board.poses.Count, board.cellAliveCount());

                if (!board.poses.Contains(board.Save(board))) {
                    board.poses.Add(board.Save(board));
                }
                else {
                    break;
                }
                board.Advance();
            }
            return resul;
        }

        public static List<Dictionary<int, int>> create_list(List<double> d, int num) {
            var lis = new List<Dictionary<int, int>>();

            for (int ind = 0; ind < num; ind++) {
                if (d[ind] < 0.3 || d[ind] > 0.5)
                    break;
                lis.Add(alive_in_generat(d[ind]));
            }
            lis.Sort((x, y) => x.Count - y.Count);
            return lis;
        }
        public static void grafic() {
            var p = new ScottPlot.Plot();
            p.XLabel("generation");
            p.YLabel("alive cells");
            p.ShowLegend();
            Random rnd = new Random();
            List<double> density = new List<double>() { 0.3, 0.4, 0.5 };
            var lis = create_list(density, density.Count);
            int num = 0;
            foreach (var item in lis) {
                var scatter = p.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                scatter.LegendText = density[num].ToString();
                scatter.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                num++;
            }
            p.SavePng("plot.png", 1920, 1080);
        }
    }



    public class Program
    {
        public static Board board;
        public static void Reset(Settings settings) {

            board = new Board(
                settings.Width,
                settings.Height,
                settings.CellSize,
                settings.LiveDensity);
        }
        static void Render() {

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
            Settings settings = LoadSettings("../../../../config.json");
            Reset(settings);
            int stepsCount = 0;
            while(true)
            {
                stepsCount++;
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(10);
                if (!board.IsStable())
                    board.poses.Add(board.Save(board));
                else
                    break;

                if (Console.KeyAvailable) {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.S) {
                        board.SaveBoardState();
                    }
                    else if (key == ConsoleKey.L) {
                        board.LoadBoardState("../../../../Board.txt");
                    }
                }
            }
            Console.WriteLine("Steps: " + stepsCount);
            board.SaveBoardState();
            Grafic.grafic();
        }

        public static Settings LoadSettings(string filePath) {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Settings>(json);
        }

        
    }
}