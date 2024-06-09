using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json;


namespace cli_life
{
    public class Figure
    {
        [JsonProperty("m")]
        public int m { get; set; }
        [JsonProperty("n")]
        public int n { get; set; }
        [JsonProperty("table")]
        public bool[,] table { get; set; } 

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
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        public double liveDensity;
        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public double LiveDensity { get { return liveDensity; } }
        
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
        public void ConnectNeighbors()
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
            Randomize(liveDensity);
        }
    }
    public class Program
    {
        public static Board board;
        static public void Reset()
        {
            var data = File.ReadAllText("Board.json");
            var settings = (data);
            JsonNode forecastNode = JsonNode.Parse(data)!;
            var options = new JsonSerializerOptions { WriteIndented = true };
            board = new Board(
                 width: (int)forecastNode!["Width"]!,
                 height: (int)forecastNode!["Height"]!,
                cellSize: 1,
                liveDensity: (double)forecastNode!["LiveDensity"]!);
        }

        public static void Render()
        {
            double[,] c = new double[board.Columns, board.Rows];
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                        c[col, row] = 1;
                    }
                    else
                    {
                        Console.Write(' ');
                        c[col, row] = 0;
                    }
                }
                Console.Write('\n');
            }
            var plt = new ScottPlot.Plot(800,600);
           plt.AddHeatmap(c);
           plt.SaveFig("1.png");

        }
        static void Main(string[] args)
        {
            read("file2.txt");
            Render();
            Console.ReadLine();
            Console.ReadLine();
        }
        public static void add_figure(string name, int y, int x)
        {
            var data = File.ReadAllText("example.json");
            var settings = (data);
            JsonNode forecastNode = JsonNode.Parse(data)!;
            Figure fig = JsonConvert.DeserializeObject<Figure>(forecastNode![name]!.ToString());
            int rowIndexStart = x-1;
            int rowIndexFinish = fig.m+x-1;
            int colIndexStart = y-1;
            int colIndexFinish = fig.n+y-1;
            int i=0, j = 0;
            for (int row = x - 1; row < rowIndexFinish; row++)
            {
                for (int col=y-1; col < colIndexFinish; col++) 
                {
                    board.Cells[row, col].IsAlive = fig.table[i, j];
                    j++;
                }
                j = 0;
                i++;
            }
            
        }
        public static void save()
        {
            //if (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.B) { 

            string fileName = "SavingFile.txt";
            string str = "";
            StreamWriter f = new StreamWriter(fileName, true);
            f.WriteLine(board.Columns.ToString());
            f.WriteLine(board.Rows.ToString());
            for (int row = 0; row < board.Columns; row++)
            {
                str = "";
                for (int col = 0; col < board.Rows; col++)
                {
                    //var cell = board.Cells[row, col];
                    if (board.Cells[row, col].IsAlive == true)
                    {
                        str = str + "*";
                        //Console.Write('*');
                    }
                    else
                    {
                        str = str + "_";
                        //Console.Write(' ');
                    }
                }
                f.WriteLine(str);
            }
            //Cell cell = board.Cells[col, row];
            f.Close();
        //}
        }
        public static void read(string str)
        {
            bool[,] C;
           
            
                StreamReader f = new StreamReader(str);
                int m = Int32.Parse(f.ReadLine());
                int n = Int32.Parse(f.ReadLine());
                //C = new bool[m, n];
            board = new Board(
                width: n,
                height: m,
                cellSize: 1,
             
                liveDensity: 0.5
            );


            int j = 0;
                while (!f.EndOfStream)
                {
                    string s = f.ReadLine();
                    for (int i = 0; i < n; i++)
                    {
                        if (s[i] == '*')
                        board.Cells[i,j].IsAlive = true;
                        else
                        board.Cells[i, j].IsAlive = false;
                    }
                    // что-нибудь делаем с прочитанной строкой s
                    j++;
                }
                f.Close();
                
         
            
        }
        public static bool symmetry_horizontally()
        {
            
            bool flag = true;
            for (int i=0; i<board.Columns; i++)
            {
                for (int j = 0; j < board.Rows/2; j++)
                {
                    if (board.Cells[i, j].IsAlive != board.Cells[i,board.Rows-1-j].IsAlive)
                        flag = false;   
                }
            }
            return flag;
        }

        public static bool symmetry_vertically()
        {
            bool flag = true;
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns / 2; j++)
                {
                    if (board.Cells[j, i].IsAlive != board.Cells[board.Columns - 1 -j,  i].IsAlive)
                        flag = false;
                }
            }
            return flag;
        }
        public static int find(string name)
        {
            var data = File.ReadAllText("example.json");
            var settings = (data);
            JsonNode forecastNode = JsonNode.Parse(data)!;


            Figure fig = JsonConvert.DeserializeObject<Figure>(forecastNode![name]!.ToString());

            int j = 0;
            int rowIndexStart = 0;
            int rowIndexFinish = fig.m;
            int colIndexStart = 0;
            int colIndexFinish = fig.n;
            bool flag = true;
            int sum = 0;
            while (rowIndexFinish < board.Columns)
            {
                while (colIndexFinish < board.Rows)
                {
                    int i = 0;
                    j = 0;
                    for (int row = rowIndexStart; row < rowIndexFinish; row++)
                    {
                        for (int col = colIndexStart; col < colIndexFinish; col++)
                        {
                            if (board.Cells[row, col].IsAlive != fig.table[i,j])
                            {
                                flag = false;
                                break;
                            }
                            j++;

                        }
                        j = 0;
                        i++;
                    }
                    i = 0;

                    colIndexStart++;
                    colIndexFinish++;
                    if (flag) sum++;
                    flag = true;
                }
                colIndexFinish = fig.n;
                colIndexStart = 0;
                rowIndexStart++;
                rowIndexFinish++;
            }

            return sum;
        }
    }
}
