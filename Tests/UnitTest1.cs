using cli_life;
namespace Tests;

[TestClass]
public class UnitTest1
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
        Assert.ThrowsException<OverflowException>(act);
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

    [TestMethod]
    public void CountAliveCells_ReturnNumberOfAliveCells()
    {
        //Arrange
        Cell[][] cells = new Cell[][]
        {
            new Cell[] { new Cell() { IsAlive = true }, new Cell() { IsAlive = true } },
            new Cell[] { new Cell() { IsAlive = true }, new Cell() { IsAlive = false } },

        };
        var board = new Board(cells, 1, 1);

        //Act
        var result = board.CountAliveCells();

        //Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void CountPatternsAsync_HorizontalHive_ReturnNumberOfVerticalHives()
    {
        Board board = cli_life.Program.Reset();
        //Arrange
        board.LoadStateFile("Hives.txt");
        var pattern = WordFile.LoadPatterns("patterns.json").Where(a => a.Name == "HiveHorizontal");

        //Act
        var result = board.CountPatterns(pattern).Single();

        //Assert
        Assert.AreEqual(2, result.Value);
    }

    [TestMethod]
    public void CountPatternsAsync_Loaf_ReturnNumberOfLoafs()
    {
        Board board = Program.Reset();
        //Arrange
        board.LoadStateFile("somePatterns.txt");
        var pattern = WordFile.LoadPatterns("patterns.json").Where(a => a.Name == "Loaf");

        //Act
        var result = board.CountPatterns(pattern).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public void CountPatternsAsync_Tub_ReturnNumberOfTubs()
    {
        Board board = Program.Reset();
        //Arrange
        board.LoadStateFile("somePatterns.txt");
        var pattern = WordFile.LoadPatterns("patterns.json").Where(a => a.Name == "Tub");

        //Act
        var result = board.CountPatterns(pattern).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public void CountPatternsAsync_Boat_ReturnNumberOfBoats()
    {
        Board board = Program.Reset();
        //Arrange
        board.LoadStateFile("somePatterns.txt");
        var pattern = WordFile.LoadPatterns("patterns.json").Where(a => a.Name == "Boat");

        //Act
        var result = board.CountPatterns(pattern).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public void CountPatternsAsync_Ship_ReturnNumberOfShips()
    {
        Board board = Program.Reset();
        //Arrange
        board.LoadStateFile("somePatterns.txt");
        var pattern = WordFile.LoadPatterns("patterns.json").Where(a => a.Name == "Ship");

        //Act
        var result = board.CountPatterns(pattern).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public void CountPatternsAsync_Pond_ReturnNumberOfPonds()
    {
        Board board = Program.Reset();
        //Arrange
        board.LoadStateFile("somePatterns.txt");
        var pattern = WordFile.LoadPatterns("patterns.json").Where(a => a.Name == "Pond");

        //Act
        var result = board.CountPatterns(pattern).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public void CountPatternsAsync_Snake_ReturnNumberOfSnakes()
    {
        Board board = Program.Reset();
        //Arrange
        board.LoadStateFile("somePatterns.txt");
        var pattern = WordFile.LoadPatterns("patterns.json").Where(a => a.Name == "Snake");

        //Act
        var result = board.CountPatterns(pattern).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public void CountPatternsAsync_Canoe_ReturnNumberOfCanoes()
    {
        Board board = Program.Reset();
        //Arrange
        board.LoadStateFile("somePatterns.txt");
        var pattern = WordFile.LoadPatterns("patterns.json").Where(a => a.Name == "Canoe");

        //Act
        var result = board.CountPatterns(pattern).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public void CountPatternsAsync_AircraftCarrier_ReturnNumberOfAircraftCarriers()
    {
        Board board = Program.Reset();
        //Arrange
        board.LoadStateFile("somePatterns.txt");
        var pattern = WordFile.LoadPatterns("patterns.json").Where(a => a.Name == "AircraftCarrier");

        //Act
        var result = board.CountPatterns(pattern).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }
}