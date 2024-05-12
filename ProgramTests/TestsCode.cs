using cli_life;
using System.Numerics;
using System.Text.Json;

namespace ProgramTests;

[TestClass]
public class TestsCode
{
    [TestMethod]
    public void WhenSavingFigures_ShouldSaveFigures()
    {
        var expectedFigures = new List<Figure>
    {
        new Figure(name: "LongBarge", width: 7, height: 7, formatFigure: "         *     * *     * *     * *     *         ")
    };
        var targetFilePath = Path.Combine("../../../", "figuresFile.json");

        WorkingWithFiles.SaveFigures(expectedFigures.ToArray(), targetFilePath);

        bool fileExists = File.Exists(targetFilePath);
        Assert.IsTrue(fileExists, "Файл был сохранен неправильно.");
    }

    [TestMethod]
    public void WhenLoadingFigures_ShouldLoadFigures()
    {
        var expectedFigures = new List<Figure>
    {
        new Figure(name: "LongBarge", width: 7, height: 7, formatFigure: "         *     * *     * *     * *     *         ")
    };
        var targetFilePath = Path.Combine("../../../", "figuresFile.json");

        WorkingWithFiles.SaveFigures(expectedFigures.ToArray(), targetFilePath);

        var actualFigures = WorkingWithFiles.LoadFigures(targetFilePath);

        var areEqual = expectedFigures[0].Equals(actualFigures[0]);
        Assert.IsTrue(areEqual, "Загруженная фигура не соответствует ожидаемой.");
    }

    [TestMethod]
    public void WhenAliveCellHasThreeAliveNeighbours()
    {
        var cell = new Cell { IsAlive = true };
        var neighbours = Enumerable.Range(0, 3).Select(_ => new Cell { IsAlive = true });

        foreach (var neighbour in neighbours)
        {
            cell.neighbours.Add(neighbour);
        }

        bool nextLiveState = cell.DetermineNextLiveState();

        Assert.IsTrue(nextLiveState);
    }

    [TestMethod]
    public void TwoOrThreeNeighbours_NewAliveCell()
    {
        Cell neighbour;
        var cell = new Cell { IsAlive = false };

        for (var i = 0; i < 3; i++)
        {
            neighbour = new Cell { IsAlive = true };
            cell.neighbours.Add(neighbour);
        }

        bool expected = true;
        bool actual = cell.DetermineNextLiveState();
        Assert.AreEqual(expected, actual, "Ожидается, что ячейка будет оживлена с тремя соседями.");
    }

    [TestMethod]
    public void ShouldCreateFileWithCorrectPath()
    {
        string filePath = "../../../savesFile_1.txt";

        WorkingWithFiles.SaveBoardState(new Board(), filePath);

        Assert.IsTrue(File.Exists(filePath), "The file was not created.");
    }

    [TestMethod]
    public void ShouldLoadCorrectBoardParametersFromExistingFile()
    {
        SettingUpBorders boardSettings = new SettingUpBorders(50, 20, 1);
        string filePath = "../../../savesFile_2.txt";
        Board expectedBoard = new Board(boardSettings);

        WorkingWithFiles.SaveBoardState(expectedBoard, filePath);
        Board actualBoard = WorkingWithFiles.LoadBoardState(filePath);

        Assert.AreEqual(boardSettings.Width, actualBoard.Width, "Width property is not equal.");
        Assert.AreEqual(boardSettings.Height, actualBoard.Height, "Height property is not equal.");
        Assert.AreEqual(boardSettings.CellSize, actualBoard.CellSize, "CellSize property is not equal.");
    }

    [TestMethod]
    public void WhenLessThanThreeNeighbours_DoesNotCreateNewCell()
    {
        Cell neighbour;
        var cell = new Cell { IsAlive = false };

        for (int index = 0; index < 2; index++)
        {
            neighbour = new Cell { IsAlive = true };
            cell.neighbours.Add(neighbour);
        }

        Assert.IsFalse(cell.DetermineNextLiveState());
    }

    [TestMethod]
    public void AliveCellMoreThanThreeNeighbours()
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
    public void IsAliveCellIsAliveNext()
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
    public void CellSizeGreaterThanOne()
    {
        var settings = new SettingUpBorders(60, 30, 3);
        var board = new Board(settings);

        Assert.AreEqual(board.Columns, settings.Width / settings.CellSize);
        Assert.AreEqual(board.Rows, settings.Height / settings.CellSize);
    }

    [TestMethod]
    public void Board_ConnectsCellNeighbours()
    {
        var settings = new SettingUpBorders(3, 3, 1);

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
    public void LoadBoardSettings()
    {
        var originalSettings = new SettingUpBorders(50, 20, 1, 0.5);
        WorkingWithFiles.SaveBoardSettings(originalSettings, "../../../boardSettings.json");

        var jsonSettings = WorkingWithFiles.LoadBoardSettings("../../../boardSettings.json");

        Assert.AreEqual(originalSettings.Width, jsonSettings.Width);
        Assert.AreEqual(originalSettings.Height, jsonSettings.Height);
        Assert.AreEqual(originalSettings.CellSize, jsonSettings.CellSize);
        Assert.AreEqual(originalSettings.LiveDensity, jsonSettings.LiveDensity);
    }

    [TestMethod]
    public void IsAliveCellIsNotAliveNext()
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
    public void GetAliveCellsCountReturnsCorrectAmount()
    {
        var board = new Board(new SettingUpBorders(3, 3, 1, 1));

        Assert.AreEqual(9, AnalysisOfBorder.GetAliveCellsCount(board));
    }

    [TestMethod]
    public void FigureEquals()
    {
        var firstFigure =
            new Figure(name: "LongBarge", width: 7, height: 7, formatFigure: "         *     * *     * *     * *     *         ");
        var secondFigure =
            new Figure(name: "LongBarge", width: 7, height: 7, formatFigure: "         *     * *     * *     * *     *         ");

        Assert.IsTrue(firstFigure.Equals(secondFigure));
    }
}
