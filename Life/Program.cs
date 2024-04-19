using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SkiaSharp;
using System.Text.Json;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;

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
        public Board(string data)
        {
            CellSize = 1;
            int width = data.IndexOf('\n');
            int height = (data.Length + 1) / (width + 1);
            Cells = new Cell[width, height];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                    if (data[Rows * y + x] == '*')
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
        }
        static void Main(string[] args)
        {
            Dialog();
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
                Thread.Sleep(500);
            }
        }
        static void Dialog()
        {
            string menu = "\nВыберите действие: " +
            "\n 'w'- загрузить состояние из файла;" +
            "\n 's' - сохранить состояние в файл;" +
            "\n 'd' - начать/продолжить работу;" +
            "\n 'e' - завершение работы;" +
            "\n 'q' - создать новую доску";
            Console.WriteLine(menu);
            while (true)
            {
                Console.Write("Выбор действия: ");
                string action = Console.ReadLine();
                switch (action.Trim())
                {
                    case "w":
                        UploadBoard();
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
                }
            }
        }
        static void UploadBoard()
        {
            Console.WriteLine("Введите название файла: ");
            string filePath = Console.ReadLine();
            filePath = "../../../" + filePath + ".txt";
            using (FileStream fstream = new FileStream(filePath, FileMode.Open))
            {
                try
                {
                    byte[] data = new byte[fstream.Length];
                    fstream.Read(data, 0, data.Length);
                    fstream.Close();
                    board = new Board(Encoding.Default.GetString(data));
                    Console.WriteLine($"Файл успешно считан");
                    Render();
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
            Console.WriteLine("Введите название нового/существующего файла: ");
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
    }
}