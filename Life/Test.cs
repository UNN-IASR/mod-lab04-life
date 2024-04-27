using cli_life;
using System.Text.Json;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            string fileName = "Settings.json";
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
            board.Upload("cube.txt");
            Figure[] fig = Figure.getFig("fig.json");
            Figure cube = fig[0];
            int count = Figure.findFig(cube, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestNoFigure()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Upload("No.txt");
            Figure[] fig = Figure.getFig("fig.json");
            Figure cube = fig[0];
            int count = Figure.findFig(cube, board);
            Assert.AreEqual(count, 0);

        }
        [TestMethod]
        public void TestFigureRhombe()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("romb.txt");
            Figure[] fig = Figure.getFig("fig.json");
            Figure Rhombe = fig[1];
            int count = Figure.findFig(Rhombe, board);
            Assert.AreEqual(count, 1);

        }
        [TestMethod]
        public void TestFigureRing()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("ring.txt");
            Figure[] fig = Figure.getFig("fig.json");
            Figure Ring = fig[2];
            int count = Figure.findFig(Ring, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureShip()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("ship.txt");
            Figure[] fig = Figure.getFig("fig.json");
            Figure Ship = fig[3];
            int count = Figure.findFig(Ship, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureLeaf()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("leaf.txt");
            Figure[] fig = Figure.getFig("fig.json");
            Figure leaf = fig[4];
            int count = Figure.findFig(leaf, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureMany()
        {
            Board board = new Board(7, 4, 1, 1);
            board.Upload("many.txt");
            Figure[] fig = Figure.getFig("fig.json");
            Figure cube = fig[0];
            int count = Figure.findFig(cube, board);
            Assert.AreEqual(count, 2);
        }
        [TestMethod]
        public void TestFigureBarge()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("barge.txt");
            Figure[] fig = Figure.getFig("fig.json");
            Figure barge = fig[5];
            int count = Figure.findFig(barge, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureFrigate()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("frigate.txt");
            Figure[] fig = Figure.getFig("fig.json");
            Figure Frigate = fig[6];
            int count = Figure.findFig(Frigate, board);
            Assert.AreEqual(count, 1);
        }
    }
}
