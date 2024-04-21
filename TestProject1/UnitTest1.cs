using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;

namespace UnitTest1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            LifeGame life = new LifeGame();
            var cells = life.Run("example.txt", "user_settings.json");
            Assert.AreEqual(cells.Iters, 71);
        }

        [TestMethod]
        public void TestMethod2()
        {
            LifeGame life = new LifeGame();
            var cells = life.Run("example3.txt", "user_settings.json");
            Assert.AreEqual(cells.aliveCells, 6);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Board board = new Board(50, 20, 1, 0.5);
            board.GetCellsFromFile("box.txt");
            Assert.AreEqual(board.BoxesAmount(), 2);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Board board = new Board(50, 20, 1, 0.5);
            board.GetCellsFromFile("block.txt");
            Assert.AreEqual(board.BlocksAmount(), 3);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Board board = new Board(50, 20, 1, 0.5);
            board.GetCellsFromFile("hive.txt");
            Assert.AreEqual(board.HivesAmount(), 1);
        }

        [TestMethod]
        public void TestDataClassInstance()
        {
            Data data = new Data();
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void TestCellClassInstance()
        {
            Cell cell = new Cell();
            Assert.IsNotNull(cell);
        }

        [TestMethod]
        public void TestBoardClassInstance()
        {
            Board board = new Board(50, 20, 1, 0.5);
            Assert.IsNotNull(board);
        }

        [TestMethod]
        public void TestBoardWidthCalculation()
        {
            Board board = new Board(50, 20, 1, 0.5);
            Assert.AreEqual(50, board.Width);
        }

        [TestMethod]
        public void TestBoardHeightCalculation()
        {
            Board board = new Board(50, 20, 1, 0.5);
            Assert.AreEqual(20, board.Height);
        }

        [TestMethod]
        public void TestBoardCellInitialization()
        {
            Board board = new Board(50, 20, 1, 0.5);
            foreach (var cell in board.Cells)
            {
                Assert.IsNotNull(cell);
            }
        }

        [TestMethod]
        public void TestCellNextStateDetermination()
        {
            Cell cell = new Cell();
            cell.IsAlive = true;
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.DetermineNextLiveState();
            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        public void TestBoardCellStateChange()
        {
            Board board = new Board(50, 20, 1, 0.5);
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
        public void TestBoardSymmetryFiguresAmount()
        {
            Board board = new Board(50, 20, 1, 0.5);
            board.GetCellsFromFile("example1.txt");
            Assert.AreEqual(10, board.SymmetryFiguresAmount());
        }

        [TestMethod]
        public void TestLifeGameResetMethod()
        {
            LifeGame life = new LifeGame();
            var cellsCount = life.Reset("example.txt", "user_settings.json");
            Assert.AreEqual(1000, cellsCount);
        }
    }
}
