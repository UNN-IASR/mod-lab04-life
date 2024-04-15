﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Drawing;
using System.Formats.Asn1;
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

        private Dictionary<int,int> count_sleeping;
        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public int SleepAccuracy { get { return Convert.ToInt32(Math.Round(Math.Sqrt(Height * Width))); } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            count_sleeping = new Dictionary<int, int>();
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
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

        public int countAlive(){
            int count = 0;
            foreach (Cell cell in Cells){
                if (cell.IsAlive) count ++;
            }
            return count;
        }

        public bool isStable(){
            int count = countAlive();
            if (!count_sleeping.ContainsKey(count)) count_sleeping[count] = 0;
            count_sleeping[count] += 1;
            return count_sleeping[count] >= SleepAccuracy;
        }

        public Dictionary<string, int> countEntities(){
            Dictionary<string, int> entities = new Dictionary<string,int>();
            entities["cells at all"] = countAlive();
            Dictionary <string, string> life = new Dictionary<string, string>()
            {
                // block 4x4
                {"0000011001100000", "block"},
                // hive 6x5, 5x6
                {"000000001100010010001100000000", "hive"},
                {"000000010001010010100010000000", "hive"},
                // loaf 6x6
                {"000000000100001010010010001100000000", "loaf"},
                {"000000001000010100010010001100000000", "loaf"},
                {"000000001100010010010100001000000000", "loaf"},
                {"000000001100010010001010000100000000", "loaf"},
                // tub 5x5
                {"00000001000101000100000000", "tub"},
                // boat 5x5
                {"00000011000101000100000000", "boat"},
                {"00000010000101000110000000", "boat"},
                {"00000001100101000100000000", "boat"},
                {"00000001000101001100000000", "boat"},
                // ship 5x5
                {"00000011000101000110000000", "ship"},
                {"00000001100101001100000000", "ship"},
                // pond 6x6
                {"000000001100010010010010001100000000", "pond"},
                // spinner 5x3, 3x5
                {"000000111000000", "spinner"},
                {"000010010010000", "spinner"}

            };
            int[] heights = new int[] { 3, 4, 5, 6};
            int[] widths = new int[] { 3, 4, 5, 6};
            foreach (int h in heights){
                foreach (int w in widths){
                    for (int x = 0; x < Width; x++){
                        for (int y = 0; y < Height; y++){
                            string window = "";
                            for (int i = x; i < x+w; i++){
                                for (int j = y; j < y+h; j++){
                                    window += Cells[i % Width, j % Height].IsAlive ? "1" : "0";
                                } 
                            }
                            if (life.ContainsKey(window)){
                                if (!entities.ContainsKey(life[window])){
                                    entities[life[window]] = 0;
                                }
                                entities[life[window]] += 1;
                            }
                        }
                    }
                }
            }
            return entities;
        }
    }

    class SettingsParser
    {
        public int width {get; set;}
        public int height {get; set;}
        public int cellSize {get; set;}
        public double liveDensity {get; set;}
    }

    class Program
    {
        static Board board;
        static int genCount;
        static int MAXITER = Convert.ToInt32(Math.Pow(10, 4));
        static private void Reset(double dens=0)
        {
            if(!Directory.Exists("Input")) Directory.CreateDirectory("Input/");
            if(!File.Exists("Input/settings.json")) File.Create("Input/settings.json");
            string raw = File.ReadAllText("Input/settings.json");
            if (raw != ""){
                var settings = JsonSerializer.Deserialize<SettingsParser>(raw);
                if (dens != 0){
                    board = new Board(
                        width: settings.width,
                        height: settings.height,
                        cellSize: settings.cellSize,
                        liveDensity: dens);
                }
                else{
                    board = new Board(
                        width: settings.width,
                        height: settings.height,
                        cellSize: settings.cellSize,
                        liveDensity: settings.liveDensity);
                }
            }
            else {
                board = new Board(
                    width: 20,
                    height: 20,
                    cellSize: 1,
                    liveDensity: dens);
            }
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

        static int Load()
        {
            if(!Directory.Exists("Input")) {
                Directory.CreateDirectory("Input/");
                File.WriteAllText("Input/gen-0.txt", "");
            }
            else if(!File.Exists("Input/gen-0.txt")) {
                File.WriteAllText("Input/gen-0.txt", "");
            }

            string[] raw = File.ReadAllLines("Input/gen-0.txt");

            int wid = 0;
            int hei = 0;
            int gen = 0;
            if (raw.Length > 0) {
                wid = raw[0].Length;
                hei = raw.Length - 1;
                gen = int.Parse(raw[hei]);
            }

            board = new Board(
                width: wid,
                height: hei,
                cellSize: 1,
                liveDensity: 0);

            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (raw[row][col] == '1') {
                        cell.IsAlive = true;
                    }
                    else {
                        cell.IsAlive = false;
                    }
                }
            }
            return gen;
        }

        static void Save(Dictionary<string, int> c_data=null)
        {
            string fname = "gen-" + genCount.ToString();
            if(!Directory.Exists("Data")) Directory.CreateDirectory("Data/");
            StreamWriter writer = new StreamWriter("Data/"+ fname + ".txt");
            double[,] data = new double[board.Rows, board.Columns];
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive) {
                        writer.Write('1');
                        data[row,col] = 1;
                    }
                    else {
                        writer.Write('0');
                        data[row,col] = 0;
                    }
                }
                writer.Write("\n");
            }
            writer.Write(genCount);
            if (c_data != null){
                foreach (var entity in c_data) {
                    writer.WriteLine(entity.Key + " - " + entity.Value);
                }
            }
            writer.Close();
        }

        static void Main(string[] args)
        {
            int left = 20;
            int right = 70;
            int step = 5;
            int num_sim = 10;
            Dictionary<double, double> avg_life = new Dictionary<double, double>();
            ScottPlot.Plot GreatPlot = new ScottPlot.Plot();
            for (int dens = left; dens <= right; dens += step){
                ScottPlot.Plot scottPlot= new();
                int sum_life = 0;
                List <int> dataGX = new List<int>();
                List <int> dataGY = new List<int>();
                for (int i = 0; i < num_sim; i++){
                    List<int> dataY = new List<int>();
                    List<int> dataX = new List<int>();
                    Reset(dens / 100.0);
                    genCount = 0;
                    while(true)
                    {
                        if(Console.KeyAvailable) {
                            ConsoleKeyInfo name = Console.ReadKey();
                            if(name.KeyChar == 'q')
                                break;
                            else if(name.KeyChar == 's') {
                                Save();
                            }
                            else if (name.KeyChar == 'l') {
                                genCount = Load();
                            }
                        }  
                        
                        //Console.Clear();
                        //Render();
                        board.Advance();

                        dataY.Add(board.countAlive());

                        if (board.isStable()) {
                            genCount -= board.SleepAccuracy;
                            Dictionary<string, int> count_data = board.countEntities();
                            //Save(count_data);
                            Console.Clear();
                            Render();
                            foreach (var entity in count_data) {
                                Console.WriteLine(entity.Key + " - " + entity.Value);
                            }
                            break;
                        }

                        //Thread.Sleep(500);
                        ++genCount;
                        if (genCount >= board.Width*board.Height) break;
                    }
                    
                    if (genCount == MAXITER) {
                        IEnumerable<int> numberSequence = Enumerable.Range(0, genCount);
                        dataX = numberSequence.ToList();
                    }
                    else {
                        dataY.RemoveRange(genCount + 3, board.SleepAccuracy - 3);
                        IEnumerable<int> numberSequence = Enumerable.Range(0, genCount + 4);
                        dataX = numberSequence.ToList();
                    }
                    scottPlot.Add.Scatter(dataX, dataY);
                    if (dataGX.Count < dataX.Count){
                        dataGX = dataX.GetRange(0, dataX.Count);
                        dataGY = dataY.GetRange(0, dataY.Count);
                    }
                    sum_life += genCount;
                }
                scottPlot.Add.Annotation((dens / 100.0).ToString());
                scottPlot.SavePng("plot-" + (dens / 100.0).ToString() + ".png", 1920, 1080);
                avg_life[dens / 100.0] = sum_life / num_sim;
                GreatPlot.Add.Scatter(dataGX, dataGY);
                GreatPlot.Add.Text((dens / 100.0).ToString(), dataGX.Last()+3, dataGY.Last()+3);
            }
            GreatPlot.SavePng("plot_all.png", 1920, 1080);
            ScottPlot.Plot AvgPlot = new ScottPlot.Plot();
            ScottPlot.Bar[] bars = new ScottPlot.Bar[avg_life.Count];
            int k = 0;
            foreach (var avg in avg_life){
                bars[k] = new () {Position = avg.Key, Value = avg.Value, 
                    FillColor=Colors.AliceBlue, Size = 0.03};
                k ++;
            }
            AvgPlot.Add.Bars(bars);
            AvgPlot.SavePng("plot_avg.png", 1920, 1080);
        }
    }
}