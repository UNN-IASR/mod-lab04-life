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
    public readonly double LiveDensity;

    public int Columns { get { return Cells.GetLength(0); } }
    public int Rows { get { return Cells.GetLength(1); } }
    public int Width { get { return Columns * CellSize; } }
    public int Height { get { return Rows * CellSize; } }

    public Board(int width, int height, int cellSize, double liveDensity = .1)
    {
        LiveDensity=liveDensity;
        CellSize = cellSize;
        Cells = new Cell[width / cellSize, height / cellSize];
        for (int x = 0; x < Columns; x++)
            for (int y = 0; y < Rows; y++)
                Cells[x, y] = new Cell();

        ConnectNeighbors();
        Randomize(liveDensity);
    }

    public Board(int width, int height, int cellSize, Cell[,] cells)
    {
        CellSize = cellSize;
        Cells = new Cell[width / cellSize, height / cellSize];
        for (int x = 0; x < Columns; x++)
            for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell();
                    if(cells[x, y].IsAlive)
                        Cells[x, y].IsAlive=true;
                    else Cells[x, y].IsAlive=false;
                }
    }

    public Board Clone()
    {
        Board clone=new Board(Width, Height, CellSize, Cells);
        return clone;
    }

    public void LoadStateSistem(string filePath)
    {
        try
        {
            using StreamReader reader = new StreamReader(filePath);
            for (int row = 0; row < Cells.GetLength(1); row++)
            {
                var line = reader.ReadLine();
                var simbols = line.ToCharArray();
                for (int col = 0; col < Cells.GetLength(0); col++)
                {
                    var cell = Cells[col, row];
                    cell.IsAlive = simbols[col].Equals('*');
                }
            }
        }
        catch (FileNotFoundException )
        {
            throw new FileLoadException($"Не удалось прочитать файл {filePath}");
        }
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
internal class Setting
{
    public class  SettingBoard
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }
}
 public class Program
{
    static Board board;
    static public Board Reset()
    {    
        // board = new Board(
        //    width: 80,
        //    height: 20,
        //    cellSize: 1,
        //    liveDensity: 0.5);
        using (FileStream fs = new FileStream("setting.json", FileMode.Open))
        {
            Setting.SettingBoard s = JsonSerializer.Deserialize<Setting.SettingBoard>(fs);
            board = new Board(
                width: s.Width,
                height: s.Height,
                cellSize: s.CellSize,
                liveDensity: s.LiveDensity);            
        }
        return board;
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
    public static void SaveStateSistem(int genCount)
    {
        
        string fname = "gen-" + genCount.ToString() + ".txt";
        using (StreamWriter writer = File.CreateText(fname))
        {

            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        writer.Write('*');
                    }
                    else
                    {
                        writer.Write(' ');
                    }
                }
                writer.Write("\n");
            }
        }            
    }

    public static int CountAliveCells(Board boardThis)
        {
            int count = 0;

            for (int row = 0; row < boardThis.Rows; row++)
            {
                for (int col = 0; col < boardThis.Columns; col++)
                {
                    var cell = boardThis.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    public static string BuildString(Board boardThis, int col, int row, int n, int m, int a) //a - сколько столбиков назад
        {
            char[] arr = new char[n * m];

            int index = 0;
            for (int j = 0; j < n; j++) //строчка
            {
                for (int i = 0; i < m; i++) //столбик
                {
                    if (board.Cells[col - a + i, row - 1 + j].IsAlive) arr[index] = '*';
                    else arr[index] = '.';
                    index++;
                }
            }
            string s = new string(arr);

            return s;
        }

    public static int[] DetectionElem(Board boardThis)
        {
            int[] countElem = new int[7];

            //4*4
            string block = ".....**..**....."; 
            //4*6
            string snake = ".......*.**..**.*........";

            //5*5
            string box = ".......*...*.*...*......."; 
            string boat = ".......*...*.*...**......"; 
            string ship = "......**...*.*...**......"; 

            //6*5
            string hive = ".......*...*.*..*.*...*.......";

            //5*6
            string hive2 = ".......**...*..*...**........";

            //6*6
            string loaf = "........**...*..*...*.*....*........";
            string pond = "........**...*..*..*..*...**........";

            string s; 

            for (int row = 1; row < boardThis.Rows - 4; row++)
            {
                for (int col = 2; col < boardThis.Columns - 4; col++)
                {
                    var cell = boardThis.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        //4*4
                        s = BuildString(boardThis, col, row, 4, 4, 1);
                        if (s.Equals(block)) countElem[0]++; 

                        //5*5
                        s = BuildString(boardThis, col, row, 5, 5, 2);
                        if (s.Equals(box)) countElem[1]++;
                        if (s.Equals(boat)) countElem[2]++;
                        s = BuildString(boardThis, col, row, 5, 5, 1);
                        if (s.Equals(ship)) countElem[3]++;

                        //6*5
                        s = BuildString(boardThis, col, row, 6, 5, 2);
                        if (s.Equals(hive)) countElem[4]++;

                        //5*6
                        s = BuildString(boardThis, col, row, 5, 6, 2);
                        if (s.Equals(hive2)) countElem[4]++;

                        //4*6
                        s = BuildString(boardThis, col, row, 4, 6, 1);
                        if (s.Equals(snake)) countElem[5]++; 
                        
                        //6*6
                        s = BuildString(boardThis, col, row, 6, 6, 2);
                        if (s.Equals(loaf)) countElem[6]++;
                        if (s.Equals(pond)) countElem[7]++;
                    }
                }
            }
            return countElem;
        }
    
    public static bool StatePhase(List<Board> boards){
            for (int row = 0; row < boards[2].Rows; row++)
            {
                for (int col = 0; col < boards[2].Columns; col++)
                {
                    bool a=boards[0].Cells[col, row].IsAlive;
                    bool b=boards[1].Cells[col, row].IsAlive;
                    bool c=boards[2].Cells[col, row].IsAlive;
                    if (a!=c && b!=c)
                        return false;
                }
            }
            return true;
    }

    static void Main(string[] args)
    {
        List<Board> boards = new List<Board>(3);
        Reset();
        int genCount = 0;
        while (true)
        {   
            boards.Add(board.Clone());
            if(genCount>1){  
                if(StatePhase(boards)){
                    Console.WriteLine(genCount);
                    //break;
                }
                boards.RemoveAt(0);
            }

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo name = Console.ReadKey();
                if (name.KeyChar == 'n')
                    break;
                else {
                    if (name.KeyChar == 's')
                        SaveStateSistem(genCount);
                    if (name.KeyChar == 'l')
                        board.LoadStateSistem("gen-53.txt");
                    if (name.KeyChar == 'p') //проверка на фигуры
                    {
                        int[] numberOfFigures = DetectionElem(board);
                        foreach (int a in numberOfFigures)
                            Console.WriteLine(a.ToString());
                        break;
                    }
                    else if (name.KeyChar == 'c') //количество живых клеток
                    {
                        int countSym = CountAliveCells(board);
                        Console.WriteLine(countSym);
                        break;
                    }
                }
            }
            Console.Clear();
            Render();
            board.Advance();
            Thread.Sleep(5);
            ++genCount;
        }
    }
}
}