using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;
namespace NET
{
    [TestClass]
    public class UnitTest1
    {
    [TestMethod]
    public void TestBoardInitialization()
    {
        var board = new Board(50, 20, 1, 1);
        
        Assert.AreEqual(50, board.Width);
        Assert.AreEqual(20, board.Height);
        Assert.AreEqual(1, board.CellSize);
    }
    
    [TestMethod]
    public void TestBoardInitializationWithNegativeSize()
    {
        int width = -5;
        int height = -5;
        int cellSize = 1;

        Assert.Throws<OverflowException>(() => new Board(width, height, cellSize));
    }
    
    [TestMethod]
    public void TestCellNextState()
    {
        var cell = new Cell();
        cell.IsAlive = true;
        cell.Neighbors.Add(new Cell { IsAlive = false });
        cell.Neighbors.Add(new Cell { IsAlive = true });
        cell.Neighbors.Add(new Cell { IsAlive = true });

        cell.DetermineNextLiveState();

        Assert.IsTrue(cell.IsAliveNext); 
    }
    
    [TestMethod]
    public void TestBoardRandomizationWithZeroDensity()
    {
        int width = 5;
        int height = 5;
        int cellSize = 1;
        double liveDensity = 0;

        var board = new Board(width, height, cellSize, liveDensity);

        foreach (var cell in board.Cells)
        {
            Assert.IsFalse(cell.IsAlive); 
        }
    }
    
    [TestMethod]
    public void TestBoardRandomizationWithFullDensity()
    {
        int width = 5;
        int height = 5;
        int cellSize = 1;
        double liveDensity = 1;

        var board = new Board(width, height, cellSize, liveDensity);

        foreach (var cell in board.Cells)
        {
            Assert.IsTrue(cell.IsAlive);
        }
    }
    
    [TestMethod]
    public void TestCellAdvanceWithOvercrowding()
    {
        var cell = new Cell();
        cell.IsAlive = true;
        cell.Neighbors.AddRange(new[]
        {
            new Cell { IsAlive = true },
            new Cell { IsAlive = true },
            new Cell { IsAlive = true },
            new Cell { IsAlive = true }
        });

        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.IsFalse(cell.IsAlive); 
    }
    
    [TestMethod]
    public void TestBoardInitializationWithLargeSize()
    {
        int width = 1000;
        int height = 1000;
        int cellSize = 1;

        var board = new Board(width, height, cellSize);

        Assert.AreEqual(width, board.Columns);
        Assert.AreEqual(height, board.Rows);
    }

    [TestMethod]
    public void TestCellAdvance()
    {
        var cell = new Cell();

        cell.IsAlive = true;
        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.IsFalse(cell.IsAlive); 
    }
    
    [TestMethod]
    public void TestBoardRandomization()
    {
        var board = new Board(10, 10, 1);
        double liveDensity = 0.5;

        board.Randomize(liveDensity);

        int countAlive = 0;
        foreach (var cell in board.Cells)
        {
            if (cell.IsAlive)
                countAlive++;
        }
        
        double calculatedDensity = (double)countAlive / (board.Columns * board.Rows);
        
        Assert.AreEqual(liveDensity, calculatedDensity, 0.1); 
    }
    
    
    
    [TestMethod]
    public void TestLoadingSettings()
    {
        string fileName = "settings.json";
        int expectedWidth = 50;
        int expectedHeight = 20;
        int expectedCellSize = 1;

        int width, height, cellSize;
        double liveDensity;
        Program.LoadSettings(fileName, out width, out height, out cellSize, out liveDensity);

        Assert.AreEqual(expectedWidth, width);
        Assert.AreEqual(expectedHeight, height);
        Assert.AreEqual(expectedCellSize, cellSize);
    }
    
    [TestMethod]
    public void TestBoardDimensions()
    {
        int width = 10;
        int height = 20;
        int cellSize = 2;

        var board = new Board(width * cellSize, height * cellSize, cellSize);

        Assert.AreEqual(width, board.Columns);
        Assert.AreEqual(height, board.Rows);
        Assert.AreEqual(width * cellSize, board.Width);
        Assert.AreEqual(height * cellSize, board.Height);
    }
    
    [TestMethod]
    public void TestBoardBoundaryCells() 
    {
        var board = new Board(5, 5, 1);

        bool allBoundaryCellsDead = true;
        for (int i = 0; i < board.Columns; i++)
        {
            if (board.Cells[i, 0].IsAlive || board.Cells[i, board.Rows - 1].IsAlive ||
                board.Cells[0, i].IsAlive || board.Cells[board.Columns - 1, i].IsAlive)
            {
                allBoundaryCellsDead = false;
                break;
            }
        }

        Assert.IsFalse(allBoundaryCellsDead);
    }
    
    [TestMethod]
    public void TestBoardConnectivity()
    {
        var board = new Board(5, 5, 1);

        bool allCellsConnected = true;
        foreach (var cell in board.Cells)
        {
            bool isConnected = false;
            foreach (var neighbor in cell.Neighbors)
            {
                if (neighbor != null)
                {
                    isConnected = true;
                    break;
                }
            }
            if (!isConnected)
            {
                allCellsConnected = false;
                break;
            }
        }
        Assert.IsTrue(allCellsConnected);
    }
    
   

    [TestMethod]
    public void TestBoardInitializationWithNonSquareSize()
    {
        int width = 10;
        int height = 5;
        int cellSize = 1;

        var board = new Board(width, height, cellSize);

        Assert.AreEqual(width, board.Columns);
        Assert.AreEqual(height, board.Rows);
    }

    [TestMethod]
    public void TestProgramSimulationBoard()
    {
        int width = 10;
        int height = 10;
        var board = new Board(width, height, 1);

        for (int i = 0; i < 10; i++)
        {
            board.Advance(); 
        }

        Assert.Pass();
    }   
    }
}
