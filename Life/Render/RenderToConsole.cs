using System;
using System.Threading;

namespace Life
{
    public class RenderToConsole : IRender
    {
        public CGL ConnectedMap { get; }
        public SettingsRender Settings { get; }

        public RenderToConsole(CGL connectedMap, SettingsRender settings)
        {
            ConnectedMap = connectedMap;
            Settings = settings;
        }

        public void Render()
        {
            for (int i = 0; i < Settings.MaxIteration; i++)
            {
                Console.Clear();
                RenderStep();
                ConnectedMap.Advance();
                Thread.Sleep(Settings.TimeDelay);
            }
        }

        public void RenderStep()
        {
            for (int row = 0; row < ConnectedMap.Map.Rows; row++)
            {
                for (int col = 0; col < ConnectedMap.Map.Columns; col++)
                {
                    if (ConnectedMap[col, row])
                        Console.Write(Settings.SymbolLive);
                    else
                        Console.Write(Settings.SymbolDead);
                }
                Console.Write('\n');
            }
        }
    }
}
