using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;

namespace NET
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void test_is_symmetrical_1()
        {
            Element element = new Element("glider", new List<bool[,]> {
                new bool[5, 5] {
                    { false, false, false, false, false },
                    { false, false, true, false, false },
                    { false, true, true, true, false },
                    { false, false, true, false, false },
                    { false, false, false, false, false },
                },
                new bool[5, 5] {
                    { false, false, false, false, false },
                    { false, false, true, true, false },
                    { false, false, true, true, false },
                    { false, false, false, false, false },
                    { false, false, false, false, false },
                }
            });
            Assert.IsTrue(element.is_symmetrical == false);
        }

        [TestMethod]
        public void test_is_symmetrical_2()
        {
            Element element = new Element("glider", new List<bool[,]> {
                new bool[5, 5] {
                    { false, false, false, false, false },
                    { false, true, true, true, false },
                    { false, true, true, true, false },
                    { false, true, true, true, false },
                    { false, false, false, false, false },
                },
                new bool[5, 5] {
                    { false, false, false, false, false },
                    { false, false, true, true, false },
                    { false, true, true, true, false },
                    { false, true, true, false, false },
                    { false, false, false, false, false },
                }
            });
            Assert.IsTrue(element.is_symmetrical == true);
        }

        [TestMethod]
        public void test_get_condition()
        {
            int[,] condition =
            {
                { 1, 0, 0, 1, 1, 0, 1, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1 },
                { 0, 0, 0, 1, 1, 0, 1, 1, 1, 1 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 0, 1, 1, 1, 0, 0 }
            };

            StreamWriter writer = new StreamWriter("test.txt");
            for (int i = 0; i < condition.GetLength(0); i++)
            {
                for (int j = 0; j < condition.GetLength(0); j++)
                {
                    writer.Write(condition[i, j] != 0 ? "1," : "0,");
                }
                writer.Write("\n");
            }

            Board board = new Board (
                                width: 10,
                                height: 10,
                                cellSize: 1,
                                fname: "test");

            List<List<bool>> board_condition = board.get_condition();
            bool is_equal = true;
            for (int i = 0; i < condition.GetLength(0) && is_equal; i++)
            {
                for (int j = 0; j < condition.GetLength(0) && is_equal; j++)
                {
                    is_equal = (condition[i, j] != 0) == board_condition[i][j];
                }
            }

            Assert.IsTrue(is_equal);
        }

        [TestMethod]
        public void test_element_counter()
        {
            int[,] condition =
            {
                { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1 },
                { 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 }
            };

            StreamWriter writer = new StreamWriter("test.txt");
            for (int i = 0; i < condition.GetLength(0); i++)
            {
                for (int j = 0; j < condition.GetLength(0); j++)
                {
                    writer.Write(condition[i, j] != 0 ? "1," : "0,");
                }
                writer.Write("\n");
            }

            Board board = new Board(
                                width: 10,
                                height: 10,
                                cellSize: 1,
                                fname: "test");

            Element element = new Element("glider", new List<bool[,]> {
                new bool[4, 5] {
                    { false, false, false, false, false },
                    { false, false, true, false, false },
                    { false, true, false, true, false },
                    { false, true, true, true, false }
                },
                new bool[5, 4] {
                    { false, false, false, false },
                    { false, true, false, false },
                    { false, true, true, false },
                    { false, false, true, false },
                    { false, false, true, false }
                }
            });

            Assert.IsTrue(board.element_counter(element) == 5);
        }

        [TestMethod]
        public void test_counting()
        {
            int[,] condition =
            {
                { 1, 0, 0, 1 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 1 }
            };

            StreamWriter writer = new StreamWriter("test.txt");
            for (int i = 0; i < condition.GetLength(0); i++)
            {
                for (int j = 0; j < condition.GetLength(0); j++)
                {
                    writer.Write(condition[i, j] != 0 ? "1," : "0,");
                }
                writer.Write("\n");
            }

            Board board = new Board(
                                width: 10,
                                height: 10,
                                cellSize: 1,
                                fname: "test");

            Assert.IsTrue(Program.counting() == (7, 5));
        }

        [TestMethod]
        public void test_stabilization()
        {
            int[,] condition =
            {
                { 0, 0, 0 },
                { 1, 1, 1 },
                { 0, 0, 0 }
            };

            StreamWriter writer = new StreamWriter("test.txt");
            for (int i = 0; i < condition.GetLength(0); i++)
            {
                for (int j = 0; j < condition.GetLength(0); j++)
                {
                    writer.Write(condition[i, j] != 0 ? "1," : "0,");
                }
                writer.Write("\n");
            }

            Board board = new Board(
                                width: 10,
                                height: 10,
                                cellSize: 1,
                                fname: "test");

            Assert.IsTrue(Program.stabilization() == (1, 2));
        }
    }
}
