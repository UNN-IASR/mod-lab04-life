using cli_life;
using System.Text.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using NuGet.Frameworks;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void BoardCreate()
        {
            Board b = new Board(40, 40, 1, 0.5);
            Assert.AreEqual(40, b.Width);
            Assert.AreEqual(40, b.Height);
        }
        [TestMethod]
        public void BoardAdvance()
        {
            Board b = new Board(40, 40, 1, 0.5);
            b.Advance();
            b.Advance();
            b.Advance();
            Assert.AreEqual(3, b.gens.Count);
        }
        [TestMethod]
        public void BoardSaveTo()
        {
            Board b = new Board(40, 40, 1, 0.4);
            b.SaveToFile("savetest.txt");
            Assert.IsTrue(File.Exists("savetest.txt"));
        }
        [TestMethod]
        public void BoardLoadFrom()
        {
            Board b = Board.LoadFromFile("../../../research_board.txt");
            Assert.IsTrue(b.living_cells() > 0);
        }
        [TestMethod]
        public void ResetBoardFrom()
        {
            Board b = Board.ResetFromFile("../../../settings.json");
            Assert.AreEqual(40, b.Width);
            Assert.AreEqual(50, b.Height);
        }
        [TestMethod]
        public void AliveCheck1()
        {
            Board b = Board.LoadFromFile("../../../loadboard-2.txt");
            int alive = b.living_cells();
            Assert.AreEqual(alive, 20);
        }
        [TestMethod]
        public void AliveCheck2()
        {
            Board b = Board.LoadFromFile("../../../loadboard-3.txt");
            int alive = b.living_cells();
            Assert.AreEqual(alive, 21);
        }
        [TestMethod]
        public void TestCube()
        {
            Board board = new Board(4, 4, 1, 1);
            board = Board.LoadFromFile("../../../../cube.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure cube = fig[0];
            int count = Figure.FindFigure(cube, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestRing()
        {
            Board board = new Board(6, 6, 1, 1);
            board = Board.LoadFromFile("../../../../ring.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure ring = fig[2];
            int count = Figure.FindFigure(ring, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestShip()
        {
            Board board = new Board(5, 5, 1, 1);
            board = Board.LoadFromFile("../../../../ship.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure ship = fig[3];
            int count = Figure.FindFigure(ship, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestRhombe()
        {
            Board board = new Board(5, 5, 1, 1);
            board = Board.LoadFromFile("../../../../rhombe.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure rhomb = fig[1];
            int count = Figure.FindFigure(rhomb, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFrigate()
        {
            Board board = new Board(5, 5, 1, 1);
            board = Board.LoadFromFile("../../../../frigate.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure frigate = fig[6];
            int count = Figure.FindFigure(frigate, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestLeaf()
        {
            Board board = new Board(6, 6, 1, 1);
            board = Board.LoadFromFile("../../../../leaf.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure leaf = fig[4];
            int count = Figure.FindFigure(leaf, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestBarge()
        {
            Board board = new Board(6, 6, 1, 1);
            board = Board.LoadFromFile("../../../../barge.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure barge = fig[5];
            int count = Figure.FindFigure(barge, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestNoRing()
        {
            Board board = new Board(6, 6, 1, 1);
            board = Board.LoadFromFile("../../../../no_fig.txt");
            Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
            Figure notaring = fig[2];
            int count = Figure.FindFigure(notaring, board);
            Assert.AreEqual(count, 0);
        }
    }
}
