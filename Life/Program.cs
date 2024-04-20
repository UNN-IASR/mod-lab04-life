using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using ScottPlot;

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

        //для продолжения запуска проги
        public Board(int width, int height, int cellSize, int[,] gameInPause)
        {
            CellSize = cellSize;
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                    if (gameInPause[x, y] == 1) Cells[x, y].IsAlive = true;
                    else Cells[x, y].IsAlive = false;
                }
            ConnectNeighbors();
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

    public class Setting
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }
    public class FileSetting
    {
        public static void In(string fileName, Board board, Setting setting, int genCount)
        {
            int countLive = 0;
            StreamWriter streamWriter = new StreamWriter(fileName);
            streamWriter.WriteLine($"{setting.Width} {setting.Height} {setting.CellSize} {setting.LiveDensity}");
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    if (board.Cells[j, i].IsAlive)
                    {
                        streamWriter.Write('1');
                        countLive++;
                    }
                    else
                    {
                        streamWriter.Write('0');
                    }
                }
                streamWriter.Write('\n');
            }
            streamWriter.Write("GenCount " + genCount);

            streamWriter.Close();
        }
        public static void Out(string filePath, out Board board, out Setting setting, out int genCount)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string[] settingsArray = streamReader.ReadLine().Split(' ');
            setting = new Setting
            {
                Width = int.Parse(settingsArray[0]),
                Height = int.Parse(settingsArray[1]),
                CellSize = int.Parse(settingsArray[2]),
                LiveDensity = double.Parse(settingsArray[3])
            };

            int rows = setting.Height / setting.CellSize;
            int columns = setting.Width / setting.CellSize;
            
            int[,] gameInPause = new int[columns, rows];
            for (int i = 0; i < rows; i++)
            {
                string line = streamReader.ReadLine();
                for (int j = 0; j < columns; j++)
                {
                    if (line[j] == '1') gameInPause[j, i] = 1;
                    else gameInPause[j, i] = 0;
                    Console.Write(gameInPause[j, i]);
                }
                Console.WriteLine();
            }
            genCount = int.Parse(streamReader.ReadLine().Split(' ')[1]);

            board = new Board(
                width: setting.Width,
                height: setting.Height,
                cellSize: setting.CellSize,
                gameInPause);

        }
    }
    public struct Point
    {
        public int x;
        public int y;
    }

    public class Figure
    {
        int[,] looked;
        public List<Point> comeBackPoint = new List<Point>();
        public int countFigure = 0;
       
        public void CountOfFigure(Board board, ref Dictionary<string, int> dStableFigure)
        {
            looked = new int[board.Columns, board.Rows];

            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    if (board.Cells[col, row].IsAlive == false)
                    {
                        looked[col, row] = 1;
                    }   
                    else if (looked[col, row] == 1)
                    {
                        continue;
                    }  
                    else 
                    {
                        Cell root = board.Cells[col, row];
                        Point point = IsFigure(root, board, row, col);
                        if (point.x == col && point.y == row)
                        {
                            countFigure++;
                            IsStableFigureFromTheSet(ref comeBackPoint, board, ref dStableFigure);
                            comeBackPoint.Clear();
                        }
                           
                    }
                }
            }
        }
        public void IsFigure_InWay(Cell list, Board board, int y, int x)
        {
            if (list.IsAlive && looked[x, y] == 0)
            {
               looked[x, y] = 1;
               IsFigure(list, board, y, x);
            }   
        }
        public void IsFigure_ComeBack(Cell list, Board board, int y, int x)
        {
            Point point = new Point();
            point.x = x;
            point.y = y;
            if (list.IsAlive && !(comeBackPoint.Contains(point)))
            {
                comeBackPoint.Add(point);
                IsFigure(list, board, y, x);
            }  
        }
        public Point IsFigure(Cell list, Board board, int y, int x)
        {
            looked[x, y] = 1;
            int xL = (x > 0) ? x - 1 : board.Columns - 1;
            int xR = (x < board.Columns - 1) ? x + 1 : 0;
            int yT = (y > 0) ? y - 1 : board.Rows - 1;
            int yB = (y < board.Rows - 1) ? y + 1 : 0;

            //Обход по всем соседям
            IsFigure_InWay(board.Cells[xL, yT], board, yT, xL);
            IsFigure_InWay(board.Cells[xL, y], board, y, xL);
            IsFigure_InWay(board.Cells[xL, yB], board, yB, xL);
            IsFigure_InWay(board.Cells[x, yB], board, yB, x);
            IsFigure_InWay(board.Cells[xR, yB], board, yB, xR);
            IsFigure_InWay(board.Cells[xR, y], board, y, xR);
            IsFigure_InWay(board.Cells[xR, yT], board, yT, xR);
            IsFigure_InWay(board.Cells[x, yT], board, yT, x);


            Point point = new Point();
            point.x = x;
            point.y = y;
            if (!comeBackPoint.Contains(point))
                comeBackPoint.Add(point);

            IsFigure_ComeBack(board.Cells[x, yT], board, yT, x);
            IsFigure_ComeBack(board.Cells[xR, yT], board, yT, xR);
            IsFigure_ComeBack(board.Cells[xR, y], board, y, xR);
            IsFigure_ComeBack(board.Cells[xR, yB], board, yB, xR);
            IsFigure_ComeBack(board.Cells[x, yB], board, yB, x);
            IsFigure_ComeBack(board.Cells[xL, yB], board, yB, xL);
            IsFigure_ComeBack(board.Cells[xL, y], board, y, xL);
            IsFigure_ComeBack(board.Cells[xL, yT], board, yT, xL);
            
            return point;
        }

        public void IsStableFigureFromTheSet(ref List<Point> comeBackPoint, Board board, ref Dictionary<string, int> dStableFigure)
        {
            int minimizeBiazX;
            int minimizeBiazY;
            minimizeBiazX = board.Columns;
            minimizeBiazY = board.Rows;

            for (int i = 0; i < comeBackPoint.Count; i++)
            {
                if (comeBackPoint[i].x < minimizeBiazX) minimizeBiazX = comeBackPoint[i].x;
                if (comeBackPoint[i].y < minimizeBiazY) minimizeBiazY = comeBackPoint[i].y;
            }

            for (int i = 0; i < comeBackPoint.Count; i++)
            {
                Point point = new Point();
                point.x = comeBackPoint[i].x - minimizeBiazX;
                point.y = comeBackPoint[i].y - minimizeBiazY;
                comeBackPoint[i] = point;
            }

            List<String> stableFigure = new List<string> 
            {
                "Barge",
                "Block",
                "Boat",
                "Box", 
                "Hive", 
                "Loaf", 
                "Pond", 
                "Ship", 
                "Snake", 
                "Stick"
            };

            foreach (var fig in stableFigure)
            {
                ReadStableFigureInList(fig, comeBackPoint, ref dStableFigure);
            }
        }

        public void ReadStableFigureInList(string nameFigure, List<Point> comeBackPoint, ref Dictionary<string, int> dStableFigure)
        {
            string nameFile = "figure_" + nameFigure + ".txt";
            StreamReader streamReader = new StreamReader(nameFile);
            string str = streamReader.ReadLine();

            int rows = int.Parse(str.Split(' ')[0]);
            int columns = int.Parse(str.Split(' ')[1]);

            int[,] mStableFigure = new int[columns, rows];
            for (int i = 0; i < rows; i++)
            {
                string line = streamReader.ReadLine();
                for (int j = 0; j < columns; j++)
                {
                    if (line[j] == '1') mStableFigure[j, i] = 1;
                    else mStableFigure[j, i] = 0;
                }
            }
            streamReader.Close();

            int maxX = 0;
            int maxY = 0;
            for (int i = 0; i < comeBackPoint.Count; i++)
            {
                if (comeBackPoint[i].x > maxX) maxX = comeBackPoint[i].x;
                if (comeBackPoint[i].y > maxY) maxY = comeBackPoint[i].y;
            }


            int[,] figure = new int[maxX+1, maxY+1];
            for (int i = 0; i < maxY+1; i++)
            {
                for (int j = 0; j < maxX+1; j++)
                {
                    Point point = new Point();
                    point.x = j;
                    point.y = i;
                    if (comeBackPoint.Contains(point))
                    {
                        figure[j, i] = 1;
                    }
                    else figure[j, i] = 0;
                }
            }

            int[,] rotata = mStableFigure;
            for (int i = 0; i < 4; i++)
            {
                rotata = RotateStableFigure(rotata);
                if (IsEquals(figure, rotata))
                {
                    dStableFigure[nameFigure] += 1;
                    break;
                }    
            }
        }
        public bool IsEquals(int[,] figure, int[,] mStableFigure)
        {
            int figCol = figure.GetLength(0);
            int figRow = figure.GetLength(1);
            int stFigCol = mStableFigure.GetLength(0);
            int stFigRow = mStableFigure.GetLength(1);

            if (figCol == stFigCol && figRow == stFigRow)
            {
                for (int i = 0; i < figRow; i++)
                {
                    for (int j = 0; j < figCol; j++)
                    {
                        if (!(figure[j, i] == mStableFigure[j, i]))
                        {
                            return false;
                        } 
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        public int[,] RotateStableFigure(int[,] mStableFigure)
        {
            int row = mStableFigure.GetLength(1);
            int col = mStableFigure.GetLength(0);
            int[,] rotate = new int[row, col];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0, t = col - 1; j < col; j++, --t)
                {
                    rotate[i, j] = mStableFigure[t, i];
                }
            }
            return rotate;
        }
    }

    public class Program
    {
        static Board board;
        static Setting setting;
        Figure figure = new Figure();

        static public void Reset(Setting setting)
        {
            board = new Board(
                setting.Width,
                setting.Height,
                setting.CellSize,
                setting.LiveDensity);
        }
        static int Render(int countLive)
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        countLive++;
                        //Console.Write('*');
                    }
                    else
                    {
                        //Console.Write('-');
                    }
                }
                //Console.Write('\n');
            }
            return countLive;
        }


        static void Main(string[] args)
        {
            int chislo = 500;
            ScottPlot.Plot plot = new Plot();
            double p = 0.3;
            double[] dataX = new double[chislo-2];

            double[] dataY;
           
            while (p < 0.81)
            {
            dataY = new double[chislo-2];
            int genCount = 1;
            string fullPath_Json = Path.GetFullPath("settings.json");
            string fullPath_GamePause = Path.GetFullPath("GamePause.txt");

            if (File.Exists(fullPath_GamePause))
            {
                FileSetting.Out(fullPath_GamePause, out board, out setting, out genCount);
            }
            else
            {
                string settingJson = File.ReadAllText(fullPath_Json);
                setting = JsonConvert.DeserializeObject<Setting>(settingJson);
                Reset(setting);
            }
            
                int count = 0;
                int countLive;

                Dictionary<string, int> dStableFigure = new Dictionary<string, int>()
                {
                { "Barge", 0},
                { "Block", 0 },
                { "Boat", 0 },
                { "Box", 0 },
                { "Hive", 0  },
                { "Loaf", 0  },
                { "Pond" , 0 },
                { "Ship", 0  },
                { "Snake" , 0 },
                { "Stick" , 0 }
                };

                List<int> stablePhase = new List<int>(10);
                while (count < chislo)
                {
                    if (count != 0 && count != 1)
                    {
                        dataX[count - 2] = count;
                    }
                    if (Console.KeyAvailable)
                    {
                        // q - выход из цикла
                        // s - сохранить состояние доски и продолжить работу
                        // w - сохранить и закрыть
                        ConsoleKeyInfo name = Console.ReadKey();
                        if (name.KeyChar == 'q')
                            break;
                        else if (name.KeyChar == 's')
                        {
                            string fname = "gen-" + genCount.ToString() + ".txt";
                            //StreamWriter streamWriter = new StreamWriter(fname + ".txt");
                            FileSetting.In(fname, board, setting, genCount);

                           // streamWriter.Close();
                        }
                        else if (name.KeyChar == 'w')
                        {
                            FileSetting.In("GamePause.txt", board, setting, genCount);
                            break;
                        }
                    }

                    countLive = 0;
                    Console.Clear();
                    countLive = Render(countLive);

                    var v = new Figure();
                    v.CountOfFigure(board, ref dStableFigure);
                    Console.WriteLine("Шаг: " + count);
                    Console.WriteLine("Кол-во фигур: " + v.countFigure);
                    Console.WriteLine("Кол-во живых: " + countLive);
                    foreach (var f in dStableFigure)
                    {
                        Console.WriteLine($"{f.Key}: {f.Value}");
                    }

                    dStableFigure.Clear();
                    dStableFigure = new Dictionary<string, int>()
                {
                { "Barge", 0},
                { "Block", 0 },
                { "Boat", 0 },
                { "Box", 0 },
                { "Hive", 0  },
                { "Loaf", 0  },
                { "Pond" , 0 },
                { "Ship", 0  },
                { "Snake" , 0 },
                { "Stick" , 0 }
                };

                    //Переход в стабильную фазу
                    /*int y = 0;
                    if (stablePhase.Count < 10)
                        stablePhase.Add(countLive);
                    else
                    {
                        for (int i = 0; i < stablePhase.Count - 1; i++)
                        {
                            stablePhase[i] = stablePhase[i + 1];
                        }
                        stablePhase[9] = countLive;

                        for (int i = 1; i < stablePhase.Count; i++)
                        {
                            if (stablePhase[0] == stablePhase[i]) y++;
                        }
                    }
                    if (y == 9)
                    {
                        Console.WriteLine("Stable phase!");
                        Console.WriteLine("Фаза: " + (count - 9));
                        break;
                    }*/

                    if (count != 0 && count != 1)
                    {
                        dataY[count - 2] = countLive;
                    }

                    board.Advance();
                    Thread.Sleep(50);
                    count++;
                    genCount++;
                }

                string str = p.ToString();
                var Skatter = plot.Add.Scatter(dataX, dataY);
                Skatter.Label = str;
                Skatter.LineWidth = 2;
                Skatter.MarkerSize = 0;
                p +=0.05;
            }
            plot.ShowLegend();
            plot.Legend.Orientation = Orientation.Horizontal;
            plot.XLabel("Кол-во поколений");
            plot.YLabel("Кол-во живых клеток");
            plot.SavePng("plot.png", 1800, 1000);
            //Console.ReadKey();
        }
    }
}
