using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Tests
{
    [TestClass()]
    public class Test
    {
        [TestMethod()]
        public void PrefabTest()
        {
            Cell cell = new Cell(false);
            Assert.AreEqual(cell.IsAlive,false);
        }
        [TestMethod()]
        public void Test2()
        {
            Templates templates = new Templates();
            string[] myStrings = { "11", "11"};
            int[,] mtr = { { 1, 1 }, { 1, 1 } };
            var go = templates.GetMatrix(myStrings);
            string fgf = "";
            string gfg = "";
            foreach(var fg in go)
            {
                fgf += fg.ToString();
            }
            foreach(var ds in mtr)
            {
                gfg += ds.ToString();
            }
            Assert.AreEqual(fgf, gfg);
        }
        [TestMethod()]
        public void Test3()
        {
            Templates templates = new Templates();
            string[] myStrings = { "110", "110","111" };
            int[,] mtr = { { 1, 1,0 }, { 1, 1,0 }, {1,1,1} };
            var go = templates.GetMatrix(myStrings);
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
            Assert.AreEqual(str, "Выполнено");
        }
        [TestMethod()]
        public void Test5()
        {
            int[,] mtr = { { 1, 1 }, { 1, 1 } };
            Prefab prefab = new(2, 2, mtr, "Я хочу домой");
            Assert.AreEqual(prefab._height,2);
        }
        [TestMethod()]
        public void Test6()
        {
            int[,] mtr = { { 1, 1 }, { 1, 1 } };
            Prefab prefab = new(2, 2, mtr, "Я хочу домой");
            Assert.AreEqual(prefab._name, "Я хочу домой");
        }
        [TestMethod()]
        public void Test7()
        {
            int[,] mtr = { { 1, 1 }, { 1, 1 } };
            Prefab prefab = new(2, 2, mtr, "Я хочу домой");
            Assert.AreEqual(prefab.CheckPrefab(new Board(1,1,1)),0);
        }
        [TestMethod()]
        public void Test8()
        {
            int[,] mtr = { { 1, 1, 1 }, { 1, 1, 1 } };
            Prefab prefab = new(2, 2, mtr, "Я хочу домой");
            Assert.AreEqual(prefab.CheckPrefab(new Board(1, 1, 1)), 0);
        }
        [TestMethod()]
        public void Test9()
        {
            var board = new Board(1, 1, 1);
            board.Advance();
            Assert.AreEqual(board.CheckActive(),true );
        }
        [TestMethod()]
        public void Test101()
        {
            var board = new Board(1, 1, 1);
            var str = board.BoardSave();
            Assert.AreEqual(str,"Сохранено");
        }
        [TestMethod()]
        public void Test11()
        {
            var board = new Board(1, 1, 1);
            var str = board.Discharge();
            Assert.AreEqual(str, "Выгружено");
        }
        [TestMethod()]
        public void Test12()
        {
            var board = new Board(1, 1, 1);
            var str = Board.Loading();
            Assert.IsTrue(str is Board);
        }
        [TestMethod()]
        public void Test13()
        {
            var board = new Board(1, 1, 1);
            var str = Board.Loading();
            Assert.IsTrue(str.Width==30);
        }
        [TestMethod()]
        public void Test14()
        {
            var board = new Board(1, 1, 1);
            var str = Board.Loading();
            Cell cell = new Cell(false);
            Assert.AreEqual(cell.IsAlive, false);
        }
        [TestMethod()]
        public void Test15()
        {
            var str = Board.Loading();
            Cell cell = new Cell(false);
            var board = new Board(1, 1, 1);
            board.Advance();
            Assert.AreEqual(board.CheckActive(), true);
        }
    }
}
