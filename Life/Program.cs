// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using System.Threading;
// using System.IO;
// using System.Text.Json;
// using System.Numerics;
// using ScottPlot;

// namespace cli_life
// {
   
//     public class Figure
//     {
//         public int Width { get; set; }
//         public int Height { get; set; }
//         public string Name { get; set; }
//         public int[] value { get; set; }
        
//         public int[,] ReadFigure()
//         {
//             int[,] mas = new int[Width, Height];
//             int n = 0;
//             for (int i=0; i<Height; i++)
//             {
//                 for(int j=0; j<Width; j++)
//                 {
//                     mas[i, j] = value[n];
//                     n++;
//                 }
//             }
//             return mas;
//         }
//         public static Figure[] GetFigure(string name)
//         {
//             string filename = name;
//             string jsonString = File.ReadAllText(filename);
//             Figure[] figure = JsonSerializer.Deserialize<Figure[]>(jsonString); 
//             return figure;
//         }
//         public static int FindFigure(Figure figure, Board board)
//         {
//             int count = 0;
//             int[,] matrix = new int[figure.Width, figure.Height];
//             int[,] fmatrix = figure.ReadFigure();
//             for (int row = 0; row < board.Rows; row++)
//             {
//                 for (int col = 0; col < board.Columns; col++)
//                 {
//                     for (int i = 0; i < figure.Height; i++)
//                     {
//                         for (int j = 0; j < figure.Width; j++)
//                         {
//                             int x = col + j < board.Columns ? col+j : col + j - board.Columns;
//                             int y = row + i < board.Rows ? row+i : row + i - board.Rows;
//                             if (board.Cells[x, y].IsAlive)  
//                                 matrix[i, j] = 1;
//                             else  
//                                 matrix[i, j] = 0;
//                         }
//                     }
//                     count += CompareFigure(matrix, fmatrix);
//                 }
//             }
//             return count;
//         }
//         static int CompareFigure(int[,] matr1, int[,] matr2)
//         {
//             int result = 1;
//             int rows = matr1.GetUpperBound(0)+1;
//             int columns = matr1.Length / rows;
//             for (int i = 0; i < rows; i++)
//             {
//                 for (int j = 0; j < columns; j++)
//                 {
//                     if (matr1[i, j] != matr2[i, j]) 
//                         result = 0;    
//                 }
//             }
//             return result;
//         }
//     }
//     public class Cell
//     {
//         public bool IsAlive;
//         public readonly List<Cell> neighbors = new List<Cell>();
//         private bool IsAliveNext; 
//         public void DetermineNextLiveState()
//         {
//             int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
//             if (IsAlive)
//                 IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
//             else
//                 IsAliveNext = liveNeighbors == 3;
//         }
//         public void Advance()
//         {
//             IsAlive = IsAliveNext;
//         }
//     }
//     public class Board
//     {
//         public List<string> positions = new List<string>();
//         public Cell[,] Cells;
//         public readonly int CellSize;

//         public int Columns { get { return Cells.GetLength(0); } }
//         public int Rows { get { return Cells.GetLength(1); } }
//         public int Width { get { return Columns * CellSize; } }
//         public int Height { get { return Rows * CellSize; } }

//         public Board(int width, int height, int cellSize, double liveDensity = .1)
//         {
//             CellSize = cellSize;

//             Cells = new Cell[width / cellSize, height / cellSize];
//             for (int x = 0; x < Columns; x++)
//                 for (int y = 0; y < Rows; y++)
//                     Cells[x, y] = new Cell();

//             ConnectNeighbors();
//             Randomize(liveDensity);
//         }
//         public Board(int cellSize, Cell[,] cells)
//         {
//             CellSize = cellSize;
//             Cells = cells;
//             ConnectNeighbors();
//         }
//         readonly Random rand = new Random();
//         public void Randomize(double liveDensity)
//         {
//             foreach (var cell in Cells)
//                 cell.IsAlive = rand.NextDouble() < liveDensity;
//         }
//         public int CellsAliveCount()
//         {
//             int count = 0;
//             foreach (Cell cell in Cells)
//             {
//                 if (cell.IsAlive)
//                     count++;
//             }
//             return count;
//         }
//         public void Advance()
//         {
//             foreach (var cell in Cells)
//                 cell.DetermineNextLiveState();
//             foreach (var cell in Cells)
//                 cell.Advance();
//         }
//         public string RecordPositions(Board board)
//         {
//             string str = "";
//             foreach(Cell cell in board.Cells)
//             {
//                 if (cell.IsAlive)
//                     str += '1';
//                 else 
//                     str += '0';
//             }
//             return str;

