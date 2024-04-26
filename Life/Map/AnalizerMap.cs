using cli_life;
using System.Collections.Generic;

namespace Life
{
    public static class AnalizerMap
    {
        public static double AlivePercent(Map map)
        {
            return map.LiveDensity;
        }

        public static bool VerticalSymmetry(Map map)
        {
            int midX = map.Colums / 2;

            for (int xLeft = 0, xRight = map.Colums - 1; xLeft < midX; xLeft++, xRight--)
            {
                for (int y = 0; y < map.Rows; y++)
                    if (map.Cells[xLeft, y].IsAlive != map.Cells[xRight, y].IsAlive)
                        return false;
            }

            return true;
        }

        public static bool HorizontalSymmetry(Map map)
        {
            int midY = map.Rows / 2;

            for (int yUp = 0, yDown = map.Rows - 1; yUp < midY; yUp++, yDown--)
            {
                for (int x = 0; x < map.Colums; x++)
                    if (map.Cells[x, yUp].IsAlive != map.Cells[x, yDown].IsAlive)
                        return false;
            }

            return true;
        }

        public static int StabilitySystem(Board board, uint countIteration)
        {
            Board copyBoard = new Board(board.Settings, board.AlgConnect);

            for (int i = 0; i < countIteration; i++)
            {
                bool isStability = true;

                copyBoard.Advance();

                for (int y = 0; y < copyBoard.Rows && isStability; y++)
                {
                    for (int x = 0; x < copyBoard.Colums; x++)
                    {
                        if (copyBoard.Cells[x, y].IsAlive != board.Cells[x, y].IsAlive)
                        {
                            isStability = false;
                            break;
                        }
                    }
                }

                if (isStability)
                    return i + 1;
            }

            return -1;
        }

        public static int Classification(Map board, Map fig)
        {
            int count = 0;

            for (int y = 0; y < board.Rows; y++)
            {
                for (int x = 0; x < board.Colums; x++)
                {
                    bool equalsCells = true;

                    for (int yAbs = y, yRel = 0; yRel < fig.Rows && equalsCells; yAbs++, yRel++)
                    {
                        if (yAbs >= board.Rows)
                            yAbs = 0;

                        for (int xAbs = x, xRel = 0; xRel < fig.Colums; xAbs++, xRel++)
                        {
                            if (xAbs >= board.Colums)
                                xAbs = 0;

                            if (board.Cells[xAbs, yAbs].IsAlive != fig.Cells[xRel, yRel].IsAlive)
                            {
                                equalsCells = false;
                                break;
                            }
                        }
                    }

                    if (equalsCells)
                        count++;
                }
            }
            return count;
        }


        public static string AnalizeAll(Board board, Map[] figurs)
        {
            string ansStr = $"Statistic \"{board.Name}\" board\n";
            ansStr += "Alive percent: " + AlivePercent(board).ToString() + "\n";
            ansStr += "Vertical symmetry: " + VerticalSymmetry(board).ToString() + "\n";
            ansStr += "Horizontal symmetry: " + HorizontalSymmetry(board).ToString() + "\n";

            int stabilityAns = StabilitySystem(board, 10);

            Dictionary<string, int> figs = new Dictionary<string, int>();
            foreach (Map fig in figurs)
                if (!figs.ContainsKey(fig.Name))
                    figs.Add(fig.Name, 0);

            ansStr += stabilityAns < 1 ?
                "Board isn't stable\n" :
                $"Board is stable: count iteration {stabilityAns}\n";

            foreach (Map fig in figurs)
                figs[fig.Name] += Classification(board, fig);

            foreach (var fig in figs)
                ansStr += $"Figure \"{fig.Key}\" {fig.Value}\n";

            return ansStr;
        }
    }
}