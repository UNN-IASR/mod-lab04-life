using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Life.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            String s = "life.json";
            Board board = new Board();
            int? result = board.stepstostablefile(s);
            Assert.IsTrue(result == 105);
        }
        [TestMethod]
        public void TestMethod2()
        {
            String s = "life.json";
            Board board = new Board();
            int? result = board.stablefile(s);
            Assert.IsTrue(result == 8);
        }
        [TestMethod]
        public void TestMethod3()
        {
            String s = "life.json";
            Board board = new Board();
            int? result = board.simmetricfile(s);
            Assert.IsTrue(result == 7);
        }
        [TestMethod]
        public void TestMethod4()
        {
            String s = "life.json";
            Board board = new Board();
            int? result = board.Hivefile(s);
            Assert.IsTrue(result == 2);
        }
        [TestMethod]
        public void TestMethod5()
        {
            String s = "life.json";
            Board board = new Board();
            int? result = board.Blockfile(s);
            Assert.IsTrue(result == 3);
        }
        [TestMethod]
        public void TestMethod6()
        {
            String s = "life.json";
            Board board = new Board();
            int? result = board.Boxfile(s);
            Assert.IsTrue(result == 1);
        }
    }
}