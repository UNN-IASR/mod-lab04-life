using cli_life;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace LifeTests;

[TestClass]
public class UnitTest {
    readonly string path = "../../../../Life/";
    [TestMethod]
    public void BoardRows() {
        Board board = new Board(10, 5);
        Assert.AreEqual(10, board.Rows);
    }
    [TestMethod]
    public void BoardColumns() {
        Board board = new Board(10, 5);
        Assert.AreEqual(5, board.Columns);
    }
    [TestMethod]
    public void BoardFromJsonRows() {
        Board board = BoardUtils.FromJSON(path + "settings.json");
        Assert.AreEqual(30, board.Rows);
    }
    [TestMethod]
    public void BoardFromJsonColumns() {
        Board board = BoardUtils.FromJSON(path + "settings.json");
        Assert.AreEqual(100, board.Columns);
    }
    [TestMethod]
    public void BoardFromTXTRows() {
        Board board = BoardUtils.FromTXT(path + "stable_gen_737.txt");
        Assert.AreEqual(30, board.Rows);
    }
    [TestMethod]
    public void BoardFromTXTColumns() {
        Board board = BoardUtils.FromTXT(path + "stable_gen_737.txt");
        Assert.AreEqual(100, board.Columns);
    }
    [TestMethod]
    public void SaveBoard() {
        Board board = new Board(30, 100, 0.5);
        BoardUtils.Save(board, "saved.txt");
        Assert.IsTrue(File.Exists("saved.txt"));
    }
    [TestMethod]
    public void CountAlive() {
        Board board = BoardUtils.FromTXT(path + "stable_gen_737.txt");
        Assert.AreEqual(76, BoardUtils.CountAlive(board));
    }
    [TestMethod]
    public void ZeroAlive() {
        Board board = new Board(10, 10, 0);
        Assert.AreEqual(0, BoardUtils.CountAlive(board));
    }
    [TestMethod]
    public void LoadPatterns() {
        BoardUtils.LoadPatterns(path + "patterns.json");
        Assert.AreEqual(19, BoardUtils.patterns.Count());
    }
    [TestMethod]
    public void FindPattern() {
        Board board = BoardUtils.FromTXT(path + "gen_69.txt");
        Pattern block = new Pattern() {
            Name = "Block",
            Height = 4,
            Width = 4,
            Body = "     **  **     "
        };
        int count = BoardUtils.FindPattern(board, block);
        Assert.AreEqual(6, count);
    }
    [TestMethod]
    public void FindPatternOnEdge() {
        Board board = BoardUtils.FromTXT(path + "stable_gen_2.txt");
        Pattern block = new Pattern() {
            Name = "Block",
            Height = 4,
            Width = 4,
            Body = "     **  **     "
        };
        int count = BoardUtils.FindPattern(board, block);
        Assert.AreEqual(4, count);
    }
    [TestMethod]
    public void FindPatterns() {
        Board board = BoardUtils.FromTXT(path + "stable_gen_737.txt");
        BoardUtils.LoadPatterns(path + "patterns.json");
        Dictionary<string, int> expected = new Dictionary<string, int>() {
            { "Block" , 4 },
            { "Beehive" , 6 },
            { "Loaf", 0 },
            { "Boat", 0 },
            { "Ship", 1 },
            { "Tub", 0 },
            { "Pond", 0 },
            { "Blinker", 6 },
            { "Toad", 0 }
        };
        Dictionary<string, int> result = BoardUtils.FindPatterns(board);
        Assert.IsTrue(expected.SequenceEqual(result));
    }
    [TestMethod]
    public void BoardStable() {
        Board board = BoardUtils.FromTXT(path + "stable_gen_737.txt");
        for (int _ = 0; _ < 3; _++) {
            board.CheckStable();
            board.Advance();
        }
        Assert.IsTrue(board.CheckStable());
    }
    [TestMethod]
    public void BoardUnstable() {
        Board board = BoardUtils.FromTXT(path + "glider_gun.txt");
        for (int _ = 0; _ < 10; _++) {
            board.CheckStable();
            board.Advance();
        }
        Assert.IsTrue(!board.CheckStable());
    }
}