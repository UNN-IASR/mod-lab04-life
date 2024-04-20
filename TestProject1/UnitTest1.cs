using cli_life;
using System.Text.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using NuGet.Frameworks;
namespace TestProject1;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Board board = new Board(100, 30, 1, 0.1); 
        
        Assert.AreEqual(100, board.Width);
        Assert.AreEqual(30, board.Height);
    }
    [TestMethod]
    public void TestMethod2()
    {   
        string filename = "../../../../test2.json";
        string jsonString = File.ReadAllText(filename);
        Settings? settings = JsonSerializer.Deserialize<Settings>(jsonString);

        Assert.AreEqual(50, settings.Width);
        Assert.AreEqual(20, settings.Height);
        Assert.AreEqual(2, settings.cellSize);
        Assert.AreEqual(0.5, settings.liveDensity);
    }
    [TestMethod]
    public void TestMethod3()
    {
        Board board = new Board(4, 4, 1, 1);
        board.Upload("../../../../test3.txt");
        Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
        Figure block = fig[0];
        int count = Figure.FindFigure(block, board);
        Assert.AreEqual(count, 1);

    }
    [TestMethod]
    public void TestMethod4()
    {
        Board board = new Board(6, 6, 1, 1);
        board.Upload("../../../../test4.txt");
        Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
        Figure pond = fig[1];
        int count = Figure.FindFigure(pond, board);
        Assert.AreEqual(count, 1);
    }
    [TestMethod]
    public void TestMethod5()
    {
        Board board = new Board(5, 5, 1, 1);
        board.Upload("../../../../test5.txt");
        Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
        Figure tub = fig[2];
        int count = Figure.FindFigure(tub, board);
        Assert.AreEqual(count, 1);
    }
    [TestMethod]
    public void TestMethod6()
    {
        Board board = new Board(5, 5, 1, 1);
        board.Upload("../../../../test6.txt");
        Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
        Figure boat = fig[3];
        int count = Figure.FindFigure(boat, board);
        Assert.AreEqual(count, 1);
    }
    [TestMethod]
    public void TestMethod7()
    {
        Board board = new Board(5, 5, 1, 1);
        board.Upload("../../../../test7.txt");
        Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
        Figure ship = fig[4];
        int count = Figure.FindFigure(ship, board);
        Assert.AreEqual(count, 1);
    }
    [TestMethod]
    public void TestMethod8()
    {
        Board board = new Board(6, 6, 1, 1);
        board.Upload("../../../../test8.txt");
        Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
        Figure block = fig[0];
        int count = Figure.FindFigure(block, board);
        Assert.AreEqual(count, 4);
    }
    [TestMethod]
    public void TestMethod9()
    {
        Board board = new Board(4, 4, 1, 1);
        board.Upload("../../../../test9.txt");
        Figure[] fig = Figure.GetFigure("../../../../figuretest.json");
        Figure block = fig[0];
        int count = Figure.FindFigure(block, board);
        Assert.AreEqual(count, 0);
    }
    [TestMethod]
    public void TestMethod10()
    {
        Board board = new Board(4, 4, 1, 1);
        board.Upload("../../../../test10.txt");
        int count = board.CellsAliveCount();
        Assert.AreEqual(count,4);
    }
    [TestMethod]
    public void TestMethod11()
    {
        Board board = new Board(6, 6, 1, 1);
        board.Upload("../../../../test11.txt");
        int count = board.CellsAliveCount();
        Assert.AreEqual(count,16);
    }
    [TestMethod]
    public void TestMethod12()
    {
        Board board = new Board(6, 6, 1, 1);
        board.Upload("../../../../test11.txt");
        int CountPositions = 0;
        bool flag = true;
        while(flag)
        {
            if (!board.positions.Contains(board.RecordPositions(board)))
                board.positions.Add(board.RecordPositions(board));
            else 
                {
                    CountPositions = board.positions.Count;

                    flag = false;
                }
            board.Advance();
        }
        Assert.AreEqual(CountPositions,1);
    }
    [TestMethod]
    public void TestMethod13()
    {
        Board board = new Board(6, 6, 1, 1);
        board.Upload("../../../../test11.txt");
        bool flag = true;
        for (int i = 0; i<2; i++)
        {
            if (!board.positions.Contains(board.RecordPositions(board)))
                board.positions.Add(board.RecordPositions(board));
            else 
                flag = false;

            board.Advance();
        }
        Assert.AreEqual(flag,false);
    }
    [TestMethod]
    public void TestMethod14()
    {
        Board board = new Board(6, 6, 1, 1);
        board.Upload("../../../../test14.txt");
        int count = board.CellsAliveCount();
        Assert.AreEqual(count,2);
    }
    [TestMethod]
    public void TestMethod15()
    {
        Board board = new Board(6, 6, 1, 1);
        board.Upload("../../../../test14.txt");
        for (int i = 0; i<2; i++)
        {
            board.Advance();
        }
        int count = board.CellsAliveCount();
        Assert.AreEqual(count,0);
    }
}