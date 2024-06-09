using cli_life;
using System.Text.Json;
using System.Net;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestBoard()
        {
            Board board = new Board(70, 70, 1, 0.5);
            Assert.AreEqual(70, board.Height);
        }
        
        [TestMethod]
        public void TestBoardGen()
        {
            Board board = new Board(80, 80, 1, 0.5);
            Assert.AreEqual(80, board.Width);
        }
        
        [TestMethod]
        public void TestBoardGener()
        {
            Board board = new Board(100, 100, 1, 0.5);
            Assert.AreEqual(100, board.Height);
            Assert.AreEqual(100, board.Width);
        }

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
            string fileName = "../../../../Life/config.json";
            string jsonString = File.ReadAllText(fileName);
            Setting settings = JsonSerializer.Deserialize<Setting>(jsonString);
            Assert.AreEqual(0, settings.Height);
        }
        
        [TestMethod]
        public void TestJsonSet()
        {
            string fileName = "../../../../Life/config.json";
            string jsonString = File.ReadAllText(fileName);
            Setting settings = JsonSerializer.Deserialize<Setting>(jsonString);
            Assert.AreEqual(0, settings.Width);
        }
        
        [TestMethod]
        public void TestFigure()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Upload("../../../../cube.txt");
            Figure[] fig = Figure.getFig("../../../../Life/fig.json");
            Figure cube = fig[0];
            int count = Figure.findFig(cube, board);
            Assert.AreEqual(count, 1);
        }
        
        [TestMethod]
        public void TestFigureCube()
        {
            Board bo = new Board(4, 4, 1, 1);
            bo.Upload("../../../../cube.txt");
            Figure[] fig = Figure.getFig("../../../../Life/fig.json");
            Figure cub = fig[0];
            int co = Figure.findFig(cub, bo);
            Assert.AreEqual(co, 1);
        }
        
        [TestMethod]
        public void TestNo()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Upload("../../../../no.txt");
            Figure[] fig = Figure.getFig("../../../../Life/fig.json");
            Figure cube = fig[0];
            int count = Figure.findFig(cube, board);
            Assert.AreEqual(count, 0);
        }
        
        [TestMethod]
        public void TestFigureRing()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../ring.txt");
            Figure[] fig = Figure.getFig("../../../../Life/fig.json");
            Figure Ring = fig[2];
            int count = Figure.findFig(Ring, board);
            Assert.AreEqual(count, 0);
        }
        
        [TestMethod]
        public void TestFigureR()
        {
            Board bo = new Board(6, 6, 1, 1);
            bo.Upload("../../../../ring.txt");
            Figure[] fi = Figure.getFig("../../../../Life/fig.json");
            Figure ring = fig[2];
            int co = Figure.findFig(ring, bo);
            Assert.AreEqual(co, 0);
        }
        
        [TestMethod]
        public void TestFigureShip()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("../../../../ship.txt");
            Figure[] fig = Figure.getFig("../../../../Life/fig.json");
            Figure Ship = fig[3];
            int count = Figure.findFig(Ship, board);
            Assert.AreEqual(count, 0);
        }
        [TestMethod]
        public void TestFigureLeaf()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../leaf.txt");
            Figure[] fig = Figure.getFig("../../../../Life/fig.json");
            Figure leaf = fig[4];
            int count = Figure.findFig(leaf, board);
            Assert.AreEqual(count, 0);
        }
        [TestMethod]
        public void TestFigureBarge()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../barge.txt");
            Figure[] fig = Figure.getFig("../../../../Life/fig.json");
            Figure barge = fig[5];
            int count = Figure.findFig(barge, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureFrigate()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("../../../../frigate.txt");
            Figure[] fig = Figure.getFig("../../../../Life/fig.json");
            Figure Frigate = fig[6];
            int count = Figure.findFig(Frigate, board);
            Assert.AreEqual(count, 0);
        }
    }
}
