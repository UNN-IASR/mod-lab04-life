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
        var board = new Board(new BoardConfigurations(3, 3, 1, 0));
        Assert.AreEqual(0, TableEvaluationStrategy.FindAliveCellsCount(board));
    }

    [TestMethod]
    public void Test_Equals_ReturnsTrue_ForEquivalentEntities()
    {
        var entity1 =
            new Entity("EntityOne", 6, 6, "        *    * *    * *    *        ");
        var entity2 =
            new Entity("EntityTwo", 6, 6, "        *    * *    * *    *        ");

        Assert.IsTrue(entity1.Equals(entity2));
    }

    [TestMethod]
    public void Test_EntitiesSaved_ToJsonFile_OnProvidedPath()
    {
        var entities = new Entity[]
        {
        new Entity("Loaf", 6, 6, "        **   *  *   * *    *        ")
        };
        string path = "../../../entities.json";

        DocumentHandler.SaveEntities(entities, path);

        Assert.IsTrue(File.Exists(path));
    }

    [TestMethod]
    public void LoadBoardConfigurations_FileDoesNotExist_ThrowsFileNotFoundException()
    {
        Assert.ThrowsException<FileNotFoundException>(
            () => DocumentHandler.LoadBoardConfiguration("non_existent_file.json"));
    }

    [TestMethod]
    public void Test_EqualEntities_SameReference_ReturnsTrue()
    {
        var entity =
            new Entity("EntityOne", 4, 4, "      **    **      ");
        Assert.IsTrue(entity.Equals(entity));
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
        var configurations = new BoardConfigurations(50, 20, 2);
        var board = new Board(configurations);

        Assert.AreEqual(configurations.Width / configurations.CellSize, board.Columns);
        Assert.AreEqual(configurations.Height / configurations.CellSize, board.Rows);
    }

    [TestMethod]
    public void Test_EveryCell_ConnectedWith_EightNeighbours()
    {
        var configurations = new BoardConfigurations(3, 3, 1);
        var board = new Board(configurations);

        for (var row = 0; row < configurations.Height; row++)
        {
            for (var col = 0; col < configurations.Width; col++)
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
    public void Test_BoardConfigurationsLoaded_FromExistingJsonFile()
    {
        var originalConfigurations = new BoardConfigurations(50, 20, 1, 0.5);
        string path = "../../../boardConfigurations.json";
        DocumentHandler.SaveBoardConfiguration(originalConfigurations, path);

        var loadedConfigurations = DocumentHandler.LoadBoardConfiguration(path);

        Assert.AreEqual(originalConfigurations.Width, loadedConfigurations.Width);
        Assert.AreEqual(originalConfigurations.Height, loadedConfigurations.Height);
        Assert.AreEqual(originalConfigurations.CellSize, loadedConfigurations.CellSize);
        Assert.AreEqual(originalConfigurations.LiveDensity, loadedConfigurations.LiveDensity);
    }

    [TestMethod]
    public void Test_FileIsCreated_When_BoardStateSaved()
    {
        string path = "../../../resultGameOfLife.txt";
        DocumentHandler.SaveBoardCondition(new Board(), path);
        Assert.IsTrue(File.Exists(path));
    }

    [TestMethod]
    public void Test_BoardParametersCorrectlyLoaded_FromExistingFile()
    {
        var initialBoardConfigurations = new BoardConfigurations(50, 20, 1);
        string path = "../../../resultGameOfLife.txt";
        var board = new Board(initialBoardConfigurations);

        DocumentHandler.SaveBoardCondition(board, path);
        var loadedBoard = DocumentHandler.LoadBoardCondition(path);

        Assert.AreEqual(initialBoardConfigurations.Width, loadedBoard.Width);
        Assert.AreEqual(initialBoardConfigurations.Height, loadedBoard.Height);
        Assert.AreEqual(initialBoardConfigurations.CellSize, loadedBoard.CellSize);
    }

    [TestMethod]
    public void Test_CorrectAliveCellCount_Returned_ForBoardWithAliveCells()
    {
        var board = new Board(new BoardConfigurations(3, 3, 1, 1));
        Assert.AreEqual(9, TableEvaluationStrategy.FindAliveCellsCount(board));
    }

    [TestMethod]
    public void Test_EntitiesLoaded_FromJsonFile_OnProvidedPath()
    {
        var entities = new Entity[]
        {
        new Entity("Block", 4, 4, "      **    **      ")
        };
        string path = "../../../entities.json";
        DocumentHandler.SaveEntities(entities, path);

        var loadEntities = DocumentHandler.LoadEntities(path);

        Assert.IsTrue(entities[0].Equals(loadEntities[0]));
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
