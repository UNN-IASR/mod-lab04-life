using cli_life;

namespace UnitTests;

[TestClass]
public class BoardAnalyzerTests
{
    //Test count patterns
    //Test IsStable

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
    public async Task CountPatternsAsync_Block_ReturnNumberOfBlocks()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("blocks.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "Block");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(5, result.Value);
    }

    [TestMethod]
    public async Task CountPatternsAsync_VerticalHive_ReturnNumberOfVerticalHives()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("hives.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "HiveVertical");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(3, result.Value);
    }
    
    [TestMethod]
    public async Task CountPatternsAsync_HorizontalHive_ReturnNumberOfVerticalHives()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("hives.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "HiveHorizontal");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(2, result.Value);
    }

    [TestMethod]
    public async Task CountPatternsAsync_Loaf_ReturnNumberOfLoafs()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("somePatterns.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "Loaf");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public async Task CountPatternsAsync_Tub_ReturnNumberOfTubs()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("somePatterns.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "Tub");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public async Task CountPatternsAsync_Boat_ReturnNumberOfBoats()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("somePatterns.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "Boat");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public async Task CountPatternsAsync_Ship_ReturnNumberOfShips()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("somePatterns.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "Ship");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public async Task CountPatternsAsync_Pond_ReturnNumberOfPonds()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("somePatterns.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "Pond");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public async Task CountPatternsAsync_Snake_ReturnNumberOfSnakes()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("somePatterns.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "Snake");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public async Task CountPatternsAsync_Canoe_ReturnNumberOfCanoes()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("somePatterns.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "Canoe");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }

    [TestMethod]
    public async Task CountPatternsAsync_AircraftCarrier_ReturnNumberOfAircraftCarriers()
    {
        //Arrange
        var board = Helpers.LoadBoardFromFile("somePatterns.txt");
        var pattern = Helpers.LoadPatterns("patterns.json").Where(a => a.Name == "AircraftCarrier");

        //Act
        var result = (await board.CountPatternsAsync(pattern, default)).Single();

        //Assert
        Assert.AreEqual(1, result.Value);
    }
}
