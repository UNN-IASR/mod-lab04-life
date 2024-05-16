using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

namespace cli_life
{
    class Program
    {
        static void Main(string[] args)
        {
            string solutionRootPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            
            GameOfLife gameOfLife = JsonReader.ReadSettings($"{solutionRootPath}/Life/Settings.json");
            gameOfLife.Reset($"{solutionRootPath}/States/Example1.txt");
            //gameOfLife.Reset();
            
            for(int i = 0; i < gameOfLife.iterations; i++)
            {
                Console.Clear();
                gameOfLife.Render();
                if (i < gameOfLife.iterations - 1) gameOfLife.board.Advance();
                Thread.Sleep(gameOfLife.delay);
            }
            gameOfLife.board.ExportState($"{solutionRootPath}/States");
        }
    }
}