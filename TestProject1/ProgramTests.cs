using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace cli_life;

[TestClass]
public class ProgramTests {
    [TestMethod]
    public void TestMethod1() {
        Board board = new Board(20, 10, 1);
        Assert.AreEqual(10, board.Rows);
    }
    [TestMethod]
    public void TestMethod2() {
        Board board = new Board(20, 10, 1);
        Assert.AreEqual(20, board.Columns);
    }
    [TestMethod]
    public void TestMethod3() {
        Board board = new Board(50, 50, 1, 0.3);
        Assert.IsTrue(0.2 < (double)board.cellAliveCount() / 2500 && (double)board.cellAliveCount() / 2500 < 0.4);
    }
    [TestMethod]
    public void TestMethod4() {
        Board board = new Board(50, 50, 1, 0);
        Assert.AreEqual(0, board.cellAliveCount());
    }
    [TestMethod]
    public void TestMethod5() {
        Board board = new Board(50, 50, 1, 1);
        Assert.AreEqual(2500, board.cellAliveCount());
    }
    [TestMethod]
    public void TestMethod6() {
        Board board = new Board(50, 20, 1);
        foreach (var cell in board.Cells) {
            Assert.IsNotNull(cell);
        }
    }
    [TestMethod]
    public void TestMethod7() {
        Board board = new Board(50, 20, 1);
        foreach (var cell in board.Cells) {
            cell.IsAlive = true;
        }
        board.Advance();
        foreach (var cell in board.Cells) {
            Assert.IsFalse(cell.IsAlive);
        }
    }
    [TestMethod]
    public void TestMethod8() {
        Program.board = new Board(50, 25, 1);
        Program.board.LoadBoardState("../../../../Board.txt");
        Program.board.SaveBoardState();
        Assert.AreEqual(File.ReadAllText("../../../../Board.txt"), File.ReadAllText("savedBoard.txt"));
    }
    [TestMethod]
    public void TestMethod9() {
        Settings settings = Program.LoadSettings("../../../../config.json");
        Program.board = new Board(10, 10, 1);
        Program.Reset(settings);
        Assert.AreEqual(50, Program.board.Width);
        Assert.AreEqual(25, Program.board.Height);
    }
    [TestMethod]
    public void TestMethod10() {
        var cell = new Cell();
        Assert.IsFalse(cell.IsAlive);
    }

    [TestMethod]
    public void TestMethod11() {
        var cell = new Cell();
        var neighbor1 = new Cell();
        var neighbor2 = new Cell();
        neighbor1.IsAlive = true;
        neighbor2.IsAlive = true;
        cell.neighbors.Add(neighbor1);
        cell.neighbors.Add(neighbor2);

        var neighborCount = cell.neighbors.Where(x => x.IsAlive).Count();
        Assert.AreEqual(2, neighborCount);
    }

    [TestMethod]
    public void TestMethod12() {
        var cell = new Cell();
        var neighbor1 = new Cell { IsAlive = true };
        var neighbor2 = new Cell { IsAlive = true };
        var neighbor3 = new Cell { IsAlive = true };
        cell.neighbors.Add(neighbor1);
        cell.neighbors.Add(neighbor2);
        cell.neighbors.Add(neighbor3);

        cell.DetermineNextLiveState();

        Assert.IsTrue(cell.IsAliveNext);
    }

    [TestMethod]
    public void TestMethod13() {
        var cell = new Cell { IsAlive = true };
        cell.Advance();
        Assert.IsFalse(cell.IsAlive);
    }
    [TestMethod]
    public void TestMethod14() {
        Cell cell = new Cell { IsAlive = true };
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.neighbors.Add(new Cell { IsAlive = true });
        cell.DetermineNextLiveState();
        cell.Advance();
        Assert.IsTrue(cell.IsAlive);
    }
    [TestMethod]
    public void TestMethod15() {
        Board b = new Board(10, 10, 1, 0.5);
        Assert.AreEqual(8, b.Cells[2, 2].neighbors.Count());
    }
}