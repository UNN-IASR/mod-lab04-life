﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SkiaSharp;
using System.IO;
using System.Text.Json;

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
    }

    class SettingsParser
    {
        public int width {get; set;}
        public int height {get; set;}
        public int cellSize {get; set;}
        public double liveDensity {get; set;}
    }

    class Program
    {
        static Board board;
        static private void Reset()
        {
            if(!Directory.Exists("Input")) Directory.CreateDirectory("Input/");
            if(!File.Exists("Input/settings.json")) File.Create("Input/settings.json");
            string raw = File.ReadAllText("Input/settings.json");
            var settings = JsonSerializer.Deserialize<SettingsParser>(raw);
            board = new Board(
                width: settings.width,
                height: settings.height,
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

        static int Load()
        {
            if(!Directory.Exists("Input")) {
                Directory.CreateDirectory("Input/");
                File.WriteAllText("Input/gen-0.txt", "");
            }
            else if(!File.Exists("Input/gen-0.txt")) {
                File.WriteAllText("Input/gen-0.txt", "");
            }

            string raw = File.ReadAllText("Input/gen-0.txt");

            int wid = 0;
            int hei = 0;
            int gen = 0;
            if (raw.Length > 0) {
                wid = raw.Split('\n', StringSplitOptions.RemoveEmptyEntries)[0].Length;
                hei = raw.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length - 1;
                gen = int.Parse(raw.Split('\n', StringSplitOptions.RemoveEmptyEntries)[hei]);
            }

            board = new Board(
                width: wid,
                height: hei,
                cellSize: 1,
                liveDensity: 0);

            for (int row = 0; row < board.Rows; row++)
            {
                string str = raw.Split('\n')[row];
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (str[col] == '1') {
                        cell.IsAlive = true;
                    }
                    else {
                        cell.IsAlive = false;
                    }
                }
            }
            return gen;
        }
        static void Main(string[] args)
        {
            Reset();
            int genCount = 0;
            while(true)
            {
                if(Console.KeyAvailable) {
                    ConsoleKeyInfo name = Console.ReadKey();
                    if(name.KeyChar == 'q')
                        break;
                    else if(name.KeyChar == 's') {
                        string fname = "gen-" + genCount.ToString();
                        if(!Directory.Exists("Data")) Directory.CreateDirectory("Data/");
                        StreamWriter writer = new StreamWriter("Data/"+ fname + ".txt");
                        double[,] data = new double[board.Rows, board.Columns];
                        for (int row = 0; row < board.Rows; row++)
                        {
                           for (int col = 0; col < board.Columns; col++)   
                           {
                               var cell = board.Cells[col, row];
                               if (cell.IsAlive) {
                                   writer.Write('1');
                                   data[row,col] = 1;
                               }
                               else {
                                   writer.Write('0');
                                   data[row,col] = 0;
                               }
                           }
                           writer.Write("\n");
                        }
                        writer.Write(genCount);
                        writer.Close();
                    }
                    else if (name.KeyChar == 'l') {
                        genCount = Load();
                    }
                }  
                
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(500);
                ++genCount;
            }
        }
    }
}