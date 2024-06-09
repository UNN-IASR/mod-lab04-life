using cli_life;
using System.Text.Json;
using System.Net;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestBoar()
        {
            Board board = new Board(75, 75, 1, 0.5);
            Assert.AreEqual(75, board.Height);
        }

        [TestMethod]
        public void TestBoardGen()
        {
            Board board = new Board(99, 99, 1, 0.5);
            Assert.AreEqual(99, board.Width);
        }

        [TestMethod]
        public void TestBoardGene()
        {
            Board board = new Board(101, 101, 1, 0.5);
            Assert.AreEqual(101, board.Height);
            Assert.AreEqual(101, board.Width);
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
            string fname = "../../../../Life/configuration.json";
            string jstr = File.ReadAllText(fname);
            Setting settings = JsonSerializer.Deserialize<Setting>(jstr);
            Assert.AreEqual(0, settings.Width);
        }
        
        [TestMethod]
        public void TestJsonS()
        {
            string fname = "../../../../Life/configuration.json";
            string jstr = File.ReadAllText(fname);
            Setting settings = JsonSerializer.Deserialize<Setting>(jstr);
            Assert.AreEqual(0, settings.Height);
        }
        
        [TestMethod]
        public void TestFigureCube()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Upload("../../../../figure_3.txt");
            Figure[] fig = Figure.get_figure("../../../../Life/figures.json");
            Figure cube = fig[0];
            int count = Figure.find_figure(cube, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestNoFigure()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Upload("../../../../figure_7.txt");
            Figure[] fig = Figure.get_figure("../../../../Life/figures.json");
            Figure cube = fig[0];
            int count = Figure.find_figure(cube, board);
            Assert.AreEqual(count, 0);

        }
        [TestMethod]
        public void TestFigureRhombe()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("../../../../romb.txt");
            Figure[] fig = Figure.get_figure("../../../../Life/figures.json");
            Figure Rhombe = fig[1];
            int count = Figure.find_figure(Rhombe, board);
            Assert.AreEqual(count, 1);

        }
        [TestMethod]
        public void TestFigureRing()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../figure_8.txt");
            Figure[] fig = Figure.get_figure("../../../../Life/figures.json");
            Figure Ring = fig[2];
            int count = Figure.find_figure(Ring, board);
            Assert.AreEqual(count, 0);
        }
        [TestMethod]
        public void TestFigureShip()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("../../../../figure_9.txt");
            Figure[] fig = Figure.get_figure("../../../../Life/figures.json");
            Figure Ship = fig[3];
            int count = Figure.find_figure(Ship, board);
            Assert.AreEqual(count, 0);
        }
        [TestMethod]
        public void TestFigureLeaf()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../figure_5.txt");
            Figure[] fig = Figure.get_figure("../../../../Life/figures.json");
            Figure leaf = fig[4];
            int count = Figure.find_figure(leaf, board);
            Assert.AreEqual(count, 0);
        }
        [TestMethod]
        public void TestFigureBarge()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("../../../../figure_2.txt");
            Figure[] fig = Figure.get_figure("../../../../Life/figures.json");
            Figure barge = fig[5];
            int count = Figure.find_figure(barge, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureFrigate()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("../../../../figure_4.txt");
            Figure[] fig = Figure.get_figure("../../../../Life/figures.json");
            Figure Frigate = fig[6];
            int count = Figure.find_figure(Frigate, board);
            Assert.AreEqual(count, 0);
        }
        [TestMethod]
        public void TestFigureFrig()
        {
            Board bo = new Board(6, 6, 1, 1);
            bo.Upload("../../../../figure_4.txt");
            Figure[] fiп = Figure.get_figure("../../../../Life/figures.json");
            Figure Frig = fig[6];
            int co = Figure.find_figure(Frig, bo);
            Assert.AreEqual(co, 0);
        }
    }
}
