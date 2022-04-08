using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;
using System.Collections.Generic;
using System;

namespace Life.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string filename = "firstTest.json";
            Board board = Program.Reset(filename);
            Assert.IsTrue((board.Columns == 20 && board.Rows == 30) == true);
        }
        [TestMethod]
        public void TestMethod2()
        {
            Board board = Board.ReadFromFile("secondAndThirdTest.txt");
            Board boardForComparison = new Board(5, 5, 1);
            for(int i = 0; i < boardForComparison.Rows; i++)
            {
                for(int j = 0; j < boardForComparison.Columns; j++)
                {
                    if((i == 0 && j == 0) || (i == 1 && j == 2) || (i == 2 && j == 1) || (i == 2 && j == 3) || (i == 4 && j == 4) || (i == 3 && j == 2))
                    {
                        boardForComparison.Cells[i, j].IsAlive = true;
                    }
                    else
                    {
                        boardForComparison.Cells[i, j].IsAlive = false;
                    }
                }
            }
            Assert.IsTrue(board.cellsMatch(boardForComparison));
        }
        [TestMethod]
        public void TestMethod3()
        {
            Board board = Board.ReadFromFile("secondAndThirdTest.txt");
            Assert.IsTrue(board.getCountOfAliveCells() == 6);
        }
        [TestMethod]
        public void TestMethod4()
        {
            Board board = Board.ReadFromFile("recognizeEmpty.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 0);
        }
        [TestMethod]
        public void TestMethod5()
        {
            Board board = Board.ReadFromFile("recognizeBlock.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 1 && resultOfPatternRecognation.ContainsKey(Figure.Block));
        }
        [TestMethod]
        public void TestMethod6()
        {
            Board board = Board.ReadFromFile("recognizeTub.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 1 && resultOfPatternRecognation.ContainsKey(Figure.Tub));
        }
        [TestMethod]
        public void TestMethod7()
        {
            Board board = Board.ReadFromFile("recognizeBeehive.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 2 && resultOfPatternRecognation.ContainsKey(Figure.Beehive1) 
                && resultOfPatternRecognation.ContainsKey(Figure.Beehive2));
        }
        [TestMethod]
        public void TestMethod8()
        {
            Board board = Board.ReadFromFile("recognizePond.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 1 && resultOfPatternRecognation.ContainsKey(Figure.Pond));
        }
        [TestMethod]
        public void TestMethod9()
        {
            Board board = Board.ReadFromFile("recognizeShip.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 2 && resultOfPatternRecognation.ContainsKey(Figure.Ship1)
                && resultOfPatternRecognation.ContainsKey(Figure.Ship2));
        }
        [TestMethod]
        public void TestMethod10()
        {
            Board board = Board.ReadFromFile("recognizeLoaf.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 4 && resultOfPatternRecognation.ContainsKey(Figure.Loaf1)
                && resultOfPatternRecognation.ContainsKey(Figure.Loaf2) && resultOfPatternRecognation.ContainsKey(Figure.Loaf3)
                && resultOfPatternRecognation.ContainsKey(Figure.Loaf4));
        }
        [TestMethod]
        public void TestMethod11()
        {
            Board board = Board.ReadFromFile("recognizeBoat.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 4 && resultOfPatternRecognation.ContainsKey(Figure.Boat1)
                && resultOfPatternRecognation.ContainsKey(Figure.Boat2) && resultOfPatternRecognation.ContainsKey(Figure.Boat3)
                && resultOfPatternRecognation.ContainsKey(Figure.Boat4));
        }
        [TestMethod]
        public void TestMethod12()
        {
            Board board = Board.ReadFromFile("recognizeBlinker.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 2 && resultOfPatternRecognation.ContainsKey(Figure.Blinker1)
                && resultOfPatternRecognation.ContainsKey(Figure.Blinker2));
        }
        [TestMethod]
        public void TestMethod13()
        {
            Board board = Board.ReadFromFile("recognizeEight.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 2 && resultOfPatternRecognation.ContainsKey(Figure.Eight1)
                && resultOfPatternRecognation.ContainsKey(Figure.Eight2));
        }
        [TestMethod]
        public void TestMethod14()
        {
            Board board = Board.ReadFromFile("recognizeEverything.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 4 && resultOfPatternRecognation.ContainsKey(Figure.Block)
                && resultOfPatternRecognation.ContainsKey(Figure.Beehive1) && resultOfPatternRecognation.ContainsKey(Figure.Beehive2)
                && resultOfPatternRecognation.ContainsKey(Figure.Tub));
        }
        [TestMethod]
        public void TestMethod15()
        {
            Board board = Board.ReadFromFile("recognizeEverything.txt");
            Dictionary<Figure, List<Point>> resultOfPatternRecognation = board.recognizePatternsOnBoard(new PatternMap());
            Assert.IsTrue(resultOfPatternRecognation.Count == 4 && resultOfPatternRecognation.ContainsKey(Figure.Block)
                && resultOfPatternRecognation.ContainsKey(Figure.Beehive1) && resultOfPatternRecognation.ContainsKey(Figure.Beehive2)
                && resultOfPatternRecognation.ContainsKey(Figure.Tub) 
                && Board.countNumberSymmetricalFigures(resultOfPatternRecognation, new PatternMap()) == 5);
        }
        [TestMethod]
        public void TestMethod16()
        {
            Board board = Board.ReadFromFile("recognizeEverything.txt");
            string filename = "writeBoard.txt";
            Board.WriteToFile(filename, board);
            Board boardAfterWritting = Board.ReadFromFile(filename);
            Assert.IsTrue(board.cellsMatch(boardAfterWritting));
        }
    }
}
