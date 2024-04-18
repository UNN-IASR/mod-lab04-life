using cli_life;

namespace TestProject1;

[TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void BoardConstructorTest()
        {
            Board board = new Board(20, 30, 1, 0.1);

            Assert.AreEqual(20, board.Width);
            Assert.AreEqual(30, board.Height);
        }

        [TestMethod]
        public void CheckBoatTest()
        {
            Board board = new Board(5, 5, 1);
            board.Load("../../../../Life/boat.txt");
            Pattern boat = new Pattern("Boat", ".......*...*.*..**.......", 5);

            board.FindPattern(boat);
            Assert.AreEqual(boat.count, 1);
        }

        [TestMethod]
        public void CheckSomeBoatTest()
        {
            Board board = new Board(5, 21, 1);
            board.Load("../../../../Life/boats.txt");
            Pattern boat = new Pattern("Boat", ".......*...*.*..**.......", 5);

            board.FindPattern(boat);
            Assert.AreEqual(boat.count, 5);
        }

        [TestMethod]
        public void CheckBlockTest()
        {
            Board board = new Board(4, 4, 1);
            board.Load("../../../../Life/block.txt");
            Pattern block = new Pattern("Block", ".....**..**.....", 4);

            board.FindPattern(block);
            Assert.AreEqual(block.count, 1);
        }

        [TestMethod]
        public void CheckSomeBlockTest()
        {
            Board board = new Board(4, 13, 1);
            board.Load("../../../../Life/blocks.txt");
            Pattern block = new Pattern("Block", ".....**..**.....", 4);

            board.FindPattern(block);
            Assert.AreEqual(block.count, 4);
        }

        [TestMethod]
        public void CheckShipTest()
        {
            Board board = new Board(5, 5, 1);
            board.Load("../../../../Life/ship.txt");
            Pattern ship = new Pattern("Ship", ".......**..*.*..**.......", 5);

            board.FindPattern(ship);
            Assert.AreEqual(ship.count, 1);
        }

        [TestMethod]
        public void CheckSomeShipTest()
        {
            Board board = new Board(5, 17, 1);
            board.Load("../../../../Life/ships.txt");
            Pattern ship = new Pattern("Ship", ".......**..*.*..**.......", 5);

            board.FindPattern(ship);
            Assert.AreEqual(4, ship.count);
        }

        [TestMethod]
        public void CheckTubTest()
        {
            Board board = new Board(5, 5, 1);
            board.Load("../../../../Life/tub.txt");
            Pattern tub = new Pattern("Tub", ".......*...*.*...*.......", 5);

            board.FindPattern(tub);
            Assert.AreEqual(tub.count, 1);
        }

        [TestMethod]
        public void CheckSomeTubTest()
        {
            Board board = new Board(5, 17, 1);
            board.Load("../../../../Life/tubs.txt");
            Pattern tub = new Pattern("Tub", ".......*...*.*...*.......", 5);

            board.FindPattern(tub);
            Assert.AreEqual(tub.count, 4);
        }

        [TestMethod]
        public void CheckLoafTest()
        {
            Board board = new Board(6, 6, 1);
            board.Load("../../../../Life/loaf.txt");
            Pattern loaf = new Pattern("Loaf", "........**...*..*...*.*....*........", 6);

            board.FindPattern(loaf);
            Assert.AreEqual(loaf.count, 1);
        }

        [TestMethod]
        public void CheckSomeLoafTest()
        {
            Board board = new Board(6, 16, 1);
            board.Load("../../../../Life/loafs.txt");
            Pattern loaf = new Pattern("Loaf", "........**...*..*...*.*....*........", 6);

            board.FindPattern(loaf);
            Assert.AreEqual(loaf.count, 3);
        }

        [TestMethod]
        public void CheckAliveInBlockTest()
        {
            Board board = new Board(4, 4, 1);
            board.Load("../../../../Life/block.txt");

            Assert.AreEqual(board.CheckAlive(), 4);
        }

        [TestMethod]
        public void CheckAliveInBoatTest()
        {
            Board board = new Board(4, 4, 1);
            board.Load("../../../../Life/boat.txt");

            Assert.AreEqual(board.CheckAlive(), 5);
        }

        [TestMethod]
        public void IsStableTest()
        {
            Board board = new Board(100, 100, 1);
            board.Load("../../../../Life/state11.txt");
            for(int i=0; i<5; i++) board.Advance();

            Assert.IsFalse(board.CheckStable());
        }

        [TestMethod]
        public void ResetTest()
        {
            Board board = Program.Reset("../../../../Life/settings.json");

            Assert.AreEqual(board.Width, 100);
        }

    }