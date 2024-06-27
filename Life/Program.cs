using System.Text;
using Newtonsoft.Json;
using ScottPlot;
using ScottPlot.Plottables;
using static System.ConsoleKey;

namespace mod_lab04_life.Life
{
    public class Figure
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Pattern { get; set; }

        public Figure(string name, int width, int height, string pattern)
        {
            Name = name;
            Width = width;
            Height = height;
            Pattern = pattern;
        }

        public bool IsTheSame(Figure figure)
        {
            if (figure is null)
                return false;

            return figure.Width == Width && figure.Height == Height &&
                   figure.Pattern == Pattern;
        }
    }

    public class DataHandler
    {
        public void SaveBoardSettings(Settings settings, string filePath)
        {
            var jsonString = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filePath, jsonString);
        }

        public Settings LoadSettingsData(string filePath)
        {
            try
            {
                var jsonContent = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Settings>(jsonContent);
            }
            catch
            {
                return new Settings(100, 40, 1);
            }
        }

        public void SaveGridState(GameBoard board, string filePath)
        {
            var sb = new StringBuilder(300);

            for (var row = 0; row < board.Height; row++)
            {
                for (var column = 0; column < board.Width; column++)
                {
                    var cell = board.Cells[row, column].IsAlive ? '*' : ' ';
                    sb.Append(cell);
                }

                sb.AppendLine();
            }

            sb.AppendLine($"{board.CellSize}");
            sb.AppendLine($"{board.Generation}");

            File.WriteAllText(filePath, sb.ToString());
        }

        public static GameBoard LoadGridState(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var sizeCell = int.Parse(lines[^2].Split('=').Last());
            var gen = int.Parse(lines[^1].Split('=').Last());

            sizeCell = Math.Max(1, sizeCell);
            gen = Math.Max(0, gen);

            var boardHeight = lines.Length - 2;
            var boardWidth = lines[0].Length;

            var gridSetup = new Settings(boardWidth, boardHeight, sizeCell);
            var grid = new Cell[boardHeight / sizeCell, boardWidth / sizeCell];

            for (var y = 0; y < boardHeight; y += sizeCell)
            {
                for (var x = 0; x < boardWidth; x += sizeCell)
                {
                    var isAlive = lines[y][x] is '*';
                    grid[y / sizeCell, x / sizeCell] = new Cell() { IsAlive = isAlive };
                }
            }

            return new GameBoard(gridSetup, gen, grid);
        }

        public static Figure[] LoadGeometries(string filePath)
        {
            var text = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Figure[]>(text);
        }

        public static void SavePlotPng(Plot plot, string filePath, int width = 1920, int height = 1080)
        {
            plot.SavePng(filePath, width, height);
        }
    }

    public class Settings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }

        public Settings(int width, int height, int cellSize, double liveDensity = 0.5)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            LiveDensity = liveDensity;
        }
    }

    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> Neighbours = new();

        private bool _isAliveNext;

        public void CheckNextGeneration()
        {
            var liveNeighbors = Neighbours.Count(x => x.IsAlive);

            if (IsAlive)
                _isAliveNext = liveNeighbors is 2 or 3;
            else
                _isAliveNext = liveNeighbors == 3;
        }

        public void UpdateGeneration()
        {
            IsAlive = _isAliveNext;
        }
    }

    public class GameBoard
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        public int Columns => Cells.GetLength(1);
        public int Rows => Cells.GetLength(0);
        public int Width { get; }
        public int Height { get; }

        public int Generation { get; set; }

        public GameBoard(Settings settings)
        {
            Generation = 0;

            CellSize = settings.CellSize;
            Cells = new Cell[settings.Height / CellSize, settings.Width / CellSize];
            Width = Columns * CellSize;
            Height = Rows * CellSize;

            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    Cells[row, col] = new Cell();
                }
            }

            ConnectNeighbours();
            Randomize(settings.LiveDensity);
        }

        public GameBoard(Settings settings, int generation, Cell[,] cells)
        {
            Generation = generation;
            CellSize = settings.CellSize;
            Cells = cells;
            Width = settings.Width;
            Height = settings.Height;
            ConnectNeighbours();
        }

        public GameBoard(double density = 0.5) : this(new Settings(100, 40, 1, density))
        {
        }

        private void ConnectNeighbours()
        {
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    var rowBottom = row > 0 ? row - 1 : Rows - 1;
                    var rowTop = row < Rows - 1 ? row + 1 : 0;
                    var colLeft = col > 0 ? col - 1 : Columns - 1;
                    var colRight = col < Columns - 1 ? col + 1 : 0;

                    Cells[row, col].Neighbours.Add(Cells[rowTop, colLeft]);
                    Cells[row, col].Neighbours.Add(Cells[rowTop, col]);
                    Cells[row, col].Neighbours.Add(Cells[rowTop, colRight]);
                    Cells[row, col].Neighbours.Add(Cells[row, colLeft]);
                    Cells[row, col].Neighbours.Add(Cells[row, colRight]);
                    Cells[row, col].Neighbours.Add(Cells[rowBottom, colLeft]);
                    Cells[row, col].Neighbours.Add(Cells[rowBottom, col]);
                    Cells[row, col].Neighbours.Add(Cells[rowBottom, colRight]);
                }
            }
        }

        private void Randomize(double liveDensity)
        {
            var rand = new Random();

            foreach (var cell in Cells)
            {
                cell.IsAlive = rand.NextDouble() < liveDensity;
            }
        }

        public void UpdateGeneration()
        {
            foreach (var cell in Cells)
            {
                cell.CheckNextGeneration();
            }

            foreach (var cell in Cells)
            {
                cell.UpdateGeneration();
            }

            Generation++;
        }
    }

    public class Analytics
    {
        public Figure[] Figures;
        private readonly GameBoard gameboard;

        public Analytics(GameBoard board)
        {
            gameboard = board;
        }

        public int GetAliveCellsCount()
        {
            var aliveCount = 0;
            foreach (var cell in gameboard.Cells)
            {
                aliveCount += cell.IsAlive ? 1 : 0;
            }

            return aliveCount;
        }

        public Dictionary<string, int> GetFigures()
        {
            var figuresCount = new Dictionary<string, int>();
            var rows = gameboard.Rows;
            var columns = gameboard.Columns;

            foreach (var fig in Figures)
            {
                figuresCount[fig.Name] = 0;
            }

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    foreach (var figure in Figures)
                    {
                        var tempFigure = ConstructFigure(i, j, figure.Width, figure.Height);
                        if (!figure.IsTheSame(tempFigure))
                            continue;

                        if (!figuresCount.TryAdd(figure.Name, 1))
                            figuresCount[figure.Name]++;
                    }
                }
            }

            return figuresCount;
        }

        private Figure ConstructFigure(int rowIndex, int columnIndex, int figW, int figH)
        {
            var sb = new StringBuilder();

            for (var row = rowIndex; row < rowIndex + figH; ++row)
            {
                for (var col = columnIndex; col < columnIndex + figW; ++col)
                {
                    var figChar = ' ';
                    if (row >= 0 && row < gameboard.Rows && col >= 0 && col < gameboard.Columns)
                        figChar = gameboard.Cells[row, col].IsAlive ? '*' : ' ';

                    sb.Append(figChar);
                }
            }

            return new Figure("Figure", figW, figH, sb.ToString());
        }
    }

    class Program
    {
        private static int _iterationCount = 100;
        private static bool _loadData;
        private static bool _saveData;
        private static GameBoard GameBoard;
        private static Analytics Analytics;
        private static Settings Settings = new(100, 40, 1);

        private const string SETTINGS_PATH = "Life/settings.json";
        private const string SAVE_PATH = "Life/save.txt";
        private const string FIGURES_PATH = "Life/figures.json";
        private const string PICTURE_PATH = "Life/plot.png";

        private static readonly DataHandler DataHandler = new();

        private static void Restart()
        {
            try
            {
                var settings = DataHandler.LoadSettingsData(SETTINGS_PATH);
                GameBoard = new GameBoard(settings);
                Analytics = new Analytics(GameBoard)
                {
                    Figures = DataHandler.LoadGeometries(FIGURES_PATH)
                };
            }
            catch
            {
                GameBoard = new GameBoard(Settings);
            }
        }

        static void Render()
        {
            for (var row = 0; row < GameBoard.Rows; row++)
            {
                for (var col = 0; col < GameBoard.Columns; col++)
                {
                    var cell = GameBoard.Cells[row, col];

                    Console.Write(cell.IsAlive ? '*' : ' ');
                }

                Console.Write('\n');
            }
        }

        private static void SimStart()
        {
            Console.Write("Укажите количество итераций для симуляции: ");
            if (!int.TryParse(Console.ReadLine(), out _iterationCount))
            {
                Console.WriteLine("Ошибка: Введено некорректное число. Устанавливаю значение по умолчанию: 100.");
            }
            Console.WriteLine();

            Console.Write("Загружать стартовое состояние из файла? (Y - да, N - нет): ");
            _loadData = Console.ReadKey().Key == Y;
            Console.WriteLine();
            Console.Write("Сохранять ли конечное состояние в файл? (Y - да, N - нет): ");
            _saveData = Console.ReadKey().Key == Y;
            Console.WriteLine();
        }

        private static void ExecuteRegularSim()
        {
            SimStart();
            Restart();

            if (_loadData)
            {
                try
                {
                    GameBoard = DataHandler.LoadGridState(SAVE_PATH);
                    Console.WriteLine("Данные из файла загружены!");
                }
                catch
                {
                    Console.WriteLine("Ошибка при чтении данных из файла.");
                    Restart();
                }

                Thread.Sleep(2_000);
            }

            for (var index = 0; index < _iterationCount; index++)
            {
                Console.Clear();
                Render();
                Console.WriteLine($"Генерация: {GameBoard.Generation}");
                Console.WriteLine($"Живых клеток: {Analytics.GetAliveCellsCount()}");
                Console.WriteLine("Фигуры:");
                foreach (var figure in Analytics.GetFigures())
                {
                    Console.WriteLine($"{figure.Key}: {figure.Value}");
                }

                GameBoard.UpdateGeneration();
                Thread.Sleep(2_000);
            }

            if (_saveData)
                DataHandler.SaveGridState(GameBoard, SAVE_PATH);
        }

        private static void ExecuteResearchSim()
        {
            var rnd = new Random();
            var plot = new Plot();
            plot.XLabel("Генерация");
            plot.YLabel("Живые клетки");
            plot.ShowLegend();

            double[] populationValues = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9];

            var populationGenerations = Enumerable.Range(0, _iterationCount).ToList();
            var totalBoards = populationValues.Length;
            var settings = new Settings(Settings.Width, Settings.Height, Settings.CellSize);

            for (var index = 0; index < totalBoards; index++)
            {
                var countOfAliveCells = new List<int>();
                var currentGeneration = 0;

                settings.LiveDensity = populationValues[index];
                GameBoard currentBoard = new(populationValues[index]);
                var simulationAnalytics = new Analytics(currentBoard);

                while (currentGeneration < _iterationCount)
                {
                    countOfAliveCells.Add(simulationAnalytics.GetAliveCellsCount());
                    currentBoard.UpdateGeneration();
                    currentGeneration++;
                }

                var populationChart = plot.Add.Scatter(populationGenerations, countOfAliveCells);
                populationChart.LegendText = $"Популяция {populationValues[index]}";
                populationChart.LineWidth = 2;
                populationChart.Color = new Color(rnd.Next(255), rnd.Next(255), rnd.Next(255));
            }

            DataHandler.SavePlotPng(plot, PICTURE_PATH);
        }

        static void SelectingSimulation()
        {
            Console.Write("Выберите тип симуляции: 1 - Выполнить анализ, 2 - Построить график: ");
            var key = Console.ReadKey();
            while (key.Key is not D1 and D2)
            {
                key = Console.ReadKey();
            }

            Console.WriteLine();

            if (key.Key is D1)
                ExecuteRegularSim();
            else
                ExecuteResearchSim();
        }

        static void Main(string[] args)
        {
            SelectingSimulation();
        }
    }
}
