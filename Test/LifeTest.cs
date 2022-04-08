using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;

namespace life_test
{
    [TestClass]
    public class LifeTest
    {
        [TestMethod]
        public void Test1() {
            bool t = false;
            Board board;
            board = infile.readf("1box.json");
            if ((board.block()==1) && board.xsimmetriya() && board.ysimmetriya()) {
                t = true;
            }
            Assert.IsTrue(t == true);
        }
        [TestMethod]
        public void Test2()
        {
            bool t = false;
            Board board;
            board = infile.readf("4mig.json");
            if ((board.migalka() == 4) &&!board.xsimmetriya() && !board.ysimmetriya())
            {
                t = true;
            }
            Assert.IsTrue(t == true);
        }
        [TestMethod]
        public void Test3()
        {
            bool t = false;
            Board board;
            board = infile.readf("6tub.json");
            if ((board.tub() == 6) && board.xsimmetriya() && !board.ysimmetriya())
            {
                t = true;
            }
            Assert.IsTrue(t == true);
        }
        [TestMethod]
        public void Test4()
        {
            bool t = false;
            Board board;
            board = infile.readf("8s8b.json");
            if ((board.ship() == 8) && (board.boat() == 8) && !board.xsimmetriya() && !board.ysimmetriya())
            {
                t = true;
            }
            Assert.IsTrue(t == true);
        }
        [TestMethod]
        public void Test5()
        {
            bool t = false;
            Board board;
            board = infile.readf("6block.json");
            if ((board.block() == 6) && board.xsimmetriya() && !board.ysimmetriya())
            {
                t = true;
            }
            Assert.IsTrue(t == true);
        }

        [TestMethod]
        public void Test6()
        {
            bool t = false;
            Board board;
            board = infile.readf("4s4b4t6m.json");
            if ((board.ship() == 4) && (board.boat() == 4) && (board.tub() == 4) && (board.migalka() == 6) && !board.xsimmetriya() && !board.ysimmetriya())
            {
                t = true;
            }
            Assert.IsTrue(t == true);
        }
        [TestMethod]
        public void Test7()
        {
            bool t = false;
            Board board;
            board = infile.readf("4s4b4t7b6m.json");
            if ((board.ship() == 4) && (board.boat() == 4) && (board.tub() == 4) && (board.migalka() == 6)
                && (board.block() == 7) && !board.xsimmetriya() && !board.ysimmetriya())
            {
                t = true;
            }
            Assert.IsTrue(t == true);
        }
    }
}
