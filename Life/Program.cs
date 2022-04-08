using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace GameLife
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
        public readonly int MaxCounter;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(Parameters parameters)
        {
            CellSize = parameters.cellSize;
            MaxCounter = parameters.maxCounter;

            Cells = new Cell[parameters.width / parameters.cellSize, parameters.height / parameters.cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(parameters.liveDensity);
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
        public int GetAliveCells()
        {
            int counter = 0;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Cells[col, row].IsAlive) counter++;
                }
            }

            return counter;
        }

        public int GetDeadCells()
        {
            int counter = 0;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (!Cells[col, row].IsAlive) counter++;
                }
            }

            return counter;
        }

        public double GetDensity()
        {
            double density = (double)GetAliveCells() / (GetAliveCells() + GetDeadCells());
            return density;
        }

        public int CountBlocks()
        {
            int counter = 0;

            for (int row = 0; row < Rows - 2; row++)
            {
                for (int col = 0; col < Columns - 2; col++)
                {
                    if (Cells[col, row].IsAlive && Cells[col, row + 1].IsAlive && Cells[col + 1, row].IsAlive && Cells[col + 1, row + 1].IsAlive)
                    {
                        if (!Cells[col - 1, row - 1].IsAlive && !Cells[col, row - 1].IsAlive && !Cells[col + 1, row - 1].IsAlive && !Cells[col + 2, row - 1].IsAlive)
                        {
                            if (!Cells[col - 1, row].IsAlive && !Cells[col + 2, row].IsAlive && !Cells[col - 1, row + 2].IsAlive && !Cells[col + 2, row + 2].IsAlive)
                            {
                                if (!Cells[col - 1, row + 2].IsAlive && !Cells[col, row + 2].IsAlive && !Cells[col + 1, row + 2].IsAlive && !Cells[col + 2, row + 2].IsAlive)
                                {
                                    counter++;
                                }
                            }
                        }
                    }
                }
            }

            return counter;
        }

        public int CountBox()
        {
            int counter = 0;

            for (int row = 0; row < Rows - 2; row++)
            {
                for (int col = 1; col < Columns - 1; col++)
                {
                    if (Cells[col, row].IsAlive && Cells[col - 1, row + 1].IsAlive && Cells[col + 1, row + 1].IsAlive && Cells[col, row + 2].IsAlive)
                    {
                        if (!Cells[col, row + 1].IsAlive && !Cells[col - 1, row].IsAlive && !Cells[col + 1, row].IsAlive && !Cells[col - 1, row + 2].IsAlive && !Cells[col + 1, row + 2].IsAlive)
                        {
                            counter++;
                        }
                    }
                }
            }

            return counter;
        }

        public int CountBoat()
        {
            int counter = 0;

            for (int row = 0; row < Rows - 2; row++)
            {
                for (int col = 1; col < Columns - 1; col++)
                {
                    if (Cells[col, row].IsAlive && Cells[col - 1, row + 1].IsAlive && Cells[col + 1, row + 1].IsAlive && Cells[col, row + 2].IsAlive && Cells[col + 1, row + 2].IsAlive)
                    {
                        if (!Cells[col, row + 1].IsAlive && !Cells[col - 1, row].IsAlive && !Cells[col + 1, row].IsAlive && !Cells[col - 1, row + 2].IsAlive)
                        {
                            counter++;
                        }
                    }
                }
            }

            return counter;
        }

        public int CountShip()
        {
            int counter = 0;

            for (int row = 0; row < Rows - 2; row++)
            {
                for (int col = 1; col < Columns - 1; col++)
                {
                    if (Cells[col, row].IsAlive && Cells[col - 1, row + 1].IsAlive && Cells[col + 1, row + 1].IsAlive && Cells[col, row + 2].IsAlive && Cells[col + 1, row + 2].IsAlive && Cells[col - 1, row].IsAlive)
                    {
                        if (!Cells[col, row + 1].IsAlive && !Cells[col + 1, row].IsAlive && !Cells[col - 1, row + 2].IsAlive)
                        {
                            counter++;
                        }
                    }
                }
            }

            return counter;
        }

        public int CountHives()
        {
            int counter = 0;

            for (int row = 0; row < Rows - 3; row++)
            {
                for (int col = 1; col < Columns - 1; col++)
                {
                    if (Cells[col, row].IsAlive && Cells[col - 1, row + 1].IsAlive && Cells[col - 1, row + 2].IsAlive && Cells[col, row + 3].IsAlive && Cells[col + 1, row + 1].IsAlive && Cells[col + 1, row + 2].IsAlive)
                    {
                        if (!Cells[col, row + 1].IsAlive && !Cells[col, row + 2].IsAlive && !Cells[col - 1, row].IsAlive && !Cells[col + 1, row].IsAlive && !Cells[col - 1, row + 3].IsAlive && !Cells[col + 1, row + 3].IsAlive)
                        {
                            counter++;
                        }
                    }
                }
            }

            return counter;
        }







        public bool ToContinue(int counter)
        {
            if (counter < MaxCounter) return true;
            return false;
        }

        public void LoadFromFile(string filePath)
        {
            string state = File.ReadAllText(filePath);


            int col = 0, row = 0;
            bool toFill = true;

            foreach (char c in state)
            {
                if (row >= Rows) return;
                if (col >= Columns) toFill = false;

                if (c == '\n')
                {
                    if (col < Columns - 1)
                    {
                        for (col = col + 1; col < Columns; col++)
                        {
                            Cells[col, row].IsAlive = false;
                        }
                    }
                    row++;
                    col = -1;
                    toFill = true;
                }
                else if (c == ' ' && toFill)
                {
                    Cells[col, row].IsAlive = false;
                }
                else if (c == '*' && toFill)
                {
                    Cells[col, row].IsAlive = true;
                }
                col++;
            }

            if (row < Rows - 1)
            {
                for (row = row + 1; row < Rows; row++)
                {
                    for (col = 0; col < Columns; col++)
                        Cells[col, row].IsAlive = false;
                }
            }
        }
    }

    public class Parameters
    {
        struct Data
        {
            public int height { get; set; }
            public int width { get; set; }
            public int cellSize { get; set; }
            public double liveDensity { get; set; }
            public int maxCounter { get; set; }
        }

        Data newdata;
        public int height
        {
            get { return newdata.height; }
        }
        public int width
        {
            get { return newdata.width; }
        }
        public int cellSize
        {
            get { return newdata.cellSize; }
        }
        public double liveDensity
        {
            get { return newdata.liveDensity; }
        }
        public int maxCounter
        {
            get { return newdata.maxCounter; }
        }
        public void LoadParameters(int height, int width, int cellSize, int liveDensity, int maxCounter)
        {
            newdata.height = height;
            newdata.width = width;
            newdata.cellSize = cellSize;
            newdata.liveDensity = liveDensity;
            newdata.maxCounter = maxCounter;

        }
    }

    class Program
    {
        static Board board;
        static private void Reset()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(40, 50, 1, 0, 10);

            board = new Board(parameters);
            board.LoadFromFile(".\\BoardsExamples\\Example1.txt");
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
            int counter = 0;
            string state = string.Empty;
            bool isStable = false;

            Reset();
            while (!isStable)
            {
                Console.Clear();
                Render();

                if (GetState(board) == state)
                {
                    isStable = true;
                    Console.WriteLine("The number of generations to transition to a stable phase: " + counter);
                    GetFullInfo(board);
                }
                state = GetState(board);

                if (!board.ToContinue(counter))
                {
                    SaveFile(board, ".\\BoardsExamples\\SavedGame.txt");
                    Console.WriteLine("Number of generations: " + counter);
                    GetFullInfo(board);
                    //  Console.ReadKey();
                    return;
                }

                counter++;
                board.Advance();
                Thread.Sleep(1000);
            }
        }

        static void GetFullInfo(Board board)
        {
            Console.WriteLine("Number of alive cells:  " + board.GetAliveCells());
            Console.WriteLine("Number of dead cells:   " + board.GetDeadCells());
            Console.WriteLine("Density of alive cells: " + board.GetDensity());


            Console.WriteLine("Number of blocks: " + board.CountBlocks());
            Console.WriteLine("Number of hives:  " + board.CountHives());
            Console.WriteLine("Number of boxes:  " + board.CountBox());
            Console.WriteLine("Number of boats:  " + board.CountBoat());
            Console.WriteLine("Number of ships:  " + board.CountShip());
        }
        static string GetState(Board board)
        {
            string state = string.Empty;

            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    bool cellIsAlive = board.Cells[col, row].IsAlive;

                    if (cellIsAlive) state += '*';
                    else state += ' ';
                }
                state += '\n';
            }

            return state;
        }

        static void SaveFile(Board board, string filePath)
        {
            string state = string.Empty;

            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    bool cellIsAlive = board.Cells[col, row].IsAlive;

                    if (cellIsAlive) state += '*';
                    else state += ' ';
                }
                state += '\n';
            }

            File.WriteAllText(filePath, state);
        }
    }
}