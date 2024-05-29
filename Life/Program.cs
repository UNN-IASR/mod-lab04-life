using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace cli_life
{
    public class Cell //клетка
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;

        public void DetermineNextLiveState() //метод определяет состояние клетки на следующем шаге
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count(); //пробегаемся по списку с лямда выражением (считаем количество живых клеток рядом с нашей)

            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }

        public void Advance() //вызывается, когда все клетки получили статус
        {
            IsAlive = IsAliveNext;
        }
    }

    public class Board //доска
    {
        public readonly Cell[,] Cells; //двумерный массив из клеток
        public readonly int CellSize; //размер ячейки

        //свойства
        public int Columns { get { return Cells.GetLength(0); } } //количество столбцов
        public int Rows { get { return Cells.GetLength(1); } } //колисетво строк
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public int CountGeneration { get; private set; }

        public Board(int width, int height, int cellSize, double liveDensity = .1) //liveDensity - плотность распределения живых и мертвых
        {
            CountGeneration = 1;
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize]; //тк в пикселях
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors(); //соединяет всех соседий друг с другом
            Randomize(liveDensity);
        }

        public Board(Cell[][] cells, int cellSize, int generation)
        {
            if (cells.Length <= 0 || cells[0].Length <= 0)
            {
                throw new ArgumentException();
            }

            CountGeneration = generation;

            CellSize = cellSize;
            Cells = new Cell[cells[0].Length, cells.Length];
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell()
                    {
                        IsAlive = cells[y][x].IsAlive
                    };
                }
            }

            ConnectNeighbors();
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity; //смотря в какой диапозон попала => живая или мертвая
        }

        public void Advance()
        {
            CountGeneration++;

            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }

        public void LoadStateFile(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            for (int row = 0; row < Cells.GetLength(1); row++)
            {
                var line = reader.ReadLine();
                var simbols = line.ToCharArray();
                for (int col = 0; col < Cells.GetLength(0); col++)
                {
                    var cell = Cells[col, row];
                    cell.IsAlive = simbols[col].Equals('*');
                }
            }
        }

        private void ConnectNeighbors() //шарообразный мир
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
    }

    public class Pattern
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public static class WordFile
    {
        public static void SaveStateFile(Board board, string fileName)
        {
            using (var sw = new StreamWriter(fileName, false))
            {
                for (int x = 0; x < board.Columns; x++)
                {
                    string line = "";
                    for (int y = 0; y < board.Rows; y++)
                    {
                        line += board.Cells[x, y].IsAlive ? "*" : " ";
                    }

                    sw.WriteLine(line);
                }
            }
            Console.WriteLine("File saved");
        }

        public static List<Pattern> LoadPatterns(string fileName)
        {
            var patterns = new List<Pattern>();

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                patterns = JsonSerializer.Deserialize<List<Pattern>>(fs);
            }

            return patterns;
        }
    }

    public static class BoardAnalyzer
    {
        public static Dictionary<Pattern, int> CountPatterns(this Board src, IEnumerable<Pattern> patterns)
        {
            var result = new Dictionary<Pattern, int>();

            foreach (var pattern in patterns)
            {
                result[pattern] = CountPattern(src, pattern);
            }

            return result;
        }

        public static Cell[,] CloneWithoutNeighbors(this Cell[,] src)
        {
            int Columns = src.GetLength(0);
            int Rows = src.GetLength(1);
            var cells = new Cell[Columns, Rows];
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    cells[x, y] = new Cell() { IsAlive = src[x, y].IsAlive };
                }
            }

            return cells;
        }

        private static int CountPattern(Board board, Pattern pattern)
        {
            var cells = board.Cells.CloneWithoutNeighbors();
            int count = 0;

            for (int y = 0; y < board.Rows; y++)
            {
                for (int x = 0; x < board.Columns; x++)
                {
                    string str = "";
                    for (int i = 0; i < pattern.Height && i < board.Rows; i++)
                    {
                        for (int j = 0; j < pattern.Width && j < board.Columns; j++)
                        {
                            int xCoord = x + j < board.Columns ? x + j : (x + j) - board.Columns;
                            int yCoord = y + i < board.Rows ? y + i : (i + y) - board.Rows;
                            str += cells[xCoord, yCoord].IsAlive ? '*' : '.';
                        }
                    }

                    if (str == pattern.Value)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static int CountAliveCells(this Board src)
        {
            int aliveCells = 0;
            for (int x = 0; x < src.Rows; x++)
            {
                for (int y = 0; y < src.Columns; y++)
                {
                    if (src.Cells[x, y].IsAlive)
                    {
                        aliveCells++;
                    }
                }
            }

            return aliveCells;
        }
    }

    public class Program
    {
        static Board board; //нужна одна доска, поэтому поле статическое 
        static object boardLocker = new object();

        static public Board Reset()
        {
            var settings = new BoardSettings();

            using (var fs = new FileStream("settings.json", FileMode.Open))
            {
                settings = JsonSerializer.Deserialize<BoardSettings>(fs);
            }
            board = new Board(settings.Width, settings.Height, settings.CellSize, settings.LiveDensity);

            return board;
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

        static void HandleConsoleInput()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey().Key;
                    if (key == ConsoleKey.N)
                    {
                        break;
                    }
                    if (key == ConsoleKey.S)
                    {
                        lock (boardLocker)
                        {
                            WordFile.SaveStateFile(board, $"gen-{board.CountGeneration}.txt");
                        }
                    }
                    if (key == ConsoleKey.L)
                    {
                        board.LoadStateFile("systemStatus.txt");
                    }
                    if (key == ConsoleKey.P)
                    {
                        var patterns = WordFile.LoadPatterns("patterns.json");
                        var result = board.CountPatterns(patterns);

                        foreach (var pc in result)
                        {
                            Console.WriteLine($"{pc.Key.Name}: {pc.Value}");
                        }
                    }
                    if (key == ConsoleKey.C)
                    {
                        int countSym = board.CountAliveCells();
                        Console.WriteLine(countSym);
                    }
                }
            }
        }

        static void Simulate()
        {
            while (true)
            {
                Console.Clear();
                Render();
                lock (boardLocker)
                {
                    board.Advance();
                }
                Thread.Sleep(1000);
            }
        }

        async static Task Main(string[] args)
        {
            Reset();

            var consoleInput = Task.Run(() => HandleConsoleInput());
            var simulation = Task.Run(() => Simulate());

            await Task.WhenAll(consoleInput, simulation);
        }
    }
}