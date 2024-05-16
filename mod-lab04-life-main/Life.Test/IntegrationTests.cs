using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;

namespace Life.Test {

[TestClass]
public class IntegrationTests
{
    string solutionRootPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

    [TestMethod]
    public void IntegrationTestStatic()
    {
        GameOfLife gameOfLife = JsonReader.ReadSettings($"{solutionRootPath}/Life.Test/TestSettings.json");;
        gameOfLife.Reset($"{solutionRootPath}/Life.Test/TestState1.txt");
        
        for(int i = 0; i < gameOfLife.iterations; i++)
        {
            gameOfLife.Render();
            gameOfLife.board.Advance();
        }
        Cell[,] actual = gameOfLife.board.Cells;
        
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
    public void IntegrationTestDynamic()
    {
        GameOfLife gameOfLife = JsonReader.ReadSettings($"{solutionRootPath}/Life.Test/TestSettings.json");;
        gameOfLife.Reset($"{solutionRootPath}/Life.Test/TestState2.txt");
        
        for(int i = 0; i < gameOfLife.iterations; i++)
        {
            gameOfLife.Render();
            gameOfLife.board.Advance();
        }
        Cell[,] actual = gameOfLife.board.Cells;

        Cell c0 = new Cell { IsAlive = false };
        Cell c1 = new Cell { IsAlive = false };
        Cell c2 = new Cell { IsAlive = false };
        Cell c3 = new Cell { IsAlive = false };
        Cell c4 = new Cell { IsAlive = false };
        Cell c5 = new Cell { IsAlive = false };
        Cell c6 = new Cell { IsAlive = false };
        Cell c7 = new Cell { IsAlive = false };
        Cell c8 = new Cell { IsAlive = false };
        Cell[,] expected = { { c0, c3, c6 }, { c1, c4, c7 }, { c2, c5, c8 } };

        CollectionAssert.AreEqual(expected, actual);
    }
}}
