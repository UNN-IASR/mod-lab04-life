using cli_life;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test1()
        {
            BoardSettings bs = new BoardSettings();
            Board b = new Board(bs.Width, bs.Height, bs.CellSize, bs.LiveDensity, "../../../gen1.txt");
            Assert.AreEqual(b.isStable(), true);
        }
        [TestMethod]
        public void Test2()
        {
            BoardSettings bs = new BoardSettings();
            Board b = new Board(bs.Width, bs.Height, bs.CellSize, bs.LiveDensity, "../../../gen2.txt");
            Assert.AreEqual(b.isStable(), true);
        }
        [TestMethod]
        public void Test3()
        {
            BoardSettings bs = new BoardSettings();
            Board b = new Board(bs.Width, bs.Height, bs.CellSize, bs.LiveDensity, "../../../pulsar.txt");
            Assert.AreEqual(b.isStable(), false);
        }
    }
}