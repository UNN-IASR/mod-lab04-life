using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            String s = "file1.json";
            Board board = new Board();
            int? result = board.stepstostablefile(s);
            Assert.IsTrue(result == 330);
        }
        [TestMethod]
        public void TestMethod2()
        {
            String s = "file1.json";
            Board board = new Board();
            int? result = board.stablefile(s);
            Assert.IsTrue(result == 10);
        }
        [TestMethod]
        public void TestMethod3()
        {
            String s = "file1.json";
            Board board = new Board();
            int? result = board.simmetricfile(s);
            Assert.IsTrue(result == 9);
        }
        [TestMethod]
        public void TestMethod4()
        {
            String s = "file1.json";
            Board board = new Board();
            int? result = board.Hivefile(s);
            Assert.IsTrue(result == 2);
        }
        [TestMethod]
        public void TestMethod5()
        {
            String s = "file1.json";
            Board board = new Board();
            int? result = board.Blockfile(s);
            Assert.IsTrue(result == 6);
        }
        [TestMethod]
        public void TestMethod6()
        {
            String s = "file1.json";
            Board board = new Board();
            int? result = board.Boxfile(s);
            Assert.IsTrue(result == 0);
        }
        [TestMethod]
        public void TestMethod7()
        {
            String s = "file1.json";
            Board board = new Board();
            int? result = board.Pondfile(s);
            Assert.IsTrue(result == 0);
        }

        [TestMethod]
        public void TestMethod8()
        {
            String s = "file2.json";
            Board board = new Board();
            int? result = board.stepstostablefile(s);
            Assert.IsTrue(result == 365);
        }
        [TestMethod]
        public void TestMethod9()
        {
            String s = "file2.json";
            Board board = new Board();
            int? result = board.stablefile(s);
            Assert.IsTrue(result == 5);
        }
        [TestMethod]
        public void TestMethod10()
        {
            String s = "file2.json";
            Board board = new Board();
            int? result = board.simmetricfile(s);
            Assert.IsTrue(result == 6);
        }
        [TestMethod]
        public void TestMethod11()
        {
            String s = "file2.json";
            Board board = new Board();
            int? result = board.Hivefile(s);
            Assert.IsTrue(result == 1);
        }
        [TestMethod]
        public void TestMethod12()
        {
            String s = "file2.json";
            Board board = new Board();
            int? result = board.Blockfile(s);
            Assert.IsTrue(result == 1);
        }
        [TestMethod]
        public void TestMethod13()
        {
            String s = "file2.json";
            Board board = new Board();
            int? result = board.Boxfile(s);
            Assert.IsTrue(result == 0);
        }
        [TestMethod]
        public void TestMethod14()
        {
            String s = "file2.json";
            Board board = new Board();
            int? result = board.Pondfile(s);
            Assert.IsTrue(result == 0);
        }
    }
}

