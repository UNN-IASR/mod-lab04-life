using cli_life;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace LifeTest
{
    [TestClass]
    public class UnitTest1
    {
        static Settings s = new Settings()
        {
            width = 50,
            height = 40,
            cellSize = 1,
            liveDensity = 0.3f
        };
        static Settings s2 = new Settings()
        {
            width = 20,
            height = 20,
            cellSize = 1,
            liveDensity = 0.5f
        };
        static Settings s3 = new()
        {
            width = 100,
            height = 100,
            cellSize = 1,
            liveDensity = 0.5f
        };
        [TestMethod]
        public void Sanity1()
        {
            Board board = new(s);
            Assert.AreEqual(board.Height, 40);
            Assert.AreEqual(board.Width, 50);
            Assert.AreEqual(board.Columns, 50);
            Assert.AreEqual(board.Rows, 40);
        }
        [TestMethod]
        public void Sanity2()
        {
            Board board = new(s);
            Assert.ThrowsException<Exception>(() => board.Randomize(10.0f));
        }
        [TestMethod]
        public void Sanity3()
        {
            Board board = new(s);
            board.Advance();
        }
        [TestMethod]
        public void Spaceship()
        {
            Runner r = new(s2, "../../../spaceship.txt");
            r.Run(100);
            Assert.AreEqual(r.overflow, false);
            Assert.AreEqual(r.step, 41);
        }
        [TestMethod]
        public void Stable()
        {
            Runner r = new(s2, "../../../stable.txt");
            r.Run(100);
            Assert.AreEqual(r.overflow, false);
            Assert.AreEqual(r.step, 7);
        }
        [TestMethod]
        public void Static()
        {
            Runner r = new(s2, "../../../static.txt");
            r.Run(100);
            Assert.AreEqual(r.overflow, false);
            Assert.AreEqual(r.step, 16);
        }
        [TestMethod]
        public void Logo()
        {
            Runner r = new(s2, "../../../logo.txt");
            r.Run(100);
            Assert.AreEqual(r.overflow, true);
            Assert.AreEqual(r.step, 100);
        }
        [TestMethod]
        public void Eater()
        {
            Runner r = new(s2, "../../../eater.txt");
            r.Run(100);
            Assert.AreEqual(r.overflow, false);
            Assert.AreEqual(r.step, 6);
        }
        [TestMethod]
        public void Gun()
        {
            Runner r = new(s3, "../../../gun.txt");
            r.Run(100);
            Assert.AreEqual(r.overflow, true);
            Assert.AreEqual(r.step, 100);
        }
        [TestMethod]
        public void Train()
        {
            Runner r = new(s3, "../../../train.txt");
            r.Run(100);
            Assert.AreEqual(r.overflow, true);
            Assert.AreEqual(r.step, 100);
        }
        [TestMethod]
        public void Save()
        {
            Runner r = new(s2, "../../../spaceship.txt");
            r.Run(100);
            r.board.Save("spaceship_save.txt");
            if (!File.Exists("spaceship_save.txt")) Assert.Fail();
        }
        [TestMethod]
        public void SaveCheck()
        {
            Runner r = new(s3, "../../../train.txt");
            r.Run(100);
            r.board.Save("train_save.txt");
            var expect = File.ReadAllLines("../../../train_expect.txt");
            var real = File.ReadAllLines("train_save.txt");
            for (int i = 0; i < expect.Length; i++)
            {
                Assert.AreEqual(expect[i], real[i]);
            }
        }
        [TestMethod]
        public void Blank()
        {
            Runner r = new(s2, "../../../blank.txt");
            r.Run(100);
            Assert.AreEqual(r.overflow, false);
            Assert.AreEqual(r.step, 2);
        }
        [TestMethod]
        public void Chart()
        {
            int[] dataX = { 1, 2, 4 };
            int[] dataY = { 3, 4, 5 };
            ScottPlot.Plot myPlot = new();
            myPlot.Add.Scatter(dataX, dataY);
            myPlot.SavePng("chart.png", 1000, 800);
            Assert.IsTrue(File.Exists("chart.png"));
        }
        [TestMethod]
        public void ChartStatistics()
        {
            List<float> dataX = new();
            List<float> dataY = new();
            List<int> dataoverflows = new();
            int current_sum = 0;
            float previous = 0.0f;
            int overflow = 0;
            for (float i = 0.0001f; i < 1; i += 0.0001f)
            {
                if (Math.Ceiling(i * 100.0f) != previous)
                {
                    Console.WriteLine($"AVG iterations for {previous}% density is {current_sum / 100.0f} with {overflow} overflows.");
                    dataX.Add(previous);
                    dataY.Add(current_sum / 100.0f);
                    dataoverflows.Add(overflow);
                    current_sum = 0;
                    overflow = 0;
                }
                Settings s = new Settings()
                {
                    width = 30,
                    height = 30,
                    cellSize = 1,
                    liveDensity = (float)Math.Ceiling(i * 100) / 100.0f
                };
                Runner runner = new(s, "");
                const int max_iter = 2;
                runner.Run(max_iter);
                if (runner.overflow) overflow++;
                current_sum += runner.step;
                previous = (float)Math.Ceiling(i * 100);
            }

            ScottPlot.Plot myPlot = new();
            myPlot.Add.Scatter(dataX, dataY);
            myPlot.SavePng("chart_stat.png", 1000, 800);
            Assert.IsTrue(File.Exists("chart_stat.png"));
        }
    }
}