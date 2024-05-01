using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Linq;
using cli_life;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Text;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Cell cell = new Cell();
            Assert.IsFalse(cell.IsAlive);

            cell.IsAlive = true;
            Assert.IsTrue(cell.IsAlive);
        }
        [TestMethod]
        public void TestMethod2()
        {
            int width = 5;
            int height = 5;
            int cellSize = 1;

            int[,] i1 = new int[,]
            {
                { 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1 }
            };

            int[,] i2 = new int[,]
            {
                { 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1 }
            };

            Board board1 = new Board(width, height, cellSize, i1);
            Board board2 = new Board(width, height, cellSize, i2);

            bool isEqual = board1.Sovpadenie(board2);

            Assert.IsTrue(isEqual);
        }
        [TestMethod]
        public void TestMethod3()
        {
            Cell cell = new Cell();
            cell.IsAlive = true;
            cell.neighbors = new List<Cell>
            {
                new Cell { IsAlive = false },
                new Cell { IsAlive = true },
                new Cell { IsAlive = true },
                new Cell { IsAlive = false },
                new Cell { IsAlive = false },
                new Cell { IsAlive = false },
                new Cell { IsAlive = false },
                new Cell { IsAlive = false }
            };
            cell.DetermineNextLiveState();
            Assert.IsTrue(cell.IsAliveNext);
        }
        [TestMethod]
        public void TestMethod4()
        {
            int width = 5;
            int height = 5;
            int cellSize = 1;

            int[,] i = new int[,]
            {
                { 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1 }
            };

            Board board = new Board(width, height, cellSize, i);

            Board clonedBoard = board.Clone();

            Assert.AreEqual(board.Cells.Length, clonedBoard.Cells.Length);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Assert.AreEqual(board.Cells[x, y].IsAlive, clonedBoard.Cells[x, y].IsAlive);
                }
            }
        }
        [TestMethod]
        public void TestMethod5()
        {
            setting setting = new setting
            {
                width = 10,
                height = 10,
                cellSize = 1,
                liveDensity = 0.5
            };

            string json = JsonConvert.SerializeObject(setting);

            setting deserializedSetting = JsonConvert.DeserializeObject<setting>(json);

            Assert.AreEqual(setting.width, deserializedSetting.width);
            Assert.AreEqual(setting.height, deserializedSetting.height);
            Assert.AreEqual(setting.cellSize, deserializedSetting.cellSize);
            Assert.AreEqual(setting.liveDensity, deserializedSetting.liveDensity);
        }
        [TestMethod]
        public void TestMethod6()
        {
            string filePath = "C:\\Users\\ASUS\\OneDrive\\Рабочий стол\\settings.json";

            setting setting = new setting
            {
                width = 10,
                height = 10,
                cellSize = 1,
                liveDensity = 0.5
            };

            string json = JsonConvert.SerializeObject(setting);

            File.WriteAllText(filePath, json);

            Board board = new Board(setting.width, setting.height, setting.cellSize, setting.liveDensity);

            Assert.AreEqual(setting.width, board.Width);
            Assert.AreEqual(setting.height, board.Height);
            Assert.AreEqual(setting.cellSize, board.CellSize);
        }
        [TestMethod]
        public void TestMethod7()
        {
            int width = 5;
            int height = 5;
            int cellSize = 1;

            int[,] i = new int[,]
            {
                { '*', '*', 0, 0, 0 },
                { '*', '*', 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
            };

            Board board = new Board(width, height, cellSize, i);

            bool isStabilnoe = board.StabilnoeSostoyanie();

            Assert.IsTrue(isStabilnoe);
        }
        [TestMethod]
        public void TestMethod8()
        {
            int width = 5;
            int height = 5;
            int cellSize = 1;

            int[,] i = new int[,]
            {
                { '*', '*', 0, 0, 0 },
                { '*', '*', 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
            };

            Board board1 = new Board(width, height, cellSize, i);

            int blockCount = 0;
            int prudCount = 0;
            int bargeCount = 0;
            int zmeyaCount = 0;

            for (int x = 0; x < board1.Columns; x++)
            {
                for (int y = 0; y < board1.Rows; y++)
                {
                    if (board1.Cells[x, y].IsAlive)
                    {
                        if (x < board1.Columns - 1 && y < board1.Rows - 1 &&
                            board1.Cells[x, y].IsAlive && board1.Cells[x + 1, y].IsAlive &&
                            board1.Cells[x, y + 1].IsAlive && board1.Cells[x + 1, y + 1].IsAlive)
                        {
                            blockCount++;
                        }
                    }
                }
            }
            for (int x = 1; x < board1.Columns - 2; x++)
            {
                for (int y = 0; y < board1.Rows - 3; y++)
                {
                    if (board1.Cells[x, y].IsAlive && board1.Cells[x + 1, y].IsAlive && board1.Cells[x - 1, y + 1].IsAlive &&
                        !board1.Cells[x, y + 1].IsAlive && !board1.Cells[x + 1, y + 1].IsAlive && board1.Cells[x + 2, y + 1].IsAlive
                        && board1.Cells[x - 1, y + 2].IsAlive && board1.Cells[x + 2, y + 2].IsAlive &&
                        board1.Cells[x, y + 3].IsAlive && board1.Cells[x + 1, y + 3].IsAlive)
                    {
                        prudCount++;
                    }
                }
            }
            for (int x = 0; x < board1.Columns - 3; x++)
            {
                for (int y = 0; y < board1.Rows - 3; y++)
                {
                    if (!board1.Cells[x, y].IsAlive && board1.Cells[x + 1, y].IsAlive && !board1.Cells[x + 2, y].IsAlive && !board1.Cells[x + 3, y].IsAlive &&
                        board1.Cells[x, y + 1].IsAlive && !board1.Cells[x + 1, y + 1].IsAlive && board1.Cells[x + 2, y + 1].IsAlive && !board1.Cells[x + 3, y + 1].IsAlive &&
                        !board1.Cells[x, y + 2].IsAlive && board1.Cells[x + 1, y + 2].IsAlive && !board1.Cells[x + 2, y + 2].IsAlive && board1.Cells[x + 3, y + 2].IsAlive
                        && !board1.Cells[x, y + 3].IsAlive && !board1.Cells[x + 1, y + 3].IsAlive && board1.Cells[x + 2, y + 3].IsAlive && !board1.Cells[x + 3, y + 3].IsAlive)
                    {
                        bargeCount++;
                    }
                }
            }
            for (int x = 0; x < board1.Columns - 3; x++)
            {
                for (int y = 0; y < board1.Rows - 1; y++)
                {
                    if (board1.Cells[x, y].IsAlive &&
                        !board1.Cells[x + 1, y].IsAlive && board1.Cells[x + 2, y].IsAlive && board1.Cells[x + 3, y].IsAlive &&
                        board1.Cells[x, y + 1].IsAlive && board1.Cells[x + 1, y + 1].IsAlive &&
                        !board1.Cells[x + 2, y + 1].IsAlive && board1.Cells[x + 3, y + 1].IsAlive)
                    {
                        zmeyaCount++;
                    }
                }
            }


            int expectedBlockCount = 1;
            int expectedPrudCount = 0;
            int expectedBargeCount = 0;
            int expectedZmeyaCount = 0;

            Assert.AreEqual(expectedBlockCount, blockCount);
            Assert.AreEqual(expectedPrudCount, prudCount);
            Assert.AreEqual(expectedBargeCount, bargeCount);
            Assert.AreEqual(expectedZmeyaCount, zmeyaCount);
        }
        [TestMethod]
        public void TestMethod9()
        {
            int width = 5;
            int height = 5;
            int cellSize = 1;

            int[,] i = new int[,]
            {
                { '*', '*', 0, 0, 0 },
                { '*', '*', 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
            };

            Board board = new Board(width, height, cellSize, i);
            board.ClassificiruemElements();
            int liveCellCount = 0;
            int combinationCount = 0;

            foreach (var cell in board.Cells)
            {
                if (cell.IsAlive == true)
                {
                    liveCellCount++;
                }
            }

            //считаем комбинации: 

            //комбинация 1 -  блок
            for (int x = 0; x < board.Columns; x++)
            {
                for (int y = 0; y < board.Rows; y++)
                {
                    if (x < board.Columns - 1 && y < board.Rows - 1 &&
                        board.Cells[x, y].IsAlive && board.Cells[x + 1, y].IsAlive &&
                        board.Cells[x, y + 1].IsAlive && board.Cells[x + 1, y + 1].IsAlive)
                    {
                        combinationCount++;
                    }
                }
            }
            //комбинация 2 - улей 
            for (int x = 0; x < board.Columns; x++)
            {
                for (int y = 0; y < board.Rows; y++)
                {
                    if (x < board.Columns - 2 && y < board.Rows - 3 &&
                        !board.Cells[x, y].IsAlive && board.Cells[x + 1, y].IsAlive && !board.Cells[x + 2, y].IsAlive &&
                        board.Cells[x, y + 1].IsAlive && !board.Cells[x + 1, y + 1].IsAlive && board.Cells[x + 2, y + 1].IsAlive &&
                        board.Cells[x, y + 2].IsAlive && !board.Cells[x + 1, y + 2].IsAlive && board.Cells[x + 2, y + 2].IsAlive &&
                        !board.Cells[x, y + 3].IsAlive && board.Cells[x + 1, y + 3].IsAlive && !board.Cells[x + 2, y + 3].IsAlive)
                    {
                        combinationCount++;
                    }
                }
            }
            //комбинация 3 - планер
            for (int x = 0; x < board.Columns - 2; x++)
            {
                for (int y = 0; y < board.Rows - 2; y++)
                {
                    if (!board.Cells[x, y].IsAlive &&
                        !board.Cells[x + 1, y].IsAlive && board.Cells[x + 2, y].IsAlive &&
                        board.Cells[x, y + 1].IsAlive && !board.Cells[x + 1, y + 1].IsAlive && board.Cells[x + 2, y + 1].IsAlive &&
                        !board.Cells[x, y + 2].IsAlive && board.Cells[x + 1, y + 2].IsAlive && board.Cells[x + 2, y + 2].IsAlive)
                    {
                        combinationCount++;
                    }
                }
            }
            //комбинация 4 - пруд
            for (int x = 1; x < board.Columns - 2; x++)
            {
                for (int y = 0; y < board.Rows - 3; y++)
                {
                    if (board.Cells[x, y].IsAlive && board.Cells[x + 1, y].IsAlive && board.Cells[x - 1, y + 1].IsAlive &&
                        !board.Cells[x, y + 1].IsAlive && !board.Cells[x + 1, y + 1].IsAlive && board.Cells[x + 2, y + 1].IsAlive
                        && board.Cells[x - 1, y + 2].IsAlive && board.Cells[x + 2, y + 2].IsAlive &&
                        board.Cells[x, y + 3].IsAlive && board.Cells[x + 1, y + 3].IsAlive)
                    {
                        combinationCount++;
                    }
                }
            }
            int expectedLiveCellCount = 4;
            int expectedCombinationCount = 1;

            Assert.AreEqual(expectedLiveCellCount, liveCellCount);
            Assert.AreEqual(expectedCombinationCount, combinationCount);
        }
        [TestMethod]
        public void TestMethod10()
        {
            int width = 5;
            int height = 5;
            int cellSize = 1;
            int[,] i = new int[,]
            {
                { '*', '*', '*', 0, 0 },
                { '*', '*', '*', 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
            };

            Board board = new Board(width, height, cellSize, i);
            board.counts();
            int liveCellCount = 0;

            foreach (var cell in board.Cells)
            {
                if (cell.IsAlive == true)
                {
                    liveCellCount++;
                }
            }
            Assert.AreEqual(6, liveCellCount);
        }
        [TestMethod]
        public void TestMethod11()
        {
            int width = 10;
            int height = 10;
            int cellSize = 1;
            int[,] cimvol = new int[width, height];
            var board = new Board(width, height, cellSize, cimvol);

            int widthValue = board.Width;

            Assert.AreEqual(width, widthValue);
        }
        [TestMethod]
        public void TestMethod12()
        {
            int width = 10;
            int height = 10;
            int cellSize = 1;
            int[,] cimvol = new int[width, height];
            var board = new Board(width, height, cellSize, cimvol);
            int heightValue = board.Height;
            Assert.AreEqual(height, heightValue);
        }
        [TestMethod]
        public void TestMethod13()
        {
            int width = 10;
            int height = 10;
            int cellSize = 1;
            int[,] cimvol = new int[width, height];
            var board = new Board(width, height, cellSize, cimvol);
            int cellSizeValue = board.CellSize;
            Assert.AreEqual(cellSize, cellSizeValue);
        }
        [TestMethod]
        public void TestMethod14()
        {
            int width = 5;
            int height = 5;
            int cellSize = 1;

            int[,] i = new int[,]
            {
                { '*', '*', 0, 0, 0 },
                { '*', '*', 0, 0, '*' },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, '*', 0 },
                { '*', 0, 0, 0, 0 }
            };

            Board board = new Board(width, height, cellSize, i);

            bool isStabilnoe = board.StabilnoeSostoyanie();

            Assert.IsFalse(isStabilnoe);
        }
        [TestMethod]
        public void TestMethod15()
        {
            int width = 5;
            int height = 5;
            int cellSize = 1;

            int[,] i = new int[,]
            {
                { '*', '*', 0, 0, 0 },
                { '*', '*', 0, 0, 0 },
                { 0, 0, 0, '*', '*' },
                { 0, 0, 0, '*', '*' },
                { 0, 0, 0, '*', '*' }
            };

            Board board = new Board(width, height, cellSize, i);
            board.ClassificiruemElements();
            int liveCellCount = 0;
            foreach (var cell in board.Cells)
            {
                if (cell.IsAlive == true)
                {
                    liveCellCount++;
                }
            }
            int expectedLiveCellCount = 10;
            Assert.AreEqual(expectedLiveCellCount, liveCellCount);
        }
    }
}