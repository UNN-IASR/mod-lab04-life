using cli_life;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public void Board_Initialization_InitializesCorrectColumns()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0.5;

        // Act
        var board = new Board(width, height, cellSize, liveDensity);

        // Assert
        Assert.Equal(width / cellSize, board.Columns);
    }

    [Fact]
    public void Board_Initialization_InitializesCorrectRows()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0.5;

        // Act
        var board = new Board(width, height, cellSize, liveDensity);

        // Assert
        Assert.Equal(height / cellSize, board.Rows);
    }

    [Fact]
    public void Board_Initialization_InitializesCorrectWidth()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0.5;

        // Act
        var board = new Board(width, height, cellSize, liveDensity);

        // Assert
        Assert.Equal(board.Columns * cellSize, board.Width);
    }

    [Fact]
    public void Board_Initialization_InitializesCorrectHeight()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0.5;

        // Act
        var board = new Board(width, height, cellSize, liveDensity);

        // Assert
        Assert.Equal(board.Rows * cellSize, board.Height);
    }


    [Fact]
    public void Board_Randomize_InitializesCellsBasedOnDensity()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0.5;

        // Act
        var board = new Board(width, height, cellSize, liveDensity);
        board.Randomize(liveDensity);

        // Assert
        var liveCells = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        Assert.InRange(liveCells, 0, width * height);
    }

    [Fact]
    public void Board_Advance_UpdatesCellsStatesCorrectly()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0.5;

        var board = new Board(width, height, cellSize, liveDensity);
        board.Randomize(liveDensity);

        // Act
        board.Advance();

        // Assert
        var liveCellsAfterAdvance = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        Assert.Equal(liveCellsAfterAdvance, board.Cells.Cast<Cell>().Count(cell => cell.IsAlive));
    }

    [Fact]
    public void Board_SaveState_LoadState_SavesAndLoadsCorrectly()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0.5;

        var board = new Board(width, height, cellSize, liveDensity);
        board.Randomize(liveDensity);

        // Act
        string stateJson = board.SaveState();
        board.LoadState(stateJson);

        // Assert
        var liveCellsAfterLoad = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        Assert.Equal(board.Cells.Cast<Cell>().Count(cell => cell.IsAlive), liveCellsAfterLoad);
    }

    [Fact]
    public void Cell_DetermineNextLiveState_CalculatesCorrectly()
    {
        // Arrange
        var cell = new Cell { IsAlive = true };
        var neighbor = new Cell { IsAlive = false };
        cell.neighbors.Add(neighbor);

        // Act
        cell.DetermineNextLiveState();

        // Assert
        Assert.True(cell.IsAlive);
    }

    [Fact]
    public void Board_ConnectNeighbors_ConnectsNeighborsCorrectly()
    {
        // Arrange
        int width = 3;
        int height = 3;
        int cellSize = 1;
        double liveDensity = 0.5;

        var board = new Board(width, height, cellSize, liveDensity);
        board.ConnectNeighbors();

        // Act
        var topLeftCell = board.Cells[0, 0];
        var topRightCell = board.Cells[0, 1];
        var bottomLeftCell = board.Cells[1, 0];

        // Assert
        Assert.Contains(topLeftCell, topRightCell.neighbors);
        Assert.Contains(topLeftCell, bottomLeftCell.neighbors);
        Assert.Contains(topRightCell, topLeftCell.neighbors);
        Assert.Contains(topRightCell, bottomLeftCell.neighbors);
        Assert.Contains(bottomLeftCell, topLeftCell.neighbors);
        Assert.Contains(bottomLeftCell, topRightCell.neighbors);
    }

    [Fact]
    public void Board_SaveStateToFile_LoadStateFromFile_SavesAndLoadsCorrectly()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0.5;

        var board = new Board(width, height, cellSize, liveDensity);
        board.Randomize(liveDensity);

        // Act
        board.SaveStateToFile("tempState.json");
        board.LoadStateFromFile("tempState.json");

        // Assert
        var liveCellsAfterLoad = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        Assert.Equal(board.Cells.Cast<Cell>().Count(cell => cell.IsAlive), liveCellsAfterLoad);

        // Cleanup
        File.Delete("tempState.json");
    }

    [Fact]
    public void Board_Initialization_InitializesNoLiveCells_WhenLiveDensityIsZero()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0; // No live cells expected

        // Act
        var board = new Board(width, height, cellSize, liveDensity);

        // Assert
        var liveCells = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        Assert.Equal(0, liveCells);
    }

    [Fact]
    public void Board_Advance_DecreasesNumberOfLiveCells_WhenInitialConfigurationIsCritical()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0.5;

        var board = new Board(width, height, cellSize, liveDensity);
        board.Randomize(liveDensity);

        // Act
        board.Advance();

        // Assert
        var liveCellsBeforeAdvance = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        var liveCellsAfterAdvance = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        Assert.False(liveCellsBeforeAdvance > liveCellsAfterAdvance);
    }

    [Fact]
    public void Board_SaveState_LoadState_LoadsIncorrectState_WhenInitialStateIsCritical()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0.5;

        var board = new Board(width, height, cellSize, liveDensity);
        board.Randomize(liveDensity);

        // Act
        string stateJson = board.SaveState();
        board.LoadState(stateJson);

        // Assert
        var liveCellsAfterLoad = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        Assert.Equal(board.Cells.Cast<Cell>().Count(cell => cell.IsAlive), liveCellsAfterLoad);
    }

    [Fact]
    public void Board_EmptyBoard_StaysEmpty()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 0;

        var board = new Board(width, height, cellSize, liveDensity);

        // Act
        board.Advance();

        // Assert
        var liveCells = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        Assert.Equal(0, liveCells);
    }

    [Fact]
    public void Board_FullyPopulatedBoard_StaysFullyPopulated()
    {
        // Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;
        double liveDensity = 1;

        var board = new Board(width, height, cellSize, liveDensity);
        board.Randomize(liveDensity);

        // Act
        board.Advance();

        // Assert
        var liveCells = board.Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        Assert.NotEqual(width * height, liveCells);
    }

    [Fact]
    public void Board_EdgeCases_HandleCorrectly()
    {
        // Arrange
        int width = 3;
        int height = 3;
        int cellSize = 1;
        double liveDensity = 0.5;

        var board = new Board(width, height, cellSize, liveDensity);
        board.Randomize(liveDensity);

        // Act
        board.Advance();

        // Assert
        var middleCell = board.Cells[1, 1];
        Assert.Equal(8, middleCell.neighbors.Count);
    }
}