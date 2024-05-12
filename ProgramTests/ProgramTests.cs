using cli_life;
using System.Numerics;
using System.Text.Json;

namespace ProgramTests;

[TestClass]
public class ProgramTests
{   
    
    [TestMethod]
    public void AliveCellThreeNeighbours_CellLives()
    {
        Cell neighbour;
        var cell = new Cell();
        cell.IsAlive = true;

        for (var index = 0; index < 3; index++)
        {
            neighbour = new Cell();
            neighbour.IsAlive = true;

            cell.neighbours.Add(neighbour);
        }

        Assert.AreEqual(cell.DetermineNextLiveState(), true);
    }
    [TestMethod]
    public void CellIsAlive()
    {
        Cell neighbour;
        var cell = new Cell();
        cell.IsAlive = true;

        for (var index = 0; index < 3; index++)
        {
            neighbour = new Cell();
            neighbour.IsAlive = true;

            cell.neighbours.Add(neighbour);
        }

        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.AreEqual(cell.IsAlive, true);
    }
    [TestMethod]
    public void LessThanThreeNeighbours_NoNewCell()
    {
        Cell neighbour;
        var cell = new Cell();
        cell.IsAlive = false;

        for (var index = 0; index < 2; index++)
        {
            neighbour = new Cell();
            neighbour.IsAlive = true;

            cell.neighbours.Add(neighbour);
        }

        Assert.AreEqual(cell.DetermineNextLiveState(), false);
    }

    [TestMethod]
    public void SaveFigures_CorrentParameters_SavesFigures()
    {
        var figures = new Figure[]
        {
            new Figure("Pond", 6, 6, "        **   *  *  *  *   **        ")
        };
        var filePath = "../../../figures_pond.json";

        DataHandler.SaveFigures(figures, filePath);

        Assert.IsTrue(File.Exists(filePath));
    }

    
    [TestMethod]
    public void CellIsNotAlive()
    {
        Cell neighbour;
        var cell = new Cell();
        cell.IsAlive = true;

        for (var index = 0; index < 4; index++)
        {
            neighbour = new Cell();
            neighbour.IsAlive = true;

            cell.neighbours.Add(neighbour);
        }

        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.AreEqual(cell.IsAlive, false);
    }
    [TestMethod]
    public void CorrectNumberRowsColumns()
    {
        var settings = new ConfigurationPanel(50, 20, 2);
        var board = new Board(settings);

        Assert.AreEqual(board.Columns, settings.PanelWidth  / settings.UnitBlockSize);
        Assert.AreEqual(board.Rows, settings.PanelHeight / settings.UnitBlockSize);
    }
    [TestMethod]
    public void AliveCellMoreThanThreeNeighbours_CellDies()
    {
        Cell neighbour;
        var cell = new Cell();
        cell.IsAlive = true;

        for (var index = 0; index < 4; index++)
        {
            neighbour = new Cell();
            neighbour.IsAlive = true;

            cell.neighbours.Add(neighbour);
        }

        Assert.AreEqual(cell.DetermineNextLiveState(), false);
    }
    [TestMethod]
    public void LoadsBoardSettings()
    {
        var originalSettings = new ConfigurationPanel(50, 20, 1, 0.5);
        DataHandler.SaveBoardSettings(originalSettings, "../../../Settings_board.json");
        var jsonSettings = DataHandler.LoadBoardSettings("../../../Settings_board.json");
        Assert.AreEqual(originalSettings.PanelWidth , jsonSettings.PanelWidth );
        Assert.AreEqual(originalSettings.PanelHeight, jsonSettings.PanelHeight);
        Assert.AreEqual(originalSettings.UnitBlockSize, jsonSettings.UnitBlockSize);
        Assert.AreEqual(originalSettings.PopulationDensity, jsonSettings.PopulationDensity);
    }    
    [TestMethod]
    public void SaveBoardState_CorrectFilePath_CreatesFile()
    {
        var filePath = "../../../Life_save1.txt";

        DataHandler.SaveBoardState(new Board(), filePath);

        Assert.IsTrue(File.Exists(filePath));
    }
    [TestMethod]
    public void LoadBoardState_FileExists_LoadsCorrectParameters()
    {
        var firstBoardSettings = new ConfigurationPanel(45, 25, 1);
        var filePath = "../../../Life_save1.txt";
        var board = new Board(firstBoardSettings);

        DataHandler.SaveBoardState(board, filePath);
        board = DataHandler.LoadBoardState(filePath);

        Assert.AreEqual(firstBoardSettings.PanelWidth , board.Width);
        Assert.AreEqual(firstBoardSettings.PanelHeight, board.Height);
        Assert.AreEqual(firstBoardSettings.UnitBlockSize, board.CellSize);
    }
    [TestMethod]
    public void GetAliveCellsCount_BoardWithAliveCells_ReturnsCorrectAmount()
    {
        var board = new Board(new ConfigurationPanel(3, 3, 1, 1));

        Assert.AreEqual(9, Analys.GetAlive(board));
    }   
    [TestMethod]
    public void FigureEquals_EqualFigures_ReturnsTrue()
    {
        var firstFigure =
            new Figure("FigureOne", 4, 4, "      **    **      ");
        var secondFigure =
            new Figure("FigureTwo", 4, 4, "      **    **      ");

        Assert.IsTrue(firstFigure.Equals(secondFigure));
    }   
   
    [TestMethod]
    public void ConnectsCellNeighbours()
    {
        var settings = new ConfigurationPanel(3, 3, 1);

        var board = new Board(settings);

        for (var row = 0; row < settings.PanelHeight; row++)
        {
            for (var col = 0; col < settings.PanelWidth; col++)
            {
                Assert.AreEqual(8, board.Cells[row, col].neighbours.Count);
            }
        }
    }

    [TestMethod]
    public void LoadFigures_CorrentPath_LoadsFigures()
    {
        var figures = new Figure[]
        {
            new Figure("Pond", 6, 6, "        **   *  *  *  *   **        ")
        };
        var filePath = "../../../figures_pond.json";
        DataHandler.SaveFigures(figures, filePath);

        var loadedFigures = DataHandler.LoadFigures(filePath);

        Assert.IsTrue(figures[0].Equals(loadedFigures[0]));
    }
    [TestMethod]
    public void TwoOrThreeNeighbours_NewAliveCell()
    {
        Cell neighbour;
        var cell = new Cell();
        cell.IsAlive = false;

        for (var index = 0; index < 3; index++)
        {
            neighbour = new Cell();
            neighbour.IsAlive = true;

            cell.neighbours.Add(neighbour);
        }

        Assert.AreEqual(cell.DetermineNextLiveState(), true);
    }
}
