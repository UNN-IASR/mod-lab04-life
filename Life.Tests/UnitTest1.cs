using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;
using System.IO;
using Newtonsoft.Json;

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
            LG.ReadBoard("test_output.txt");
            Assert.AreEqual(LG.CountAliveCells(), 64);
        }
        [TestMethod]
        public void TestMethod5()
        {
            LifeGame LG = new LifeGame();
            LG.Reset();
            LG.ReadBoard("test_output.txt");
            Assert.AreEqual(LG.BlocksCount(), 3);
        }
    }
}