//         }
//         private void ConnectNeighbors()
//         {
//             for (int x = 0; x < Columns; x++) 
//             {
//                 for (int y = 0; y < Rows; y++)
//                 {
//                     int xL = (x > 0) ? x - 1 : Columns - 1;
//                     int xR = (x < Columns - 1) ? x + 1 : 0;
//                     int yT = (y > 0) ? y - 1 : Rows - 1;
//                     int yB = (y < Rows - 1) ? y + 1 : 0;

//                     Cells[x, y].neighbors.Add(Cells[xL, yT]);
//                     Cells[x, y].neighbors.Add(Cells[x, yT]);
//                     Cells[x, y].neighbors.Add(Cells[xR, yT]);
//                     Cells[x, y].neighbors.Add(Cells[xL, y]);
//                     Cells[x, y].neighbors.Add(Cells[xR, y]);
//                     Cells[x, y].neighbors.Add(Cells[xL, yB]);
//                     Cells[x, y].neighbors.Add(Cells[x, yB]);
//                     Cells[x, y].neighbors.Add(Cells[xR, yB]);
//                 }
//             }
//         }
//         public void Upload(string name)
//         {
//             string[] str = File.ReadAllLines(name);
//             Cell[,] newCells = new Cell[Columns, Rows];
//             for (int row = 0; row <  Rows; row++)
//             {
//                 for (int col = 0; col < Columns; col++)
//                 {
//                     if (str[row][col] == '1') newCells[col, row] = new Cell {IsAlive = true};
//                     if (str[row][col] == '0') newCells[col, row] = new Cell {IsAlive = false};
//                 }
//             }
//             Cells =  newCells;
//             ConnectNeighbors();
//         }
//     }

