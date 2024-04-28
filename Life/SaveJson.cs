using System.IO;
using System.Text.Json;

namespace Life
{
    public static class SaveJson
    {
        public class SettingsMap
        {
            public string Name { get; init; }
            public int Columns { get; init; }
            public int Rows { get; init; }
            public string BoardStr { get; init; }
        }

        public static void SaveToJson(string path, string name, MapCGL map)
        {
            if (File.Exists(path))
                File.Delete(path);
            else
                File.Create(path).Close();

            var options = new JsonSerializerOptions { WriteIndented = true };

            SettingsMap settingsMap = new SettingsMap()
            {
                Columns = map.Columns,
                Rows = map.Rows,
                BoardStr = map.ToString(),
                Name = name
            };

            string jsonStr = JsonSerializer.Serialize(settingsMap, options: options);
            File.AppendAllText(path, jsonStr);
        }

        public static MapCGL LoadFromJSon(string path)
        {
            string jsonStr = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<SettingsMap>(jsonStr);
            return loadMapFromStr(settings);
        }

        private static MapCGL loadMapFromStr(SettingsMap settings)
        {
            string[] strLines = settings.BoardStr.Split('\n');
            MapCGL map = new MapCGL(strLines[0].Length, strLines.Length - 1);

            for (int y = 0; y < strLines.Length; y++)
                for (int x = 0; x < strLines[y].Length; x++)
                    map.Cells[x, y].IsAlive = strLines[y][x] == '0' ? false : true;

            return map;
        }
    }


}
