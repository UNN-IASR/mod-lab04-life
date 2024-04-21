using cli_life;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Test_lab4;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void Test1()
    {
        var settings = new Settings { Width = 10, Height = 20, cellSize = 3, liveDensity = 0.5 };
        Assert.AreEqual(10, settings.Width);
        Assert.AreEqual(20, settings.Height);
        Assert.AreEqual(3, settings.cellSize);
        Assert.AreEqual(0.5, settings.liveDensity);
    }

    [TestMethod]
    public void Test2()
    {
        var board = new Board(5, 5, 1);
        for (int x = 0; x < board.Columns; x++)
        {
            for (int y = 0; y < board.Rows; y++)
            {
                var cell = board.Cells[x, y];
                Assert.AreEqual(8, cell.neighbors.Count);
            }
        }
    }

    [TestMethod]
    public void Test3()
    {
        Board board = new Board(100, 30, 1, 0.1);

        Assert.AreEqual(100, board.Width);
        Assert.AreEqual(30, board.Height);
    }
    [TestMethod]
    public void Test4()
    {
        Board board = new Board(2, 2, 1, .1);
        board.Cells[0, 0].IsAlive = true;
        board.Cells[0, 1].IsAlive = false;
        board.Cells[1, 0].IsAlive = false;
        board.Cells[1, 1].IsAlive = true;
        int count = board.count_live_cell();
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public void Test5()
    {
        var board = new Board(5, 5, 1);
        board.start_board("start_board.txt");
        for (int x = 0; x < board.Columns; x++)
        {
            for (int y = 0; y < board.Rows; y++)
            {
                var cell = board.Cells[x, y];
                if (cell.IsAlive)
                {
                    Assert.AreEqual('*', File.ReadAllText("start_board.txt")[x + y * board.Columns]);
                }
            }
        }
    }

    [TestMethod]
    public void Test6()
    {
        var board = new Board(5, 5, 1);
        var expected = "******   **   **   ******";
        board.Cells[0, 0].IsAlive = true;
        board.Cells[0, 1].IsAlive = true;
        board.Cells[0, 2].IsAlive = true;
        board.Cells[0, 3].IsAlive = true;
        board.Cells[0, 4].IsAlive = true;
        board.Cells[1, 0].IsAlive = true;
        board.Cells[1, 1].IsAlive = false;
        board.Cells[1, 2].IsAlive = false;
        board.Cells[1, 3].IsAlive = false;
        board.Cells[1, 4].IsAlive = true;
        board.Cells[2, 0].IsAlive = true;
        board.Cells[2, 1].IsAlive = false;
        board.Cells[2, 2].IsAlive = false;
        board.Cells[2, 3].IsAlive = false;
        board.Cells[2, 4].IsAlive = true;
        board.Cells[3, 0].IsAlive = true;
        board.Cells[3, 1].IsAlive = false;
        board.Cells[3, 2].IsAlive = false;
        board.Cells[3, 3].IsAlive = false;
        board.Cells[3, 4].IsAlive = true;
        board.Cells[4, 0].IsAlive = true;
        board.Cells[4, 1].IsAlive = true;
        board.Cells[4, 2].IsAlive = true;
        board.Cells[4, 3].IsAlive = true;
        board.Cells[4, 4].IsAlive = true;
        var actual = board.save_board(board);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test7()
    {
        var board = new Board(5, 5, 1);
        var expected = " *  * ** ** *    ******  ";
        board.Cells[0, 0].IsAlive = false;
        board.Cells[0, 1].IsAlive = true;
        board.Cells[0, 2].IsAlive = false;
        board.Cells[0, 3].IsAlive = false;
        board.Cells[0, 4].IsAlive = true;
        board.Cells[1, 0].IsAlive = false;
        board.Cells[1, 1].IsAlive = true;
        board.Cells[1, 2].IsAlive = true;
        board.Cells[1, 3].IsAlive = false;
        board.Cells[1, 4].IsAlive = true;
        board.Cells[2, 0].IsAlive = true;
        board.Cells[2, 1].IsAlive = false;
        board.Cells[2, 2].IsAlive = true;
        board.Cells[2, 3].IsAlive = false;
        board.Cells[2, 4].IsAlive = false;
        board.Cells[3, 0].IsAlive = false;
        board.Cells[3, 1].IsAlive = false;
        board.Cells[3, 2].IsAlive = true;
        board.Cells[3, 3].IsAlive = true;
        board.Cells[3, 4].IsAlive = true;
        board.Cells[4, 0].IsAlive = true;
        board.Cells[4, 1].IsAlive = true;
        board.Cells[4, 2].IsAlive = true;
        board.Cells[4, 3].IsAlive = false;
        board.Cells[4, 4].IsAlive = false;
        var actual = board.save_board(board);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test8()
    {
        var res = CrateGraph.AliveInGeneration(0.5);
        Assert.IsTrue(res.ContainsKey(0));
        Assert.IsTrue(res.ContainsKey(1));
        Assert.IsTrue(res.ContainsKey(2));
        Assert.IsTrue(res.ContainsKey(3));
        Assert.IsTrue(res.ContainsKey(4));
        Assert.IsTrue(res.ContainsKey(5));
        Assert.IsTrue(res.ContainsKey(6));
        Assert.IsTrue(res.ContainsKey(7));
        Assert.IsTrue(res.ContainsKey(8));
        Assert.IsTrue(res.ContainsKey(9));
    }

    [TestMethod]
    public void Test9()
    {
        var density = new List<double>() { 0.3, 0.4, 0.5, 0.6, 0.7 };
        var list = CrateGraph.CreateList(density, density.Count);
        Assert.AreEqual(density.Count, list.Count);
        for (int i = 0; i < density.Count; i++)
        {
            Assert.IsTrue(list[i].ContainsKey(0));
            Assert.IsTrue(list[i].ContainsKey(1));
            Assert.IsTrue(list[i].ContainsKey(2));
            Assert.IsTrue(list[i].ContainsKey(3));
            Assert.IsTrue(list[i].ContainsKey(4));
            Assert.IsTrue(list[i].ContainsKey(5));
            Assert.IsTrue(list[i].ContainsKey(6));
            Assert.IsTrue(list[i].ContainsKey(7));
            Assert.IsTrue(list[i].ContainsKey(8));
            Assert.IsTrue(list[i].ContainsKey(9));
        }
    }


    [TestMethod]
    public void Test10()
    {
        var cell = new Cell();
        Assert.IsFalse(cell.IsAlive);
    }

    [TestMethod]
    public void Test11()
    {
        var cell = new Cell { IsAlive = true };
        cell.Advance();
        Assert.IsFalse(cell.IsAlive);
    }

    [TestMethod]
    public void Test12t()
    {
        Board board = new Board(9, 7, 1);
        board.start_board("Block1.txt");
        Figure figure2 = new Figure(4, "Block.txt");
        int count = figure2.find_figures(figure2, board, "Block.txt");
        Assert.AreEqual(count, 4);
    }
    [TestMethod]
    public void Test13t()
    {
        Board board = new Board(5, 5, 1);
        board.start_board("Tub1.txt");
        Figure figure2 = new Figure(5, "Tub.txt");
        int count = figure2.find_figures(figure2, board, "Tub.txt");
        Assert.AreEqual(count, 1);
    }
    [TestMethod]
    public void Test15()
    {
        Board board = new Board(10, 10, 1, 0.8);
        board.Randomize(0.8);
        Assert.IsTrue(board.count_live_cell() > 0);
    }
}
