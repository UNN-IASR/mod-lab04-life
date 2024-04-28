namespace Life
{
    public static class AnalyzerMap
    {

        public static int CountAlive(MapCGL map)
        {
            int countLive = 0;

            for (int y = 0; y < map.Rows; y++)
                for (int x = 0; x < map.Columns; x++)
                    countLive += map[x, y] ? 1 : 0;

            return countLive;
        }

        public static int StabilitySystem(CGL connectedMap, uint countIteration)
        {
            CGL copyConnectedMap = connectedMap.Copy();

            for (int i = 0; i < countIteration; i++)
            {
                bool isStability = true;

                copyConnectedMap.Advance();

                for (int y = 0; y < copyConnectedMap.Rows && isStability; y++)
                {
                    for (int x = 0; x < copyConnectedMap.Columns; x++)
                    {
                        if (copyConnectedMap[x, y] != connectedMap[x, y])
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

        public static int Classification(MapCGL board, MapCGL fig)
        {
            int count = 0;

            for (int y = 0; y < board.Rows; y++)
            {
                for (int x = 0; x < board.Columns; x++)
                {
                    bool equalsCells = true;

                    for (int yAbs = y, yRel = 0; yRel < fig.Rows && equalsCells; yAbs++, yRel++)
                    {
                        if (yAbs >= board.Rows)
                            yAbs = 0;

                        for (int xAbs = x, xRel = 0; xRel < fig.Columns; xAbs++, xRel++)
                        {
                            if (xAbs >= board.Columns)
                                xAbs = 0;

                            if (board[xAbs, yAbs] != fig[xRel, yRel])
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
    }
}
