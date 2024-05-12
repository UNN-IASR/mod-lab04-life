namespace cli_life.Tests
{
    [TestClass]
    public class LifeTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            BoardSettings settings = new BoardSettings
            {
                Width = 20,
                Height = 8,
                CellSize = 1,
                LiveDensity = .1
            };

            settings.SaveInFile("../../../settings.json");

            Assert.IsTrue(File.Exists("../../../settings.json"));
        }

        [TestMethod]
        public void TestMethod2()
        {
            BoardSettings settings = new BoardSettings
            {
                Width = 20,
                Height = 8,
                CellSize = 1,
                LiveDensity = .1
            };
            settings.SaveInFile("../../../settings.json");

            settings.LoadFromFile("../../../settings.json");

            Assert.IsTrue(settings.Width == 20 && settings.Height == 8
                && settings.CellSize == 1 && settings.LiveDensity == .1);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Cell cell = new Cell();
            cell.neighbors.Add(new Cell());

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsFalse(cell.IsAlive);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Cell cell = new Cell();
            cell.IsAlive = true;
            cell.neighbors.Add(new Cell());

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsFalse(cell.IsAlive);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Cell cell = new Cell();
            Cell cellNeighbour = new Cell();
            cellNeighbour.IsAlive = true;
            for (int index = 0; index < 3; index++)
            {
                cell.neighbors.Add(cellNeighbour);
            }

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsTrue(cell.IsAlive);
        }

        [TestMethod]
        public void TestMethod6()
        {
            Cell cell = new Cell();
            cell.IsAlive = true;
            Cell cellNeighbour = new Cell();
            cellNeighbour.IsAlive = true;
            for (int index = 0; index < 3; index++)
            {
                cell.neighbors.Add(cellNeighbour);
            }

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsTrue(cell.IsAlive);
        }

        [TestMethod]
        public void TestMethod7()
        {
            Cell cell = new Cell();
            Cell cellNeighbour = new Cell();
            cellNeighbour.IsAlive = true;
            for (int index = 0; index < 9; index++)
            {
                cell.neighbors.Add(cellNeighbour);
            }

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsFalse(cell.IsAlive);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Cell cell = new Cell();
            cell.IsAlive = true;
            Cell cellNeighbour = new Cell();
            cellNeighbour.IsAlive = true;
            for (int index = 0; index < 9; index++)
            {
                cell.neighbors.Add(cellNeighbour);
            }

            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsFalse(cell.IsAlive);
        }

        [TestMethod]
        public void TestMethod9()
        {
            Board board = new Board(3, 3, 1, .1);

            board.SaveInFile("../../../board.txt");

            Assert.IsTrue(File.Exists("../../../board.txt"));
        }

        [TestMethod]
        public void TestMethod10()
        {
            Board board = new Board(3, 3, 1, .1);
            board.SaveInFile("../../../board.txt");

            board = new Board("../../../board.txt");

            Assert.AreEqual(9, board.Rows * board.Columns);
        }

        [TestMethod]
        public void TestMethod11()
        {
            Board board = new Board(3, 3, 1, 1);

            Assert.AreEqual(9, board.GetAliveCellsNumber());
        }

        [TestMethod]
        public void TestMethod12()
        {
            Board board = new Board(3, 3, 1, 1);

            board.Advance();

            Assert.AreEqual(9, board.PreviousAliveCells);
        }

        [TestMethod]
        public void TestMethod13()
        {
            Board board = new Board(3, 3, 1, 1);

            for (int index = 0; index < 3; index++)
            {
                board.Advance();
            }

            Assert.AreEqual(1, board.StableStateGeneration);
        }

        [TestMethod]
        public void TestMethod14()
        {
            Board board = new Board(3, 3, 1, 1);

            for (int index = 0; index < 3; index++)
            {
                board.Advance();
            }

            Assert.AreEqual(3, board.Generation);
        }

        [TestMethod]
        public void TestMethod15()
        {
            Cell[,] cells = new Cell[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    cells[i, j] = new Cell();
                }
            }
            cells[1, 1].IsAlive = true;

            StaticFigure figure = StaticFigure.CreateFigure(cells, 0, 0, 3, 3);

            Assert.AreEqual("   ", figure.Figure[0]);
            Assert.AreEqual(" * ", figure.Figure[1]);
            Assert.AreEqual("   ", figure.Figure[2]);
        }

        [TestMethod]
        public void TestMethod16()
        {
            StaticFigure firstFigure = new StaticFigure
            {
                Name = "first",
                Figure = new string[]
                {
                    " * "
                }
            };
            StaticFigure secondFigure = new StaticFigure
            {
                Name = "second",
                Figure = new string[]
                {
                    " * "
                }
            };

            Assert.IsTrue(firstFigure.Equals(secondFigure));
        }

        [TestMethod]
        public void TestMethod17()
        {
            StaticFigure figure = new StaticFigure
            {
                Name = "figure",
                Figure = new string[]
                {
                    " * "
                }
            };

            Assert.IsFalse(figure.Equals(new object()));
        }

        [TestMethod]
        public void TestMethod18()
        {
            StaticFigure firstFigure = new StaticFigure
            {
                Name = "first",
                Figure = new string[]
                {
                    " *  ",
                    "    "
                }
            };
            StaticFigure secondFigure = new StaticFigure
            {
                Name = "second",
                Figure = new string[]
                {
                    " ** ",
                    " ** "
                }
            };

            Assert.IsFalse(firstFigure.Equals(secondFigure));
        }
    }
}