using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;
using System.IO;

namespace test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1_ReadFile()
        {
            Program.read("file.txt");
        }

        [TestMethod]
        public void TestMethod2_AddFigure()
        {
            Program.read("file.txt");
            Program.add_figure("block", 1, 2);
        }

        [TestMethod]
        public void TestMethod3_FindFigure()
        {
            Program.read("file.txt");
            Program.add_figure("block", 1, 2);
            int k = Program.find("block");
            Assert.AreEqual(1, k);
        }

        [TestMethod]
        public void TestMethod4_ReadSymmetryVertically()
        {
            Program.read("file2.txt");
        }

        [TestMethod]
        public void TestMethod5_CheckSymmetryVertically()
        {
            Program.read("file2.txt");
            Assert.AreEqual(true, Program.symmetry_vertically());
        }

        [TestMethod]
        public void TestMethod6_ReadSymmetryHorizontally()
        {
            Program.read("file3.txt");
        }

        [TestMethod]
        public void TestMethod7_CheckSymmetryHorizontally()
        {
            Program.read("file3.txt");
            Assert.AreEqual(true, Program.symmetry_horizontally());
        }

        [TestMethod]
        public void TestMethod8_ResetBoard()
        {
            Program.Reset();
        }

        [TestMethod]
        public void TestMethod9_CheckBoardWidth()
        {
            Program.Reset();
            Assert.AreEqual(50, Program.board.Width);
        }

        [TestMethod]
        public void TestMethod10_CheckBoardHeight()
        {
            Program.Reset();
            Assert.AreEqual(50, Program.board.Height);
        }

        [TestMethod]
        public void TestMethod11_RenderBoard()
        {
            Program.Reset();
            Program.Render();
        }

        [TestMethod]
        public void TestMethod12_CheckFileExists()
        {
            FileStream stream = File.Open("plot.png", FileMode.Open);
            Assert.IsNotNull(stream);
        }

        [TestMethod]
        public void TestMethod13_SaveBoard()
        {
            Program.Reset();
            Program.save();
            Assert.IsTrue(File.Exists("SavingFile.txt"));
        }

        [TestMethod]
        public void TestMethod14_CheckFindFunction()
        {
            Program.read("file.txt");
            Program.add_figure("block", 1, 2);
            int k = Program.find("block");
            Assert.AreEqual(1, k);
        }

        [TestMethod]
        public void TestMethod15_TestLiveDensity()
        {
            Program.Reset();
            Assert.IsTrue(Program.board.LiveDensity > 0);
        }
    }
}
