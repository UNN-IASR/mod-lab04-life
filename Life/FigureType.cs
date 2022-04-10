using cli_life;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Life
{
    public class FigureType
    {
        public Dictionary<string, string> Examples;
        public FigureType()
        {
            Examples = new Dictionary<string, string>();
            Examples.Add("Block", File.ReadAllText("Block.txt"));
            Examples.Add("Pond", File.ReadAllText("Pond.txt"));
            Examples.Add("Hive", File.ReadAllText("Hive.txt"));
            Examples.Add("Ship", File.ReadAllText("Ship.txt"));
            Examples.Add("Boat", File.ReadAllText("Boat.txt"));
            Examples.Add("Box", File.ReadAllText("Box.txt"));
            Examples.Add("Barge", File.ReadAllText("Barge.txt"));
            Examples.Add("Line", File.ReadAllText("Line.txt"));
        }

        public int CheckSimilarity(Board board, string[] example)
        {
            int result = 0;

            for (int row = 0; row < board.Rows - (example.Length - 1); row++)
            {
                for (int col = 0; col < board.Columns - example[0].Length + 1; col++)
                {
                    int tempresult = 0;
                    int temprow = 0;

                    for (int temp1 = row; temp1 < row + example.Length; temp1++)
                    {
                        int tempcol = 0;
                        for (int temp2 = col; temp2 < col + example[0].Length; temp2++)
                        {
                            if (((board.Cells[temp2, temp1].IsAlive == true) && (example[temprow][tempcol] == '*')) ||
                                    ((board.Cells[temp2, temp1].IsAlive == false) && (example[temprow][tempcol] == ' ')))
                            {
                                tempresult++;
                            }
                            tempcol++;
                        }
                        temprow++;

                    }
                    if (tempresult == (example.Length * (example[0].Length)))
                    {
                        result++;
                    }

                }
            }
            return result;
        }
        public Dictionary<string, int> CountAllFigures(Board board)//Также обработаны случаи, когда фигуры повернуты относительно шаблонов
        {
            Dictionary<string, int> figureCount = new Dictionary<string, int>();
            foreach (var item in Examples)
            {
                int[] rotateCount = new int[4];
                string[] tempstr;
                if (item.Value.Contains('\r'))
                {
                    tempstr = item.Value.Split("\r\n");
                }
                else
                {
                    tempstr = item.Value.Split('\n');
                }

                rotateCount[0] = CheckSimilarity(board, tempstr);
                string[] tempstr1 = new string[RotateMatrix(tempstr).Length];
                Array.Copy(RotateMatrix(tempstr), tempstr1, RotateMatrix(tempstr).Length);

                rotateCount[1] = CheckSimilarity(board, tempstr1);

                string[] tempstr2 = new string[RotateMatrix(tempstr1).Length];
                Array.Copy(RotateMatrix(tempstr1), tempstr2, RotateMatrix(tempstr1).Length);
                rotateCount[2] = CheckSimilarity(board, tempstr2);

                string[] tempstr3 = new string[RotateMatrix(tempstr2).Length];
                Array.Copy(RotateMatrix(tempstr2), tempstr3, RotateMatrix(tempstr2).Length);
                rotateCount[3] = CheckSimilarity(board, tempstr3);

                int sum = 0;
                for (int i = 0; i < 4; i++)
                {
                    sum = sum + rotateCount[i];
                }
                switch (item.Key)
                {
                    case "Block":
                    case "Pond":
                    case "Box":
                        sum = sum / 4;
                        break;
                    case "Hive":
                    case "Barge":
                    case "Ship":
                    case "Line":
                        sum = sum / 2;
                        break;
                }
                figureCount.Add(item.Key, sum);
            }
            return figureCount;
        }
        public int StablePhase(Board board)//обработаны случаи с переодической фигурой 3-х клеточной линии
        {
            int step = 0;
            int diff = 1;
            int key = 1;
            while (key != 0)
            {
                Cell[,] previousState = new Cell[board.Columns, board.Rows];
                for (int i = 0; i < board.Columns; i++)
                {
                    for (int j = 0; j < board.Rows; j++)
                    {
                        previousState[i, j] = new Cell();
                        previousState[i, j].IsAlive = board.Cells[i, j].IsAlive;
                    }
                }

                board.Advance();
                Cell[,] newState = new Cell[board.Columns, board.Rows];
                for (int i = 0; i < board.Columns; i++)
                {
                    for (int j = 0; j < board.Rows; j++)
                    {
                        newState[i, j] = new Cell();
                        newState[i, j].IsAlive = board.Cells[i, j].IsAlive;
                    }
                }
                diff = 0;
                for (int i = 0; i < board.Columns; i++)
                {
                    for (int j = 0; j < board.Rows; j++)
                    {
                        if (previousState[i, j].IsAlive != newState[i, j].IsAlive)
                        {
                            diff++;
                        }
                    }
                }
                step++;

                Dictionary<string, int> answer = new Dictionary<string, int>();
                answer = CountAllFigures(board);

                int temp = diff - answer["Line"] * 4;

                if (diff == 0)
                {
                    key = 0;
                }
                else
                {
                    if (temp == 0)
                    {
                        key = 0;
                    }
                }
            }
            return step;
        }

        public int StablePhaseRender(Board board)//обработаны случаи с переодической фигурой 3-х клеточной линии
        {
            int step = 0;
            int diff = 1;
            int key = 1;
            while (key != 0)
            {
                Console.Clear();
                board.Render();
                Cell[,] previousState = new Cell[board.Columns, board.Rows];
                for (int i = 0; i < board.Columns; i++)
                {
                    for (int j = 0; j < board.Rows; j++)
                    {
                        previousState[i, j] = new Cell();
                        previousState[i, j].IsAlive = board.Cells[i, j].IsAlive;
                    }
                }

                board.Advance();
                Cell[,] newState = new Cell[board.Columns, board.Rows];
                for (int i = 0; i < board.Columns; i++)
                {
                    for (int j = 0; j < board.Rows; j++)
                    {
                        newState[i, j] = new Cell();
                        newState[i, j].IsAlive = board.Cells[i, j].IsAlive;
                    }
                }
                diff = 0;
                for (int i = 0; i < board.Columns; i++)
                {
                    for (int j = 0; j < board.Rows; j++)
                    {
                        if (previousState[i, j].IsAlive != newState[i, j].IsAlive)
                        {
                            diff++;
                        }
                    }
                }
                step++;

                Dictionary<string, int> answer = new Dictionary<string, int>();
                answer = CountAllFigures(board);

                int temp = diff - answer["Line"] * 4;

                if (diff == 0)
                {
                    key = 0;
                }
                else
                {
                    if (temp == 0)
                    {
                        key = 0;
                    }
                }
            }
            return step;
        }
        public int CountSymm(Board board)
        {
            Dictionary<string, int> figures = CountAllFigures(board);
            int result = 0;
            result = figures["Block"] + figures["Pond"] + figures["Ship"] + figures["Box"] + figures["Barge"];
            return result;
        }
        public string[] RotateMatrix(string[] oldMatrix)
        {
            string[] newMatrix = new string[oldMatrix[0].Length];
            int newColumn, newRow = 0;
            for (int oldColumn = oldMatrix[0].Length - 1; oldColumn >= 0; oldColumn--)
            {
                string temp = default;

                newColumn = 0;
                for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
                {
                    temp = temp + oldMatrix[oldRow][oldColumn];
                    newColumn++;
                }
                newMatrix[newRow] = temp;
                newRow++;
            }
            return newMatrix;
        }
    }
}
