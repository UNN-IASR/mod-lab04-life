using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using Newtonsoft.Json;
using System.IO;
using Life;

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
        public Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        [JsonConstructor]
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
        public Board() { }

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
                    int xL = (x > 0) ? x - 1 : -1;
                    int xR = (x < Columns - 1) ? x + 1 : -1;

                    int yT = (y > 0) ? y - 1 : -1;
                    int yB = (y < Rows - 1) ? y + 1 : -1;


                    if ((xL != -1) && (yT != -1))
                        Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    if (yT != -1)
                        Cells[x, y].neighbors.Add(Cells[x, yT]);
                    if ((xR != -1) && (yT != -1))
                        Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    if (xL != -1)
                        Cells[x, y].neighbors.Add(Cells[xL, y]);
                    if (xR != -1)
                        Cells[x, y].neighbors.Add(Cells[xR, y]);
                    if ((xL != -1) && (yB != -1))
                        Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    if (yB != -1)
                        Cells[x, y].neighbors.Add(Cells[x, yB]);
                    if ((xR != -1) && (yB != -1))
                        Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }

        public void StateWrite(int step, string path)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(this.Columns);
                sw.WriteLine(this.Rows);
                sw.WriteLine(this.Width);
                sw.WriteLine(this.Height);
                sw.WriteLine(this.CellSize);
                sw.WriteLine(step);
                for (int row = 0; row < this.Rows; row++)
                {
                    for (int col = 0; col < this.Columns; col++)
                    {
                        var cell = this.Cells[col, row];
                        if (cell.IsAlive)
                        {
                            sw.Write('*');
                        }
                        else
                        {
                            sw.Write(' ');
                        }
                    }
                    sw.Write("\r\n");
                }
            }
        }
        public void StateRead(string filename)
        {
            string content = File.ReadAllText(filename);
            string[] temp;
            if (content.Contains('\r'))
            {
                temp = content.Split("\r\n");
            }
            else
            {
                temp = content.Split('\n');
            }
            int col = Int32.Parse(temp[0]);
            int row = Int32.Parse(temp[1]);
            int wid = Int32.Parse(temp[2]);
            int hei = Int32.Parse(temp[3]);
            int cel = Int32.Parse(temp[4]);

            for (int trow = 0; trow < row; trow++)
            {
                for (int tcol = 0; tcol < col; tcol++)
                {
                    if (temp[trow + 6][tcol] == '*')
                    {
                        this.Cells[tcol, trow].IsAlive = true;
                    }
                }
            }
        }
        public void StateReadFromString(string content)
        {
            string[] temp;
            if (content.Contains('\r'))
            {
                temp = content.Split("\r\n");
            }
            else
            {
                temp = content.Split('\n');
            }
            int col = Int32.Parse(temp[0]);
            int row = Int32.Parse(temp[1]);
            int wid = Int32.Parse(temp[2]);
            int hei = Int32.Parse(temp[3]);
            int cel = Int32.Parse(temp[4]);

            for (int trow = 0; trow < row; trow++)
            {
                for (int tcol = 0; tcol < col; tcol++)
                {
                    if (temp[trow + 6][tcol] == '*')
                    {
                        this.Cells[tcol, trow].IsAlive = true;
                    }
                }
            }
        }
        public void Render()
        {
            for (int row = 0; row < this.Rows; row++)
            {
                for (int col = 0; col < this.Columns; col++)
                {
                    var cell = this.Cells[col, row];
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
    }
    public class Program
    {
        static void Main(string[] args)
        {
            FigureType ft = new FigureType();
            Board board;
            using (StreamReader file = File.OpenText("jsonConfig.json"))
            {
                Newtonsoft.Json.JsonSerializer ser = new Newtonsoft.Json.JsonSerializer();
                board = (Board)ser.Deserialize(file, typeof(Board));
            }
            int stableStep = ft.StablePhaseRender(board);
            Console.WriteLine("Becomes stable on step - {0}", stableStep);
            Thread.Sleep(5000);

            Board board1 = new Board(50, 20, 1, 0);
            board1.StateRead("LText2.txt");

            stableStep = ft.StablePhase(board1);
            Console.WriteLine("Becomes stable on step - {0}", stableStep);

            board1 = new Board(50, 20, 1, 0);
            board1.StateRead("LText2.txt");
            int step = 0;
            int finalStep = stableStep;
            bool key = true;
            while (key)
            {
                step++;
                if (step == finalStep)
                {
                    key = false;
                }
                Console.Clear();
                board1.Render();
                board1.Advance();
                Thread.Sleep(1000);
            }

            Dictionary<string, int> answer = new Dictionary<string, int>();
            answer = ft.CountAllFigures(board1);
            foreach (var item in answer)
            {
                Console.WriteLine("{0} - {1}", item.Key, item.Value);
            }

        }
    }
}