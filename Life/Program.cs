using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ScottPlot;
using System.Text.Json;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Security;

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
        public char toChar()
        {
            if (IsAlive) return '*';
            else return ' ';
        }
    }
    public class Settings
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        private Cell[,] prevCells;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public int Generations { get; private set; }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;
            Generations = 0;
            prevCells = new Cell[width / cellSize, height / cellSize];
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                    prevCells[x, y] = new Cell();
                }
            ConnectNeighbors();
            Randomize(liveDensity);
        }
        public Board(string data)
        {
            CellSize = 1;
            Generations = 0;
            int width = data.IndexOf('\n');
            int height = (data.Length + 1) / (width + 1);
            Cells = new Cell[width, height];
            prevCells = new Cell[width, height];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                {
                    prevCells[x,y] = new Cell();
                    Cells[x, y] = new Cell();
                    if (data[(Columns + 1) * y + x] == '*')
                        Cells[x, y].IsAlive = true;
                    else Cells[x, y].IsAlive = false;
                }
            ConnectNeighbors();
        }
        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }
        public void RememberCells() 
        {
            for (int i = 0; i < Columns; i++)
                for (int j = 0; j < Rows; j++)
                    prevCells[i,j].IsAlive = Cells[i,j].IsAlive;
        }
        public void Advance(bool rememberCells = false)
        {
            if (rememberCells) RememberCells();
            Generations++;
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        public int Count()
        {
            return Cells.Cast<Cell>().Count(x => x.IsAlive == true);
        }
        public bool HasChanged()
        {
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    if (prevCells[j, i].IsAlive != Cells[j, i].IsAlive)
                        return true;
            return false;
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
        public static int ConvergenceTime(int maxGeneratons, Board board)
        {
            board.Advance(true);
            Console.WriteLine(board.toString());
            while ((board.Generations < maxGeneratons) && (board.HasChanged()))
                board.Advance();
            return board.Generations;
        }
        public static readonly ReadOnlyDictionary<string, (string view, int height, int width)> Shapes = CreateShapes();
        private static ReadOnlyDictionary<string, (string, int, int)> CreateShapes()
        {
            var dictionary = new Dictionary<string, (string, int, int)>
            {
                {"block", ("     **  **     ", 4, 4) },
                {"HiveHorizontal", ("        **   *  *   **        ", 5, 6) },
                {"HiveVertical", ("        **   *  *   **        ", 6, 5) },
                {"Loaf", ("         *    * *  *  *   **        ", 6, 6) },
                {"Tub", ("       *   * *   *       ", 5, 5) },
                {"Boat", ("      **   * *   *       ", 5, 5) },
                {"Ship", ("      **   * *   **      ", 5, 5) },
                {"Pond", ("        **   *  *  *  *   **        ", 6, 6) },
                {"Canoe", ("           **      *     *   * *    **           ", 7, 7) },
                {"Snake", ("       ** *  * **       ", 4 ,6) },
                {"AircraftCarrier", ("       **    *  *    **       ", 5, 6) },
                {"StickVertical", ("    *  *  *    ", 5, 3) },
                {"StickHorizontal", ("      ***      ", 3, 5) }
            };
            return new ReadOnlyDictionary<string, (string, int, int)>(dictionary);
        }
        public Dictionary<string, int> SearchForShapes()
        {
            var cells = CellsTrans();
            var results = new Dictionary<string, int>();
            foreach (var shape in Shapes)
            {
                int count = 0;
                for (int i = 0; i < Rows; i++)
                    for (int j = 0; j < Columns; j++)
                    {
                        string str = "";
                        for (int k = 0; k < shape.Value.height; k++)
                            for (int w = 0; w < shape.Value.width; w++)
                                str += cells[(i+k)%Rows, (j+w)%Columns].toChar();
                        if (str.Equals(shape.Value.view)) count++;
                    }
                if (count > 0) results[shape.Key] = count;
            }
            return results;
        }
        public void PrintSearch()
        {
            var result = SearchForShapes();
            if (result.Count > 0)
                foreach (var shape in result)
                    Console.WriteLine($"{shape.Key}: {shape.Value}");
            else Console.WriteLine("Известных фигур не найдено.");
        }
        public Cell[,] CellsTrans()
        {
            var cells = new Cell[Rows, Columns];
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                    cells[row, col] = Cells[col, row];
            }
            return cells;
        }
        public string toString()
        {
            string toString = "";
            for (int i = 0; i < Rows - 1; i++)
            {
                for (int j = 0; j < Columns; j++)
                    toString += Cells[j, i].toChar();
                toString += "\n";
            }
            for (int j = 0; j < Columns; j++)
                toString += Cells[j, Rows - 1].toChar();
            return toString;
        }
    }
    class Program
    {
        static Board board;
        static private void Reset()
        {
            board = new Board(
            width: 50,
            height: 30,
            cellSize: 1,
            liveDensity: 0.5);
        }
        static private Board ResetFromFile()
        {
            using (FileStream fs = File.OpenRead("../../../settings.json"))
            {
                Settings s = JsonSerializer.Deserialize<Settings>(fs);
                board = new Board(
                width: s.Width,
                height: s.Height,
                cellSize: s.CellSize,
                liveDensity: s.LiveDensity);
            }
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
            Console.WriteLine(board.Generations);
        }
        static void Main(string[] args)
        {
            //CreateGraph(new double[] { 0.1, 0.2, 0.3 }, 50, 20, 1000);
            /*Dialog();
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo act = Console.ReadKey();
                    if (act.KeyChar == 'a')
                        Dialog();
                }
                Console.Clear();
                board.Advance();
                Render();
                Thread.Sleep(50);
            }*/
            string view = "" +
                "               \n" +
                "       **      \n" +
                "       **      \n" +
                "               \n" +
                "       *       \n" +
                " **          * \n" +
                " **            \n" +
                "       *       \n" +
                "               \n" +
                "          *    \n" +
                "               \n" +
                "     **        \n" +
                "     **        \n" +
                "               ";
            Board board = new Board(view);
            Console.WriteLine($"{board.Width} , {board.Height}");
        }
        static void Dialog()
        {
            string menu = "\nВыберите действие: " +
            "\n 'w'- загрузить состояние из файла;" +
            "\n 's' - сохранить состояние в файл;" +
            "\n 'd' - начать/продолжить работу;" +
            "\n 'e' - завершение работы;" +
            "\n 'q' - создать новую доску;" +
            "\n 't' - поиск фигур";
            Console.WriteLine(menu);
            while (true)
            {
                Console.Write("Выбор действия: ");
                string action = Console.ReadLine();
                switch (action.Trim())
                {
                    case "w":
                        UploadBoard();
                        Console.Clear();
                        Render();
                        Console.WriteLine(menu);
                        break;
                    case "s":
                        if (board == null)
                            Console.WriteLine("Доска пустая!");
                        else SaveBoard();
                        break;
                    case "d":
                        return;
                    case "e":
                        Environment.Exit(0);
                        break;
                    case "q":
                        ResetFromFile();
                        Console.Clear();
                        Render();
                        Console.WriteLine(menu);
                        break;
                    case "t":
                        Console.Clear();
                        Render();
                        board.PrintSearch();
                        Console.WriteLine("\n" + menu);
                        break;
                }
            }
        }
        static Board BoardFromFile(string name)
        {
            string filePath = "../../../" + name + ".txt";
            Board newBoard;
            try
            {
                using (FileStream fstream = new FileStream(filePath, FileMode.Open))
                {
                    byte[] data = new byte[fstream.Length];
                    fstream.Read(data, 0, data.Length);
                    fstream.Close();
                    newBoard = new Board(Encoding.UTF8.GetString(data).Replace("\r", string.Empty));
                    return newBoard;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        static void UploadBoard()
        {
            Console.Write("Введите название файла: ");
            string filePath = Console.ReadLine();
            filePath = "../../../" + filePath + ".txt";
            using (FileStream fstream = new FileStream(filePath, FileMode.Open))
            {
                try
                {
                    byte[] data = new byte[fstream.Length];
                    fstream.Read(data, 0, data.Length);
                    fstream.Close();
                    board = new Board(Encoding.UTF8.GetString(data).Replace("\r", string.Empty));
                    Console.WriteLine($"Файл успешно считан");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Некорректное название файла.");
                }
                catch (IOException)
                {
                    Console.WriteLine("Файл недоступен.");
                }
            }
            return;
        }

        static void SaveBoard()
        {
            Console.Write("Введите название нового/существующего файла: ");
            string filePath = Console.ReadLine();
            filePath = "../../../" + filePath + ".txt";
            using (FileStream fstream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                try
                {
                    fstream.Write(Encoding.Default.GetBytes(board.toString()));
                    fstream.Close();
                    Console.WriteLine("Файл успешно сохранён");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Некорректное название файла.");
                }
                catch (IOException)
                {
                    Console.WriteLine("Файл недоступен.");
                }
            }
            return;
        }
        static List<(double density, Dictionary<int, int> data)> GetData(double[] densities, int width, int height, int maxGenerations)
        {
            List<(double density, Dictionary<int, int> data)> results = new List<(double density, Dictionary<int, int> data)>(maxGenerations);
            Board board;
            foreach (var density in densities) 
            {
                var data = new Dictionary<int, int>();
                board = new Board(width, height, 1, density);
                data[board.Generations] = board.Count();
                while ((board.Generations < maxGenerations) && (board.HasChanged()))
                {
                    board.Advance();
                    data[board.Generations] = board.Count();
                }
                results.Add((density, data));
            }
            return results;
        }
        static void CreateGraph(double[] densities, int width, int height, int maxGenerations)
        {
            var plot = new Plot();
            plot.XLabel("Generation");
            plot.YLabel("Alive cells");
            plot.ShowLegend();
            var dataPerDensity = GetData(densities, width, height, maxGenerations);
            foreach (var densityData in dataPerDensity)
            {
                var scatter = plot.Add.Scatter(densityData.data.Keys.ToArray(), densityData.data.Values.ToArray());
                scatter.Smooth = true;
                scatter.Label = $"{densityData.density}";
                scatter.Color = RandomColor();
            }
            plot.SavePng("../../../plot.png", 1920, 1080);
        }
        static Color RandomColor()
        {
            int red = 100 + Random.Shared.Next() % 255;
            int green = 100 + Random.Shared.Next() % 255;
            int blue = 100 + Random.Shared.Next() % 255;

            return new Color(red, green, blue);
        }
    }
}