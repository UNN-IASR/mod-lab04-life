using System.IO;
using System.Text.Json;

namespace Life
{
    public static class SaveJson<T>
    {
        public static void SaveToJson(string path, T obj)
        {
            if (!File.Exists(path))
                File.Create(path).Close();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string jsonStr = JsonSerializer.Serialize(obj, options: options);
            File.AppendAllText(path, jsonStr);
        }

        public static T LoadFromJSon(string path)
        {
            string jsonStr = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(jsonStr);
        }
    }
}