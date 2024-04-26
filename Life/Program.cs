using System;
using System.Collections.Generic;
using System.Threading;
using Life;

namespace cli_life
{
    class Program
    {
        static Board board;
        static Board[] figurs;
        static string PathSave = "";
        static Thread threadRender;
        static bool breakProgram = false;
        static bool exit = false;

        static private void Reset()
        {
            SettingsMap settings = new SettingsMap(100, 40, name: "MainBoard");
            board = new Board(settings, new SphereConnect(), 0.5);

            string path = "Figurs.txt";
            SettingsMap[] figSettings = SaveJson<SettingsMap[]>.LoadFromJSon(path);

            figurs = new Board[figSettings.Length];
            for (int i = 0; i < figSettings.Length; i++)
                figurs[i] = new Board(figSettings[i], new SphereConnect());

            PathSave = "";
        }

        static void Main(string[] args)
        {
            Reset();
            SettingsRender sRB = new SettingsRender();
            sRB.WidthMap = 1000;
            sRB.HeightMap = 400;
            sRB.MaxIteration = 100;
            sRB.TimeDelay = 100;

            RenderToConsole renderToConsole = new RenderToConsole(board, sRB);
            Thread threadWaitKey = new Thread(WaitKey);

            threadWaitKey.Start();
            for (int step = 0; step < sRB.MaxIteration && !exit; step++)
            {
                Console.Clear();
                renderToConsole.RenderStep();
                board.Advance();
                Thread.Sleep(sRB.TimeDelay);
                while (breakProgram)
                    Thread.Sleep(100);
            }
        }

        public static void WaitKey()
        {
            while (!exit)
            {
                string ans = "\n\n";
                var key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.S:
                        if (PathSave != null)
                        {
                            board.SaveToJson(PathSave + "settingsBoard.txt");
                            ans += "Board are save";
                        }
                        else
                            ans += "The path is null";
                        break;
                    case ConsoleKey.E:
                        ans += "Exit from program";
                        exit = true;
                        break;
                    case ConsoleKey.A:
                        ans += AnalizerMap.AnalizeAll(board, figurs);
                        break;
                    case ConsoleKey.P:
                        ans += "The picture is saved";
                        RenderInScottPlot render = new RenderInScottPlot();
                        render.SavePict(board, new SettingsRender(), PathSave + "Image.png");
                        break;
                    case ConsoleKey.B:
                        ans += "The game is interrupted, to continue, press С";
                        breakProgram = true;
                        break;
                    case ConsoleKey.C:
                        breakProgram = false;
                        break;
                    default:
                        ans += "Invalid command";
                        break;
                }
                Console.WriteLine(ans);
            }
        }

        static void CriateMoreFig()
        {
            List<SettingsMap> bSettings = new List<SettingsMap>
            {
                new SettingsMap(4, 4, "0000\n0110\n0110\n0000\n", "cube"),
                new SettingsMap(5, 3, "00000\n01110\n00000\n", "stick"),
                new SettingsMap(3, 5, "000\n010\n010\n010\n000\n", "stick"),
                new SettingsMap(5, 5, "00000\n00100\n01010\n00100\n00000\n", "box")
            };

            SaveJson<List<SettingsMap>>.SaveToJson("C:\\Users\\mr_fi\\source\\repos\\mod-lab04-life\\Life\\Figurs.json", bSettings);
        }
    }
}