using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Configuration configuration = new Configuration();
            configuration.LoadConfig("C:\\Users\\Makskyreer\\Desktop\\mod-lab04-life-b34\\Configuration.json");
            var board = new Board(configuration);
            configuration.LoadState(board, "C:\\Users\\Makskyreer\\Desktop\\mod-lab04-life-b34\\Tests\\test1.txt");
            Assert.IsTrue(configuration.GetAliveCells(board) == 15);
        }
        [TestMethod]
        public void TestMethod2()
        {
            Configuration configuration = new Configuration();
            configuration.LoadConfig("C:\\Users\\Makskyreer\\Desktop\\mod-lab04-life-b34\\Configuration.json");
            var board = new Board(configuration);
            configuration.LoadState(board, "C:\\Users\\Makskyreer\\Desktop\\mod-lab04-life-b34\\Tests\\test2.txt");
            Assert.IsTrue(configuration.GetAliveCells(board) == 17);
        }
        [TestMethod]
        public void TestMethod3()
        {
            Configuration configuration = new Configuration();
            configuration.LoadConfig("C:\\Users\\Makskyreer\\Desktop\\mod-lab04-life-b34\\Configuration.json");
            var board = new Board(configuration);
            configuration.LoadState(board, "C:\\Users\\Makskyreer\\Desktop\\mod-lab04-life-b34\\Tests\\test3.txt");
            Assert.IsTrue(configuration.GetAliveCells(board) == 3);
        }
        [TestMethod]
        public void TestMethod4()
        {
            Configuration configuration = new Configuration();
            configuration.LoadConfig("C:\\Users\\Makskyreer\\Desktop\\mod-lab04-life-b34\\Configuration.json");
            var board = new Board(configuration);
            configuration.LoadState(board, "C:\\Users\\Makskyreer\\Desktop\\mod-lab04-life-b34\\Tests\\test4.txt");
            Assert.IsTrue(configuration.GetAliveCells(board) == 9);
        }
        [TestMethod]
        public void TestMethod5()
        {
            Configuration configuration = new Configuration();
            configuration.LoadConfig("C:\\Users\\Makskyreer\\Desktop\\mod-lab04-life-b34\\Configuration.json");
            var board = new Board(configuration);
            configuration.LoadState(board, "C:\\Users\\Makskyreer\\Desktop\\mod-lab04-life-b34\\Tests\\test5.txt");
            Assert.IsTrue(configuration.GetAliveCells(board) == 20);
        }
    }
}

