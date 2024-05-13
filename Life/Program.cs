using System.Text.Json;
using ScottPlot;

namespace cli_life
{
    public class Settings
    {
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }
        public double liveDensity { get; set; }
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

    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        private readonly Random rand = new Random();

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
        }

        public void SetCellsFromFile(string filename)
        {
            string[] str = File.ReadAllLines(filename);
            char[][] arr = new char[Rows][];
            for (int i = 0; i < str.Length; i++)
            {
                arr[i] = new char[Columns];
                for (int j = 0; j < Rows; j++)
                {
                    arr[i][j] = str[i][j];
                }
            }
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (arr[i][j] == '*')
                    {
                        Cells[i, j].IsAlive = true;
                    }
                }
            }
        }

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

        public int BlocksAmount()
        {
            int num = 0;
            for (int i = 1; i < Rows - 2; i++)
            {
                for (int j = 1; j < Columns - 2; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j, i + 1].IsAlive && Cells[j + 1, i].IsAlive && Cells[j + 1, i + 1].IsAlive)
                    {
                        if (!Cells[j - 1, i - 1].IsAlive && !Cells[j, i - 1].IsAlive && !Cells[j + 1, i - 1].IsAlive && !Cells[j + 2, i - 1].IsAlive
                        && !Cells[j - 1, i + 2].IsAlive && !Cells[j, i + 2].IsAlive && !Cells[j + 1, i + 2].IsAlive && !Cells[j + 2, i + 2].IsAlive
                        && !Cells[j - 1, i].IsAlive && !Cells[j + 2, i].IsAlive && !Cells[j - 1, i + 2].IsAlive && !Cells[j + 2, i + 2].IsAlive)
                        {
                            num++;
                        }
                    }
                }
            }
            return num;
        }

        public int BoxesAmount()
        {
            int num = 0;
            for (int i = 0; i < Rows - 2; i++)
            {
                for (int j = 1; j < Columns - 1; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j - 1, i + 1].IsAlive && Cells[j + 1, i + 1].IsAlive && Cells[j, i + 2].IsAlive
                    && !Cells[j, i + 1].IsAlive && !Cells[j - 1, i].IsAlive && !Cells[j + 1, i].IsAlive && !Cells[j - 1, i + 2].IsAlive && !Cells[j + 1, i + 2].IsAlive)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public int HivesAmount()
        {
            int num = 0;
            for (int i = 0; i < Rows - 3; i++)
            {
                for (int j = 1; j < Columns - 1; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j - 1, i + 1].IsAlive && Cells[j - 1, i + 2].IsAlive
                    && Cells[j, i + 3].IsAlive && Cells[j + 1, i + 1].IsAlive && Cells[j + 1, i + 2].IsAlive
                    && !Cells[j, i + 1].IsAlive && !Cells[j, i + 2].IsAlive && !Cells[j - 1, i].IsAlive
                    && !Cells[j + 1, i].IsAlive && !Cells[j - 1, i + 3].IsAlive && !Cells[j + 1, i + 3].IsAlive)
                    {
                        num++;
                    }
                }
            }
            return num;
        }
    }

    public class GameOfLife
    {
        private Board board;

        public int Reset(string filename, string settings)
        {
            string json = File.ReadAllText(settings);
            Settings data = JsonSerializer.Deserialize<Settings>(json);
            board = new Board(data.width, data.height, data.cellSize);
            board.SetCellsFromFile(filename);
            return board.Width * board.Height;
        }

        public int Reset(double liveDensity, string settings)
        {
            string json = File.ReadAllText(settings);
            Settings data = JsonSerializer.Deserialize<Settings>(json);
            board = new Board(data.width, data.height, data.cellSize);
            board.Randomize(liveDensity);
            return board.Width * board.Height;
        }

        public void Render()
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

        public int CountAliveCells()
        {
            int count = 0;
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void WriteToFile()
        {
            char[][] lines = new char[20][];
            for (int k = 0; k < 20; k++)
            {
                lines[k] = new char[50];
            }
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    var cell = board.Cells[j, i];
                    if (cell.IsAlive)
                    {
                        lines[i][j] = '*';
                    }
                    else
                    {
                        lines[i][j] = ' ';
                    }
                }
            }
            File.Create("../../../../result.txt").Close();
            using (StreamWriter writer = new StreamWriter("../../../../result.txt", true))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    writer.WriteLineAsync(lines[i]);
                }
            }
        }

        public (int allCells, int aliveCells, int Iters) Run(string filename, string settings)
        {
            int[] alives = { -1, -1, -1, -1, -1 };
            int iters = 0;
            int aliveCells;
            int allCells;

            allCells = Reset(filename, settings);

            while (true)
            {
                iters++;
                Console.Clear();
                Render();
                aliveCells = CountAliveCells();
                alives[iters % 5] = aliveCells;
                if ((alives[0] == alives[1]) && (alives[0] == alives[2]) && (alives[0] == alives[3]) && (alives[0] == alives[4]))
                {
                    break;
                }
                board.Advance();
                Thread.Sleep(100);
            }

            Console.WriteLine("\n\tКоличество блоков: " + board.BlocksAmount());
            Console.WriteLine("\tКоличество ящиков: " + board.BoxesAmount());
            Console.WriteLine("\tКоличество ульев: " + board.HivesAmount());

            (int, int, int) cells = (allCells, aliveCells, iters - 2);
            return cells;
        }

        public void CreateGrafic()
        {
            var plot = new Plot();
            plot.XLabel("Итерация");
            plot.YLabel("Живые");
            plot.ShowLegend();
            Random random = new Random();
            List<double> density = new List<double>() { 0.3, 0.4, 0.5, 0.6, 0.7, 0.8 };

            for (int i = 0; i < density.Count; i++)
            {
                Dictionary<int, int> item = new Dictionary<int, int>();
                int[] alives = { -1, -1, -1, -1, -1 };
                int iters = 0;
                int aliveCells;
                int allCells;

                allCells = Reset(density[i], "../../../../settings.json");

                while (true)
                {
                    iters++;
                    aliveCells = CountAliveCells();
                    alives[iters % 5] = aliveCells;
                    if ((alives[0] == alives[1]) && (alives[0] == alives[2]) && (alives[0] == alives[3]) && (alives[0] == alives[4]))
                    {
                        break;
                    }
                    board.Advance();
                    item.Add(iters, aliveCells);
                }

                var drow = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                drow.Label = density[i].ToString();
                drow.Color = new Color(random.Next(256), random.Next(256), random.Next(256));
            }
            plot.SavePng("../../../../plot.png", 1920, 1080);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GameOfLife life = new GameOfLife();
            var cells = life.Run("../../../../example1.txt", "../../../../settings.json");

            Console.Write("\n\tКоличество живых клеток: " + cells.aliveCells);
            Console.Write("\n\tКоличество мертвых клеток: " + (cells.allCells - cells.aliveCells));
            Console.Write("\n\tПлотность живых клеток: " + ((double)cells.aliveCells / cells.allCells));
            Console.Write("\n\tПлотность мертвых клеток: " + ((double)(cells.allCells - cells.aliveCells) / cells.allCells));
            Console.Write("\n\n\tСтабильность на " + (cells.Iters) + " итерации.\n\n");

            life.WriteToFile();
            life.CreateGrafic();
        }
    }
}