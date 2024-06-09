using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace cli_life
{


    class Program
    {
        static int count_step = 0;
        static int count_stable_phase = 0;
        static Cell[,] stable_phase;

        static Board board;
        static private void Reset()
        {
            board = new Board(
                width: 50,
                height: 20,
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


        static void Start()
        {

            Console.Clear();
            Reset();
            count_step = 0;
            Render();
            stable_phase = new Cell[board.Columns, board.Rows];
            board.Get_stable_phase(stable_phase);
        }

        static void Step()
        {

            Console.Clear();
            board.Advance();
            int count_isAlive = board.Count_isAlive();
            Render();

            if (board.Check_stable_phase(stable_phase) == true) count_stable_phase++;
            else
            {
                board.Get_stable_phase(stable_phase);
                count_stable_phase = 0;
            }

            count_step++;
            if (count_stable_phase == 10) Console.WriteLine("Count_stable_phase " + (count_step - 10));

            Console.WriteLine("Count_step= " + count_step);
            Console.WriteLine("Count is Alive=" + count_isAlive);
        }

        static void Load()
        {
            Console.Clear();
            count_step = 0;
            string file_name = Console.ReadLine();
            board = TextFile.Read_File(file_name);

            Render();

            stable_phase = new Cell[board.Columns, board.Rows];
            board.Get_stable_phase(stable_phase);
        }


        static void Save()
        {

            string file_name = "save.txt";
            file_name = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\", file_name);
            TextFile.Save_File(file_name, board);
        }

        static void Change_setting()
        {
            Console.Clear();
            string fileName = "setting.json";
            board = Json.Change_setting(fileName);
            count_step = 0;

            Render();
        }

        static void Simmetry_X()
        {

            bool sim_OX = board.Check_Simmetri_OX();

            if (sim_OX == true) Console.WriteLine("OX_simmetry");
            else Console.WriteLine("not OX_simmetry");

        }

        static void Simmetry_Y()
        {

            bool sim_OY = board.Check_Simmetri_OX();

            if (sim_OY == true) Console.WriteLine("OY_simmetry");
            else Console.WriteLine("not OY_simmetry");
        }

        static void Figure()
        {
            Console.WriteLine();
            string file_name = Console.ReadLine();
            board = TextFile.Read_File(file_name);
            Render();
            stable_phase = new Cell[board.Columns, board.Rows];
            board.Get_stable_phase(stable_phase);
            Console.WriteLine('\n' + TextFile.Type_Figures(file_name, board));

        }

        static void Main(string[] args)
        {

            Reset();
            while (true)
            {

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();

                    switch (key.KeyChar)
                    {
                        case 'g':
                            Start();
                            break;

                        case ' ':
                            Step();
                            break;
                        case 'l':
                            Load();
                            break;
                        case 's':
                            Save();
                            break;
                        case 'c':
                            Change_setting();
                            break;
                        case 'x':
                            Simmetry_X();
                            break;
                        case 'y':
                            Simmetry_Y();
                            break;
                        case 'f':
                            Figure();
                            break;
                    }
                }


                // Render();
                //board.Advance();
                //Thread.Sleep(1000);
            }
        }
    }
}