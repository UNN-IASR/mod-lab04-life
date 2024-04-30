using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using ScottPlot;
using System.Numerics;
using System.Drawing;
namespace cli_life {
    public class Cell {
        public bool IsAlive;
        public List<Cell> neighbors = new List<Cell>();
        public bool IsAliveNext;
        public void DetermineNextLiveState() {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        //продвижение
        public void Advance() {
            IsAlive = IsAliveNext;
        }
    }
    public class Board {
        public readonly Cell[,] Cells;
        public Cell[,] previousCells;
        public int generationsToStabilnoeSostoyanie = -1;
        public readonly int CellSize;
        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public Board(int width, int height, int cellSize, double liveDensity = .1) {
            CellSize = cellSize;
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();
            ConnectNeighbors();
            Randomize(liveDensity);
        }
        readonly Random rand = new Random();
        public void Randomize(double liveDensity) {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }
        public void Advance() {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        public void ConnectNeighbors() {
            for (int x = 0; x < Columns; x++) {
                for (int y = 0; y < Rows; y++) {
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
        public Board(int width, int height, int cellSize, int[,] cimvol) {
            CellSize = cellSize;
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++) {
                    Cells[x, y] = new Cell();
                    if (cimvol[x, y] == '*') {
                        Cells[x, y].IsAlive = true;
                    }else {
                        Cells[x, y].IsAlive = false;
                    }
                }
            ConnectNeighbors();
            Advance();
        }
        public int counts() {
            int cellCount = 0;
            int combinationCount = 0;
            foreach (var cell in Cells) {
                if (cell.IsAlive == true) {
                    cellCount++;
                }
            }
            //считаем комбинации: 
            //комбинация 1 -  блок
            for (int x = 0; x < Columns; x++) {
                for (int y = 0; y < Rows; y++) {
                    if (x < Columns - 1 && y < Rows - 1 &&
                        Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive &&
                        Cells[x, y + 1].IsAlive && Cells[x + 1, y + 1].IsAlive) {
                        combinationCount++;
                    }
                }
            }
            //комбинация 2 - улей 
            for (int x = 0; x < Columns; x++) {
                for (int y = 0; y < Rows; y++) {
                    if (x < Columns - 2 && y < Rows - 3 &&
                        !Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive && !Cells[x + 2, y].IsAlive &&
                        Cells[x, y + 1].IsAlive && !Cells[x + 1, y + 1].IsAlive && Cells[x + 2, y + 1].IsAlive &&
                        Cells[x, y + 2].IsAlive && !Cells[x + 1, y + 2].IsAlive && Cells[x + 2, y + 2].IsAlive &&
                        !Cells[x, y + 3].IsAlive && Cells[x + 1, y + 3].IsAlive && !Cells[x + 2, y + 3].IsAlive) {
                        combinationCount++;
                    }
                }
            }
            //комбинация 3 - планер
            for (int x = 0; x < Columns - 2; x++) {
                for (int y = 0; y < Rows - 2; y++) {
                    if (!Cells[x, y].IsAlive &&
                        !Cells[x + 1, y].IsAlive && Cells[x + 2, y].IsAlive &&
                        Cells[x, y + 1].IsAlive && !Cells[x + 1, y + 1].IsAlive && Cells[x + 2, y + 1].IsAlive &&
                        !Cells[x, y + 2].IsAlive && Cells[x + 1, y + 2].IsAlive && Cells[x + 2, y + 2].IsAlive) {
                        combinationCount++;
                    }
                }
            }
            //комбинация 4 - пруд
            for (int x = 1; x < Columns - 2; x++) {
                for (int y = 0; y < Rows - 3; y++) {
                    if (Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive && Cells[x - 1, y + 1].IsAlive &&
                        !Cells[x, y + 1].IsAlive && !Cells[x + 1, y + 1].IsAlive && Cells[x + 2, y + 1].IsAlive
                        && Cells[x - 1, y + 2].IsAlive && Cells[x + 2, y + 2].IsAlive &&
                        Cells[x, y + 3].IsAlive && Cells[x + 1, y + 3].IsAlive) {
                        combinationCount++;
                    }
                }
            }
            return cellCount + combinationCount;
        }
        public void ClassificiruemElements() {
            int blockCount = 0;
            int prudCount = 0;
            int bargeCount = 0;
            int zmeyaCount = 0;
            for (int x = 0; x < Columns; x++) {
                for (int y = 0; y < Rows; y++) {
                    if (Cells[x, y].IsAlive) {
                        if (x < Columns - 1 && y < Rows - 1 &&
                            Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive &&
                            Cells[x, y + 1].IsAlive && Cells[x + 1, y + 1].IsAlive) {
                            blockCount++;
                        }
                    }
                }
            }
            for (int x = 1; x < Columns - 2; x++) {
                for (int y = 0; y < Rows - 3; y++) {
                    if (Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive && Cells[x - 1, y + 1].IsAlive &&
                        !Cells[x, y + 1].IsAlive && !Cells[x + 1, y + 1].IsAlive && Cells[x + 2, y + 1].IsAlive
                        && Cells[x - 1, y + 2].IsAlive && Cells[x + 2, y + 2].IsAlive &&
                        Cells[x, y + 3].IsAlive && Cells[x + 1, y + 3].IsAlive) {
                        prudCount++;
                    }
                }
            }
            for (int x = 0; x < Columns - 3; x++) {
                for (int y = 0; y < Rows - 3; y++) {
                    if (!Cells[x, y].IsAlive && Cells[x + 1, y].IsAlive && !Cells[x + 2, y].IsAlive && !Cells[x + 3, y].IsAlive &&
                        Cells[x, y + 1].IsAlive && !Cells[x + 1, y + 1].IsAlive && Cells[x + 2, y + 1].IsAlive && !Cells[x + 3, y + 1].IsAlive &&
                        !Cells[x, y + 2].IsAlive && Cells[x + 1, y + 2].IsAlive && !Cells[x + 2, y + 2].IsAlive && Cells[x + 3, y + 2].IsAlive
                        && !Cells[x, y + 3].IsAlive && !Cells[x + 1, y + 3].IsAlive && Cells[x + 2, y + 3].IsAlive && !Cells[x + 3, y + 3].IsAlive) {
                        bargeCount++;
                    }
                }
            }
            for (int x = 0; x < Columns - 3; x++) {
                for (int y = 0; y < Rows - 1; y++) {
                    if (Cells[x, y].IsAlive &&
                        !Cells[x + 1, y].IsAlive && Cells[x + 2, y].IsAlive && Cells[x + 3, y].IsAlive &&
                        Cells[x, y + 1].IsAlive && Cells[x + 1, y + 1].IsAlive &&
                        !Cells[x + 2, y + 1].IsAlive && Cells[x + 3, y + 1].IsAlive) {
                        zmeyaCount++;
                    }
                }
            }
            if (blockCount != 0) Console.WriteLine("There is block. Block count: " + blockCount);
            if (prudCount != 0) Console.WriteLine("There is prud. Prud count: " + prudCount);
            if (bargeCount != 0) Console.WriteLine("There is barga. Barge count: " + bargeCount);
            if (zmeyaCount != 0) Console.WriteLine("There is zmeya. Zmeya count: " + zmeyaCount);
        }
        public bool StabilnoeSostoyanie() {
            bool isStable = true;
            Board currentBoard = this.Clone();

            for (int i = 0; i < 2; i++) {
                currentBoard.Advance();
            }
            if (!currentBoard.Sovpadenie(this)) {
                isStable = false;
            }
            return isStable;
        }
        public Board Clone() {
            int width = this.Width;
            int height = this.Height;
            int cellSize = this.CellSize;

            Board newBoard = new Board(width, height, cellSize);
            for (int x = 0; x < this.Columns; x++) {
                for (int y = 0; y < this.Rows; y++) {
                    newBoard.Cells[x, y].IsAlive = this.Cells[x, y].IsAlive;
                }
            }
            return newBoard;
        }
        public bool Sovpadenie(Board board) {
            if (this.Columns != board.Columns || this.Rows != board.Rows) return false;

            for (int x = 0; x < this.Columns; x++) {
                for (int y = 0; y < this.Rows; y++) {
                    if (this.Cells[x, y].IsAlive != board.Cells[x, y].IsAlive) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
    public class setting {
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }
        public double liveDensity { get; set; }
    }
    class Program {
        static Board board;
        static Board board1;//Для фигур-колоний загруженных з файла
        static  void Reset() {
            // Путь к файлу JSON
            string filePath = "..\\..\\..\\settings.json";
            // Чтение данных из файла JSON
            string jsonData = File.ReadAllText(filePath);
            // Десериализация данных в объект
            setting data = JsonConvert.DeserializeObject<setting>(jsonData);
            int w = data.width;
            int h = data.height;
            int s = data.cellSize;
            double l = data.liveDensity;
            board = new Board(
                width: w,
                height: h,
                cellSize: s,
                liveDensity: l);
        }
        static void Render() {
            for (int row = 0; row < board.Rows; row++) {
                for (int col = 0; col < board.Columns; col++) {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive) {
                        Console.Write('*');
                    }else {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
        static void Render1() {
            for (int row = 0; row < board1.Rows; row++) {
                for (int col = 0; col < board1.Columns; col++) {
                    var cell = board1.Cells[col, row];
                    if (cell.IsAlive) {
                        Console.Write('*');
                    }else {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
        static void SaveToFile(string fileName) {
            using (StreamWriter sw = new StreamWriter(fileName)) {
                for (int row = 0; row < board.Rows; row++) {
                    for (int col = 0; col < board.Columns; col++) {
                        var cell = board.Cells[col, row];
                        if (cell.IsAlive) {
                            sw.Write('*');
                        }else {
                            sw.Write(' ');
                        }
                    }
                    sw.Write('\n');
                }
            }
        }
        static void LoadFromFile(string fileName) {
            using (StreamReader reader = new StreamReader(fileName)) {
                // Путь к файлу JSON
                string filePath = "..\\..\\..\\settings.json";
                // Чтение данных из файла JSON
                string jsonData = File.ReadAllText(filePath);
                // Десериализация данных в объект
                setting data = JsonConvert.DeserializeObject<setting>(jsonData);
                int w = data.width;
                int h = data.height;
                int s = data.cellSize;
                double l = data.liveDensity;
                int rows = h / s;
                int columns = w / s;
                string[] lines = File.ReadAllLines(fileName);
                int[,] symbol = new int[columns, rows];
                for (int row = 0; row < rows; row++) {
                    char[] a = lines[row].ToCharArray();
                    for (int col = 0; col < columns; col++) {
                        if (a[col] == '*') {
                            symbol[col, row] = '*';
                            Console.Write('*');
                        }else {
                            symbol[col, row] = ' ';
                            Console.Write(' ');
                        }
                    }
                    Console.WriteLine();
                }
                board1 = new Board(w, h, s, symbol);
            }
        }
        static double SredneeVrema() {
            Reset();
            Board board2 = board.Clone();
            int generations = 0;
            int g = 0;
            while (!board.StabilnoeSostoyanie() && g < 1000) {
                generations++;
                Console.WriteLine(generations);
                Console.Clear();
                board2.Advance();
                Render();
                Thread.Sleep(100);
                if (!board.StabilnoeSostoyanie()) {
                    board = board2.Clone();
                }
                g++;
            }
            return (double)generations / 1000;
        }
        static double Sredneevremy1() {
            board1.Advance();
            Reset();
            Board board2 = board1.Clone();
            int generations = 1;
            int g = 0;
            while (!board1.StabilnoeSostoyanie() && g < 1000) {
                generations++;
                Console.WriteLine(generations);
                Console.Clear();
                board2.Advance();
                Render1();
                Thread.Sleep(100);
                if (!board1.StabilnoeSostoyanie()) {
                    board1 = board2.Clone();
                }
                g++;
            }
            return (double)generations / 1000;
        }
        static void Main(string[] args) {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Действие 1");
            Console.WriteLine("2. Действие 2");
            Console.WriteLine("3. Действие 3");
            Console.WriteLine("4. Действие 4");
            Console.WriteLine("5. Действие 5");
            Console.WriteLine("6. Действие 6");
            Console.WriteLine("7. Действие 7");
            Console.WriteLine("8. Действие 8");
            Console.WriteLine("9. Действие 9");
            Console.WriteLine("10. Действие 10");
            Console.WriteLine("11. Действие 11");
            Console.WriteLine("12. Действие 12");
            Console.WriteLine("13. Действие 13");
            int choice = Convert.ToInt32(Console.ReadLine());
            switch (choice) {
                case 1:
                    DoAction1();
                    break;
                case 2:
                    DoAction2();
                    break;
                case 3:
                    DoAction3();
                    break;
                case 4:
                    DoAction4();
                    break;
                case 5:
                    DoAction5();
                    break;
                case 6:
                    DoAction6();
                    break;
                case 7:
                    DoAction7();
                    break;
                case 8:
                    DoAction8();
                    break;
                case 9:
                    DoAction9();
                    break;
                case 10:
                    DoAction10();
                    break;
                case 11:
                    DoAction11();
                    break;
                case 12:
                    DoAction12();
                    break;
                case 13:
                    DoAction13();
                    break;
                default:
                    Console.WriteLine("Некорректный выбор");
                    break;
            }
        }
        static void DoAction1() {
            Console.WriteLine("Вы выбрали действие 1");
            Console.WriteLine("Создать файл с настройками, позволяющими менять параметры КЛА (json-формат).");
            Reset();
            while (true) {
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(1000);
            }
        }
        static void DoAction2() {
            Console.WriteLine("Вы выбрали действие 2");
            Console.WriteLine("Разработать возможность сохранения состояния системы в текстовый файл, загрузку состояния системы из файла и продолжение моделирования.");
            Reset();
            Render();
            SaveToFile("..\\..\\..\\board.txt");
        }
        static void DoAction3() {
            Console.WriteLine("Вы выбрали действие 3");
            Console.WriteLine("Разработать возможность сохранения состояния системы в текстовый файл, загрузку состояния системы из файла и продолжение моделирования.");
            Reset();
            Render();
            SaveToFile("..\\..\\..\\board.txt");
            while (true) {
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(1000);
            }
        }
        static void DoAction4() {
            Console.WriteLine("Вы выбрали действие 4");
            Console.WriteLine("Разработать возможность сохранения состояния системы в текстовый файл, загрузку состояния системы из файла и продолжение моделирования.");
            Reset();
            Render();
            SaveToFile("..\\..\\..\\board.txt");
            board.Advance();
            Thread.Sleep(1000);
            Console.Clear();
            LoadFromFile("..\\..\\..\\board.txt");
            Thread.Sleep(1000);
            while (true) {
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(1000);
            }
        }
        static void DoAction5() {
            Console.WriteLine("Вы выбрали действие 5");
            Console.WriteLine("Подготовить набор файлов с заранее определенными фигурами-колониями, провести загрузку и изучить процесс моделирования.");
            Reset();
            //один из примеров файла, так как в файлах все фигуры усточивы, то их поведение одинково
            LoadFromFile("..\\..\\..\\blok.txt");
            board1.Advance();
            Thread.Sleep(1000);
            while (true) {
                Console.Clear();
                Render1();
                board1.Advance();
                Thread.Sleep(1000);
            }
        }
        static void DoAction6() {
            Console.WriteLine("Вы выбрали действие 6");
            Console.WriteLine("Подсчитать количество элементов (клеток+комбинаций) на поле");
            Reset();
            //один из примеров файла, так как в файлах все фигуры усточивы, то их поведение одинково
            LoadFromFile("..\\..\\..\\blok.txt");
            //5 = 4 + 1(комбинация)
            Console.WriteLine("Количество живых клеток + комбинаций(блок, улей, планер, круг) - " + board1.counts());
        }
        static void DoAction7() {
            Console.WriteLine("Вы выбрали действие 7");
            Console.WriteLine("Подсчитать количество элементов (клеток+комбинаций) на поле");
            Reset();
            Render();
            Console.WriteLine("Количество живых клеток + комбинаций(блок, улей, планер, пруд) - " + board.counts());
        }
        static void DoAction8() {
            Console.WriteLine("Вы выбрали действие 8");
            Console.WriteLine("Классифицировать элементы, сопоставляя с образцами");
            Reset();
            Render();
            board.ClassificiruemElements();
        }
        static void DoAction9() {
            Console.WriteLine("Вы выбрали действие 9");
            Console.WriteLine("Классифицировать элементы, сопоставляя с образцами");
            //LoadFromFile("..\\..\\..\\prud.txt");
            LoadFromFile("..\\..\\..\\barga.txt");
            //LoadFromFile("..\\..\\..\\zmeya.txt");
            board1.ClassificiruemElements();
        }
        static void DoAction10() {
            Console.WriteLine("Вы выбрали действие 10");
            Console.WriteLine("Исследовать среднее время (число поколений) перехода в стабильную фазу");
            Reset();
            //делаем 1000 раз
            LoadFromFile("..\\..\\..\\barga.txt");
            double stability = Sredneevremy1();
            Console.WriteLine("Stability: " + stability);
        }
        static void DoAction11() {
            Console.WriteLine("Вы выбрали действие 11");
            Console.WriteLine("Исследовать среднее время (число поколений) перехода в стабильную фазу");
            Reset();
            //делаем 1000 раз
            double srednee_vremia = SredneeVrema();
            Console.WriteLine("Srednee vremia: " + srednee_vremia);
        }
        static void DoAction12() {
            Console.WriteLine("Вы выбрали действие 12");
            Console.WriteLine("Построить график от одного начального графика перехода в стабильное состояние (числа поколений) от попыток случайного распределения с разной плотностью заполнения поля");
            Reset();
            int[] plotnosti = { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
            double[] stabilnoevremaValues = new double[plotnosti.Length];
            for (int i = 0; i < plotnosti.Length; i++) {
                int plotnost = plotnosti[i];
                board1 = new Board(50, 20, 1, plotnost / 100.0);
                LoadFromFile("..\\..\\..\\board.txt");
                double stabilnoesost = Sredneevremy1();
                stabilnoevremaValues[i] = stabilnoesost;
            }
            for (int i = 0; i < plotnosti.Length; i++) {
                Console.WriteLine(stabilnoevremaValues[i]);
            }
            ScottPlot.Plot plt = new Plot();
            plt.Add.Scatter(plotnosti, stabilnoevremaValues);
            plt.Title("Perechod v stabilnoe sostoyanie");
            plt.XLabel("Plotnost");
            plt.YLabel("Colichestvo pokoleni srednee");
            plt.SavePng("..\\..\\..\\hbc.bmp", 800, 600);
        }
        static void DoAction13() {
            Console.WriteLine("Вы выбрали действие 13");
            Console.WriteLine("Построить график от разных видов игры перехода в стабильное состояние (числа поколений) от попыток случайного распределения с разной плотностью заполнения поля");
            Reset();
            int[] plotnosti = { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
            double[] stabilnoevremaValues = new double[plotnosti.Length];
            for (int i = 0; i < plotnosti.Length; i++) {
                int plotnost = plotnosti[i];
                Board board = new Board(50, 20, 1, plotnost/ 100.0);
                double stabilnoesost = SredneeVrema();
                stabilnoevremaValues[i] = stabilnoesost;
            }
            for (int i = 0; i < plotnosti.Length; i++) {
                Console.WriteLine(stabilnoevremaValues[i]);
            }
            ScottPlot.Plot plt = new Plot();
            plt.Add.Scatter(plotnosti, stabilnoevremaValues);
            plt.Title("Perechod v stabilnoe sostoyanie");
            plt.XLabel("Plotnost");
            plt.YLabel("Colichestvo pokoleni srednee");
            plt.SavePng("..\\..\\..\\hbc.bmp", 800, 600);
        }
    }
}
