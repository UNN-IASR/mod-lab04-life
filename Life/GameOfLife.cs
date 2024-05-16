using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace cli_life
{
    public class GameOfLife
    {
        public int boardWidth { get; set; }
        public int boardHeight { get; set; }
        public double liveDensity { get; set; }
        public string liveSymbol { get; set; }
        public string deadSymbol { get; set; }
        public int iterations { get; set; }
        public int delay { get; set; }
        
        public Board board { get; set; }
        
        public void Reset(string pathToState = null)
        {
            board = new Board(
                width: boardWidth,
                height: boardHeight,
                liveDensity: liveDensity,
                pathToState: pathToState);
        }
        
        public void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write(liveSymbol);
                    }
                    else
                    {
                        Console.Write(deadSymbol);
                    }
                }
                Console.Write('\n');
            }
        }
        
        public override bool Equals(object obj)
        {
            if (obj as GameOfLife == null)
                return false;
            GameOfLife evaluated = (GameOfLife) obj;
            if (evaluated.boardHeight == boardHeight &&
                evaluated.boardWidth == boardWidth &&
                evaluated.liveDensity == liveDensity &&
                evaluated.liveSymbol == liveSymbol &&
                evaluated.deadSymbol == deadSymbol &&
                evaluated.iterations == iterations &&
                evaluated.delay == delay)
                return true;
            else
                return false;
        }
    }
}