using Newtonsoft.Json;
using ScottPlot;
namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public List<Cell> neighbors = new List<Cell>();
        public bool IsAliveNext;
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
        public Cell[,]? previousCells;
        public int generationsTillStableState = -1;
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
        public void ConnectNeighbors()
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
        public Board(int width, int height, int cellSize, int[,] map)
        {
            CellSize = cellSize;
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                    if (map[x, y] == '*')
                    {
                        Cells[x, y].IsAlive = true;
                    }
                    else
                    {
                        Cells[x, y].IsAlive = false;
                    }
                }
            ConnectNeighbors();
            Advance();
        }
        public int counts()
        {
            int cellCount = 0;
            int combinationCount = 0;
            foreach (var cell in Cells)
            {
                if (cell.IsAlive == true)
                {
                    cellCount++;
                }
            }
            //считаем комбинации: 
            //комбинация 1 -  блок
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (x < Columns - 1 && y < Rows - 1 &&
                        Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive &&
                        Cells[x, y + 1].IsAlive && Cells[x + 1, y + 1].IsAlive)
                    {
                        combinationCount++;
                    }
                }
            }
            //комбинация 2 - улей 
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (x < Columns - 2 && y < Rows - 3 &&
                        !Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive && !Cells[x + 2, y].IsAlive &&
                        Cells[x, y + 1].IsAlive && !Cells[x + 1, y + 1].IsAlive && Cells[x + 2, y + 1].IsAlive &&
                        Cells[x, y + 2].IsAlive && !Cells[x + 1, y + 2].IsAlive && Cells[x + 2, y + 2].IsAlive &&
                        !Cells[x, y + 3].IsAlive && Cells[x + 1, y + 3].IsAlive && !Cells[x + 2, y + 3].IsAlive)
                    {
                        combinationCount++;
                    }
                }
            }
            //комбинация 3 - планер
            for (int x = 0; x < Columns - 2; x++)
            {
                for (int y = 0; y < Rows - 2; y++)
                {
                    if (!Cells[x, y].IsAlive &&
                        !Cells[x + 1, y].IsAlive && Cells[x + 2, y].IsAlive &&
                        Cells[x, y + 1].IsAlive && !Cells[x + 1, y + 1].IsAlive && Cells[x + 2, y + 1].IsAlive &&
                        !Cells[x, y + 2].IsAlive && Cells[x + 1, y + 2].IsAlive && Cells[x + 2, y + 2].IsAlive)
                    {
                        combinationCount++;
                    }
                }
            }
            //комбинация 4 - пруд
            for (int x = 1; x < Columns - 2; x++)
            {
                for (int y = 0; y < Rows - 3; y++)
                {
                    if (Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive && Cells[x - 1, y + 1].IsAlive &&
                        !Cells[x, y + 1].IsAlive && !Cells[x + 1, y + 1].IsAlive && Cells[x + 2, y + 1].IsAlive
                        && Cells[x - 1, y + 2].IsAlive && Cells[x + 2, y + 2].IsAlive &&
                        Cells[x, y + 3].IsAlive && Cells[x + 1, y + 3].IsAlive)
                    {
                        combinationCount++;
                    }
                }
            }
            return cellCount + combinationCount;
        }
        public void ClassifyElements()
        {
            int blockCount = 0;
            int prudCount = 0;
            int bargeCount = 0;
            int zmeyaCount = 0;
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (Cells[x, y].IsAlive)
                    {
                        if (x < Columns - 1 && y < Rows - 1 &&
                            Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive &&
                            Cells[x, y + 1].IsAlive && Cells[x + 1, y + 1].IsAlive)
                        {
                            blockCount++;
                        }
                    }
                }
            }
            for (int x = 1; x < Columns - 2; x++)
            {
                for (int y = 0; y < Rows - 3; y++)
                {
                    if (Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive && Cells[x - 1, y + 1].IsAlive &&
                        !Cells[x, y + 1].IsAlive && !Cells[x + 1, y + 1].IsAlive && Cells[x + 2, y + 1].IsAlive
                        && Cells[x - 1, y + 2].IsAlive && Cells[x + 2, y + 2].IsAlive &&
                        Cells[x, y + 3].IsAlive && Cells[x + 1, y + 3].IsAlive)
                    {
                        prudCount++;
                    }
                }
            }
            for (int x = 0; x < Columns - 3; x++)
            {
                for (int y = 0; y < Rows - 3; y++)
                {
                    if (!Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive && !Cells[x + 2, y].IsAlive && !Cells[x + 3, y].IsAlive &&
                        Cells[x, y + 1].IsAlive && !Cells[x + 1, y + 1].IsAlive && Cells[x + 2, y + 1].IsAlive && !Cells[x + 3, y + 1].IsAlive &&
                        !Cells[x, y + 2].IsAlive && Cells[x + 1, y + 2].IsAlive && !Cells[x + 2, y + 2].IsAlive && Cells[x + 3, y + 2].IsAlive
                        && !Cells[x, y + 3].IsAlive && !Cells[x + 1, y + 3].IsAlive && Cells[x + 2, y + 3].IsAlive && !Cells[x + 3, y + 3].IsAlive)
                    {
                        bargeCount++;
                    }
                }
            }
            for (int x = 0; x < Columns - 3; x++)
            {
                for (int y = 0; y < Rows - 1; y++)
                {
                    if (Cells[x, y].IsAlive &&
                        !Cells[x + 1, y].IsAlive && Cells[x + 2, y].IsAlive && Cells[x + 3, y].IsAlive &&
                        Cells[x, y + 1].IsAlive && Cells[x + 1, y + 1].IsAlive &&
                        !Cells[x + 2, y + 1].IsAlive && Cells[x + 3, y + 1].IsAlive)
                    {
                        zmeyaCount++;
                    }
                }
            }
            if (blockCount != 0) Console.WriteLine("Найден блок. Кол-во блоков: " + blockCount);
            if (prudCount != 0) Console.WriteLine("Найден пруд. Кол-во прудов: " + prudCount);
            if (bargeCount != 0) Console.WriteLine("Найдена баржа. Кол-во барж: " + bargeCount);
            if (zmeyaCount != 0) Console.WriteLine("Найдена змея. Кол-во змей: " + zmeyaCount);
        }
        public bool StableState()
        {
            bool isStable = true;
            Board currentBoard = this.Clone();

            for (int i = 0; i < 2; i++)
            {
                currentBoard.Advance();
            }
            if (!currentBoard.Overlap(this))
            {
                isStable = false;
            }
            return isStable;
        }
        public Board Clone()
        {
            int width = this.Width;
            int height = this.Height;
            int cellSize = this.CellSize;

            Board newBoard = new Board(width, height, cellSize);
            for (int x = 0; x < this.Columns; x++)
            {
                for (int y = 0; y < this.Rows; y++)
                {
                    newBoard.Cells[x, y].IsAlive = this.Cells[x, y].IsAlive;
                }
            }
            return newBoard;
        }
        public bool Overlap(Board board)
        {
            if (this.Columns != board.Columns || this.Rows != board.Rows) return false;

            for (int x = 0; x < this.Columns; x++)
            {
                for (int y = 0; y < this.Rows; y++)
                {
                    if (this.Cells[x, y].IsAlive != board.Cells[x, y].IsAlive)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
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
        static Board? board;

        static void Reset()
        {
            string filePath = "..\\..\\..\\settings.json";
            string jsonData = File.ReadAllText(filePath);
            Settings? data = JsonConvert.DeserializeObject<Settings>(jsonData);
            int w = data.Width;
            int h = data.Height;
            int s = data.CellSize;
            double l = data.LiveDensity;
            board = new Board(
                width: w,
                height: h,
                cellSize: s,
                liveDensity: l);
        }
        static void Render(Board board)
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
        static void SaveToFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Columns; col++)
                    {
                        var cell = board.Cells[col, row];
                        if (cell.IsAlive)
                        {
                            sw.Write('*');
                        }
                        else
                        {
                            sw.Write(' ');
                        }
                    }
                    sw.Write('\n');
                }
            }
        }
        static void LoadFromFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string filePath = "..\\..\\..\\settings.json";
                string jsonData = File.ReadAllText(filePath);
                Settings? data = JsonConvert.DeserializeObject<Settings>(jsonData);
                int w = data.Width;
                int h = data.Height;
                int s = data.CellSize;
                double l = data.LiveDensity;
                int rows = h / s;
                int columns = w / s;
                string[] lines = File.ReadAllLines(fileName);
                int[,] symbol = new int[columns, rows];
                for (int row = 0; row < rows; row++)
                {
                    char[] a = lines[row].ToCharArray();
                    for (int col = 0; col < columns; col++)
                    {
                        if (a[col] == '*')
                        {
                            symbol[col, row] = '*';
                            Console.Write('*');
                        }
                        else
                        {
                            symbol[col, row] = ' ';
                            Console.Write(' ');
                        }
                    }
                    Console.WriteLine();
                }
                board = new Board(w, h, s, symbol);
            }
        }
        static double MeanTime(Board board)
        {
            board.Advance();
            Reset();
            Board board2 = board.Clone();
            int generations = 0;
            int g = 0;
            while (!board.StableState() && g < 1000)
            {
                generations++;
                board2.Advance();
                if (!board.StableState())
                {
                    board = board2.Clone();
                }
                g++;
            }
            return (double)generations / 1000;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Запустить симуляцию");
            Console.WriteLine("2. Запустить симуляцию, сохранить ее в файл");
            Console.WriteLine("3. Запустить симуляцию, сохранить ее в файл, продолжить моделирование");
            Console.WriteLine("4. Загрузить фигуру и начать моделирование");
            Console.WriteLine("5. Подсчитать количество элементов (клеток+комбинаций) на поле (из файла)");
            Console.WriteLine("6. Подсчитать количество элементов (клеток+комбинаций) на поле (случайно сгенерированном)");
            Console.WriteLine("7. Классифицировать элементы, сопоставляя с образцами на поле (случайно сгенерированном)");
            Console.WriteLine("8. Классифицировать элементы, сопоставляя с образцами на поле (из файла)");
            Console.WriteLine("9. Исследовать среднее время (число поколений) перехода в стабильную фазу");
            Console.WriteLine("10. Построить график от одного начального графика перехода в стабильное состояние (числа поколений) от попыток случайного распределения с разной плотностью заполнения поля");
            int choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    Menu1();
                    break;
                case 2:
                    Menu2();
                    break;
                case 3:
                    Menu3();
                    break;
                case 4:
                    Menu4();
                    break;
                case 5:
                    Menu5();
                    break;
                case 6:
                    Menu6();
                    break;
                case 7:
                    Menu7();
                    break;
                case 8:
                    Menu8();
                    break;
                case 9:
                    Menu9();
                    break;
                case 10:
                    Menu10();
                    break;
                default:
                    Console.WriteLine("Некорректный выбор");
                    break;
            }
        }
        static void Menu1()
        {
            Reset();
            while (true)
            {
                Console.Clear();
                Render(board);
                board.Advance();
                Thread.Sleep(1000);
            }
        }
        static void Menu2()
        {
            Reset();
            Render(board);
            SaveToFile("..\\..\\..\\board.txt");
        }
        static void Menu3()
        {
            Reset();
            Render(board);
            SaveToFile("..\\..\\..\\board.txt");
            while (true)
            {
                Console.Clear();
                Render(board);
                board.Advance();
                Thread.Sleep(1000);
            }
        }
        static void Menu4()
        {
            Reset();
            LoadFromFile("..\\..\\..\\prud.txt");
            board.Advance();
            //board_from_file.Advance();
            Thread.Sleep(1000);
            while (true)
            {
                Console.Clear();
                Render(board);
                board.Advance();
                Thread.Sleep(1000);
            }
        }
        static void Menu5()
        {
            Reset();
            //один из примеров файла, так как в файлах все фигуры усточивы, то их поведение одинаково
            LoadFromFile("..\\..\\..\\blok.txt");
            Console.WriteLine("Количество живых клеток + комбинаций(блок, улей, планер, круг) - " + board.counts());
        }
        static void Menu6()
        {
            Reset();
            Render(board);
            Console.WriteLine("Количество живых клеток + комбинаций(блок, улей, планер, пруд) - " + board.counts());
        }
        static void Menu7()
        {
            Reset();
            Render(board);
            board.ClassifyElements();
        }
        static void Menu8()
        {
            //LoadFromFile("..\\..\\..\\prud.txt");
            LoadFromFile("..\\..\\..\\barga.txt");
            //LoadFromFile("..\\..\\..\\zmeya.txt");
            board.ClassifyElements();
        }
        static void Menu9()
        {
            Reset();
            //делаем 1000 раз
            double srednee_vremia = MeanTime(board);
            Console.WriteLine("Srednee vremia: " + srednee_vremia);
        }
        static void Menu10()
        {
            Console.WriteLine("Ожидайте..");
            Reset();
            int[] plotnosti = { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
            double[] stabilnoevremaValues = new double[plotnosti.Length];
            for (int i = 0; i < plotnosti.Length; i++)
            {
                int plotnost = plotnosti[i];
                Board board = new Board(50, 20, 1, plotnost / 100.0);
                double stabilnoesost = MeanTime(board);
                stabilnoevremaValues[i] = stabilnoesost;
            }
            for (int i = 0; i < plotnosti.Length; i++)
            {
                Console.WriteLine(stabilnoevremaValues[i]);
            }
            ScottPlot.Plot plt = new Plot();
            plt.Add.Scatter(plotnosti, stabilnoevremaValues);
            plt.Title("Переход в стабильное состояние");
            plt.XLabel("Плотность");
            plt.YLabel("Среднее кол-во поколений");
            plt.SavePng("..\\..\\..\\hbc.bmp", 800, 600);
        }
    }
}
