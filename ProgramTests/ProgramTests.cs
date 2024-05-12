using cli_life;
using System.Numerics;
using System.Text.Json;

namespace ProgramTests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void Test_FindAliveCellCountWithEmptyBoard_ReturnsZero()
    {
        var board = new Board(new BoardSettings(3, 3, 1, 0));
        Assert.AreEqual(0, BoardAnalysis.FindAliveCellsCount(board));
    }

    [TestMethod]
    public void Test_Equals_ReturnsTrue_ForEquivalentFigures()
    {
        var figure1 =
            new Figure("FigureOne", 6, 6, "        *    * *    * *    *        ");
        var figure2 =
            new Figure("FigureTwo", 6, 6, "        *    * *    * *    *        ");

        Assert.IsTrue(figure1.Equals(figure2));
    }

    [TestMethod]
    public void Test_FiguresSaved_ToJsonFile_OnProvidedPath()
    {
        var figures = new Figure[]
        {
        new Figure("Loaf", 6, 6, "        **   *  *   * *    *        ")
        };
        string path = "../../../figures.json";

        FileManager.SaveFigures(figures, path);

        Assert.IsTrue(File.Exists(path));
    }

    [TestMethod]
    public void LoadBoardSettings_FileDoesNotExist_ThrowsFileNotFoundException()
    {
        Assert.ThrowsException<FileNotFoundException>(
            () => FileManager.LoadBoardSettings("non_existent_file.json"));
    }

    [TestMethod]
    public void Test_EqualFigures_SameReference_ReturnsTrue()
    {
        var figure =
            new Figure("FigureOne", 4, 4, "      **    **      ");
        Assert.IsTrue(figure.Equals(figure));
    }

    [TestMethod]
    public void Test_NewCellAppears_When_ItHasExactlyThreeNeighbours()
    {
        var cell = new Cell { IsAlive = false };

        for (var i = 0; i < 3; i++)
        {
            var neighbour = new Cell { IsAlive = true };
            cell.neighbours.Add(neighbour);
        }

        Assert.IsTrue(cell.DetermineNextLiveState());
    }

    [TestMethod]
    public void Test_CorrectRowAndColumnCount_When_CellSizeGreaterOne()
    {
        var settings = new BoardSettings(50, 20, 2);
        var board = new Board(settings);

        Assert.AreEqual(settings.Width / settings.CellSize, board.Columns);
        Assert.AreEqual(settings.Height / settings.CellSize, board.Rows);
    }

    [TestMethod]
    public void Test_EveryCell_ConnectedWith_EightNeighbours()
    {
        var settings = new BoardSettings(3, 3, 1);
        var board = new Board(settings);

        for (var row = 0; row < settings.Height; row++)
        {
            for (var col = 0; col < settings.Width; col++)
            {
                Assert.AreEqual(8, board.Cells[row, col].neighbours.Count);
            }
        }
    }

    [TestMethod]
    public void Test_NoNewCellAppears_When_LessThanThreeNeighbours()
    {
        var cell = new Cell { IsAlive = false };

        for (var i = 0; i < 2; i++)
        {
            var neighbour = new Cell { IsAlive = true };
            cell.neighbours.Add(neighbour);
        }

        Assert.IsFalse(cell.DetermineNextLiveState());
    }

    [TestMethod]
    public void Test_CellRemainsAlive_When_NextStateIsAlive()
    {
        var cell = new Cell { IsAlive = true };

        for (var i = 0; i < 3; i++)
        {
            var neighbour = new Cell { IsAlive = true };
            cell.neighbours.Add(neighbour);
        }

        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.IsTrue(cell.IsAlive);
    }

    [TestMethod]
    public void Test_CellDies_When_NextStateIsDead()
    {
        var cell = new Cell { IsAlive = true };

        for (var i = 0; i < 4; i++)
        {
            var neighbour = new Cell { IsAlive = true };
            cell.neighbours.Add(neighbour);
        }

        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.IsFalse(cell.IsAlive);
    }

    [TestMethod]
    public void Test_BoardSettingsLoaded_FromExistingJsonFile()
    {
        var originalSettings = new BoardSettings(50, 20, 1, 0.5);
        string path = "../../../boardSettings.json";
        FileManager.SaveBoardSettings(originalSettings, path);

        var loadedSettings = FileManager.LoadBoardSettings(path);

        Assert.AreEqual(originalSettings.Width, loadedSettings.Width);
        Assert.AreEqual(originalSettings.Height, loadedSettings.Height);
        Assert.AreEqual(originalSettings.CellSize, loadedSettings.CellSize);
        Assert.AreEqual(originalSettings.LiveDensity, loadedSettings.LiveDensity);
    }

    [TestMethod]
    public void Test_FileIsCreated_When_BoardStateSaved()
    {
        string path = "../../../resultGameOfLife.txt";
        FileManager.SaveBoardState(new Board(), path);
        Assert.IsTrue(File.Exists(path));
    }

    [TestMethod]
    public void Test_BoardParametersCorrectlyLoaded_FromExistingFile()
    {
        var initialBoardSettings = new BoardSettings(50, 20, 1);
        string path = "../../../resultGameOfLife.txt";
        var board = new Board(initialBoardSettings);

        FileManager.SaveBoardState(board, path);
        var loadedBoard = FileManager.LoadBoardState(path);

        Assert.AreEqual(initialBoardSettings.Width, loadedBoard.Width);
        Assert.AreEqual(initialBoardSettings.Height, loadedBoard.Height);
        Assert.AreEqual(initialBoardSettings.CellSize, loadedBoard.CellSize);
    }

    [TestMethod]
    public void Test_CorrectAliveCellCount_Returned_ForBoardWithAliveCells()
    {
        var board = new Board(new BoardSettings(3, 3, 1, 1));
        Assert.AreEqual(9, BoardAnalysis.FindAliveCellsCount(board));
    }

    [TestMethod]
    public void Test_FiguresLoaded_FromJsonFile_OnProvidedPath()
    {
        var figures = new Figure[]
        {
        new Figure("Block", 4, 4, "      **    **      ")
        };
        string path = "../../../figures.json";
        FileManager.SaveFigures(figures, path);

        var loadedFigures = FileManager.LoadFigures(path);

        Assert.IsTrue(figures[0].Equals(loadedFigures[0]));
    }

    [TestMethod]
    public void Test_CellDies_When_MoreThanThreeNeighbours()
    {
        var cell = new Cell { IsAlive = true };

        for (var i = 0; i < 4; i++)
        {
            var neighbour = new Cell { IsAlive = true };
            cell.neighbours.Add(neighbour);
        }

        Assert.IsFalse(cell.DetermineNextLiveState());
    }

    [TestMethod]
    public void Test_CellLives_When_HasTwoOrThreeNeighbours()
    {
        var cell = new Cell { IsAlive = true };

        for (var i = 0; i < 3; i++)
        {
            var neighbour = new Cell { IsAlive = true };
            cell.neighbours.Add(neighbour);
        }

        Assert.IsTrue(cell.DetermineNextLiveState());
    }
}
