namespace Life
{
    public class MapCGL
    {
        public Cell[,] Cells { get; }
        public int Columns { get; }
        public int Rows { get; }

        public MapCGL(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
            Cells = InitializeCells();
        }

        public MapCGL(Cell[,] cells)
        {
            Columns = cells.GetUpperBound(0) + 1;
            Rows = cells.GetUpperBound(1) + 1;
            Cells = cells;
        }

        public bool this[int x, int y] { get => Cells[x, y].IsAlive; set => Cells[x, y].IsAlive = value; }

        public static MapCGL LoadFromStr(string str)
        {
            string[] strLines = str.Split('\n');
            MapCGL map = new MapCGL(strLines[0].Length, strLines.Length - 1);

            for (int y = 0; y < strLines.Length; y++)
                for (int x = 0; x < strLines[y].Length; x++)
                    map[x, y] = strLines[y][x] == '0' ? false : true;

            return map;
        }

        public MapCGL Copy()
        {
            return LoadFromStr(ToString());
        }

        public override string ToString()
        {
            string strLines = "";

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                    strLines += this[x, y] ? '1' : '0';

                strLines += '\n';
            }

            return strLines;
        }

        private Cell[,] InitializeCells()
        {
            var cells = new Cell[Columns, Rows];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    cells[x, y] = new Cell();

            return cells;
        }
    }
}
