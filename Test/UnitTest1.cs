using cli_life;
namespace Test;
using System.Text.Json;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1_Reset()
    {
        Board board=cli_life.Program.Reset();
        Assert.AreEqual(board.Width, 50);     
    }
    [TestMethod]
    public void TestMethod2_Reset()
    {
        Board board=cli_life.Program.Reset();
        Assert.AreEqual(board.Height, 20);       
    }
    [TestMethod]
    public void TestMethod3_Reset()
    {
        Board board=cli_life.Program.Reset();
        Assert.AreEqual(board.CellSize, 1);        
    }
    [TestMethod]
    public void TestMethod4_Reset()
    {
        Board board=cli_life.Program.Reset();
        Assert.AreEqual(board.LiveDensity, 0.5);        
    }
    [TestMethod]
    public void TestMethod5_Load()
    {
        Board board=cli_life.Program.Reset();
        board.LoadStateSistem("gen-53.txt");  
        Assert.AreEqual(board.Cells[2, 1].IsAlive, true);
        Assert.AreEqual(board.Cells[1, 2].IsAlive, true);
        Assert.AreEqual(board.Cells[3, 2].IsAlive, true);
        Assert.AreEqual(board.Cells[2, 3].IsAlive, true);
        Assert.AreEqual(board.Cells[3, 3].IsAlive, true);
        Assert.AreEqual(board.Cells[4, 3].IsAlive, false);
        Assert.AreEqual(board.Cells[3, 4].IsAlive, false); 
        Assert.AreEqual(board.Cells[4, 4].IsAlive, false); 
    }
     [TestMethod]
    public void TestMethod6_Save()
    {
        Board exs_board=cli_life.Program.Reset();
        cli_life.Program.SaveStateSistem(1); //gen-1.txt
        Board board=cli_life.Program.Reset();
        board.LoadStateSistem("gen-1.txt");  
        Assert.AreEqual(board.Cells[1, 1].IsAlive, exs_board.Cells[1, 1].IsAlive);
        Assert.AreEqual(board.Cells[2, 1].IsAlive, exs_board.Cells[2, 1].IsAlive);
        Assert.AreEqual(board.Cells[1, 2].IsAlive, exs_board.Cells[1, 2].IsAlive);
    }
    [TestMethod]
    public void TestMethod7_CountAliveCells()
    {
        Board board=cli_life.Program.Reset();
        board.LoadStateSistem("gen-53.txt"); 
        int count;
        count=cli_life.Program.CountAliveCells(board); 
        Assert.AreEqual(count, 5);
    }
    [TestMethod]
    public void TestMethod8_CountAliveCells()
    {
        Board board=cli_life.Program.Reset();
        board.LoadStateSistem("gen-737.txt"); 
        int count;
        count=cli_life.Program.CountAliveCells(board); 
        Assert.AreEqual(count, 20);
    }
    [TestMethod]
    public void TestMethod9_BuildString()
    {
        Board board=cli_life.Program.Reset();
        board.LoadStateSistem("gen-53.txt"); 
        string s = cli_life.Program.BuildString(board, 2, 1, 5, 5, 2);;
        string boat = ".......*...*.*...**......"; 
        Assert.AreEqual(s, boat);
    }
    [TestMethod]
    public void TestMethod10_DetectionElem()
    {
        Board board=cli_life.Program.Reset();
        board.LoadStateSistem("gen-53.txt"); 
        int[] numberOfFigures = cli_life.Program.DetectionElem(board);
        Assert.AreEqual(numberOfFigures[0], 0);
        Assert.AreEqual(numberOfFigures[2], 1);
    }
    [TestMethod]
    public void TestMethod11_DetectionElem()
    {
        Board board=cli_life.Program.Reset();
        board.LoadStateSistem("gen-737.txt"); 
        int[] numberOfFigures = cli_life.Program.DetectionElem(board);
        Assert.AreEqual(numberOfFigures[0], 1);
        Assert.AreEqual(numberOfFigures[1], 1);
        Assert.AreEqual(numberOfFigures[2], 0);
        Assert.AreEqual(numberOfFigures[3], 0);
        Assert.AreEqual(numberOfFigures[4], 2);
        Assert.AreEqual(numberOfFigures[5], 0);
        Assert.AreEqual(numberOfFigures[6], 0);
    }

     [TestMethod]
    public void TestMethod12_Advance()
    {
        Board board=cli_life.Program.Reset();
        board.LoadStateSistem("gen-240.txt");
        board.Advance();
        board.Advance(); 
        Assert.AreEqual(board.Cells[2, 2].IsAlive, true);
        Assert.AreEqual(board.Cells[4, 1].IsAlive, true);
        Assert.AreEqual(board.Cells[3, 2].IsAlive, true);
        Assert.AreEqual(board.Cells[3, 0].IsAlive, true);
        Assert.AreEqual(board.Cells[4, 2].IsAlive, true);
        Assert.AreEqual(board.Cells[0, 0].IsAlive, false);
        Assert.AreEqual(board.Cells[0, 1].IsAlive, false); 
        Assert.AreEqual(board.Cells[1, 1].IsAlive, false); 
    }
    [TestMethod]
    public void TestMethod13_StatePhase()
    {
        Board board=cli_life.Program.Reset();
        board.LoadStateSistem("gen-240.txt");
        List<Board> boards = new List<Board>(3);
        boards.Add(board.Clone());
        board.Advance();
        boards.Add(board.Clone());
        board.Advance();
        boards.Add(board.Clone());
        bool state= cli_life.Program.StatePhase(boards);
        Assert.AreEqual(state, false);
    }
     [TestMethod]
    public void TestMethod14_StatePhase()
    {
        Board board=cli_life.Program.Reset();
        board.LoadStateSistem("gen-53.txt");
        List<Board> boards = new List<Board>(3);
        boards.Add(board.Clone());
        board.Advance();
        boards.Add(board.Clone());
        board.Advance();
        boards.Add(board.Clone());
        bool state= cli_life.Program.StatePhase(boards);
        Assert.AreEqual(state, true);
    }
    [TestMethod]
    public void TestMethod15_Clone()
    {
        Board exs_board=cli_life.Program.Reset();
        exs_board.LoadStateSistem("gen-53.txt");
        Board board=exs_board.Clone();
        Assert.AreEqual(board.Cells[1, 1].IsAlive, exs_board.Cells[1, 1].IsAlive);
        Assert.AreEqual(board.Cells[2, 1].IsAlive, exs_board.Cells[2, 1].IsAlive);
        Assert.AreEqual(board.Cells[1, 2].IsAlive, exs_board.Cells[1, 2].IsAlive);
    }

}