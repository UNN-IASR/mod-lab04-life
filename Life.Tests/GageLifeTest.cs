using cli_life;

namespace Life.Tests;

[TestClass]
public class GameLifeTest
{
    private Board Board1;
    private Board Board2;
    private Startup _startup;
    
    private Board _board;

    [TestInitialize]
    public void TestInitialize()
    {
        _startup = new Startup();
        var config = _startup._appConfig;

        _board = new Board(
            filePath: config.LoadFilePath,
            width: config.BoardHeight,
            height: config.BoardWidth,
            cellSize: config.BoardCellSize,
            liveDensity: config.BoardLiveDensity);
    }
    
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

        Assert.IsTrue(cells[1, 1].IsAlive);
        Assert.IsTrue(cells[2, 2].IsAlive);
        Assert.IsTrue(cells[0, 3].IsAlive);
        Assert.IsTrue(cells[1, 3].IsAlive);
        Assert.IsTrue(cells[2, 3].IsAlive);
    }

    [TestMethod]
    public void TestAdvance()
    {
        ResetLoadPlaner();
        var cells = Board1.Cells;
        Board1.Advance();

        Assert.IsTrue(cells[0, 2].IsAlive);
        Assert.IsTrue(cells[2, 2].IsAlive);
        Assert.IsTrue(cells[1, 3].IsAlive);
        Assert.IsTrue(cells[2, 3].IsAlive);
        Assert.IsTrue(cells[1, 4].IsAlive);
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
    
    [TestMethod]
    public void TestBoardDimensions()
    {
        Assert.AreEqual(_board.Width, _board.Columns * _board.CellSize);
        Assert.AreEqual(_board.Height, _board.Rows * _board.CellSize);
    }

    [TestMethod]
    public void TestCellNeighborCount()
    {
        foreach (var cell in _board.Cells)
        {
            Assert.AreEqual(cell.neighbors.Count, 8);
        }
    }

    [TestMethod]
    public void TestCellAdvanceDeadToAlive()
    {
        var cell = new Cell { IsAlive = false };
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.DetermineNextLiveState();
        cell.Advance();
        Assert.IsTrue(cell.IsAlive);
    }

    [TestMethod]
    public void TestCellAdvanceAliveToDead()
    {
        var cell = new Cell { IsAlive = true };
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.DetermineNextLiveState();
        cell.Advance();
        Assert.IsFalse(cell.IsAlive);
    }

    [TestMethod]
    public void TestCellStaysAlive()
    {
        var cell = new Cell { IsAlive = true };
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.DetermineNextLiveState();
        cell.Advance();
        Assert.IsTrue(cell.IsAlive);
    }

    [TestMethod]
    public void TestBoardRandomizeDensity()
    {
        double liveDensity = 0.5;
        _board.Randomize(liveDensity);
        int liveCells = 0;
        foreach (var cell in _board.Cells)
        {
            if (cell.IsAlive)
            {
                liveCells++;
            }
        }
        double actualLiveDensity = (double)liveCells / (_board.Rows * _board.Columns);
        Assert.IsTrue(Math.Abs(liveDensity - actualLiveDensity) < 0.1);
    }

    [TestMethod]
    public void TestBoardConnectNeighbors()
    {
        foreach (var cell in _board.Cells)
        {
            Assert.AreEqual(cell.neighbors.Count, 8);
        }
    }

    [TestMethod]
    public void TestCellDetermineNextLiveState()
    {
        var cell = new Cell { IsAlive = true };
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.DetermineNextLiveState();
        cell.Advance();
        Assert.IsFalse(cell.IsAlive);
    }

    [TestMethod]
    public void TestCellAdvance()
    {
        var cell = new Cell { IsAlive = true };
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.DetermineNextLiveState();
        cell.Advance();
        Assert.IsFalse(cell.IsAlive);
    }
    
    [TestMethod]
    public void TestBoardStaysConstantWithNoLiveCells()
    {
        foreach (var cell in _board.Cells)
        {
            cell.IsAlive = false;
        }
        var initialBoardState = (Cell[,])_board.Cells.Clone();
        _board.Advance();
        CollectionAssert.AreEqual(initialBoardState, _board.Cells);
    }
}