using cli_life;
using ScottPlot;
using System.Numerics;
using System.Text.Json;

namespace TestsForProgram;

[TestClass]
public class UnitTestsForProgram
{
    [TestMethod]
    public void TestLivingCellWithThreeNeighbors_Survives()
    {
        Cell adjacent;
        var testCell = new Cell { IsAlive = true };

        for (int i = 0; i < 3; i++)
        {
            adjacent = new Cell { IsAlive = true };
            testCell.neighbours.Add(adjacent);
        }

        Assert.IsTrue(testCell.DetermineNextLiveState());
    }

    [TestMethod]
    public void VerifyCellSurvivesWithThreeNeighbors()
    {
        Cell adjacent;
        var testCell = new Cell { IsAlive = true };

        for (int counter = 0; counter < 3; counter++)
        {
            adjacent = new Cell { IsAlive = true };
            testCell.neighbours.Add(adjacent);
        }

        testCell.DetermineNextLiveState();
        testCell.ProceedToNextState();

        Assert.IsTrue(testCell.IsAlive);
    }

    [TestMethod]
    public void TestWithoutEnoughNeighbors_DeathOccurs()
    {
        Cell adjacent;
        var testCell = new Cell { IsAlive = false };

        for (int count = 0; count < 2; count++)
        {
            adjacent = new Cell { IsAlive = true };
            testCell.neighbours.Add(adjacent);
        }

        Assert.IsFalse(testCell.DetermineNextLiveState());
    }

    [TestMethod]
    public void VerifySavingOfGeometry_CorrectParams_DataPreserved()
    {
        var geometries = new[] { new Figure("Pond", 6, 6, "        **   *  *  *  *   **        ") };
        string path = "../../../.json";

        DataHandler.StoreGeometries(geometries, path);

        Assert.IsTrue(File.Exists(path));
    }

    [TestMethod]
    public void ValidateCellPerishes()
    {
        Cell adjacent;
        var testCell = new Cell { IsAlive = true };

        for (int idx = 0; idx < 4; idx++)
        {
            adjacent = new Cell { IsAlive = true };
            testCell.neighbours.Add(adjacent);
        }

        testCell.DetermineNextLiveState();
        testCell.ProceedToNextState();

        Assert.IsFalse(testCell.IsAlive);
    }

    [TestMethod]
    public void AssertBoardDimensionsCorrect()
    {
        var uiSettings = new ConfigurationPanel(50, 20, 2);
        var grid = new Board(uiSettings);

        Assert.AreEqual(grid.Columns, uiSettings.PanelWidth / uiSettings.UnitBlockSize);
        Assert.AreEqual(grid.Rows, uiSettings.PanelHeight / uiSettings.UnitBlockSize);
    }

    [TestMethod]
    public void LivingCellWithOverpopulation_Perishes()
    {
        Cell adjacent;
        var testCell = new Cell { IsAlive = true };

        for (int iter = 0; iter < 4; iter++)
        {
            adjacent = new Cell { IsAlive = true };
            testCell.neighbours.Add(adjacent);
        }

        Assert.IsFalse(testCell.DetermineNextLiveState());
    }

    [TestMethod]
    public void ValidateSettingsRetrieval()
    {
        var expectedSettings = new ConfigurationPanel(50, 20, 1, 0.5);
        DataHandler.SaveBoardSettings(expectedSettings, "../../../Settings_board.json");
        var retrievedSettings = DataHandler.LoadPanelConfig("../../../Settings_board.json");

        Assert.AreEqual(expectedSettings.PanelWidth, retrievedSettings.PanelWidth);
        Assert.AreEqual(expectedSettings.PanelHeight, retrievedSettings.PanelHeight);
        Assert.AreEqual(expectedSettings.UnitBlockSize, retrievedSettings.UnitBlockSize);
        Assert.AreEqual(expectedSettings.PopulationDensity, retrievedSettings.PopulationDensity);
    }

    [TestMethod]
    public void CheckGameStateSaving_ProperPath_FileGenerated()
    {
        string path = "../../../Live_save1.txt";
        DataHandler.SaveGridState(new Board(), path);

        Assert.IsTrue(File.Exists(path));
    }

    [TestMethod]
    public void ConfirmStateLoading_FilePresent_CorrectDataFetched()
    {
        var initialSettings = new ConfigurationPanel(45, 25, 1);
        string path = "../../../Live_save1.txt";
        var gameBoard = new Board(initialSettings);

        DataHandler.SaveGridState(gameBoard, path);
        gameBoard = DataHandler.LoadGridState(path);

        Assert.AreEqual(initialSettings.PanelWidth, gameBoard.Width);
        Assert.AreEqual(initialSettings.PanelHeight, gameBoard.Height);
        Assert.AreEqual(initialSettings.UnitBlockSize, gameBoard.CellSize);
    }

    [TestMethod]
    public void CountLivingCells_OnPopulatedBoard_AccurateCount()
    {
        var gameBoard = new Board(new ConfigurationPanel(3, 3, 1, 1));
        Assert.AreEqual(9, gameBoard.CountLivingCells());
    }

    [TestMethod]
    public void CompareFigures_Identical_ReturnTrue()
    {
        var first = new Figure("ShapeOne", 4, 4, "    **    **    ");
        var second = new Figure("ShapeTwo", 4, 4, "    **    **    ");

        Assert.IsTrue(first.Equals(second));
    }

    [TestMethod]
    public void EnsureNeighbourConnections_CorrectCount()
    {
        var settings = new ConfigurationPanel(3, 3, 1);
        var gameBoard = new Board(settings);

        foreach (var cell in gameBoard.Cells)
        {
            Assert.AreEqual(8, cell.neighbours.Count);
        }
    }

    [TestMethod]
    public void ValidateGeometryLoading_CorrectPath_CorrectData()
    {
        var geometries = new[] { new Figure("Pond", 6, 6, "        **   *  *  *  *   **        ") };
        string path = "../../../Settings_board.json";
        DataHandler.StoreGeometries(geometries, path);

        var retrievedGeometries = DataHandler.LoadGeometries(path);

        Assert.IsTrue(geometries[0].Equals(retrievedGeometries[0]));
    }

    [TestMethod]
    public void ReviveCellWithTwoOrThreeNeighbors_BecomesAlive()
    {
        Cell adjacent;
        var testCell = new Cell { IsAlive = false };

        for (int i = 0; i < 3; i++)
        {
            adjacent = new Cell { IsAlive = true };
            testCell.neighbours.Add(adjacent);
        }

        Assert.IsTrue(testCell.DetermineNextLiveState());
    }
}
     