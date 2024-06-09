using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace cli_life
{
    public class Json
    {
        static public Board Change_setting(string fileName)
        {
            Board board;
            string json_string = File.ReadAllText(fileName);

            var settings = JsonSerializer.Deserialize<Settings>(json_string);
            board = new Board(settings.Width, settings.Height, settings.CellSize, settings.LiveDensity);
            return board;
        }
    }
}
