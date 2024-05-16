using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cli_life
{
    public class Board
    {
        public Cell[,] Cells;
        
        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }

        public Board(int width, int height, double liveDensity = 0.5, string pathToState = null)
        {
            Cells = new Cell[width, height];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            if (pathToState == null)
            {
                Randomize(liveDensity);
            }
            else
            {
                ImportState(@pathToState);
            }
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void ImportState(string pathToState)
        {
            List<string[]> contents = File.ReadLines(pathToState).Select(c => c.TrimEnd('\n').Split(',')).ToList();
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    Cells[x, y].IsAlive = contents[y][x] != "0";
                }
            }
        }

        public void ExportState(string folderPath)
        {
            DateTime timestamp = DateTime.Now;
            string fileName = timestamp.ToString("s") + ".txt";
            using (StreamWriter writer = new StreamWriter(folderPath + "/" + fileName))
            {
                for (int y = 0; y < Rows; y++)
                {
                    for (int x = 0; x < Columns; x++)
                    {
                        writer.Write(Cells[x, y].IsAlive ? "1," : "0,");
                    }
                    writer.Write("\n");
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
                    if (x > 0 && y > 0)
                        Cells[x, y].neighbors.Add(Cells[x - 1, y - 1]);
                    if (y > 0)
                        Cells[x, y].neighbors.Add(Cells[x, y - 1]);
                    if (x < Columns - 1 && y > 0)
                        Cells[x, y].neighbors.Add(Cells[x + 1, y - 1]);
                    if (x > 0)
                        Cells[x, y].neighbors.Add(Cells[x - 1, y]);
                    if (x < Columns - 1)
                        Cells[x, y].neighbors.Add(Cells[x + 1, y]);
                    if (x > 0 && y < Rows - 1)
                        Cells[x, y].neighbors.Add(Cells[x - 1, y + 1]);
                    if (y < Rows - 1)
                        Cells[x, y].neighbors.Add(Cells[x, y + 1]);
                    if (x < Columns - 1 && y < Rows - 1)
                        Cells[x, y].neighbors.Add(Cells[x + 1, y + 1]);
                }
            }
        }
    }
}