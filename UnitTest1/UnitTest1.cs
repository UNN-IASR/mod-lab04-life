using cli_life;
using System.Text.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using NuGet.Frameworks;
namespace Tests1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestBoardGenerating()
        {
            Board board = new Board(50, 50, 1, 0.5);
            Assert.AreEqual(50, board.Height);
            Assert.AreEqual(50, board.Width);
        }

        [TestMethod]
        public void TestJsonSettings()
        {
            string fileName = "../../../../testSettings.json";
            string jsonString = File.ReadAllText(fileName);
            Settings settings = JsonSerializer.Deserialize<Settings>(jsonString);
            Assert.AreEqual(40, settings.Width);
            Assert.AreEqual(15, settings.Height);
            Assert.AreEqual(0.4, settings.liveDensity);
            Assert.AreEqual(2, settings.cellSize);
        }
        [TestMethod]
        public void TestFigureCube()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Upload("../../../../testCube.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure cube = fig[0];
            int count = Figure.FindFigure(cube, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestNoFigure()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Upload("../../../../testNoCube.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure cube = fig[0];
            int count = Figure.FindFigure(cube, board);
            Assert.AreEqual(count, 0);

        }
        [TestMethod]
        public void TestFigureRhombe()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("../../../../testRhombe.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure Rhombe = fig[1];
            int count = Figure.FindFigure(Rhombe, board);
            Assert.AreEqual(count, 1);

        }
        [TestMethod]
        public void TestFigureRing()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../testRing.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure Ring = fig[2];
            int count = Figure.FindFigure(Ring, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureShip()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("../../../../testShip.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure Ship = fig[3];
            int count = Figure.FindFigure(Ship, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureLeaf()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../testleaf.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure leaf = fig[4];
            int count = Figure.FindFigure(leaf, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureMany()
        {
            Board board = new Board(7, 4, 1, 1);
            board.Upload("../../../../testManyCube.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure cube = fig[0];
            int count = Figure.FindFigure(cube, board);
            Assert.AreEqual(count, 2);
        }
        [TestMethod]
        public void TestLivingCellsAmount()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("../../../../testAliveCells.txt");
            int count = board.CellsAliveCount();
            Assert.AreEqual(count, 5);
        }
        [TestMethod]
        public void TestNoLivingCell()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../testDeadCells.txt");
            int count = board.CellsAliveCount();
            Assert.AreEqual(count, 0);
        }
        [TestMethod]
        public void TestNoAliveCellAfterAdvance()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../testAdvance.txt");
            for (int i = 0; i < 2; i++)
            {
                board.Advance();
            }
            int count = board.CellsAliveCount();
            Assert.AreEqual(count, 0);
        }
        [TestMethod]
        public void TestFigureBarge()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../testbarge.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure barge = fig[5];
            int count = Figure.FindFigure(barge, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureFrigate()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("../../../../testFrigate.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure Frigate = fig[6];
            int count = Figure.FindFigure(Frigate, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestDifferentFigures()
        {
            Board board = new Board(10, 6, 1, 1);
            board.Upload("../../../../testFigures.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure cube = fig[0];
            Figure Ring = fig[2];
            int countCube = Figure.FindFigure(cube, board);
            int countRing = Figure.FindFigure(Ring, board);
            Assert.AreEqual(countCube, 1);
            Assert.AreEqual(countRing, 1);
        }
    }
}