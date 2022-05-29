using Microsoft.VisualStudio.TestTools.UnitTesting;
using Life;

namespace StringLibraryTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            LifeGame life = new LifeGame();
            var cells = life.Run("../../../../user_stuff/example1.txt", "../../../../user_stuff/user_settings.json");
            Assert.AreEqual(cells.Iters, 47);
        }

        [TestMethod]
        public void TestMethod2()
        {
            LifeGame life = new LifeGame();
            var cells = life.Run("../../../../user_stuff/example3.txt", "../../../../user_stuff/user_settings.json");
            Assert.AreEqual(cells.aliveCells, 4);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Board board = new Board(50, 20, 1, 0.5);
            board.GetCellsFromFile("../../../../user_stuff/box.txt");
            Assert.AreEqual(board.BoxesAmount(), 4);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Board board = new Board(50, 20, 1, 0.5);
            board.GetCellsFromFile("../../../../user_stuff/block.txt");
            Assert.AreEqual(board.BlocksAmount(), 2);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Board board = new Board(50, 20, 1, 0.5);
            board.GetCellsFromFile("../../../../user_stuff/hive.txt");
            Assert.AreEqual(board.HivesAmount(), 3);
        }

        [TestMethod]
        public void TestMethod6()
        {
            LifeGame life = new LifeGame();
            var cells = life.Run("../../../../user_stuff/example2.txt", "../../../../user_stuff/user_settings.json");
            Assert.AreEqual(cells.aliveCells, 24);
        }

        [TestMethod]
        public void TestMethod7()
        {
            Board board = new Board(50, 20, 1, 0.5);
            board.GetCellsFromFile("../../../../user_stuff/example2.txt");
            Assert.AreEqual(board.HivesAmount(), 1);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Board board = new Board(50, 20, 1, 0.5);
            board.GetCellsFromFile("../../../../user_stuff/example2.txt");
            Assert.AreEqual(board.BoxesAmount(), 0);
        }
    }
}
