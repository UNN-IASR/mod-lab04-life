using Life;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        Random rd = new Random(124);

        [TestMethod]
        public void AdvanceCell1()
        {
            Cell cell = new Cell(false);
            cell.DetermineNextLiveState();
            cell.Advance();
            Assert.IsFalse(cell.IsAliveNow);
        }

        [TestMethod]
        public void AdvanceCell2()
        {
            Cell cell = new Cell(true);
            Assert.IsTrue(cell.IsAliveNow);
            cell.DetermineNextLiveState();
            cell.Advance();
            Assert.IsFalse(cell.IsAliveNow);
        }

        [TestMethod]
        public void AdvanceCell3()
        {
            Cell cell = new Cell(false);

            cell.neighbors.Add(new Cell(true));
            cell.neighbors.Add(new Cell(true));
            cell.neighbors.Add(new Cell(true));

            cell.DetermineNextLiveState();
            cell.Advance();
            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        public void AdvanceCell4()
        {
            Cell cell = new Cell(false);

            cell.neighbors.Add(new Cell(true));
            cell.neighbors.Add(new Cell(true));
            cell.neighbors.Add(new Cell(true));
            cell.neighbors.Add(new Cell(true));

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsFalse(cell.IsAliveNext);
        }

        [TestMethod]
        public void AdvanceCell5()
        {
            Cell cell = new Cell(true);

            cell.neighbors.Add(new Cell(true));
            cell.neighbors.Add(new Cell(true));
            cell.DetermineNextLiveState();
            cell.Advance();
            Assert.IsTrue(cell.IsAliveNext);

            cell.neighbors.Add(new Cell(true));
            cell.DetermineNextLiveState();
            cell.Advance();
            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        public void AdvanceCell6()
        {
            Cell cell = new Cell(true);

            cell.neighbors.Add(new Cell(true));
            cell.neighbors.Add(new Cell(true));
            cell.neighbors.Add(new Cell(true));
            cell.neighbors.Add(new Cell(true));

            cell.DetermineNextLiveState();
            cell.Advance();
            Assert.IsFalse(cell.IsAliveNext);
        }

        [TestMethod]
        public void GenerateRandomBoardWithMinDensity()
        {
            int columns = 20;
            int rows = 20;
            double liveDensity = 0;

            Board board = BoardGenerator.GenerateRandom(columns, rows, liveDensity, rd);

            Assert.AreEqual(0, Analyzer.countAliveCell(board));
        }

        [TestMethod]
        public void GenerateRandomBoardWithMaxDensity()
        {
            int columns = 20;
            int rows = 20;
            double liveDensity = 1;

            Board board = BoardGenerator.GenerateRandom(columns, rows, liveDensity, rd);

            Assert.AreEqual(columns * rows, Analyzer.countAliveCell(board));
        }

        [TestMethod]
        public void GenerateRandomBoardWithAverageDensity()
        {
            int columns = 20;
            int rows = 20;
            double liveDensity = 0.5;

            Board board = BoardGenerator.GenerateRandom(columns, rows, liveDensity, rd);

            Assert.AreEqual(columns * rows * liveDensity, Analyzer.countAliveCell(board), 10);
        }

        [TestMethod]
        public void TestCorrectCreate()
        {
            Cell[,] cells = {
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(true), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(true), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
            };

            Board board = new Board(cells);

            Assert.AreEqual(5, board.Columns);
            Assert.AreEqual(4, board.Rows);
        }

        [TestMethod]
        public void TestAdvance1()
        {
            Cell[,] cells = {
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(true), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(true), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
            };

            Board board = new Board(cells);

            Assert.AreEqual(4, Analyzer.countAliveCell(board));
            board.Advance();
            Assert.AreEqual(4, Analyzer.countAliveCell(board));
        }

        [TestMethod]
        public void TestAdvance2()
        {
            Cell[,] cells = {
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(true), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
            };

            Board board = new Board(cells);

            Assert.AreEqual(3, Analyzer.countAliveCell(board));
            board.Advance();
            Assert.AreEqual(4, Analyzer.countAliveCell(board));
            board.Advance();
            Assert.AreEqual(4, Analyzer.countAliveCell(board));
        }

        [TestMethod]
        public void TestAdvance3()
        {
            Cell[,] cells = {
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(true), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(false), new Cell(false) },
                { new Cell(true), new Cell(true), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
            };

            Board board = new Board(cells);

            Assert.AreEqual(5, Analyzer.countAliveCell(board));
            board.Advance();
            Assert.AreEqual(4, Analyzer.countAliveCell(board));
            board.Advance();
            Assert.AreEqual(2, Analyzer.countAliveCell(board));
            board.Advance();
            Assert.AreEqual(0, Analyzer.countAliveCell(board));
        }

        [TestMethod]
        public void testSaveAndLoadBoard1()
        {
            string path = "TestBoardSave.json";

            Cell[,] cells = {
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(true), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
            };

            Board board = new Board(cells);

            board.Save(path);
        }

        [TestMethod]
        public void testSaveAndLoadBoard2()
        {

            string path = "TestBoardSave.json";

            Cell[,] cells = {
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(true), new Cell(false) },
                { new Cell(false), new Cell(true), new Cell(false), new Cell(false) },
                { new Cell(false), new Cell(false), new Cell(false), new Cell(false) },
            };

            Board board = Board.Load(path);

            for (int column = 0; column < board.Columns; column++)
                for (int row = 0; row < board.Rows; row++)
                    Assert.AreEqual(cells[column, row].IsAliveNow, board.isAliveCell(column, row));
        }
    }
}
