using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text.Json;

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
        readonly Random rand = new Random();

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public Board(int width, int height, int cellSize, double liveDensity = 0.1)
        {
            CellSize = cellSize;
            Cells = new Cell[width / cellSize, height / cellSize];

            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                }
            }

            connect_neighbors();

            randomize(liveDensity);
        }

        public Board(int width, int height, int cellSize, string fname)
        {
            CellSize = cellSize;
            Cells = new Cell[width / cellSize, height / cellSize];

            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                }
            }

            connect_neighbors();

            foreach (var cell in Cells)
            {
                cell.IsAlive = false;
            }
            string[] lines = File.ReadAllLines(fname + ".txt");
            for (int x = 0; x < lines.Count(); x++)
            {
                List<char> values = lines[x].ToList().FindAll(e => e == '0' || e == '1');
                for (int y = 0; y < values.Count - 1; y++)
                {
                    Cells[y, x].IsAlive = values[y] != '0';
                }
            }
        }

        public List<List<bool>> get_condition()
        {
            List<List<bool>> condition = new List<List<bool>>();
            for (int row = 0; row < Rows; row++)
            {
                condition.Add(new List<bool>());
                for (int col = 0; col < Columns; col++)
                {
                    condition[row].Add(Cells[col, row].IsAlive);
                }
            }
            return condition;
        }

        public void randomize(double liveDensity)
        {
            foreach (var cell in Cells)
            {
                cell.IsAlive = rand.NextDouble() < liveDensity;
            }
        }

        public void advance()
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
        private void connect_neighbors()
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

        public int element_counter(Element element)
        {
            int count = 0;
            foreach (bool[,] condition in element.conditions)
            {
                for (int x = 0; x <= Rows - condition.GetLength(0); x++)
                {
                    for (int y = 0; y <= Columns - condition.GetLength(1); y++)
                    {
                        bool is_coincedence = true;
                        for (int i = 0; i < condition.GetLength(0) && is_coincedence; i++)
                        {
                            for (int j = 0; j < condition.GetLength(1) && is_coincedence; j++)
                            {
                                if (Cells[y + j, x + i].IsAlive != condition[i, j])
                                {
                                    is_coincedence = false;
                                }
                            }
                        }
                        if (is_coincedence)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;

        }

        public float[] percentage_symmetry()
        {
            int horizontal_coincedence_counter = 0;
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    if (Cells[y, x].IsAlive == Cells[y, Rows - x - 1].IsAlive)
                    {
                        horizontal_coincedence_counter++;
                    }
                }
            }
            int wertical_coincedence_counter = 0;
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    if (Cells[y, x].IsAlive == Cells[Columns - y - 1, x].IsAlive)
                    {
                        wertical_coincedence_counter++;
                    }
                }
            }
            int radial_coincedence_counter = 0;
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    if (Cells[y, x].IsAlive == Cells[Columns - y - 1, Rows - x - 1].IsAlive)
                    {
                        radial_coincedence_counter++;
                    }
                }
            }
            return new float[] {
                100 * horizontal_coincedence_counter / (Rows * Columns),
                100 * wertical_coincedence_counter / (Rows * Columns),
                100 * radial_coincedence_counter / (Rows * Columns)
            };
        }
    }

    public class Settings
    {
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }
        public float liveDensity { get; set; }
        public int delay { get; set; }
    }

    public class Element
    {
        public readonly string name;
        public readonly List<bool[,]> conditions;
        public readonly bool is_symmetrical;

        public Element(string name, List<bool[,]> conditions)
        {
            this.name = name;
            this.conditions = conditions;
            is_symmetrical = symmetry();
        }

        bool symmetry()
        {
            foreach (bool[,] x in conditions)
            {
                for (int i = 0; i < x.GetLength(0); i++)
                {
                    for (int j = 0; j < x.GetLength(1); j++)
                    {
                        if (x[i, j] != x[x.GetLength(0) - i - 1, x.GetLength(1) - j - 1])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    class Program
    {
        static Board board;
        static int delay;
        static List<Element> elements = new List<Element>();

        static private void reset()
        {
            string text = File.ReadAllText(@"settings.json");
            Settings settings = JsonSerializer.Deserialize<Settings>(text);

            Console.WriteLine("Start configuration:\nr - random\nf - file");
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo name = Console.ReadKey();
                    switch (name.KeyChar)
                    {
                        case 'r':
                            board = new Board(
                                width: settings.width,
                                height: settings.height,
                                cellSize: settings.cellSize,
                                liveDensity: settings.liveDensity);
                            delay = settings.delay;
                            return;

                        case 'f':
                            Console.WriteLine("\nfile name:");
                            string fname = Console.ReadLine();
                            board = new Board(
                                width: settings.width,
                                height: settings.height,
                                cellSize: settings.cellSize,
                                fname: fname);
                            delay = settings.delay;
                            return;
                    }
                }
            }
        }

        static void render()
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

        static void modeling()
        {
            int genCount = 0;
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo name = Console.ReadKey();
                    if (name.KeyChar == 'q')
                    {
                        break;
                    }
                    else if (name.KeyChar == 's')
                    {
                        string fname = "gen-" + genCount.ToString();
                        StreamWriter writer = new StreamWriter(fname + ".txt");
                        double[,] data = new double[board.Rows, board.Columns];

                        for (int row = 0; row < board.Rows; row++)
                        {
                            for (int col = 0; col < board.Columns; col++)
                            {
                                var cell = board.Cells[col, row];
                                if (cell.IsAlive)
                                {
                                    writer.Write('1');
                                    data[row, col] = 1;
                                }
                                else
                                {
                                    writer.Write('0');
                                    data[row, col] = 0;
                                }
                                writer.Write(',');
                            }
                            writer.Write("\n");
                        }
                        writer.Close();

                        Console.WriteLine("Saved in file: " + fname + ".txt");
                    }
                }

                Console.Clear();
                render();
                board.advance();
                Thread.Sleep(delay);
                ++genCount;
            }
        }

        public static (int, int) counting()
        {
            int alive_count = 0;
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    if (board.Cells[col, row].IsAlive)
                    {
                        alive_count++;
                    }
                }
            }
            return (alive_count, board.Rows * board.Columns - alive_count);
        }

        public static void classification()
        {
            Console.WriteLine("elements");
            foreach (Element element in elements)
            {
                Console.WriteLine(element.name + ": " + board.element_counter(element).ToString());
            }
        }

        public static (int, int) stabilization()
        {
            List<List<List<bool>>> conditions = new List<List<List<bool>>>();
            while (true)
            {
                conditions.Add(board.get_condition());
                Console.Clear();
                render();
                board.advance();
                List<List<bool>> condition = board.get_condition();
                for (int i = 0; i < conditions.Count; i++)
                {
                    bool is_equal = true;
                    for (int x = 0; x < board.Rows && is_equal; x++)
                    {
                        for (int y = 0; y < board.Columns && is_equal; y++)
                        {
                            if (condition[x][y] != conditions[i][x][y])
                            {
                                is_equal = false;
                            }
                        }
                    }
                    if (is_equal)
                    {
                        return (conditions.Count + 1, conditions.Count - i);
                    }
                }
            }
        }

        public static void symmetry()
        {
            Console.WriteLine("symmetrical elements");
            foreach (Element element in elements)
            {
                if (element.is_symmetrical)
                {
                    Console.WriteLine(element.name + ": " + board.element_counter(element).ToString());
                }
            }
            Console.WriteLine("\npercentage of symmetry by steps, horizontal / wertical / radial:");
            int step = 0;
            while (true)
            {
                step++;
                Console.Write("\nstep " + step.ToString() + ")");
                foreach (float x in board.percentage_symmetry())
                {
                    Console.Write(" " + (int)x);
                }
                board.advance();
                Thread.Sleep(delay);
            }
        }

        static void Main()
        {
            elements.Add(new Element("block", new List<bool[,]> {
                new bool[4, 4] {
                    { false, false, false, false },
                    { false, true, true, false },
                    { false, true, true, false },
                    { false, false, false, false }
                }
            }));
            elements.Add(new Element("blinker", new List<bool[,]> {
                new bool[3, 5] {
                    { false, false, false, false, false},
                    { false, true, true, true, false},
                    { false, false, false, false, false}
                },
                new bool[5, 3]
                {
                    { false, false, false },
                    { false, true, false },
                    { false, true, false },
                    { false, true, false },
                    { false, false, false }
                }
            }));
            elements.Add(new Element("glider", new List<bool[,]> {
                new bool[5, 5] {
                    { false, false, false, false, false },
                    { false, false, true, false, false },
                    { false, false, false, true, false },
                    { false, true, true, true, false },
                    { false, false, false, false, false },
                },
                new bool[5, 5] {
                    { false, false, false, false, false },
                    { false, true, false, true, false },
                    { false, false, true, true, false },
                    { false, false, true, false, false },
                    { false, false, false, false, false },
                },
                new bool[5, 5] {
                    { false, false, false, false, false },
                    { false, false, false, true, false },
                    { false, true, false, true, false },
                    { false, false, true, true, false },
                    { false, false, false, false, false },
                },
                new bool[5, 5] {
                    { false, false, false, false, false },
                    { false, true, false, false, false },
                    { false, false, true, true, false },
                    { false, true, true, false, false },
                    { false, false, false, false, false },
                }
            }));

            reset();
            Console.WriteLine(
                "\nm - modeling" +
                "\nn - number of cells" +
                "\nc - classification of elements" +
                "\nt - time of stabilization" +
                "\ns - symmetry and number of symmetrical elements");
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo name = Console.ReadKey();
                    switch (name.KeyChar)
                    {
                        case 'm':
                            Console.Clear();
                            modeling();
                            break;
                        case 'n':
                            Console.Clear();
                            (int alive, int dead) = counting();
                            Console.WriteLine("alive cells: " + alive.ToString() + "\ndead cells: " + dead.ToString());
                            break;
                        case 'c':
                            Console.Clear();
                            classification();
                            break;
                        case 't':
                            Console.Clear();
                            (int step, int duration) = stabilization();
                            Console.WriteLine("\n\ntransition to a stable phase in step: " + step.ToString() + " with cycle duration: " + duration.ToString());
                            break;
                        case 's':
                            Console.Clear();
                            symmetry();
                            break;
                    }
                }
            }
        }
    }
}
