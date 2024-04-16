namespace Life.Tests;
using cli_life;

[TestClass]
public class UnitTest1
{

    [TestMethod]
    public void BoardSize()
    {
        int w = 10;
        int h = 10;
        int cz = 1;
        Board board = new Board(w, h, cz, 1);

        Assert.AreEqual(w, board.Width);
        Assert.AreEqual(h, board.Height);
        Assert.AreEqual(cz, board.CellSize);
    }

    [TestMethod]
    public void CellBirth()
    {
        //####
        //#**#
        //##*#
        //####
        Cell[][] cells = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        //####
        //#**#
        //#**#
        //####
        Cell[][] correct = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        Board board = new Board(cells, 1);
        board.Advance();
        for (int x = 0; x < board.Columns; x++)
            for (int y = 0; y < board.Rows; y++)
                Assert.AreEqual(correct[x][y].IsAlive, board.Cells[x, y].IsAlive);
    }

    [TestMethod]
    public void CellDiedFromLoneliness()
    {
        //####
        //####
        //##*#
        //####
        Cell[][] cells = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        //####
        //####
        //####
        //####
        Cell[][] correct = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        Board board = new Board(cells, 1);
        board.Advance();
        for (int x = 0; x < board.Columns; x++)
            for (int y = 0; y < board.Rows; y++)
                Assert.AreEqual(correct[x][y].IsAlive, board.Cells[x, y].IsAlive);
    }

    [TestMethod]
    public void CellDiedFromOverpopulating()
    {
        //#####
        //##*##
        //#***#
        //##*##
        //#####
        Cell[][] cells = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        //#####
        //#***#
        //#*#*#
        //#***#
        //#####
        Cell[][] correct = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        Board board = new Board(cells, 1);
        board.Advance();
        for (int x = 0; x < board.Columns; x++)
            for (int y = 0; y < board.Rows; y++)
                Assert.AreEqual(correct[x][y].IsAlive, board.Cells[x, y].IsAlive);
    }

    [TestMethod]
    public void CellsLiveStable()
    {
        //#####
        //#**##
        //#**##
        //#####
        //#####
        Cell[][] cells = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        //#####
        //#**##
        //#**##
        //#####
        //#####
        Cell[][] correct = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        Board board = new Board(cells, 1);
        for (int i = 0; i < 10; i++)
            board.Advance();
        for (int x = 0; x < board.Columns; x++)
            for (int y = 0; y < board.Rows; y++)
                Assert.AreEqual(correct[x][y].IsAlive, board.Cells[x, y].IsAlive);
    }

    [TestMethod]
    public void CellsLivePeriodic()
    {
        //#####
        //#*###
        //#*###
        //#*###
        //#####
        Cell[][] cells = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        //#####
        //#*###
        //#*###
        //#*###
        //#####
        Cell[][] correct = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        Board board = new Board(cells, 1);
        for (int i = 0; i < 10; i++)
            board.Advance();
        for (int x = 0; x < board.Columns; x++)
            for (int y = 0; y < board.Rows; y++)
                Assert.AreEqual(correct[x][y].IsAlive, board.Cells[x, y].IsAlive);
    }

    [TestMethod]
    public void CellsGlider()
    {
        //#####
        //##*##
        //###*#
        //#***#
        //#####
        Cell[][] cells = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        //#####
        //##*##
        //###*#
        //#***#
        //#####
        Cell[][] correct = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        Board board = new Board(cells, 1);
        for (int i = 0; i < 20; i++)
            board.Advance();
        for (int x = 0; x < board.Columns; x++)
            for (int y = 0; y < board.Rows; y++)
                Assert.AreEqual(correct[x][y].IsAlive, board.Cells[x, y].IsAlive);
    }


    [TestMethod]
    public void CountAll()
    {
        //#####
        //##*##
        //###*#
        //#***#
        //#####
        Cell[][] cells = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        Board board = new Board(cells, 1);
        Assert.AreEqual(5, board.countAlive());
    }

    [TestMethod]
    public void CountAllInEntities()
    {
        //#####
        //#**##
        //#**##
        //#####
        //#####
        Cell[][] cells = new Cell[][] 
        {
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = true}, new Cell(){IsAlive = true}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false} },
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
            new Cell[] {new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}, new Cell(){IsAlive = false}},
        };
        
        Board board = new Board(cells, 1);
        Dictionary<string, int> entities = board.countEntities();
        Assert.AreEqual(4, entities["cells at all"]);
    }

    [TestMethod]
    public void CountBlock()
    {
        string[] raw = File.ReadAllLines("Tests.Data/test.txt");
        Board board = new Board(60, 30, 1, raw);
        Dictionary<string, int> entities = board.countEntities();
        Assert.AreEqual(3, entities["block"]);
    }

    [TestMethod]
    public void CountSpinner()
    {
        string[] raw = File.ReadAllLines("Tests.Data/test.txt");
        Board board = new Board(60, 30, 1, raw);
        Dictionary<string, int> entities = board.countEntities();
        Assert.AreEqual(2, entities["spinner"]);
    }

    [TestMethod]
    public void CountLoaf()
    {
        string[] raw = File.ReadAllLines("Tests.Data/test.txt");
        Board board = new Board(60, 30, 1, raw);
        Dictionary<string, int> entities = board.countEntities();
        Assert.AreEqual(4, entities["loaf"]);
    }

    [TestMethod]
    public void CountPond()
    {
        string[] raw = File.ReadAllLines("Tests.Data/test.txt");
        Board board = new Board(60, 30, 1, raw);
        Dictionary<string, int> entities = board.countEntities();
        Assert.AreEqual(1, entities["pond"]);
    }

    [TestMethod]
    public void CountTub()
    {
        string[] raw = File.ReadAllLines("Tests.Data/test.txt");
        Board board = new Board(60, 30, 1, raw);
        Dictionary<string, int> entities = board.countEntities();
        Assert.AreEqual(1, entities["tub"]);
    }

    [TestMethod]
    public void CountBoat()
    {
        string[] raw = File.ReadAllLines("Tests.Data/test.txt");
        Board board = new Board(60, 30, 1, raw);
        Dictionary<string, int> entities = board.countEntities();
        Assert.AreEqual(4, entities["boat"]);
    }

    [TestMethod]
    public void CountShip()
    {
        string[] raw = File.ReadAllLines("Tests.Data/test.txt");
        Board board = new Board(60, 30, 1, raw);
        Dictionary<string, int> entities = board.countEntities();
        Assert.AreEqual(2, entities["ship"]);
    }

    [TestMethod]
    public void CountHive()
    {
        string[] raw = File.ReadAllLines("Tests.Data/test.txt");
        Board board = new Board(60, 30, 1, raw);
        Dictionary<string, int> entities = board.countEntities();
        Assert.AreEqual(2, entities["hive"]);
    }

    [TestMethod]
    public void LongLife()
    {
        Board board = new Board(60, 30, 1, 0.2);
        while(!board.isStable()){
            board.Advance();
        }
        Assert.IsTrue(board.isStable());
    }

    [TestMethod]
    public void EmptyLife()
    {
        Board board = new Board(60, 30, 1, 0);
        while(!board.isStable()){
            board.Advance();
        }
        Assert.IsTrue(board.isStable());
    }
}