//      public class Settings
//     {
//         public int Width { get; set; }
//         public int Height { get; set; }
//         public int cellSize { get; set; }
//         public double liveDensity { get; set; }
//     }
//     public class CrateGraph
//     {
//         public static Dictionary<int, int> AliveInGeneration(double density) {
//             var res = new Dictionary<int, int>();
//             Board board = new Board(100, 30, 1, density);
//             while (true) {
//                 res.Add(board.positions.Count, board.CellsAliveCount());
//                 if (!board.positions.Contains(board.RecordPositions(board)))
//                 {
//                     board.positions.Add(board.RecordPositions(board));
//                 }
//                 else 
//                 {
//                     break;
//                 }
//                 board.Advance();
//             }
//             return res;
//         }
//         public static List<Dictionary<int, int>> CreateList(List<double> density, int count) {
//             var list = new List<Dictionary<int,int>>();
//             for(int i=0; i<count; i++) {
//                 if(density[i] < 0.3 || density[i] > 0.5) break;
//                 list.Add(AliveInGeneration(density[i]));
//             }
//             list.Sort((x, y) => x.Count - y.Count);
//             return list;
//         }
//         public static void GraphCrate()
//         {
//             var plot = new Plot();
//             plot.XLabel("generation");
//             plot.YLabel("alive cells");
//             plot.ShowLegend();
//             Random rnd = new Random();
//             List<double> density = new List<double>() {0.3, 0.4, 0.5};
//             var list = CreateList(density, density.Count);
//             int count = 0;
//             foreach(var item in list) {
//                 var scatter = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
//                 scatter.Label = density[count].ToString();
//                 scatter.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
//                 count++;
//             }
//             plot.SavePng("plot.png", 1920, 1080);
//         }     
//     }
//     public class Program
//     {
//         static Board board;
//         static private void Reset()
//         {
//             string filename = "../config.json";
//             string jsonString = File.ReadAllText(filename);
//             Settings settings = JsonSerializer.Deserialize<Settings>(jsonString);
//             board = new Board(
//                 width: settings.Width,
//                 height: settings.Height,
//                 cellSize: settings.cellSize,
//                 liveDensity: settings.liveDensity);
//         }
//         static void Render()
//         {
//             for (int row = 0; row < board.Rows; row++)
//             {
//                 for (int col = 0; col < board.Columns; col++)   
//                 {
//                     var cell = board.Cells[col, row];
//                     if (cell.IsAlive)
//                     {
//                         Console.Write('*');
//                     }
//                     else
//                     {
//                         Console.Write(' ');
//                     }
//                 }
//                 Console.Write('\n');
//             }
//         }
//         static void Save()
//         {
//             string filename;
//             filename = "SavedBoard.txt";
//             StreamWriter sw = new StreamWriter(filename);
//             for (int row = 0; row < board.Rows; row++)
//             {
//                 for (int col = 0; col < board.Columns; col++)
//                 {
//                     var cell = board.Cells[col, row];
//                     if (cell.IsAlive)
//                     {
//                         sw.Write('1');
//                     }
//                     else
//                     {
//                         sw.Write('0');
//                     }
//                 }
//                 sw.Write('\n');
//             }
//             sw.Close();
//         }
//         static void Main(string[] args)
//         {
//             CrateGraph.GraphCrate();
//             Reset();
//             Figure[] fig = Figure.GetFigure("../figure.json");
//             string name;
//             int count = 0;
//             int ac = 0;
//             int CountPositions = 0;
//             bool flag = true;
//             while(flag)
//             {
//                 if(Console.KeyAvailable)
//                 {
//                     ConsoleKeyInfo key = Console.ReadKey();
//                     if (key.KeyChar == 'q')
//                         flag = false;
//                     else if (key.KeyChar == 's') 
//                         Save();
//                     else if (key.KeyChar == 'u')
//                         board.Upload("SavedBoard.txt");
//                 }
//                 //Console.Clear();
//                 Render();
//                 ac = board.CellsAliveCount();
//                 Console.WriteLine("Количество живых клеток " + ac);
//                 for (int j = 0; j<fig.Length;j++)
//                 {
//                     name = fig[j].Name;
//                     count = Figure.FindFigure(fig[j], board);
//                     Console.WriteLine(name + " " + count);
//                 }
//                 if (!board.positions.Contains(board.RecordPositions(board)))
//                     board.positions.Add(board.RecordPositions(board));
//                 else 
//                     flag = false;
//                 CountPositions = board.positions.Count;
//                 Console.WriteLine("Количество поколений " + CountPositions);
//                 board.Advance();
//                 Thread.Sleep(1);
//             }
//         }
//     }
// }

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;
using System.Text.Json;
using ScottPlot;

namespace cli_life
{
    public class Figure
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public int[] value { get; set; }

