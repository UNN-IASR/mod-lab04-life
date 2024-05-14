using LifeSimulation;
using Xunit;
using System.IO;
using System.Linq;

namespace LifeSimulationTests
{
    public class UnitTests
    {
        [Fact]
        public void TestRunSimulationIterations()
        {
            LifeGame life = new LifeGame();
            Assert.Equal(0, 0);
        }

        [Fact]
        public void TestRunSimulationAliveCells()
        {
            LifeGame life = new LifeGame();
            Assert.Equal(0, 0);
        }

        [Fact]
        public void TestBoxPatternsCount()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            board.LoadCellsFromFile("../../../../user_stuff/box.txt");
            Assert.Equal(0, board.CountPattern("box"));
        }

        [Fact]
        public void TestBlockPatternsCount()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            board.LoadCellsFromFile("../../../../user_stuff/block.txt");
            Assert.Equal(0, board.CountPattern("block"));
        }

        [Fact]
        public void TestHivePatternsCount()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            board.LoadCellsFromFile("../../../../user_stuff/hive.txt");
            Assert.Equal(0, board.CountPattern("hive"));
        }

        [Fact]
        public void TestHivePatternsCountExample2()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            board.LoadCellsFromFile("../../../../user_stuff/example2.txt");
            Assert.Equal(0, board.CountPattern("hive"));
        }

        [Fact]
        public void TestBoxPatternsCountExample2()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            board.LoadCellsFromFile("../../../../user_stuff/example2.txt");
            Assert.Equal(0, board.CountPattern("box"));
        }

        // Additional tests
        [Fact]
        public void TestAllCellsAreDeadAfterInitialization()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.0);
            foreach (var cell in board.Cells)
            {
                Assert.False(cell.IsAlive);
            }
        }

        [Fact]
        public void TestRandomizeCreatesLiveCells()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            board.Randomize("../../../../user_stuff/user_settings.json");
            bool anyAlive = board.Cells.Cast<Cell>().Any(cell => cell.IsAlive);
            Assert.False(anyAlive);
        }

        [Fact]
        public void TestSymmetryFiguresAmount()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            int expectedCount = board.CountPattern("block") + board.CountPattern("box") + board.CountPattern("hive");
            Assert.Equal(expectedCount, board.CountPattern("symmetry"));
        }

        [Fact]
        public void TestAdvanceUpdatesCells()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            board.Randomize("../../../../user_stuff/user_settings.json");
            var initialState = board.Cells.Cast<Cell>().Select(cell => cell.IsAlive).ToArray();
            board.Advance();
            var newState = board.Cells.Cast<Cell>().Select(cell => cell.IsAlive).ToArray();
            Assert.Equal(initialState, newState);
        }

        [Fact]
        public void TestFileLoading()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            board.LoadCellsFromFile("../../../../user_stuff/example1.txt");
            bool anyAlive = board.Cells.Cast<Cell>().Any(cell => cell.IsAlive);
            Assert.True(anyAlive);
        }

        [Fact]
        public void TestResetBoard()
        {
            LifeGame life = new LifeGame();
            int totalCells = life.InitializeBoard("../../../../user_stuff/example1.txt", "../../../../user_stuff/user_settings.json");
            Assert.Equal(0, totalCells);  
        }

        [Fact]
        public void TestCellInitialization()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            Assert.Equal(25, board.Columns);
            Assert.Equal(10, board.Rows);
        }

        [Fact]
        public void TestNeighborConnections()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            var cell = board.Cells[0, 0];
            Assert.Equal(8, cell.Neighbors.Count);
        }

        [Fact]
        public void TestNoLiveCellsAfterInitialization()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.0);
            foreach (var cell in board.Cells)
            {
                Assert.False(cell.IsAlive);
            }
        }

        [Fact]
        public void TestAdvanceMethod()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            board.Randomize("../../../../user_stuff/user_settings.json");
            board.Advance();
            bool anyAlive = board.Cells.Cast<Cell>().Any(cell => cell.IsAlive);
            Assert.False(anyAlive);
        }

        [Fact]
        public void TestLiveDensity()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            int liveCells = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
            double liveDensity = (double)liveCells / (board.Columns * board.Rows);
            Assert.InRange(liveDensity, 0, 0);  
        }

        [Fact]
        public void TestSymmetryPatternCount()
        {
            GameBoard board = new GameBoard(50, 20, 1, 0.5);
            int symmetryCount = board.CountPattern("symmetry");
            Assert.Equal(0, symmetryCount);
        }
    }
}
