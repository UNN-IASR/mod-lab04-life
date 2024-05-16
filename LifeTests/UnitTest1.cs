using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSimulateGameOfLife()
        {
            double density = 0.4;
            int expectedGenerations = 10;
            int actualGenerations = Program.SimulateGameOfLife(density);
            Assert.AreEqual(expectedGenerations, actualGenerations);
        }

        [TestMethod]
        public void TestCountNeighbors()
        {
            bool[,] field = new bool[,]
            {
                { true, false, false },
                { false, true, true },
                { true, true, false }
            };
            int actualCount = Program.CountNeighbors(field, 1, 1);
            Assert.AreEqual(4, actualCount);
        }

        [TestMethod]
        public void TestCompareFields()
        {
            bool[,] field1 = new bool[,]
            {
                { true, false, false },
                { false, true, true },
                { true, true, false }
            };

            bool[,] field2 = new bool[,]
            {
                { true, false, false },
                { false, true, true },
                { true, true, false }
            };

            bool areEqual = Program.CompareFields(field1, field2);
            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void TestLoadSettings()
        {          
            LifeSettings expectedSettings = new LifeSettings
            {
                Width = 10,
                Height = 10,
                CellSize = 5,
                LiveDensity = 0.5
            };
          
            LifeSettings loadedSettings = SettingsManager.LoadSettings("testSettings.json");
          
            Assert.AreEqual(expectedSettings.Width, loadedSettings.Width);
            Assert.AreEqual(expectedSettings.Height, loadedSettings.Height);
            Assert.AreEqual(expectedSettings.CellSize, loadedSettings.CellSize);
            Assert.AreEqual(expectedSettings.LiveDensity, loadedSettings.LiveDensity);
        }
    }
}
