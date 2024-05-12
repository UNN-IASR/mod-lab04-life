using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace cli_life
{
    public class liveData
    {
        public int gridWidth { get; set; }
        public int gridHeight { get; set; }
        public int cellDimension { get; set; }
        public double livingCellsRatio { get; set; }
    }

    public class Cell
    {
        public bool isAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;

        public void CalculateNextState()
        {
            int liveNeighbors = neighbors.Count(x => x.isAlive);
            if (isAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }

        public void UpdateState()
        {
            isAlive = IsAliveNext;
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

        public Board(int Width, int Height, int cellSize, double liveDensity)
        {
            CellSize = cellSize;

            Cells = new Cell[Width / cellSize, Height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
        }

        readonly Random rand = new Random();

        public void LoadCellsFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            char[][] gridArray = new char[Rows][];
            for (int i = 0; i < lines.Length; i++)
            {
                gridArray[i] = new char[Columns];
                for (int j = 0; j < Rows; j++)
                {
                    gridArray[i][j] = lines[i][j];
                }
            }
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (gridArray[i][j] == '*')
                    {
                        Cells[i, j].isAlive = true;
                    }
                }
            }
        }

        public void RandomizeBoard(string filePath)
        {
            string jsonContent = File.ReadAllText(filePath);
            liveData data = JsonSerializer.Deserialize<liveData>(jsonContent);
            double CellSize = data.livingCellsRatio;
            foreach (var cell in Cells)
                cell.isAlive = rand.NextDouble() < CellSize;
        }

        public void AdvanceGrid()
        {
            foreach (var cell in Cells)
                cell.CalculateNextState();
            foreach (var cell in Cells)
                cell.UpdateState();
        }

        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xLeft = (x > 0) ? x - 1 : Columns - 1;
                    int xRight = (x < Columns - 1) ? x + 1 : 0;

                    int yTop = (y > 0) ? y - 1 : Rows - 1;
                    int yBottom = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xLeft, yTop]);
                    Cells[x, y].neighbors.Add(Cells[x, yTop]);
                    Cells[x, y].neighbors.Add(Cells[xRight, yTop]);
                    Cells[x, y].neighbors.Add(Cells[xLeft, y]);
                    Cells[x, y].neighbors.Add(Cells[xRight, y]);
                    Cells[x, y].neighbors.Add(Cells[xLeft, yBottom]);
                    Cells[x, y].neighbors.Add(Cells[x, yBottom]);
                    Cells[x, y].neighbors.Add(Cells[xRight, yBottom]);
                }
            }
        }

        public int CountOfBlocks()
        {
            int count = 0;
            for (int i = 1; i < Rows - 2; i++)
            {
                for (int j = 1; j < Columns - 2; j++)
                {
                    if (Cells[j, i].isAlive && Cells[j, i + 1].isAlive && Cells[j + 1, i].isAlive && Cells[j + 1, i + 1].isAlive)
                    {
                        if (!Cells[j - 1, i - 1].isAlive && !Cells[j, i - 1].isAlive && !Cells[j + 1, i - 1].isAlive && !Cells[j + 2, i - 1].isAlive
                        && !Cells[j - 1, i + 2].isAlive && !Cells[j, i + 2].isAlive && !Cells[j + 1, i + 2].isAlive && !Cells[j + 2, i + 2].isAlive
                        && !Cells[j - 1, i].isAlive && !Cells[j + 2, i].isAlive && !Cells[j - 1, i + 2].isAlive && !Cells[j + 2, i + 2].isAlive)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        public int CountOfBoxes()
        {
            int count = 0;
            for (int i = 0; i < Rows - 2; i++)
            {
                for (int j = 1; j < Columns - 1; j++)
                {
                    if (Cells[j, i].isAlive && Cells[j - 1, i + 1].isAlive && Cells[j + 1, i + 1].isAlive && Cells[j, i + 2].isAlive
                    && !Cells[j, i + 1].isAlive && !Cells[j - 1, i].isAlive && !Cells[j + 1, i].isAlive && !Cells[j - 1, i + 2].isAlive && !Cells[j + 1, i + 2].isAlive)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public int CountOfBeehives()
        {
            int count = 0;
            for (int i = 0; i < Rows - 3; i++)
            {
                for (int j = 1; j < Columns - 1; j++)
                {
                    if (Cells[j, i].isAlive && Cells[j - 1, i + 1].isAlive && Cells[j - 1, i + 2].isAlive
                    && Cells[j, i + 3].isAlive && Cells[j + 1, i + 1].isAlive && Cells[j + 1, i + 2].isAlive
                    && !Cells[j, i + 1].isAlive && !Cells[j, i + 2].isAlive && !Cells[j - 1, i].isAlive
                    && !Cells[j + 1, i].isAlive && !Cells[j - 1, i + 3].isAlive && !Cells[j + 1, i + 3].isAlive)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public int CountSymmetryFigures()
        {
            return CountOfBlocks() + CountOfBoxes() + CountOfBeehives();
        }
    }

    public class LifeSimulation
    {
        static Board board;

        public int Initialize(string filePath, string settingsPath)
        {
            string jsonContent = File.ReadAllText(settingsPath);
            liveData data = JsonSerializer.Deserialize<liveData>(jsonContent);
            int gridWidth = data.gridWidth;
            int gridHeight = data.gridHeight;
            int cellDimension = data.cellDimension;
            double livingCellsRatio = data.livingCellsRatio;
            board = new Board(gridWidth, gridHeight, cellDimension, livingCellsRatio);
            board.LoadCellsFromFile(filePath);
            return board.Width * board.Height;
        }

        public int Display()
        {
            int count = 0;
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    var cell = board.Cells[j, i];
                    if (cell.isAlive)
                    {
                        Console.Write('*');
                        count++;
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
            return count;
        }


        public void SaveToFile()
        {
            // Создаем список для хранения строк сетки
            List<List<char>> gridLines = new List<List<char>>();

            // Заполняем список строк сетки, соответствующими текущим размерам сетки
            for (int i = 0; i < board.Rows; i++)
            {
                List<char> line = new List<char>();
                for (int j = 0; j < board.Columns; j++)
                {
                    var cell = board.Cells[j, i];
                    if (cell.isAlive)
                    {
                        line.Add('*');
                    }
                    else
                    {
                        line.Add(' ');
                    }
                }
                gridLines.Add(line);
            }

            // Сохраняем сетку в файл
            File.Create("User settings chd/result4.txt").Close();
            using (StreamWriter writer = new StreamWriter("User settings chd/result4.txt", true))
            {
                foreach (var line in gridLines)
                {
                    writer.WriteLine(string.Join("", line));
                }
            }
        }

        public (int allCells, int aliveCells, int Iters) Run(string filePath, string settingsPath)
        {
            int[] list = { -1, -1, -1, -1, -1 };
            int iters = 0;
            int alive_cells = 0;
            int all_cells = 0;

            all_cells = Initialize(filePath, settingsPath);

            while (true)
            {
                iters++;
                Console.Clear();
                alive_cells = Display();
                list[iters % 5] = alive_cells;
                if ((list[0] == list[1]) && (list[0] == list[2]) && (list[0] == list[3]) && (list[0] == list[4]))
                {
                    break;
                }
                board.AdvanceGrid();
                Thread.Sleep(100);
            }

            Console.WriteLine("\n\tКоличество блоков: " + board.CountOfBlocks());
            Console.WriteLine("\tКоличество ящиков: " + board.CountOfBoxes());
            Console.WriteLine("\tКоличество ульев: " + board.CountOfBeehives());

            (int, int, int) cells = (all_cells, alive_cells, iters - 2);
            return cells;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {// Установка текущей рабочей директории на директорию, где находится файл
            Directory.SetCurrentDirectory(@"C:\1\Lab4");

            LifeSimulation simulation = new LifeSimulation();
            var cells = simulation.Run("User settings chd/example4.txt", @"User settings chd\User settings chd.json");

            Console.Write("\n\tКоличество живых клеток: " + cells.aliveCells);
            Console.Write("\n\tКоличество мертвых клеток: " + (cells.allCells - cells.aliveCells));
            Console.Write("\n\tПлотность живых клеток: " + ((double)cells.aliveCells / cells.allCells));
            Console.Write("\n\tПлотность живых клеток: " + ((double)(cells.allCells - cells.aliveCells) / cells.allCells));

            simulation.SaveToFile();

            Console.Write("\n\n\tСтабильность на " + (cells.Iters) + " итерации.\n\n");
        }
    }
}
