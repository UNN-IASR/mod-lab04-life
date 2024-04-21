using cli_life;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Board board = new Board(20, 10, 1);
        Assert.AreEqual(10, board.Rows);
    }
    [TestMethod]
    public void TestMethod2()
    {
        Board board = new Board(20, 10, 1);
        Assert.AreEqual(20, board.Columns);
    }
    [TestMethod]
    public void TestMethod3()
    {
        Board board = new Board(50, 50, 1, 0.3);
        Assert.IsTrue(0.2 < (double)board.Alive()/2500 && (double)board.Alive()/2500 < 0.4);
    }
    [TestMethod]
    public void TestMethod4()
    {
        Board board = new Board(50, 50, 1, 0);
        Assert.AreEqual(0, board.Alive());
    }
    [TestMethod]
    public void TestMethod5()
    {
        Board board = new Board(50, 50, 1, 1);
        Assert.AreEqual(2500, board.Alive());
    }
    [TestMethod]
    public void TestMethod6()
    {
        cli_life.Program.board = new Board(80, 40, 1);
        cli_life.Program.board = cli_life.Program.Load("stableboard.txt");
        cli_life.Program.board.Advance();
        cli_life.Program.board.IsStable();
        cli_life.Program.board.Advance();
        cli_life.Program.board.IsStable();
        cli_life.Program.board.Advance();
        Assert.AreEqual(true, cli_life.Program.board.IsStable());
    }
    [TestMethod]
    public void TestMethod7()
    {
        cli_life.Program.board = new Board(100, 20, 1);
        cli_life.Program.board = cli_life.Program.Load("board.txt");
        cli_life.Program.board.Advance();
        cli_life.Program.board.IsStable();
        cli_life.Program.board.Advance();
        cli_life.Program.board.IsStable();
        cli_life.Program.board.Advance();
        Assert.AreEqual(false, cli_life.Program.board.IsStable());
    }
    [TestMethod]
    public void TestMethod8()
    {
        cli_life.Program.board = new Board(100, 20, 1);
        cli_life.Program.board = cli_life.Program.Load("board.txt");
        cli_life.Program.Save("save.txt");
        Assert.AreEqual(File.ReadAllText("board.txt"), File.ReadAllText("save.txt"));
    }
    [TestMethod]
    public void TestMethod9()
    {
        cli_life.Program.board = new Board(10, 10, 1);
        cli_life.Program.Reset();
        Assert.AreEqual(80, cli_life.Program.board.Width);
        Assert.AreEqual(40, cli_life.Program.board.Height);
    }
    [TestMethod]
    public void TestMethod10()
    {
        cli_life.Program.board = new Board(80, 40, 1);
        cli_life.Program.board = cli_life.Program.Load("stableboard.txt");
        Figure[] figures = new Figure[]
        {
            new Figure {name = "block", width = 4, height = 4, fig = [0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0]}
        };
        cli_life.Program.FindFigures(figures);
        Assert.AreEqual(4, figures[0].count);
    }
    [TestMethod]
    public void TestMethod11()
    {
        cli_life.Program.board = new Board(80, 40, 1);
        cli_life.Program.board = cli_life.Program.Load("stableboard.txt");
        Figure[] figures = new Figure[]
        {
            new Figure {name = "beehive_h", width = 6, height = 5, fig = [0,0,0,0,0,0,0,0,1,1,0,0,0,1,0,0,1,0,0,0,1,1,0,0,0,0,0,0,0,0]}
        };
        cli_life.Program.FindFigures(figures);
        Assert.AreEqual(5, figures[0].count);
    }
    [TestMethod]
    public void TestMethod12()
    {
        cli_life.Program.board = new Board(80, 40, 1);
        cli_life.Program.board = cli_life.Program.Load("stableboard.txt");
        Figure[] figures = new Figure[]
        {
            new Figure {name = "boat_1", width = 5, height = 5, fig = [0,0,0,0,0,0,0,1,0,0,0,1,0,1,0,0,1,1,0,0,0,0,0,0,0]}
        };
        cli_life.Program.FindFigures(figures);
        Assert.AreEqual(0, figures[0].count);
    }
    [TestMethod]
    public void TestMethod13()
    {
        Board b = new Board(10, 10, 1, 0.5);
        b.Advance();
        b.Advance();
        b.Advance();
        b.Advance();
        Assert.AreEqual(4, b.gen);
    }
    [TestMethod]
    public void TestMethod14()
    {
        cli_life.Program.board = new Board(5, 5, 1);
        cli_life.Program.board = cli_life.Program.Load("advance.txt");
        Cell[,] cells = new Cell[,] {
            { new Cell { IsAlive = false}, new Cell { IsAlive = false}, new Cell { IsAlive = false}, new Cell { IsAlive = false}, new Cell { IsAlive = false} },
            { new Cell { IsAlive = false}, new Cell { IsAlive = true}, new Cell { IsAlive = true}, new Cell { IsAlive = false}, new Cell { IsAlive = false} },
            { new Cell { IsAlive = false}, new Cell { IsAlive = false}, new Cell { IsAlive = true}, new Cell { IsAlive = true}, new Cell { IsAlive = false} },
            { new Cell { IsAlive = false}, new Cell { IsAlive = false}, new Cell { IsAlive = false}, new Cell { IsAlive = false}, new Cell { IsAlive = false} },
            { new Cell { IsAlive = false}, new Cell { IsAlive = false}, new Cell { IsAlive = false}, new Cell { IsAlive = false}, new Cell { IsAlive = false} }
        };
        cli_life.Program.board.Advance();
        for (int x = 0; x < cli_life.Program.board.Columns; x++)
        {
            for (int y = 0; y < cli_life.Program.board.Rows; y++)
            {
                Assert.AreEqual(cli_life.Program.board.Cells[x,y].IsAlive, cells[x,y].IsAlive);
            }
        }
    }
    [TestMethod]
    public void TestMethod15()
    {
        Board b = new Board(10, 10, 1, 0.5);
        Assert.AreEqual(8, b.Cells[2,2].neighbors.Count());
    }
}