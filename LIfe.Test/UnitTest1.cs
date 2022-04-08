using cli_life;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LIfe.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var data = new Data();
            data.CellSize = 1;
            data.Height = 5;
            data.Width = 5;
            data.LiveDensity = 0;

            var board = new Board(data);
            board.GetCellsFromFile(".//testboard//test1.txt");
            Assert.IsTrue(board.GetAliveCells() == 10);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var data = new Data();
            data.CellSize = 1;
            data.Height = 5;
            data.Width = 5;
            data.LiveDensity = 0;

            var board = new Board(data);
            board.GetCellsFromFile(".//testboard//test1.txt");
            Assert.IsTrue(board.BlocksAmount() == 0);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var data = new Data();
            data.CellSize = 1;
            data.Height = 5;
            data.Width = 5;
            data.LiveDensity = 0;

            var board = new Board(data);
            board.GetCellsFromFile(".//testboard//test1.txt");
            Assert.IsTrue(board.BoxesAmount() == 0);
        }

        [TestMethod]
        public void TestMethod4()
        {
            var data = new Data();
            data.CellSize = 1;
            data.Height = 5;
            data.Width = 5;
            data.LiveDensity = 0;

            var board = new Board(data);
            board.GetCellsFromFile(".//testboard//test3.txt");
            Assert.IsTrue(board.BoxesAmount() == 1);
        }

        [TestMethod]
        public void TestMethod5()
        {
            var data = new Data();
            data.CellSize = 1;
            data.Height = 5;
            data.Width = 5;
            data.LiveDensity = 0;

            var board = new Board(data);
            board.GetCellsFromFile(".//testboard//test2.txt");
            Assert.IsTrue(board.BlocksAmount() == 1);
        }
    }
}
