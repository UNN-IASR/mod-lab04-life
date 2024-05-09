using cli_life;
using System.Numerics;
using System.Text.Json;

namespace ProgramTests;

[TestClass]
public class ProgramTests
{
    /// <summary>
    /// Клетка умирает при числе соседей большем трёх.
    /// </summary>
    [TestMethod]
    public void DetermineNextLiveState_AliveCellMoreThanThreeNeighbours_CellDies()
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

    /// <summary>
    /// Клетка остается живой при числе соседей равном двум или трём.
    /// </summary>
    [TestMethod]
    public void DetermineNextLiveState_AliveCellThreeNeighbours_CellLives()
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

    /// <summary>
    /// При числе соседей равном трём появляется новая клетка.
    /// </summary>
    [TestMethod]
    public void DetermineNextLiveState_TwoOrThreeNeighbours_NewAliveCell()
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

    /// <summary>
    /// При числе соседей меньшем трёх новая клетка не появляется.
    /// </summary>
    [TestMethod]
    public void DetermineNextLiveState_LessThanThreeNeighbours_NoNewCell()
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

    /// <summary>
    /// При следующем состоянии живой клетки "жива" клетка остаётся живой.
    /// </summary>
    [TestMethod]
    public void Advance_IsAliveCellIsAliveNext_CellIsAlive()
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

    /// <summary>
    /// При следующем состоянии живой клетки "мертва" клетка умирает.
    /// </summary>
    [TestMethod]
    public void Advance_IsAliveCellIsNotAliveNext_CellIsNotAlive()
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

    /// <summary>
    /// При передаче параметра размера клетки больше единицы 
    /// корректно высчитывается число строк и столбцов массива клеток.
    /// </summary>
    [TestMethod]
    public void BoardConstructor_CellSizeGreaterThanOne_CorrectNumberRowsColumns()
    {
        var settings = new BoardSettings(50, 20, 2);
        var board = new Board(settings);

        Assert.AreEqual(board.Columns, settings.Width / settings.CellSize);
        Assert.AreEqual(board.Rows, settings.Height / settings.CellSize);
    }

    /// <summary>
    /// Соединяет каждую клетку с её соседями, число соседей равно восьми.
    /// </summary>
    [TestMethod]
    public void ConnectNeighbours_Board_ConnectsCellNeighbours()
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

    /// <summary>
    /// Загружает настройки доски из JSON-файла при его наличии.
    /// </summary>
    [TestMethod]
    public void LoadBoardSettings_JsonFileExists_LoadsBoardSettings()
    {
        var originalSettings = new BoardSettings(50, 20, 1, 0.5);
        FileManager.SaveBoardSettings(originalSettings, "../../../boardSettings.json");

        var jsonSettings = FileManager.LoadBoardSettings("../../../boardSettings.json");

        Assert.AreEqual(originalSettings.Width, jsonSettings.Width);
        Assert.AreEqual(originalSettings.Height, jsonSettings.Height);
        Assert.AreEqual(originalSettings.CellSize, jsonSettings.CellSize);
        Assert.AreEqual(originalSettings.LiveDensity, jsonSettings.LiveDensity);
    }

    /// <summary>
    /// Сохраняет результат в файл.
    /// </summary>
    [TestMethod]
    public void SaveBoardState_CorrectFilePath_CreatesFile()
    {
        var filePath = "../../../gameOfLife_sf1.txt";

        FileManager.SaveBoardState(new Board(), filePath);

        Assert.IsTrue(File.Exists(filePath));
    }

    /// <summary>
    /// Загружает корректные параметры доски из файла.
    /// </summary>
    [TestMethod]
    public void LoadBoardState_FileExists_LoadsCorrectParameters()
    {
        var firstBoardSettings = new BoardSettings(50, 20, 1);
        var filePath = "../../../gameOfLife_sf2.txt";
        var board = new Board(firstBoardSettings);

        FileManager.SaveBoardState(board, filePath);
        board = FileManager.LoadBoardState(filePath);

        Assert.AreEqual(firstBoardSettings.Width, board.Width);
        Assert.AreEqual(firstBoardSettings.Height, board.Height);
        Assert.AreEqual(firstBoardSettings.CellSize, board.CellSize);
    }

    /// <summary>
    /// Возвращает корректное число живых клеток на доске.
    /// </summary>
    [TestMethod]
    public void GetAliveCellsCount_BoardWithAliveCells_ReturnsCorrectAmount()
    {
        var board = new Board(new BoardSettings(3, 3, 1, 1));

        Assert.AreEqual(9, BoardAnalysis.GetAliveCellsCount(board));
    }

    /// <summary>
    /// Возвращает true, если фигуры эквивалентны.
    /// </summary>
    [TestMethod]
    public void FigureEquals_EqualFigures_ReturnsTrue()
    {
        var firstFigure =
            new Figure("FigureOne", 4, 4, "      **    **      ");
        var secondFigure =
            new Figure("FigureTwo", 4, 4, "      **    **      ");

        Assert.IsTrue(firstFigure.Equals(secondFigure));
    }

    /// <summary>
    /// Сохраняет список фигур в JSON-файл.
    /// </summary>
    [TestMethod]
    public void SaveFigures_CorrentParameters_SavesFigures()
    {
        var figures = new Figure[]
        {
            new Figure("Block", 4, 4, "      **    **      ")
        };
        var filePath = "../../../figures.json";

        FileManager.SaveFigures(figures, filePath);

        Assert.IsTrue(File.Exists(filePath));
    }

    /// <summary>
    /// Загружает список фигур из JSON-файла.
    /// </summary>
    [TestMethod]
    public void LoadFigures_CorrentPath_LoadsFigures()
    {
        var figures = new Figure[]
        {
            new Figure("Block", 4, 4, "      **    **      ")
        };
        var filePath = "../../../figures.json";
        FileManager.SaveFigures(figures, filePath);

        var loadedFigures = FileManager.LoadFigures(filePath);

        Assert.IsTrue(figures[0].Equals(loadedFigures[0]));
    }
}
