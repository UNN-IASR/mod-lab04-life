using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace Life
{
    public class Program
    {
        static Random rd;
        static ConsoleRender consoleRender;
        static ProgramSetting programSetting;
        static Board board;
        static void Main(string[] args)
        {
            string settingsPath = "ProgramSettings.json";


            LoadSettings(settingsPath);
            setRandomSettings();
            setConsoleRenderSettings();
            setBoardSettings();

            saveGraphic();

            consoleRender.Render(board);
        }

        static void LoadSettings(string path)
        {
            string jsonStr = File.ReadAllText(path);
            programSetting = JsonSerializer.Deserialize<ProgramSetting>(jsonStr);
        }

        static void setRandomSettings() =>
            rd = new Random(programSetting.RandomSeed);

        static void setConsoleRenderSettings() =>
            consoleRender = new ConsoleRender(
                programSetting.SymbolLive,
                programSetting.SymbolDead,
                programSetting.RenderDelayMillisecond,
                programSetting.CountIterations
            );

        static void setBoardSettings() =>
            board = BoardGenerator.GenerateRandom(
                programSetting.Columns,
                programSetting.Rows,
                programSetting.LiveDensity,
                rd
            );

        static void saveGraphic()
        {
            double stepDensity = 0.2;
            ScottPlotGraphic graphic = new ScottPlotGraphic(
                rd, programSetting.Columns, programSetting.Rows,
                programSetting.CountIterations, stepDensity);
            graphic.GetGraphic("ResultGraphic.png");
        }
    }

    public class ProgramSetting
    {
        public int Columns { get; init; }
        public int Rows { get; init; }

        public int RandomSeed { get; init; }
        public double LiveDensity { get; init; }

        public string SymbolLive { get; init; }
        public string SymbolDead { get; init; }
        public int RenderDelayMillisecond { get; init; }
        public int CountIterations { get; init; }
    }

    public class ConsoleRender
    {
        public string SymbolLive { get; }
        public string SymbolDead { get; }
        public int TimeDelay { get; }
        public int Iterations { get; }

        public ConsoleRender(string symbolLive, string symbolDead, int timeDelay, int iterations)
        {
            SymbolLive = symbolLive;
            SymbolDead = symbolDead;
            TimeDelay = timeDelay;
            Iterations = iterations;
        }

        public void Render(Board board)
        {
            for (int i = 0; i < Iterations; i++)
            {
                RenderStep(board);

                if (i == 50)
                    board.Save("Board.json");
                board.Advance();
            }
        }

        private void RenderStep(Board board)
        {
            Console.Clear();
            Console.Write(BoardToString(board));
            Thread.Sleep(TimeDelay);
        }

        private string BoardToString(Board board)
        {
            string result = "";
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                    result += board.isAliveCell(col, row) ? SymbolLive : SymbolDead;
                result += "\n";
            }
            return result;
        }
    }

    public static class BoardGenerator
    {
        public static Board GenerateRandom(int columns, int rows, double liveDensity, Random rd)
        {
            Cell[,] cells = new Cell[columns, rows];

            for (int column = 0; column < columns; column++)
                for (int row = 0; row < rows; row++)
                    cells[column, row] = new Cell(rd.NextDouble() < liveDensity);

            return new Board(cells);
        }
    }

    public class Board
    {
        private Cell[,] cells;
        public int Columns { get => cells.GetUpperBound(0) + 1; }
        public int Rows { get => cells.GetUpperBound(1) + 1; }
        public Board(Cell[,] cells)
        {
            this.cells = cells;
            ConnectCells();
        }

        public bool isAliveCell(int column, int row) =>
            cells[column, row].IsAliveNow;

        public void Advance()
        {
            SetNextLiveStateCells();
            AdvanceCells();
        }

        private void SetNextLiveStateCells()
        {
            foreach (var cell in cells)
                cell.DetermineNextLiveState();
        }

        private void AdvanceCells()
        {
            foreach (var cell in cells)
                cell.Advance();
        }

        private void ConnectCells()
        {
            int columns = cells.GetUpperBound(0) + 1;
            int rows = cells.GetUpperBound(1) + 1;

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : columns - 1;
                    int xR = (x < columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : rows - 1;
                    int yB = (y < rows - 1) ? y + 1 : 0;

                    cells[x, y].neighbors.Add(cells[xL, yT]);
                    cells[x, y].neighbors.Add(cells[x, yT]);
                    cells[x, y].neighbors.Add(cells[xR, yT]);
                    cells[x, y].neighbors.Add(cells[xL, y]);
                    cells[x, y].neighbors.Add(cells[xR, y]);
                    cells[x, y].neighbors.Add(cells[xL, yB]);
                    cells[x, y].neighbors.Add(cells[x, yB]);
                    cells[x, y].neighbors.Add(cells[xR, yB]);
                }
            }
        }

        public void Save(string path) =>
            File.WriteAllText(path, JsonSerializer.Serialize(ConvertBoard()));

        private int[][] ConvertBoard()
        {
            int[][] converted = new int[Rows][];

            for (int row = 0; row < Rows; row++)
            {
                converted[row] = new int[Columns];
                for (int column = 0; column < Columns; column++)
                    converted[row][column] = isAliveCell(column, row) ? 1 : 0;
            }

            return converted;
        }

        public static Board Load(string path)
        {
            string file = File.ReadAllText(path);
            var cellsInt = JsonSerializer.Deserialize<int[][]>(file);
            return new Board(ConvertToCells(cellsInt));
        }

        private static Cell[,] ConvertToCells(int[][] converted)
        {
            Cell[,] cells = new Cell[converted[0].Length, converted.Length];
            for (int column = 0; column < cells.GetLength(0); column++)
                for (int row = 0; row < cells.GetLength(1); row++)
                    cells[column, row] = new Cell(converted[row][column] == 1 ? true : false);

            return cells;
        }
    }

    public class Cell
    {
        public bool IsAliveNow { get; set; }
        public bool IsAliveNext { get; private set; }

        public readonly List<Cell> neighbors = new List<Cell>();

        public Cell(bool isAliveNow)
        {
            IsAliveNow = isAliveNow;
        }

        public void Advance() =>
            IsAliveNow = IsAliveNext;

        public void DetermineNextLiveState()
        {
            int liveNeighbors = CountLiveNeighbors();
            if (IsAliveNow)
                SetAliveNextForAlive(liveNeighbors);
            else
                SetAliveNextForDead(liveNeighbors);
        }

        private int CountLiveNeighbors() =>
            neighbors.Where(x => x.IsAliveNow).Count();

        private void SetAliveNextForAlive(int countAliveNeighbors) =>
            IsAliveNext = countAliveNeighbors == 2 || countAliveNeighbors == 3;

        private void SetAliveNextForDead(int countAliveNeighbors) =>
            IsAliveNext = countAliveNeighbors == 3;
    }

    public class Analyzer
    {
        public static int countAliveCell(Board board)
        {
            int count = 0;
            for (int column = 0; column < board.Columns; column++)
                for (int row = 0; row < board.Rows; row++)
                    count += board.isAliveCell(column, row) ? 1 : 0;
            return count;
        }
    }

    public class ScottPlotGraphic
    {
        private ScottPlot.Plot plot;
        private Random rd;
        private int columns;
        private int rows;
        private int maxIterations;
        private double stepDensity;

        private readonly double MAX_DENSITY = 1;

        public ScottPlotGraphic(Random rd, int columns, int rows, int maxIterations, double stepDensity)
        {
            this.rd = rd;
            this.columns = columns;
            this.rows = rows;
            this.maxIterations = maxIterations;
            this.stepDensity = stepDensity;
        }

        public void GetGraphic(string savePath)
        {
            SetNewPlt();

            for (double density = stepDensity; density < MAX_DENSITY; density += stepDensity)
                StepPlt(density);

            plot.SaveFig(savePath);
        }

        private void SetNewPlt()
        {
            plot = new ScottPlot.Plot(800, 800);
            plot.Title("Transition to Stable State");
            plot.XLabel("Epoch");
            plot.YLabel("Live cell");
            plot.Legend();
        }

        private void StepPlt(double density)
        {
            var (iterations, countsLive) = Step(density);
            plot.AddScatter(iterations, countsLive, label: density.ToString());
        }

        private (double[], double[]) Step(double density)
        {
            var board = BoardGenerator.GenerateRandom(columns, rows, density, rd);
            var countsLive = new List<double>();
            var iterations = Enumerable.Range(0, maxIterations).Select(item => (double)item);

            foreach (var iteration in iterations)
            {
                countsLive.Add(Analyzer.countAliveCell(board));
                board.Advance();
            }

            return (iterations.ToArray(), countsLive.ToArray());
        }
    }
}
