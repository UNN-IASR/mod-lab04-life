using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;

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
        public char charRepresentation()
        {
            return IsAlive ? '1' : '0';
        }
        public void boolRepresentation(char ch)
        {
            IsAlive =  ch == '1' ? true : false;
        }
    }
    public class BoardSettings
    {
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }
        public double liveDensity { get; set; }

        public BoardSettings(int width, int height, int cellSize, double liveDensity)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.liveDensity = liveDensity;
        }

        public BoardSettings()
        {
            width = 0;
            height = 0;
            cellSize = 0;
            liveDensity = 0;
        }

        public BoardSettings(BoardSettings bs)
        {
            this.width = bs.width;
            this.height = bs.height;
            this.cellSize = bs.cellSize;
            this.liveDensity = bs.liveDensity;
        }
        
        public static void writeToFile(string filename, BoardSettings settings)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(filename, jsonString);
        }
    }
    public class Point
    {
        public int x { get; set; }
        public int y { get; set; }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static List<Point> constructListOfPoints(int[] points)
        {
            List<Point> result = new List<Point>();
            for (int i = 0; i < points.Length; i += 2)
            {
                result.Add(new Point(points[i], points[i + 1]));
            }
            return result;
        }
    }
    public class Pattern
    {
        public List<Point> livingСells { get; set; }
        public List<Point> deadCells { get; set; }

        public Pattern(List<Point> livingCells, List<Point> deadCells)
        {
            this.livingСells = livingCells;
            this.deadCells = deadCells;
        }
    }
    public enum Figure
    {
        Block,
        NotRecognized,
        Tub,
        Beehive1,
        Beehive2,
        Pond,
        Ship1,
        Ship2,
        Loaf1,
        Loaf2,
        Loaf3,
        Loaf4,
        Boat1,
        Boat2,
        Boat3,
        Boat4,
        Blinker1,
        Blinker2,
        Eight1,
        Eight2
    }
    public class PatternMap
    {
        public Dictionary<Figure, Pattern> dictOfPatternAndFigure { get; set; }
        public List<Figure> symmetricalFigures;

        public PatternMap()
        {
            dictOfPatternAndFigure = new Dictionary<Figure, Pattern>();
            symmetricalFigures = new List<Figure>();
            List<Point> bufferOfLive = new List<Point>();
            List<Point> bufferOfDead = new List<Point>();
            for(int i = -1; i <= 2; i++)
            {
                for(int j = -1; j <= 2; j++)
                {
                    if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 0) || (i == 1 && j == 1))
                    {
                        bufferOfLive.Add(new Point(i, j));
                    }
                    else
                    {
                        bufferOfDead.Add(new Point(i, j));
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Block, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for(int i = -1; i <= 3; i++)
            {
                for(int j = -2; j <= 2; j++)
                {
                    if(!((i == -1 && j == -2) || (i == 3 && j == -2) || (i == -1 && j == 2) || (i == 3 && j == 2)))
                    {
                        if ((i == 0 && j == 0) || (i == 1 && j == 1) || (i == 1 && j == -1) || (i == 2 && j == 0))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Tub, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 4; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (!((i == -1 && j == -2) || (i == 4 && j == -2) || (i == -1 && j == 2) || (i == 4 && j == 2)))
                    {
                        if ((i == 0 && j == 0) || (i == 1 && j == 1) || (i == 1 && j == -1)
                            || (i == 2 && j == -1) || (i == 2 && j == 1) || (i == 3 && j == 0))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Beehive1, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 4; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (!((i == -1 && j == -2) || (i == 3 && j == -2) || (i == -1 && j == 3) || (i == 3 && j == 3)))
                    {
                        if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 2)
                            || (i == 2 && j == 1) || (i == 2 && j == 0) || (i == 1 && j == -1))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Beehive2, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 4; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (!((i == -1 && j == -2) || (i == 4 && j == -2) || (i == -1 && j == 3) || (i == 4 && j == 3)))
                    {
                        if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 2)
                            || (i == 2 && j == 2) || (i == 3 && j == 0) || (i == 3 && j == 1)
                            || (i == 1 && j == -1) || (i == 2 && j == -1))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Pond, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 3; i++)
            {
                for (int j = -1; j <= 3; j++)
                {
                    if (!((i == 3 && j == -1) || (i == -1 && j == 3)))
                    {
                        if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 0)
                            || (i == 1 && j == 2) || (i == 2 && j == 1) || (i == 2 && j == 2))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Ship1, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 3; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (!((i == -1 && j == -2) || (i == 3 && j == 2)))
                    {
                        if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 1)
                            || (i == 1 && j == -1) || (i == 2 && j == -1) || (i == 2 && j == 0))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Ship2, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 4; i++)
            {
                for (int j = -2; j <= 3; j++)
                {
                    if (!( (i == -1 && j == -2) || (i == 3 && j == -2) || (i == 4 && j == -2) 
                        || (i == 4 && j == -1) || (i == -1 && i == 3) || (i == 4 && j == 3) ))
                    {
                        if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == -1)
                            || (i == 1 && j == 2) || (i == 2 && j == 2) || (i == 2 && j == 0)
                            || (i == 3 && j == 1))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Loaf1, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 4; i++)
            {
                for (int j = -2; j <= 3; j++)
                {
                    if (!((i == -1 && j == -2) || (i == -1 && j == 3) || (i == 4 && j == -2)
                        || (i == 4 && j == 2) || (i == 4 && i == 3) || (i == 3 && j == 3)))
                    {
                        if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == -1)
                            || (i == 2 && j == 1) || (i == 2 && j == -1) || (i == 1 && j == 2)
                            || (i == 3 && j == 0))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Loaf2, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 4; i++)
            {
                for (int j = -2; j <= 3; j++)
                {
                    if (!((i == -1 && j == -2) || (i == -1 && j == 2) || (i == -1 && j == 3)
                        || (i == 0 && j == 3) || (i == 4 && i == -2) || (i == 4 && j == 3)))
                    {
                        if ((i == 0 && j == 0) || (i == 1 && j == 1) || (i == 1 && j == -1)
                            || (i == 2 && j == -1) || (i == 2 && j == 2) || (i == 3 && j == 0)
                            || (i == 3 && j == 1))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Loaf3, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 4; i++)
            {
                for (int j = -3; j <= 2; j++)
                {
                    if (!((i == -1 && j == -3) || (i == -1 && j == -2) || (i == 0 && j == -3)
                        || (i == -1 && j == 2) || (i == 4 && i == -3) || (i == 4 && j == 2) ))
                    {
                        if ((i == 0 && j == 0) || (i == 1 && j == 1) || (i == 1 && j == -1)
                            || (i == 2 && j == 1) || (i == 2 && j == -2) || (i == 3 && j == 0)
                            || (i == 3 && j == -1))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Loaf4, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 3; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (!((i == -1 && j == -2) || (i == -1 && j == 2) || (i == 3 && j == -2)))
                    {
                        if ((i == 0 && j == 0) || (i == 1 && j == 1) || (i == 1 && j == -1)
                            || (i == 2 && j == 1) || (i == 2 && j == 0))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Boat1, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 3; i++)
            {
                for (int j = -1; j <= 3; j++)
                {
                    if (!((i == -1 && j == 3) || (i == 3 && j == -1) || (i == 3 && j == 3)))
                    {
                        if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 0)
                            || (i == 1 && j == 2) || (i == 2 && j == 1))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Boat2, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 3; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (!((i == -1 && j == -2) || (i == 3 && j == -2) || (i == 3 && j == 2)))
                    {
                        if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 1 && j == 1)
                            || (i == 1 && j == -1) || (i == 2 && j == 0))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Boat3, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 3; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (!((i == -1 && j == -2) || (i == -1 && j == 2) || (i == 3 && j == 2)))
                    {
                        if ((i == 0 && j == 0) || (i == 1 && j == 1) || (i == 1 && j == -1)
                            || (i == 2 && j == -1) || (i == 2 && j == 0))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Boat4, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 3; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i == 0 && j == 0) || (i == 1 && j == 0) || (i == 2 && j == 0))
                    {
                        bufferOfLive.Add(new Point(i, j));
                    }
                    else
                    {
                        bufferOfDead.Add(new Point(i, j));
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Blinker1, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 3; j++)
                {
                    if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 0 && j == 2))
                    {
                        bufferOfLive.Add(new Point(i, j));
                    }
                    else
                    {
                        bufferOfDead.Add(new Point(i, j));
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Blinker2, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 2; i++)
            {
                for (int j = -2; j <= 1; j++)
                {
                    if (!((i == -1 && j == -2) || (i == 1 && j == 2)))
                    {
                        if ((i == 0 && j == 0) || (i == 1 && j == -1))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Eight1, new Pattern(bufferOfLive, bufferOfDead));

            bufferOfLive = new List<Point>();
            bufferOfDead = new List<Point>();
            for (int i = -1; i <= 2; i++)
            {
                for (int j = -1; j <= 2; j++)
                {
                    if (!((i == -1 && j == 2) || (i == 2 && j == -1)))
                    {
                        if ((i == 0 && j == 0) || (i == 1 && j == 1))
                        {
                            bufferOfLive.Add(new Point(i, j));
                        }
                        else
                        {
                            bufferOfDead.Add(new Point(i, j));
                        }
                    }
                }
            }
            dictOfPatternAndFigure.Add(Figure.Eight2, new Pattern(bufferOfLive, bufferOfDead));

            symmetricalFigures.Add(Figure.Block);
            symmetricalFigures.Add(Figure.Beehive1);
            symmetricalFigures.Add(Figure.Beehive2);
            symmetricalFigures.Add(Figure.Tub);
            symmetricalFigures.Add(Figure.Pond);
            symmetricalFigures.Add(Figure.Blinker1);
            symmetricalFigures.Add(Figure.Blinker2);
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(1); } }
        public int Rows { get { return Cells.GetLength(0); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[height / cellSize, width / cellSize];
            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Columns; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        public Board(BoardSettings settings) : this(settings.width, settings.height, settings.cellSize, settings.liveDensity)
        {
        }

        public Board(Board board)
        {
            Cells = new Cell[board.Height / board.CellSize, board.Width / board.CellSize];
            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Columns; y++)
                {
                    Cells[x, y] = new Cell();
                    Cells[x, y].IsAlive = board.Cells[x, y].IsAlive;
                }
            ConnectNeighbors();
            CellSize = board.CellSize;
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
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    int xL = (x > 0) ? x - 1 : Rows - 1;
                    int xR = (x < Rows - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Columns - 1;
                    int yB = (y < Columns - 1) ? y + 1 : 0;

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
        public static void WriteToFile(string filename, Board board)
        {
            using (StreamWriter sw = File.CreateText(filename))
            {
                int col = board.Columns;
                int row = board.Rows;
                int cellSize = board.CellSize;

                sw.WriteLine(col.ToString());
                sw.WriteLine(row.ToString());
                sw.WriteLine(cellSize.ToString());

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        sw.Write(board.Cells[i, j].charRepresentation());
                    }
                    sw.Write('\n');
                }
            }
        }
        public static Board ReadFromFile(string filename)
        {
            using (StreamReader sr = File.OpenText(filename))
            {
                string currentStr;

                currentStr = sr.ReadLine();
                int c = int.Parse(currentStr);

                currentStr = sr.ReadLine();
                int r = int.Parse(currentStr);

                currentStr = sr.ReadLine();
                int cellSize = int.Parse(currentStr);

                Board board = new Board(c, r, cellSize, 0);

                for (int i = 0; i < r; i++)
                {
                    currentStr = sr.ReadLine();

                    for (int j = 0; j < c; j++)
                    {
                        char ch = currentStr[j];
                        board.Cells[i, j].boolRepresentation(ch);
                    }
                }

                return board;
            }
        }
        public int getCountOfAliveCells()
        {
            int count = 0;
            for(int i = 0; i < Rows; i++)
            {
                for(int j = 0; j < Columns; j++)
                {
                    if (Cells[i, j].IsAlive)
                    {
                        count += 1;
                    }
                }
            }
            return count;
        }
        public bool cellsMatch(Board valueToCheck)
        {
            for(int i = 0; i < Rows; i++)
            {
                for(int j = 0; j < Columns; j++)
                {
                    if(Cells[i,j].IsAlive != valueToCheck.Cells[i, j].IsAlive)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool cellСomparison(int x, int y, Pattern pattern, bool typeOfComparison)
        {
            List<Point> listForComparison = new List<Point>();
            if (typeOfComparison)
            {
                listForComparison = pattern.livingСells;
            }
            else
            {
                listForComparison = pattern.deadCells;
            }
            foreach(Point point in listForComparison)
            {
                int x_current;
                if(x + point.x < 0)
                {
                    x_current = x + point.x + Rows;
                }
                else
                {
                    x_current = (x + point.x) % Rows;
                }

                int y_current;
                if (y + point.y < 0)
                {
                    y_current = y + point.y + Columns;
                }
                else
                {
                    y_current = (y + point.y) % Columns;
                }

                if (Cells[x_current, y_current].IsAlive != typeOfComparison)
                {
                    return false;
                }
            }
            return true;
        }
        public Dictionary<Figure, List<Point>> recognizePatternsOnBoard(PatternMap patternMap)
        {
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = new Dictionary<Figure, List<Point>>();
            for (int i = 0; i < Rows; i++)
            {
                for(int j = 0; j < Columns; j++)
                {
                    if (Cells[i, j].IsAlive)
                    {
                        Figure resultOfRecognition = Figure.NotRecognized;
                        foreach (KeyValuePair<Figure,Pattern> pattent in patternMap.dictOfPatternAndFigure)
                        {
                            if(cellСomparison(i,j,pattent.Value,true))
                            {
                                if(cellСomparison(i, j, pattent.Value, false))
                                {
                                    resultOfRecognition = pattent.Key;
                                }
                            }
                        }
                        if(resultOfRecognition != Figure.NotRecognized)
                        {
                            List<Point> alreadyRemembered = new List<Point>();
                            if (resultOfPatternRecognation.ContainsKey(resultOfRecognition))
                            {
                                alreadyRemembered = resultOfPatternRecognation[resultOfRecognition];
                                resultOfPatternRecognation.Remove(resultOfRecognition);
                            }
                            alreadyRemembered.Add(new Point(i, j));
                            resultOfPatternRecognation.Add(resultOfRecognition, alreadyRemembered);
                        }
                    }
                }
            }
            return resultOfPatternRecognation;
        }

        public static int countNumberSymmetricalFigures(Dictionary<Figure, List<Point>> resultOfPatternRecognation, PatternMap patternMap)
        {
            int count = 0;
            foreach(var result in resultOfPatternRecognation)
            {
                if (patternMap.symmetricalFigures.Contains(result.Key))
                {
                    count += result.Value.Count();
                }
            }
            return count;
        }
    }
    public class Program
    {
        static Board board;
        static int countSymmetricalFigures = 0;
        static int countOfIteration = 0;
        static public Board Reset(string filename)
        {
            BoardSettings settings = new BoardSettings(50,20,1,0.5f);

            if (File.Exists(filename))
            {
                settings = new BoardSettings(JsonSerializer.Deserialize<BoardSettings>(File.ReadAllText(filename)));
            }
            else
            {
                BoardSettings.writeToFile(filename, settings);
            }

            return new Board(settings);
        }
        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[row, col];
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
        static Board workOfAlgo(PatternMap map)
        {
            Console.Clear();
            Render();
            board.Advance();
            Thread.Sleep(10);
            Dictionary<Figure, List<Point>> resultOfRecognition = board.recognizePatternsOnBoard(map);
            countSymmetricalFigures += Board.countNumberSymmetricalFigures(resultOfRecognition, map);
            countOfIteration += 1;
            return new Board(board);
        }
        static void Main(string[] args)
        {
            string filename = "BoardSettings.json";
            board = Reset(filename);
            Board bufferBoard1 = new Board(board);
            Board bufferBoard2 = new Board(board);
            PatternMap map = new PatternMap();
            do
            {
                bufferBoard1 = workOfAlgo(map);
                bufferBoard2 = workOfAlgo(map);
                workOfAlgo(map);
            } while (!board.cellsMatch(bufferBoard1) && !board.cellsMatch(bufferBoard2));

            Dictionary<Figure, List<Point>> boardResult = board.recognizePatternsOnBoard(map);
            foreach (var buffer in boardResult)
            {
                Console.WriteLine(buffer.Key);
            }
            Console.WriteLine(countSymmetricalFigures);
            Console.WriteLine(countOfIteration);
        }
    }
}