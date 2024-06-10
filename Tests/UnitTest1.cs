namespace cli_life.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            Assert.AreEqual(cli_life.Program.board.Width, 50);
        }
        [TestMethod]
        public void TestMethod2()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            Assert.AreEqual(cli_life.Program.board.Height, 20);
        }
        [TestMethod]
        public void TestMethod3()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            Assert.AreEqual(cli_life.Program.board.CellSize, 1);
        }
        [TestMethod]
        public void TestMethod4()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            Assert.AreEqual(cli_life.Program.board.LiveDensity, 0.5);
        }
        [TestMethod]
        public void TestMethod5()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("Pulsar.txt");
            Assert.AreEqual(cli_life.Program.CountAliveCells(), 48);
        }
        [TestMethod]
        public void TestMethod6()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("Acorn.txt");
            Assert.AreEqual(cli_life.Program.CountAliveCells(), 7);
        }
        [TestMethod]
        public void TestMethod7()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("Diehard.txt");
            Assert.AreEqual(cli_life.Program.CountAliveCells(), 7);
        }
        [TestMethod]
        public void TestMethod8()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("The R-pentomino.txt");
            Assert.AreEqual(cli_life.Program.CountAliveCells(), 5);
        }
        [TestMethod]
        public void TestMethod9()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("Pulsar.txt");
            cli_life.Program.board.Advance();
            cli_life.Program.board.Advance();
            Assert.AreEqual(cli_life.Program.CountAliveCells(), 72);
        }
        [TestMethod]
        public void TestMethod10()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("Acorn.txt");
            cli_life.Program.board.Advance();
            cli_life.Program.board.Advance();
            Assert.AreEqual(cli_life.Program.CountAliveCells(), 10);
        }
        [TestMethod]
        public void TestMethod11()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("Diehard.txt");
            cli_life.Program.board.Advance();
            cli_life.Program.board.Advance();
            Assert.AreEqual(cli_life.Program.CountAliveCells(), 8);
        }
        [TestMethod]
        public void TestMethod12()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("The R-pentomino.txt");
            cli_life.Program.board.Advance();
            cli_life.Program.board.Advance();
            Assert.AreEqual(cli_life.Program.CountAliveCells(), 7);
        }
        [TestMethod]
        public void TestMethod13()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("The R-pentomino.txt");
            cli_life.Program.IsStable();
            cli_life.Program.board.Advance();
            cli_life.Program.IsStable();
            cli_life.Program.board.Advance();
            Assert.AreEqual(cli_life.Program.IsStable(), false);
        }
        [TestMethod]
        public void TestMethod14()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("Loaf.txt");
            cli_life.Program.IsStable();
            cli_life.Program.board.Advance();
            cli_life.Program.IsStable();
            cli_life.Program.board.Advance();
            Assert.AreEqual(cli_life.Program.IsStable(), true);
        }
        [TestMethod]
        public void TestMethod15()
        {
            var settings = cli_life.Program.LoadSettings("board_settings.json");
            cli_life.Program.Reset(settings);
            cli_life.Program.LoadBoardState("Block.txt");
            cli_life.Program.IsStable();
            cli_life.Program.board.Advance();
            cli_life.Program.IsStable();
            cli_life.Program.board.Advance();
            Assert.AreEqual(cli_life.Program.IsStable(), true);
        }
    }
}