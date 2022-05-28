using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cli_life
{
    public class BoardState {
        public int width { get; set; }
        public int height { get; set; }
        public string[] cellstext { get; set; }

        public Cell[,] GetCells(){
            Cell[,] Cells;
            Cells = new Cell[width , height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++){
                    Cells[x, y] = new Cell();
                    Cells[x, y].IsAlive = (cellstext[y][x] == '*');
                }

            return Cells;
        }
        public BoardState(){

        }

        public BoardState(Board board) {
            this.width = board.Width;
            this.height = board.Height;
            cellstext = new string[height];
            for(int i = 0; i < height; i++){
                cellstext[i] = "";
                for(int j = 0; j < width; j++){
                    cellstext[i] += board.Cells[i, j].IsAlive ? "*": " ";
                }
            }
        }
    }
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void PrepareStep()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void DoStep(){
            IsAlive = IsAliveNext;
        }

    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public int Width { get { return Cells.GetLength(0); } }
        public int Height { get { return Cells.GetLength(1); } }

        public Board(int width, int height, double liveDensity = .1)
        {

            Cells = new Cell[width, height ];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        public Board(string filepath){
            BoardState state = JsonSerializer.Deserialize<BoardState>(File.ReadAllText(filepath));
            Cells = state.GetCells();
            ConnectNeighbors();
        }

        public void SaveState(string filepath){
            BoardState state = new BoardState(this);
            string jsonString = JsonSerializer.Serialize<BoardState>(state);
            File.WriteAllText(filepath, jsonString);
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
                cell.PrepareStep();
            foreach (var cell in Cells)
                cell.DoStep();
        }

        // Gives every cell it's neighboors cell.neighboor
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int xL = (x > 0) ? x - 1 : Width - 1;
                    int xR = (x < Width - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Height - 1;
                    int yB = (y < Height - 1) ? y + 1 : 0;

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
    class Program
    {
        static Board board;
        static private void Reset()
        {
            board = new Board(
                width: 20,
                height: 20,
                liveDensity: 0.1);
        }
        static void Render()
        {
            for (int row = 0; row < board.Height; row++)
            {
                for (int col = 0; col < board.Width; col++)
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
            Reset();
            board = new Board("hive.json");
            int iter = 0;
            while(true)
            {
                Console.Clear();
                Console.WriteLine("Iter: " + iter.ToString());
                Render();
                board.Advance();
                Thread.Sleep(1000);
                iter ++;
            }
        }
    }
}