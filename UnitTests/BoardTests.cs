using cli_life;

namespace UnitTests;

[TestClass]
public class BoardTests
{
    [TestMethod]
    public void Construct_LegalInput()
    {
        //Arrange
        int width = 10;
        int height = 10;
        int cellSize = 1;

        //Act
        var board = new Board(width, height, cellSize, 1);

        //Assert
        Assert.AreEqual(width, board.Width);
        Assert.AreEqual(height, board.Height);
        Assert.AreEqual(cellSize, board.CellSize);
    }

    [TestMethod]
    public void Construct_IllegalInput()
    {
        //Arrange
        int width = -1;
        int height = -1;
        int cellSize = 1;

        //Act
        var act = () => new Board(width, height, cellSize, 0);

        //Assert
        Assert.ThrowsException<ArgumentException>(act);
    }

    [TestMethod]
    public void Advance_DieToLoneliness_Success()
    {
        //Arrange
        Cell[][] cells = new Cell[][]
        {
            new Cell[] { new Cell() { IsAlive = true }, new Cell() { IsAlive = false } },
            new Cell[] { new Cell() { IsAlive = false }, new Cell() { IsAlive = false } },

        };
        var board = new Board(cells, 1, 1);

        //Act
        board.Advance();

        //Assert
        Cell[,] expectedState = new Cell[,]
        {
            { new Cell() { IsAlive = false }, new Cell() { IsAlive = false } },
            { new Cell() { IsAlive = false }, new Cell() { IsAlive = false } },
        };

        for (int x = 0; x < board.Columns; x++)
        {
            for (int y = 0; y < board.Rows; y++)
            {
                Assert.AreEqual(expectedState[x, y].IsAlive, board.Cells[x, y].IsAlive);
            }
        }
    }

    [TestMethod]
    public void Advance_DieToOverpopulation_Success()
    {
        //Arrange
        Cell[][] cells = new Cell[][]
        {
            new Cell[] { new Cell() { IsAlive = true }, new Cell() { IsAlive = true } },
            new Cell[] { new Cell() { IsAlive = true }, new Cell() { IsAlive = true } },

        };
        var board = new Board(cells, 1, 1);

        //Act
        board.Advance();

        //Assert
        Cell[,] expectedState = new Cell[,]
        {
            { new Cell() { IsAlive = false }, new Cell() { IsAlive = false } },
            { new Cell() { IsAlive = false }, new Cell() { IsAlive = false } },
        };

        for (int x = 0; x < board.Columns; x++)
        {
            for (int y = 0; y < board.Rows; y++)
            {
                Assert.AreEqual(expectedState[x, y].IsAlive, board.Cells[x, y].IsAlive);
            }
        }
    }

    [TestMethod]
    public void Advance_CellGoesLive_Success()
    {
        //Arrange
        Cell[][] cells = new Cell[][]
        {
            new Cell[] { new Cell() { IsAlive = false }, new Cell() { IsAlive = false }, new Cell() { IsAlive = false }, new Cell() { IsAlive = false} },
            new Cell[] { new Cell() { IsAlive = false }, new Cell() { IsAlive = true }, new Cell() { IsAlive = true }, new Cell() { IsAlive = false} },
            new Cell[] { new Cell() { IsAlive = false }, new Cell() { IsAlive = true }, new Cell() { IsAlive = false }, new Cell() { IsAlive = false} },
            new Cell[] { new Cell() { IsAlive = false }, new Cell() { IsAlive = false }, new Cell() { IsAlive = false }, new Cell() { IsAlive = false} }
        };
        var board = new Board(cells, 1, 1);

        //Act
        board.Advance();

        //Assert
        Cell[,] expectedState = new Cell[,]
        {
            { new Cell() { IsAlive = false }, new Cell() { IsAlive = false }, new Cell() { IsAlive = false }, new Cell() { IsAlive = false} },
            { new Cell() { IsAlive = false }, new Cell() { IsAlive = true }, new Cell() { IsAlive = true }, new Cell() { IsAlive = false} },
            { new Cell() { IsAlive = false }, new Cell() { IsAlive = true }, new Cell() { IsAlive = true }, new Cell() { IsAlive = false } },
            { new Cell() { IsAlive = false }, new Cell() { IsAlive = false }, new Cell() { IsAlive = false }, new Cell() { IsAlive = false } }
        };

        for (int x = 0; x < board.Columns; x++)
        {
            for (int y = 0; y < board.Rows; y++)
            {
                Assert.AreEqual(expectedState[x, y].IsAlive, board.Cells[x, y].IsAlive);
            }
        }
    }
}