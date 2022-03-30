using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace Life
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
        public int X { get; set; }
        public int Y { get; set; }
        public static implicit operator Cell(bool v)
        {
            throw new NotImplementedException();
        }
        public bool Dead { get; set; }
        public bool Stable { get; set; }
        public bool NotStable { get; set; }
        public int LiveCount { get; set; }
        public int DeadCount { get; set; }
        public bool InFigure { get; set; }
    }
    public class Board
    {
        public int Columns { get { return Cells.GetLength(0); } set { Columns = value; } }
        public int Rows { get { return Cells.GetLength(1); } set { Rows = value; } }
        public int Width { get { return Columns * CellSize; } set { Width = value; } }
        public int Height { get { return Rows * CellSize; } set { Height = value; } }
        public int CellSize { get; set; }
        public string CellStates { get; set; }
        [JsonIgnore]
        public Cell[,] Cells { get; set; }
        [JsonIgnore]
        public int Stable { get; set; }
        [JsonIgnore]
        public int Periodic { get; set; }
        [JsonIgnore]
        public int Moving { get; set; }
        [JsonIgnore]
        public List<List<int[]>> Figures { get; set; }
        [JsonIgnore]
        public int Barge { get; set; }
        [JsonIgnore]
        public int Hive { get; set; }
        [JsonIgnore]
        public int Block { get; set; }
        [JsonIgnore]
        public int Box { get; set; }
        [JsonIgnore]
        public int Boat { get; set; }
        [JsonIgnore]
        public int Ship { get; set; }
        [JsonIgnore]
        public int Pond { get; set; }
        [JsonIgnore]
        public int Flasher { get; set; }

        public Board(int width = 50, int height = 20, int cellSize = 1, double liveDensity = .5)
        {
            CellSize = cellSize;
            Cells = new Cell[width / cellSize, height / cellSize];
            CellStates = "";
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                    Cells[x, y].X = x;
                    Cells[x, y].Y = y;
                }
            Stable = 0;
            Periodic = 0;
            Moving = 0;
            Figures = new List<List<int[]>>();
            Block = 0;
            Barge = 0;
            Hive = 0;
            Box = 0;
            Boat = 0;
            Ship = 0;
            Pond = 0;
            Flasher = 0;
            ConnectNeighbors();
            Randomize(liveDensity);
        }
        public Board(int width, int height, int cellSize, string CellStates)
        {
            CellSize = cellSize;
            this.CellStates = CellStates;
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                    Cells[x, y].X = x;
                    Cells[x, y].Y = y;
                }
            Stable = 0;
            Periodic = 0;
            Moving = 0;
            Figures = new List<List<int[]>>();
            Block = 0;
            Barge = 0;
            Hive = 0;
            Box = 0;
            Boat = 0;
            Ship = 0;
            Pond = 0;
            Flasher = 0;
            ConnectNeighbors();
            LoadCellsStates();
        }
        public Board() { }
        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }
        public void SaveCellsStates()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    CellStates = CellStates + ((Cells[x, y].IsAlive) ? "1" : "0");
                }
            }
        }
        public void LoadCellsStates()
        {
            int length = CellStates.Length;
            if (length < Columns * Rows)
                for (int i = 0; i < Columns * Rows - length; i++)
                {
                    CellStates = CellStates + "1";
                }
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (CellStates[x * Rows + y].Equals('1'))
                        Cells[x, y].IsAlive = true;
                    else Cells[x, y].IsAlive = false;
                }
            }
        }
        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y].Stable = false;
                    Cells[x, y].NotStable = false;
                    Cells[x, y].Dead = false;
                    if (Cells[x, y].IsAlive)
                    {
                        Cells[x, y].DeadCount = 0;
                        Cells[x, y].LiveCount++;
                    }
                    else
                    {
                        Cells[x, y].DeadCount++;
                        Cells[x, y].LiveCount = 0;
                    }
                }
            }
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
                    Cells[x, y].Dead = false;
                    Cells[x, y].Stable = false;
                    Cells[x, y].NotStable = false;
                    Cells[x, y].DeadCount = 0;
                    Cells[x, y].LiveCount = 0;
                    Cells[x, y].InFigure = false;
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
        public int FindFigure(int x, int y)
        {
            int k = 0;
            for (int i = 0; i < Figures.Count(); i++)
            {
                for (int j = 0; j < Figures[i].Count(); j++)
                {
                    if (Figures[i][j][0] == x && Figures[i][j][1] == y)
                    {
                        k = i;
                    }
                }
            }
            return k;
        }
        public void DisplayFigures()
        {
            for (int i = 0; i < Figures.Count(); i++)
            {
                Console.Write("{");
                for (int j = 0; j < Figures[i].Count(); j++)
                {
                    Console.Write("(" + Figures[i][j][0] + ", " + Figures[i][j][1] + ")");
                }
                Console.WriteLine("}");
            }
        }
        public void SumFigures()
        {
            for (int i = 0; i < Figures.Count(); i++)
            {
                for (int j = i + 1; j < Figures.Count(); j++)
                {
                    for (int k = 0; k < Figures[i].Count(); k++)
                    {
                        for (int s = 0; s < Figures[j].Count(); s++)
                        {
                            for (int v = 0; v < 8; v++)
                            {
                                if (Cells[Figures[i][k][0], Figures[i][k][1]].neighbors[v].X
                                    == Cells[Figures[j][s][0], Figures[j][s][1]].X &&
                                    Cells[Figures[i][k][0], Figures[i][k][1]].neighbors[v].Y
                                    == Cells[Figures[j][s][0], Figures[j][s][1]].Y)
                                {
                                    Figures[i].Add(Figures[j][s]);
                                    Figures[j].RemoveAt(s);
                                    s--;
                                    if (s < 0)
                                        s = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < Figures.Count(); i++)
            {
                if (Figures[i].Count() == 0)
                {
                    Figures.RemoveAt(i);
                    i--;
                    if (i < 0) i = 0;
                }
            }
        }
        public int SimmetricFigures()
        {
            int k = 0;
            int minX1 = Columns;
            int minX2 = Columns;
            int minY1 = Rows;
            int minY2 = Rows;
            int maxX1 = -1;
            int maxX2 = -1;
            int maxY1 = -1;
            int maxY2 = -1;
            int width = 0;
            int height = 0;
            bool simmX = true;
            bool simmY = true;
            bool[,] simmetricFigure = new bool[Columns, Rows];
            Block = 0;
            Barge = 0;
            Hive = 0;
            Box = 0;
            Boat = 0;
            Ship = 0;
            Pond = 0;
            Flasher = 0;
            for (int i = 0; i < Figures.Count(); i++)
            {
                minX1 = Columns;
                minX2 = Columns;
                minY1 = Rows;
                minY2 = Rows;
                maxX1 = -1;
                maxX2 = -1;
                maxY1 = -1;
                maxY2 = -1;
                simmX = true;
                simmY = true;
                for (int j = 0; j < Figures[i].Count(); j++)
                {
                    if (Figures[i][j][0] > maxX1) maxX1 = Figures[i][j][0];
                    if (Figures[i][j][0] < minX1) minX1 = Figures[i][j][0];
                    if (Figures[i][j][1] > maxY1) maxY1 = Figures[i][j][1];
                    if (Figures[i][j][1] < minY1) minY1 = Figures[i][j][1];
                }
                if (maxX1 - minX1 == Columns - 1 || maxY1 - minY1 == Rows - 1)
                {
                    if (maxX1 - minX1 == Columns - 1 && maxY1 - minY1 == Rows - 1)
                    {
                        minX1 = Columns;
                        minX2 = Columns;
                        maxX1 = -1;
                        maxX2 = -1;
                        minY1 = Rows;
                        minY2 = Rows;
                        maxY1 = -1;
                        maxY2 = -1;
                        for (int j = 0; j < Figures[i].Count(); j++)
                        {
                            if (Figures[i][j][0] > maxX1 && Figures[i][j][0] < Columns / 2) maxX1 = Figures[i][j][0];
                            if (Figures[i][j][0] > maxX2) maxX2 = Figures[i][j][0];
                            if (Figures[i][j][0] < minX1) minX1 = Figures[i][j][0];
                            if (Figures[i][j][0] < minX2 && Figures[i][j][0] >= Columns / 2) minX2 = Figures[i][j][0];
                            if (Figures[i][j][1] > maxY1 && Figures[i][j][1] < Rows / 2) maxY1 = Figures[i][j][1];
                            if (Figures[i][j][1] > maxY2) maxY2 = Figures[i][j][1];
                            if (Figures[i][j][1] < minY1) minY1 = Figures[i][j][1];
                            if (Figures[i][j][1] < minY2 && Figures[i][j][1] >= Rows / 2) minY2 = Figures[i][j][1];
                        }
                        width = (maxX1 - minX1) + (maxX2 - minX2) + 2;
                        height = (maxY1 - minY1) + (maxY2 - minY2) + 2;
                        simmetricFigure = new bool[width, height];
                        for (int j = 0; j < width; j++)
                        {
                            for (int s = 0; s < height; s++)
                            {
                                if (j < maxX2 - minX2 + 1)
                                {
                                    if (s < maxY2 - minY2 + 1)
                                        simmetricFigure[j, s] = Cells[minX2 + j, minY2 + s].IsAlive;
                                    else simmetricFigure[j, s] = Cells[minX2 + j, minY1 + s - (maxY2 - minY2 + 1)].IsAlive;
                                }
                                else
                                {
                                    if (s < maxY2 - minY2 + 1)
                                        simmetricFigure[j, s] = Cells[minX1 + j - (maxX2 - minX2 + 1), minY2 + s].IsAlive;
                                    else simmetricFigure[j, s] = Cells[minX1 + j - (maxX2 - minX2 + 1), minY1 + s - (maxY2 - minY2 + 1)].IsAlive;
                                }

                            }
                        }
                    }
                    else if (maxX1 - minX1 != Columns - 1 && maxY1 - minY1 == Rows - 1)
                    {
                        minY1 = Rows;
                        minY2 = Rows;
                        maxY1 = -1;
                        maxY2 = -1;
                        for (int j = 0; j < Figures[i].Count(); j++)
                        {
                            if (Figures[i][j][1] > maxY1 && Figures[i][j][1] < Rows / 2) maxY1 = Figures[i][j][1];
                            if (Figures[i][j][1] > maxY2) maxY2 = Figures[i][j][1];
                            if (Figures[i][j][1] < minY1) minY1 = Figures[i][j][1];
                            if (Figures[i][j][1] < minY2 && Figures[i][j][1] >= Rows / 2) minY2 = Figures[i][j][1];
                        }
                        width = maxX1 - minX1 + 1;
                        height = (maxY1 - minY1) + (maxY2 - minY2) + 2;
                        simmetricFigure = new bool[width, height];
                        for (int j = 0; j < width; j++)
                        {
                            for (int s = 0; s < height; s++)
                            {
                                if (s < maxY2 - minY2 + 1)
                                    simmetricFigure[j, s] = Cells[minX1 + j, minY2 + s].IsAlive;
                                else simmetricFigure[j, s] = Cells[minX1 + j, minY1 + s - (maxY2 - minY2 + 1)].IsAlive;
                            }
                        }
                    }
                    else if (maxX1 - minX1 == Columns - 1 && maxY1 - minY1 != Rows - 1)
                    {
                        minX1 = Columns;
                        minX2 = Columns;
                        maxX1 = -1;
                        maxX2 = -1;
                        for (int j = 0; j < Figures[i].Count(); j++)
                        {
                            if (Figures[i][j][0] > maxX1 && Figures[i][j][0] < Columns / 2) maxX1 = Figures[i][j][0];
                            if (Figures[i][j][0] > maxX2) maxX2 = Figures[i][j][0];
                            if (Figures[i][j][0] < minX1) minX1 = Figures[i][j][0];
                            if (Figures[i][j][0] < minX2 && Figures[i][j][0] >= Columns / 2) minX2 = Figures[i][j][0];
                        }
                        width = (maxX1 - minX1) + (maxX2 - minX2) + 2;
                        height = maxY1 - minY1 + 1;
                        simmetricFigure = new bool[width, height];
                        for (int j = 0; j < width; j++)
                        {
                            for (int s = 0; s < height; s++)
                            {
                                if (j < maxX2 - minX2 + 1)
                                    simmetricFigure[j, s] = Cells[minX2 + j, minY1 + s].IsAlive;
                                else simmetricFigure[j, s] = Cells[minX1 + j - (maxX2 - minX2 + 1), minY1 + s].IsAlive;
                            }
                        }
                    }
                }
                else
                {
                    width = maxX1 - minX1 + 1;
                    height = maxY1 - minY1 + 1;
                    simmetricFigure = new bool[width, height];
                    for (int j = 0; j < width; j++)
                    {
                        for (int s = 0; s < height; s++)
                        {
                            simmetricFigure[j, s] = Cells[minX1 + j, minY1 + s].IsAlive;
                        }
                    }
                }
                for (int j = 0; j < width; j++)
                {
                    for (int s = 0; s < height; s++)
                    {
                        if (simmetricFigure[j, s] != simmetricFigure[j, height - s - 1])
                            simmX = false;
                    }
                }
                for (int s = 0; s < height; s++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (simmetricFigure[j, s] != simmetricFigure[j, height - s - 1])
                            simmY = false;
                    }
                }
                if (width == 2 && height == 2)
                {

                    if (simmetricFigure[0, 0] && simmetricFigure[0, 1] && simmetricFigure[1, 0] && simmetricFigure[1, 1]) Block++;
                }
                else if (width == 3 && height == 3)
                {
                    if (!simmetricFigure[0, 0] && simmetricFigure[1, 0] && !simmetricFigure[2, 0]
                        && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && simmetricFigure[2, 1]
                        && !simmetricFigure[0, 2] && simmetricFigure[1, 2] && !simmetricFigure[2, 2])
                    {
                        Box++;
                    }
                    else if ((!simmetricFigure[0, 0] && simmetricFigure[1, 0] && !simmetricFigure[2, 0]
                      && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && simmetricFigure[2, 1]
                      && !simmetricFigure[0, 2] && simmetricFigure[1, 2] && simmetricFigure[2, 2])
                      || (!simmetricFigure[0, 0] && simmetricFigure[1, 0] && simmetricFigure[2, 0]
                      && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && simmetricFigure[2, 1]
                      && !simmetricFigure[0, 2] && simmetricFigure[1, 2] && !simmetricFigure[2, 2])
                      || (simmetricFigure[0, 0] && simmetricFigure[1, 0] && !simmetricFigure[2, 0]
                      && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && simmetricFigure[2, 1]
                      && !simmetricFigure[0, 2] && simmetricFigure[1, 2] && simmetricFigure[2, 2])
                      || (!simmetricFigure[0, 0] && simmetricFigure[1, 0] && !simmetricFigure[2, 0]
                      && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && simmetricFigure[2, 1]
                      && simmetricFigure[0, 2] && simmetricFigure[1, 2] && simmetricFigure[2, 2]))
                    {
                        Boat++;
                    }
                    else if ((simmetricFigure[0, 0] && simmetricFigure[1, 0] && !simmetricFigure[2, 0]
                      && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && simmetricFigure[2, 1]
                      && !simmetricFigure[0, 2] && simmetricFigure[1, 2] && simmetricFigure[2, 2])
                      ||
                      (!simmetricFigure[0, 0] && simmetricFigure[1, 0] && simmetricFigure[2, 0]
                      && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && simmetricFigure[2, 1]
                      && simmetricFigure[0, 2] && simmetricFigure[1, 2] && !simmetricFigure[2, 2]))
                    {
                        Ship++;
                    }
                }
                else if (width == 3 && height == 4)
                {
                    if (!simmetricFigure[0, 0] && simmetricFigure[1, 0] && !simmetricFigure[2, 0]
                      && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && simmetricFigure[2, 1]
                      && simmetricFigure[0, 2] && !simmetricFigure[1, 2] && simmetricFigure[2, 2]
                      && !simmetricFigure[0, 3] && simmetricFigure[1, 3] && !simmetricFigure[2, 3])
                        Hive++;
                }
                else if (width == 4 && height == 3)
                {
                    if (!simmetricFigure[0, 0] && simmetricFigure[1, 0] && simmetricFigure[2, 0] && !simmetricFigure[3, 0]
                     && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && !simmetricFigure[2, 1] && simmetricFigure[3, 1]
                     && !simmetricFigure[0, 2] && simmetricFigure[1, 2] && simmetricFigure[2, 2] && !simmetricFigure[3, 2])
                        Hive++;
                }
                else if (width == 4 && height == 4)
                {
                    if (!simmetricFigure[0, 0] && simmetricFigure[1, 0] && simmetricFigure[2, 0] && !simmetricFigure[3, 0]
                     && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && !simmetricFigure[2, 1] && simmetricFigure[3, 1]
                     && simmetricFigure[0, 2] && !simmetricFigure[1, 2] && !simmetricFigure[2, 2] && simmetricFigure[3, 2]
                     && !simmetricFigure[0, 3] && simmetricFigure[1, 3] && simmetricFigure[2, 3] && !simmetricFigure[3, 3])
                        Pond++;
                    else if
                        (!simmetricFigure[0, 0] && simmetricFigure[1, 0] && !simmetricFigure[2, 0] && !simmetricFigure[3, 0]
                     && simmetricFigure[0, 1] && !simmetricFigure[1, 1] && simmetricFigure[2, 1] && !simmetricFigure[3, 1]
                     && !simmetricFigure[0, 2] && simmetricFigure[1, 2] && !simmetricFigure[2, 2] && simmetricFigure[3, 2]
                     && !simmetricFigure[0, 3] && !simmetricFigure[1, 3] && simmetricFigure[2, 3] && !simmetricFigure[3, 3])
                        Barge++;
                }
                else if (width == 3 && height == 1)
                {
                    Flasher++;
                }
                else if (width == 1 && height == 3)
                {
                    Flasher++;
                }
                if (simmX || simmY) k++;
            }
            return k;
        }
        public void CalculateFigures()
        {
            Stable = 0;
            Periodic = 0;
            Moving = 0;
            bool infigure = false;
            bool havestable = false;
            bool havenotstable = false;
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (Cells[x, y].LiveCount > 5)
                    {
                        Cells[x, y].Stable = true;
                        Cells[x, y].NotStable = false;
                        Cells[x, y].Dead = false;
                    }
                    else if (Cells[x, y].DeadCount < 5)
                    {
                        Cells[x, y].Stable = false;
                        Cells[x, y].NotStable = true;
                        Cells[x, y].Dead = false;
                    }
                    else
                    {
                        Cells[x, y].Stable = false;
                        Cells[x, y].NotStable = false;
                        Cells[x, y].Dead = true;
                    }
                    Cells[x, y].InFigure = false;
                }
            }
            int coordinate = 0;
            Figures.Clear();
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    infigure = false;
                    havestable = false;
                    havenotstable = false;
                    for (int i = 0; i < 8; i++)
                    {
                        if (Cells[x, y].neighbors[i].InFigure && Cells[x, y].neighbors[i].IsAlive)
                        {
                            infigure = true;
                            coordinate = FindFigure(Cells[x, y].neighbors[i].X, Cells[x, y].neighbors[i].Y);
                        }
                        if (Cells[x, y].neighbors[i].Stable && !Cells[x, y].neighbors[i].Dead)
                            havestable = true;
                        if (Cells[x, y].neighbors[i].NotStable && !Cells[x, y].neighbors[i].Dead)
                            havenotstable = true;
                    }
                    if (!infigure)
                    {
                        if (Cells[x, y].IsAlive)
                        {
                            Cells[x, y].InFigure = true;
                            Figures.Add(new List<int[]>());
                            Figures[Figures.Count() - 1].Add(new int[2] { x, y });
                            if (Cells[x, y].Stable && !havenotstable) Stable++;
                            else if ((Cells[x, y].Stable && havenotstable) || (Cells[x, y].NotStable && havestable)) Periodic++;
                            else if (Cells[x, y].NotStable && !havestable) Moving++;
                        }
                    }
                    else
                    {
                        if (Cells[x, y].IsAlive)
                            Figures[coordinate].Add(new int[2] { x, y });
                        Cells[x, y].InFigure = true;
                    }
                }
            }
            SumFigures();
            while (Periodic + Stable + Moving > Figures.Count())
            {
                Periodic--;
            }
            if (Periodic < 0) Periodic = 0;
        }
        public bool[,] copy()
        {
            bool[,] state = new bool[Columns, Rows];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    state[x, y] = Cells[x, y].IsAlive;
            return state;
        }
        public bool equals(bool[,] board1, bool[,] board2)
        {
            bool equal = true;
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    if (board1[x, y] != board2[x, y]) equal = false;
            return equal;
        }

        public int stepstostablefile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    Console.WriteLine(steps);
                    return steps;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int stepstostablestring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    Console.WriteLine(steps);
                    return steps;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int stablefile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    Console.WriteLine(board1.Stable);
                    return board1.Stable;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int stablestring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    Console.WriteLine(board1.Stable);
                    return board1.Stable;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int periodicfile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    Console.WriteLine(board1.Periodic);
                    return board1.Periodic;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int periodicstring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    Console.WriteLine(board1.Periodic);
                    return board1.Periodic;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int simmetricfile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    Console.WriteLine(board1.SimmetricFigures());
                    return board1.SimmetricFigures();
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int simmetricstring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    Console.WriteLine(board1.SimmetricFigures());
                    return board1.SimmetricFigures();
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Blockfile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();

                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Block);
                    return board1.Block;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Blockstring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Block);
                    return board1.Block;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Bargefile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Barge);
                    return board1.Barge;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Bargestring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Barge);
                    return board1.Barge;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Hivefile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Hive);
                    return board1.Hive;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Hivestring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Hive);
                    return board1.Hive;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Boxfile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Box);
                    return board1.Box;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Boxstring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Box);
                    return board1.Box;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Boatfile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Boat);
                    return board1.Boat;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Boatstring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Boat);
                    return board1.Boat;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Shipfile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Ship);
                    return board1.Ship;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Shipstring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Ship);
                    return board1.Ship;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Pondfile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Pond);
                    return board1.Pond;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Pondstring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Pond);
                    return board1.Pond;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Flasherfile(string s)
        {
            Board board1 = new Board();
            int steps = -1;
            using (StreamReader r = new StreamReader(s))
            {
                var json = r.ReadToEnd();
                string[] words = json.Split(new char[] { '"' });
                words[6] = words[6].Substring(1, words[6].Length - 2);
                words[8] = words[8].Substring(1, words[8].Length - 2);
                words[10] = words[10].Substring(1, words[10].Length - 2);
                board1 = new Board(
                    width: Int32.Parse(words[6]),
                    height: Int32.Parse(words[8]),
                    cellSize: Int32.Parse(words[10]),
                    CellStates: words[13]);
            }
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Flasher);
                    return board1.Flasher;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        public int Flasherstring(string s)
        {
            int steps = -1;
            string[] words = s.Split(new char[] { '"' });
            words[6] = words[6].Substring(1, words[6].Length - 2);
            words[8] = words[8].Substring(1, words[8].Length - 2);
            words[10] = words[10].Substring(1, words[10].Length - 2);
            Board board1 = new Board(
                                        width: Int32.Parse(words[6]),
                                        height: Int32.Parse(words[8]),
                                        cellSize: Int32.Parse(words[10]),
                                        CellStates: words[13]);
            int k = 0;
            bool[,] bufferboard0 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard1 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard2 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard3 = new bool[board1.Columns, board1.Rows];
            bool[,] bufferboard4 = new bool[board1.Columns, board1.Rows];
            while (true)
            {
                k++;
                board1.Advance();
                if (steps != -1)
                {
                    board1.CalculateFigures();
                    board1.SimmetricFigures();
                    Console.WriteLine(board1.Flasher);
                    return board1.Flasher;
                }
                if (steps == -1)
                {
                    if (k % 5 == 0)
                    {
                        bufferboard0 = board1.copy();
                    }
                    else if (k % 5 == 1)
                    {
                        bufferboard1 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard1))
                        {
                            steps = k - 1;
                        }
                    }
                    else if (k % 5 == 2)
                    {
                        bufferboard2 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard2))
                        {
                            steps = k - 2;
                        }
                    }
                    else if (k % 5 == 3)
                    {
                        bufferboard3 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard3))
                        {
                            steps = k - 3;
                        }
                    }
                    else if (k % 5 == 4)
                    {
                        bufferboard4 = board1.copy();
                        if (board1.equals(bufferboard0, bufferboard4))
                        {
                            steps = k - 4;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
    class Program
    {
        static Board board;
        static private void Reset()
        {
            board = new Board(
                width: 100,
                height: 40,
                cellSize: 1,
                liveDensity: 0.5);
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
        static string ReadWay()
        {
            string way = Console.ReadLine();
            return way;
        }
        static void KeyGet()
        {
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
            }
            while (key.Key != ConsoleKey.Enter);
        }
        static async Task Main(string[] args)
        {
            /* board = new Board(50, 20);
             int steps = -1;
             Console.WriteLine("Хотите загрузить данные из файла? 1 - Да, 0 - Нет");
             int choice = 0;
             while (choice != 0 && choice != 1)
             {
                 choice = int.Parse(Console.ReadLine());
             }
             if (choice == 1)
             {
                 Console.WriteLine("Напишите имя файла: ");
                 using (StreamReader r = new StreamReader("user.json"))
                 {
                     var json = r.ReadToEnd();
                     string[] words = json.Split(new char[] { '"' });
                     words[6] = words[6].Substring(1, words[6].Length - 2);
                     words[8] = words[8].Substring(1, words[8].Length - 2);
                     words[10] = words[10].Substring(1, words[10].Length - 2);
                     board = new Board(
                         width: Int32.Parse(words[6]),
                         height: Int32.Parse(words[8]),
                         cellSize: 1,
                         CellStates: words[13]);
                 }
             }
             else Reset();
             new Thread(async () =>
             {
                 while (true)
                 {
                     KeyGet();
                     using (FileStream fs = new FileStream("test4.json", FileMode.OpenOrCreate))
                     {
                         await JsonSerializer.SerializeAsync<Board>(fs, board);
                         Console.WriteLine("Данные были сохранены");
                     }
                 }
             }).Start();
             int k = 0;
             bool[,] bufferboard0 = new bool[board.Columns, board.Rows];
             bool[,] bufferboard1 = new bool[board.Columns, board.Rows];
             bool[,] bufferboard2 = new bool[board.Columns, board.Rows];
             bool[,] bufferboard3 = new bool[board.Columns, board.Rows];
             bool[,] bufferboard4 = new bool[board.Columns, board.Rows];
             while (true)
             {
                 k++;
                 board.CalculateFigures();
                 Console.Clear();
                 Console.WriteLine("Чтобы сохранить состояние, нажмите Enter");
                 Render();
                 board.Advance();
                 board.SaveCellsStates();
                 if (steps != -1)
                     Console.WriteLine("Переход в стабильную фазу произошёл через " + steps + " шагов");
                 if (k >= 100 && steps == -1)
                 {
                     if (k % 5 == 0)
                     {
                         bufferboard0 = board.copy();
                     }
                     else if (k % 5 == 1)
                     {
                         bufferboard1 = board.copy();
                         if (board.equals(bufferboard0, bufferboard1))
                         {
                             steps = k - 1;
                         }
                     }
                     else if (k % 5 == 2)
                     {
                         bufferboard2 = board.copy();
                         if (board.equals(bufferboard0, bufferboard2))
                         {
                             steps = k - 2;
                         }
                     }
                     else if (k % 5 == 3)
                     {
                         bufferboard3 = board.copy();
                         if (board.equals(bufferboard0, bufferboard3))
                         {
                             steps = k - 3;
                         }
                     }
                     else if (k % 5 == 4)
                     {
                         bufferboard4 = board.copy();
                         if (board.equals(bufferboard0, bufferboard4))
                         {
                             steps = k - 4;
                         }
                     }
                 }
                 if (k > 10)
                 {
                     Console.WriteLine("Устойчивых: " + board.Stable);
                     Console.WriteLine("Периодических: " + board.Periodic);
                     Console.WriteLine("Двигающихся: " + board.Moving);
                     Console.WriteLine("Симметричных: " + board.SimmetricFigures());
                     //board.DisplayFigures();
                     Console.WriteLine("Барж:" + board.Barge);
                     Console.WriteLine("Ульев:" + board.Hive);
                     Console.WriteLine("Блоков:" + board.Block);
                     Console.WriteLine("Ящиков:" + board.Box);
                     Console.WriteLine("Лодок:" + board.Boat);
                     Console.WriteLine("Кораблей:" + board.Ship);
                     Console.WriteLine("Прудов:" + board.Pond);
                     Console.WriteLine("Мигалок:" + board.Flasher);
                     Thread.Sleep(50);
                 }
                 Console.WriteLine(k);
                 Thread.Sleep(1);
             }*/
            board = new Board();
            board.stepstostablefile("test4.json");
            board.stablefile("test4.json");
            board.periodicfile("test4.json");
            board.simmetricfile("test4.json");
            board.Bargefile("test4.json");
            board.Hivefile("test4.json");
            board.Blockfile("test4.json");
            board.Boxfile("test4.json");
            board.Boatfile("test4.json");
            board.Shipfile("test4.json");
            board.Pondfile("test4.json");
            board.Flasherfile("test4.json");
        }
    }
}