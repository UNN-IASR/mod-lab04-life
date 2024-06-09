using cli_life;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Life.Tests
{
    [TestClass]
    public class GameLifeTest
    {
        private Board Board1;
        private Board Board2;
        private Startup _startup;

        [TestMethod]
        public void TestPropertiesHasBeenSet()
        {
            ResetMock();
            Assert.AreEqual(Board1.Width, 30);
            Assert.AreEqual(Board1.Height, 15);
            Assert.AreEqual(Board1.CellSize, 1);
        }

        [TestMethod]
        public void TestRandomizeSuccess()
        {
            ResetMock();
            var boardOne = Board1.Cells;
            var boardTwo = Board2.Cells;

            bool isDifferent = false;
            for (var x = 0; x < boardTwo.GetLength(0); x++)
            for (var y = 0; y < boardTwo.GetLength(1); y++)
            {
                var cellBefore = boardOne[x, y].IsAlive;
                var cellAfter = boardTwo[x, y].IsAlive;
                if (!cellBefore.Equals(cellAfter))
                {
                    isDifferent = true;
                    break;
                }
            }

            Assert.IsTrue(isDifferent);
        }
        
        [TestMethod]
        public void TestRandomizeNotTriggered()
        {
            ResetMock();
            var boardOne = Board1.Cells;
            Board1.Randomize(0.2);
            var afterRandomize = Board1.Cells;

            bool isDifferent = false;
            for (var x = 0; x < boardOne.GetLength(0); x++)
            for (var y = 0; y < boardOne.GetLength(1); y++)
            {
                var cellBefore = boardOne[x, y].IsAlive;
                var cellAfter = afterRandomize[x, y].IsAlive;
                if (!cellBefore.Equals(cellAfter))
                {
                    isDifferent = true;
                    break;
                }
            }

            Assert.IsFalse(isDifferent);
        }
        
        [TestMethod]
        public void TestLoadFile()
        {
            ResetLoadPlaner();
            var cells = Board1.Cells;

            Assert.IsTrue(cells[1,1].IsAlive);
            Assert.IsTrue(cells[2,2].IsAlive);
            Assert.IsTrue(cells[0,3].IsAlive);
            Assert.IsTrue(cells[1,3].IsAlive);
            Assert.IsTrue(cells[2,3].IsAlive);
        }
        
        [TestMethod]
        public void TestAdvance()
        {
            ResetLoadPlaner();
            var cells = Board1.Cells;
            Board1.Advance();

            Assert.IsTrue(cells[0,2].IsAlive);
            Assert.IsTrue(cells[2,2].IsAlive);
            Assert.IsTrue(cells[1,3].IsAlive);
            Assert.IsTrue(cells[2,3].IsAlive);
            Assert.IsTrue(cells[1,4].IsAlive);
        }

        private void ResetLoadPlaner()
        {
            Board1 = new Board(
                filePath: "planer_30_15.txt",
                width: 30,
                height: 15,
                cellSize: 1,
                liveDensity: 1337);
        }

        private void ResetMock()
        {
            Board1 = new Board(
                filePath: "",
                width: 30,
                height: 15,
                cellSize: 1,
                liveDensity: 0.2);
            Board2 = new Board(
                filePath: "",
                width: 30,
                height: 15,
                cellSize: 1,
                liveDensity: 0.2);
        }
    }
}
