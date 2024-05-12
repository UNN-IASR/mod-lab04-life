using Xunit;
using cli_life;
using System.IO;
using System;
using System.Linq;

namespace Life.Tests
{
    public class GameOfLifeTests
    {
        // Тест 1: Проверка, что клетка с двумя живыми соседями остается живой
        [Fact]
        public void Cell_DetermineNextLiveState_AliveWithTwoNeighbors_StaysAlive()
        {
            Cell cell = new Cell { IsAlive = true };
            AddNeighbors(cell, 2, true);
            AddNeighbors(cell, 6, false);
            cell.DetermineNextLiveState();
            Assert.True(cell.IsAlive);
        }

        // Тест 2: Проверка, что мертвая клетка с тремя живыми соседями оживает
        [Fact]
        public void Cell_DetermineNextLiveState_DeadWithThreeNeighbors_BecomesAlive()
        {
            Cell cell = new Cell { IsAlive = false };
            AddNeighbors(cell, 3, true);
            AddNeighbors(cell, 5, false);
            cell.DetermineNextLiveState();
            Assert.True(cell.IsAlive);
        }

        // Тест 3: Проверка, что живая клетка с одним соседом умирает
        [Fact]
        public void Cell_DetermineNextLiveState_AliveWithOneNeighbor_Dies()
        {
            Cell cell = new Cell { IsAlive = true };
            AddNeighbors(cell, 1, true);
            AddNeighbors(cell, 7, false);
            cell.DetermineNextLiveState();
            Assert.False(cell.IsAlive);
        }

        // Тест 4: Проверка корректной инициализации количества столбцов в Board
        [Fact]
        public void Board_Initialization_InitializesCorrectColumns()
        {
            int width = 10;
            int height = 10;
            int cellSize = 1;
            double liveDensity = 0.5;
            int expectedColumns = width / cellSize;
            var board = new Board(width, height, cellSize, liveDensity);
            Assert.Equal(expectedColumns, board.Columns);
        }

        // Тест 5: Проверка корректной инициализации количества строк в Board
        [Fact]
        public void Board_Initialization_InitializesCorrectRows()
        {
            int width = 10;
            int height = 10;
            int cellSize = 1;
            double liveDensity = 0.5;
            int expectedRows = height / cellSize;
            var board = new Board(width, height, cellSize, liveDensity);
            Assert.Equal(expectedRows, board.Rows);
        }

        // Тест 6: Проверка корректного применения правил игры в Board.Advance()
        [Fact]
        public void Board_Advance_AppliesRulesCorrectly()
        {
            Board board = new Board(width: 3, height: 3, cellSize: 1, liveDensity: 0);
            board.Cells[1, 1].IsAlive = true;
            board.Cells[1, 2].IsAlive = true;
            board.Cells[2, 1].IsAlive = true;

            board.Advance();

            Assert.True(board.Cells[1, 1].IsAlive);
            Assert.True(board.Cells[2, 2].IsAlive);
            Assert.False(board.Cells[1, 2].IsAlive);
        }

        // Тест 7: Проверка, что SaveState создает файл
        [Fact]
        public void Program_SaveState_CreatesFile()
        {
            string filename = "test.txt";
            Program.board = new Board(width: 5, height: 5, cellSize: 1, liveDensity: 0);
            Program.SaveState(filename);
            Assert.True(File.Exists(filename));
            File.Delete(filename);
        }

        // Тест 8: Проверка, что LoadState загружает правильное состояние
        [Fact]
        public void Program_LoadState_LoadsCorrectState()
        {
            string filename = "test.txt";
            string fileContent = ".....\n.*...\n..*..\n.***.\n.....\n";
            File.WriteAllText(filename, fileContent);
            Program.board = new Board(width: 5, height: 5, cellSize: 1, liveDensity: 0);
            Program.LoadState(filename);
            Assert.True(Program.board.Cells[1, 1].IsAlive);
            Assert.True(Program.board.Cells[2, 2].IsAlive);
            Assert.True(Program.board.Cells[1, 3].IsAlive);
            Assert.True(Program.board.Cells[2, 3].IsAlive);
            Assert.True(Program.board.Cells[3, 3].IsAlive);
            File.Delete(filename);
        }

        // Тест 9: Проверка, что CheckStability определяет стабильное состояние
        [Fact]
        public void Board_CheckStability_DetectsStableState()
        {
            Board board = new Board(width: 3, height: 3, cellSize: 1, liveDensity: 0);
            board.Cells[1, 1].IsAlive = true;
            board.CheckStability();
            board.Advance();
            board.CheckStability();
            Assert.True(board.IsStable);
        }

        // Тест 10: Проверка, что функция ComparePattern работает корректно
        [Fact]
        public void Program_ComparePattern_ReturnsTrueForMatchingPattern()
        {
            Program.board = new Board(width: 5, height: 5, cellSize: 1, liveDensity: 0);
            Program.board.Cells[1, 1].IsAlive = true;
            Program.board.Cells[1, 2].IsAlive = true;
            Program.board.Cells[2, 1].IsAlive = true;
            Program.board.Cells[2, 2].IsAlive = true;

            bool[,] blockPattern = new bool[,] { { true, true }, { true, true } };
            bool isMatch = Program.ComparePattern(1, 1, blockPattern);

            Assert.True(isMatch);
        }

        // Вспомогательная функция для добавления соседей к клетке
        private void AddNeighbors(Cell cell, int count, bool isAlive)
        {
            for (int i = 0; i < count; i++)
            {
                cell.neighbors.Add(new Cell { IsAlive = isAlive });
            }
        }
    }
}