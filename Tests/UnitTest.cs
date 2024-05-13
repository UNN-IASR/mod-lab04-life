using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            GameOfLife life = new GameOfLife();
            var cells = life.Run("../../../../example1.txt", "../../../../settings.json");
            Assert.AreEqual(cells.Iters, 47);
        }

        [TestMethod]
        public void TestMethod2()
        {
            GameOfLife life = new GameOfLife();
            var cells = life.Run("../../../../example2.txt", "../../../../settings.json");
            Assert.AreEqual(cells.aliveCells, 24);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Board board = new Board(50, 20, 1);
            board.SetCellsFromFile("../../../../box.txt");
            Assert.AreEqual(board.BoxesAmount(), 2);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Board board = new Board(50, 20, 1);
            board.SetCellsFromFile("../../../../block.txt");
            Assert.AreEqual(board.BlocksAmount(), 3);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Board board = new Board(50, 20, 1);
            board.SetCellsFromFile("../../../../hive.txt");
            Assert.AreEqual(board.HivesAmount(), 1);
        }

        [TestMethod]
        public void TestMethod6()
        {
            Settings data = new Settings();
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void TestMethod7()
        {
            Cell cell = new Cell();
            Assert.IsNotNull(cell);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Board board = new Board(50, 20, 1);
            Assert.IsNotNull(board);
        }

        [TestMethod]
        public void TestMethod9()
        {
            Board board = new Board(50, 20, 1);
            Assert.AreEqual(50, board.Width);
        }

        [TestMethod]
        public void TestMethod10()
        {
            Board board = new Board(50, 20, 1);
            Assert.AreEqual(20, board.Height);
        }

        [TestMethod]
        public void TestMethod11()
        {
            Board board = new Board(50, 20, 1);
            foreach (var cell in board.Cells)
            {
                Assert.IsNotNull(cell);
            }
        }

        [TestMethod]
        public void TestMethod12()
        {
            Cell cell = new Cell();
            cell.IsAlive = true;
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.DetermineNextLiveState();
        }

        [TestMethod]
        public void TestMethod13()
        {
            Board board = new Board(50, 20, 1);
            foreach (var cell in board.Cells)
            {
                cell.IsAlive = true;
            }
            board.Advance();
            foreach (var cell in board.Cells)
            {
                Assert.IsFalse(cell.IsAlive);
            }
        }

        [TestMethod]
        public void TestMethod14()
        {
            Board board = new Board(50, 20, 1);
            board.SetCellsFromFile("../../../../example1.txt");
            Assert.AreEqual(4, board.BoxesAmount() + board.HivesAmount() + board.BlocksAmount());
        }

        [TestMethod]
        public void TestMethod15()
        {
            GameOfLife life = new GameOfLife();
            var cellsCount = life.Reset("../../../../example2.txt", "../../../../settings.json");
            Assert.AreEqual(1000, cellsCount);
        }
    }
}