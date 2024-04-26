namespace Life
{
    public abstract class Map
    {
        public Cell[,] Cells { get; protected set; }
        public int Colums { get; private set; }
        public int Rows { get; private set; }
        public string Name { get; set; }
        public double LiveDensity { get => ColculateLiveDensity(Cells); }
        public IConnectNeighbors AlgConnect { get; }
        public SettingsMap Settings
        {
            get
            {
                SettingsMap settings = new SettingsMap(Colums, Rows, CriateStrFromCells(Cells), Name);
                return settings;
            }
        }

        public Map(SettingsMap settings, IConnectNeighbors algConnect)
        {
            Colums = settings.Colums;
            Rows = settings.Rows;
            Name = settings.Name;

            AlgConnect = algConnect;
            Cells = InitilazeCells(Colums, Rows);
        }

        public static bool Equals(Map map1, Map map2)
        {
            SettingsMap settingsMap1 = map1.Settings;
            SettingsMap settingsMap2 = map2.Settings;

            if (settingsMap1.BoardStr == settingsMap2.BoardStr)
                return true;
            return false;
        }

        public void SaveToJson(string path)
        {
            SaveJson<SettingsMap>.SaveToJson(path, Settings);
        }

        protected Cell[,] InitilazeCells(int colums, int rows)
        {
            Cell[,] cells = new Cell[colums, rows];

            for (int x = 0; x < colums; x++)
                for (int y = 0; y < rows; y++)
                    cells[x, y] = new Cell();

            cells = ConnectNeighbors(cells, AlgConnect);

            return cells;
        }

        protected Cell[,] CriateCellsFromStr(string str)
        {
            string[] strLines = str.Split('\n');
            Cell[,] cells = InitilazeCells(strLines[0].Length, strLines.Length - 1);

            for (int y = 0; y < strLines.Length; y++)
                for (int x = 0; x < strLines[y].Length; x++)
                    cells[x, y].IsAlive = strLines[y][x] == '0' ? false : true;

            return cells;
        }

        protected string CriateStrFromCells(Cell[,] cells)
        {
            int colums = cells.GetUpperBound(0) + 1;
            int rows = cells.GetUpperBound(1) + 1;
            string strLines = "";

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < colums; x++)
                    strLines += cells[x, y].IsAlive ? '1' : '0';

                strLines += '\n';
            }

            return strLines;
        }

        protected static Cell[,] ConnectNeighbors(Cell[,] cells, IConnectNeighbors connectNeighbors)
        {
            return connectNeighbors.Connect(cells);
        }

        protected static double ColculateLiveDensity(Cell[,] cells)
        {
            int colums = cells.GetUpperBound(0) + 1;
            int rows = cells.GetUpperBound(1) + 1;

            int countLive = 0;
            int countEmpty = 0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < colums; x++)
                {
                    if (cells[x, y].IsAlive)
                        countLive++;
                    else
                        countEmpty++;
                }
            }

            return (double)(countLive) / countEmpty;
        }
    }
}