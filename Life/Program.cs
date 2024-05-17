using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace cli_life
{
    // Класс, представляющий отдельную клетку
    public class Cell
    {
        public bool IsAlive; // Флаг, показывающий, жива ли клетка
        public readonly List<Cell> neighbors = new List<Cell>(); // Список соседних клеток
        private bool IsAliveNext; // Состояние клетки в следующем поколении

        // Определяет, будет ли клетка жить в следующем поколении
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Count(x => x.IsAlive); // Считаем живых соседей
            IsAliveNext = IsAlive ? (liveNeighbors == 2 || liveNeighbors == 3) : liveNeighbors == 3; // Правила игры
        }

        // Переходит к следующему поколению
        public void Advance()
        {
            IsAlive = IsAliveNext; // Обновляем текущее состояние клетки
        }
    }

    // Класс, представляющий игровое поле
    public class Board
    {
        public readonly Cell[,] Cells; // Двумерный массив клеток
        public readonly int CellSize; // Размер клетки

        public int Columns => Cells.GetLength(0); // Количество столбцов
        public int Rows => Cells.GetLength(1); // Количество строк
        public int Width => Columns * CellSize; // Ширина поля
        public int Height => Rows * CellSize; // Высота поля

        private List<string> previousStates = new List<string>(); // Список предыдущих состояний поля

        // Конструктор, создающий поле с заданными размерами и плотностью живых клеток
        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors(); // Соединяем соседние клетки
            Randomize(liveDensity); // Рандомно инициализируем клетки
        }

        // Конструктор, создающий поле с заданным начальными состоянием клеток
        public Board(int width, int height, int cellSize, bool[,] initialState)
        {
            CellSize = cellSize;
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell { IsAlive = initialState[x, y] };

            ConnectNeighbors(); // Соединяем соседние клетки
        }

        private readonly Random rand = new Random();
        // Инициализация клеток случайным образом
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        // Переход поля к следующему поколению
        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }

        // Соединение клеток с их соседями
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

        // Установка паттерна на поле
        public void PlacePattern(int[,] pattern, int posX, int posY)
        {
            for (int x = 0; x < pattern.GetLength(0); x++)
            {
                for (int y = 0; y < pattern.GetLength(1); y++)
                {
                    int boardX = (posX + x) % Columns;
                    int boardY = (posY + y) % Rows;
                    Cells[boardX, boardY].IsAlive = pattern[x, y] == 1;
                }
            }
        }

        // Проверка соответствия паттерну
        public bool MatchPattern(int[,] pattern, int posX, int posY)
        {
            for (int x = 0; x < pattern.GetLength(0); x++)
            {
                for (int y = 0; y < pattern.GetLength(1); y++)
                {
                    int boardX = (posX + x) % Columns;
                    int boardY = (posY + y) % Rows;
                    if (Cells[boardX, boardY].IsAlive != (pattern[x, y] == 1))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Классификация паттернов на поле
        public Dictionary<string, int> ClassifyPatterns(List<Tuple<string, int[,]>> patterns)
        {
            Dictionary<string, int> patternCounts = new Dictionary<string, int>();

            foreach (var pattern in patterns)
            {
                string name = pattern.Item1;
                int[,] shape = pattern.Item2;
                int count = 0;

                for (int x = 0; x < Columns; x++)
                {
                    for (int y = 0; y < Rows; y++)
                    {
                        if (MatchPattern(shape, x, y))
                        {
                            count++;
                        }
                    }
                }

                patternCounts[name] = count;
            }

            return patternCounts;
        }

        // Возвращает общее количество клеток на поле
        public int GetTotalCells()
        {
            return Columns * Rows;
        }

        // Возвращает общее количество возможных комбинаций состояния клеток
        public double GetTotalCombinations()
        {
            return Math.Pow(2, GetTotalCells());
        }

        // Проверяет, является ли текущее состояние стабильным
        public bool IsStable()
        {
            string currentState = GetBoardState();
            if (previousStates.Contains(currentState))
            {
                return true;
            }

            previousStates.Add(currentState);
            return false;
        }

        // Получение текущего состояния поля в виде строки
        public string GetBoardState()
        {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    sb.Append(Cells[col, row].IsAlive ? '1' : '0');
                }
            }
            return sb.ToString();
        }

        // Очистка истории состояний поля
        public void ClearHistory()
        {
            previousStates.Clear();
        }
    }

    // Класс конфигурации, загружаемой из JSON
    public class Config
    {
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }
        public double liveDensity { get; set; }
    }

    // Класс паттерна, загружаемого из JSON
    public class Pattern
    {
        public int[][] pattern { get; set; }
        public int[] position { get; set; }
    }

    public class Program
    {
        public static Board board;

        // Сбор данных по количеству поколений до стабильного состояния для различных плотностей
        public static List<Tuple<double, int>> GatherData(int width, int height, int cellSize, List<double> densities, int runsPerDensity)
        {
            List<Tuple<double, int>> data = new List<Tuple<double, int>>();

            foreach (var density in densities)
            {
                int totalGenerations = 0;

                for (int i = 0; i < runsPerDensity; i++)
                {
                    board = new Board(width, height, cellSize, density);
                    board.ClearHistory();
                    int generations = 0;

                    while (true)
                    {
                        board.Advance();
                        generations++;
                        if (board.IsStable())
                        {
                            break;
                        }
                    }

                    totalGenerations += generations;
                }

                int averageGenerations = totalGenerations / runsPerDensity;
                data.Add(new Tuple<double, int>(density, averageGenerations));
                Console.WriteLine($"Плотность {density}: Среднее количество поколений до стабильного состояния: {averageGenerations}");
            }

            return data;
        }

        // Главная функция программы
        public static void Main(string[] args)
        {
            var config = LoadConfig<Config>("config.json");
            List<double> densities = new List<double> { 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5 };
            int runsPerDensity = 10;

            var data = GatherData(config.width, config.height, config.cellSize, densities, runsPerDensity);

            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText("density_data.json", jsonData);

            Console.WriteLine("Сбор данных завершен и сохранен в density_data.json");
        }

        // Загрузка конфигурации из файла JSON
        public static T LoadConfig<T>(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        // Сохранение состояния поля в файл
        public static void SaveState(string filePath)
        {
            var state = new bool[board.Columns, board.Rows];
            for (int x = 0; x < board.Columns; x++)
            {
                for (int y = 0; y < board.Rows; y++)
                {
                    state[x, y] = board.Cells[x, y].IsAlive;
                }
            }

            string text = JsonConvert.SerializeObject(state);
            File.WriteAllText(filePath, text);
        }

        // Загрузка состояния поля из файла
        public static void LoadState(string filePath)
        {
            string text = File.ReadAllText(filePath);
            var state = JsonConvert.DeserializeObject<bool[,]>(text);

            var config = LoadConfig<Config>("config.json");
            board = new Board(
                width: config.width,
                height: config.height,
                cellSize: config.cellSize,
                initialState: state);
        }

        // Загрузка паттерна из файла
        public static void LoadPattern(string filePath)
        {
            string text = File.ReadAllText(filePath);
            var pattern = JsonConvert.DeserializeObject<Pattern>(text);

            board.PlacePattern(ConvertTo2DArray(pattern.pattern), pattern.position[0], pattern.position[1]);
        }

        // Преобразование зубчатого массива в двумерный
        public static int[,] ConvertTo2DArray(int[][] jaggedArray)
        {
            int rows = jaggedArray.Length;
            int cols = jaggedArray[0].Length;
            int[,] array2D = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array2D[i, j] = jaggedArray[i][j];
                }
            }
            return array2D;
        }
    }
}
