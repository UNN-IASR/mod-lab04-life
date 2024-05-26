using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;

namespace NET
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        // Тест проверяет, что клетка с тремя живыми соседями оживает
        public void TestCellRevivesWithThreeNeighbors()
        {
            Cell cell = new Cell();
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        // Тест проверяет, что клетка с двумя живыми соседями остается живой
        public void TestCellStaysAliveWithTwoNeighbors()
        {
            Cell cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        // Тест проверяет, что клетка с одной живой соседкой умирает от одиночества
        public void TestCellDiesWithOneNeighbor()
        {
            Cell cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            Assert.IsFalse(cell.IsAliveNext);
        }

        [TestMethod]
        // Тест проверяет, что клетка с четырьмя живыми соседями умирает от перенаселения
        public void TestCellDiesWithFourNeighbors()
        {
            Cell cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            Assert.IsFalse(cell.IsAliveNext);
        }

        [TestMethod]
        // Тест проверяет правильное обновление состояния клетки
        public void TestCellAdvanceState()
        {
            Cell cell = new Cell { IsAliveNext = true };
            cell.Advance();
            Assert.IsTrue(cell.IsAlive);
        }

        [TestMethod]
        // Тест проверяет инициализацию доски с заданной плотностью живых клеток
        public void TestBoardInitialization()
        {
            Board board = new Board(10, 10, 1, 0.5);
            int liveCells = board.CountAliveCells();
            Assert.IsTrue(liveCells > 0 && liveCells < 100);
        }

        [TestMethod]
        // Тест проверяет сохранение состояния доски
        public void TestSaveBoardState()
        {
            Board board = new Board(10, 10, 1, 0.5);
            string filePath = "test_state.json";
            board.SaveState(filePath);
            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod]
        // Тест проверяет загрузку состояния доски
        public void TestLoadBoardState()
        {
            string filePath = "test_state.json";
            Board board = new Board(10, 10, 1, 0.5);
            board.SaveState(filePath);

            Board newBoard = new Board(10, 10, 1, 0.0);
            newBoard.LoadState(filePath);
            Assert.AreEqual(board.CountAliveCells(), newBoard.CountAliveCells());
        }

        [TestMethod]
        // Тест проверяет, что доска корректно определяет стабильное состояние
        public void TestBoardStability()
        {
            Board board = new Board(10, 10, 1, 0.5);
            board.Advance();
            board.Advance();
            Assert.IsTrue(board.IsStable());
        }

        [TestMethod]
        // Тест проверяет классификацию элементов доски
        public void TestBoardClassification()
        {
            Board board = new Board(10, 10, 1, 0.5);
            var classifications = board.ClassifyElements();
            Assert.IsTrue(classifications.Count > 0);
        }

        [TestMethod]
        // Тест проверяет метод подсчета живых клеток
        public void TestCountAliveCells()
        {
            Board board = new Board(10, 10, 1, 0.5);
            int liveCells = board.CountAliveCells();
            Assert.IsTrue(liveCells > 0 && liveCells < 100);
        }

        [TestMethod]
        // Тест проверяет метод загрузки паттерна
        public void TestLoadPattern()
        {
            Board board = new Board(10, 10, 1, 0.0);
            string pattern = "1\n0\n1\n0\n1\n0\n1\n0\n1\n0\n";
            File.WriteAllText("test_pattern.txt", pattern);
            board.LoadPattern("test_pattern.txt");
            Assert.AreEqual(5, board.CountAliveCells());
        }

        [TestMethod]
        // Тест проверяет создание стандартных паттернов
        public void TestStandardPatterns()
        {
            var patterns = Pattern.GetStandardPatterns();
            Assert.IsTrue(patterns.Count > 0);
        }

        [TestMethod]
        // Тест проверяет правильное подключение соседей клеток
        public void TestCellNeighbors()
        {
            Board board = new Board(10, 10, 1, 0.5);
            Cell cell = board.Cells[1, 1];
            Assert.AreEqual(8, cell.neighbors.Count);
        }

        [TestMethod]
        // Тест проверяет, что клетка остаётся мёртвой при отсутствии живых соседей
        public void TestCellStaysDead()
        {
            Cell cell = new Cell();
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            Assert.IsFalse(cell.IsAliveNext);
        }

        [TestMethod]
        // Тест проверяет, что клетка остаётся живой при трёх живых соседях
        public void TestCellStaysAliveWithThreeNeighbors()
        {
            Cell cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.neighbors.Add(new Cell { IsAlive = false });

            cell.DetermineNextLiveState();
            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        // Тест проверяет, что доска правильно рандомизирует состояние клеток
        public void TestBoardRandomization()
        {
            Board board = new Board(10, 10, 1, 0.5);
            board.Randomize(0.7);
            int liveCells = board.CountAliveCells();
            Assert.IsTrue(liveCells > 50 && liveCells < 100);
        }

        [TestMethod]
        // Тест проверяет правильное сохранение и загрузку конфигурации
        public void TestSaveLoadConfig()
        {
            Config config = new Config { Width = 50, Height = 50, CellSize = 1, LiveDensity = 0.5 };
            string filePath = "test_config.json";
            File.WriteAllText(filePath, JsonSerializer.Serialize(config));
            Config loadedConfig = Config.Load(filePath);
            Assert.AreEqual(config.Width, loadedConfig.Width);
            Assert.AreEqual(config.Height, loadedConfig.Height);
            Assert.AreEqual(config.CellSize, loadedConfig.CellSize);
            Assert.AreEqual(config.LiveDensity, loadedConfig.LiveDensity);
        }

        [TestMethod]
        // Тест проверяет, что система достигает стабильного состояния за разумное количество поколений
        public void TestStabilityAfterGenerations()
        {
            Board board = new Board(10, 10, 1, 0.5);
            int generations = 0;
            while (!board.IsStable() && generations < 100)
            {
                board.Advance();
                generations++;
            }
            Assert.IsTrue(generations < 100);
        }

        [TestMethod]
        // Тест проверяет, что система правильно классифицирует элементы после нескольких поколений
        public void TestClassificationAfterGenerations()
        {
            Board board = new Board(10, 10, 1, 0.5);
            for (int i = 0; i < 10; i++)
            {
                board.Advance();
            }
            var classifications = board.ClassifyElements();
            Assert.IsTrue(classifications.Count > 0);
        }
    }
}
