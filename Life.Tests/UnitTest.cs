using cli_life;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;

namespace Life.Tests
{
    [TestClass]
    public class UnitTest
    {
        private static Board board;
        private static string fileState = "game_state4.txt";
        private static string fileStateFigures = "game_state5.txt";
        private static string fileNotStateFigures = "game_state6.txt";
        private static string fileSettings = "settings.json";

        [TestInitialize]
        public void Setup()
        {
            int width = 50;
            int height = 20;
            int cellSize = 1;
            double liveDensity = 0.5;
            board = new Board(width, height, cellSize, liveDensity);            
        }
        [TestMethod]
        public void BoardWidth()
        {
            Assert.AreEqual(50, board.Width);
        }
        [TestMethod]
        public void BoardHeight()
        {
            Assert.AreEqual(20, board.Height);
        }
        [TestMethod]
        public void BoardCells()
        {
            Assert.AreEqual(1000, board.Cells.Length);
        }
        [TestMethod]
        public void SettingsFromFile()
        {
            string json = File.ReadAllText(fileSettings);
            Settings settings = JsonConvert.DeserializeObject<Settings>(json);
            Program.Reset(settings, out board);
            Assert.AreEqual(settings.Width, board.Width);
            Assert.AreEqual(settings.Height, board.Height);
            Assert.AreEqual(settings.CellSize, board.CellSize);
        }
        [TestMethod]
        public void SettingsFromFileGameState()
        {
            Settings settings;
            FileHandler.LoadFromFile(fileState, out board, out settings);
            Assert.AreEqual(settings.Width, 50);
            Assert.AreEqual(settings.Height, 20);
            Assert.AreEqual(settings.CellSize, 1);
            Assert.AreEqual(settings.LiveDensity, 0.5);
        }
        [TestMethod]
        public void BoardFromFileGameState()
        {
            Settings settings;
            FileHandler.LoadFromFile(fileState, out board, out settings);
            Assert.AreEqual(settings.Width, board.Width);
            Assert.AreEqual(settings.Height, board.Height);
            Assert.AreEqual(settings.CellSize, board.CellSize);
        }
        [TestMethod]
        public void CellCountNeighborhood()
        {
            Cell cell = board.Cells[5, 5];
            Assert.AreEqual(8, cell.neighbors.Count);
        }
        [TestMethod]
        public void BoardAdvanceTest()
        {
            board.Cells[5, 5].IsAlive = true;
            bool initialState = board.Cells[5, 5].IsAlive;
            board.Advance();
            bool nextState = board.Cells[5, 5].IsAlive;
            Assert.AreNotEqual(initialState, nextState);
        }
        [TestMethod]
        public void FieldAnalyzerCountAliveCells()
        {
            FileHandler.LoadFromFile(fileState, out board, out _);
            int actualAliveCount = FieldAnalyzer.CountAliveCells(board);

            Assert.AreEqual(4, actualAliveCount);
        }

        [TestMethod]
        public void FieldAnalyzerAliveCells()
        {
            FileHandler.LoadFromFile(fileState, out board, out _);
            Assert.AreEqual(true, board.Cells[21, 9].IsAlive);
            Assert.AreEqual(true, board.Cells[22, 9].IsAlive);
            Assert.AreEqual(true, board.Cells[21, 10].IsAlive);
            Assert.AreEqual(true, board.Cells[22, 10].IsAlive);

        }
        [TestMethod]
        public void CombinationCount()
        {
            FileHandler.LoadFromFile(fileState, out board, out _);
            var (combinationCount, _) = FieldAnalyzer.CountCombinations(board);
            Assert.AreEqual(1, combinationCount);
        }
        [TestMethod]
        public void Block()
        {
            FileHandler.LoadFromFile(fileState, out board, out _);
            var (_, classification) = FieldAnalyzer.CountCombinations(board);
            Assert.AreEqual(1, classification["Stable"]["Block"]);
        }
        [TestMethod]
        public void Figures()
        {
            FileHandler.LoadFromFile(fileStateFigures, out board, out _);
            var (combinationCount, classification) = FieldAnalyzer.CountCombinations(board);
            Assert.AreEqual(8, combinationCount);
            Assert.AreEqual(1, classification["Stable"]["Block"]);
            Assert.AreEqual(2, classification["Stable"]["Beehive"]);
            Assert.AreEqual(2, classification["Stable"]["Box"]);
            Assert.AreEqual(1, classification["Stable"]["Pond"]);
            Assert.AreEqual(2, classification["Stable"]["Snake"]);
        }
        [TestMethod]
        public void InitStableGame()
        {
            FileHandler.LoadFromFile(fileStateFigures, out board, out _);
            FieldAnalyzer fieldAnalyzer = new FieldAnalyzer();
            for (int i = 1; i < 20; i++)
            {
                fieldAnalyzer.AddCountStableState();
                board.Advance();
            }
            Assert.AreEqual(0, fieldAnalyzer.CountStableState - 19);
        }
        [TestMethod]
        public void StableGame()
        {
            FileHandler.LoadFromFile(fileNotStateFigures, out board, out _);
            FieldAnalyzer fieldAnalyzer = new FieldAnalyzer();
            QueueDensity queueDensity = new QueueDensity(20);
            while (true)
            {
                int countAliveCells = FieldAnalyzer.CountAliveCells(board);
                if (FieldAnalyzer.IsStable(queueDensity))
                {
                    break;
                }
                else
                {
                    fieldAnalyzer.AddCountStableState();
                    queueDensity.AddDensity(countAliveCells);
                }

                board.Advance();
            }
            Assert.AreEqual(7, fieldAnalyzer.CountStableState - (queueDensity.MaxSize - 1));
        }
    }
}