using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScottPlot;
using Newtonsoft.Json;

namespace GameOfLife
{
    public class BoardAnalysisData
    {
        public int GenerationNumber { get; set; }
        public int NumberOfLivingCells { get; set; }
        public Dictionary<string, int> NumberOfPatterns { get; set; }
    }

    public class PatternData
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class GameSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
        public int RefreshRate { get; set; }
    }

    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> Neighbors = new List<Cell>();
        private bool IsAliveNext;

        public void DetermineNextLiveState()
        {
            int liveNeighbors = Neighbors.Count(neighbor => neighbor.IsAlive);
            IsAliveNext = IsAlive ? liveNeighbors == 2 || liveNeighbors == 3 : liveNeighbors == 3;
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

        public int Rows => Cells.GetLength(0);
        public int Columns => Cells.GetLength(1);
        public int Width => Columns * CellSize;
        public int Height => Rows * CellSize;

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[height / cellSize, width / cellSize];
            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Columns; y++)
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

        private readonly Random rand = new Random();
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
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    int xL = (x > 0) ? x - 1 : Rows - 1;
                    int xR = (x < Rows - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Columns - 1;
                    int yB = (y < Columns - 1) ? y + 1 : 0;

                    Cells[x, y].Neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].Neighbors.Add(Cells[x, yT]);
                    Cells[x, y].Neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].Neighbors.Add(Cells[xL, y]);
                    Cells[x, y].Neighbors.Add(Cells[xR, y]);
                    Cells[x, y].Neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].Neighbors.Add(Cells[x, yB]);
                    Cells[x, y].Neighbors.Add(Cells[xR, yB]);
                }
            }
        }

        public static Board ResetFromFile(string fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                GameSettings settings = Newtonsoft.Json.JsonConvert.DeserializeObject<GameSettings>(json);
                return new Board(
                    settings.Width,
                    settings.Height,
                    settings.CellSize,
                    settings.LiveDensity);
            }
        }

        public void SaveToFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        writer.Write(Cells[i, j].IsAlive ? '*' : ' ');
                    }
                    writer.WriteLine();
                }
            }
        }

        public static Board LoadFromFile(string fileName, int cellSize = 1)
        {
            string[] lines = File.ReadAllLines(fileName);
            Cell[,] newCells = new Cell[lines.Length, lines[0].Length];

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length && j < newCells.GetLength(1); j++)
                {
                    newCells[i, j] = new Cell { IsAlive = lines[i][j] == '*' };
                }
            }

            return new Board(cellSize, newCells);
        }

        public void Render()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Console.Write(Cells[row, col].IsAlive ? '*' : ' ');
                }
                Console.WriteLine();
            }
        }

        public string GetCellsInString()
        {
            string str = "";

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    str += Cells[row, col].IsAlive ? "*" : ".";
                }
            }
            return str;
        }

        public int CountLivingCells()
        {
            return Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        }
    }

    public class BoardAnalyzer
    {
        public static PatternData[] Patterns;

        public static void CreateGraph(List<double> densities, int width, int height)
        {
            var plot = new Plot();
            plot.XLabel("Generation");
            plot.YLabel("Alive cells");
            plot.ShowLegend();

            foreach (var density in densities)
            {
                Board newBoard = new Board(width, height, 1, density);
                Dictionary<int, int> data = Simulate(newBoard);

                var scatter = plot.Add.Scatter(data.Keys.ToArray(), data.Values.ToArray());
                scatter.Smooth = true;
                scatter.Label = $"{density}";
                scatter.Color = RandomColor();
            }

            plot.SavePng("plot.png", 1920, 1080);
        }

        private static Color RandomColor()
        {
            int red = 100 + Random.Shared.Next() % 255;
            int green = 100 + Random.Shared.Next() % 255;
            int blue = 100 + Random.Shared.Next() % 255;

            return new Color(red, green, blue);
        }

        public static Dictionary<int, int> Simulate(Board board)
        {
            int genNum = 0;
            string currentState = board.GetCellsInString();
            HashSet<string> previousStates = new HashSet<string>();
            Dictionary<int, int> data = new Dictionary<int, int>();

            while (!CheckStability(previousStates, currentState))
            {
                previousStates.Add(currentState);

                board.Advance();
                genNum++;
                currentState = board.GetCellsInString();
                data.Add(genNum, board.CountLivingCells());
            }
            return data;
        }

        public static void SimulateWithPrint(Board board)
        {
            int genNum = 0;
            string currentState = board.GetCellsInString();
            HashSet<string> previousStates = new HashSet<string>();

            while (!CheckStability(previousStates, currentState))
            {
                previousStates.Add(currentState);

                board.Advance();
                board.Render();
                genNum++;
                currentState = board.GetCellsInString();

                BoardAnalysisData data = new BoardAnalysisData()
                {
                    GenerationNumber = genNum,
                    NumberOfLivingCells = board.CountLivingCells(),
                    NumberOfPatterns = CheckPatterns(board)
                };
                PrintData(data);
            }
        }

        private static void PrintData(BoardAnalysisData data)
        {
            Console.WriteLine($"Generation Number: {data.GenerationNumber}");
            Console.WriteLine($"Number of Living Cells: {data.NumberOfLivingCells}");
            foreach (var pattern in data.NumberOfPatterns)
            {
                Console.WriteLine($"{pattern.Key}: {pattern.Value}");
            }
        }

        private static bool CheckStability(HashSet<string> prevStates, string curState)
        {
            return prevStates.Contains(curState);
        }

        public static Dictionary<string, int> CheckPatterns(Board board)
        {
            Dictionary<string, int> numPatterns = new Dictionary<string, int>();

            foreach (var pattern in Patterns)
            {
                numPatterns[pattern.Name] = 0;
            }

            for (int x = 0; x < board.Rows; x++)
            {
                for (int y = 0; y < board.Columns; y++)
                {
                    foreach (var pattern in Patterns)
                    {
                        if (ContainsPattern(board, x, y, pattern))
                        {
                            numPatterns[pattern.Name]++;
                        }
                    }
                }
            }

            return numPatterns;
        }

        private static bool ContainsPattern(Board board, int startX, int startY, PatternData pattern)
        {
            for (int x = 0; x < pattern.Height; x++)
            {
                for (int y = 0; y < pattern.Width; y++)
                {
                    if (board.Cells[(startX + x) % board.Rows, (startY + y) % board.Width].IsAlive != (pattern.Image[x * pattern.Width + y] == '*'))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void LoadPatterns(string fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                Patterns = Newtonsoft.Json.JsonConvert.DeserializeObject<PatternData[]>(json);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Board board = Board.LoadFromFile("gen-118.txt");
            BoardAnalyzer.LoadPatterns("patterns.json");
            // List<double> densities = new List<double>() { 0.1, 0.4, 0.7 }; 
            // BoardAnalyzer.CreateGraph(densities, board.Width, board.Height);
            BoardAnalyzer.SimulateWithPrint(board);
        }
    }
}
