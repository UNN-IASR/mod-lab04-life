using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            /*
             * Если поле изначально пустое, то количество живых ячеек
             * на нём равно нулю
             */

            var board = new Board(
                width: 50,
                height: 20,
                cellSize: 1,
                liveDensity: 0);

            var result = Program.ElementsAmount(board);
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var board = new Board(
                width: 50,
                height: 20,
                cellSize: 1,
                liveDensity: 0);

            for (int y = 0; y < board.Rows; y++)
            {
                board.Cells[0, y].IsAlive = true;
            }

            var result = Program.ElementsAmount(board);
            Assert.AreEqual(result, board.Rows);
        }

        [TestMethod]
        public void TestMethod3()
        {
            /*
             * Если поле изначально пустое, то стабильное состояние достигается
             * на первом поколении
             */

            int previousAmount = 0;
            int numberOfGenerations = 0;

            Program.Reset();

            int sameAmountCounter = 0;
            int generation = 1;

            var board = new Board(
            width: 50,
            height: 20,
            cellSize: 1,
            liveDensity: 0);

            while (sameAmountCounter < 10)
            {
                if (Program.ElementsAmount(board) == previousAmount) sameAmountCounter++;
                else sameAmountCounter = 0;

                previousAmount = Program.ElementsAmount(board);
                generation++;

                board.Advance();
            }
            numberOfGenerations = generation - 10;

            Assert.AreEqual(numberOfGenerations, 1);
        }

        [TestMethod]
        public void TestMethod4()
        {
            //Проверка json-файла с настройками

            var board = new Board(
            width: 50,
            height: 20,
            cellSize: 1,
            liveDensity: 0);

            Program.SaveSettings("settings_test.json", board);
            Assert.IsTrue(File.Exists("settings_test.json"));
            Assert.IsNotNull(File.ReadAllText("settings_test.json"));
        }


        [TestMethod]
        public void TestMethod5()
        {
            //Проверка json-файла с сохранённой выгрузкой

            int previousAmount = 0;

            Program.Reset();

            int sameAmountCounter = 0;
            int generation = 1;

            var board = new Board(
            width: 50,
            height: 20,
            cellSize: 1,
            liveDensity: 0);

            while (sameAmountCounter < 10)
            {
                if (Program.ElementsAmount(board) == previousAmount) sameAmountCounter++;
                else sameAmountCounter = 0;

                previousAmount = Program.ElementsAmount(board);
                generation++;

                board.Advance();
            }

            Program.SaveResult("unloading_test.json");
            Assert.IsTrue(File.Exists("unloading_test.json"));
            Assert.IsNotNull(File.ReadAllBytes("unloading_test.json"));
        }

        [TestMethod]
        public void TestMethod6()
        {
            /*
             * Создаём json-файл с готовыми настройками, идентичный файлу ready_settings1.json
             * и проверяем, верно ли присваиваются значения полям класса Board
             */

            File.WriteAllText("ready_settings1.json", "{\"width\":50,\"height\":20,\"cellSize\":1,\"liveDensity\":0.1}");
            Board b1 = JsonConvert.DeserializeObject<Board>(File.ReadAllText("ready_settings1.json"))!;
            Assert.AreEqual(b1.Width, 50);
            Assert.AreEqual(b1.Height, 20);
            Assert.AreEqual(b1.CellSize, 1);
        }

        [TestMethod]
        public void TestMethod7()
        {
            /*
             * Создаём json-файл с готовыми настройками, идентичный файлу ready_settings2.json
             * и проверяем, верно ли присваиваются значения полям класса Board
             */

            File.WriteAllText("ready_settings2.json", "{\"width\":50,\"height\":20,\"cellSize\":1,\"liveDensity\":0.2}");
            Board b2 = JsonConvert.DeserializeObject<Board>(File.ReadAllText("ready_settings2.json"))!;
            Assert.AreEqual(b2.Width, 50);
            Assert.AreEqual(b2.Height, 20);
            Assert.AreEqual(b2.CellSize, 1);
        }

        [TestMethod]
        public void TestMethod8()
        {
            /*
             * Создаём json-файл с готовыми настройками, идентичный файлу ready_settings3.json
             * и проверяем, верно ли присваиваются значения полям класса Board
             */

            File.WriteAllText("ready_settings3.json", "{\"width\":50,\"height\":20,\"cellSize\":1,\"liveDensity\":0.3}");
            Board b3 = JsonConvert.DeserializeObject<Board>(File.ReadAllText("ready_settings3.json"))!;
            Assert.AreEqual(b3.Width, 50);
            Assert.AreEqual(b3.Height, 20);
            Assert.AreEqual(b3.CellSize, 1);
        }


        [TestMethod]
        public void TestMethod9()
        {
            /*
             * Создаём json-файл с готовыми настройками, идентичный файлу ready_settings3.json
             * и проверяем, верно ли присваиваются значения полям класса Board
             */

            File.WriteAllText("ready_settings4.json", "{\"width\":50,\"height\":20,\"cellSize\":1,\"liveDensity\":0.4}");
            Board b4 = JsonConvert.DeserializeObject<Board>(File.ReadAllText("ready_settings4.json"))!;
            Assert.AreEqual(b4.Width, 50);
            Assert.AreEqual(b4.Height, 20);
            Assert.AreEqual(b4.CellSize, 1);
        }
    }
}