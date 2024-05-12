using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;
using ScottPlot;
using System.Drawing;

namespace cli_life
{
    #region Интерфейсы

    public interface ILoadable
    {
        abstract static object LoadFromFile(string filePath);
    }

    public interface ISaveable
    {
        void SaveToFile(string filePath);
        abstract static void SaveToFile(object saveObject, string filePath);
    }

    #endregion

    public class BoardSettings : ILoadable, ISaveable
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }

        #region Реализация интерфейсов

        public static object LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<BoardSettings>(json);
        }

        public void SaveToFile(string filePath)
        {
            SaveToFile(this, filePath);
        }

        public static void SaveToFile(object saveObject, string filePath)
        {
            var json = JsonSerializer.Serialize((BoardSettings)saveObject, new
                JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }

        #endregion
    }

    public class Figure
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string FigureString { get; set; }

        public Figure() : this(string.Empty, 0, 0, string.Empty)
        {
        }

        public Figure(string name, int width, int height, string figureString)
        {
            Name = name;
            Width = width;
            Height = height;
            FigureString = figureString;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Figure)
            {
                return false;
            }

            var figure = obj as Figure;

            return Width == figure.Width && Height == figure.Height
                && FigureString.Equals(figure.FigureString);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height, FigureString);
        }
    }

    public class FigureArray : ISaveable, ILoadable
    {
        public int Length { get; private set; }
        public List<Figure> Figures { get; private set; }

        public FigureArray() : this([])
        {
        }

        public FigureArray(Figure[] figures)
        {
            Figures = figures.ToList();
            Length = Figures.Count;
        }

        public void Add(Figure figure)
        {
            Figures.Add(figure);
            Length++;
        }

        public void Remove(Figure figure)
        {
            var removeIndex = -1;

            for (var index = 0; index < Length; index++)
            {
                if (Figures[index].Equals(figure) && Figures[index].Name == figure.Name)
                {
                    removeIndex = index;
                }
            }

            if (removeIndex == -1)
            {
                return;
            }

            Figures.RemoveAt(removeIndex);
            Length--;
        }

        #region Реализация интерфейсов

        public void SaveToFile(string filePath)
        {
            SaveToFile(this, filePath);
        }

        public static void SaveToFile(object saveObject, string filePath)
        {
            var json = JsonSerializer.Serialize(((FigureArray)saveObject).Figures, new
                JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }

        public static object LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);

            var figureArray = JsonSerializer.Deserialize<Figure[]>(json);

            return new FigureArray(figureArray);
        }

        #endregion
    }

    public class BoardHelper
    {
        private Board _board;
        private FigureArray _figures;

        public BoardHelper(Board board, FigureArray figures)
        {
            _board = board;
            _figures = figures;
        }

        public int GetElemsCount()
        {
            var result = GetAliveCellsCount();

            foreach (var figure in GetFiguresCount())
            {
                result += figure.Value;
            }

            return result;
        }

        public int GetAliveCellsCount()
        {
            var result = 0;

            foreach (var cell in _board.Cells)
            {
                if (cell.IsAlive)
                {
                    result++;
                }
            }

            return result;
        }

        public Dictionary<string, int> GetFiguresCount()
        {
            var result = new Dictionary<string, int>();

            if (_figures is null || _figures.Length == 0)
            {
                return result;
            }

            foreach (var figure in _figures.Figures)
            {
                result.Add(figure.Name, 0);
            }

            var rows = _board.Rows;
            var columns = _board.Columns;

            for (var row = -1; row < rows; row++)
            {
                for (var column = -1; column < columns; column++)
                {
                    foreach (var figure in _figures.Figures)
                    {
                        if (figure.Equals(GetBoardFigure(row, column, figure.Width, figure.Height)))
                        {
                            result[figure.Name]++;
                        }
                    }
                }
            }

            return result;
        }

        private Figure GetBoardFigure(int startRow, int startCol, int width, int height)
        {
            var figureString = new StringBuilder();

            for (var row = startRow; row < startRow + height; row++)
            {
                for (var column = startCol; column < startCol + width; column++)
                {
                    if (row < 0 || column < 0 || row >= _board.Height || column >= _board.Width)
                    {
                        figureString.Append(' ');
                        continue;
                    }

                    figureString.Append(_board.Cells[column, row].IsAlive
                        ? '*'
                        : ' ');
                }
            }

            return new Figure(string.Empty, width, height, figureString.ToString());
        }
    }

    public class Plot : ISaveable
    {
        private ScottPlot.Plot _plot;
        private bool _isLegendVisible;

        public Plot(string xLabel, string yLabel, bool isLegendVisible = true)
        {
            _plot = new();
            _plot.XLabel(xLabel);
            _plot.YLabel(yLabel);
            _isLegendVisible = isLegendVisible;

            ChangeLegendVisibility(_isLegendVisible);
        }

        public void ChangeLegendVisibility(bool isLegendVisible)
        {
            _isLegendVisible = isLegendVisible;

            if (_isLegendVisible)
            {
                _plot.ShowLegend();
                return;
            }

            _plot.HideLegend();
        }

        public void AddScatter<T>(List<T> xCords, List<T> yCords, ScottPlot.Color color,
            string legendText = "plot", int lineWidth = 2, int markerSize = 1)
        {
            var scat = _plot.Add.Scatter(xCords, yCords);
            scat.LegendText = legendText;
            scat.LineWidth = lineWidth;
            scat.MarkerSize = markerSize;
            scat.Color = color;
        }

        #region Реализация интерфейсов

        public void SaveToFile(string filePath)
        {
            _plot.SavePng(filePath, 1920, 1080);
        }

        public static void SaveToFile(object saveObject, string filePath)
        {
            ((Plot)saveObject).SaveToFile(filePath);
        }

        #endregion
    }

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
    public class Board : ISaveable, ILoadable
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

        public Board(Cell[,] cells, int cellSize = 1)
        {
            CellSize = cellSize;
            Cells = cells;

            ConnectNeighbors();
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

        #region Реализация интерфейсов

        public void SaveToFile(string filePath)
        {
            SaveToFile(this, filePath);
        }

        public static void SaveToFile(object saveObject, string filePath)
        {
            var board = saveObject as Board;

            var columns = board.Columns;
            var rows = board.Rows;

            var boardString = new StringBuilder();

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    boardString.Append(board.Cells[column, row].IsAlive
                        ? '*'
                        : ' ');
                }

                boardString.AppendLine();
            }

            File.WriteAllText(filePath, boardString.ToString());
        }

        public static object LoadFromFile(string filePath)
        {
            var board = File.ReadAllLines(filePath);

            var rows = board.GetLength(0);
            var columns = board[0].Length;

            var cells = new Cell[columns, rows];

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    cells[column, row] = new();

                    cells[column, row].IsAlive =
                        board[row]
                            .ElementAt(column)
                            .Equals('*');
                }
            }

            return new Board(cells);
        }

        #endregion
    }
    class Program
    {
        #region Настройки программы

        static int IterationsCount = 1000;
        static bool LoadFromFile = true;
        static bool SaveToFile = true;

        static string BoardSettingsPath = "../../../boardSettings.json";
        static string BoardStatePath = "../../../boardState.txt";
        static string FiguresPath = "../../../figures.json";
        static string ScatterPlotPath = "../../../plot.png";

        #endregion

        static Board board;
        static FigureArray figures = new();
        static BoardHelper boardHelper;

        static private void Reset()
        {
            if (File.Exists(FiguresPath))
            {
                figures = (FigureArray)FigureArray.LoadFromFile(FiguresPath);
            }

            if (LoadFromFile && File.Exists(BoardStatePath))
            {
                board = (Board)Board.LoadFromFile(BoardStatePath);
                boardHelper = new(board, figures);
                return;
            }

            var boardSettings = File.Exists(BoardSettingsPath)
                ? (BoardSettings)BoardSettings.LoadFromFile(BoardSettingsPath)
                : new BoardSettings
                {
                    Width = 50,
                    Height = 20,
                    CellSize = 1,
                    LiveDensity = 0.5
                };

            board = new Board(
                width: boardSettings.Width,
                height: boardSettings.Height,
                cellSize: boardSettings.CellSize,
                liveDensity: boardSettings.LiveDensity);

            boardHelper = new(board, figures);
        }
        static void ResetAnalysis(double density = 0.1)
        {
            var boardSettings = File.Exists(BoardSettingsPath)
                ? (BoardSettings)BoardSettings.LoadFromFile(BoardSettingsPath)
                : new BoardSettings
                {
                    Width = 50,
                    Height = 20,
                    CellSize = 1,
                    LiveDensity = 0.5
                };

            boardSettings.LiveDensity = density;

            board = new Board(
                width: boardSettings.Width,
                height: boardSettings.Height,
                cellSize: boardSettings.CellSize,
                liveDensity: boardSettings.LiveDensity);

            boardHelper = new(board, figures);
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
        static void Finish()
        {
            if (SaveToFile)
            {
                board.SaveToFile(BoardStatePath);
            }
        }
        static void DisplayBoardInfo()
        {
            var elemsCount = boardHelper.GetAliveCellsCount();
            var figures = boardHelper.GetFiguresCount();

            foreach (var figure in figures)
            {
                elemsCount += figure.Value;
            }

            Console.WriteLine($"Число элементов: {elemsCount}");
            Console.WriteLine("Число фигур:");
            foreach (var figure in boardHelper.GetFiguresCount())
            {
                Console.WriteLine($"{figure.Key} - {figure.Value}");
            }
        }
        static void RunLifeSimulation()
        {
            Reset();

            for (int ctr = 0; ctr < IterationsCount; ctr++)
            {
                Console.Clear();
                Render();
                DisplayBoardInfo();
                board.Advance();
                Thread.Sleep(1000);
            }

            Console.Clear();
            Render();
            DisplayBoardInfo();

            Finish();
        }
        static void RunAnalysisSimulation()
        {
            var rand = new Random();

            var plot = new Plot("Поколение", "Живые клетки");

            var density = new double[]
            {
                0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9
            };

            var generationNumbers =
                Enumerable
                    .Repeat(1, IterationsCount)
                    .Select((index, elem) => elem + index)
                    .ToList();
            List<int> aliveCellsNumbers;

            var boardsCount = density.Length;
            var generationsUntilStableState = new List<int>();

            var generation = 0;

            for (var boardIndex = 0; boardIndex < boardsCount; boardIndex++)
            {
                ResetAnalysis(density[boardIndex]);

                generation = 0;
                aliveCellsNumbers = new();

                while (generation < IterationsCount)
                {
                    aliveCellsNumbers.Add(boardHelper.GetAliveCellsCount());
                    board.Advance();
                    generation++;
                }

                plot.AddScatter(generationNumbers, aliveCellsNumbers,
                    new(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)),
                    density[boardIndex].ToString());
            }

            plot.SaveToFile(ScatterPlotPath);
            Console.WriteLine($"График сохранен по пути {ScatterPlotPath}");
        }
        static void GetUserChoice()
        {
            Console.WriteLine("Выберите вид симуляции:\n1.Игра \"Жизнь\"\n2.Анализ и построение графиков");
            var result = Console.ReadLine();

            switch (result)
            {
                case "1":
                    RunLifeSimulation();
                    break;
                case "2":
                    RunAnalysisSimulation();
                    break;
                default:
                    Console.WriteLine("Некорректный выбор");
                    break;
            }
        }
        static void Main(string[] args)
        {
            GetUserChoice();
        }
    }
}