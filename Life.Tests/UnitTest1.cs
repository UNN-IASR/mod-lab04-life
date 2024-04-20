using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UnitTest1
{
    [TestClass]
    public class UnitTest1
    {
        private static Board board;
        private static string fullPath_Json = "settings.json";

        Dictionary<string, int> dStableFigure = new Dictionary<string, int>()
                {
                { "Barge", 0},
                { "Block", 0 },
                { "Boat", 0 },
                { "Box", 0 },
                { "Hive", 0  },
                { "Loaf", 0  },
                { "Pond" , 0 },
                { "Ship", 0  },
                { "Snake" , 0 },
                { "Stick" , 0 }
                };

        [TestInitialize]
        public void Setup()
        {
            int width = 50;
            int height = 20;
            int cellSize = 1;
            double liveDensity = 0.5;
            board = new Board(width, height, cellSize, liveDensity);
        }
        [TestMethod]
        public void Test_BoardWidth()
        {
            Assert.AreEqual(50, board.Width);
        }
        [TestMethod]
        public void Test_BoardHeight()
        {
            Assert.AreEqual(20, board.Height);
        }
        [TestMethod]
        public void Test_FileSetting_Width()
        {
            Setting settings = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(fullPath_Json));
            Program.Reset(settings);
            Assert.AreEqual(settings.Width, board.Width);
        }
        [TestMethod]
        public void Test_FileSetting_Height()
        {
            Setting settings = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(fullPath_Json));
            Program.Reset(settings);
            Assert.AreEqual(settings.Height, board.Height);
        }
        [TestMethod]
        public void Test_FileSetting_CellSize()
        {
            Setting settings = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(fullPath_Json));
            Program.Reset(settings);
            Assert.AreEqual(settings.CellSize, board.CellSize);
        }
        //6
        [TestMethod]
        public void Test_CellNeighbor_1()
        {
            Cell cell = board.Cells[5, 5];
            Assert.AreEqual(8, cell.neighbors.Count);
        }
        [TestMethod]
        public void Test_CellNeighbor_2()
        {
            Cell cell = board.Cells[0, 0];
            Assert.AreEqual(8, cell.neighbors.Count);
        }
        [TestMethod]
        public void Test_FileOUT_Width()
        {
            Setting settings;
            int count;
            FileSetting.Out("gen-31.txt", out board, out settings, out count);
            Assert.AreEqual(settings.Width, board.Width);
        }
        [TestMethod]
        public void Test_FileOUT_Height()
        {
            Setting settings;
            int count;
            FileSetting.Out("gen-31.txt", out board, out settings, out count);
            Assert.AreEqual(settings.Height, board.Height);
        }
        //8
        [TestMethod]
        public void Test_FileOUT_CellSize()
        {
            Setting settings;
            int count;
            FileSetting.Out("gen-31.txt", out board, out settings, out count);
            Assert.AreEqual(settings.CellSize, board.CellSize);
        }
        //9
        [TestMethod]
        public void Test_CountFigure_1()
        {
            Setting settings;
            int count;
            FileSetting.Out("1.txt", out board, out settings, out count);
            var fig = new Figure();
            fig.CountOfFigure(board, ref dStableFigure);
            Assert.AreEqual(fig.countFigure, 4);
        }

        [TestMethod]
        public void Test_CountFigure_2()
        {
            Setting settings;
            int count;
            FileSetting.Out("2.txt", out board, out settings, out count);
            var fig = new Figure();
            fig.CountOfFigure(board, ref dStableFigure);
            Assert.AreEqual(fig.countFigure, 0);
        }

        [TestMethod]
        public void Test_NameStateFigure_1()
        {
            Setting settings;
            int count;
            FileSetting.Out("3.txt", out board, out settings, out count);
            var fig = new Figure();
            fig.CountOfFigure(board, ref dStableFigure);
            string name = "";
            foreach (var i in dStableFigure)
            {
                if (i.Value == 1) name = i.Key;
            }
            //if (dStableFigure.ContainsValue(1))
            //    name = dStableFigure.
            Assert.AreEqual(name, "Barge");
        }

        [TestMethod]
        public void Test_NameStateFigure_2()
        {
            Setting settings;
            int count;
            FileSetting.Out("1.txt", out board, out settings, out count);
            var fig = new Figure();
            fig.CountOfFigure(board, ref dStableFigure);
            string name = "";
            foreach (var i in dStableFigure)
            {
                if (i.Value == 1) name = i.Key;
            }
            //if (dStableFigure.ContainsValue(1))
            //    name = dStableFigure.
            Assert.AreEqual(name, "Block");
        }

        [TestMethod]
        public void Test_SameFigure()
        {
            Setting settings;
            int count;

            Dictionary<string, int> dStableFigure1 = new Dictionary<string, int>()
                {
                { "Barge", 0},
                { "Block", 0 },
                { "Boat", 0 },
                { "Box", 0 },
                { "Hive", 0  },
                { "Loaf", 0  },
                { "Pond" , 0 },
                { "Ship", 0  },
                { "Snake" , 0 },
                { "Stick" , 0 }
                };
            Dictionary<string, int> dStableFigure2 = new Dictionary<string, int>()
                {
                { "Barge", 0},
                { "Block", 0 },
                { "Boat", 0 },
                { "Box", 0 },
                { "Hive", 0  },
                { "Loaf", 0  },
                { "Pond" , 0 },
                { "Ship", 0  },
                { "Snake" , 0 },
                { "Stick" , 0 }
                };

            FileSetting.Out("3.txt", out board, out settings, out count);
            var fig1 = new Figure();
            fig1.CountOfFigure(board, ref dStableFigure1);
            string name1 = "";
            foreach (var i in dStableFigure1)
            {
                if (i.Value == 1) name1 = i.Key;
            }

            FileSetting.Out("4.txt", out board, out settings, out count);
            var fig2 = new Figure();
            fig2.CountOfFigure(board, ref dStableFigure2);
            string name2 = "";
            foreach (var i in dStableFigure2)
            {
                if (i.Value == 1) name2 = i.Key;
            }
            //if (dStableFigure.ContainsValue(1))
            //    name = dStableFigure.
            Assert.AreEqual(name1, name2);
        }
    }
}
