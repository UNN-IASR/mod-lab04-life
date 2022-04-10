using cli_life;
using Life;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod0()
        {
            Assert.IsTrue(File.Exists("Box.txt"));
        }
        [TestMethod]
        public void TestMethod1()
        {
            FigureType ft = new FigureType();
            Board board = new Board(50, 20, 1, 0);
            board.StateRead("LText1.txt");

            int stableStep = ft.StablePhase(board);
            Assert.IsTrue(stableStep == 3);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Board board = new Board(50, 20, 1, 0);
            board.StateRead("Test1.txt");

            FigureType ft = new FigureType();
            Dictionary<string, int> result = new Dictionary<string, int>();
            result = ft.CountAllFigures(board);
            Assert.IsTrue(result["Hive"] == 2);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Board board = new Board(50, 20, 1, 0);
            board.StateRead("Test2.txt");

            FigureType ft = new FigureType();
            Dictionary<string, int> result = new Dictionary<string, int>();
            result = ft.CountAllFigures(board);
            Assert.IsTrue(result["Block"] == 1);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Board board = new Board(50, 20, 1, 0);
            board.StateRead("Test2.txt");

            FigureType ft = new FigureType();
            Dictionary<string, int> result = new Dictionary<string, int>();
            result = ft.CountAllFigures(board);
            Assert.IsTrue(result["Line"] == 1);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Board board = new Board(50, 20, 1, 0);
            board.StateRead("Test3.txt");

            FigureType ft = new FigureType();
            Dictionary<string, int> result = new Dictionary<string, int>();
            result = ft.CountAllFigures(board);
            Assert.IsTrue(result["Boat"] == 1);
        }

        [TestMethod]
        public void TestMethod6()
        {
            Board board = new Board(50, 20, 1, 0);
            board.StateRead("Test3.txt");

            FigureType ft = new FigureType();
            Dictionary<string, int> result = new Dictionary<string, int>();
            result = ft.CountAllFigures(board);
            Assert.IsTrue(result["Barge"] == 1);
        }

        [TestMethod]
        public void TestMethod7()
        {
            Board board = new Board(50, 20, 1, 0);
            board.StateRead("Test3.txt");

            FigureType ft = new FigureType();
            Dictionary<string, int> result = new Dictionary<string, int>();
            result = ft.CountAllFigures(board);
            Assert.IsTrue(result["Ship"] == 1);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Board board = new Board(50, 20, 1, 0);
            board.StateRead("Test3.txt");

            FigureType ft = new FigureType();
            Dictionary<string, int> result = new Dictionary<string, int>();
            result = ft.CountAllFigures(board);
            Assert.IsTrue(result["Pond"] == 1);
        }

        [TestMethod]
        public void TestMethod9()
        {
            Board board = new Board(50, 20, 1, 0);
            board.StateRead("Test3.txt");

            FigureType ft = new FigureType();
            Dictionary<string, int> result = new Dictionary<string, int>();
            result = ft.CountAllFigures(board);
            Assert.IsTrue(result["Box"] == 1);
        }

        [TestMethod]
        public void TestMethod10()
        {
            FigureType ft = new FigureType();
            Board board = new Board(50, 20, 1, 0);
            board.StateRead("LText2.txt");

            int stableStep = ft.StablePhase(board);
            Assert.IsTrue(stableStep == 9);
        }
    }
}
