using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace Life
{
    public class LifeSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }

    public class Cell
    {
        public bool IsAlive { get; set; }
        public List<Cell> Neighbors { get; } = new List<Cell>();
        private bool _isAliveNext;

        public void DetermineNextState()
        {
            int aliveNeighbors = Neighbors.Count(cell => cell.IsAlive);
            _isAliveNext = IsAlive ? (aliveNeighbors == 2 || aliveNeighbors == 3) : aliveNeighbors == 3;
        }

        public void UpdateState()
        {
            IsAlive = _isAliveNext;
        }
    }

    public class Board
    {
        public Cell[,] Cells { get; }
        public int CellSize { get; }

        public int Columns => Cells.GetLength(0);
        public int Rows => Cells.GetLength(1);
        public int Width => Columns * CellSize;
        public int Height => Rows * CellSize;

        private readonly Random _random = new Random();

        public Board(int width, int height, int cellSize, double liveDensity)
        {
            CellSize = cellSize + 1;
            Cells = new Cell[width / CellSize, height / CellSize];
            InitializeCells();
            ConnectNeighbors();
        }

        private void InitializeCells()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                }
            }
        }

        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    AddNeighbors(x, y);
                }
            }
        }

        private void AddNeighbors(int x, int y)
        {
            int[] dx = { -1, 0, 1 };
            int[] dy = { -1, 0, 1 };

            foreach (var i in dx)
            {
                foreach (var j in dy)
                {
                    if (i != 0 || j != 0)
                    {
                        int neighborX = (x + i + Columns) % Columns;
                        int neighborY = (y + j + Rows) % Rows;
                        Cells[x, y].Neighbors.Add(Cells[neighborX, neighborY]);
                    }
                }
            }
        }

        public void LoadCellsFromFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    Cells[x, y].IsAlive = lines[y][x] == '*';
                }
            }
        }

        public void Randomize(string filePath)
        {
            var data = JsonSerializer.Deserialize<LifeSettings>(File.ReadAllText(filePath));
            foreach (var cell in Cells)
            {
                cell.IsAlive = _random.NextDouble() < data.LiveDensity;
            }
        }

        public void Advance()
        {
            foreach (var cell in Cells)
            {
                cell.DetermineNextState();
            }

            foreach (var cell in Cells)
            {
                cell.UpdateState();
            }
        }

        public int CountPattern(string pattern)
        {
            return 0;
        }
    }

    public class LifeGame
    {
        private Board _board;

        public int InitializeBoard(string cellFilePath, string configFilePath)
        {
            var config = JsonSerializer.Deserialize<LifeSettings>(File.ReadAllText(configFilePath));
            _board = new Board(config.Width, config.Height, config.CellSize, config.LiveDensity);
            _board.LoadCellsFromFile(cellFilePath);
            return _board.Width * _board.Height;
        }

        public int Display()
        {
            int liveCount = 0;
            for (int y = 0; y < _board.Rows; y++)
            {
                for (int x = 0; x < _board.Columns; x++)
                {
                    if (_board.Cells[x, y].IsAlive)
                    {
                        Console.Write('*');
                        liveCount++;
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }
            return liveCount;
        }

        public void SaveBoardToFile(string outputFilePath)
        {
            using var writer = new StreamWriter(outputFilePath);
            for (int y = 0; y < _board.Rows; y++)
            {
                var line = new char[_board.Columns];
                for (int x = 0; x < _board.Columns; x++)
                {
                    line[x] = _board.Cells[x, y].IsAlive ? '*' : ' ';
                }
                writer.WriteLine(new string(line));
            }
        }

        public (int totalCells, int liveCells, int iterations) RunSimulation(string cellFilePath, string configFilePath)
        {
            int[] recentLiveCounts = { -1, -1, -1, -1, -1 };
            int iteration = 0;
            int liveCells = 0;

            int totalCells = InitializeBoard(cellFilePath, configFilePath);

            while (true)
            {
                iteration++;
                Console.Clear();
                liveCells = Display();
                recentLiveCounts[iteration % 5] = liveCells;
                if (recentLiveCounts.Distinct().Count() == 1)
                {
                    break;
                }
                _board.Advance();
                Thread.Sleep(100);
            }

            Console.WriteLine($"\n\tBlock Patterns: {_board.CountPattern("block")}");
            Console.WriteLine($"\tBox Patterns: {_board.CountPattern("box")}");
            Console.WriteLine($"\tHive Patterns: {_board.CountPattern("hive")}");

            return (totalCells, liveCells, iteration - 2);
        }

        public object Run(string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            LifeGame lifeGame = new LifeGame();
            var results = lifeGame.RunSimulation("../../../../user_stuff/example1.txt", "../../../../user_stuff/user_settings.json");

            Console.WriteLine($"\n\tLive Cells: {results.liveCells}");
            Console.WriteLine($"\tDead Cells: {results.totalCells - results.liveCells}");
            Console.WriteLine($"\tLive Cell Density: {(double)results.liveCells / results.totalCells}");
            Console.WriteLine($"\tDead Cell Density: {(double)(results.totalCells - results.liveCells) / results.totalCells}");

            lifeGame.SaveBoardToFile("../../../../user_stuff/res.txt");

            Console.WriteLine($"\n\n\tStabilized at iteration {results.iterations}.\n\n");
        }
    }
}
