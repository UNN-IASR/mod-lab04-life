using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;
using System.Numerics;
using ScottPlot;
using System.Drawing;

namespace cli_life
{
    // Класс для хранения настроек доски.
    public class Setting
    {
        public int Width { get; set; } // Ширина доски.
        public int Height { get; set; } // Высота доски.
        public int CellSize { get; set; } // Размер ячейки.
        public double LiveDensity { get; set; } // Плотность живых клеток.
    }

    // Класс для работы с фигурами.
    public class Figure
    {
        public int Width { get; set; } // Ширина фигуры.
        public int Height { get; set; } // Высота фигуры.
        public string Name { get; set; } // Название фигуры.
        public int[] Value { get; set; } // Значения фигуры.

        // Метод для чтения фигуры из массива значений.
        public int[,] ReadFigure()
        {
            int[,] figure = new int[Width, Height];
            int k = 0;
            // Заполнение двумерного массива значениями фигуры.
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    figure[i, j] = Value[k];
                    k++;
                }
            }
            return figure;
        }

        // Метод для загрузки фигур из JSON-файла
        public static Figure[] GetFigure(string fileName)
        {
            string name = fileName;
            try
            {
                string str = File.ReadAllText(name);
                Figure[] figure = JsonSerializer.Deserialize<Figure[]>(str);
                return figure;
            }
            catch (IOException ex)
            {
                // Обработка исключения, связанного с вводом-выводом
                Console.WriteLine($"Ошибка чтения файла: {ex.Message}");
                return null; // или возвращаем пустой массив, если это приемлемо для вашего приложения
            }
        }

        // Метод для поиска фигур на доске
        public static int SearchFigureOnBoard(Figure figure, Board board)
        {
            // Инициализация счетчика совпадений и двумерных массивов для хранения текущего состояния доски и фигуры.
            int matchCount = 0;
            int[,] currentBoardState = new int[board.Rows, board.Columns];
            int[,] figureMatrix = figure.ReadFigure();

            // Заполнение двумерного массива текущего состояния доски.
            for (int i = 0; i < board.Rows; i++)//строки
            {
                for (int j = 0; j < board.Columns; j++)//колонки
                {
                    currentBoardState[i, j] = board.Cells[i, j].IsAlive ? 1 : 0;
                }
            }

            // Проходим по каждой клетке доски и сравниваем ее с фигурой.
            for (int i = 0; i < board.Rows; i++)//строки
            {
                for (int j = 0; j < board.Columns; j++)//колонки
                {
                    // Вычисляем координаты текущей клетки относительно фигуры.
                    int figureRow = i - figure.Height / 2;
                    int figureCol = j - figure.Width / 2;

                    // Проверяем, находится ли текущая клетка внутри границ фигуры.
                    if (figureRow >= 0 && figureRow < figure.Height &&
                        figureCol >= 0 && figureCol < figure.Width)
                    {
                        // Сравниваем текущую клетку с соответствующей клеткой фигуры.
                        if (currentBoardState[i, j] == figureMatrix[figureRow, figureCol])
                        {
                            // Если клетки совпадают, увеличиваем счетчик совпадений.
                            matchCount++;
                        }
                    }
                }
            }

            return matchCount;
        }


        // Метод для сравнения двух матриц.
        /*
         static int CompareMatrix(int[,] matrixA, int[,] matrixB)
        {
            int compareResult = 1;
            int rowCount = matrixA.GetLength(0);
            int columnCount = matrixA.GetLength(1);

            // Проверка каждого элемента матриц на равенство.
            for (int i = 0; i < rowCount; i++)//строки
            {
                for (int j = 0; j < columnCount; j++)//колонки
                {
                    if (matrixA[i, j] != matrixB[i, j])
                        compareResult = 0;
                }
            }
            return compareResult;
        }
         */

    }
    // Класс для представления клетки.
    public class Cell
    {
        public bool IsAlive; // Состояние клетки (живая или мертвая).
        public List<Cell> neighbors = new List<Cell>(); // Список соседних клеток.
        private bool IsAliveNext; // Состояние клетки на следующий шаг.

        // Метод для определения следующего состояния клетки.
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            // Правила выживания клетки: 2 или 3 живых соседа - клетка остается живой.
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                // Клетка остается мертвой, если у нее 3 живых соседа.
                IsAliveNext = liveNeighbors == 3;
        }

        // Метод для обновления состояния клетки.
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }

    // Класс для представления доски.
    public class Board
    {
        public Cell[,] Cells; // Массив клеток доски.
        public readonly int CellSize; // Размер ячейки.
        public List<string> position = new List<string>(); // Список позиций доски.

        // Свойства для получения размеров доски.
        public int Columns => Cells.GetLength(0);
        public int Rows => Cells.GetLength(1);
        public int Width => Columns * CellSize;
        public int Height => Rows * CellSize;

        // Конструктор доски.
        public Board(int Width, int Height, int CellSize, double liveDensity = .1)
        {
            this.CellSize = CellSize;

            // Инициализация массива клеток.
            Cells = new Cell[Width / CellSize, Height / CellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            // Подключение соседних клеток.
            ConnectNeighbors();
            // Инициализация случайного состояния клеток.
            Randomize(liveDensity);
        }

        // Метод для подсчета количества живых клеток.
        public int СellAliveCount()
        {
            int k = 0;

            // Подсчет живых клеток.
            foreach (Cell c in Cells)
            {
                if (c.IsAlive)
                    k++;
            }
            return k;
        }

        // Метод для случайного инициализации состояния клеток.
        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        // Метод для обновления состояния доски.
        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }

        // Метод для подключения соседних клеток.
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

                    // Добавление соседних клеток в список соседей.
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

        // Метод для записи текущего состояния доски.
        public string BoardState(Board board)
        {
            StringBuilder boardStatus = new StringBuilder();

            // Запись состояния каждой клетки.
            foreach (Cell cell in board.Cells)
            {
                if (cell.IsAlive)
                    boardStatus.Append('1');
                else
                    boardStatus.Append('0');
            }
            return boardStatus.ToString();
        }

        // Метод для загрузки состояния доски из файла.
        public void LoadBoardState(string filePath)
        {
            string[] fileContent = File.ReadAllLines(filePath);
            Cell[,] updatedCells = new Cell[Rows, Columns];

            // Загрузка состояния доски из файла.
            for (int i = 0; i < Rows; i++)//строки
            {
                for (int j = 0; j < Columns; j++)//колонки
                {
                    if (fileContent[i][j] == '1')
                        updatedCells[i, j] = new Cell { IsAlive = true };
                    else if (fileContent[i][j] == '0')
                        updatedCells[i, j] = new Cell { IsAlive = false };
                }
            }
            Cells = updatedCells;
            ConnectNeighbors();
        }
    }

    // Класс для работы с графиками.
    public class Grafic
    {
        // Метод для подсчета количества живых клеток в каждом поколении.
        public static Dictionary<int, int> CountAliveCellsInGenerations(double generationInterval)
        {
            var result = new Dictionary<int, int>();
            Board board = new Board(100, 30, 1, generationInterval);

            while (true)
            {
                result.Add(board.position.Count, board.СellAliveCount());

                // Если текущее состояние доски уже было ранее, прерываем цикл.
                var currentState = board.BoardState(board);
                if (!board.position.Contains(currentState))
                {
                    board.position.Add(currentState);
                }
                else
                {
                    break;
                }

                // Продвижение доски к следующему поколению.
                board.Advance();
            }
            return result;
        }

        // Метод для создания списка словарей с количеством живых клеток в каждом поколении.
        public static List<Dictionary<int, int>> GenerateList(List<double> generationRates, int num)
        {
            var resultList = new List<Dictionary<int, int>>();

            // Создание списка словарей с количеством живых клеток в каждом поколении.
            // Фильтрация generationRates, оставляя только элементы в диапазоне от 0.4 до 0.8.
            var filteredGenerationRates = generationRates
               .Where(rate => rate >= 0.4 && rate <= 0.8)
               .Take(num)
               .ToList();

            // Добавление в resultList только для фильтрованных generationRates.
            foreach (var rate in filteredGenerationRates)
            {
                resultList.Add(CountAliveCellsInGenerations(rate));
            }

            // Сортировка resultList по количеству живых клеток.
            resultList.Sort((x, y) => x.Count - y.Count);
            return resultList;
        }
        // Метод для создания графика, отображающего количество живых клеток в каждом поколении.
        public static void PlotDataAllive()
        {
            // Создание нового объекта Plot для построения графика.
            var graph = new Plot();
            graph.XLabel("Generation");
            graph.YLabel("Alive Cells");
            graph.ShowLegend();
            // Создание экземпляра класса Random для генерации случайных чисел.
            Random randomGenerator = new Random();
            // Создание списка плотностей для генерации данных.
            List<double> populationDensities = new List<double>() { 0.4, 0.6, 0.8 };
            // Генерация списка словарей с количеством живых клеток в каждом поколении.
            var dataSeries = GenerateList(populationDensities, populationDensities.Count);
            int index = 0;
            // Итерация по каждому словарю в списке.
            foreach (var series in dataSeries)
            {
                // Добавление точек на график для каждого словаря.
                var scatter = graph.Add.Scatter(series.Keys.ToArray(), series.Values.ToArray());
                // Установка метки для каждой серии данных.
                scatter.Label = populationDensities[index].ToString();
                // Установка случайного цвета для каждой серии данных.
                scatter.Color = new ScottPlot.Color(randomGenerator.Next(256), randomGenerator.Next(256), randomGenerator.Next(256));
                index++;
            }
            // Сохранение графика в файле.
            graph.SavePng("plot.png", 1920, 1080);
        }
    }
        // Класс Program, содержащий основной метод Main и вспомогательные методы.
        class Program
        {
            // Статическая переменная для хранения экземпляра класса Board.
            static Board board;
            // Метод для сброса состояния доски из файла конфигурации.
            static private void Reset()
            {
            // Чтение файла конфигурации.
            string fileName = "settings.json";
            try
            {
                string str = File.ReadAllText(fileName);
                Setting set = JsonSerializer.Deserialize<Setting>(str);
                // Создание нового экземпляра класса Board с параметрами из файла конфигурации.
                board = new Board
                    (
                    Width: set.Width,
                    Height: set.Height,
                    CellSize: set.CellSize,
                    liveDensity: set.LiveDensity
                    );
            }
            catch (IOException ex)
            {
                // Обработка исключения, связанного с вводом-выводом
                Console.WriteLine($"Ошибка чтения файла: {ex.Message}");
            }
        }
            // Метод для отображения текущего состояния доски в консоли.
            static void Render()
            {
                // Итерация по каждой клетке доски.
                for (int i = 0; i < board.Rows; i++)//строки
                {
                    for (int j = 0; j < board.Columns; j++)//колонки
                    {
                        // Проверка, является ли клетка живой.
                        var cell = board.Cells[j, i];
                        if (cell.IsAlive)
                        {
                            // Вывод символа '*' для живой клетки.
                            Console.Write('*');
                        }
                        else
                        {
                            // Вывод пробела для мертвой клетки.
                            Console.Write(' ');
                        }
                    }
                    // Переход на новую строку после вывода каждой строки доски.
                    Console.Write('\n');
                }
            }

            // Метод для сохранения текущего состояния доски в файл.
            static void SaveBoardState()
            {
                // Определение имени файла для сохранения состояния доски.
                string fileName = "boardState.txt";
                // Создание потока записи в файл.
                using (StreamWriter fileWriter = new StreamWriter(fileName))
                {
                    // Итерация по каждой клетке доски.
                    for (int i = 0; i < board.Rows; i++)//строки
                    {
                        for (int j = 0; j < board.Columns; j++)//колонки
                        {
                            // Получение состояния клетки.
                            var cell = board.Cells[i, j];
                            // Запись состояния клетки в файл.
                            fileWriter.Write(cell.IsAlive ? '1' : '0');
                        }
                        // Переход на новую строку после вывода каждой строки доски.
                        fileWriter.Write('\n');
                    }
                }
            }

            // Основной метод программы.
            static void Main(string[] args)
            {
                // Вызов метода для создания графика.
                Grafic.PlotDataAllive();
                // Сброс состояния доски из файла конфигурации.
                Reset();
                // Загрузка фигур из файла.
                Figure[] figures = Figure.GetFigure("figures.json");
                string figureName;
                int figureCount = 0;
                int aliveCellsCount = 0;
                int generationCount = 0;
                bool simulationRunning = true;
                // Цикл, выполняющий симуляцию.
                while (simulationRunning)
                {
                    // Отображение текущего состояния доски.
                    Render();
                    // Подсчет количества живых клеток.
                    aliveCellsCount = board.СellAliveCount();
                    Console.WriteLine($"Кол-во живых клеток: {aliveCellsCount}");

                    // Проверка каждой фигуры на доске.
                    foreach (var figure in figures)
                    {
                        figureName = figure.Name;
                        int figurePosition = Figure.SearchFigureOnBoard(figure, board);
                        Console.WriteLine($"{figureName} {figurePosition}");
                    }

                    // Проверка, было ли уже достигнуто текущее состояние доски.
                    if (!board.position.Contains(board.BoardState(board)))
                        board.position.Add(board.BoardState(board));
                    else
                        simulationRunning = false;

                    // Подсчет количества поколений.
                    generationCount = board.position.Count;
                    Console.WriteLine($"Кол-во поколений: {generationCount}");
                    // Продвижение доски к следующему поколению.
                    board.Advance();
                }
            }
        }
}
