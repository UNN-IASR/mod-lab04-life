using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace LifeSimulation
{
    public class LifeData
    {
        public int gridWidth { get; set; }
        public int gridHeight { get; set; }
        public int cellDimension { get; set; }
        public double livingCellsRatio { get; set; }
    }

    public class LifeCell
    {
        public bool isActive;
        public readonly List<LifeCell> surroundingCells = new List<LifeCell>();
        private bool willBeActive;

        public void CalculateNextState()
        {
            int activeNeighbors = surroundingCells.Count(x => x.isActive);
            if (isActive)
                willBeActive = activeNeighbors == 2 || activeNeighbors == 3;
            else
                willBeActive = activeNeighbors == 3;
        }

        public void UpdateState()
        {
            isActive = willBeActive;
        }
    }

    public class LifeGrid
    {
        public readonly LifeCell[,] gridCells;
        public readonly int cellDimension;

        public int GridColumns { get { return gridCells.GetLength(0); } }
        public int GridRows { get { return gridCells.GetLength(1); } }
        public int GridWidth { get { return GridColumns * cellDimension; } }
        public int GridHeight { get { return GridRows * cellDimension; } }

        public LifeGrid(int gridWidth, int gridHeight, int cellDimension, double livingCellsRatio)
        {
            cellDimension = cellDimension;

            gridCells = new LifeCell[gridWidth / cellDimension, gridHeight / cellDimension];
            for (int x = 0; x < GridColumns; x++)
                for (int y = 0; y < GridRows; y++)
                    gridCells[x, y] = new LifeCell();

            ConnectSurroundingCells();
        }

        readonly Random rand = new Random();

        public void LoadCellsFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            char[][] gridArray = new char[GridRows][];
            for (int i = 0; i < lines.Length; i++)
            {
                gridArray[i] = new char[GridColumns];
                for (int j = 0; j < GridRows; j++)
                {
                    gridArray[i][j] = lines[i][j];
                }
            }
            for (int i = 0; i < GridRows; i++)
            {
                for (int j = 0; j < GridColumns; j++)
                {
                    if (gridArray[i][j] == '*')
                    {
                        gridCells[i, j].isActive = true;
                    }
                }
            }
        }

        public void RandomizeGrid(string filePath)
        {
            string jsonContent = File.ReadAllText(filePath);
            LifeData data = JsonSerializer.Deserialize<LifeData>(jsonContent);
            double livingCellsRatio = data.livingCellsRatio;
            foreach (var cell in gridCells)
                cell.isActive = rand.NextDouble() < livingCellsRatio;
        }

        public void AdvanceGrid()
        {
            foreach (var cell in gridCells)
                cell.CalculateNextState();
            foreach (var cell in gridCells)
                cell.UpdateState();
        }

        private void ConnectSurroundingCells()
        {
            for (int x = 0; x < GridColumns; x++)
            {
                for (int y = 0; y < GridRows; y++)
                {
                    int xLeft = (x > 0) ? x - 1 : GridColumns - 1;
                    int xRight = (x < GridColumns - 1) ? x + 1 : 0;

                    int yTop = (y > 0) ? y - 1 : GridRows - 1;
                    int yBottom = (y < GridRows - 1) ? y + 1 : 0;

                    gridCells[x, y].surroundingCells.Add(gridCells[xLeft, yTop]);
                    gridCells[x, y].surroundingCells.Add(gridCells[x, yTop]);
                    gridCells[x, y].surroundingCells.Add(gridCells[xRight, yTop]);
                    gridCells[x, y].surroundingCells.Add(gridCells[xLeft, y]);
                    gridCells[x, y].surroundingCells.Add(gridCells[xRight, y]);
                    gridCells[x, y].surroundingCells.Add(gridCells[xLeft, yBottom]);
                    gridCells[x, y].surroundingCells.Add(gridCells[x, yBottom]);
                    gridCells[x, y].surroundingCells.Add(gridCells[xRight, yBottom]);
                }
            }
        }

        public int CountBlocks()
        {
            int count = 0;
            for (int i = 1; i < GridRows - 2; i++)
            {
                for (int j = 1; j < GridColumns - 2; j++)
                {
                    if (gridCells[j, i].isActive && gridCells[j, i + 1].isActive && gridCells[j + 1, i].isActive && gridCells[j + 1, i + 1].isActive)
                    {
                        if (!gridCells[j - 1, i - 1].isActive && !gridCells[j, i - 1].isActive && !gridCells[j + 1, i - 1].isActive && !gridCells[j + 2, i - 1].isActive
                        && !gridCells[j - 1, i + 2].isActive && !gridCells[j, i + 2].isActive && !gridCells[j + 1, i + 2].isActive && !gridCells[j + 2, i + 2].isActive
                        && !gridCells[j - 1, i].isActive && !gridCells[j + 2, i].isActive && !gridCells[j - 1, i + 2].isActive && !gridCells[j + 2, i + 2].isActive)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        public int CountBoxes()
        {
            int count = 0;
            for (int i = 0; i < GridRows - 2; i++)
            {
                for (int j = 1; j < GridColumns - 1; j++)
                {
                    if (gridCells[j, i].isActive && gridCells[j - 1, i + 1].isActive && gridCells[j + 1, i + 1].isActive && gridCells[j, i + 2].isActive
                    && !gridCells[j, i + 1].isActive && !gridCells[j - 1, i].isActive && !gridCells[j + 1, i].isActive && !gridCells[j - 1, i + 2].isActive && !gridCells[j + 1, i + 2].isActive)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public int CountHives()
        {
            int count = 0;
            for (int i = 0; i < GridRows - 3; i++)
            {
                for (int j = 1; j < GridColumns - 1; j++)
                {
                    if (gridCells[j, i].isActive && gridCells[j - 1, i + 1].isActive && gridCells[j - 1, i + 2].isActive
                    && gridCells[j, i + 3].isActive && gridCells[j + 1, i + 1].isActive && gridCells[j + 1, i + 2].isActive
                    && !gridCells[j, i + 1].isActive && !gridCells[j, i + 2].isActive && !gridCells[j - 1, i].isActive
                    && !gridCells[j + 1, i].isActive && !gridCells[j - 1, i + 3].isActive && !gridCells[j + 1, i + 3].isActive)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public int CountSymmetryFigures()
        {
            return CountBlocks() + CountBoxes() + CountHives();
        }
    }

    public class LifeSimulation
    {
        static LifeGrid grid;

        public int Initialize(string filePath, string settingsPath)
        {
            string jsonContent = File.ReadAllText(settingsPath);
            LifeData data = JsonSerializer.Deserialize<LifeData>(jsonContent);
            int gridWidth = data.gridWidth;
            int gridHeight = data.gridHeight;
            int cellDimension = data.cellDimension;
            double livingCellsRatio = data.livingCellsRatio;
            grid = new LifeGrid(gridWidth, gridHeight, cellDimension, livingCellsRatio);
            grid.LoadCellsFromFile(filePath);
            return grid.GridWidth * grid.GridHeight;
        }

        public int Display()
        {
            int count = 0;
            for (int i = 0; i < grid.GridRows; i++)
            {
                for (int j = 0; j < grid.GridColumns; j++)
                {
                    var cell = grid.gridCells[j, i];
                    if (cell.isActive)
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
            for (int i = 0; i < grid.GridRows; i++)
            {
                List<char> line = new List<char>();
                for (int j = 0; j < grid.GridColumns; j++)
                {
                    var cell = grid.gridCells[j, i];
                    if (cell.isActive)
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
            File.Create("User_sett/result4.txt").Close();
            using (StreamWriter writer = new StreamWriter("User_sett/result4.txt", true))
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
                grid.AdvanceGrid();
                Thread.Sleep(100);
            }

            Console.WriteLine("\n\tКоличество блоков: " + grid.CountBlocks());
            Console.WriteLine("\tКоличество ящиков: " + grid.CountBoxes());
            Console.WriteLine("\tКоличество ульев: " + grid.CountHives());

            (int, int, int) cells = (all_cells, alive_cells, iters - 2);
            return cells;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {// Установка текущей рабочей директории на директорию, где находится файл
            Directory.SetCurrentDirectory(@"C:\Users\rshah\Desktop\4 курс\2 семестр\Shtanyk\Lab_4");
            
            LifeSimulation simulation = new LifeSimulation();
            var cells = simulation.Run("User_sett/example4.txt", @"User_sett\user_settings.json");

            Console.Write("\n\tКоличество живых клеток: " + cells.aliveCells);
            Console.Write("\n\tКоличество мертвых клеток: " + (cells.allCells - cells.aliveCells));
            Console.Write("\n\tПлотность живых клеток: " + ((double)cells.aliveCells / cells.allCells));
            Console.Write("\n\tПлотность живых клеток: " + ((double)(cells.allCells - cells.aliveCells) / cells.allCells));

            simulation.SaveToFile();

            Console.Write("\n\n\tСтабильность на " + (cells.Iters) + " итерации.\n\n");
        }
    }
}
