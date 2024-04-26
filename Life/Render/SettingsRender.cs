namespace Life
{
    public class SettingsRender
    {
        public char SymbLive { get; set; } = '#';
        public char SymbDead { get; set; } = ' ';
        public int WidthMap { get; set; } = 1000;
        public int HeightMap { get; set; } = 1000;
        public int? SizeCell { get; set; } = null;
        public int MaxIteration { get; set; } = 100;
        public int TimeDelay { get; set; } = 500;
    }
}