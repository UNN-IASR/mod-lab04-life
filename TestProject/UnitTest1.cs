using game_life;

namespace TestProject;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void CheckCellLife_OvercrowdingLeadsToDeath()
    {
        var CellTest = new Cell { IsAlive = true };
        var maxAliveNeighbours = 4;

        for (var index = 0; index < maxAliveNeighbours; index++)
        {
            var livingNeighbour = new Cell { IsAlive = true };
            CellTest.neighbours.Add(livingNeighbour);
        }

        var nextState = CellTest.DetermineNextLiveState();

        Assert.AreEqual(nextState, false);
    }

    [TestMethod]
    public void ValidateLifeCycle_AliveWithThreeNeighbors_StaysAlive()
    {
        var initialCell = new Cell { IsAlive = true };
        var maxAliveNeighbors = 3;

        for (var index = 0; index < maxAliveNeighbors; index++)
        {
            var aliveNeighbor = new Cell { IsAlive = true };
            initialCell.neighbours.Add(aliveNeighbor);
        }

        var actualState = initialCell.DetermineNextLiveState();

        Assert.AreEqual(true, actualState);
    }

    [TestMethod]
    public void ValidateLifeCycle_DeadCellWithThreeNeighbours_BecomesAlive()
    {
        var CellTest = new Cell { IsAlive = false };
        var aliveNeighbours = 3;

        for (var index = 0; index < aliveNeighbours; index++)
        {
            var aliveNeighbour = new Cell { IsAlive = true };
            CellTest.neighbours.Add(aliveNeighbour);
        }

        bool actualState = CellTest.DetermineNextLiveState();

        Assert.AreEqual(true, actualState);
    }

    [TestMethod]
    public void ValidateLifeCycle_DeadCellWithTwoNeighbours_StaysDead()
    {
        var CellTest = new Cell { IsAlive = false };
        var numOfNeighbours = 2;

        for (var i = 0; i < numOfNeighbours; i++)
        {
            var aliveNeighbour = new Cell { IsAlive = true };
            CellTest.neighbours.Add(aliveNeighbour);
        }

        var actualState = CellTest.DetermineNextLiveState();

        Assert.AreEqual(false, actualState);
    }

    [TestMethod]
    public void CheckLifeCycle_AliveCellRemainsAlive_AfterAdvancing()
    {
        var CellTest = new Cell { IsAlive = true };
        var numOfNeighbours = 3;

        for (var i = 0; i < numOfNeighbours; i++)
        {
            var aliveNeighbour = new Cell { IsAlive = true };
            CellTest.neighbours.Add(aliveNeighbour);
        }

        CellTest.DetermineNextLiveState();
        CellTest.Advance();

        Assert.AreEqual(true, CellTest.IsAlive);
    }

    [TestMethod]
    public void CheckLifeCycle_AliveCellDies_AfterAdvancing()
    {
        var CellTest = new Cell { IsAlive = true };
        var numOfNeighbours = 4;

        for (var i = 0; i < numOfNeighbours; i++)
        {
            CellTest.neighbours.Add(new Cell { IsAlive = true });
        }

        CellTest.DetermineNextLiveState();
        CellTest.Advance();

        Assert.AreEqual(false, CellTest.IsAlive);
    }

    [TestMethod]
    public void CheckBoardCreation_WithCellSizeGreaterThanOne_ValidatesRowsAndColumns()
    {
        var boardConfig = new BoardSettings(50, 20, 2);
        var testBoard = new Board(boardConfig);

        int expectedColumns = boardConfig.Width / boardConfig.CellSize;
        int expectedRows = boardConfig.Height / boardConfig.CellSize;

        Assert.AreEqual(expectedColumns, testBoard.Columns);
        Assert.AreEqual(expectedRows, testBoard.Rows);
    }

    [TestMethod]
    public void ValidateCellNeighbourConnections_WithNewBoard_CorrectCountInEachCell()
    {
        var boardConfig = new BoardSettings(3, 3, 1);
        var testBoard = new Board(boardConfig);

        for (var row = 0; row < boardConfig.Height; row++)
        {
            for (var col = 0; col < boardConfig.Width; col++)
            {
                Assert.AreEqual(8, testBoard.Cells[row, col].neighbours.Count);
            }
        }
    }

    [TestMethod]
    public void ValidateBoardSettingsLoading_FromExistingJsonFile_MatchingSettingsLoaded()
    {
        var initialSettings = new BoardSettings(50, 20, 1, 0.5);
        initialSettings.Save("../../../SaveDataGame.txt");

        var loadedSettings = BoardSettings.Load("../../../boardSettings.json");

        Assert.AreEqual(initialSettings.Width, loadedSettings.Width);
        Assert.AreEqual(initialSettings.Height, loadedSettings.Height);
        Assert.AreEqual(initialSettings.CellSize, loadedSettings.CellSize);
        Assert.AreEqual(initialSettings.LiveDensity, loadedSettings.LiveDensity);
    }

    [TestMethod]
    public void ValidateSaveBoardState_WithValidPath_FileIsCreated()
    {
        new Board().Save("../../../SaveDataGame.txt");
        Assert.IsTrue(File.Exists("../../../SaveDataGame.txt"));
    }

    [TestMethod]
    public void TestLoadBoardState_FileExists_VerifyLoadedParameters()
    {
        var initialSettings = new BoardSettings(50, 20, 1);
        var board = new Board(initialSettings);

        board.Save("../../../SaveDataGame.txt");
        board = Board.Load("../../../SaveDataGame.txt");

        Assert.AreEqual(initialSettings.Width, board.Width);
        Assert.AreEqual(initialSettings.Height, board.Height);
        Assert.AreEqual(initialSettings.CellSize, board.CellSize);
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
        var Shape1 = new Shape("Korovai", 6, 6, "        **   *  *   * *    *        ");
        var Shape2 = new Shape("Korovai", 6, 6, "        **   *  *   * *    *        ");

        Assert.IsTrue(Shape1.Equals(Shape2));
    }

    [TestMethod]
    public void CheckSaveFigures_WithValidParameters_VerifyFileExists()
    {
        var shapes = new Shape[]
        {
        new Shape("Korovai", 6, 6, "        **   *  *   * *    *        ")
        };
        var testFilePath = "../../../shapes.json";

        IOmanager.SaveShapes(shapes, testFilePath);

        Assert.IsTrue(File.Exists(testFilePath));
    }

    [TestMethod]
    public void ValidateLoadFigures_FromValidPath_VerifiesLoadedFigures()
    {
        var shapes = new Shape[]
        {
        new Shape("Korovai", 6, 6, "        **   *  *   * *    *        ")
        };

        IOmanager.SaveShapes(shapes, "../../../shapes.json");

        var loadedShapes = IOmanager.LoadShapes("../../../shapes.json");

        Assert.IsTrue(shapes[0].Equals(loadedShapes[0]));
    }
}
