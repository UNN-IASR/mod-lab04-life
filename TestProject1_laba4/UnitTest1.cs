using cli_life;
using ScottPlot.Colormaps;

namespace TestProject_laba4;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Board board = new Board(50, 20, 1, 0.5);

        Assert.AreEqual(50, board.Width);
        Assert.AreEqual(20, board.Height);
    }

    [TestMethod]
    public void TestMethod2()
    {
        Board board = new Board(50, 20, 1);
        board.loadfromfile("boxs.txt");
        Figure box = new Figure("Box", 4, 4, "0000011001100000");

        int count = box.find_countfile(box, board, "box.txt");
        Assert.AreEqual(count, 8);
    }

    [TestMethod]
    public void TestMethod3()
    {
        Board board = new Board(50, 20, 1);
        board.loadfromfile("hives.txt");
        Figure hive = new Figure("Hive", 6, 5, "000000010001010010100010000000");

        int count = hive.find_countfile(hive, board, "hive.txt");
        Assert.AreEqual(count, 5);
    }

    [TestMethod]
    public void TestMehtod4()
    {
        Board board = new Board(50, 20, 1);
        board.loadfromfile("snake6.txt");
        Figure snake = new Figure("Snake", 4, 6, "000000010110011010000000");

        int count = snake.find_countfile(snake, board, "snake.txt");
        Assert.AreEqual(count, 6);
    }

    [TestMethod]
    public void TestMethod5()
    {
        Board board = new Board(50, 20, 1);
        board.loadfromfile("ships.txt");
        Figure ship = new Figure("Ship", 5, 5, "0000001100010100011000000");

        int count = ship.find_countfile(ship, board, "ship.txt");
        Assert.AreEqual(count, 6);
    }

    [TestMethod]
    public void TestMethod6()
    {
        Board board = new Board(50, 20, 1);
        board.loadfromfile("boxs.txt");

        Assert.AreEqual(board.Check_count_live(), 32);
    }

    [TestMethod]
    public void TestMethod7()
    {
        Board board = new Board(50, 20, 1);
        board.loadfromfile("snake6.txt");

        Assert.AreEqual(board.Check_count_live(), 36);
    }

    [TestMethod]
    public void TestMethod8()
    {
        Board board = new Board(50, 20, 1);
        board.loadfromfile("res.txt");

        Assert.IsNotNull(board);
    }

    [TestMethod]
    public void TestMethod9()
    {
        Graph.CreateGraph();
        Assert.IsTrue(System.IO.File.Exists("plot.png"));
    }

    [TestMethod]
    public void TestMethod10()
    {
        int width = 4;
        int height = 4;
        int cellSize = 1;

        Board board = new Board(width, height, cellSize);
        board.Cells[0, 0].IsAlive = true;
        board.Cells[1, 1].IsAlive = true;
        board.Cells[2, 2].IsAlive = true;
        board.Cells[3, 3].IsAlive = true;
        board.Cells[0, 1].IsAlive = false;
        board.Cells[1, 0].IsAlive = false;
        board.Cells[0, 2].IsAlive = false;
        board.Cells[2, 0].IsAlive = false;
        board.Cells[0, 3].IsAlive = false;
        board.Cells[3, 0].IsAlive = false;
        board.Cells[1, 2].IsAlive = false;
        board.Cells[2, 1].IsAlive = false;
        board.Cells[1, 3].IsAlive = false;
        board.Cells[3, 1].IsAlive = false;
        board.Cells[2, 3].IsAlive = false;
        board.Cells[3, 2].IsAlive = false;

        string actualGen = board.WriteGen(board);
        string expectedGen = "*    *    *    *";
        Assert.AreEqual(expectedGen, actualGen);
    }

    [TestMethod]
    public void TestMethod11()
    {
        Board board = new Board(5, 4, 1);
        board.loadfromfile("test11.txt");
        Assert.AreEqual(board.Cells[1, 0].IsAlive, true);
        Assert.AreEqual(board.Cells[3, 0].IsAlive, true);
        Assert.AreEqual(board.Cells[0, 1].IsAlive, true);
        Assert.AreEqual(board.Cells[2, 1].IsAlive, true);
        Assert.AreEqual(board.Cells[4, 1].IsAlive, true);
        Assert.AreEqual(board.Cells[1, 2].IsAlive, true);
        Assert.AreEqual(board.Cells[3, 2].IsAlive, true);
        Assert.AreEqual(board.Cells[2, 3].IsAlive, true);
    }

    [TestMethod]
    public void TestMethod12()
    {
        Board board = new Board(50, 20, 1, 0.5);
        foreach (var cell in board.Cells)
        {
            Assert.IsNotNull(cell);
        }
    }

    [TestMethod]
    public void TestMethod13()
    {
        Board board = new Board(50, 20, 1);
        board.loadfromfile("res.txt");

        Assert.AreEqual(board.Check_count_live(), 188);
    }

    [TestMethod]
    public void TestMethod14()
    {
        Board board = new Board(100, 50, 1);
        Assert.AreEqual(50, board.Rows);
    }

    [TestMethod]
    public void TestMethod15()
    {
        Board board = new Board(100, 50, 1);
        Assert.AreEqual(100, board.Columns);
    }
}
