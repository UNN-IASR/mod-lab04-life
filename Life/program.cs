using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

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

        public int Columns
        {
            get { return Cells.GetLength(0); }
        }

        public int Rows
        {
            get { return Cells.GetLength(1); }
        }

        public int Width
        {
            get { return Columns * CellSize; }
        }

        public int Height
        {
            get { return Rows * CellSize; }
        }

        public Board(string filePath, int width, int height, int cellSize, double liveDensity)
        {
            CellSize = cellSize;
            Cells = new Cell[width / CellSize, height / CellSize];
            for (int x = 0; x < Columns; x++)
            for (int y = 0; y < Rows; y++)
                Cells[x, y] = new Cell();

            ConnectNeighbors();

            if (string.IsNullOrEmpty(filePath))
            {
                Randomize(liveDensity);
            }
            else
            {
                LoadFromFile(filePath);
            }
        }

        private void LoadFromFile(string filePath)
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            try
            {
                using StreamReader reader = new StreamReader(filePath);
                for (int row = 0; row < Cells.GetLength(1); row++)
                {
                    var line = reader.ReadLine();
                    var split = line.ToCharArray();
                    for (int col = 0; col < Cells.GetLength(0); col++)
                    {
                        var cell = Cells[col, row];
                        cell.IsAlive = split[col].Equals('*');
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                throw new FileLoadException($"Не удалось прочитать файл {filePath}");
            }
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
    }

    class Program
    {
        private static Board board;
        private static Startup _startup;

        private static void Reset()
        {
            _startup = new Startup();
            var config = _startup._appConfig;

            board = new Board(
                filePath: config.LoadFilePath,
                width: config.BoardHeight,
                height: config.BoardWidth,
                cellSize: config.BoardCellSize,
                liveDensity: config.BoardLiveDensity);
        }

        private static void Render()
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

        private static void Main(string[] args)
        {
            Reset();
            int iterCount = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Итерация: {++iterCount}");
                Render();
                board.Advance();

                if (iterCount % 5 == 0)
                {
                    CreateFile();
                }

                Thread.Sleep(400);
            }
        }

        private static void CreateFile()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\last_state.txt");

            using (StreamWriter streamWriter = File.CreateText(filePath))
            {
                var cells = board.Cells;
                for (var x = 0; x < cells.GetLength(0); x++)
                {
                    for (var y = 0; y < cells.GetLength(1); y++)
                    {
                        var cell = cells[x, y];
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
    }
}
