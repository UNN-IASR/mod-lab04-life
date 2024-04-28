using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;
using System.Text.Json;
namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Board board = new Board(100, 100, 1, 0.5);
            Assert.AreEqual(100, board.Height);
            Assert.AreEqual(100, board.Width);
        }
        [TestMethod]
        public void TestMethod2()
        {
            string fileName = "../../../../Settings.json";
            string jsonString = File.ReadAllText(fileName);
            Setting setting = JsonSerializer.Deserialize<Setting>(jsonString);
            Assert.AreEqual(100, setting.Width);
            Assert.AreEqual(100, setting.Height);
        }
        [TestMethod]
        public void TestMethod3()
        {
            string fileName = "../../../../Settings.json";
            string jsonString = File.ReadAllText(fileName);
            Setting setting = JsonSerializer.Deserialize<Setting>(jsonString);
            Assert.AreEqual(0.5, setting.LiveDensity);
            Assert.AreEqual(1, setting.CellSize);
        }
        [TestMethod]
        public void TestMethod4()
        {
            Board board = new Board(4, 4, 1, 0.5);
            board.LoadFromFile("../../../../Block.txt");
            Assert.AreEqual(4, board.AliveCells());
        }
        [TestMethod]
        public void TestMethod5()
        {
            Board board = new Board(5, 5, 1, 0.5);
            board.LoadFromFile("../../../../Boat.txt");
            Assert.AreEqual(5, board.AliveCells());
        }
        [TestMethod]
        public void TestMethod6()
        {
            Board board = new Board(5, 5, 1, 0.5);
            board.LoadFromFile("../../../../Box.txt");
            Assert.AreEqual(4, board.AliveCells());
        }
        [TestMethod]
        public void TestMethod7()
        {
            Board board = new Board(7, 7, 1, 0.5);
            board.LoadFromFile("../../../../Canoe.txt");
            Assert.AreEqual(8, board.AliveCells());
        }
        [TestMethod]
        public void TestMethod8()
        {
            Board board = new Board(6, 5, 1, 0.5);
            board.LoadFromFile("../../../../Hive.txt");
            Assert.AreEqual(6, board.AliveCells());
        }
        [TestMethod]
        public void TestMethod10()
        {
            Board board = new Board(7, 7, 1, 0.5);
            board.LoadFromFile("../../../../Integral.txt");
            Assert.AreEqual(9, board.AliveCells());
        }
        [TestMethod]
        public void TestMethod11()
        {
            Board board = new Board(6, 6, 1, 0.5);
            board.LoadFromFile("../../../../Karavai.txt");
            Assert.AreEqual(7, board.AliveCells());
        }
        [TestMethod]
        public void TestMethod12()
        {
            Board board = new Board(5, 5, 1, 0.5);
            board.LoadFromFile("../../../../Ship.txt");
            Assert.AreEqual(6, board.AliveCells());
        }
        [TestMethod]
        public void TestMethod13()
        {
            Board board = new Board(4, 4, 1, 0.5);
            board.LoadFromFile("../../../../Block.txt");
            Program.Konstryct(board);
            Assert.AreEqual(1, Program.CountFigure("../../../../Block.txt", "../../../../setBlock.json"));
        }
        [TestMethod]
        public void TestMethod14()
        {
            Board board = new Board(5, 5, 1, 0.5);
            board.LoadFromFile("../../../../Boat.txt");
            Program.Konstryct(board);
            Assert.AreEqual(1, Program.CountFigure("../../../../Boat.txt", "../../../../setBoat.json"));
        }
        [TestMethod]
        public void TestMethod15()
        {
            Board board = new Board(5, 5, 1, 0.5);
            board.LoadFromFile("../../../../Box.txt");
            Program.Konstryct(board);
            Assert.AreEqual(1, Program.CountFigure("../../../../Box.txt", "../../../../setBox.json"));
        }
        [TestMethod]
        public void TestMethod16()
        {
            Board board = new Board(7, 7, 1, 0.5);
            board.LoadFromFile("../../../../Canoe.txt");
            Program.Konstryct(board);
            Assert.AreEqual(1, Program.CountFigure("../../../../Canoe.txt", "../../../../setCanoe.json"));
        }
        [TestMethod]
        public void TestMethod17()
        {
            Board board = new Board(6, 5, 1, 0.5);
            board.LoadFromFile("../../../../Hive.txt");
            Program.Konstryct(board);
            Assert.AreEqual(1, Program.CountFigure("../../../../Hive.txt", "../../../../setHive.json"));
        }
        [TestMethod]
        public void TestMethod18()
        {
            Board board = new Board(7, 7, 1, 0.5);
            board.LoadFromFile("../../../../Integral.txt");
            Program.Konstryct(board);
            Assert.AreEqual(1, Program.CountFigure("../../../../Integral.txt", "../../../../setIntegral.json"));
        }
        [TestMethod]
        public void TestMethod19()
        {
            Board board = new Board(6, 6, 1, 0.5);
            board.LoadFromFile("../../../../Karavai.txt");
            Program.Konstryct(board);
            Assert.AreEqual(1, Program.CountFigure("../../../../Karavai.txt", "../../../../setKaravai.json"));
        }
        [TestMethod]
        public void TestMethod20()
        {
            Board board = new Board(5, 5, 1, 0.5);
            board.LoadFromFile("../../../../Ship.txt");
            Program.Konstryct(board);
            Assert.AreEqual(1, Program.CountFigure("../../../../Ship.txt", "../../../../setShip.json"));
        }
    }
}
