using System.IO;
using System.Text.Json;

namespace cli_life
{
    public static class JsonReader
    {
        public static GameOfLife ReadSettings(string pathToSettings)
        {
            string rawContents = File.ReadAllText(@pathToSettings);
            GameOfLife contents = JsonSerializer.Deserialize<GameOfLife>(rawContents);
            return contents;
        }
    }
}