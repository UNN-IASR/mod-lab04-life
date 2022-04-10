using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;
using Newtonsoft.Json;
using System.IO;

namespace UnitTestLife
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ReadingFile()
        {
            String s = "    **    ";
            Board board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("data.json"));
            board.OpenFile("standard_figures/block.txt");
            String res = "";
            for (int row = 0; row < board.Rows; row++)
            {
                var cell = board.Cells[0, row];
                if (cell.IsAlive)
                {
                    res += '*';
                }
                else
                {
                    res += ' ';
                }
            }
            Assert.IsTrue(res == s);
        }
        [TestMethod]
        public void CheckParams()
        {
            Board board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("data.json"));
            int width = board.Width;
            Assert.IsTrue(width == 10);
        }
        [TestMethod]
        public void FindShip()
        {
            String s = " ship";
            Board board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("data.json"));
            board.OpenFile("standard_figures/ship.txt");
            String res = board.findShip();
            Assert.IsTrue(res == s);
        }
        [TestMethod]
        public void CountAliveCells()
        {
            Board board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("data.json"));
            board.OpenFile("standard_figures/hive.txt");
            int count = board.countAliveCells();
            Assert.IsTrue(count == 6);
        }
        [TestMethod]
        public void FindFigures()
        {
            String s = " box hive boat";
            Board board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("data.json"));
            board.OpenFile("colonies/second.txt");
            String res = board.findBlock() + board.findBox() + board.findHive() + board.findBoat() + board.findShip();
            Assert.IsTrue(res == s);
        }
        [TestMethod]
        public void ComeToStablePhase()
        {
            int stopParametr = 5;
            int i = 0;
            int stableTime = 0;
            Board board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("data.json"));
            board.OpenFile("colonies/first.txt");
            while (stopParametr > i)
            {
                int bTime = board.countAliveCells();
                board.Advance();
                i++;
                stableTime = board.becomeStablePhase(bTime, stableTime);
            }
            Assert.IsTrue(stableTime == 5);
        }
        [TestMethod]
        public void hasHorizontalSymmetry()
        {
            Board board = JsonConvert.DeserializeObject<Board>(File.ReadAllText("data.json"));
            board.OpenFile("standard_figures/block.txt");
            bool res = board.hasHorizontalSymmetry();
            Assert.IsTrue(res == true);
        }


    }
}
