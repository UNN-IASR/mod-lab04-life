using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

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

        public int countAliveCells()
        {
            int count = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var cell = Cells[col, row];
                    if (cell.IsAlive)
                    {
                        count ++;
                    }
                }
            }
            return count;
        }

        public int countDeadCells()
        {
            int count = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var cell = Cells[col, row];
                    if (!cell.IsAlive)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public string findBlock()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    
                    if(Cells[col, row].IsAlive && !Cells[col, row].neighbors[0].IsAlive && !Cells[col, row].neighbors[1].IsAlive && !Cells[col, row].neighbors[2].IsAlive
                        && !Cells[col, row].neighbors[3].IsAlive && Cells[col, row].neighbors[4].IsAlive && !Cells[col, row].neighbors[5].IsAlive
                        && Cells[col, row].neighbors[6].IsAlive && Cells[col, row].neighbors[7].IsAlive
                        && !Cells[col, row].neighbors[4].neighbors[2].IsAlive && !Cells[col, row].neighbors[4].neighbors[4].IsAlive
                        && !Cells[col, row].neighbors[4].neighbors[7].IsAlive && !Cells[col, row].neighbors[6].neighbors[5].IsAlive
                        && !Cells[col, row].neighbors[6].neighbors[6].IsAlive && !Cells[col, row].neighbors[6].neighbors[7].IsAlive
                        && !Cells[col, row].neighbors[7].neighbors[7].IsAlive)
                    {
                        return " block";
                    }
                }
            }

            return "";
        }

        public string findBox()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {

                    if (Cells[col, row].IsAlive && !Cells[col, row].neighbors[0].IsAlive && !Cells[col, row].neighbors[1].IsAlive && !Cells[col, row].neighbors[2].IsAlive
                        && !Cells[col, row].neighbors[3].IsAlive && !Cells[col, row].neighbors[4].IsAlive && Cells[col, row].neighbors[5].IsAlive
                        && !Cells[col, row].neighbors[6].IsAlive && Cells[col, row].neighbors[7].IsAlive
                        && !Cells[col, row].neighbors[5].neighbors[0].IsAlive && !Cells[col, row].neighbors[5].neighbors[3].IsAlive
                        && !Cells[col, row].neighbors[5].neighbors[5].IsAlive && !Cells[col, row].neighbors[5].neighbors[6].IsAlive && Cells[col, row].neighbors[5].neighbors[7].IsAlive
                        && !Cells[col, row].neighbors[7].neighbors[2].IsAlive && !Cells[col, row].neighbors[7].neighbors[4].IsAlive
                        && !Cells[col, row].neighbors[7].neighbors[7].IsAlive && !Cells[col, row].neighbors[7].neighbors[6].IsAlive 
                        && !Cells[col, row].neighbors[6].neighbors[6].neighbors[5].IsAlive && !Cells[col, row].neighbors[6].neighbors[6].neighbors[6].IsAlive
                        && !Cells[col, row].neighbors[6].neighbors[6].neighbors[7].IsAlive)
                    {
                        return " box";
                    }
                }
            }

            return "";
        }


        public string findHive()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {

                    if (Cells[col, row].IsAlive && !Cells[col, row].neighbors[0].IsAlive && !Cells[col, row].neighbors[1].IsAlive && !Cells[col, row].neighbors[2].IsAlive
                        && !Cells[col, row].neighbors[3].IsAlive && !Cells[col, row].neighbors[4].IsAlive && Cells[col, row].neighbors[5].IsAlive
                        && !Cells[col, row].neighbors[6].IsAlive && Cells[col, row].neighbors[7].IsAlive
                        && !Cells[col, row].neighbors[5].neighbors[0].IsAlive && !Cells[col, row].neighbors[5].neighbors[3].IsAlive
                        && !Cells[col, row].neighbors[5].neighbors[5].IsAlive && Cells[col, row].neighbors[5].neighbors[6].IsAlive && !Cells[col, row].neighbors[5].neighbors[7].IsAlive
                        && !Cells[col, row].neighbors[7].neighbors[2].IsAlive && !Cells[col, row].neighbors[7].neighbors[4].IsAlive
                        && !Cells[col, row].neighbors[7].neighbors[7].IsAlive && Cells[col, row].neighbors[7].neighbors[6].IsAlive && !Cells[col, row].neighbors[5].neighbors[6].neighbors[6].IsAlive
                        && !Cells[col, row].neighbors[5].neighbors[6].neighbors[6].neighbors[3].IsAlive && Cells[col, row].neighbors[5].neighbors[6].neighbors[6].neighbors[4].IsAlive
                        && !Cells[col, row].neighbors[5].neighbors[6].neighbors[6].neighbors[6].IsAlive && !Cells[col, row].neighbors[5].neighbors[6].neighbors[6].neighbors[7].IsAlive
                        && !Cells[col, row].neighbors[7].neighbors[6].neighbors[6].IsAlive && !Cells[col, row].neighbors[7].neighbors[6].neighbors[6].neighbors[5].IsAlive
                        && !Cells[col, row].neighbors[7].neighbors[6].neighbors[6].neighbors[6].IsAlive)
                    {
                        return " hive";
                    }
                }
            }

            return "";
        }

        public string findBoat()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {

                    if (Cells[col, row].IsAlive && !Cells[col, row].neighbors[0].IsAlive && !Cells[col, row].neighbors[1].IsAlive && !Cells[col, row].neighbors[2].IsAlive
                        && !Cells[col, row].neighbors[3].IsAlive && !Cells[col, row].neighbors[4].IsAlive && Cells[col, row].neighbors[5].IsAlive
                        && !Cells[col, row].neighbors[6].IsAlive && Cells[col, row].neighbors[7].IsAlive
                        && !Cells[col, row].neighbors[5].neighbors[0].IsAlive && !Cells[col, row].neighbors[5].neighbors[3].IsAlive
                        && !Cells[col, row].neighbors[5].neighbors[5].IsAlive && !Cells[col, row].neighbors[5].neighbors[6].IsAlive && Cells[col, row].neighbors[5].neighbors[7].IsAlive
                        && !Cells[col, row].neighbors[7].neighbors[2].IsAlive && !Cells[col, row].neighbors[7].neighbors[4].IsAlive
                        && !Cells[col, row].neighbors[7].neighbors[7].IsAlive && Cells[col, row].neighbors[7].neighbors[6].IsAlive
                        && !Cells[col, row].neighbors[6].neighbors[6].neighbors[5].IsAlive && !Cells[col, row].neighbors[6].neighbors[6].neighbors[6].IsAlive
                        && !Cells[col, row].neighbors[6].neighbors[6].neighbors[7].IsAlive && !Cells[col, row].neighbors[7].neighbors[6].neighbors[7].IsAlive)
                    {
                        return " boat";
                    }
                }
            }

            return "";
        }

        public string findShip()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {

                    if (Cells[col, row].IsAlive && !Cells[col, row].neighbors[0].IsAlive && !Cells[col, row].neighbors[1].IsAlive && !Cells[col, row].neighbors[2].IsAlive
                        && !Cells[col, row].neighbors[3].IsAlive && Cells[col, row].neighbors[4].IsAlive && !Cells[col, row].neighbors[5].IsAlive
                        && Cells[col, row].neighbors[6].IsAlive && !Cells[col, row].neighbors[7].IsAlive
                        && !Cells[col, row].neighbors[4].neighbors[4].IsAlive && !Cells[col, row].neighbors[4].neighbors[4].neighbors[1].IsAlive
                        && !Cells[col, row].neighbors[4].neighbors[4].neighbors[4].IsAlive && Cells[col, row].neighbors[4].neighbors[4].neighbors[6].IsAlive
                        && !Cells[col, row].neighbors[4].neighbors[4].neighbors[7].IsAlive && !Cells[col, row].neighbors[6].neighbors[6].IsAlive
                        && !Cells[col, row].neighbors[6].neighbors[6].neighbors[3].IsAlive && !Cells[col, row].neighbors[6].neighbors[6].neighbors[6].IsAlive
                        && !Cells[col, row].neighbors[6].neighbors[6].neighbors[7].IsAlive && Cells[col, row].neighbors[6].neighbors[6].neighbors[4].IsAlive
                        && Cells[col, row].neighbors[7].neighbors[7].IsAlive && !Cells[col, row].neighbors[7].neighbors[7].neighbors[4].IsAlive
                        && !Cells[col, row].neighbors[7].neighbors[7].neighbors[6].IsAlive && !Cells[col, row].neighbors[7].neighbors[7].neighbors[7].IsAlive)
                    {
                        return " ship";
                    }
                }
            }

            return "";
        }

        public int becomeStablePhase(int k, int time)
        {
            if(countAliveCells() == k)
            {
                time++;
            }
            else
            {
                time = 0;
            }
            return time;
        }

        public bool hasHorizontalSymmetry()
        {
            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows / 2; row++)
                {
                        if (!(Cells[col, row].IsAlive && Cells[col, Rows-row-1].IsAlive || !(Cells[col, row].IsAlive) && !(Cells[col, Rows - row-1].IsAlive))){
                            return false;
                        }
                }
            }
            return true;
        }

        public bool hasVertycalSymmetry()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns / 2; col++)
                {
                    if (!(Cells[col, row].IsAlive && Cells[Columns - col - 1, row].IsAlive || !(Cells[col, row].IsAlive) && !(Cells[Columns - col - 1, row].IsAlive)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }



        public void OpenFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                int r = Int32.Parse(reader.ReadLine());
                int c = Int32.Parse(reader.ReadLine());

                for (int row = 0; row < r; row++)
                {
                    string str = reader.ReadLine();
                    int i = 0;
                    foreach (char s in str)
                    {
                        if (s == '*')
                        {
                            Cells[i, row].IsAlive = true;
                            Console.Write('*');
                        }
                        else
                        {
                            Console.Write(' ');
                            Cells[i, row].IsAlive = false;
                        }
                        i++;
                    }
                    Console.Write('\n');
                }

            }
        }

        public void SaveSystem(string path)
        {
            File.WriteAllText(path, string.Empty);
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(Rows);
                writer.WriteLine(Columns);
                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Columns; col++)
                    {
                        var cell = Cells[col, row];
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
        }
    }
    class Program
    {
        static Board board;
        static private void Reset()
        {
            board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("data.json"));
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
            int stopParametr = 5;
            int i = 0;
            int stableTime = 0;
            Reset();
            board.OpenFile("colonies/second.txt");
            //OpenFile("standard_figures/hive.txt");
            while (stopParametr > i)
            {
                int bTime = board.countAliveCells();
                Console.Clear();
                Render();
                //OpenFile("s.txt");
                board.Advance();
                Thread.Sleep(1000);
                //SaveSystem(board, "s.txt");
                i++;
                stableTime = board.becomeStablePhase(bTime,stableTime);
            }
            Console.WriteLine("Alive cells: " + board.countAliveCells());
            Console.WriteLine("Dead cells: " + board.countDeadCells());
            Console.WriteLine("The colony has" + board.findBlock() + board.findBox() + board.findHive() + board.findBoat() + board.findShip());
            Console.WriteLine("The colony is stable for " + stableTime + " generations") ;
            if (board.hasHorizontalSymmetry()) { Console.WriteLine("The colony has horizontal symmetry "); }
            if (board.hasVertycalSymmetry()) { Console.WriteLine("The colony has vertycal symmetry "); }

        }
    }
}