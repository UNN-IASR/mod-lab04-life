using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Life.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            Assert.AreEqual(LG.GetCellSize(), 1);
        }
        [TestMethod]
        public void TestMethod2()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            Assert.AreEqual(LG.GetWidth(), 100);
        }
        [TestMethod]
        public void TestMethod3()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            Assert.AreEqual(LG.GetHeight(), 20);
        }
        [TestMethod]
        public void TestMethod4()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            LG.ReadBoard("../../../test_output.txt");
            Assert.AreEqual(LG.CountAliveCells(), 64);
        }
        [TestMethod]
        public void TestMethod5()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            LG.ReadBoard("../../../test_output.txt");
            Assert.AreEqual(LG.BlocksCount(), 3);
        }
        [TestMethod]
        public void TestMethod6()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            int start = LG.CountAliveCells();
            LG.Advance();
            int end = LG.CountAliveCells();
            Assert.IsTrue(end <= start);
        }

        [TestMethod]
        public void TestMethod7()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            int start = LG.CountAliveCells();
            int i = 0;
            while (i != 500)
            {
                LG.Advance();
                i++;
            }

            int end = LG.CountAliveCells();
            Assert.IsTrue(end <= (int)start/2);
        }
        [TestMethod]
        public void TestMethod8()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            int start = LG.CountAliveCells();
            Assert.IsTrue((900<=start)&&(start<=1100));
        }
        [TestMethod]
        public void TestMethod9()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            LG.board.Randomize(0.3);
            int start = LG.CountAliveCells();
            Assert.IsTrue((500 <= start) && (start <= 700));
        }

        [TestMethod]
        public void TestMethod10()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            LG.board.Randomize(0.7);
            int start = LG.CountAliveCells();
            Assert.IsTrue((1300 <= start) && (start <= 1500));
        }

        [TestMethod]
        public void TestMethod11()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            LG.board.Randomize(1.0);
            int start = LG.CountAliveCells();
            Assert.IsTrue(start == 2000);
        }

        [TestMethod]
        public void TestMethod12()
        {
            LifeGame LG = new LifeGame();   
            LG.Reset();
            LG.board.Randomize(1.0);
            LG.Advance();
            Assert.AreEqual(LG.CountAliveCells(), 0);
        }

        [TestMethod]
        public void TestMethod13()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            LG.board.Randomize(0.001);
            LG.Advance();
            Assert.AreEqual(LG.CountAliveCells(), 0);
        }

        [TestMethod]
        public void TestMethod14()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            LG.board.Randomize(0.0);
            
            Assert.AreEqual(LG.CountAliveCells(), 0);
        }

        [TestMethod]
        public void TestMethod15()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            LG.board.Randomize(0.0);
            LG.Advance();
            Assert.AreEqual(LG.CountAliveCells(), 0);
        }
    }
}