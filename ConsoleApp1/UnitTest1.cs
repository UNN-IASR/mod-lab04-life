using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameLife;


namespace NET
{
    [TestClass]
    public class UnitTest1
    {
        static Board board;
        [TestMethod]
        public void TestMethod1()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(40, 50, 1, 0, 10);

            board = new Board(parameters);
            board.LoadFromFile(".//BoardsExamples//Example1.txt");
            Assert.IsTrue(board.CountBox() == 0);
        }
        [TestMethod]
        public void TestMethod2()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(40, 50, 1, 0, 10);

            board = new Board(parameters);
            board.LoadFromFile(".//BoardsExamples//Example1.txt");
            Assert.IsTrue(board.GetAliveCells() == 28);
        }
        [TestMethod]
        public void TestMethod3()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(40, 50, 1, 0, 10);

            board = new Board(parameters);
            board.LoadFromFile(".//BoardsExamples//Example1.txt");
            Assert.IsTrue(board.CountBlocks() == 1);
        }
        [TestMethod]
        public void TestMethod4()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(40, 50, 1, 0, 10);

            board = new Board(parameters);
            board.LoadFromFile(".//BoardsExamples//Example1.txt");
            Assert.IsTrue(board.CountBoat() == 0);
        }
        [TestMethod]
        public void TestMethod5()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(40, 50, 1, 0, 10);

            board = new Board(parameters);
            board.LoadFromFile(".//BoardsExamples//Example1.txt");
            Assert.IsTrue(board.CountHives() == 0);
        }

    }
}
