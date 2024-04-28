using System;

namespace Life
{
    public static class FillMap
    {
        public static MapCGL FillRandom(MapCGL map, double liveDensity, Random rd)
        {
            MapCGL newMap = map.Copy();

            for (int x = 0; x < newMap.Columns; x++)
                for (int y = 0; y < newMap.Rows; y++)
                    newMap[x, y] = rd.NextDouble() < liveDensity;

            return newMap;
        }

        public static MapCGL AddFigure(uint posX, uint posY, MapCGL map, MapCGL figure)
        {
            MapCGL newMap = map.Copy();

            for (int yAbs = (int)posY, yRel = 0; yRel < figure.Rows; yAbs++, yRel++)
            {
                if (yAbs >= map.Rows)
                    yAbs = 0;

                for (int xAbs = (int)posX, xRel = 0; xRel < figure.Columns; xAbs++, xRel++)
                {
                    if (xAbs >= map.Columns)
                        xAbs = 0;

                    newMap[xAbs, yAbs] = figure[xRel, yRel];
                }
            }

            return newMap;
        }
    }
}
