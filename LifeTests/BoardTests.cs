using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;
using System;
using System.Collections.Generic;
using System.Text;

namespace cli_life.Tests
{
    [TestClass()]
    public class BoardTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);

            int col = board.Columns;

            Assert.IsTrue(col == 50);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test1.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            Assert.IsTrue(counter == 5);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            int aliveCells = board.GetAliveCells();

            Assert.IsTrue(aliveCells == 92);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            int deadCells = board.GetDeadCells();

            Assert.IsTrue(deadCells == 1908);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            double dens = board.GetDensity();

            Assert.IsTrue(dens == 0.046);
        }

        [TestMethod]
        public void TestMethod6()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            bool symX = board.IsXSymmetrical();

            Assert.IsTrue(!symX);
        }

        [TestMethod]
        public void TestMethod7()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            bool symY = board.IsYSymmetrical();

            Assert.IsTrue(!symY);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            int blocks = board.GetNumberOfBlocks();

            Assert.IsTrue(blocks == 1);
        }

        [TestMethod]
        public void TestMethod9()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            int hives = board.GetNumberOfHives();

            Assert.IsTrue(hives == 1);
        }

        [TestMethod]
        public void TestMethod10()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            int boxes = board.GetNumberOfBoxes();

            Assert.IsTrue(boxes == 1);
        }

        [TestMethod]
        public void TestMethod11()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            int ships = board.GetNumberOfShips();

            Assert.IsTrue(ships == 2);
        }

        [TestMethod]
        public void TestMethod12()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            int boats = board.GetNumberOfBoats();

            Assert.IsTrue(boats == 0);
        }

        [TestMethod]
        public void TestMethod13()
        {
            Parameters parameters = new Parameters();
            parameters.LoadParameters(@"Parameters.json");
            Board board = new Board(parameters);
            board.LoadFromFile("Colonies/test2.txt");

            int counter = 0;

            while (board.ToContinue(counter))
            {
                counter++;
                board.Advance();
            }

            int symFigures = board.GetSymmetricalFigures();

            Assert.IsTrue(symFigures == 3);
        }
    }
}
