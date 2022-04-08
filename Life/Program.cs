using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

namespace cli_life
{
    public class Cell {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState() {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance() {
            IsAlive = IsAliveNext;
        }
    }


    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        public int liveCellsCounter;
        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        [JsonConstructor]

        public Board(int width, int height, int cellSize, double liveDensity = .1) {
            CellSize = cellSize;

            Cells = new Cell[width / CellSize, height / CellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }
        public Board(string boardinstring) {
            CellSize = 1;

            string[] boardinmatrix = boardinstring.Split('\n');
            int textColumns = boardinmatrix[0].Length;
            int textRows = 1 + (boardinstring.Length - textColumns) / textColumns;
            Cells = new Cell[textColumns, textRows];

            Console.WriteLine(Cells.GetLength(0));
            Console.WriteLine(Cells.GetLength(1));
            for (int i = 0; i < textRows; i++) {
                for (int j = 0; j < textColumns; j++) {
                    Cells[j, i] = new Cell();
                    if (boardinmatrix[i][j] == '*') {
                        Cells[j, i].IsAlive = true;
                    } else {
                        Cells[j, i].IsAlive = false;
                    }
                }
            }
            ConnectNeighbors();
        }
        readonly Random rand = new Random();
        public void Randomize(double liveDensity) {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance() {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors() {
            for (int x = 0; x < Columns; x++) {
                for (int y = 0; y < Rows; y++) {
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
        public void lifecells() {
            int lcells = 0;
            int acells = 0;
            for (int i = 0; i < Rows; i++) {
                for (int j = 0; j < Columns; j++) {
                    acells += 1;
                    if (Cells[j, i].IsAlive) {
                        lcells += 1;
                    }
                }
            }
            Console.WriteLine("____________________________");
            Console.WriteLine("Всего клеток: " + acells);
            Console.WriteLine("Живых клеток: " + lcells);
            Console.WriteLine("Мертвых клеток: " + (acells - lcells));

            if (xsimmetriya()) {
                Console.WriteLine("Симетрично по Ох");
            } else {
                Console.WriteLine("Не симметрично по Ох");
            }
            if(ysimmetriya()) {
                Console.WriteLine("Симметрично по Oy");
            } else
            {
                Console.WriteLine("Не симметрично по Oy");
            }
            Console.WriteLine("____________________________");
            Console.WriteLine("Количество блоков: " + (block()));
            Console.WriteLine("Количество ящиков: " + (tub()));
            Console.WriteLine("Количество лодок: " + (boat()));
            Console.WriteLine("Количество кораблей: " + (ship()));
            Console.WriteLine("Количество мигалок: " + (migalka()));
            Console.WriteLine("____________________________");
            Console.WriteLine("Устойчивых фигур: " + (block()+ship()+tub()+boat()));
        }
        public string intextforfile() {
            string result = "";
            for (int i = 0; i < Rows; i++) {
                for (int j = 0; j < Columns; j++) {
                    var cell = Cells[j, i];
                    if (cell.IsAlive)
                        result += "*";
                    else
                        result += " ";
                }
                result += "\n";
            }
            return result;
        }
        public bool xsimmetriya() {
            bool flag = true;
           
            for (int row = 0; row < Rows / 2; row++) {
                for (int col = 0; col < Columns; col++) {
                    if (Cells[col, row].IsAlive != Cells[col, (Rows - row - 1)].IsAlive) {
                        flag = false;
                    }
                }
            }
            return flag;
        }
        public bool ysimmetriya() {
            bool flag = true;
            for (int row = 0; row < Rows; row++){
                for (int col = 0; col < Columns / 2; col++) {
                    if (Cells[col, row].IsAlive != Cells[(Columns - col - 1), row].IsAlive) {
                        flag = false;
                    }
                }
            }
            return flag;
        }
        public int block() {
            int count = 0;
            for (int row = 1; row < Rows - 2; row++) {
                for (int col = 1; col < Columns - 2; col++) {
                    if (Cells[col, row].IsAlive && Cells[col + 1, row].IsAlive && Cells[col + 1, row + 1].IsAlive && Cells[col, row + 1].IsAlive
                    && !Cells[col - 1, row - 1].IsAlive && !Cells[col - 1, row].IsAlive && !Cells[col - 1, row + 1].IsAlive 
                    && !Cells[col - 1, row + 2].IsAlive && !Cells[col, row - 1].IsAlive && !Cells[col, row + 2].IsAlive
                    && !Cells[col + 1, row - 1].IsAlive && !Cells[col + 1, row + 2].IsAlive && !Cells[col + 2, row - 1].IsAlive
                    && !Cells[col + 2, row].IsAlive && !Cells[col + 2, row + 1].IsAlive && !Cells[col + 2, row + 2].IsAlive) {
                        count++;
                    }
                }
            }
            return count;
        }
        public int tub() {
            int count = 0;
            for (int row = 1; row < Rows - 3; row++) {
                for (int col = 2; col < Columns - 2; col++) {
                    if (Cells[col, row].IsAlive && Cells[col - 1, row + 1].IsAlive && Cells[col + 1, row + 1].IsAlive && Cells[col, row + 2].IsAlive
                    && !Cells[col - 2, row].IsAlive && !Cells[col - 2, row + 1].IsAlive && !Cells[col - 2, row + 2].IsAlive
                    && !Cells[col - 1, row - 1].IsAlive && !Cells[col - 1, row].IsAlive && !Cells[col - 1, row + 2].IsAlive
                    && !Cells[col - 1, row + 3].IsAlive && !Cells[col, row - 1].IsAlive && !Cells[col, row + 1].IsAlive
                    && !Cells[col, row + 3].IsAlive && !Cells[col + 1, row - 1].IsAlive && !Cells[col + 1, row].IsAlive 
                    && !Cells[col + 1, row + 2].IsAlive && !Cells[col + 1, row + 3].IsAlive && !Cells[col + 2, row].IsAlive 
                    && !Cells[col + 2, row + 1].IsAlive && !Cells[col + 2, row + 2].IsAlive) {
                        count++;
                    }
                }
            }
            return count;
        }
        public int migalka() {
            int count = 0;
            for (int row = 1; row < Rows - 3; row++) {
                for (int col = 1; col < Columns - 2; col++) {
                    if (Cells[col, row].IsAlive && Cells[col, row + 1].IsAlive && Cells[col, row + 2].IsAlive 
                    && !Cells[col - 1, row - 1].IsAlive && !Cells[col - 1, row].IsAlive && !Cells[col - 1, row +1].IsAlive 
                    && !Cells[col - 1, row + 3].IsAlive  && !Cells[col - 1, row + 2].IsAlive && !Cells[col, row - 1].IsAlive 
                    && !Cells[col, row + 3].IsAlive && !Cells[col + 1, row - 1].IsAlive && !Cells[col + 1, row].IsAlive 
                    && !Cells[col + 1, row + 1].IsAlive && !Cells[col + 1, row + 3].IsAlive && !Cells[col + 1, row + 2].IsAlive) {
                        count++;
                    }
                }
            }
            for (int row = 1; row < Rows - 2; row++) {
                for (int col = 1; col < Columns - 3; col++) {
                    if (Cells[col, row].IsAlive && Cells[col + 1, row].IsAlive && Cells[col + 2, row].IsAlive
                    && !Cells[col - 1, row - 1].IsAlive && !Cells[col - 1, row].IsAlive && !Cells[col - 1, row + 1].IsAlive
                    && !Cells[col, row - 1].IsAlive && !Cells[col, row + 1].IsAlive && !Cells[col + 1, row - 1].IsAlive 
                    && !Cells[col + 1, row + 1].IsAlive && !Cells[col + 2, row - 1].IsAlive && !Cells[col + 2, row + 1].IsAlive
                    && !Cells[col + 3, row - 1].IsAlive && !Cells[col + 3, row].IsAlive && !Cells[col + 3, row + 1].IsAlive) {
                        count++;
                    }
                }
            }
            return count;
        }
        public int boat() {
            int count = 0;
            for (int row = 1; row < Rows - 3; row++) {
                for (int col = 2; col < Columns - 2; col++) {
                    if (Cells[col, row].IsAlive && Cells[col - 1, row + 1].IsAlive && Cells[col + 1, row + 1].IsAlive && Cells[col, row + 2].IsAlive
                    && !Cells[col - 2, row].IsAlive && !Cells[col - 2, row + 1].IsAlive && !Cells[col - 2, row + 2].IsAlive && !Cells[col - 2, row - 1].IsAlive
                    && !Cells[col - 1, row - 1].IsAlive && Cells[col - 1, row].IsAlive && !Cells[col - 1, row + 2].IsAlive
                    && !Cells[col - 1, row + 3].IsAlive && !Cells[col, row - 1].IsAlive && !Cells[col, row + 1].IsAlive
                    && !Cells[col, row + 3].IsAlive && !Cells[col + 1, row - 1].IsAlive && !Cells[col + 1, row].IsAlive
                    && !Cells[col + 1, row + 2].IsAlive && !Cells[col + 1, row + 3].IsAlive && !Cells[col + 2, row].IsAlive
                    && !Cells[col + 2, row + 1].IsAlive && !Cells[col + 2, row + 2].IsAlive) {
                        count++;
                    }
                }
            }
            return count;
        }
        public int ship() {
            int count = 0;
            for (int row = 1; row < Rows - 3; row++) {
                for (int col = 2; col < Columns - 2; col++) {
                    if (Cells[col, row].IsAlive && Cells[col - 1, row + 1].IsAlive && Cells[col + 1, row + 1].IsAlive && Cells[col, row + 2].IsAlive
                    && !Cells[col - 2, row].IsAlive && !Cells[col - 2, row + 1].IsAlive && !Cells[col - 2, row + 2].IsAlive && !Cells[col - 2, row - 1].IsAlive
                    && !Cells[col - 1, row - 1].IsAlive && Cells[col - 1, row].IsAlive && !Cells[col - 1, row + 2].IsAlive
                    && !Cells[col - 1, row + 3].IsAlive && !Cells[col, row - 1].IsAlive && !Cells[col, row + 1].IsAlive
                    && !Cells[col, row + 3].IsAlive && !Cells[col + 1, row - 1].IsAlive && !Cells[col + 1, row].IsAlive
                    && Cells[col + 1, row + 2].IsAlive && !Cells[col + 1, row + 3].IsAlive && !Cells[col + 2, row].IsAlive
                    && !Cells[col + 2, row + 1].IsAlive && !Cells[col + 2, row + 2].IsAlive && !Cells[col + 2, row + 3].IsAlive) {
                        count++;
                    }
                }
            }
            return count;
        }
    }
    public class infile {
        public static void writef(Board board, string filename) {
            var filen = new StreamWriter(filename);
            filen.Write(board.intextforfile());
            filen.Close();
        }
        public static Board readf(string filename) {
            string textBoard = File.ReadAllText(filename);
            var board = new Board(textBoard);
            return board;
        }
    }
    class Program {
        static Board board;
        static bool save = false;
        static private void Reset() {
            if (File.Exists("setting.json")) {
               board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("setting.json"));
            } else {
               board = new Board (
                 width: 50,
                 height: 20,
                 cellSize: 1,
                 liveDensity: 0.5);
            }
        }
        static void Render() {
            for (int i = 0; i < board.Rows; i++) {
                for (int j = 0; j < board.Columns; j++) {
                    var cell = board.Cells[j, i];
                    if (cell.IsAlive) {
                        Console.Write('*');
                    } else {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
       
        static void Main(string[] args) {
         
            string fileName = "";
            Console.WriteLine("Нажми F для загрузки сохранения\nНажми что-нибудь другое для новой колонии!");
            
            ConsoleKeyInfo k = Console.ReadKey();
            if (k.Key == ConsoleKey.F) {
                Console.Clear();
                Console.WriteLine("Введи имя файла с сохранением");
                fileName = Console.ReadLine();
                board = infile.readf(fileName);
            } else {
                Reset();
            }
            var waitKeySave = new Thread(()=>{
                while (true) {
                    ConsoleKeyInfo k = Console.ReadKey();
                    if (k.Key == ConsoleKey.S)
                        save = true;
                }
            });
            waitKeySave.Start();
            while (true) {
                Console.Clear();
                Console.WriteLine("Нажми S для сохранения!");
                Render();
                board.lifecells();
                if (save) {
                    Console.Clear();
                    Console.WriteLine("Введи имя файла для сохранения");
                    fileName = Console.ReadLine();
                    
                    infile.writef(board, fileName);
                   
                    System.Environment.Exit(0);
                }
               
                board.Advance();
                Thread.Sleep(1500);
            }
        }
    }
}