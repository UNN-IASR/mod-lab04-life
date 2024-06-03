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

        public Board(int width, int height, int cellSize, double liveDensity = .1, string? filePath = null)
        {
            CellSize = cellSize;
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            if (filePath == null)
            {
                Randomize(liveDensity);
            }
            else
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // Чтение содержимого файла строка за строкой
                    string line;
                    for(int i = 0; (line = reader.ReadLine()) != null; i++)
                    {
                        for (int j = 0; j < line.Length; j++)
                        {
                            if (line[j] == '*')
                            {
                                Cells[j,i].IsAlive = true;
                            }
                        }
                    }
                }
            }
        }
        public bool isStable()
        {
            int firstNum = countElements();
            Advance();
            int secondNum = countElements();
            Advance();
            int thirdNum = countElements();
            Advance();
            if (firstNum == secondNum && secondNum == thirdNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int countElements()
        {
            int count = 0;
            foreach (Cell cell in this.Cells)
            {
                if (cell.IsAlive)
                {
                    count++;
                }
            }
            return count;
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

    public class BoardSettings
    {
        public int CellSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double LiveDensity { get; set; }

        public BoardSettings()
        {
            Width = 50;
            Height = 20;
            CellSize = 1;
            LiveDensity = 0.5;
        }
        public BoardSettings(double _LiveDensity) 
        {
            Width = 50;
            Height = 20;
            CellSize = 1;
            LiveDensity = _LiveDensity;
        }
    }
    class Program
    {
        static int fileCount = 0;

        static Board board;
        static private void Reset(double _LiveDensity,string? filePath = null, string? settingsFile = null)
        {
            var settings = new BoardSettings(_LiveDensity);

            if (settingsFile!= null)
            {
                using (StreamReader reader = new StreamReader(settingsFile))
                {
                    string jsonContent = reader.ReadToEnd(); // Читаем содержимое файла
                    settings = JsonConvert.DeserializeObject<BoardSettings>(jsonContent);// Выводим содержимое в консоль
                }

            }
            board = new Board(settings.Width, settings.Height, settings.CellSize, settings.LiveDensity, filePath);
        }
        static void Render()
        {
            string filePath = "../../../gen.txt";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Columns; col++)
                    {
                        var cell = board.Cells[col, row];
                        if (cell.IsAlive)
                        {
                            writer.Write('*');
                            Console.Write('*');
                        }
                        else
                        {
                           Console.Write(' ');
                           writer.Write(' ');
                        }
                    }
                    Console.Write('\n');
                    writer.WriteLine();
                }
                writer.Close();
            }
        }
        static void Save()
        { 
                string sourceFileName = "../../../gen.txt"; // Исходный файл

                string destinationFileName = "../../../gens/gen" + fileCount + ".txt"; // Файл назначени

                fileCount++;

                File.Copy(sourceFileName, destinationFileName, true);
        }
        static void Main(string[] args)
        {
            double[] xs = new double[50]; 
            double[] ys = new double[50];


            for (int i = 1; i <= 50; i++)
            {
                double liveDensity = i/100d;
                Reset(liveDensity);
                int numGen = 0;
                int sumGen = 0;
                double avgNumgen = 0;
                while (fileCount <= 100)
                {
                    numGen++;
                    if (board.isStable() || numGen == 500)
                    {
                        fileCount++;
                        sumGen += numGen;
                        avgNumgen = sumGen / fileCount;
                        numGen = 0;
                        Reset(liveDensity);
                    }
                }
                fileCount = 0;
                xs[i-1] = liveDensity;
                ys[i-1] = avgNumgen;
            }

            ScottPlot.Plot myPlot = new();
            myPlot.Add.Scatter(xs, ys);
            myPlot.SavePng("../../../plot.png",1000,800);
        }
    }
}