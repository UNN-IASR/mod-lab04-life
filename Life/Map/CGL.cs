namespace Life
{
    public class CGL
    {
        public MapCGL Map { get; }
        public int Columns { get => Map.Columns; }
        public int Rows { get => Map.Rows; }
        public IConnectNeighbors ConnectNeighbors { get; }
        public CGL(MapCGL map, IConnectNeighbors connectNeighbors)
        {
            Map = map;
            ConnectNeighbors = connectNeighbors;

            ConnectCells();
        }

        public bool this[int x, int y] { get => Map[x, y]; set => Map[x, y] = value; }

        public CGL Copy()
        {
            return new CGL(Map.Copy(), ConnectNeighbors);
        }

        public void Advance()
        {
            foreach (var cell in Map.Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Map.Cells)
                cell.Advance();
        }

        private Cell[,] ConnectCells()
        {
            return ConnectNeighbors.Connect(Map.Cells);
        }
    }
}
