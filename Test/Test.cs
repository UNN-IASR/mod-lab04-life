using cli_life;
using System.Text.Json;
using System.Net;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        // Тест проверяет корректность генерации доски с заданными параметрами.
        [TestMethod]
        public void TestBoardGeneration()
        {
            var gameBoard = new Board(10, 10, 2, 0.3);
            Assert.AreEqual(10, gameBoard.Height);
            Assert.AreEqual(10, gameBoard.Width);
        }

        // Тест проверяет корректность чтения настроек из файла JSON.
        [TestMethod]
        public void TestJsonSettingsDeserialization()
        {
            string settingsPath = "Settings.json";
            string jsonContent = File.ReadAllText(settingsPath);
            Settings gameSettings = JsonSerializer.Deserialize<Settings>(jsonContent);
            Assert.AreEqual(50, gameSettings.Width);
            Assert.AreEqual(50, gameSettings.Height);
            Assert.AreEqual(1, gameSettings.cellSize);
            Assert.AreEqual(0.5, gameSettings.liveDensity);
        }

        // Тест проверяет корректность загрузки и поиска фигуры cube.
        [TestMethod]
        public void TestCubeFigureDetection()
        {
            var gameBoard = new Board(6, 6, 1, 0.5);
            gameBoard.Upload("cube.txt");
            Figure[] figures = Figure.GetFigures("figures.json");
            Figure cubeFigure = figures[0];
            int cubeCount = Figure.FindFigures(cubeFigure, gameBoard);
            Assert.AreEqual(1, cubeCount);
        }

        // Тест проверяет корректность загрузки и поиска фигуры barge
        [TestMethod]
        public void TestBargeFigureDetection()
        {
            var gameBoard = new Board(8, 8, 1, 0.7);
            gameBoard.Upload("barge.txt");
            Figure[] figures = Figure.GetFigures("figures.json");
            Figure ringFigure = figures[2];
            int ringCount = Figure.FindFigures(ringFigure, gameBoard);
            Assert.AreEqual(1, ringCount);
        }

        // Тест проверяет отсутствие фигуры ship
        [TestMethod]
        public void TestNoCubeFigure()
        {
            var gameBoard = new Board(7, 7, 1, 0.5);
            gameBoard.Upload("ship.txt");
            Figure[] figures = Figure.GetFigures("figures.json");
            Figure cubeFigure = figures[0];
            int cubeCount = Figure.FindFigures(cubeFigure, gameBoard);
            Assert.AreEqual(0, cubeCount);
        }
    }
}
