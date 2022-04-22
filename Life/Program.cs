using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
        public int option;
        public static string filePath = "D:\\projects\\МИПиС\\test1.txt";

        public Board(int width, int height, int cellSize, double liveDensity = .1, int option = 1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            switch (option)
            {
                case 1:
                    Randomize(liveDensity);
                    break;
                case 2:
                    ReadFile();
                    break;
            }
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }
        public void ReadFile()
        {
            var list = new List<string>();           
            string line;
            StreamReader sr = new StreamReader(filePath);
            line = sr.ReadLine();
            int colLength = line.Length;            
            list.Add(line);
            int rowLength = 1;
            while (line != null)
            {
                line = sr.ReadLine();
                list.Add(line);
                rowLength++;
            }
            sr.Close();
            for (int row = 0; row < rowLength-1; row++)
            {
                for (int col = 0; col < colLength; col++)
                {
                    var simb = list[row][col];
                    if (simb == '*')
                    {
                        Cells[col, row].IsAlive = true;                    
                    }
                    else
                    {
                        Cells[col, row].IsAlive = false;                  
                    }
                }          
            }
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
    }
    public class Program
    {
        public static int numberOfAlive;
        static Board board;
        static private void Reset()
        {
            board = new Board(
                width: Settings.width,
                height: Settings.height,
                cellSize: Settings.cellSize,
                liveDensity: Settings.liveDensity,
                option: Settings.option);
        }
        static void Render()
        {
            numberOfAlive = 0;
            string filePath = "D:\\projects\\МИПиС\\file.txt";
            FileStream fileStream = File.Open(filePath, FileMode.Create);
            StreamWriter output = new StreamWriter(fileStream);
            string text = "";
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                        text += '*';
                        numberOfAlive++;
                    }
                    else
                    {
                        Console.Write(' ');
                        text += ' ';
                    }
                }
                Console.Write('\n');
                text += '\n';
            }
            output.Write(text);
            output.Close();
        }

       
        public static void Main(string[] args)
        {
            int test = 5;
            int[] Results = { 12, 17, 19, 8, 28 };
            switch (test)
            {
                case 1:
                    Board.filePath = "D:\\projects\\МИПиС\\test1.txt";
                    break;
                case 2:
                    Board.filePath = "D:\\projects\\МИПиС\\test2.txt";
                    break;
                case 3:
                    Board.filePath = "D:\\projects\\МИПиС\\test3.txt";
                    break;
                case 4:
                    Board.filePath = "D:\\projects\\МИПиС\\test4.txt";
                    break;
                case 5:
                    Board.filePath = "D:\\projects\\МИПиС\\test5.txt";
                    break;
            }
            
            Reset();
            for (int i = 0; i < 10; i++)
            {
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(1000);
            }

            bool result = numberOfAlive == Results[test-1];
            Console.WriteLine(result);
        }
    }
}
