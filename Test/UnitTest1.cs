using cli_life;

using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestBoard1()
        {
            Board board = new Board(50, 20, 1, 0.5);
            bool x1 = (board.Height == 20);
            bool x2 = (board.Width == 50);
            Assert.IsTrue(x1 && x2);
        }
        [TestMethod]
        public void TestBoard2()
        {
            string view = "              \n       **     \n       **     \n              \n     ****     \n ** *    *    \n ** **   *    \n    * ** * ** \n    *    * ** \n     ****     \n              \n     **       \n     **       \n              ";
            Board board = new Board(view);
            Assert.IsTrue(view == board.toString());
        }
        [TestMethod]
        public void TestBoard3()
        {
            string view = "" +
                "              \n" +
                "       **     \n" +
                "       **     \n" +
                "              \n" +
                "     ****     \n" +
                " ** *    *    \n" +
                " ** **   *    \n" +
                "    * ** * ** \n" +
                "    *    * ** \n" +
                "     ****     \n" +
                "              \n" +
                "     **       \n" +
                "     **       \n" +
                "              ";
            Board board = new Board(view);
            int count1 = board.Count();
            board.Advance();
            int count2 = board.Count();
            Assert.IsTrue(count1 == count2);
        }
        [TestMethod]
        public void TestBoard4()
        {
            string view = "" +
                "              \n" +
                "       **     \n" +
                "       **     \n" +
                "              \n" +
                "              \n" +
                " **           \n" +
                " **           \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "     **       \n" +
                "     **       \n" +
                "              ";
            Board board = new Board(view);
            string str1 = board.toString();
            board.Advance();
            string str2 = board.toString();
            Assert.IsTrue(str1 == str2);
        }
        [TestMethod]
        public void TestBoard5()
        {
            string view1 = "" +
                "              \n" +
                "       **     \n" +
                "       **     \n" +
                "              \n" +
                "              \n" +
                " **    *      \n" +
                " **    *      \n" +
                "       *      \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "     **       \n" +
                "     **       \n" +
                "              ";
            string view2 = "" +
                "              \n" +
                "       **     \n" +
                "       **     \n" +
                "              \n" +
                "              \n" +
                " **           \n" +
                " **   ***     \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "     **       \n" +
                "     **       \n" +
                "              ";
            Board board = new Board(view1);
            board.Advance();
            Assert.IsTrue(view2 == board.toString());
        }
        [TestMethod]
        public void TestBoard6()
        {
            string view = "" +
                "          \n" +
                "    **    \n" +
                "   ****   \n" +
                "  *    *  \n" +
                " **    ** \n" +
                " **    ** \n" +
                "  *    *  \n" +
                "   ****   \n" +
                "    **    \n" +
                "          ";
            Board board = new Board(view);
            string str1 = board.toString();
            board.Advance();
            string str2 = board.toString();
            for (int i = 0; i < 4; i++) board.Advance();
            string str3 = board.toString();
            Assert.IsTrue((str1 != str2) && (str1 == str3));
        }
        [TestMethod]
        public void TestBoard7()
        {
            string view = "" +
                "              \n" +
                "       **     \n" +
                "       **     \n" +
                "              \n" +
                "              \n" +
                " **           \n" +
                " **           \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "     **       \n" +
                "     **       \n" +
                "              ";
            Board board = new Board(view);
            board.Advance(true);
            Assert.IsTrue(!board.HasChanged());
        }
        [TestMethod]
        public void TestBoard8()
        {
            string view = "" +
                "              \n" +
                "       **     \n" +
                "       **     \n" +
                "              \n" +
                "              \n" +
                " ***          \n" +
                " **           \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "     **       \n" +
                "     **       \n" +
                "              ";
            Board board = new Board(view);
            board.Advance(true);
            Assert.IsTrue(board.HasChanged());
        }
        [TestMethod]
        public void TestBoard9()
        {
            string view = "" +
                "              \n" +
                "       **     \n" +
                "       **     \n" +
                "              \n" +
                "              \n" +
                " **           \n" +
                " **           \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "     **       \n" +
                "     **       \n" +
                "              ";
            Board board = new Board(view);
            var figures = board.SearchForShapes();
            Assert.IsTrue(figures["block"] == 3);
        }
        [TestMethod]
        public void TestBoard10()
        {
            string view = "" +
                "  **    *   *                          **    **  *\n" +
                "        *   *                       **       ***  \n" +
                " * *  **       *                  **   *     **   \n" +
                "** ***    *   ***                 ******      *** \n" +
                "  *  *   * * *   *                 *** *       * *\n" +
                "*    *    *   ***                   **        **  \n" +
                "      **       *                     *          **\n" +
                "        *   *         **                          \n" +
                "  *         *        *  *                         \n" +
                "** *    ** *          **                          \n" +
                "** * ***  *                         *     *       \n" +
                "    *   ***                      * * *   **       \n" +
                "    *                      ***** *      *     *   \n" +
                "     **** *                  * * *    ****    *   \n" +
                "       ***                   *        **          \n" +
                "*                            **   *   *      ** **\n" +
                "                                  ****       **   \n" +
                "  *                                         ** ** \n" +
                "  *                            * *           *    \n" +
                "*  *     ***                   **      **        *";
            Board board = new Board(view);
            var figures = board.SearchForShapes();
            bool x1 = (figures["block"] == 1);
            bool x2 = (figures["HiveHorizontal"] == 1);
            bool x3 = (figures["Tub"] == 1);
            bool x4 = (figures["StickVertical"] == 1);
            bool x5 = (figures["StickHorizontal"] == 1);
            Assert.IsTrue(x1 && x2 && x3 && x4 && x5);
        }
        [TestMethod]
        public void TestBoard11()
        {
            string view = "" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "     *        \n" +
                "      *       \n" +
                "    ***       \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              ";
            Board board = new Board(view);
            int time = Board.ConvergenceTime(1000, board);
            Assert.IsTrue(time == 56);
        }
        [TestMethod]
        public void TestBoard12()
        {
            string view = "" +
                "              \n" +
                "       **     \n" +
                "       **     \n" +
                "              \n" +
                "              \n" +
                " **           \n" +
                " **           \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "              \n" +
                "     **       \n" +
                "     **       \n" +
                "              ";
            Board board = new Board(view);
            int time = Board.ConvergenceTime(1000, board);
            Assert.IsTrue(time == 1);
        }
        [TestMethod]
        public void TestBoard13()
        {
            string view = "" +
                "               \n" +
                "       **      \n" +
                "       **      \n" +
                "               \n" +
                "       *       \n" +
                " **          * \n" +
                " **            \n" +
                "       *       \n" +
                "               \n" +
                "          *    \n" +
                "               \n" +
                "     **        \n" +
                "     **        \n" +
                "               ";
            Board board = new Board(view);
            Assert.IsTrue((board.Height == 14) && (board.Width == 15));
        }
        [TestMethod]
        public void TestBoard14()
        {
            string view = "" +
                "               \n" +
                "       **      \n" +
                "       **      \n" +
                "               \n" +
                "       *       \n" +
                " **          * \n" +
                " **            \n" +
                "       *       \n" +
                "               \n" +
                "          *    \n" +
                "               \n" +
                "     **        \n" +
                "     **        \n" +
                "               ";
            Board board = new Board(view);
            var cells = board.CellsTrans();
            Assert.IsTrue((cells.GetLength(0) == 14) && (cells.GetLength(1) == 15));
        }
        [TestMethod]
        public void TestBoard15()
        {
            string view = "" +
                    "               \n" +
                    "       **      \n" +
                    "       **      \n" +
                    "               \n" +
                    "       *       \n" +
                    " **          * \n" +
                    " **            \n" +
                    "       *       \n" +
                    "               \n" +
                    "          *    \n" +
                    "               \n" +
                    "     **        \n" +
                    "     **        \n" +
                    "               ";
            Board board = new Board(view);
            Assert.IsTrue(board.toString().Length == view.Length);
        }
    }
}