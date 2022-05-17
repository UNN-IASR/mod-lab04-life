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
            Game game = new Game();
            game.cellSize = 1;
            game.height = 5;
            game.width = 5;
            game.liveDensity = 0;
            Board board = new Board(game);
            board.ReadFile(".//Tests//test1.txt");
            Assert.IsTrue(board.AliveCellsCount() == 10);
        }

       [TestMethod]
        public void TestMethod1()
        {
            Game game = new Game();
            game.cellSize = 1;
            game.height = 10;
            game.width = 10;
            game.liveDensity = 0;
            Board board = new Board(game);
            board.ReadFile(".//Tests//test2.txt");
            Assert.IsTrue(board.AliveCellsCount() == 5);
        }

       [TestMethod]
        public void TestMethod1()
        {
            Game game = new Game();
            game.cellSize = 1;
            game.height = 10;
            game.width = 10;
            game.liveDensity = 0;
            Board board = new Board(game);
            board.ReadFile(".//Tests//test3.txt");
            Assert.IsTrue(board.BoxesCount() == 0);
        }

       [TestMethod]
        public void TestMethod1()
        {
            Game game = new game();
            game.cellSize = 1;
            game.height = 10;
            game.width = 10;
            game.liveDensity = 0;
            Board board = new Board(game);
            board.ReadFile(".//Tests//test4.txt");
            Assert.IsTrue(board.BlocksCount() == 1);
        }

        [TestMethod]
        public void TestMethod1()
        {
            Game game = new Game();
            game.cellSize = 1;
            game.height = 10;
            game.width = 10;
            game.liveDensity = 0;
            Board board = new Board(game);
            board.ReadFile(".//Tests//test5.txt");
            Assert.IsTrue(board.BoxesCount() == 1);
        }
    }
}
