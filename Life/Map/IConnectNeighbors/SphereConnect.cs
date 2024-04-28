namespace Life
{
    public class SphereConnect : IConnectNeighbors
    {
        public Cell[,] Connect(Cell[,] cells)
        {
            int columns = cells.GetUpperBound(0) + 1;
            int rows = cells.GetUpperBound(1) + 1;

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : columns - 1;
                    int xR = (x < columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : rows - 1;
                    int yB = (y < rows - 1) ? y + 1 : 0;

                    cells[x, y].neighbors.Add(cells[xL, yT]);
                    cells[x, y].neighbors.Add(cells[x, yT]);
                    cells[x, y].neighbors.Add(cells[xR, yT]);
                    cells[x, y].neighbors.Add(cells[xL, y]);
                    cells[x, y].neighbors.Add(cells[xR, y]);
                    cells[x, y].neighbors.Add(cells[xL, yB]);
                    cells[x, y].neighbors.Add(cells[x, yB]);
                    cells[x, y].neighbors.Add(cells[xR, yB]);
                }
            }
            return cells;
        }
    }
}