        public int[,] ReadFigure()
        {
            int[,] mas = new int[Width, Height];
            int n = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    mas[i, j] = value[n];
                    n++;
              }
            }
            return mas;
        }

        public static Figure[] GetFigure(string name)
        {
            string filename = Path.Combine(name);
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"File '{filename}' not found.");
            }

            string jsonString = File.ReadAllText(filename);
            Figure[] figure = JsonSerializer.Deserialize<Figure[]>(jsonString);
            return figure;
        }

        public static int FindFigure(Figure figure, Board board)
        {
            int count = 0;
            int[,] matrix = new int[figure.Width, figure.Height];
            int[,] fmatrix = figure.ReadFigure();
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    for (int i = 0; i < figure.Height; i++)
                    {
                        for (int j = 0; j < figure.Width; j++)
                        {
                            int x = col + j < board.Columns ? col + j : col + j - board.Columns;
                            int y = row + i < board.Rows ? row + i : row + i - board.Rows;
                            if (board.Cells[x, y].IsAlive)
                                matrix[i, j] = 1;
                            else
                                matrix[i, j] = 0;
                        }
                    }
                    count += CompareFigure(matrix, fmatrix);
                }
            }
            return count;
        }

        static int CompareFigure(int[,] matr1, int[,] matr2)
        {
            int result = 1;
            int rows = matr1.GetUpperBound(0) + 1;
            int columns = matr1.Length / rows;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (matr1[i, j] != matr2[i, j])
                        result = 0;
                }
            }
            return result;
        }
    }

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
        public List<string> positions = new List<string>();
        public Cell[,] Cells;
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

        public Board(int cellSize, Cell[,] cells)
        {
            CellSize = cellSize;
            Cells = cells;
            ConnectNeighbors();
        }

        readonly Random rand = new Random();

        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public int CellsAliveCount()
        {
            int count = 0;
            foreach (Cell cell in Cells)
            {
                if (cell.IsAlive)
                    count++;
            }
            return count;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }

        public string RecordPositions(Board board)
        {
            string str = "";
            foreach (Cell cell in board.Cells)
            {
                if (cell.IsAlive)
                    str += '1';
                else
                    str += '0';
            }
            return str;
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

        public void Upload(string name)
        {
            string[] str = File.ReadAllLines(name);
            Cell[,] newCells = new Cell[Columns, Rows];
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (str[row][col] == '1') newCells[col, row] = new Cell { IsAlive = true };
                    if (str[row][col] == '0') newCells[col, row] = new Cell { IsAlive = false };
                }
            }
            Cells = newCells;
            ConnectNeighbors();
        }
    }

    public class Settings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int cellSize { get; set; }
        public double liveDensity { get; set; }
    }

    public class CrateGraph
    {
        public static Dictionary<int, int> AliveInGeneration(double density)
        {
            var res = new Dictionary<int, int>();
            Board board = new Board(100, 30, 1, density);
            while (true)
            {
                res.Add(board.positions.Count, board.CellsAliveCount());
                if (!board.positions.Contains(board.RecordPositions(board)))
                {
                    board.positions.Add(board.RecordPositions(board));
                }
                else
                {
                    break;
                }
                board.Advance();
            }
            return res;
        }

        public static List<Dictionary<int, int>> CreateList(List<double> density, int count)
        {
            var list = new List<Dictionary<int, int>>();
            for (int i = 0; i < count; i++)
            {
                if (density[i] < 0.3 || density[i] > 0.5) break;
                list.Add(AliveInGeneration(density[i]));
            }
            list.Sort((x, y) => x.Count - y.Count);
            return list;
        }

        public static void GraphCrate()
        {
            var plot = new Plot();
            plot.XLabel("generation");
            plot.YLabel("alive cells");
            plot.ShowLegend();
            Random rnd = new Random();
            List<double> density = new List<double>() { 0.3, 0.4, 0.5 };
            var list = CreateList(density, density.Count);
            int count = 0;
            foreach (var item in list)
            {
                var scatter = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                scatter.Label = density[count].ToString();
                scatter.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                count++;
            }
            plot.SavePng("plot.png", 1920, 1080);
        }
    }

    public class Program
    {
        static Board board;

        static private void Reset()
        {
            string filename = "../config.json";
            string jsonString = File.ReadAllText(filename);
            Settings settings = JsonSerializer.Deserialize<Settings>(jsonString);
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
                    Console.Write(cell.IsAlive ? '*' : ' ');
                }
                Console.Write('\n');
            }
        }

        static void Save()
        {
            string filename;
            filename = "SavedBoard.txt";
            StreamWriter sw = new StreamWriter(filename);
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    sw.Write(cell.IsAlive ? '1' : '0');
                }
                sw.Write('\n');
            }
            sw.Close();
        }

        static void Main(string[] args)
        {
            CrateGraph.GraphCrate();
            Reset();
            Figure[] fig = Figure.GetFigure("../figure.json");
            string name;
            int count = 0;
            int ac = 0;
            int CountPositions = 0;
            bool flag = true;
            while (flag)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.KeyChar == 'q')
                        flag = false;
                    else if (key.KeyChar == 's')
                        Save();
                    else if (key.KeyChar == 'u')
                        board.Upload("SavedBoard.txt");
                }
                Render();
                ac = board.CellsAliveCount();
                Console.WriteLine($"Количество живых клеток {ac}");
                for (int j = 0; j < fig.Length; j++)
                {
                    name = fig[j].Name;
                    count = Figure.FindFigure(fig[j], board);
                    Console.WriteLine($"{name} {count}");
                }
                if (!board.positions.Contains(board.RecordPositions(board)))
                    board.positions.Add(board.RecordPositions(board));
                else
                    flag = false;
                CountPositions = board.positions.Count;
                Console.WriteLine($"Количество поколений {CountPositions}");
                board.Advance();
                Thread.Sleep(0);
            }
        }
    }
}