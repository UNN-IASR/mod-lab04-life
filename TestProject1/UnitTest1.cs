using cli_life;

namespace TestProject1;

[TestClass]
    public class BoardTests
    {
        private readonly Figure rhomb;
        private readonly Figure square;
        private readonly Figure ellipse;
        private readonly Figure boat;
        private readonly Figure ship;

        public BoardTests()
        {
            rhomb = new ("       *   * *   *       ", 5);
            square = new ("     **  **     ", 4);
            ellipse = new ("        **   *  *  *  *   **        ", 6);
            boat = new ("       *   * *   **      ", 5);
            ship = new ("       **   * *   **     ", 5);
        }
        [TestMethod]
        public void BoardConstructorTest()
        {
            Board board = new (20, 30, 1, 0.1);
            Assert.AreEqual(20, board.Width);
        }

        [TestMethod]
        public void CheckBoatTest()
        {
            Board board = new (5, 5, 1);
            board.Load("../../../../Life/boat.txt");
            Assert.AreEqual(1, board.FindPattern(boat));
        }

        [TestMethod]
        public void CheckBoatsTest()
        {
            Board board = new (5, 21, 1);
            board.Load("../../../../Life/boats.txt");
            Assert.AreEqual(5, board.FindPattern(boat));
        }

        [TestMethod]
        public void CheckSquareTest()
        {
            Board board = new (4, 4, 1);
            board.Load("../../../../Life/square.txt");            
            Assert.AreEqual(1, board.FindPattern(square));
        }

        [TestMethod]
        public void CheckSquaresTest()
        {
            Board board = new (4, 13, 1);
            board.Load("../../../../Life/squares.txt");
            Assert.AreEqual(4, board.FindPattern(square));
        }

        [TestMethod]
        public void CheckShipTest()
        {
            Board board = new (5, 5, 1);
            board.Load("../../../../Life/ship.txt");
            Assert.AreEqual(1, board.FindPattern(ship));
        }

        [TestMethod]
        public void CheckShipsTest()
        {
            Board board = new (5, 17, 1);
            board.Load("../../../../Life/ships.txt");
            Assert.AreEqual(4, board.FindPattern(ship));
        }

        [TestMethod]
        public void CheckRhombTest()
        {
            Board board = new (5, 5, 1);
            board.Load("../../../../Life/rhomb.txt");
            Assert.AreEqual(1, board.FindPattern(rhomb));
        }

        [TestMethod]
        public void CheckRhombsTest()
        {
            Board board = new (5, 17, 1);
            board.Load("../../../../Life/rhombs.txt");
            Assert.AreEqual(4, board.FindPattern(rhomb));
        }

        [TestMethod]
        public void CheckEllipseTest()
        {
            Board board = new (6, 6, 1);
            board.Load("../../../../Life/ellipse.txt");
            Assert.AreEqual(1, board.FindPattern(ellipse));
        }

        [TestMethod]
        public void CheckEllipsesTest()
        {
            Board board = new (6, 16, 1);
            board.Load("../../../../Life/ellipses.txt");
            Assert.AreEqual(3, board.FindPattern(ellipse));
        }

        [TestMethod]
        public void CheckAliveInSquareTest()
        {
            Board board = new (4, 4, 1);
            board.Load("../../../../Life/square.txt");
            Assert.AreEqual(4, board.CheckAlive());
        }

        [TestMethod]
        public void CheckAliveInEllipseTest()
        {
            Board board = new (6, 6, 1);
            board.Load("../../../../Life/ellipse.txt");
            Assert.AreEqual(8, board.CheckAlive());
        }

        [TestMethod]
        public void IsStableTest()
        {
            Board board = new (100, 100, 1);
            board.Load("../../../../Life/state.txt");
            for(int i=0; i<5; i++) 
            {
                board.Advance();
            }
            Assert.IsFalse(board.CheckStable());
        }

        [TestMethod]
        public void ResetTest()
        {
            Board board = Program.Reset("../../../../Life/settings.json");
            Assert.AreEqual(board.Height, 100);
        }

    }