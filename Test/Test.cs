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
        public void TestBoardGeneration1()
        {
            var gameBoard = new Board(10, 10, 1, 0.3);
            Assert.AreEqual(10, gameBoard.Height);
            Assert.AreEqual(10, gameBoard.Width);
        }

        // Тест проверяет корректность чтения настроек из файла JSON.
        [TestMethod]
        public void TestJsonSettingsDeserialization2()
        {
            string settingsPath = "Settings.json";
            string jsonContent = File.ReadAllText(settingsPath);
            Settings gameSettings = JsonSerializer.Deserialize<Settings>(jsonContent);
            Assert.AreEqual(50, gameSettings.Width);
            Assert.AreEqual(50, gameSettings.Height);
            Assert.AreEqual(1, gameSettings.cellSize);
            Assert.AreEqual(0.5, gameSettings.liveDensity);
        }

        // Тест проверяет корректность сохранения доски в файл
        [TestMethod]
        public void TestBoardStateSerialization3()
        {
            // Создаем доску и сохраняем ее состояние в файл.
            var board = new Board(5, 5, 1, 0.1);
            board.LoadBoardState("testBoardState.txt");
            // Проверяем, что состояние доски было корректно сохранено.
            Assert.IsTrue(File.Exists("testBoardState.txt"));
        }

        [TestMethod]
        public void TestFigureLoading4()
        {
            // Загружаем фигуры из файла и проверяем, что они были корректно загружены.
            var figures = Figure.GetFigure("figures.json");
            Assert.AreEqual(5, figures.Length);
        }

        // Тест проверяет корректность загрузки и поиска фигуры cube.
        [TestMethod]
        public void TestCubeFigureDetection5()
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
        public void TestBargeFigureDetection6()
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
        public void TestNoCubeFigure7()
        {
            var gameBoard = new Board(7, 7, 1, 0.5);
            gameBoard.Upload("ship.txt");
            Figure[] figures = Figure.GetFigures("figures.json");
            Figure cubeFigure = figures[0];
            int cubeCount = Figure.FindFigures(cubeFigure, gameBoard);
            Assert.AreEqual(0, cubeCount);
        }

        [TestMethod]
        public void TestRandomize8()
        {
            // Создаем доску и инициализируем ее случайным образом.
            var board = new Board(5, 5, 1, 0.1);
            board.Randomize(0.1);
            // Проверяем, что количество живых клеток соответствует ожидаемому.
            Assert.AreEqual(5 * 5 * 0.1, board.СellAliveCount());
        }

        [TestMethod]
        public void TestConnectNeighbors9()
        {
            // Создаем доску и проверяем, что соседние клетки были корректно подключены.
            var board = new Board(5, 5, 1, 0.1);
            Assert.AreEqual(25, board.Cells[0, 0].neighbors.Count);
        }

        [TestMethod]
        public void TestGraficGenerateList10()
        {
            // Проверяем, что метод Grafic.GenerateList возвращает корректный список словарей.
            var resultList = Grafic.GenerateList(new List<double> { 0.4, 0.6, 0.8 }, 3);
            Assert.AreEqual(3, resultList.Count);
        }

        [TestMethod]
        public void TestBoardStateFromFile11()
        {
            // Создаем доску и сохраняем ее состояние в файл.
            var board = new Board(5, 5, 1, 0.1);
            board.Randomize(0.1);
            board.SaveBoardState("testBoardState.txt");
            // Загружаем состояние доски из файла.
            var loadedBoard = new Board(5, 5, 1, 0.1);
            loadedBoard.LoadBoardState("testBoardState.txt");
            // Проверяем, что состояние загруженной доски соответствует сохраненному.
            Assert.AreEqual(board.СellAliveCount(), loadedBoard.СellAliveCount());
        }

        // Тест проверяет корректность загрузки и поиска фигуры frigate
        [TestMethod]
        public void TestFrigateFigureDetection12()
        {
            var gameBoard = new Board(7, 7, 1, 0.7);
            gameBoard.Upload("frigate.txt");
            Figure[] figures = Figure.GetFigures("figures.json");
            Figure ringFigure = figures[2];
            int ringCount = Figure.FindFigures(ringFigure, gameBoard);
            Assert.AreEqual(1, ringCount);
        }

        // Тест проверяет корректность загрузки и поиска фигуры leaf
        [TestMethod]
        public void TestLeafFigureDetection13()
        {
            var gameBoard = new Board(8, 8, 1, 0.7);
            gameBoard.Upload("leaf.txt");
            Figure[] figures = Figure.GetFigures("figures.json");
            Figure ringFigure = figures[2];
            int ringCount = Figure.FindFigures(ringFigure, gameBoard);
            Assert.AreEqual(1, ringCount);
        }

        // Тест проверяет корректность загрузки и поиска фигуры ring
        [TestMethod]
        public void TestRingFigureDetection14()
        {
            var gameBoard = new Board(8, 8, 1, 0.7);
            gameBoard.Upload("ring.txt");
            Figure[] figures = Figure.GetFigures("figures.json");
            Figure ringFigure = figures[2];
            int ringCount = Figure.FindFigures(ringFigure, gameBoard);
            Assert.AreEqual(1, ringCount);
        }

        // Тест проверяет отсутствие фигуры romb
        [TestMethod]
        public void TestNoRombFigure15()
        {
            var gameBoard = new Board(7, 7, 1, 0.5);
            gameBoard.Upload("romb.txt");
            Figure[] figures = Figure.GetFigures("figures.json");
            Figure cubeFigure = figures[0];
            int cubeCount = Figure.FindFigures(cubeFigure, gameBoard);
            Assert.AreEqual(0, cubeCount);
        }
    }
}
