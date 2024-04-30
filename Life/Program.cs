using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

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
    public class Figure
    {
        public string Name { get; set; }
        public List<bool> Cells { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
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
        public void LoadFigure(string filePath)
        {
            string json = System.IO.File.ReadAllText(filePath);
            Figure figure = JsonConvert.DeserializeObject<Figure>(json);

            int x = (Columns - figure.Width) / 2;
            int y = (Rows - figure.Height) / 2;

            for (int i = 0; i < figure.Cells.Count; i++)
            {
                int col = x + i % figure.Width;
                int row = y + i / figure.Width;
                Cells[col, row].IsAlive = figure.Cells[i];
            }
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

        public void SaveState(string filePath)
        {
            List<bool> cellStates = new List<bool>();
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    cellStates.Add(Cells[x, y].IsAlive);
                }
            }

            string json = JsonConvert.SerializeObject(cellStates);
            System.IO.File.WriteAllText(filePath, json);
        }

        public void LoadState(string filePath)
        {
            string json = System.IO.File.ReadAllText(filePath);
            List<bool> cellStates = JsonConvert.DeserializeObject<List<bool>>(json);

            int index = 0;
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y].IsAlive = cellStates[index];
                    index++;
                }
            }
        }
    }

    public class Settings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
        public int SleepTime { get; set; }
    }

    public class Program
    {
        static Board board;
        static Settings settings;
        static List<Figure> Figures { get; set; } = new List<Figure>();

        static void LoadFigures()
        {
            string[] files = Directory.GetFiles("figures", "*.json");
            foreach (string file in files)
            {
                Figure figure = new Figure();
                figure.Name = Path.GetFileNameWithoutExtension(file);
                board.LoadFigure(file);
                Figures.Add(figure);
            }
        }

        static void LoadSettings()
        {
            string settingsJson = System.IO.File.ReadAllText("settings.json");
            settings = JsonConvert.DeserializeObject<Settings>(settingsJson);
        }

        static private void Reset()
        {
            board = new Board(
                width: settings.Width,
                height: settings.Height,
                cellSize: settings.CellSize,
                liveDensity: settings.LiveDensity);
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
        static void Save()
        {
            board.SaveState("state.txt");
            Console.WriteLine("State saved to state.txt");
        }
        static void Load()
        {
            board.LoadState("state.txt");
            Console.WriteLine("State loaded from state.txt");
        }
        static void Main(string[] args)
        {
            LoadSettings();
            Reset();
            LoadFigures(); // Загрузка фигур-колоний
            while (true)
            {
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(settings.SleepTime);

                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.S)
                    {
                        Save();
                    }
                    else if (key == ConsoleKey.L)
                    {
                        Load();
                    }
                }
            }
        }
    }
}
