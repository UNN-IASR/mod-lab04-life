using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;

namespace Life.Test {

[TestClass]
public class BoardTests
{
    string solutionRootPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
    
    [TestMethod]
    public void ImportStateTest()
    {
        Board board = new Board(3, 3);
        board.ImportState($"{solutionRootPath}/Life.Test/TestState1.txt");
        Cell[,] actual = board.Cells;
        
        Cell c0 = new Cell { IsAlive = false };
        Cell c1 = new Cell { IsAlive = true };
        Cell c2 = new Cell { IsAlive = false };
        Cell c3 = new Cell { IsAlive = true };
        Cell c4 = new Cell { IsAlive = false };
        Cell c5 = new Cell { IsAlive = true };
        Cell c6 = new Cell { IsAlive = false };
        Cell c7 = new Cell { IsAlive = true };
        Cell c8 = new Cell { IsAlive = false };
        Cell[,] expected = { { c0, c3, c6 }, { c1, c4, c7 }, { c2, c5, c8 } };
        
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void AdvanceTestStatic()
    {
        Board board = new Board(3, 3);
        board.ImportState($"{solutionRootPath}/Life.Test/TestState1.txt");
        board.Advance();
        Cell[,] actual = board.Cells;
        
        Cell c0 = new Cell { IsAlive = false };
        Cell c1 = new Cell { IsAlive = true };
        Cell c2 = new Cell { IsAlive = false };
        Cell c3 = new Cell { IsAlive = true };
        Cell c4 = new Cell { IsAlive = false };
        Cell c5 = new Cell { IsAlive = true };
        Cell c6 = new Cell { IsAlive = false };
        Cell c7 = new Cell { IsAlive = true };
        Cell c8 = new Cell { IsAlive = false };
        Cell[,] expected = { { c0, c3, c6 }, { c1, c4, c7 }, { c2, c5, c8 } };
        
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void AdvanceTestDynamic()
    {
        Board board = new Board(3, 3);
        board.ImportState($"{solutionRootPath}/Life.Test/TestState2.txt");
        board.Advance();
        Cell[,] actual = board.Cells;
        
        Cell c0 = new Cell { IsAlive = false };
        Cell c1 = new Cell { IsAlive = true };
        Cell c2 = new Cell { IsAlive = false };
        Cell c3 = new Cell { IsAlive = false };
        Cell c4 = new Cell { IsAlive = true };
        Cell c5 = new Cell { IsAlive = false };
        Cell c6 = new Cell { IsAlive = false };
        Cell c7 = new Cell { IsAlive = false };
        Cell c8 = new Cell { IsAlive = false };
        Cell[,] expected = { { c0, c3, c6 }, { c1, c4, c7 }, { c2, c5, c8 } };
        
        CollectionAssert.AreEqual(expected, actual);
    }
}}
