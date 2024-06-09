using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cli_life
{
    public class TextFile
    {
        
        public static void Save_File(string file_name, Board board)
        {

            StreamWriter file = new StreamWriter(file_name);

            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    if (board.Cells[col, row].IsAlive)
                    {
                        file.Write("*");
                    }
                    else file.Write(" ");
                }
                file.WriteLine();
            }

            file.Close();


        }

        
        public static Board Read_File(string file_name)
        {

            StreamReader file = new StreamReader(file_name);

            string line;
            int str = 0;
            char[] lines = null;

            while (!file.EndOfStream)
            {
                line = file.ReadLine();
                lines = line.ToCharArray();


                str++;
            }

            Board board = new Board(lines.Length, str, 1, 0.5);

            file.Close();
            str = 0;
           
            file = new StreamReader(file_name);
            while (!file.EndOfStream)
            {
                line = file.ReadLine();
                lines = line.ToCharArray();
                for (int i = 0; i < lines.Length; i++)
                {


                    if (lines[i] == '*')
                        board.Cells[i, str].IsAlive = true;
                    else
                        board.Cells[i, str].IsAlive = false;

                }

                str++;
            }


            file.Close();
            return board;
        }


        public static bool Check_boat(string file_name, Board board)
        {
            Boat boat = new Boat();
            bool isBoat = true;


            if ((boat.row == board.Rows) && (boat.column == board.Columns))
            {
                string line;

                StreamReader file = new StreamReader(file_name);
                int count_str = 0;
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    string reverse = ReverseStringLinq(boat.str[count_str]);

                    if (!line.Contains(boat.str[count_str]) && !(line.Contains(reverse)))
                    {
                        isBoat = false;
                        break;
                    }
                    count_str++;

                }

                return isBoat;
            }
            else return false;

        }

        public static bool Check_box(string file_name, Board board)
        {
            Box box = new Box();
            bool isBox = true; ;

            if ((box.row == board.Rows) && (box.column == board.Columns))
            {
                string line;

                StreamReader file = new StreamReader(file_name);
                int count_str = 0;
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    string reverse = ReverseStringLinq(box.str[count_str]);

                    if (!line.Contains(box.str[count_str]) && !(line.Contains(reverse)))
                    {
                        isBox = false;
                        break;
                    }
                    count_str++;

                }

                return isBox;
            }

            else return false;

        }

        public static bool Check_block(string file_name, Board board)
        {
            Block block = new Block();
            bool isBlock = true;

            if ((block.row == board.Rows) && (block.column == board.Columns))
            {
                string line;

                StreamReader file = new StreamReader(file_name);
                int count_str = 0;
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    string reverse = ReverseStringLinq(block.str[count_str]);

                    if (!line.Contains(block.str[count_str]) && !(line.Contains(reverse)))
                    {
                        isBlock = false;
                        break;
                    }
                    count_str++;

                }

                return isBlock;
            }
            else return false;
        }

        public static bool Check_snake(string file_name, Board board)
        {
            Snake snake = new Snake();
            bool isSnake = true;
            Snake_vert snake2 = new Snake_vert();

            if (((snake.row == board.Rows) && (snake.column == board.Columns)) || ((snake2.row == board.Rows) && (snake2.column == board.Columns)))
            {
                string line;

                StreamReader file = new StreamReader(file_name);
                int count_str = 0;
                string reverse = "";
                string reverse2 = "";
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    if ((snake.row <= board.Rows) && (snake.column <= board.Columns))
                    {
                        reverse = ReverseStringLinq(snake.str[count_str]);
                        if (!line.Contains(snake.str[count_str]) && !line.Contains(reverse))
                        {
                            isSnake = false;

                        }
                    }
                    if ((snake2.row <= board.Rows) && (snake2.column <= board.Columns))
                    {
                        reverse2 = ReverseStringLinq(snake2.str[count_str]);
                        if (!line.Contains(snake2.str[count_str]) && !line.Contains(reverse2))
                        {
                            isSnake = false;
                            break;
                        }
                    }

                    count_str++;

                }

                return isSnake;
            }
            else return false;
        }

        public static bool Check_pond(string file_name, Board board)
        {
            Pond pond = new Pond();
            bool isPond = true;

            if ((pond.row == board.Rows) && (pond.column == board.Columns))
            {
                string line;

                StreamReader file = new StreamReader(file_name);
                int count_str = 0;
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    string reverse = ReverseStringLinq(pond.str[count_str]);

                    if (!line.Contains(pond.str[count_str]) && !(line.Contains(reverse)))
                    {
                        isPond = false;
                        break;
                    }
                    count_str++;

                }

                return isPond;
            }
            else return false;
        }

        public static bool Check_ship(string file_name, Board board)
        {
            Ship ship = new Ship();
            bool isShip = true;

            if ((ship.row == board.Rows) && (ship.column == board.Columns))
            {
                string line;

                StreamReader file = new StreamReader(file_name);
                int count_str = 0;
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    string reverse = ReverseStringLinq(ship.str[count_str]);

                    if (!line.Contains(ship.str[count_str]) && !(line.Contains(reverse)))
                    {
                        isShip = false;
                        break;
                    }
                    count_str++;

                }

                return isShip;
            }
            else return false;
        }

        static string ReverseStringLinq(string originalString)
        {
            return new string(originalString.Reverse().ToArray());
        }

        public static string Type_Figures(string file_name, Board board)
        {
            string type = "";

            if (Check_block(file_name, board) == true) type = "block"; ;
            if (Check_boat(file_name, board) == true) type = "boat"; ;
            if (Check_ship(file_name, board) == true) type = "ship"; ;
            if (Check_pond(file_name, board) == true) type = "pond"; ;
            if (Check_snake(file_name, board) == true) type = "snake"; ;
            if (Check_box(file_name, board) == true) type = "box"; ;

            return type;
        }
    }
}
