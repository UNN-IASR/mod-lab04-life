using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Text.RegularExpressions;
using ScottPlot.Plottables;
using System.Runtime.CompilerServices;
using ScottPlot;

namespace cli_life
{
    /// <summary>
    /// Фигура.
    /// </summary>
    public class Figure
    {
        /// <summary>
        /// Имя фигуры.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ширина фигуры.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота фигуры.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Строковое представление фигуры.
        /// </summary>
        public string FigureString { get; set; }

        /// <summary>
        /// Создает экземпляр фигуры.
        /// </summary>
        /// <param name="name">Имя фигуры.</param>
        /// <param name="width">Ширина фигуры.</param>
        /// <param name="height">Высота фигуры.</param>
        /// <param name="figureString">Строковое представление фигуры.</param>
        public Figure(string name, int width, int height, string figureString)
        {
            Name = name;
            Width = width;
            Height = height;
            FigureString = figureString;
        }

        /// <summary>
        /// Создает экземпляр фигуры.
        /// </summary>
        public Figure() : this(string.Empty, 0, 0, string.Empty)
        {
        }

        /// <summary>
        /// Возвращает результат проверки эквивалентности фигуры и другого объекта.
        /// </summary>
        /// <param name="obj">Объект для сравнения.</param>
        /// <returns>Результат проверки эквивалентности.</returns>
        public override bool Equals(object obj)
        {
            if (obj is not Figure)
            {
                return false;
            }

            var figure = obj as Figure;

            return figure.Width == this.Width && figure.Height == this.Height
                && figure.FigureString == this.FigureString;
        }
    }

    /// <summary>
    /// Настройки доски.
    /// </summary>
    public class BoardSettings
    {
        /// <summary>
        /// Ширина доски.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота доски.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Размер клетки.
        /// </summary>
        public int CellSize { get; set; }

        /// <summary>
        /// Плотность жизни.
        /// </summary>
        public double LiveDensity { get; set; }

        /// <summary>
        /// Создает экземпляр настроек доски.
        /// </summary>
        /// <param name="width">Ширина доски.</param>
        /// <param name="height">Высота доски.</param>
        /// <param name="cellSize">Размер клетки.</param>
        /// <param name="liveDensity">Плотность жизни.</param>
        public BoardSettings(int width, int height, int cellSize, double liveDensity = 0.5)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            LiveDensity = liveDensity;
        }

