using cli_life;
using System.Text.Json;
using System.Net;

namespace Test
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
            string fname = "Settings.json";
            string jstr = File.ReadAllText(fname);
            Settings settings = JsonSerializer.Deserialize<Settings>(jstr);
            Assert.AreEqual(40, settings.Width);
            Assert.AreEqual(15, settings.Height);
            Assert.AreEqual(0.4, settings.liveDensity);
            Assert.AreEqual(2, settings.cellSize);
        }
        [TestMethod]
        public void TestFigureCube()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Upload("figure_3.txt");
            Figure[] fig = Figure.get_figure("figures.json");
            Figure cube = fig[0];
            int count = Figure.find_figure(cube, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestNoFigure()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Upload("figure_7.txt");
            Figure[] fig = Figure.get_figure("figures.json");
            Figure cube = fig[0];
            int count = Figure.find_figure(cube, board);
            Assert.AreEqual(count, 0);

        }
        [TestMethod]
        public void TestFigureRhombe()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("figure_1.txt");
            Figure[] fig = Figure.get_figure("figures.json");
            Figure Rhombe = fig[1];
            int count = Figure.find_figure(Rhombe, board);
            Assert.AreEqual(count, 1);

        }
        [TestMethod]
        public void TestFigureRing()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("figure_8.txt");
            Figure[] fig = Figure.get_figure("figures.json");
            Figure Ring = fig[2];
            int count = Figure.find_figure(Ring, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureShip()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("figure_9.txt");
            Figure[] fig = Figure.get_figure("figures.json");
            Figure Ship = fig[3];
            int count = Figure.find_figure(Ship, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureLeaf()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("figure_5.txt");
            Figure[] fig = Figure.get_figure("figures.json");
            Figure leaf = fig[4];
            int count = Figure.find_figure(leaf, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureMany()
        {
            Board board = new Board(7, 4, 1, 1);
            board.Upload("figure_6.txt");
            Figure[] fig = Figure.get_figure("figures.json");
            Figure cube = fig[0];
            int count = Figure.find_figure(cube, board);
            Assert.AreEqual(count, 2);
        }
        [TestMethod]
        public void TestFigureBarge()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Upload("figure_2.txt");
            Figure[] fig = Figure.get_figure("figures.json");
            Figure barge = fig[5];
            int count = Figure.find_figure(barge, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestFigureFrigate()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Upload("figure_4.txt");
            Figure[] fig = Figure.get_figure("figures.json");
            Figure Frigate = fig[6];
            int count = Figure.find_figure(Frigate, board);
            Assert.AreEqual(count, 1);
        }
    }
}
