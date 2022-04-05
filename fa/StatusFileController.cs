using System.IO;
using System;
using cli_life;

namespace StatusFileController
{
    public class TextController
    {
        public static void SaveLifeStatus(Cell[,] theCells, string theFileName = "LifeStatus.txt")
        {
            using StreamWriter aSaveLifeStatus = new StreamWriter(theFileName);
 
            for (int row = 0; row < theCells.GetLength(0); row++)
            {
                for (int col = 0; col < theCells.GetLength(1); col++)
                {
                    if (theCells[row, col].IsAlive)
                    {
                        aSaveLifeStatus.Write("1 ");
                    }
                    else
                    {
                        aSaveLifeStatus.Write("0 ");
                    }
                }
                aSaveLifeStatus.Write("\n");
            }
        }

        public static void ReadLifeStatus(Cell[,] theCells, string theFileName = "LifeStatus.txt")
        {
            string aSaveStream = File.ReadAllText(theFileName);

            string[] aCellsLines = aSaveStream.Split("\n");

            for(int i = 0; i < aCellsLines.Length - 1; i++)
            {
                string[] aCellsLine = aCellsLines[i].Split(" ");

                for (int j = 0; j < aCellsLine.Length - 1; j++)
                {
                    
                    theCells[i,j].IsAlive = (Int32.Parse(aCellsLine[j]) == 1) ? true : false;
                }
            }
        }
    }
}