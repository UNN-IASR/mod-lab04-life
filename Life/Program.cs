using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Numerics;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using ScottPlot.Palettes;
using System.Xml.Linq;
using System.Reflection.Metadata;
using System.Net;
namespace Client
{
    public class Templates
    {
        public List<Prefab> prefabs;
        public Templates(out string str)
        {
            prefabs = new List<Prefab>();
            string[] namePrefab = File.ReadAllLines("NameFigure.txt");
            foreach (string name in namePrefab)
            {
                string[] prefab = File.ReadAllLines(name + ".txt");
                int[,] matrix = GetMatrix(prefab);
                prefabs.Add(new Prefab(prefab[0].Length, prefab.Length, matrix, name));
            }
            str = "Выполнено";
        }
        public Templates()
        {
            prefabs = new List<Prefab>();
            string[] nameFigures = File.ReadAllLines("NameFigure.txt");
            foreach (string name in nameFigures)
            {
                string[] prefab = File.ReadAllLines(name + ".txt");
                int[,] matrix = GetMatrix(prefab);
                prefabs.Add(new Prefab(prefab[0].Length, prefab.Length, matrix, name));
            }
        }
        public void Recognition(Board _board)
        {
            foreach(var prefab in prefabs)
            {
                Console.WriteLine(prefab._name + " " + prefab.CheckPrefab(_board));
            }
        }
        public int[,] GetMatrix(string[] prefab)
        {
            int[,] matrix = new int[prefab[0].Length, prefab.Length];
            for(int i=0;i<prefab.Length;i++)
            {
                for(int j = 0; j < prefab[0].Length;j++)
                {
                    
                    matrix[i,j]=(int)prefab[i][j]-48;
                }
            }
            return matrix;
        }
    }
    public class Prefab
    {
        public string _name="";
        public int _width=0;
        public List<int> buf_Matrix=new();
        public int _height=0;
        public int[,] _matrix;
        public Prefab(int Width, int Height, int[,] martix,string name) 
        {
            _width=Width;
            _height=Height;
            _matrix= martix;
            _name=name;
        }
        public int CheckPrefab(Board board)
        {
            int CreatingCoeff = 0;
            int count = 0;
            int[,] MTRX = new int[_width,_height];
            buf_Matrix.Clear();
            for (int Rows = 0; Rows < board.Rows; Rows++)
            {
                buf_Matrix.Remove(Rows);
                for (int Cool = 0; Cool < board.Columns; Cool++)
                {
                    buf_Matrix.Remove(Cool);
                    if(buf_Matrix.Count>0)
                    {
                        CreatingCoeff = 0;
                        buf_Matrix.Add(Rows-Cool);
                    }
                    else
                    {
                        CreatingCoeff++;
                    }
                    for (int MartI = 0; MartI < _height; MartI++)
                    {
                        for (int MartJ = 0; MartJ < _width; MartJ++)
                        {
                            int y = Rows + MartI < board.Rows ? Rows + MartI : Rows + MartI - board.Rows;
                            int x = Cool + MartJ < board.Columns ? Cool + MartJ : Cool + MartJ - board.Columns;
                            if (!board.Cells[x, y].IsAlive)
                            {
                                buf_Matrix.Remove(x);
                                MTRX[MartI, MartJ] = 0;
                                CreatingCoeff--;
                            }
                            else
                            {
                                buf_Matrix.Remove(y - x);
                                MTRX[MartI, MartJ] = 1;
                                CreatingCoeff--;
                            }
                                
                        }
                    }
                    count += EquipPrefab(MTRX);
                }
            }
            buf_Matrix.Clear();
            return count;
        }
        int EquipPrefab(int[,] matr1)
        {
            int rows = matr1.GetUpperBound(0) + 1;
            int columns = matr1.Length / rows;
            int result = 1;
            for (int i = 0; i < rows; i++)
            {
                buf_Matrix.Add(i);
                for (int j = 0; j < columns; j++)
                {
                    if (matr1[i, j] != _matrix[i, j])
                    {
                        buf_Matrix.Clear();
                        result = 0;
                    }
                }
            }
            return result;
        }
    }   
    public class Settings
    {
        public double liveDensity { get; set; }
        public int cellSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; } 
    }
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public Cell(bool _isAlive)
        {
            IsAlive=_isAlive;
        }
        public Cell() { }
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
        public readonly int CellSize;

        public Cell[,] Cells;
        public int NumberGenerations;
        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            NumberGenerations = 0;
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
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
        public void Randomize(double liveDensity)
        {
            Random rnd = new Random();
            foreach (var cell in Cells)
                cell.IsAlive = rnd.NextDouble() < liveDensity;
        }        
        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        public void Render()
        {
            int Live = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var cell = Cells[col, row];
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
            foreach (var item in Cells)
            {
                if (item.IsAlive)
                {
                    Live++;
                }
            }
            Console.WriteLine("Живые клетки:" + Live);
            Console.WriteLine("Поколение" + NumberGenerations++);
        }
        public string BoardSave()
        {
            string Setting = Columns.ToString() + " " + Rows.ToString() +" " +NumberGenerations.ToString()+"\n";
            foreach(var item in Cells)
            {
                Setting += item.IsAlive == true ? 1:0;
            }
            File.WriteAllText("OldBoard.txt", Setting);
            return "Сохранено";        
        }
        public bool CheckActive()
        {
            string str = "";
            foreach(var item in Cells)
            {
                str+=item.IsAlive== true ? "1" : "0";
            }
            if(!lostComponation.Contains(str))
            {
                lostComponation.Add(str);
                return true;
            }
            return false;
        }
        public string Discharge()
        {
            string[] Data = File.ReadAllLines("OldBoard.txt");
            string[] setting = Data[0].Split(" ");
            NumberGenerations = int.Parse(setting[2]);
            Cells = new Cell[int.Parse(setting[1]),int.Parse(setting[0])];
            int Colum =0;
            int Row = 0;
            for(int i=0;i<Data[1].Length;i++)
            {
                Cells[Row,Colum]= new Cell(Data[1][i] == '1');
                if(Colum==Columns-1)
                {
                    Colum = -1;
                    Row++;
                }
                Colum++;
            }
            ConnectNeighbors();
            return "Выгружено";
        }
        static public Board Loading()
        {
            string filename = "config.json";
            string jsonString = File.ReadAllText(filename);
            Settings settings = JsonSerializer.Deserialize<Settings>(jsonString);
            return new Board(width: settings.Width, height: settings.Height, cellSize: settings.cellSize, liveDensity: settings.liveDensity);

        }
        public List<string> lostComponation= new List<string>();
        public int CointStabiliti;
    }
    public class CreateGrafic
    {        
        public List<Dictionary<int, int>> NewList(List<double> listDesc, int count)
        {
            int countPoint = 0;
            var list = new List<Dictionary<int, int>>();
            for (int i = 0; i < count; i++)
            {
                countPoint++;
                if(countPoint>0)
                {
                    countPoint = 0;
                }
                list.Add(LiveCreate(listDesc[i]));
            }
            countPoint = count;
            list.Sort((x, y) => x.Count - y.Count);
            return list;
        }
        public Dictionary<int, int> LiveCreate(double density)
        {
            bool flag = true;
            Board board = new Board(30, 30, 1, density);
            var Answer = new Dictionary<int, int>();
            while (flag)
            {
                int count = 0;
                foreach(var cell in board.Cells)
                {
                    if (cell.IsAlive) 
                    { 
                        count++; 
                    }
                }
                Answer.Add(board.lostComponation.Count, count);
                string str = "";
                foreach (var item in board.Cells)
                {
                    if(count==Answer.Count)
                    {
                        board.CointStabiliti++;
                    }
                    str += item.IsAlive == true ? "1" : "0";
                }
                if (!board.lostComponation.Contains(str))
                {
                    board.lostComponation.Add(str);
                    board.CointStabiliti = 0;
                }
                else
                {
                    break;
                }
                board.Advance();
                if(board.CointStabiliti<-10)
                {
                    flag= false;
                }
                else
                {
                    flag = true;
                }

            }
            return Answer;
        }
        public Cell NewCell(Board board)
        {                
            bool live = false;
            if(board.CointStabiliti>0)
            {
                string str = "";
                foreach(var cll in board.Cells)
                {
                    if(cll.IsAlive)
                    {
                        str += "1";
                    }
                    else
                    {
                        str = "0";
                    }
                }            

                if(board.lostComponation.Contains(str))
                {
                    live = true;
                }
            }

            return new Cell(live);
        }
        public CreateGrafic()
        {
            var board = new Board(30, 30, 1);
            var plot = new Plot();
            plot.XLabel("Итерация");
            var cell = NewCell(board);
            plot.YLabel("Живые");
            plot.ShowLegend();
            Random rnd = new Random();
            List<double> density = new List<double>() { 0.3, 0.4, 0.5, 0.6 ,0.7 ,0.8 };
            var list = NewList(density, density.Count);
            int count = 0;
            foreach (var item in list)
            {
                if(cell.IsAlive)
                {
                    
                    if(board.lostComponation.Count>10)
                    {
                        board.NumberGenerations = 0;
                    }
                }
                var drow = plot.Add.Scatter(item.Keys.ToArray(), item.Values.ToArray());
                drow.Label = density[count].ToString();
                drow.Color = new ScottPlot.Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                count++;
            }
            plot.SavePng("plot.png", 1920, 1080);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            CreateGrafic graf = new();
            Board board=Board.Loading();
            Templates _tem = new Templates();
            bool flag = true;
            while (flag)
            {
                Console.Clear();
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.KeyChar == 'q')
                        flag = false;
                    else if (key.KeyChar == 's')
                        board.BoardSave();
                    else if (key.KeyChar == 'u')
                        board.Discharge();
                }
                board.Render();
                _tem.Recognition(board);
                flag = board.CheckActive();
                board.Advance();
                //Thread.Sleep(1000);
            }
        }
    }
}
