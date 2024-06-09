using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cli_life
{
    public class Board
    {

        public Cell[,] Cells;  //readonly

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

        public int Count_isAlive()
        {
            int count = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Cells[col, row].IsAlive == true) count++;
                }
            }
            return count;
        }

        public bool Check_Simmetri_OX()
        {
            bool sim = true;

            for (int row = 0; row < Rows / 2; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Cells[col, row].IsAlive != Cells[col, Rows - row - 1].IsAlive)
                    {
                        sim = false;
                        break;
                    }

                }

            }

            return sim;
        }

        public bool Check_Simmetri_OY()
        {
            bool sim = true;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns / 2; col++)
                {
                    if (Cells[col, row].IsAlive != Cells[Columns - col - 1, row].IsAlive)
                    {
                        sim = false;
                        break;
                    }

                }

            }

            return sim;
        }


        public void Get_stable_phase(Cell[,] stable_phase)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    stable_phase[col, row] = new Cell();
                    stable_phase[col, row].IsAlive = Cells[col, row].IsAlive;
                }
            }

        }


        public bool Check_stable_phase(Cell[,] stable_phase)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Cells[col, row].IsAlive != stable_phase[col, row].IsAlive)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            Board board = (Board)obj;
            if ((board.Width == Width) && (board.Height == Height) && (board.CellSize == CellSize)) return true;
            else return false;
        }
    }
}
