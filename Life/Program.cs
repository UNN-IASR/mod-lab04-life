using Life;
using System;

namespace cli_life
{
    class Program
    {
        static SettingsRender settingsRender = new SettingsRender()
        {
            SymbolLive = '#',
            SymbolDead = ' ',
            MaxIteration = 1000,
            TimeDelay = 100
        };

        static MapCGL map = new MapCGL(columns: 20, rows: 10);

        static Random rd = new Random();
        static double liveDensity = 0.1;

        static void Main(string[] args)
        {
            map = FillMap.FillRandom(map, liveDensity, rd);
            IRender renderToConsole = new RenderToConsole(new CGL(map, new SphereConnect()), settingsRender);

            renderToConsole.Render();
        }
    }
}