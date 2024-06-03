using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using ScottPlot;
using System.Reflection.Emit;

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

        public List<string> Positions = new List<string>();
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
        public int BlocksCount()
        {
            int count = 0;
            for (int i = 1; i < Rows - 2; i++)
            {
                for (int j = 1; j < Columns - 2; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j, i + 1].IsAlive && Cells[j + 1, i].IsAlive && Cells[j + 1, i + 1].IsAlive)
                    {
                        if (!Cells[j - 1, i - 1].IsAlive && !Cells[j, i - 1].IsAlive && !Cells[j + 1, i - 1].IsAlive && !Cells[j + 2, i - 1].IsAlive
                        && !Cells[j - 1, i + 2].IsAlive && !Cells[j, i + 2].IsAlive && !Cells[j + 1, i + 2].IsAlive && !Cells[j + 2, i + 2].IsAlive
                        && !Cells[j - 1, i].IsAlive && !Cells[j + 2, i].IsAlive && !Cells[j - 1, i + 2].IsAlive && !Cells[j + 2, i + 2].IsAlive)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }
        public string Note_Positions(Board board)
        {
            string str = "";
            foreach (Cell cell in board.Cells)
            {
                if (cell.IsAlive)
                {
                    str += '+';
                }
                else
                {
                    str += "-";
                }
            }
            return str;
        }
    }
    public class LifeGame
    {
        public  Board board;
        public void Reset()
        {
            string json = File.ReadAllText("../../../Settings.json");
            board = JsonConvert.DeserializeObject<Board>(json);
        }
        public int GetWidth()
        {
            return board.Width;
        }
        public int GetHeight()
        {
            return board.Height;
        }
        public int GetCellSize()
        {
            return board.CellSize;
        }
        public void Render()
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
        public void SaveBoard()
        {
            string filename = "../../../life_stop_and_start.txt";
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Columns; col++)
                    {
                        var cell = board.Cells[col, row];
                        if (cell.IsAlive)
                        {
                            writer.Write('*');
                        }
                        else
                        {
                            writer.Write(' ');
                        }
                    }
                    writer.Write('\n');
                }
            }
            Console.WriteLine("\nФайл сохранен...");
        }

        public void ReadBoard(string FileName)
        {
            int Rows = board.Rows;
            int Columns = board.Columns;
            // открываем файл и создаем объект StreamReader
            using (StreamReader reader = new StreamReader(FileName))
            {
                int CurRow = 0;
                int CurCol = 0;
                // читаем содержимое файла построчно и выводим на консоль
                int symbol;
                while ((symbol = reader.Read()) != -1)
                {
                    if (symbol == '\n') // переход на следующую строку
                    {
                        CurRow++;
                        CurCol = 0;
                    }
                    else // записываем символ в массив
                    {
                        var cell = board.Cells[CurCol, CurRow];
                        if ((char)symbol == '*')
                        {
                            cell.IsAlive = true;
                        }
                        else
                        {
                            cell.IsAlive = false;
                        }
                        CurCol++;
                    }
                }
            }
        }
        public int CountAliveCells()
        {
            int value = 0;
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        value++;
                    }
                }
                Console.Write('\n');
            }
            return value;
        }
        public void Advance()
        {
            board.Advance();
        }

        public int BlocksCount()
        {
            return board.BlocksCount();
        }

    }
    public class Graphics_Life()
    {
        public static Dictionary<int, int> Alive_in_this_generation(double density)
        {
            var result = new Dictionary<int, int>();
            LifeGame LG = new LifeGame();
            LG.Reset();
            while (true)
            {
                result.Add(LG.board.Positions.Count, LG.CountAliveCells());
                if (!LG.board.Positions.Contains(LG.board.Note_Positions(LG.board)))
                {
                    LG.board.Positions.Add(LG.board.Note_Positions(LG.board));
                }
                else
                {
                    break;
                }
                LG.board.Advance();
            }
            return result;
        }

        public static List<Dictionary<int, int>> Creation_List(List<double> density, int count)
        {
            var list = new List<Dictionary<int, int>>();
            for (int i = 0; i < count; i++)
            {
                if (density[i] < 0.3 || density[i] > 0.5)
                {
                    break;
                }
                list.Add(Alive_in_this_generation(density[i]));
            }
            list.Sort((x, y) => x.Count - y.Count);
            return list;
        }

        public static void Make_Graph_png()
        {

            var plot = new Plot();
            plot.XLabel("Поколение");
            plot.YLabel("Живые клетки");
            plot.ShowLegend();
            Random rnd = new Random();
            List<double> density = new List<double>() { 0.3, 0.4, 0.5 };
            var checklist = Creation_List(density, density.Count);
            int count = 0;
            foreach (var item in checklist)
            {
                var scatter = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                scatter.Label = density[count].ToString();
                scatter.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                count++;
            }
            string save_graph = "../../../plot.png";
            plot.SavePng(save_graph, 1920, 1080);
        }
    }


public class Program
    {
        static void Main(string[] args)
        {
            Graphics_Life.Make_Graph_png();
            LifeGame LG = new LifeGame();
            LG.Reset();
            while (true)
            {
                Console.Clear();
                LG.Render();
                Console.WriteLine("Меню:\n1. Сохранить\n2. Загрузить\n3. Продолжить\n4. Подсчитать количество живых клеток\n5. Посчитать количество блоков");
                Console.Write("Выбирите действие: ");
                char Key = Console.ReadKey().KeyChar;
                if (Key == '1')
                {
                    LG.SaveBoard();
                    Console.WriteLine("Для продолжение нажмите любую клавишу");
                    Console.ReadKey();
                }
                else if (Key == '2')
                {
                    Console.Clear();
                    
                    string FileName = "../../../life_stop_and_start.txt";
                    LG.ReadBoard(FileName);
                    Console.WriteLine("Файл загружен...\nДля продолжение нажмите любую клавишу");
                    Console.ReadKey();
                }
                else if (Key == '3')
                {
                    LG.Advance();
                }
                else if (Key == '4')
                {
                    Console.WriteLine("Живых клеток: " + LG.CountAliveCells());
                    Console.ReadKey();
                }
                else if (Key == '5')
                {
                    Console.WriteLine("Количество блоков: " + LG.BlocksCount());
                    Console.ReadKey();
                }
            }
        }
    }
}
