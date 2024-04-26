using System;

namespace Life
{
    public class SettingsMap
    {
        public string Name { get; }
        public int Colums { get; }
        public int Rows { get; }
        public string BoardStr { get; private set; }

        public SettingsMap(int colums = 50, int rows = 20, string boardStr = null, string name = null)
        {
            Colums = colums;
            Rows = rows;
            Name = name;
            if (boardStr != null)
                SetBoardStr(boardStr);
        }

        private void SetBoardStr(string boardStr)
        {
            string[] strRows = boardStr.Split('\n');

            if (strRows.Length - 1 != Rows)
                throw new ArgumentException($"The number of rows in BoardStr " +
                    $"{strRows.Length} must be equal to Rows {Rows}");

            for (int i = 0; i < strRows.Length - 1; i++)
                if (strRows[i].Length != Colums)
                    throw new ArgumentException($"The number of colums in BoardStr" +
                        $" {strRows[i].Length} must be equal to Colums {Colums}");

            BoardStr = boardStr;
        }
    }
}