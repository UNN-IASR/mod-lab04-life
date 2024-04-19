using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Text.Encodings.Web;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;

        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(row => row.IsAlive).Count();
            if (IsAlive) IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public int Width { get;}
        public int Height { get;}
        public double LiveDensity { get;}
        private readonly Random rand = new Random();

        [JsonConstructor]
        public Board(int width, int height, double liveDensity = 0.1)
        {
            Width = width;
            Height = height;
            LiveDensity = liveDensity;
            Cells = new Cell[Height, Width];
            for (int row = 0; row < Height; row++)
                for (int col = 0; col < Width; col++)
                    Cells[row, col] = new Cell();
            ConnectNeighbors();
            Randomize(liveDensity);
        }
        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[Height, Width];
            for (int row = 0; row < Height; row++)
                for (int col = 0; col < Width; col++)
                    Cells[row, col] = new Cell();
            ConnectNeighbors();
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
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    int col_left = (col > 0) ? col - 1 : Width - 1;
                    int col_right = (col < Width - 1) ? col + 1 : 0;

                    int row_up = (row > 0) ? row - 1 : Height - 1;
                    int row_down = (row < Height - 1) ? row + 1 : 0;

                    Cells[row, col].neighbors.Add(Cells[row_up, col_left]);
                    Cells[row, col].neighbors.Add(Cells[row, col_left]);
                    Cells[row, col].neighbors.Add(Cells[row_down, col_left]);
                    Cells[row, col].neighbors.Add(Cells[row_up, col]);
                    Cells[row, col].neighbors.Add(Cells[row_down, col]);
                    Cells[row, col].neighbors.Add(Cells[row_up, col_right]);
                    Cells[row, col].neighbors.Add(Cells[row, col_right]);
                    Cells[row, col].neighbors.Add(Cells[row_down, col_right]);
                }
            }
        }
        public int Number_of_alive_cells() {
            int count = 0;
            foreach (var cell in Cells)
                if (cell.IsAlive) count ++;
            return count;
        }
    }

    public class Program
    {
        static private Board Reset()
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            string fileName = "Data\\settings.json";
            string jsonString = File.ReadAllText(fileName);
            Board board = JsonSerializer.Deserialize<Board>(jsonString, options)!;
            return board;
        }
        static void Render(Board board) {
            for (int row = 0; row < board.Height; row++) {
                for (int col = 0; col < board.Width; col++) {
                    var cell = board.Cells[row, col];
                    if (cell.IsAlive) {
                        Console.Write('0');
                    }
                    else {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
        
        static void Save(int genCount, Dictionary<string, int> dict, Board board) {
            string fname = "gen-" + genCount.ToString();
            int i = 0;
            while (File.Exists(fname))
            {
                i++;
                fname = fname + i.ToString();
            }
            StreamWriter writer = new StreamWriter("Data\\" + fname + ".txt",true);
            writer.Write("{0} {1}\n", board.Width, board.Height);
            for (int col = 0; col < board.Width; col++) {
                writer.Write("_");
            }
            writer.Write("\n");
            for (int row = 0; row < board.Height; row++)
            {
                for (int col = 0; col < board.Width; col++)
                {
                    var cell = board.Cells[row, col];
                    if (cell.IsAlive)
                    {
                        writer.Write('0');
                    }
                    else
                    {
                        writer.Write(' ');
                    }
                }
                writer.Write("|");
                writer.Write("\n");
            }
            for (int col = 0; col < board.Width; col++) {
                writer.Write("‾");
            }
            writer.Write("\nGen num = {0}", genCount);
            writer.Write("\nAlive celles: {0}\n", board.Number_of_alive_cells());
            writer.Write("\nWidth = {0}", board.Width);
            writer.Write("\nHeight = {0}", board.Height);
            writer.Write("\nliveDensity = {0}\n", board.LiveDensity);
            foreach (string key in dict.Keys) {
                writer.Write("\n{0} : {1}", key, dict[key]);
            }
            
            writer.Close();
        }

        public static Board Load(string name) {
            StreamReader reader = new StreamReader("Data\\" + name , true);
            string[] s = reader.ReadLine().Split();
            reader.ReadLine();
            int w = Convert.ToInt32(s[0]), h = Convert.ToInt32(s[1]);
            Board board = new Board(w,h);
            for (int row = 0; row < board.Height; row++) {
                string line = reader.ReadLine();
                for (int col = 0; col < board.Width; col++) {
                    if (line[col] == '0') {
                        board.Cells[row, col].IsAlive = true;
                    }
                    else {
                        board.Cells[row, col].IsAlive = false;
                    }
                }
            }
            return board;
        }
        public static int[,] Rotate90(int[,] in_arr) {
            int Row = in_arr.GetLength(1), Col = in_arr.GetLength(0);
            int[,] out_arr = new int[Row, Col];
            for (int i = 0; i < Row; i++) {
                for (int j = 0, t = Col - 1; j < Col; j++, --t) {
                    out_arr[i, j] = in_arr[t, i];
                }
            }
            return out_arr;
        }
        public static Dictionary<string, int> Classify(Board board) {
            var config = new Dictionary<string, int>() {{"Block", 0}, {"Hive", 0}, {"Caravay", 0}, {"Box", 0}, {"Flashing", 0}};
            int[,] mask_block =  {{0,0,0,0},
                                  {0,1,1,0},
                                  {0,1,1,0},
                                  {0,0,0,0}};
            int[,] mask_hive1 =  {{0,0,0,0,0,0},
                                  {0,0,1,1,0,0},
                                  {0,1,0,0,1,0},
                                  {0,0,1,1,0,0},
                                  {0,0,0,0,0,0}};
            int[,] mask_hive2 =  Rotate90(mask_hive1);
            int[,] mask_caravay1 = {{0,0,0,0,0,0},
                                   {0,0,0,1,0,0},
                                   {0,0,1,0,1,0},
                                   {0,1,0,0,1,0},
                                   {0,0,1,1,0,0},
                                   {0,0,0,0,0,0}};
            int[,] mask_caravay2 = Rotate90(mask_caravay1);
            int[,] mask_caravay3 = Rotate90(mask_caravay2);
            int[,] mask_caravay4 = Rotate90(mask_caravay3);
            int[,] mask_box = {{0,0,0,0,0},
                               {0,0,1,0,0},
                               {0,1,0,1,0},
                               {0,0,1,0,0},
                               {0,0,0,0,0}};
            int[,] flashing1 = {{0,0,0},
                                {0,1,0},
                                {0,1,0},
                                {0,1,0},
                                {0,0,0}};
            int[,] flashing2 = Rotate90(flashing1);
            List<int[,]> masks = new List<int[,]>() {mask_block, mask_hive1, mask_hive2, 
                                                    mask_caravay1, mask_caravay2, mask_caravay3, 
                                                    mask_caravay4, mask_box, flashing1, flashing2};

            
            bool flag = true;
            for (int row = 0; row < board.Height; row++)
            {
                for (int col = 0; col < board.Width; col++)
                {
                    
                    for (int num_of_mask = 0; num_of_mask < masks.Count; num_of_mask++) {
                        flag = true;
                        for (int i = row; i < row +  masks[num_of_mask].GetLength(0); i++ ) {
                            if (flag == false) {
                                break;
                            }
                            for (int j = col; j < col + masks[num_of_mask].GetLength(1); j++) {
                                if (masks[num_of_mask].GetLength(0) <= board.Height && masks[num_of_mask].GetLength(1) <= board.Width) {
                                    int k,m;
                                    if (i >= board.Height) {
                                        k = i - board.Height;
                                    }
                                    else k = i;
                                    if (j >= board.Width) {
                                        m = j - board.Width;
                                    }
                                    else m = j;
                                    if (Convert.ToInt32(board.Cells[k, m].IsAlive) != masks[num_of_mask][i - row, j - col]) {
                                        flag = false;
                                        break;
                                    }
                                }
                                else {
                                    flag = false;
                                }
                            }
                        }
                        if (flag == true) {
                            if (num_of_mask == 0) config["Block"]++;
                            if (num_of_mask == 1 || num_of_mask == 2) config["Hive"]++;
                            if (num_of_mask == 3 || num_of_mask == 4 || num_of_mask == 5 || num_of_mask == 6) config["Caravay"]++;
                            if (num_of_mask == 7) config["Box"]++;
                            if (num_of_mask == 8 || num_of_mask == 9) config["Flashing"]++;
                        }
                    }
                        
                }
            }
            return config;
        }
        static void Console_print_board(Board board, int pause) {
            Console.Clear();
            Render(board);
            Thread.Sleep(pause);
        }

        static void Save_diagnostic_inf(Board board, int gen) {
            StreamWriter writer = new StreamWriter("Data\\out.txt", true);
            writer.Write("{0} {1}\n", board.LiveDensity, gen);
            writer.Close();
        }

        static void GO(Board board) {
            Dictionary<int, int> state_dict = new Dictionary<int, int>();
            int genCount = 0;
            bool state = true;
            int num_stable;
            string last_save = "test";
            while (state) {

                if(!Console.IsInputRedirected && Console.KeyAvailable) {
                    ConsoleKeyInfo name = Console.ReadKey();
                    if (name.KeyChar == 'q') {
                        Save(genCount, Classify(board), board);
                        state = false;
                    }
                    else if (name.KeyChar == 's') {
                        Save(genCount, Classify(board), board);
                        last_save = "gen-" + genCount.ToString();
                    }
                    else if (name.KeyChar == 'l') {
                        Console.Clear();
                        string namef = Console.ReadLine();
                        Load(namef);
                        state_dict = new Dictionary<int, int>();
                    }
                    else if (name.KeyChar == 'r') {
                        Load(last_save);
                        state_dict = new Dictionary<int, int>();
                    }
                }

                //Console_print_board(board, 300);

                num_stable = board.Number_of_alive_cells();
                int q = Math.Max(board.Height, board.Height);

                if (state_dict.ContainsKey(num_stable)) {
                    if(state_dict[num_stable] < 2*q) {
                        state_dict[num_stable]++;
                    }
                    else {
                        Save(genCount, Classify(board), board);
                        Save_diagnostic_inf(board, genCount - 2*q);
                        state = false;
                    }
                }
                else {
                    state_dict.Add(num_stable, 1);
                }

                board.Advance();
                ++genCount;
            }
        }



        static void Main(string[] args)
        {
            Board board = Reset();
            GO(board);
        }
    }
}