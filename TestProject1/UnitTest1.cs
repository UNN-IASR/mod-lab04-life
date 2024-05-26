using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;
using System.IO;
using System.Text.Json;

namespace NET
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        // “ест провер€ет, что клетка с трем€ живыми сосед€ми оживает
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
        // “ест провер€ет, что клетка с двум€ живыми сосед€ми остаетс€ живой
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
        // “ест провер€ет, что клетка с одной живой соседкой умирает от одиночества
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
        // “ест провер€ет, что клетка с четырьм€ живыми сосед€ми умирает от перенаселени€
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
        // “ест провер€ет правильное обновление состо€ни€ клетки
        public void TestCellAdvanceState()
        {
            Cell cell = new Cell { IsAliveNext = true };
            cell.Advance();
            Assert.IsTrue(cell.IsAlive);
        }

        [TestMethod]
        // “ест провер€ет инициализацию доски с заданной плотностью живых клеток
        public void TestBoardInitialization()
        {
            Board board = new Board(10, 10, 1, 0.5);
            int liveCells = board.CountAliveCells();
            Assert.IsTrue(liveCells > 0 && liveCells < 100);
        }

        [TestMethod]
        // “ест провер€ет сохранение состо€ни€ доски
        public void TestSaveBoardState()
        {
            Board board = new Board(10, 10, 1, 0.5);
            string filePath = "test_state.json";
            board.SaveState(filePath);
            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod]
        // “ест провер€ет загрузку состо€ни€ доски
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
        // “ест провер€ет, что доска корректно определ€ет стабильное состо€ние
        public void TestBoardStability()
        {
            Board board = new Board(10, 10, 1, 0.5);
            board.Advance();
            board.Advance();
            Assert.IsTrue(board.IsStable());
        }

        [TestMethod]
        // “ест провер€ет классификацию элементов доски
        public void TestBoardClassification()
        {
            Board board = new Board(10, 10, 1, 0.5);
            var classifications = board.ClassifyElements();
            Assert.IsTrue(classifications.Count > 0);
        }

        [TestMethod]
        // “ест провер€ет метод подсчета живых клеток
        public void TestCountAliveCells()
        {
            Board board = new Board(10, 10, 1, 0.5);
            int liveCells = board.CountAliveCells();
            Assert.IsTrue(liveCells > 0 && liveCells < 100);
        }

        [TestMethod]
        // “ест провер€ет метод загрузки паттерна
        public void TestLoadPattern()
        {
            Board board = new Board(10, 10, 1, 0.0);
            string pattern = "1\n0\n1\n0\n1\n0\n1\n0\n1\n0\n";
            File.WriteAllText("test_pattern.txt", pattern);
            board.LoadPattern("test_pattern.txt");
            Assert.AreEqual(5, board.CountAliveCells());
        }

        [TestMethod]
        // “ест провер€ет создание стандартных паттернов
        public void TestStandardPatterns()
        {
            var patterns = Pattern.GetStandardPatterns();
            Assert.IsTrue(patterns.Count > 0);
        }

        [TestMethod]
        // “ест провер€ет правильное подключение соседей клеток
        public void TestCellNeighbors()
        {
            Board board = new Board(10, 10, 1, 0.5);
            Cell cell = board.Cells[1, 1];
            Assert.AreEqual(8, cell.neighbors.Count);
        }

        [TestMethod]
        // “ест провер€ет, что клетка остаЄтс€ мЄртвой при отсутствии живых соседей
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
        // “ест провер€ет, что клетка остаЄтс€ живой при трЄх живых сосед€х
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
        // “ест провер€ет, что доска правильно рандомизирует состо€ние клеток
        public void TestBoardRandomization()
        {
            Board board = new Board(10, 10, 1, 0.5);
            board.Randomize(0.7);
            int liveCells = board.CountAliveCells();
            Assert.IsTrue(liveCells > 50 && liveCells < 100);
        }

        [TestMethod]
        // “ест провер€ет правильное сохранение и загрузку конфигурации
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
        // “ест провер€ет, что система достигает стабильного состо€ни€ за разумное количество поколений
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
        // “ест провер€ет, что система правильно классифицирует элементы после нескольких поколений
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
