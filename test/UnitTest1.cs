using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;
//using System.Windows.Media;
using System.IO;
//namespace NET;
namespace test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Program.read("file.txt");
            Program.add_figure("block", 1, 2);
            int k = Program.find("block");
            Assert.AreEqual(1, k);
        }
        [TestMethod]
        public void TestMethod2()
        {
            Program.read("file2.txt");
            Assert.AreEqual(true, Program.symmetry_vertically());
        }

        [TestMethod]
        public void TestMethod3()
        {
            Program.read("file3.txt");
            Assert.AreEqual(true, Program.symmetry_horizontally());
        }
        [TestMethod]
        public void TestMethod4()
        {
            Program.Reset();
            Assert.AreEqual(50, Program.board.Width);
            Assert.AreEqual(50, Program.board.Height);
        }
        [TestMethod]
        public void TestMethod5()
        {
            FileStream stream = null;
            
            Program.Reset();
            Program.Render();
            stream = File.Open("1.png", FileMode.Open);
            bool flag = false;
            if (stream != null)
                flag= true;
            Assert.IsTrue(flag);
        }
    }
}
