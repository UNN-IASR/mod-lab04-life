using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Numerics;
using System.Drawing;
using System.Drawing.Imaging;
using System;


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
        private List<string> history = new List<string>();
        public bool IsStable { get; private set; } = false;

        public void CheckStability()
        {
            // Преобразуем состояние поля в строку
            string state = string.Join("",
                Cells.Cast<Cell>().Select(cell => cell.IsAlive ? "*" : "."));

            // Проверяем, было ли такое состояние ранее
            if (history.Contains(state))
            {
                IsStable = true;
            }
            else
            {
                history.Add(state);
            }
        }
    }

    class Program
    {
        public static Board board;
        static dynamic settings;

        // Словарь образцов
        static Dictionary<string, bool[,]> patterns = new Dictionary<string, bool[,]>()
        {
            { "block", new bool[,] { { true, true }, { true, true } } },
            { "beehive", new bool[,] {
                { false, true, true, false },
                { true, false, false, true },
                { false, true, true, false }
            } },
            { "loaf", new bool[,] {
                { false, true, true, false },
                { true, false, false, true },
                { false, true, false, true },
                { false, false, true, false }
            }
        }

        };
        static private void Reset()
        {
            LoadSettings();
            board = new Board(
                width: Convert.ToInt32(settings.BoardWidth),
                height: Convert.ToInt32(settings.BoardHeight),
                cellSize: Convert.ToInt32(settings.CellSize),
                liveDensity: Convert.ToDouble(settings.LiveDensity));
        }

        static void LoadSettings()
        {
            string json;
            string projectDirectory  = @"D:\code\C#\mod-lab04-life-main\Life";
            string settingsPath = Path.Combine(projectDirectory, "settings.json");

            using (StreamReader r = new StreamReader(settingsPath))
            {
                json = r.ReadToEnd();
                settings = JsonConvert.DeserializeObject(json);
            }
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

        public static void SaveState(string filename)
        {
            // Добавляем расширение ".txt", если оно отсутствует
            if (!filename.EndsWith(".txt"))
            {
                filename += ".txt";
            }
            // Получаем путь к корневой папке проекта
            string rootPath = @"D:\code\C#\mod-lab04-life-main\Life";
            // Формируем полный путь к файлу сохранения
            string fullPath = Path.Combine(rootPath, filename);

            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                int liveCells = 0;

                for (int y = 0; y < board.Rows; y++)
                {
                    for (int x = 0; x < board.Columns; x++)
                    {
                        char cellSymbol = board.Cells[x, y].IsAlive ? '*' : '.';
                        writer.Write(cellSymbol);
                        if (cellSymbol == '*') liveCells++;
                    }
                    writer.WriteLine();
                }

                BigInteger totalCombinations = BigInteger.Pow(2, board.Columns * board.Rows);

                writer.WriteLine($"Live cells: {liveCells}");
                writer.WriteLine($"Total combinations: {totalCombinations}");
            }
        }

        public static void LoadState(string filename)
        {
            // Получаем путь к корневой папке проекта
            string rootPath = @"D:\code\C#\mod-lab04-life-main\Life";
            string fullPath = Path.Combine(rootPath, filename);

            string[] lines = File.ReadAllLines(fullPath);

            int rows = lines.Length;
            int columns = lines.Max(line => line.Replace(" ", "").Length);

            int centerX = board.Columns / 2;
            int centerY = board.Rows / 2;

            int offsetX = centerX - columns / 2;
            int offsetY = centerY - rows / 2;

            for (int y = 0; y < rows && y < board.Rows; y++)
            {
                // Проверка на пустую строку
                if (string.IsNullOrEmpty(lines[y]))
                    continue;

                for (int x = 0; x < columns && x < board.Columns; x++)
                {
                    int newX = x + offsetX;
                    int newY = y + offsetY;

                    // Проверка границ строки
                    if (x < lines[y].Length)
                    {
                        // Проверка границ массива и игнорирование пробелов
                        if (newX >= 0 && newX < board.Columns && newY >= 0 && newY < board.Rows && lines[y][x] != ' ')
                        {
                            board.Cells[newX, newY].IsAlive = lines[y][x] == '*';
                        }
                    }
                }
            }
        }
        public static void ClassifyElements()
        {
            Console.WriteLine("Classifying elements...");

            foreach (var pattern in patterns)
            {
                for (int y = 0; y <= board.Rows - pattern.Value.GetLength(0); y++)
                {
                    for (int x = 0; x <= board.Columns - pattern.Value.GetLength(1); x++)
                    {
                        if (ComparePattern(x, y, pattern.Value))
                        {
                            Console.WriteLine($"Found {pattern.Key} at ({x}, {y})");
                        }
                    }
                }
            }
        }

        // Функция для сравнения образца с фрагментом поля
        public static bool ComparePattern(int x, int y, bool[,] pattern)
        {
            for (int i = 0; i < pattern.GetLength(0); i++)
            {
                for (int j = 0; j < pattern.GetLength(1); j++)
                {
                    if (board.Cells[x + j, y + i].IsAlive != pattern[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        static void BuildStabilityGraph(int simulationsPerDensity, int maxGenerations)
        {
            List<PointF> dataPoints = new List<PointF>();

            for (double density = 0.1; density <= 0.9; density += 0.1)
            {
                int totalGenerations = 0;

                for (int i = 0; i < simulationsPerDensity; i++)
                {
                    board = new Board(width: 50, height: 20, cellSize: 1, liveDensity: density);

                    for (int generation = 0; generation < maxGenerations; generation++)
                    {
                        board.Advance();
                        board.CheckStability();
                        if (board.IsStable)
                        {
                            totalGenerations += generation;
                            break;
                        }
                    }
                }

                float averageGenerations = (float)totalGenerations / simulationsPerDensity;
                dataPoints.Add(new PointF((float)density, averageGenerations));
            }

            DrawGraph(dataPoints, "plot.png");
        }
        static void DrawGraph(List<PointF> dataPoints, string filename)
        {
            int width = 600;
            int height = 400;
            int margin = 50;

            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);

                // Рисуем оси
                graphics.DrawLine(Pens.Black, margin, height - margin, width - margin, height - margin); // Ось X
                graphics.DrawLine(Pens.Black, margin, margin, margin, height - margin); // Ось Y

                // Рисуем метки на оси X (плотность)
                for (float density = 0.1f; density <= 0.9f; density += 0.1f)
                {
                    int x = margin + (int)(density * (width - 2 * margin));
                    graphics.DrawLine(Pens.Black, x, height - margin - 5, x, height - margin + 5);
                    graphics.DrawString(density.ToString("F1"), SystemFonts.DefaultFont, Brushes.Black, x - 10, height - margin + 10);
                }

                // Рисуем метки на оси Y (количество поколений)
                for (int generations = 0; generations <= 1000; generations += 100)
                {
                    int y = height - margin - (int)(generations / 1000f * (height - 2 * margin));
                    graphics.DrawLine(Pens.Black, margin - 5, y, margin + 5, y);
                    graphics.DrawString(generations.ToString(), SystemFonts.DefaultFont, Brushes.Black, margin - 40, y - 5);
                }

                // Рисуем график
                Pen pen = new Pen(Color.Blue, 2);
                for (int i = 1; i < dataPoints.Count; i++)
                {
                    PointF point1 = dataPoints[i - 1];
                    PointF point2 = dataPoints[i];

                    int x1 = margin + (int)(point1.X * (width - 2 * margin));
                    int y1 = height - margin - (int)(point1.Y / 1000f * (height - 2 * margin));
                    int x2 = margin + (int)(point2.X * (width - 2 * margin));
                    int y2 = height - margin - (int)(point2.Y / 1000f * (height - 2 * margin));

                    graphics.DrawLine(pen, x1, y1, x2, y2);
                }
                string rootPath = @"D:\code\C#\mod-lab04-life-main\Life";
                string fullPath = Path.Combine(rootPath, filename);
                bitmap.Save(fullPath, ImageFormat.Png);
            }
        }
        static void Main(string[] args)
        {
            Reset();

            while (true)
            {
                Console.Clear();
                Render();

                Console.WriteLine("Press 'S' to save, 'L' to load, 'G' to build graph, any other key to continue");
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.S)
                {
                    Console.WriteLine("Enter filename to save:");
                    string filename = Console.ReadLine();
                    SaveState(filename);
                    Console.WriteLine("State saved to " + filename);
                }
                else if (key.Key == ConsoleKey.L)
                {
                    Console.WriteLine("Enter filename to load:");
                    string filename = Console.ReadLine();

                    board = new Board(width: board.Width, height: board.Height, cellSize: board.CellSize, liveDensity: 0);

                    LoadState(filename);
                    Console.WriteLine("State loaded from " + filename);
                }
                else if (key.Key == ConsoleKey.G)
                {
                    BuildStabilityGraph(simulationsPerDensity: 100, maxGenerations: 1000);
                    Console.WriteLine("Stability graph saved to plot.png in the root folder.");
                }

                board.Advance();
                Thread.Sleep((int)settings.SimulationDelay);
            }
        }
    }
}