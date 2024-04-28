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

namespace cli_life
{
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
        public readonly List<Cell> neighbors = new();

        /// <summary>
        /// Будет ли клетка жива в следующем поколении.
        /// </summary>
        private bool IsAliveNext;

        /// <summary>
        /// Определяет следующее состояние клетки.
        /// </summary>
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();

            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
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
        /// Создает экземпляр доски.
        /// </summary>
        /// <param name="width">Ширина доски.</param>
        /// <param name="height">Высота доски.</param>
        /// <param name="cellSize">Размер клетки.</param>
        /// <param name="liveDensity">Плотность жизни.</param>
        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[height / cellSize, width / cellSize];

            Width = Columns * CellSize;
            Height = Rows * CellSize;

            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    Cells[row, col] = new Cell();
                }
            }

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        /// <summary>
        /// Создает экземпляр доски с параметрами по умолчанию.
        /// </summary>
        public Board() : this(50, 20, 1, 0.5)
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
        /// Определяет состояние доски на следующем шаге симуляции.
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
        }

        /// <summary>
        /// Соединяет соседей клеток.
        /// </summary>
        private void ConnectNeighbors()
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

                    Cells[row, col].neighbors.Add(Cells[rowTop, colLeft]);
                    Cells[row, col].neighbors.Add(Cells[rowTop, col]);
                    Cells[row, col].neighbors.Add(Cells[rowTop, colRight]);
                    Cells[row, col].neighbors.Add(Cells[row, colLeft]);
                    Cells[row, col].neighbors.Add(Cells[row, colRight]);
                    Cells[row, col].neighbors.Add(Cells[rowBottom, colLeft]);
                    Cells[row, col].neighbors.Add(Cells[rowBottom, col]);
                    Cells[row, col].neighbors.Add(Cells[rowBottom, colRight]);
                }
            }
        }

        /// <summary>
        /// Записывает состояние доски в указанный файл.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        public void SaveBoardState(string filePath)
        {
            var boardStringRepr = string.Empty;

            for (var row = 0; row < Height; row++)
            {
                for (var col = 0; col < Width; col++)
                {
                    boardStringRepr +=
                        (Cells[row, col].IsAlive)
                            ? '*'
                            : ' ';
                }
                boardStringRepr += '\n';
            }
            boardStringRepr += $"cellSize={CellSize}";

            File.WriteAllText(filePath, boardStringRepr);
        }

        /// <summary>
        /// Загружает состояние доски из файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        public void LoadBoardState(string filePath)
        {
            var boardStringArrayRepr = File.ReadAllLines(filePath);

            int.TryParse(
                boardStringArrayRepr[boardStringArrayRepr.Length - 1]
                    .Substring(
                        boardStringArrayRepr[boardStringArrayRepr.Length - 1]
                        .IndexOf('=') + 1),
                out int cellSize);

            CellSize = cellSize > 1
                ? cellSize
                : 1;

            Height = boardStringArrayRepr.Length - 1;
            Width = boardStringArrayRepr[0].Length;

            Cells = new Cell[Height / CellSize, Width / CellSize];

            for (var row = 0; row < Height; row += CellSize)
            {
                for (var col = 0; col < Width; col += CellSize)
                {
                    Cells[row / CellSize, col / CellSize] = new Cell
                    {
                        IsAlive = (boardStringArrayRepr[row][col] == '*')
                    };
                }
            }

            ConnectNeighbors();
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
        const string JsonPath = "boardSettings.json";

        /// <summary>
        /// Файл с состоянием доски.
        /// </summary>
        const string BoardStateFilePath = "gameOfLife_sf1.txt";

        /// <summary>
        /// Число итераций симуляции.
        /// </summary>
        static int IterationsNum = 0;

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
        static Board SimulationBoard;

        /// <summary>
        /// Загрузить настройки доски из JSON-файла.
        /// </summary>
        /// <returns>Настройки доски.</returns>
        static private BoardSettings LoadBoardSettings()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            string json = File.ReadAllText(JsonPath);
            return JsonSerializer.Deserialize<BoardSettings>(json);
        }

        /// <summary>
        /// Сбрасывает настройки доски.
        /// </summary>
        static private void Reset()
        {
            try
            {
                var settings = LoadBoardSettings();

                SimulationBoard = new Board(
                    width: settings.Width,
                    height: settings.Height,
                    cellSize: settings.CellSize,
                    liveDensity: settings.LiveDensity);
            }
            catch
            {
                SimulationBoard = new Board();
            }
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
        /// Запускает симуляцию в соответствии с настроенной конфигурацией.
        /// </summary>
        static void RunSimulation()
        {
            if (LoadFromFile)
            {
                try
                {
                    SimulationBoard.LoadBoardState(BoardStateFilePath);
                    Console.WriteLine("Успешное чтение данных из файла.");
                }
                catch
                {
                    Reset();
                    Console.WriteLine("Ошибка при чтении данных из файла.");
                }

                Thread.Sleep(2000);
            }

            for (var index = 0; index < IterationsNum; index++)
            {
                Console.Clear();
                Render();
                SimulationBoard.Advance();
                Thread.Sleep(100);
            }

            if (SaveToFile)
            {
                try
                {
                    SimulationBoard.SaveBoardState(BoardStateFilePath);
                    Console.WriteLine("Успешное сохранение результата в файл.");
                }
                catch
                {
                    Console.WriteLine("Ошибка при сохранении результата в файл.");
                }

            }
        }

        /// <summary>
        /// Точка входа программы.
        /// </summary>
        /// <param name="args">Передаваемые аргументы.</param>
        static void Main(string[] args)
        {
            ConfigureSimulationStart();
            Reset();

            RunSimulation();
        }
    }
}