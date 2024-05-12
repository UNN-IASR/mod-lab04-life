using cli_life;

namespace ProgramTests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void CheckCellLife_OvercrowdingLeadsToDeath()
    {
        var testCell = new Cell { IsAlive = true };
        var totalAliveNeighbours = 4;

        for (var index = 0; index < totalAliveNeighbours; index++)
        {
            var livingNeighbour = new Cell { IsAlive = true };
            testCell.neighbours.Add(livingNeighbour);
        }

        var shouldBeDead = false;
        var actualNextState = testCell.DetermineNextLiveState();

        Assert.AreEqual(actualNextState, shouldBeDead);
    }

    [TestMethod]
    public void ValidateLifeCycle_AliveWithThreeNeighbors_StaysAlive()
    {
        var initialCell = new Cell { IsAlive = true };
        var totalAliveNeighbors = 3;

        for (var index = 0; index < totalAliveNeighbors; index++)
        {
            var aliveNeighbor = new Cell { IsAlive = true };
            initialCell.neighbours.Add(aliveNeighbor);
        }

        var expectedState = true;
        var actualState = initialCell.DetermineNextLiveState();

        Assert.AreEqual(expectedState, actualState);
    }

    [TestMethod]
    public void ValidateLifeCycle_DeadCellWithThreeNeighbours_BecomesAlive()
    {
        var testCell = new Cell { IsAlive = false };
        var aliveNeighboursCount = 3;

        for (var index = 0; index < aliveNeighboursCount; index++)
        {
            var aliveNeighbour = new Cell { IsAlive = true };
            testCell.neighbours.Add(aliveNeighbour);
        }

        bool expectedState = true;
        bool actualState = testCell.DetermineNextLiveState();

        Assert.AreEqual(expectedState, actualState);
    }

    [TestMethod]
    public void ValidateLifeCycle_DeadCellWithTwoNeighbours_StaysDead()
    {
        var testCell = new Cell { IsAlive = false };
        var numberOfNeighbours = 2;

        for (var i = 0; i < numberOfNeighbours; i++)
        {
            var aliveNeighbour = new Cell { IsAlive = true };
            testCell.neighbours.Add(aliveNeighbour);
        }

        var expectedState = false;
        var actualState = testCell.DetermineNextLiveState();

        Assert.AreEqual(expectedState, actualState);
    }

    [TestMethod]
    public void CheckLifeCycle_AliveCellRemainsAlive_AfterAdvancing()
    {
        var testCell = new Cell { IsAlive = true };
        var numberOfNeighbours = 3;

        for (var i = 0; i < numberOfNeighbours; i++)
        {
            var aliveNeighbour = new Cell { IsAlive = true };
            testCell.neighbours.Add(aliveNeighbour);
        }

        testCell.DetermineNextLiveState();
        testCell.Advance();

        var expectedState = true;
        Assert.AreEqual(expectedState, testCell.IsAlive);
    }

    [TestMethod]
    public void CheckLifeCycle_AliveCellDies_AfterAdvancing()
    {
        var testCell = new Cell { IsAlive = true };
        var numberOfNeighbours = 4;

        for (var i = 0; i < numberOfNeighbours; i++)
        {
            var aliveNeighbour = new Cell { IsAlive = true };
            testCell.neighbours.Add(aliveNeighbour);
        }

        testCell.DetermineNextLiveState();
        testCell.Advance();

        var expectedState = false;
        Assert.AreEqual(expectedState, testCell.IsAlive);
    }
    
    [TestMethod]
    public void CheckBoardCreation_WithCellSizeGreaterThanOne_ValidatesRowsAndColumns()
    {
        var boardConfig = new BoardSettings(width: 50, height: 20, cellSize: 2);
        var testBoard = new Board(boardConfig);

        int expectedColumns = boardConfig.Width / boardConfig.CellSize;
        int expectedRows = boardConfig.Height / boardConfig.CellSize;

        Assert.AreEqual(expectedColumns, testBoard.Columns);
        Assert.AreEqual(expectedRows, testBoard.Rows);
    }

    [TestMethod]
    public void ValidateCellNeighbourConnections_WithNewBoard_CorrectCountInEachCell()
    {
        var boardConfig = new BoardSettings(width: 3, height: 3, cellSize: 1);
        var testBoard = new Board(boardConfig);

        int expectedNeighbourCount = 8;

        for (var row = 0; row < boardConfig.Height; row++)
        {
            for (var col = 0; col < boardConfig.Width; col++)
            {
                Assert.AreEqual(expectedNeighbourCount, testBoard.Cells[row, col].neighbours.Count);
            }
        }
    }

    [TestMethod]
    public void ValidateBoardSettingsLoading_FromExistingJsonFile_MatchingSettingsLoaded()
    {
        var initialSettings = new BoardSettings(width: 50, height: 20, cellSize: 1, liveDensity: 0.5);
        FileManager.SaveBoardSettings(initialSettings, filePath: "../../../boardSettings.json");

        var loadedSettings = FileManager.LoadBoardSettings(filePath: "../../../boardSettings.json");

        Assert.AreEqual(initialSettings.Width, loadedSettings.Width);
        Assert.AreEqual(initialSettings.Height, loadedSettings.Height);
        Assert.AreEqual(initialSettings.CellSize, loadedSettings.CellSize);
        Assert.AreEqual(initialSettings.LiveDensity, loadedSettings.LiveDensity);
    }
    
    [TestMethod]
    public void ValidateSaveBoardState_WithValidPath_FileIsCreated()
    {
        var testFilePath = "../../../SaveDataGame.txt";

        FileManager.SaveBoardState(new Board(), testFilePath);

        Assert.IsTrue(File.Exists(testFilePath));
    }

    [TestMethod]
    public void TestLoadBoardState_FileExists_VerifyLoadedParameters()
    {
        var initialBoardSettings = new BoardSettings(50, 20, 1);
        var testFilePath = "../../../SaveDataGame.txt";
        var board = new Board(initialBoardSettings);

        FileManager.SaveBoardState(board, testFilePath);
        board = FileManager.LoadBoardState(testFilePath);

        Assert.AreEqual(initialBoardSettings.Width, board.Width);
        Assert.AreEqual(initialBoardSettings.Height, board.Height);
        Assert.AreEqual(initialBoardSettings.CellSize, board.CellSize);
    }

    [TestMethod]
    public void CountLiveCells_OnBoardWithAllAlive_VerifiesCorrectCount()
    {
        var board = new Board(new BoardSettings(3, 3, 1, 1));

        Assert.AreEqual(9, BoardAnalytics.GetAliveCellsCount(board));
    }

    [TestMethod]
    public void TestFigureEquality_ForIdenticalFigures_ShouldReturnTrue()
    {
        var firstFigure = new Figure("Korovai", 6, 6, "        **   *  *   * *    *        ");
        var secondFigure = new Figure("Korovai", 6, 6, "        **   *  *   * *    *        ");

        Assert.IsTrue(firstFigure.Equals(secondFigure));
    }

    [TestMethod]
    public void CheckSaveFigures_WithValidParameters_VerifyFileExists()
    {
        var figures = new Figure[]
        {
        new Figure("Korovai", 6, 6, "        **   *  *   * *    *        ")
        };
        var testFilePath = "../../../figures.json";

        FileManager.SaveFigures(figures, testFilePath);

        Assert.IsTrue(File.Exists(testFilePath));
    }

    [TestMethod]
    public void ValidateLoadFigures_FromValidPath_VerifiesLoadedFigures()
    {
        var figures = new Figure[]
        {
        new Figure("Korovai", 6, 6, "        **   *  *   * *    *        ")
        };
        var testFilePath = "../../../figures.json";
        FileManager.SaveFigures(figures, testFilePath);

        var loadedFigures = FileManager.LoadFigures(testFilePath);

        Assert.IsTrue(figures[0].Equals(loadedFigures[0]));
    }
}
