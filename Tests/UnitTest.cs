using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ConsoleApp1;

namespace UnitTestProject1
{
    [TestClass()]
    public class Test
    {
        [TestMethod()]
        public void Test1()
        {
            Cell cell = new Cell(false);
            Assert.AreEqual(cell.Is_alive, false);
        }
        [TestMethod()]
        public void Test2()
        {
            Templates templates = new Templates();
            string[] myStrings = { "10", "01" };
            int[,] mtr = { { 1, 1 }, { 1, 1 } };
            var go = templates.Get_matrix(myStrings);
            string fgf = "";
            string gfg = "";
            foreach (var fg in go)
            {
                fgf += fg.ToString();
            }
            foreach (var ds in mtr)
            {
                gfg += ds.ToString();
            }
            Assert.AreEqual(fgf, gfg);
        }
        [TestMethod()]
        public void Test3()
        {
            Templates templates = new Templates();
            string[] myStrings = { "100", "110", "111" };
            int[,] mtr = { { 1, 0, 0 }, { 1, 1, 0 }, { 1, 1, 1 } };
            var go = templates.Get_matrix(myStrings);
            string fgf = "";
            string gfg = "";
            foreach (var fg in go)
            {
                fgf += fg.ToString();
            }
            foreach (var ds in mtr)
            {
                gfg += ds.ToString();
            }
            Assert.AreEqual(fgf, gfg);
        }
        [TestMethod()]
        public void Test4()
        {
            Templates templates = new Templates(out string str);
            Assert.AreEqual(str, "done");
        }
        [TestMethod()]
        public void Test5()
        {
            Templates templates = new Templates();
            string[] myStrings = { "111", "111", "111" };
            int[,] mtr = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            var go = templates.Get_matrix(myStrings);
            string fgf = "";
            string gfg = "";
            foreach (var fg in go)
            {
                fgf += fg.ToString();
            }
            foreach (var ds in mtr)
            {
                gfg += ds.ToString();
            }
            Assert.AreEqual(fgf, gfg);
        }
        [TestMethod()]
        public void Test6()
        {
            Templates templates = new Templates(out string str);
            Assert.AreNotEqual(str, "save");
        }
        [TestMethod()]
        public void Test7()
        {
            var board = new Board(1, 1, 1);
            board.Advance();
            Assert.AreNotEqual(board.Check_active(), false);
        }
        [TestMethod()]
        public void Test8()
        {
            var board = new Board(1, 1, 1);
            board.Advance();
            Assert.AreEqual(board.Check_active(), true);
        }
        [TestMethod()]
        public void Test9()
        {
            var board = new Board(1, 1, 1);
            var str = board.Board_save();
            Assert.AreEqual(str, "save");
        }
        [TestMethod()]
        public void Test10()
        {
            var board = new Board(1, 1, 1);
            var str = board.Discharge();
            Assert.AreEqual(str, "done");
        }
        [TestMethod()]
        public void Test11()
        {
            var board = new Board(1, 1, 1);
            var str = Board.Loading();
            Assert.IsTrue(str is Board);
        }
        [TestMethod()]
        public void Test12()
        {
            var board = new Board(1, 1, 1);
            var str = Board.Loading();
            Assert.IsTrue(str.Width == 30);
        }
        [TestMethod()]
        public void Test13()
        {
            var board = new Board(1, 1, 1);
            var str = Board.Loading();
            Cell cell = new Cell(false);
            Assert.AreEqual(cell.Is_alive, false);
        }
        [TestMethod()]
        public void Test14()
        {
            var board = new Board(1, 1, 1);
            var str = Board.Loading();
            Cell cell = new Cell(false);
            Assert.AreNotEqual(cell.Is_alive, true);
        }
        [TestMethod()]
        public void Test15()
        {
            var str = Board.Loading();
            Cell cell = new Cell(false);
            var board = new Board(1, 1, 1);
            board.Advance();
            Assert.AreEqual(board.Check_active(), true);
        }
    }
}
