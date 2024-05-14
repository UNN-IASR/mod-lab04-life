using cli_life;
using ScottPlot;
using ScottPlot.Palettes;
using System.Numerics;
using System.Text.Json;

namespace TestsForProgram;

[TestClass]
public class UnitTestsForProgram
{
    // 义耱 1
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

        Assert.IsTrue(testCell.DelNextGameState());
    }
    // 义耱 2
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

        testCell.DelNextGameState();      

        Assert.IsTrue(testCell.IsAlive);
    }
    // 义耱 3
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
        Assert.AreEqual(testCell.DelNextGameState(), false);
        Assert.IsFalse(testCell.DelNextGameState());
    }
    // 义耱 4
    [TestMethod]
    public void VerifySavingOfGeometry_CorrectParams_DataPreserved()
    {
        var geometries = new[] { new Figure("Pond", 6, 6, "        **   *  *  *  *   **        ") };
        string path = "../../../.json";

        DataHandler.StoreGeometries(geometries, path);

        Assert.IsTrue(File.Exists(path));
    }
    // 义耱 5
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
                
        Assert.AreEqual(testCell.DelNextGameState(), false);        
    }
    // 义耱 6
    [TestMethod]
    public void AssertBoardDimensionsCorrect()
    {
        var uiSettings = new ConfigurationPanel(50, 20, 2);
        var grid = new GameBoard(uiSettings);

        Assert.AreEqual(grid.Columns, uiSettings.PanelWidth / uiSettings.UnitBlockSize);
        Assert.AreEqual(grid.Rows, uiSettings.PanelHeight / uiSettings.UnitBlockSize);
    }
    // 义耱 7
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

        Assert.IsFalse(testCell.DelNextGameState());
    }
    // 义耱 8
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
    // 义耱 9
    [TestMethod]
    public void CheckGameStateSaving_ProperPath_FileGenerated()
    {
        string path = "../../../Life_save1.txt";
        DataHandler.SaveGridState(new GameBoard(), path);

        Assert.IsTrue(File.Exists(path));
    }
    // 义耱 10
    [TestMethod]
    public void ConfirmStateLoading_FilePresent_CorrectDataFetched()
    {
        var initialSettings = new ConfigurationPanel(45, 25, 1);
        string path = "../../../Life_save1.txt";
        var gameBoard = new GameBoard(initialSettings);

        DataHandler.SaveGridState(gameBoard, path);
        gameBoard = DataHandler.LoadGridState(path);

        Assert.AreEqual(initialSettings.PanelWidth, gameBoard.Width);
        Assert.AreEqual(initialSettings.PanelHeight, gameBoard.Height);
        Assert.AreEqual(initialSettings.UnitBlockSize, gameBoard.CellSize);
    }
    // 义耱 11    
    [TestMethod]
    public void EnsureNeighbourConnections_CorrectCount()
    {
        var settings = new ConfigurationPanel(3, 3, 1);
        var gameBoard = new GameBoard(settings);

        foreach (var cell in gameBoard.Cells)
        {
            Assert.AreEqual(8, cell.neighbours.Count);
        }
    }
    // 义耱 12
    [TestMethod]
    public void ValidateGeometryLoading_CorrectPath_CorrectData()
    {
        var geometries = new[] { new Figure("Pond", 6, 6, "        **   *  *  *  *   **        ") };
        string path = "../../../Settings_board.json";
        DataHandler.StoreGeometries(geometries, path);

        var retrievedGeometries = DataHandler.LoadGeometries(path);

        Assert.IsTrue(geometries[0].Equals(retrievedGeometries[0]));
    }
    // 义耱 13
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

        Assert.IsTrue(testCell.DelNextGameState());
    }
    // 义耱 14
    [TestMethod]
    public void CountLivingCells_OnPopulatedBoard_AccurateCount()
    {
        var gameBoard = new GameBoard(new ConfigurationPanel(3, 3, 1, 1));
        Assert.AreEqual(9, Analys.CountAliveCells(gameBoard));
    }
    // 义耱 15
    [TestMethod]
    public void CompareFigures_Identical_ReturnTrue()
    {
        var first = new Figure("Pond", 6, 6, "        **   *  *  *  *   **        ");
        var second = new Figure("Pond", 6, 6, "        **   *  *  *  *   **        ");

        Assert.IsTrue(first.Equals(second));
    }
}
     