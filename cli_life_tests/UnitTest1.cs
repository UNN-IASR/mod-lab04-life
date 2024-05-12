using System;
using System.Collections.Generic;
using NUnit.Framework;
using cli_life;

namespace cli_life_tests.Tests
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void CellStateChangeTest()
        {
            // Подготовка
            var cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });

            // Действие
            cell.DetermineNextLiveState();
            cell.Advance();

            // Проверка
            Assert.That(cell.IsAlive, Is.False);
        }

        [Test]
        public void RandomizeBoardTest()
        {
            // Подготовка
            var board = new Board(10, 10, 1, 0.5);

            // Действие
            int liveCells = 0;
            foreach (var cell in board.Cells)
            {
                if (cell.IsAlive) liveCells++;
            }

            // Проверка
            Assert.That(liveCells, Is.GreaterThan(0));
        }

        [Test]
        public void PatternPlacementTest()
        {
            // Подготовка
            var board = new Board(10, 10, 1);
            int[,] pattern = new int[,] { { 1, 0 }, { 0, 1 } };

            // Действие
            board.PlacePattern(pattern, 0, 0);

            // Проверка
            Assert.That(board.Cells[0, 0].IsAlive, Is.True);
            Assert.That(board.Cells[1, 1].IsAlive, Is.True);
        }

        [Test]
        public void MatchPatternTest()
        {
            // Подготовка
            var board = new Board(10, 10, 1);
            int[,] pattern = new int[,] { { 1, 0 }, { 0, 1 } };
            board.PlacePattern(pattern, 0, 0);

            // Действие и проверка
            Assert.That(board.MatchPattern(pattern, 0, 0), Is.True);
        }

        [Test]
        public void AdvanceBoardTest()
        {
            // Подготовка
            var board = new Board(10, 10, 1);
            board.Randomize(0.5);

            // Действие
            board.Advance();

            // Проверка
            Assert.Pass();
        }

        [Test]
        public void StableBoardTest()
        {
            // Подготовка
            var board = new Board(10, 10, 1);
            int[,] pattern = new int[,] { { 1, 1 }, { 1, 1 } };
            board.PlacePattern(pattern, 0, 0);

            // Действие
            board.Advance();

            // Проверка
            Assert.That(board.IsStable(), Is.True);
        }

        [Test]
        public void ClassifyPatternsTest()
        {
            // Подготовка
            var board = new Board(10, 10, 1);
            int[,] pattern = new int[,] { { 1, 1 }, { 1, 1 } };
            board.PlacePattern(pattern, 0, 0);
            var patterns = new List<Tuple<string, int[,]>> { new Tuple<string, int[,]>("block", pattern) };

            // Действие
            var result = board.ClassifyPatterns(patterns);

            // Проверка
            Assert.That(result["block"], Is.EqualTo(1));
        }

        [Test]
        public void TotalCellsTest()
        {
            // Подготовка
            var board = new Board(10, 10, 1);

            // Действие и проверка
            Assert.That(board.GetTotalCells(), Is.EqualTo(100));
        }

        [Test]
        public void TotalCombinationsTest()
        {
            // Подготовка
            var board = new Board(10, 10, 1);

            // Действие и проверка
            Assert.That(board.GetTotalCombinations(), Is.EqualTo(Math.Pow(2, 100)));
        }

        [Test]
        public void ClearHistoryTest()
        {
            // Подготовка
            var board = new Board(10, 10, 1);
            int[,] pattern = new int[,] { { 1, 1 }, { 1, 1 } };
            board.PlacePattern(pattern, 0, 0);
            board.Advance();

            // Действие
            board.ClearHistory();

            // Проверка
            Assert.That(board.IsStable(), Is.False);
        }

        [Test]
        public void GetBoardStateTest()
        {
            // Подготовка
            var board = new Board(4, 4, 1);
            int[,] pattern = new int[,] { { 1, 0 }, { 0, 1 } };
            board.PlacePattern(pattern, 0, 0);

            // Ожидаемое состояние
            string expectedState = "1000010000000000";

            // Проверка
            Assert.That(board.GetBoardState(), Is.EqualTo(expectedState));
        }

        [Test]
        public void LoadConfigTest()
        {
            // Подготовка и действие
            var config = Program.LoadConfig<Config>("config.json");

            // Проверка
            Assert.That(config, Is.Not.Null);
        }

        [Test]
        public void SaveAndLoadStateTest()
        {
            // Подготовка
            var board = new Board(4, 4, 1);
            int[,] pattern = new int[,] { { 1, 0 }, { 0, 1 } };
            board.PlacePattern(pattern, 0, 0);
            
            // Сохранение состояния
            Program.SaveState("test_state.json");

            // Загрузка состояния
            Program.LoadState("test_state.json");

            // Проверка
            Assert.That(board.MatchPattern(pattern, 0, 0), Is.True);
        }

        [Test]
        public void LoadPatternTest()
        {
            // Подготовка
            var board = new Board(4, 4, 1);
            
            // Загрузка паттерна
            Program.LoadPattern("test_pattern.json");
            
            // Ожидаемый паттерн
            int[,] expectedPattern = new int[,] { { 1, 0 }, { 0, 1 } };

            // Проверка
            Assert.That(board.MatchPattern(expectedPattern, 0, 0), Is.True);
        }

        [Test]
        public void GatherDataTest()
        {
            // Подготовка и действие
            var data = Program.GatherData(10, 10, 1, new List<double> { 0.1, 0.2 }, 1);

            // Проверка
            Assert.That(data, Is.Not.Empty);
        }
    }
}
