using cli_life;
using System.IO;
using StatusFileController;

namespace ClassificationTK
{
    public class Classification
    {
        public static Cell[][,] LoadClassificationFiles()
        {
            string[] aFileNamesCL = {
                    "ClassificationFiles/BargeCl.txt",
                    "ClassificationFiles//BeehiveCl.txt",
                    "ClassificationFiles//BlockCl.txt",
                    "ClassificationFiles//BoatCl.txt",
                    "ClassificationFiles//BoxCl.txt",
                    "ClassificationFiles//LoafCl.txt",
                    "ClassificationFiles//PondCl.txt",
                    "ClassificationFiles//ShipCl.txt"
                };
            Cell[] [,] aShapesCells = new Cell[aFileNamesCL.Length] [,];

            for(int i = 0; i < aFileNamesCL.Length; ++i)
            {
                string aSaveStream = File.ReadAllText(aFileNamesCL[i]);
                string[] aNumerLines = aSaveStream.Split("\n");

                aShapesCells[i] = new Cell[aNumerLines.Length - 1,aNumerLines.Length - 1];

                for(int j = 0; j < aNumerLines.Length - 1; ++j)
                {
                    for(int k = 0; k < aNumerLines.Length - 1; ++k)
                    {
                        aShapesCells[i][j,k] = new Cell();
                    }
                }

                TextController.ReadLifeStatus(aShapesCells[i], aFileNamesCL[i]);
            }

            return aShapesCells;
        }
        public static bool IsSameClasses(Cell[,] theFirstCells, Cell[,] theSecondCells)
        {
            int countSameCells = 0;

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    if (theFirstCells[i,j].IsAlive == theSecondCells[i,j].IsAlive)
                    {
                        countSameCells ++;
                    }
                }
            }
            if (countSameCells == 16)
            {
                return true;
            }
            countSameCells = 0;

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    if (theFirstCells[i,j].IsAlive == theSecondCells[j,i].IsAlive)
                    {
                        countSameCells ++;
                    }
                }
            }
            if (countSameCells == 16)
            {
                return true;
            }

            return false;
        }

        public static int[] getClassificationBoardState(Board theBoard)
        {
            Cell[] [,] aShapeCells = Classification.LoadClassificationFiles();
            int[] aResultShapesNum = new int[aShapeCells.Length];
            string[] aShapeNames = {"BARGE",
                                    "BEEHIVE",
                                    "BLOCK",
                                    "BOAT",
                                    "BOX",
                                    "LOAF",
                                    "POND",
                                    "SHIP"};

            for (int shapeNum = 0; shapeNum < aShapeCells.Length; ++shapeNum)
            {
                for (int x = 0; x < theBoard.Width; ++x)
                {
                    for (int y = 0; y < theBoard.Height; ++y)
                    {
                        Cell [,] aCells = new Cell[aShapeCells[shapeNum].GetLength(0),aShapeCells[shapeNum].GetLength(1)];

                        for (int row = 0; row < aShapeCells[shapeNum].GetLength(0); ++row)
                        {
                            for (int col = 0; col < aShapeCells[shapeNum].GetLength(1); ++col)
                            {
                                aCells[row, col] = theBoard.Cells[(x + row) % theBoard.Cells.GetLength(0), (y + col) % theBoard.Cells.GetLength(1)];
                            }
                        }

                        if (Classification.IsSameClasses(aShapeCells[shapeNum], aCells))
                        {
                            aResultShapesNum[shapeNum]++;
                        }
                    }
                }
            }
            return aResultShapesNum;
        }

        public static int getSymmetricalElements(Board theBoard)
        {
            int[] theClassification = getClassificationBoardState(theBoard);

            return theClassification[0] + theClassification[2] + theClassification[3]
                 + theClassification[4] + theClassification[6] + theClassification[7];
        }
        public static int StabilityPhase(ref Board board)
        {
            Board aStartBoard = new Board(board.Width, board.Height, 1);

            Cell[,] aFirstCellState = new Cell[board.Width,board.Height];
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    aFirstCellState[row, col] = new Cell();
                    aFirstCellState[row, col].IsAlive = board.Cells[row,col].IsAlive;
                }
            }
            aStartBoard.Cells = aFirstCellState;
            aStartBoard.ConnectNeighbors();

            int aCellsNumber = 0;
            int aStep = 0;
            while(true)
            {
                Cell[,] aPreviousBorder = new Cell[board.Width,board.Height];

                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Columns; col++)   
                    {
                        aPreviousBorder[row, col] = new Cell();
                        aPreviousBorder[row, col].IsAlive = board.Cells[row,col].IsAlive;
                    }
                }
                board.Advance();

                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Columns; col++)   
                    {
                        if (aPreviousBorder[row,col].IsAlive == board.Cells[row, col].IsAlive)
                        {
                            aCellsNumber++;
                        }
                    }
                }

                aStep ++;
                if(aCellsNumber == board.Columns * board.Rows || aStep > 1000)
                {
                    break;
                }
                aCellsNumber = 0;
            }
            
            board = aStartBoard;

            return aStep;
        }
    }
}