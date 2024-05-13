using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cli_life;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LifeTest
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void TestDetermineNextLiveState_AliveCell_TwoLiveNeighbors_StaysAlive()
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
            Assert.IsTrue(cell.IsAlive);
        }


        [TestMethod]
        public void TestAdvance_CellStateChangesCorrectly()
        {
            Cell cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.DetermineNextLiveState();
            cell.Advance();

            Assert.IsTrue(cell.IsAlive);
        }

        [TestMethod]
        public void TestConnectNeighbors_AllNeighborsConnected()
        {
            Board board = new Board(width: 10, height: 10, cellSize: 1);
            foreach (var cell in board.Cells)
            {
                Assert.AreEqual(8, cell.neighbors.Count);
            }
        }

        [TestMethod]
        public void TestRandomize_CellsRandomlyAlive()
        {
            Board board = new Board(width: 10, height: 10, cellSize: 1, liveDensity: 0.5);
            Assert.IsTrue(board.Cells.Cast<Cell>().Any(c => c.IsAlive));
            Assert.IsTrue(board.Cells.Cast<Cell>().Any(c => !c.IsAlive));
        }

        [TestMethod]
        public void TestLoadFigure_FigureLoadedCorrectly()
        {
            Board board = new Board(width: 10, height: 10, cellSize: 1);
            board.LoadFigure("figures/Glider.json");

            int centerX = board.Columns / 2;
            int centerY = board.Rows / 2;

            Assert.IsTrue(board.Cells[centerX - 1, centerY - 1].IsAlive);
            Assert.IsTrue(board.Cells[centerX + 1, centerY].IsAlive);
            Assert.IsTrue(board.Cells[centerX, centerY + 1].IsAlive);

        [TestMethod]
        public void TestSaveState_LoadState()
        {
            Board board = new Board(width: 10, height: 10, cellSize: 1);
            board.Cells[2, 2].IsAlive = true;
            board.Cells[2, 3].IsAlive = true;
            board.Cells[3, 2].IsAlive = true;

            board.SaveState("test_state.txt");

            Board newBoard = new Board(width: 10, height: 10, cellSize: 1);
            newBoard.LoadState("test_state.txt");

            Assert.IsTrue(newBoard.Cells[2, 2].IsAlive);
            Assert.IsTrue(newBoard.Cells[2, 3].IsAlive);
            Assert.IsTrue(newBoard.Cells[3, 2].IsAlive);
        }
    }
}