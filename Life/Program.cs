using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Drawing;

using OxyPlot;
using OxyPlot.ImageSharp;
using OxyPlot.Series;
//using OxyPlot.WindowsForms;

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
        public readonly Cell[,] Cells;
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
        public void ConnectNeighbors()
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

        public string SaveState()
        {
            var stateArray = new bool[Rows, Columns];
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    stateArray[y, x] = Cells[x, y].IsAlive;
                }
            }

            string jsonString = JsonConvert.SerializeObject(stateArray
                , (Newtonsoft.Json.Formatting)System.Xml.Formatting.Indented);
            return jsonString;
        }

        public void LoadState(string state)
        {
            bool[,] stateArray = JsonConvert.DeserializeObject<bool[,]>(state);

            for (int x = 0; x < Cells.GetLength(0); x++)
            {
                for (int y = 0; y < Cells.GetLength(1); y++)
                {
                    Cells[x, y].IsAlive = stateArray[x, y];
                }
            }
        }

        public void SaveStateToFile(string filePath)
        {
            string stateJson = SaveState();
            File.WriteAllText(filePath, stateJson);
        }

        public void LoadStateFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string stateJson = File.ReadAllText(filePath);
                LoadState(stateJson);
            }
            else
            {
                Console.WriteLine("No saved state found.");
            }
        }

        
    }

    class Program
    {
        static Board board;
        static List<int> averageTimes = new();
        static double[] densities = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };

    static private void Reset()
        {
            board = new Board(
                width: 50,
                height: 20,
                cellSize: 1,
                liveDensity: 0.5);
        }

        static void Reset(GameSettings settings)
        {
            board = new Board(
                width: settings.width,
                height: settings.height,
                cellSize: settings.cellSize,
                liveDensity: settings.liveDensity);
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

        public static void RunSimulation(GameSettings settings, int iterations)
        {
            Reset(settings);
            int stableGeneration = 0;
            for (int gen = 0; gen < iterations; gen++)
            {
                board.Advance();
                if (IsStable())
                {
                    stableGeneration = gen;
                    break;
                }
            }
            averageTimes.Add(stableGeneration);
        }

        public static int previousLiveCells = 0;
        static public bool IsStable()
        {
            int currentLiveCells = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
            var result = currentLiveCells == previousLiveCells;
            previousLiveCells = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
            return result;
        }

        static public void PlotResults()
        {
            var model = new PlotModel { Title = "Stabilization Times vs Live Density" };

            var series = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                MarkerStroke = OxyColors.Black,
                MarkerStrokeThickness = 1
            };

            for (int i = 0; i < densities.Length; i++)
            {
                series.Points.Add(new DataPoint(densities[i], averageTimes[i]));
            }

            model.Series.Add(series);

            string projectRootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));

            string filePath = Path.Combine(projectRootPath, "plot.png");

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            PngExporter.Export(model, filePath, 1280, 720);
        }

        static void Main_SaveAndLoad()
        {
            string settingsFilePath = "settings.json";
            string stateFilePath = "state.txt";

            string json = File.ReadAllText(settingsFilePath);

            GameSettings settings = System.Text.Json.JsonSerializer.Deserialize<GameSettings>(json);

            Reset(settings);

            while (true)
            {
                Console.Clear();

                Render();

                board.Advance();

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key == ConsoleKey.F5)
                    {
                        board.SaveStateToFile(stateFilePath);
                    }

                    else if (keyInfo.Key == ConsoleKey.F6)
                    {
                        if (File.Exists(stateFilePath))
                        {
                            board.LoadStateFromFile(stateFilePath);
                        }
                        else
                        {
                            Console.WriteLine("No saved state found.");
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        
        }
        
        static void Main_PlotStabilization()
        {
            string settingsFilePath = "settings.json";

            string json = File.ReadAllText(settingsFilePath);

            GameSettings settings = System.Text.Json.JsonSerializer.Deserialize<GameSettings>(json);

            Reset(settings);

            foreach (var density in densities)
            {
                settings.liveDensity = density;
                RunSimulation(settings, 1000);
            }

            PlotResults();
        }
        static void Main(string[] args)
        {
            // Main_SaveAndLoad();

            Main_PlotStabilization();
        }
    }

    public class GameSettings
    {
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }
        public double liveDensity { get; set; }
    }
}