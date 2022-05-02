using cli_life;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Props props = new Props();
            props.cellSize = 1;
            props.height = 5;
            props.width = 5;
            props.liveDensity = 0;
            Board board = new Board(props);
            board.ReadFile(".//testboard//test1.txt");
            Assert.IsTrue(board.AliveCellsCount() == 10);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Props props = new Props();
            props.cellSize = 1;
            props.height = 5;
            props.width = 5;
            props.liveDensity = 0;
            Board board = new Board(props);
            board.ReadFile(".//testboard//test1.txt");
            Assert.IsTrue(board.BlocksCount() == 0);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Props props = new Props();
            props.cellSize = 1;
            props.height = 5;
            props.width = 5;
            props.liveDensity = 0;
            Board board = new Board(props);
            board.ReadFile(".//testboard//test1.txt");
            Assert.IsTrue(board.BoxesCount() == 0);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Props props = new Props();
            props.cellSize = 1;
            props.height = 5;
            props.width = 5;
            props.liveDensity = 0;
            Board board = new Board(props);
            board.ReadFile(".//testboard//test3.txt");
            Assert.IsTrue(board.BoxesCount() == 1);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Props props = new Props();
            props.cellSize = 1;
            props.height = 5;
            props.width = 5;
            props.liveDensity = 0;
            Board board = new Board(props);
            board.ReadFile(".//testboard//test2.txt");
            Assert.IsTrue(board.BlocksCount() == 1);
        }
    }
}