        /// <summary>
        /// Создает экземпляр настроек доски.
        /// </summary>
        public BoardSettings() : this(0, 0, 0, 0)
        {
        }
    }

    /// <summary>
    /// Работа с файлами.
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// Загружает настройки доски из JSON-файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        public static BoardSettings LoadBoardSettings(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var settings = JsonSerializer.Deserialize<BoardSettings>(json);

            return settings;
        }

        /// <summary>
        /// Сохраняет настройки доски в JSON-файл.
        /// </summary>
        /// <param name="settings">Настройки доски.</param>
        /// <param name="filePath">Путь к файлу.</param>
        public static void SaveBoardSettings(BoardSettings settings, string filePath)
        {
            var json = JsonSerializer.Serialize(settings);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Записывает состояние доски в указанный файл.
        /// </summary>
        /// <param name="board">Доска.</param>
        /// <param name="filePath">Путь к файлу.</param>
        public static void SaveBoardState(Board board, string filePath)
        {
            var boardStringRepr = string.Empty;

            for (var row = 0; row < board.Height; row++)
            {
                for (var col = 0; col < board.Width; col++)
                {
                    boardStringRepr +=
                        (board.Cells[row, col].IsAlive)
                            ? '*'
                            : ' ';
                }
                boardStringRepr += '\n';
            }
            boardStringRepr += $"cellSize={board.CellSize}\n";
            boardStringRepr += $"generation={board.Generation}";

            File.WriteAllText(filePath, boardStringRepr);
        }

        /// <summary>
        /// Загружает состояние доски из файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <returns>Загруженную доску.</returns>
        public static Board LoadBoardState(string filePath)
        {
            var boardStringArrayRepr = File.ReadAllLines(filePath);

            int.TryParse(
                boardStringArrayRepr[boardStringArrayRepr.Length - 2]
                    .Substring(
                        boardStringArrayRepr[boardStringArrayRepr.Length - 2]
                        .IndexOf('=') + 1),
                out int cellSize);

            int.TryParse(
                boardStringArrayRepr[boardStringArrayRepr.Length - 1]
                    .Substring(
                        boardStringArrayRepr[boardStringArrayRepr.Length - 1]
                        .IndexOf('=') + 1),
                out int generation);

            cellSize = cellSize > 1 
                ? cellSize
                : 1;

            generation = generation > 0
                ? generation
                : 0;

            var height = boardStringArrayRepr.Length - 2;
            var width = boardStringArrayRepr[0].Length;

            var settings = new BoardSettings(width, height, cellSize);

            var cells = new Cell[height / cellSize, width / cellSize];

            for (var row = 0; row < height; row += cellSize)
            {
                for (var col = 0; col < width; col += cellSize)
                {
                    cells[row / cellSize, col / cellSize] = new Cell
                    {
                        IsAlive = (boardStringArrayRepr[row][col] == '*')
                    };
                }
            }

            return new Board(settings, generation, cells);
        }

        /// <summary>
        /// Загружает список фигур из JSON-файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <returns>Список фигур.</returns>
        public static Figure[] LoadFigures(string filePath)
        {
            var json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<Figure[]>(json);
        }

        /// <summary>
        /// Сохраняет список фигур в JSON-файл.
        /// </summary>
        /// <param name="figures">Фигуры.</param>
        /// <param name="filePath">Путь к файлу.</param>
        public static void SaveFigures(Figure[] figures, string filePath)
        {
            var json = JsonSerializer.Serialize(figures, new
                JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Сохраняет график в формате png.
        /// </summary>
        /// <param name="plot">График.</param>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="width">Ширина изображения.</param>
        /// <param name="heigth">Высота изображения.</param>
        public static void SavePlotPng(Plot plot, string filePath, int width = 1920, int heigth = 1080)
        {
            plot.SavePng(filePath, 1920, 1080);
        }
    }

    /// <summary>
    /// Клетка.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Жива ли клетка.
        /// </summary>
        public bool IsAlive;

        /// <summary>
        /// Соседи клетки.
        /// </summary>
        public readonly List<Cell> neighbours = new();

        /// <summary>
        /// Будет ли клетка жива в следующем поколении.
        /// </summary>
        private bool IsAliveNext;

        /// <summary>
        /// Определяет следующее состояние клетки.
        /// </summary>
        public bool DetermineNextLiveState()
        {
            int liveNeighbors = neighbours.Where(x => x.IsAlive).Count();

            if (IsAlive)
            {
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            }
            else
            {
                IsAliveNext = liveNeighbors == 3;
            }

            return IsAliveNext;
        }

        /// <summary>
        /// Определяет состояние клетки на следующем шаге.
        /// </summary>
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }

    /// <summary>
    /// Доска симуляции жизни.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Ширина доски.
        /// </summary>
        private int _width;

        /// <summary>
        /// Высота доски.
        /// </summary>
        private int _heigth;

        /// <summary>
        /// Массив клеток.
        /// </summary>
        public Cell[,] Cells;

        /// <summary>
        /// Размер клетки.
        /// </summary>
        public int CellSize;

        /// <summary>
        /// Число колонок массива клеток.
        /// </summary>
        public int Columns { get { return Cells.GetLength(1); } }

        /// <summary>
        /// Число строк массива клеток.
        /// </summary>
        public int Rows { get { return Cells.GetLength(0); } }

        /// <summary>
        /// Ширина доски.
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
            private set
            {
                _width = value;
            }
        }

        /// <summary>
        /// Высота доски.
        /// </summary>
        public int Height
        {
            get
            {
                return _heigth;
            }
            private set
            {
                _heigth = value;
            }
        }

        /// <summary>
        /// Поколение.
        /// </summary>
        public int Generation { get; set; }

        /// <summary>
        /// Создает экземпляр доски.
        /// </summary>
        /// <param name="settings">Настройки доски.</param>
        public Board(BoardSettings settings)
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

        /// <summary>
        /// Создает экземпляр доски.
        /// </summary>
        /// <param name="settings">Настройки доски.</param>
        /// <param name="generation">Поколение.</param>
        /// <param name="cells">Заполненная доска.</param>
        public Board(BoardSettings settings, int generation, Cell[,] cells)
        {
            Generation = generation;

            CellSize = settings.CellSize;

            Cells = cells;

            Width = settings.Width;
            Height = settings.Height;

            ConnectNeighbours();
        }

        /// <summary>
        /// Создает экземпляр доски.
        /// </summary>
        /// <param name="density">Плотность жизни.</param>
        public Board(double density = 0.5) : this(new BoardSettings(50, 20, 1, density))
        {
        }

        /// <summary>
        /// Заполняет доску случайным образом.
        /// </summary>
        /// <param name="liveDensity">Плотность жизни.</param>
        public void Randomize(double liveDensity)
        {
            var rand = new Random();

            foreach (var cell in Cells)
            {
                cell.IsAlive = rand.NextDouble() < liveDensity;
            }
        }

        /// <summary>
        /// Определяет состояние доски на следующем шаге симуляции, меняе состояние доски.
        /// </summary>
        public void Advance()
        {
            foreach (var cell in Cells)
            {
                cell.DetermineNextLiveState();
            }

            foreach (var cell in Cells)
            {
                cell.Advance();
            }

            Generation++;
        }

        /// <summary>
        /// Соединяет соседей клеток.
        /// </summary>
        private void ConnectNeighbours()
        {
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    var rowBottom = (row > 0)
                        ? row - 1
                        : Rows - 1;
                    var rowTop = (row < Rows - 1)
                        ? row + 1
                        : 0;

                    var colLeft = (col > 0)
                        ? col - 1
                        : Columns - 1;
                    var colRight = (col < Columns - 1)
                        ? col + 1
                        : 0;

                    Cells[row, col].neighbours.Add(Cells[rowTop, colLeft]);
                    Cells[row, col].neighbours.Add(Cells[rowTop, col]);
                    Cells[row, col].neighbours.Add(Cells[rowTop, colRight]);
                    Cells[row, col].neighbours.Add(Cells[row, colLeft]);
                    Cells[row, col].neighbours.Add(Cells[row, colRight]);
                    Cells[row, col].neighbours.Add(Cells[rowBottom, colLeft]);
                    Cells[row, col].neighbours.Add(Cells[rowBottom, col]);
                    Cells[row, col].neighbours.Add(Cells[rowBottom, colRight]);
                }
            }
        }
    }

    /// <summary>
    /// Анализ доски.
    /// </summary>
    public class BoardAnalysis
    {
        /// <summary>
        /// Виды фигур.
        /// </summary>
        public static Figure[] Figures;

        /// <summary>
        /// Доска для анализа.
        /// </summary>
        private Board _board;

        public BoardAnalysis(Board board)
        {
            _board = board;
        }

        /// <summary>
        /// Возвращает число живых клеток на доске.
        /// </summary>
        /// <returns>Число живых клеток.</returns>
        public int GetAliveCellsCount()
        {
            return GetAliveCellsCount(_board);
        }

        /// <summary>
        /// Возвращает число живых клеток на доске.
        /// </summary>
        /// <param name="board">Доска для анализа.</param>
        /// <returns>Число живых клеток.</returns>
        public static int GetAliveCellsCount(Board board)
        {
            var result = 0;

            foreach (var cell in board.Cells)
            {
                if (cell.IsAlive)
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Возвращает число каждой из фигур на доске.
        /// </summary>
        /// <returns>Число каждой из фигур на доске.</returns>
        public Dictionary<string, int> GetFiguresCount()
        {
            var result = new Dictionary<string, int>();

            var rows = _board.Rows;
            var cols = _board.Columns;

            foreach (var figure in Figures)
            {
                result.Add(figure.Name, 0);
            }

            for (var row = -1; row < rows; row++)
            {
                for (var col = -1; col < cols; col++)
                {
                    foreach (var figure in Figures)
                    {
                        if (figure.Equals(MakeFigure(row, col, figure.Width, figure.Height)))
                        {
                            result[figure.Name]++;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Создает фигуру с требуемыми параметрами.
        /// </summary>
        /// <param name="row">Индекс строки.</param>
        /// <param name="col">Индекс столбца.</param>
        /// <param name="width">Ширина фигуры.</param>
        /// <param name="height">Высота фигуры.</param>
        /// <returns>Фигуру.</returns>
        private Figure MakeFigure(int row, int col, int width, int height)
        {
            var figureString = new StringBuilder();

            for (var rowIndex = row; rowIndex < row + height; rowIndex++)
            {
                for (var colIndex = col; colIndex < col + width; colIndex++)
                {
                    // Если выходит за пределы доски - заполнять пустыми значениями.
                    if (rowIndex < 0 || rowIndex >= _board.Rows
                        || colIndex < 0 || colIndex >= _board.Columns)
                    {
                        figureString.Append(' ');
                        continue;
                    }


                    figureString.Append(_board.Cells[rowIndex, colIndex].IsAlive
                        ? '*'
                        : ' ');
                }
            }

            return new Figure("figure", width, height, figureString.ToString());
        }
    }

    /// <summary>
    /// Консольное приложение.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Путь к файлу с настройками.
        /// </summary>
        const string SettingsJsonPath = "../../../boardSettings.json";

        /// <summary>
        /// Путь к файлу с состоянием доски.
        /// </summary>
        const string BoardStateFilePath = "../../../gameOfLife_sf1.txt";

        /// <summary>
        /// Путь к файлу с фигурами.
        /// </summary>
        const string FiguresJsonPath = "../../../figures.json";

        /// <summary>
        /// Путь к изображению с графиком.
        /// </summary>
        const string PlotPngPath = "../../../plot.png";

        /// <summary>
        /// Число итераций симуляции.
        /// </summary>
        static int IterationsNum = 100;

        /// <summary>
        /// Выполнять загрузку из файла.
        /// </summary>
        static bool LoadFromFile = false;

        /// <summary>
        /// Сохранять результат в файл.
        /// </summary>
        static bool SaveToFile = false;

        /// <summary>
        /// Доска с симуляцией жизни.
        /// </summary>
        public static Board SimulationBoard;

        /// <summary>
        /// Анализ состояния доски.
        /// </summary>
        public static BoardAnalysis BoardAnalysis;

        /// <summary>
        /// Настройки доски.
        /// </summary>
        public static BoardSettings Settings = new BoardSettings(100, 40, 1, 0.5);

        /// <summary>
        /// Сбрасывает настройки доски.
        /// </summary>
        static private void Reset()
        {
            try
            {
                var settings = FileManager.LoadBoardSettings(SettingsJsonPath);

                SimulationBoard = new Board(settings);
            }
            catch
            {
                SimulationBoard = new Board(Settings);
            }

            BoardAnalysis = new(SimulationBoard);
        }

        /// <summary>
        /// Отрисовывает доску.
        /// </summary>
        static void Render()
        {
            for (var row = 0; row < SimulationBoard.Rows; row++)
            {
                for (var col = 0; col < SimulationBoard.Columns; col++)
                {
                    var cell = SimulationBoard.Cells[row, col];

                    Console.Write(cell.IsAlive
                        ? '*'
                        : ' ');
                }

                Console.Write('\n');
            }
        }

        /// <summary>
        /// Выполняет предварительную настройку симуляции через взаимодействие с пользователем.
        /// </summary>
        static void ConfigureSimulationStart()
        {
            Console.Write("Введите число итераций симуляции: ");
            int.TryParse(Console.ReadLine(), out IterationsNum);

            Console.Write("Выполнять загрузку из файла? (1 - да, 0 - нет): ");
            int.TryParse(Console.ReadLine(), out int loadFileResponse);
            LoadFromFile =
                loadFileResponse == 0
                    ? false
                    : true;

            Console.Write("Выполнять сохранение результата в файл? (1 - да, 0 - нет): ");
            int.TryParse(Console.ReadLine(), out int saveFileResponse);
            SaveToFile =
                saveFileResponse == 0
                    ? false
                    : true;
        }

        /// <summary>
        /// Запускает обычную симуляцию в соответствии с настроенной конфигурацией.
        /// </summary>
        static void RunRegularSimulation()
        {
            ConfigureSimulationStart();
            Reset();

            BoardAnalysis.Figures = FileManager.LoadFigures(FiguresJsonPath);

            if (LoadFromFile)
            {
                try
                {
                    SimulationBoard = FileManager.LoadBoardState(BoardStateFilePath);
                    BoardAnalysis = new(SimulationBoard);
                    Console.WriteLine("Успешное чтение данных из файла.");
                }
                catch
                {
                    Console.WriteLine("Ошибка при чтении данных из файла.");
                    Reset();
                }

                Thread.Sleep(2000);
            }

            for (var index = 0; index < IterationsNum; index++)
            {
                Console.Clear();
                Render();
                Console.WriteLine($"Текущее поколение: {SimulationBoard.Generation}");
                Console.WriteLine($"Число живых клеток: {BoardAnalysis.GetAliveCellsCount()}");
                Console.WriteLine("Число каждой из фигур:");
                foreach (var figure in BoardAnalysis.GetFiguresCount())
                {
                    Console.WriteLine($"{figure.Key}: {figure.Value}");
                }
                SimulationBoard.Advance();
                Thread.Sleep(1000);
            }

            if (SaveToFile)
            {
                try
                {
                    FileManager.SaveBoardState(SimulationBoard, BoardStateFilePath);
                    Console.WriteLine("Успешное сохранение результата в файл.");
                }
                catch
                {
                    Console.WriteLine("Ошибка при сохранении результата в файл.");
                }

            }
        }

        /// <summary>
        /// Запускает исследовательскую симуляцию с выводом результатов в виде графика.
        /// </summary>
        static void RunResearchSimulation()
        {
            var rand = new Random();

            var plot = new Plot();
            plot.XLabel("Поколение");
            plot.YLabel("Живые клетки");
            plot.ShowLegend();

            var density = new double[]
            {
                0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9
            };

            var generations = Enumerable
                .Repeat(0, IterationsNum)
                .Select((index, generation) => generation + index)
                .ToList();
            List<int> aliveCells;

            int generation;
            Board board;
            BoardAnalysis boardAnalysis;
            var boardsCount = density.Count();
            var settings = new BoardSettings(Settings.Width, Settings.Height, Settings.CellSize);

            Scatter scatter;

            for (int boardIndex = 0; boardIndex < boardsCount; boardIndex++)
            {
                aliveCells = new();
                generation = 0;

                settings.LiveDensity = density[boardIndex];
                board = new(density[boardIndex]);
                boardAnalysis = new BoardAnalysis(board);

                while (generation < IterationsNum)
                {
                    aliveCells.Add(boardAnalysis.GetAliveCellsCount());
                    board.Advance();
                    generation++;
                }

                scatter = plot.Add.Scatter(generations, aliveCells);
                scatter.LegendText = density[boardIndex].ToString();
                scatter.LineWidth = 3;
                scatter.Color = new(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
            }

            FileManager.SavePlotPng(plot, PlotPngPath);
        }

        /// <summary>
        /// Получает от пользователя выбор типа симуляции, который нужно запустить.
        /// </summary>
        static void GetSimulationType()
        {
            Console.Write("Выберите тип симуляции для запуска: \n1. Обычная\n2. Исследовательская\nВыбор: ");
            if (!int.TryParse(Console.ReadLine(), out int userChoice))
            {
                throw new ArgumentException("Ответ не является числом", nameof(userChoice));
            }

            switch (userChoice)
            {
                case 1:
                    RunRegularSimulation();
                    break;
                case 2:
                    RunResearchSimulation();
                    break;
                default:
                    throw new ArgumentException("Выбран несуществующий вариант", nameof(userChoice));
            }
        }

        /// <summary>
        /// Точка входа программы.
        /// </summary>
        /// <param name="args">Передаваемые аргументы.</param>
        static void Main(string[] args)
        {
            GetSimulationType();
        }
    }
}