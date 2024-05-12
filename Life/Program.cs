using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using ScottPlot;
using Newtonsoft.Json;

namespace cli_life
{
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
    public class Board
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
    }
    public class Program
    {
        static Board board;
        static private void Reset()
        {
            board = new Board(
                width: 50,
                height: 20,
                cellSize: 1,
                liveDensity: 0.5);
        }

        //Количество живых элементов на каждой итерации
        public static int ElementsAmount(Board b)
        {
            int cnt = 0;
            for (int x = 0; x < b.Columns; x++)
            {
                for (int y = 0; y < b.Rows; y++)
                {
                    if (b.Cells[x, y].IsAlive) cnt++;
                }
            }
            return cnt;
        }

        //Сохранение результатов последнего эксперимента
        public static void SaveResult(String file)
        {
            bool[][] result = new bool[board.Columns][];
            for (int x = 0; x < board.Columns; x++) 
            {
                result[x] = new bool[board.Rows];
                for (int y = 0; y < board.Rows; y++) 
                {
                    result[x][y] = board.Cells[x, y].IsAlive;
                }
            }
            File.WriteAllText(file, System.Text.Json.JsonSerializer.Serialize(result));
        }

        //Сохранение настроек последнего эксперимента
        public static void SaveSettings(String file, Board b)
        {
            File.WriteAllText(file, System.Text.Json.JsonSerializer.Serialize(b));
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
        static void Main(string[] args)
        {
            //Создаём файлы с готовыми настройками для объектов класса Board
            File.WriteAllText("ready_settings1.json", "{\"width\":50,\"height\":20,\"cellSize\":1,\"liveDensity\":0.1}");
            File.WriteAllText("ready_settings2.json", "{\"width\":50,\"height\":20,\"cellSize\":1,\"liveDensity\":0.2}");
            File.WriteAllText("ready_settings3.json", "{\"width\":50,\"height\":20,\"cellSize\":1,\"liveDensity\":0.3}");
            File.WriteAllText("ready_settings4.json", "{\"width\":50,\"height\":20,\"cellSize\":1,\"liveDensity\":0.4}");

            int previousAmount = 0;
            int boardsAmount = 4;
            int[] numberOfGenerations = new int[boardsAmount];
            double[] densities = new double[boardsAmount];
            int averageTime = 0;

            Reset();

            for (int i = 0; i < boardsAmount; i++)
            {
                int sameAmountCounter = 0;
                int generation = 1;
                densities[i] = 0.1 * (i + 1);

                switch(i)
                {
                    case 0:
                        {
                            //Используем настройки из файла ready_settings1.json
                            board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("ready_settings1.json"))!;
                            break;
                        }
                    case 1:
                        {
                            //Используем настройки из файла ready_settings2.json
                            board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("ready_settings2.json"))!;
                            break;
                        }
                    case 2:
                        {
                            //Используем настройки из файла ready_settings3.json
                            board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("ready_settings3.json"))!;
                            break;
                        }
                    case 3:
                        {
                            //Используем настройки из файла ready_settings4.json
                            board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("ready_settings4.json"))!;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                /*
                 * Если кол-во элементов не меняется 10 итераций подряд,
                 * то считаем стабильное состояние найденным
                 */
                while (sameAmountCounter < 10)
                {
                    Console.Clear();
                    Render();

                    Console.WriteLine("Поле: " + (i + 1) + " (плотность: " + (0.1 * (i + 1)) + ")"
                    + "\nПоколение: " + generation
                    + "\nКол-во элементов: " + ElementsAmount(board));

                    if (ElementsAmount(board) == previousAmount) sameAmountCounter++;
                    else sameAmountCounter = 0;

                    Console.WriteLine("Повторений: " + sameAmountCounter + "\n");
                    previousAmount = ElementsAmount(board);
                    generation++;

                    board.Advance();
                    Thread.Sleep(5);
                }

                /*
                 * При подсчёте кол-ва поколений, требуемого для достижения
                 * стабильного состояния, вычитаем 10 повторений
                 */
                numberOfGenerations[i] = generation - 10;

                SaveSettings("settings.json", board);
                SaveResult("unloading.json");
            }

            Console.Clear();
            Console.WriteLine("Итоги:\n");
            for (int a = 0; a < boardsAmount; a++)
            {
                Console.WriteLine("Плотность: " + (0.1 * (a + 1)) + " | поколений: " + numberOfGenerations[a]);
                averageTime += numberOfGenerations[a];
            }
            Console.WriteLine("Среднее время перехода в стабильную фазу (в поколениях): " + averageTime / boardsAmount);


            /*
             * Построение графика зависимости кол-ва поколений, требуемого
             * для достижения стабильного состояния, от плотности заполнения
             */
            ScottPlot.Plot plot = new();
            plot.Add.Scatter(densities, numberOfGenerations, color: Colors.Red);
            plot.Axes.Left.Label.Text = "Количество поколений";
            plot.Axes.Bottom.Label.Text = "Плотности заполнения";
            plot.SavePng("lifeGame.png", 1600, 900);
        }
    }
}
