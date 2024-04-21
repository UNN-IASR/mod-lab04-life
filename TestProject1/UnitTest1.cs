using cli_life;
using System.Text.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using NuGet.Frameworks;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod_1()
        {
            Board board = new Board(90, 25, 1, 0.1);
            Assert.AreEqual(25, board.Height);
            Assert.AreEqual(90, board.Width);
        }

        [TestMethod]
        public void TestMethod_2()
        {
            string jsonString = File.ReadAllText("../../../for_test2.json");
            Settings? settings = JsonSerializer.Deserialize<Settings>(jsonString);

            Assert.AreEqual(60, settings.Width);
            Assert.AreEqual(30, settings.Height);
            Assert.AreEqual(3, settings.cellSize);
            Assert.AreEqual(0.2, settings.liveDensity);
        }

        [TestMethod]
        public void TestMethod_3()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Load_data_from_file("../../../../Life/test3.txt");
            Figures[] fig = Figures.Load_figures_from_file("../../../figures_tests.json");
            Figures pond = fig[1];
            int count = Figures.Find_Figures(pond, board);
            Assert.AreEqual(count, 1);
        }

        [TestMethod]
        public void TestMethod_4()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Load_data_from_file("../../../../Life/test4.txt");
            Figures[] fig = Figures.Load_figures_from_file("../../../figures_tests.json");
            Figures block = fig[0];
            int count = Figures.Find_Figures(block, board);
            Assert.AreEqual(count, 1);
        }

        [TestMethod]
        public void TestMethod_5()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Load_data_from_file("../../../../Life/test5.txt");
            Figures[] fig = Figures.Load_figures_from_file("../../../figures_tests.json");
            Figures boat = fig[3];
            int count = Figures.Find_Figures(boat, board);
            Assert.AreEqual(count, 1);
        }
        [TestMethod]
        public void TestMethod_6()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Load_data_from_file("../../../../Life/test6.txt");
            Figures[] fig = Figures.Load_figures_from_file("../../../figures_tests.json");
            Figures tub = fig[2];
            int count = Figures.Find_Figures(tub, board);
            Assert.AreEqual(count, 1);
        }

        [TestMethod]
        public void TestMethod_7()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Load_data_from_file("../../../../Life/test7.txt");
            Figures[] fig = Figures.Load_figures_from_file("../../../figures_tests.json");
            Figures block = fig[0];
            int count = Figures.Find_Figures(block, board);
            Assert.AreEqual(count, 4);
        }
        [TestMethod]
        public void TestMethod_8()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Load_data_from_file("../../../../Life/test8.txt");
            Figures[] fig = Figures.Load_figures_from_file("../../../figures_tests.json");
            Figures block = fig[0];
            int count = Figures.Find_Figures(block, board);
            Assert.AreEqual(count, 0);
        }

        [TestMethod]
        public void TestMethod_9()
        {
            Board board = new Board(5, 5, 1, 1);
            board.Load_data_from_file("../../../../Life/test9.txt");
            Figures[] fig = Figures.Load_figures_from_file("../../../figures_tests.json");
            Figures ship = fig[4];
            int count = Figures.Find_Figures(ship, board);
            Assert.AreEqual(count, 1);
        }

        [TestMethod]
        public void TestMethod_10()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Load_data_from_file("../../../../Life/test10.txt");
            int count = board.Count_Alive_in_Cells();
            Assert.AreEqual(count, 16);
        }

        [TestMethod]
        public void TestMethod_11()
        {
            Board board = new Board(4, 4, 1, 1);
            board.Load_data_from_file("../../../../Life/test11.txt");
            int count = board.Count_Alive_in_Cells();
            Assert.AreEqual(count, 4);
        }

        [TestMethod]
        public void TestMethod_12()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Load_data_from_file("../../../../Life/test12.txt");
            bool flag = true;
            for (int i = 0; i < 2; i++)
            {
                if (!board.Positions.Contains(board.Note_Positions(board)))
                {
                    board.Positions.Add(board.Note_Positions(board));
                }
                else
                {
                    flag = false;
                }

                board.Advance();
            }
            Assert.AreEqual(flag, false);
        }


        [TestMethod]
        public void TestMethod_13()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Load_data_from_file("../../../../Life/test13.txt");
            int CountPositions = 0;
            bool flag = true;
            while (flag)
            {
                if (!board.Positions.Contains(board.Note_Positions(board)))
                {
                    board.Positions.Add(board.Note_Positions(board));
                }
                else
                {
                    CountPositions = board.Positions.Count;

                    flag = false;
                }
                board.Advance();
            }
            Assert.AreEqual(CountPositions, 1);
        }

        [TestMethod]
        public void TestMethod_14()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Load_data_from_file("../../../../Life/test14.txt");
            for (int i = 0; i < 2; i++)
            {
                board.Advance();
            }
            int count = board.Count_Alive_in_Cells();
            Assert.AreEqual(count, 0);
        }

        [TestMethod]
        public void TestMethod_15()
        {
            Board board = new Board(6, 6, 1, 1);
            board.Load_data_from_file("../../../../Life/test15.txt");
            int count = board.Count_Alive_in_Cells();
            Assert.AreEqual(count, 2);
        }
    }
}