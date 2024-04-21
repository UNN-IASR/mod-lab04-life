using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Numerics;
using System.Text.Json;
using ScottPlot;

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
        public /*readonly*/ Cell[,] Cells;
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

        //Загрузка данных из saveBoard.txt
        public void Load_data_from_file(string filename)
        {
            string[] From_data = File.ReadAllLines(filename);
            Cell[,] Add_NeW_Cells = new Cell[Columns, Rows];
            for(int rows = 0; rows < Rows; rows++)
            {
                for(int cols = 0; cols < Columns; cols++)
                {
                    if (From_data[rows][cols] == '+')
                    {
                        Add_NeW_Cells[cols, rows] = new Cell { IsAlive = true }; 
                    }
                    else if(From_data[rows][cols] == '-')
                    {
                        Add_NeW_Cells[cols, rows] = new Cell { IsAlive = false };
                    }
                }
            }
            Cells = Add_NeW_Cells;
            ConnectNeighbors();
        }

        public Board(int Size_Cell, Cell[,] cells)
        {
            CellSize = Size_Cell;
            Cells = cells;
            ConnectNeighbors();
        }

        //Подсчёт выживших клеток 
        public int Count_Alive_in_Cells()
        {
            int Survivors_account = 0;
            foreach(Cell cell in Cells)
            {
                if (cell.IsAlive)
                {
                    Survivors_account++;
                }
            }
            return Survivors_account;
        }

        //Записываем клетки
        public string Note_Positions(Board board)
        {
            string str = "";
            foreach(Cell cell in board.Cells)
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

    public class Settings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int cellSize { get; set; }
        public double liveDensity { get; set; }
    }

    public class Figures()
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public int[] Value { get; set; }

        public static Figures[] Load_figures_from_file(string filename)
        {
            string jsonString = File.ReadAllText(filename);
            Figures[]? figures = JsonSerializer.Deserialize<Figures[]>(jsonString);
            return figures;
        }

        public int[,] Read_figures()
        {
            int[,] massif = new int[Width, Height];
            int point = 0;
            for (int i = 0; i < Height; i++)
            {
                for(int j = 0; j < Width; j++)
                {
                    massif[i, j] = Value[point];
                    point++;
                }
            }

            return massif;
        }

        static int Compare_Figures(int[,] matrix1, int[,] matrix2)
        {
            int result = 1;
            int rows = matrix1.GetUpperBound(0)+1;
            //Размерность матрицы
            int dimension_of_matrix = matrix1.Length / rows;

            for(int i = 0; i < rows; i++)
            {
                for(int j = 0;j < dimension_of_matrix; j++)
                {
                    if (matrix1[i,j] != matrix2[i,j])
                    {
                        result = 0;
                    }
                }
            }
            return result;
        }

        public static int Find_Figures(Figures figures, Board board)
        {
            int counts_figures = 0;
            int[,] matrix = new int[figures.Width, figures.Height];
            int[,] figures_matrix = figures.Read_figures();
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    for (int height = 0; height < figures.Height; height++)
                    {
                        for (int width = 0; width < figures.Width; width++)
                        {
                            int x = col + width < board.Columns 
                                ? col + width : col + width - board.Columns;

                            int y = row + height < board.Rows 
                                ? row + height : row + height - board.Rows;

                            if (board.Cells[x, y].IsAlive)
                            {
                                matrix[height, width] = 1;
                            }
                            else
                            {
                                matrix[height, width] = 0;
                            }
                        }
                    }
                    counts_figures += Compare_Figures(matrix, figures_matrix);
                }
            }
            return counts_figures;
        }
    }

    public class Graphics_date()
    {
        public static Dictionary<int, int> Alive_in_this_generation(double density)
        {
            var result = new Dictionary<int, int>();
            Board board = new Board(100, 30, 1, density);
            while (true)
            {
                result.Add(board.Positions.Count, board.Count_Alive_in_Cells());
                if (!board.Positions.Contains(board.Note_Positions(board)))
                {
                    board.Positions.Add(board.Note_Positions(board));
                }
                else
                {
                    break;
                }
                board.Advance();
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

        public static void Creating_a_graphic_image()
        {

            var plot = new Plot();
            plot.XLabel("Generations");
            plot.YLabel("Living cells");
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

    class Program
    {
        static Board board;

        static private void Reset()
        {
            //Считываем настройки из settings.json для размера Board
            string filename = "../../../settings.json";
            string js = File.ReadAllText(filename);
            Settings settings = new Settings();
            settings = JsonSerializer.Deserialize<Settings>(js);
            board = new Board(
                width: settings.Width,
                height: settings.Height,
                cellSize: settings.cellSize,
                liveDensity: settings.liveDensity);
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

        //Сохранение данных из saveBoard.txt
        static void Save()
        {
            string save_filename = "../../../saveBoard.txt";
            StreamWriter streamWriter = new StreamWriter(save_filename);
            for (int row = 0; row < board.Rows; row++)
            {
                for(int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        streamWriter.Write('+');
                    }
                    else
                    {
                        streamWriter.Write('-');
                    }
                }
                streamWriter.WriteLine();
            }
            streamWriter.Close();
        }

        static void Main(string[] args)
        {
            Graphics_date.Creating_a_graphic_image();
            string save_filename = "../../../saveBoard.txt";
            string figures_filename = "../../../figures.json";

            Reset();

            Figures[] figures = Figures.Load_figures_from_file(figures_filename);
            string name;
            int count = 0;
            int ac = 0;
            int CountPositions = 0;

            bool Flag = true;
            while (Flag)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.KeyChar == 'q')
                    {
                        Flag = false;
                    }
                    else if (key.KeyChar == 's')
                    {
                        Save();
                    }
                    else if (key.KeyChar == 'u')
                    {
                        board.Load_data_from_file(save_filename);
                    }
                }
                Console.Clear();
                Render();
                ac = board.Count_Alive_in_Cells();
                Console.WriteLine("\n\nКоличество живых клеток " + ac);
                for (int j = 0; j < figures.Length; j++)
                {
                    name = figures[j].Name;
                    count = Figures.Find_Figures(figures[j], board);
                    Console.WriteLine(name + " " + count);
                }
                if (!board.Positions.Contains(board.Note_Positions(board)))
                {
                    board.Positions.Add(board.Note_Positions(board));
                }
                else
                {
                    Flag = false;
                }
                CountPositions = board.Positions.Count;
                Console.WriteLine("Количество поколений " + CountPositions);
                board.Advance();
                Thread.Sleep(10);
            }
        }
    }
}