using cli_life;
using System;
using System.Threading;

namespace Life
{
    public class RenderToConsole : IRender
    {
        public Board Board { get; }
        public SettingsRender Settings { get; }

        public RenderToConsole(Board board, SettingsRender settings)
        {
            Board = board;
            Settings = settings;
        }


        public void Render()
        {
            for (int i = 0; i < Settings.MaxIteration; i++)
            {
                Console.Clear();
                RenderStep();
                Board.Advance();
                Thread.Sleep(Settings.TimeDelay);
            }
        }

        public void RenderStep()
        {
            for (int row = 0; row < Board.Rows; row++)
            {
                for (int col = 0; col < Board.Colums; col++)
                {
                    var cell = Board.Cells[col, row];
                    if (cell.IsAlive)
                        Console.Write(Settings.SymbLive);
                    else
                        Console.Write(Settings.SymbDead);
                }
                Console.Write('\n');
            }
        }
    }
}