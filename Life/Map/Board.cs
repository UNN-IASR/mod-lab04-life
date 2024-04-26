using cli_life;
using System;

namespace Life
{
    public class Board : Map
    {
        readonly Random rand = new Random();

        public Board(SettingsMap settings, IConnectNeighbors connectNeighbors, double? liveDensity = null)
            : base(settings, connectNeighbors)
        {
            if (settings.BoardStr == null && liveDensity == null)
                throw new NullReferenceException("LiveDensity and BoardStr in " +
                "Settings cannot be Null at the same time");

            Cells = settings.BoardStr == null ?
                CriateRandomizeCells((double)liveDensity, rand) :
                CriateCellsFromStr(settings.BoardStr);
        }

        protected Cell[,] CriateRandomizeCells(double liveDensity, Random rd)
        {
            Cell[,] cells = InitilazeCells(Colums, Rows);

            foreach (var cell in cells)
                cell.IsAlive = rd.NextDouble() < liveDensity;

            return cells;
        }

        public void AddFigure(uint posX, uint posY, SettingsMap settings)
        {
            if (settings.BoardStr == null)
                throw new ArgumentException("settings.BoardStr shouldn't be null");
            if (posX > Colums || posY > Rows)
                throw new ArgumentException("posX or posY shouldn't be more than Colums or Rows");

            Cell[,] cellsAdd = CriateCellsFromStr(settings.BoardStr);

            for (uint yAbs = posY, yRel = 0; yRel < cellsAdd.GetUpperBound(1) + 1; yAbs++, yRel++)
            {
                if (yAbs >= Rows)
                    yAbs = 0;

                for (uint xAbs = posX, xRel = 0; xRel < cellsAdd.GetUpperBound(0) + 1; xAbs++, xRel++)
                {
                    if (xAbs >= Colums)
                        xAbs = 0;

                    Cells[xAbs, yAbs].IsAlive = cellsAdd[xRel, yRel].IsAlive;
                }
            }
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
    }
}