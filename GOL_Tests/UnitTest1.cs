using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;

// КИТАЙСКИЕ ИЕРОГЛИФЫ ЧТОБЫ VISUAL STUDIO
// СОХРАНИЛО ФАЙЛ В КОДИРОВКЕ UTF-8 В 2022 ГОДУ
//
// Юникод появился в 1992 году.
//
// 党的耻辱

namespace GOL_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCellCounting()
        {
            // О том, что определённое поле из файла имеет нужное количество клеток

            Board board = Board.ReadFromFile("../../../test_boards/stable_test.txt");

            Assert.AreEqual(board.CountCells(), 40);
        }

        [TestMethod]
        public void TestPeriodicMatching()
        {
            // О распознавании каждого периода некоторых периодических фигур

            Board board = Board.ReadFromFile("../../../test_boards/periodic_test.txt");

            FigurePatternMap fpm = new FigurePatternMap(new FigureTypePatternFiles());

            for (int i = 0; i < 3; i++) {
                PatternMatchingResults results = new PatternMatchingResults();
                board.Match(fpm, out results);

                foreach (Figure_type ft in Enum.GetValues(typeof(Figure_type)))
                {
                    switch (ft)
                    {
                        case Figure_type.Pulsar:
                        case Figure_type.Beacon:
                            Assert.IsTrue(results[ft] == 1);
                            break;
                        default:
                            Assert.IsTrue(results[ft] == 0);
                            break;
                    }
                }
            }
        }

        [TestMethod]
        public void TestStableMatching()
        {
            // О распознавании каждого периода некоторых статических фигур

            Board board = Board.ReadFromFile("../../../test_boards/stable_test.txt");

            FigurePatternMap fpm = new FigurePatternMap(new FigureTypePatternFiles());

            for (int i = 0; i < 2; i++)
            {
                PatternMatchingResults results = new PatternMatchingResults();
                board.Match(fpm, out results);

                foreach (Figure_type ft in Enum.GetValues(typeof(Figure_type)))
                {
                    int needed = 0;

                    switch (ft)
                    {
                        case Figure_type.Hive:
                        case Figure_type.Blinker:
                            needed = 2;
                            break;
                        case Figure_type.Block:
                        case Figure_type.Loaf:
                        case Figure_type.Boat:
                        case Figure_type.Ship:
                            needed = 1;
                            break;
                        default:
                            break;
                    }

                    Assert.AreEqual(results[ft], needed);
                }
            }
        }

        [TestMethod]
        public void TestShipMatching()
        {
            // О распознавании каждого периода некоторых фигур-кораблей

            Board board = Board.ReadFromFile("../../../test_boards/ships_test.txt");

            FigurePatternMap fpm = new FigurePatternMap(new FigureTypePatternFiles());

            for (int i = 0; i < 2; i++)
            {
                PatternMatchingResults results = new PatternMatchingResults();
                board.Match(fpm, out results);

                foreach (Figure_type ft in Enum.GetValues(typeof(Figure_type)))
                {
                    int needed = 0;

                    switch (ft)
                    {
                        case Figure_type.LWSS:
                        case Figure_type.Glider:
                            needed = 1;
                            break;
                        default:
                            break;
                    }

                    Assert.AreEqual(results[ft], needed);
                }
            }
        }

        [TestMethod]
        public void TestGosperGunProducesGliders()
        {
            // О получении Glider в результате работы Gosper Gun

            Board board = Board.ReadFromFile("../../../test_boards/gospergun_test.txt");

            FigurePatternMap fpm = new FigurePatternMap(new FigureTypePatternFiles());

            for (int i = 0; i < 20; i++)
                board.Advance();

            {
                PatternMatchingResults results = new PatternMatchingResults();
                board.Match(fpm, out results);

                Assert.IsTrue(results[Figure_type.Glider] > 0);
            }
        }
    }
}
