using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

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
        public BoardState GetCurrentState()
        {
            var state = new BoardState(Columns, Rows);
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    state.CellStates[x, y] = new CellState { IsAlive = Cells[x, y].IsAlive };
                }
            }
            return state;
        }

        public void SetState(BoardState state)
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y].IsAlive = state.CellStates[x, y].IsAlive;
                }
            }
        }

    }

    public class BoardState
    {
        public CellState[,] CellStates { get; set; }

        public BoardState(int width, int height)
        {
            CellStates = new CellState[width, height];
        }
    }

    public class CellState
    {
        public bool IsAlive { get; set; }
    }

    public class Settings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }
    class Program
    {
        static Board board;
        static private void Reset(Settings settings)
        {
            board = new Board(
                width: settings.Width,
                height: settings.Height,
                cellSize: settings.CellSize,
                liveDensity: settings.LiveDensity);
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
            string file_state = "..\\..\\..\\board_state.json";
            string file_settings = "..\\..\\..\\settings.json";

            string json = File.ReadAllText(file_settings);
            Settings settings = JsonConvert.DeserializeObject<Settings>(json);

            Reset(settings);

            if (File.Exists(file_state))
            {
                string savedStateJson = File.ReadAllText(file_state);
                var savedState = JsonConvert.DeserializeObject<BoardState>(savedStateJson);
                board.SetState(savedState);
            }

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                var currentState = board.GetCurrentState();
                string currentStateJson = JsonConvert.SerializeObject(currentState);
                File.WriteAllText(file_state, currentStateJson);
            };

            while (true)
            {
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(1000);

                //var currentState = board.GetCurrentState();
                //string currentStateJson = JsonConvert.SerializeObject(currentState);
                //File.WriteAllText(file_state, currentStateJson);
            }
        }
    }
}