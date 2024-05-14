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
using ScottPlot.Palettes;

//  Мулыкин Артем 382007-3
namespace cli_life
{
    //  Класс Фигуры
    public class Figure
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string StableFigure { get; set; }

        public Figure(string name, int width, int height, string stableFigure)
        {
            Name = name;
            Width = width;
            Height = height;
            StableFigure = stableFigure;
        }

        public Figure()
            : this(string.Empty, 0, 0, string.Empty)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Figure figure))
            {
                return false;
            }

            return figure.Width == Width && figure.Height == Height &&
                   figure.StableFigure == StableFigure;
        }
    }
    public class DataHandler
    {
        public static void SaveBoardSettings(ConfigurationPanel settings, string filePath)
        {
            string jsonString = System.Text.Json.JsonSerializer.Serialize(settings);
            System.IO.File.WriteAllText(filePath, jsonString);
        }

        public static ConfigurationPanel LoadPanelConfig(string filePath)
        {
            string jsonContent = System.IO.File.ReadAllText(filePath);
            ConfigurationPanel configPanel = System.Text.Json.JsonSerializer.Deserialize<ConfigurationPanel>(jsonContent);
            return configPanel;
        }

        public static void SaveGridState(GameBoard board, string filePath)
        {
            StringBuilder boardResert = new StringBuilder();

            for (int Index_row = 0; Index_row < board.Height; Index_row++)
            {
                for (int columnIndex = 0; columnIndex < board.Width; columnIndex++)
                {
                    char Indic_cell = board.Cells[Index_row, columnIndex].IsAlive ? '*' : ' ';
                    boardResert.Append(Indic_cell);
                }
                boardResert.AppendLine();
            }

            boardResert.AppendLine($"{board.CellSize}");
            boardResert.AppendLine($"{board.Generation}");

            System.IO.File.WriteAllText(filePath, boardResert.ToString());
        }
        // Метод для загрузки состояния игровой доски из файла
        public static GameBoard LoadGridState(string filePath)
        {
            // Чтение всех строк из файла
            string[] boardDataLines = System.IO.File.ReadAllLines(filePath);
            int.TryParse(boardDataLines[^2].Split('=').Last(), out int sizeCell);
            int.TryParse(boardDataLines[^1].Split('=').Last(), out int gen);

            sizeCell = Math.Max(1, sizeCell);
            gen = Math.Max(0, gen);

            int boardHeight = boardDataLines.Length - 2;
            int boardWidth = boardDataLines[0].Length;

            // Создание конфигурации сетки
            var gridSetap = new ConfigurationPanel(boardWidth, boardHeight, sizeCell);

            Cell[,] Cellsgrid = new Cell[boardHeight / sizeCell, boardWidth / sizeCell];

            // Заполнение сетки ячеек на основе данных файла
            for (int y = 0; y < boardHeight; y += sizeCell)
            {
                for (int x = 0; x < boardWidth; x += sizeCell)
                {
                    // Определение живая ли ячейка
                    bool isCellAlive = boardDataLines[y][x] == '*';
                    Cellsgrid[y / sizeCell, x / sizeCell] = new Cell() { IsAlive = isCellAlive };
                }
            }
            // Возврат нового экземпляра игровой доски с загруженной конфигурацией и поколением
            return new GameBoard(gridSetap, gen, Cellsgrid);
        }

        public static Figure[] LoadGeometries(string filePath)
        {
            string figuresJson = System.IO.File.ReadAllText(filePath);
            Figure[] figures = System.Text.Json.JsonSerializer.Deserialize<Figure[]>(figuresJson);
            return figures;
        }

        public static void StoreGeometries(Figure[] figures, string filePath)
        {
            var options = new System.Text.Json.JsonSerializerOptions()
            {
                WriteIndented = true
            };
            string figuresJson = System.Text.Json.JsonSerializer.Serialize(figures, options);
            System.IO.File.WriteAllText(filePath, figuresJson);
        }

        public static void SavePlotPng(Plot plot, string filePath, int width = 1920, int height = 1080)
        {
            plot.SavePng(filePath, width, height);
        }
    }
    public class ConfigurationPanel
    {
        public int PanelWidth { get; set; }
        public int PanelHeight { get; set; }
        public int UnitBlockSize { get; set; }
        public double PopulationDensity { get; set; }

        public ConfigurationPanel(int panelWidth, int panelHeight, int unitBlockSize, double populationDensity = 0.5)
        {
            PanelWidth = panelWidth;
            PanelHeight = panelHeight;
            UnitBlockSize = unitBlockSize;
            PopulationDensity = populationDensity;
        }

        public ConfigurationPanel()
            : this(default, default, default, 0)
        {
        }
    }
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbours = new();
        private bool IsAliveNext;
        public bool DelNextGameState()
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
        public void UpdateGeneration()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class GameBoard
    {
        private int _width;
        private int _heigth;
        public Cell[,] Cells;
        public int CellSize;
        public int Columns { get { return Cells.GetLength(1); } }
        public int Rows { get { return Cells.GetLength(0); } }
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
        public int Generation { get; set; }
        public GameBoard(ConfigurationPanel settings)
        {
            Generation = 0;

            CellSize = settings.UnitBlockSize;

            Cells = new Cell[settings.PanelHeight / CellSize, settings.PanelWidth / CellSize];

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
            Randomize(settings.PopulationDensity);
        }
        public GameBoard(ConfigurationPanel settings, int generation, Cell[,] cells)
        {
            Generation = generation;
            CellSize = settings.UnitBlockSize;
            Cells = cells;
            Width = settings.PanelWidth;
            Height = settings.PanelHeight;
            ConnectNeighbours();
        }
        public GameBoard(double density = 0.5) : this(new ConfigurationPanel(50, 20, 1, density)) { }

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
        public void Randomize(double liveDensity)
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
                cell.DelNextGameState();
            }

            foreach (var cell in Cells)
            {
                cell.UpdateGeneration();
            }

            Generation++;
        }
    }
    public class Analys
    {
        public static Figure[] Figures;
        private GameBoard gameboard;

        public Analys(GameBoard board)
        {
            gameboard = board;
        }

        public int CountAliveCells()
        {
            return CountAliveCells(gameboard);
        }

        public static int CountAliveCells(GameBoard board)
        {
            int aliveCount = 0;
            foreach (var cell in board.Cells)
            {
                aliveCount += cell.IsAlive ? 1 : 0;
            }
            return aliveCount;
        }

        public Dictionary<string, int> GetFigures()
        {
            // Создание словаря для хранения количества каждой фигуры на доске
            Dictionary<string, int> Counts_figure = new Dictionary<string, int>();
            int GameboardR = gameboard.Rows;
            int GameboardC = gameboard.Columns;

            // Инициализация счетчика для каждого типа фигуры
            foreach (Figure fig in Figures)
            {
                Counts_figure[fig.Name] = 0;
            }

            // Перебор всех ячеек на доске с целью поиска фигур
            for (int i = 0; i < GameboardR; i++)
            {
                for (int j = 0; j < GameboardC; j++)
                {
                    // Проверка каждой фигуры на совпадение с изображением на доске
                    foreach (Figure figure in Figures)
                    {
                        Figure tempFigure = ConstructFigure(i, j, figure.Width, figure.Height);
                        // Если найдено совпадение, увеличиваем счетчик
                        if (figure.Equals(tempFigure))
                        {
                            Counts_figure[figure.Name]++;
                        }
                    }
                }
            }

            // Возврат словаря с подсчитанным количеством каждой фигуры
            return Counts_figure;
        }

        // Вспомогательный метод для построения представления фигуры
        private Figure ConstructFigure(int first_Row, int first_Col, int figW, int figH)
        {
            
            StringBuilder New_Figure = new StringBuilder();

            // Проходим по каждой строке и колонке, заданным параметрами 
            for (int row = first_Row; row < first_Row + figH; ++row)
            {
                for (int col = first_Col; col < first_Col + figW; ++col)
                {                    
                    char figChar = ' ';
                    // Если координаты находятся в пределах доски
                    if (row >= 0 && row < gameboard.Rows && col >= 0 && col < gameboard.Columns)
                    {
                        // Назначаем символ в зависимости от состояния ячейки
                        figChar = gameboard.Cells[row, col].IsAlive ? '*' : ' ';
                    }
                    // Добавляем символ в представление фигуры
                    New_Figure.Append(figChar);
                }
            }

            // Созадем и возвращаем новый объект фигуры
            return new Figure("Figure", figW, figH, New_Figure.ToString());
        }
    }
    class Program
    {
        static int const_in = 100;

        static bool LoadF = false;

        static bool SaveF = false;

        public static GameBoard SimulationBoard;

        public static Analys BoardAnalysis;

        public static ConfigurationPanel Set = new ConfigurationPanel(100, 40, 1, 0.5);

        const string SetJsPath = "../../../Settings_board.json";

        const string GameBoardFilePath = "../../../Life_save1.txt";

        const string FiJsPath = "../../../stableFigure.json";

        const string PicturePath = "../../../picture.png";

        static private void Reset()
        {
            try
            {
                var settings = DataHandler.LoadPanelConfig(SetJsPath);

                SimulationBoard = new GameBoard(settings);
            }
            catch
            {
                SimulationBoard = new GameBoard(Set);
            }

            BoardAnalysis = new(SimulationBoard);
        }
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
        // Метод запуска симуляции с пользовательскими настройками
        static void SimStart()
        {
            Console.Write("Укажите количество итераций для симуляции: ");
            if (!int.TryParse(Console.ReadLine(), out const_in))
            {
                Console.WriteLine("Ошибка: Введено некорректное число. Устанавливаю значение по умолчанию: 100.");
                const_in = 100;
            }

            Console.Write("Загружать стартовое состояние из файла? (да - 1, нет - 0): ");
            string loadInput = Console.ReadLine().Trim().ToLower();
            LoadF = loadInput == "1" || loadInput == "да";

            Console.Write("Сохранять ли конечное состояние в файл? (да - 1, нет - 0): ");
            string saveInput = Console.ReadLine().Trim().ToLower();
            SaveF = saveInput == "1" || saveInput == "да";
        }
        static void ExecuteRegularSim()
        {
            SimStart();
            Reset();

            // Загрузка начальных фигур
            Analys.Figures = DataHandler.LoadGeometries(FiJsPath);

            if (LoadF)
            {
                try
                {
                    // Попытка чтения состояния поля из файла
                    SimulationBoard = DataHandler.LoadGridState(GameBoardFilePath);
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

            for (var index = 0; index < const_in; index++)
            {
                Console.Clear();
                Render();
                // Вывод информации о текущем состоянии симуляции
                Console.WriteLine($"Текущая итерация: {SimulationBoard.Generation}");
                Console.WriteLine($"Число живых клеток: {BoardAnalysis.CountAliveCells()}");
                Console.WriteLine("Статистика по фигурам:");
                foreach (var figure in BoardAnalysis.GetFigures())
                {
                    Console.WriteLine($"{figure.Key}: {figure.Value}");
                }
                SimulationBoard.UpdateGeneration();
                Thread.Sleep(2000);
            }

            if (SaveF)
            {
                try
                {
                    // Сохранение состояния поля в файл
                    DataHandler.SaveGridState(SimulationBoard, GameBoardFilePath);
                    Console.WriteLine("Сохранение результата в файл.");
                }
                catch
                {
                    Console.WriteLine("Ошибка при сохранении результата в файл.");
                }
            }
        }
        static void ExecuteResearchSim()
        {
            Random randomGenerator = new Random();
            Plot simulationPlot = new Plot();
            simulationPlot.XLabel("Поколение");
            simulationPlot.YLabel("Количество живых клеток");
            simulationPlot.ShowLegend();

            double[] Den = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };

            var populationGenerations = Enumerable.Range(0, const_in).ToList();
            List<int> countOfAliveCells;
            int currentGeneration;
            GameBoard currentBoard;
            Analys simulationAnalys;
            var totalBoards = Den.Count();
            ConfigurationPanel settings = new ConfigurationPanel(Set.PanelWidth, Set.PanelHeight, Set.UnitBlockSize);

            Scatter populationChart;

            for (int index = 0; index < totalBoards; index++)
            {
                countOfAliveCells = new List<int>();
                currentGeneration = 0;

                settings.PopulationDensity = Den[index];
                currentBoard = new(Den[index]);
                simulationAnalys = new Analys(currentBoard);

                while (currentGeneration < const_in)
                {
                    countOfAliveCells.Add(simulationAnalys.CountAliveCells());
                    currentBoard.UpdateGeneration();
                    currentGeneration++;
                }

                populationChart = simulationPlot.Add.Scatter(populationGenerations, countOfAliveCells);
                populationChart.LegendText = $"Плотность {Den[index]}";
                populationChart.LineWidth = 2;
                populationChart.Color = new(randomGenerator.Next(0, 256), randomGenerator.Next(0, 256), randomGenerator.Next(0, 256));
            }

            DataHandler.SavePlotPng(simulationPlot, PicturePath);
        }
        static void SelectSimType()
        {
            Console.Write("Выберите тип симуляции, который вы хотите запустить: \n1. Выполнить анализ \n2. Построить график\nВаш выбор: ");
            if (!int.TryParse(Console.ReadLine(), out int selection))
            {
                throw new ArgumentException("Введенные данные не являются числом", nameof(selection));
            }
            switch (selection)
            {
                case 1:
                    ExecuteRegularSim();
                    break;
                case 2:
                    ExecuteResearchSim();
                    break;
                default:
                    throw new ArgumentException("Пожалуйста, выберите действительный вариант", nameof(selection));
            }
        }
        static void Main(string[] args)
        {
            SelectSimType();
        }
    }
}