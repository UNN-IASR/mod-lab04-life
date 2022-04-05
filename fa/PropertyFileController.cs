using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace PropertyFileController
{
    public class LifeProperty
    {
        public LifeProperty() {}

        public LifeProperty(int BoardWidth, int BoardHeight, int BoardCellSize, double LifeDensity)
        {
            this.BoardWidth = BoardWidth;
            this.BoardHeight = BoardHeight;
            this.BoardCellSize = BoardCellSize;
            this.LifeDensity = LifeDensity;
        }
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public int BoardCellSize { get; set; }
        public double LifeDensity { get; set; }
    }

    public class JSONController
    {
        public static void SerializeToJSON(LifeProperty theLifeProperty, string theFileName = "LifeProperty.json")
        {
            string jsonString = JsonSerializer.Serialize(theLifeProperty);
            File.WriteAllText(theFileName, jsonString);
        }

        public static LifeProperty DeserializeFromJSON(string theFileName = "LifeProperty.json")
        {
            string jsonString = File.ReadAllText(theFileName);

            return JsonSerializer.Deserialize<LifeProperty>(jsonString)!;
        }
    }
}