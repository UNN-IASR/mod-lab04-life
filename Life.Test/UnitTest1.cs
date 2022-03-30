using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace Life.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            String s = "user.json";
            Board board = new Board();
            int? result = board.stepstostablefile(s);
            Assert.IsTrue(result == 630);
        }
        [TestMethod]
        public void TestMethod2()
        {
            String s = "user.json";
            Board board = new Board();
            int? result = board.stablefile(s);
            Assert.IsTrue(result == 3);
        }
        [TestMethod]
        public void TestMethod3()
        {
            String s = "user.json";
            Board board = new Board();
            int? result = board.periodicfile(s);
            Assert.IsTrue(result == 1);
        }
        [TestMethod]
        public void TestMethod4()
        {
            String s = "user.json";
        Board board = new Board();
        int? result = board.simmetricfile(s);
        Assert.IsTrue(result == 2);
        }
    [TestMethod]
    public void TestMethod5()
    {
        String s = "user.json";
        Board board = new Board();
        int? result = board.Bargefile(s);
        Assert.IsTrue(result == 0);
    }
    [TestMethod]
    public void TestMethod6()
    {
        String s = "user.json";
        Board board = new Board();
        int? result = board.Hivefile(s);
        Assert.IsTrue(result == 1);
    }
    [TestMethod]
    public void TestMethod7()
    {
        String s = "user.json";
        Board board = new Board();
        int? result = board.Blockfile(s);
        Assert.IsTrue(result == 1);
    }
    [TestMethod]
    public void TestMethod8()
    {
        String s = "user.json";
        Board board = new Board();
        int? result = board.Boxfile(s);
        Assert.IsTrue(result == 0);
    }
    [TestMethod]
    public void TestMethod9()
    {
        String s = "user.json";
        Board board = new Board();
        int? result = board.Boatfile(s);
        Assert.IsTrue(result == 0);
    }
    [TestMethod]
    public void TestMethod10()
    {
        String s = "user.json";
        Board board = new Board();
        int? result = board.Shipfile(s);
        Assert.IsTrue(result == 0);
    }
    [TestMethod]
    public void TestMethod11()
    {
        String s = "user.json";
        Board board = new Board();
        int? result = board.Pondfile(s);
        Assert.IsTrue(result == 0);
    }
    [TestMethod]
    public void TestMethod12()
    {
        String s = "user.json";
        Board board = new Board();
        int? result = board.Flasherfile(s);
        Assert.IsTrue(result == 0);
    }
    [TestMethod]
    public void TestMethod13()
    {
        String s = "test1.json";
        Board board = new Board();
        int? result = board.stepstostablefile(s);
        Assert.IsTrue(result == 805);
    }
    [TestMethod]
    public void TestMethod14()
    {
        String s = "test1.json";
        Board board = new Board();
        int? result = board.stablefile(s);
        Assert.IsTrue(result == 3);
    }
    [TestMethod]
    public void TestMethod15()
    {
        String s = "test1.json";
        Board board = new Board();
        int? result = board.periodicfile(s);
        Assert.IsTrue(result == 1);
    }
    [TestMethod]
    public void TestMethod16()
        {
            String s = "test1.json";
    Board board = new Board();
    int? result = board.simmetricfile(s);
    Assert.IsTrue(result == 4);
        }
[TestMethod]
public void TestMethod17()
{
    String s = "test1.json";
    Board board = new Board();
    int? result = board.Bargefile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod18()
{
    String s = "test1.json";
    Board board = new Board();
    int? result = board.Hivefile(s);
    Assert.IsTrue(result == 2);
}
[TestMethod]
public void TestMethod19()
{
    String s = "test1.json";
    Board board = new Board();
    int? result = board.Blockfile(s);
    Assert.IsTrue(result == 1);
}
[TestMethod]
public void TestMethod20()
{
    String s = "test1.json";
    Board board = new Board();
    int? result = board.Boxfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod21()
{
    String s = "test1.json";
    Board board = new Board();
    int? result = board.Boatfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod22()
{
    String s = "test1.json";
    Board board = new Board();
    int? result = board.Shipfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod23()
{
    String s = "test1.json";
    Board board = new Board();
    int? result = board.Pondfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod24()
{
    String s = "test1.json";
    Board board = new Board();
    int? result = board.Flasherfile(s);
    Assert.IsTrue(result == 1);
}
[TestMethod]
public void TestMethod25()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.stepstostablefile(s);
    Assert.IsTrue(result == 145);
}
[TestMethod]
public void TestMethod26()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.stablefile(s);
    Assert.IsTrue(result == 2);
}
[TestMethod]
public void TestMethod27()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.periodicfile(s);
    Assert.IsTrue(result == 1);
}
[TestMethod]
public void TestMethod28()
        {
            String s = "test2.json";
Board board = new Board();
int? result = board.simmetricfile(s);
Assert.IsTrue(result == 3);
        }
        [TestMethod]
public void TestMethod29()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.Bargefile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod30()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.Hivefile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod31()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.Blockfile(s);
    Assert.IsTrue(result == 1);
}
[TestMethod]
public void TestMethod32()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.Boxfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod33()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.Boatfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod34()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.Shipfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod35()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.Pondfile(s);
    Assert.IsTrue(result == 1);
}
[TestMethod]
public void TestMethod36()
{
    String s = "test2.json";
    Board board = new Board();
    int? result = board.Flasherfile(s);
    Assert.IsTrue(result == 1);
}
[TestMethod]
public void TestMethod37()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.stepstostablefile(s);
    Assert.IsTrue(result == 745);
}
[TestMethod]
public void TestMethod38()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.stablefile(s);
    Assert.IsTrue(result == 8);
}
[TestMethod]
public void TestMethod39()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.periodicfile(s);
    Assert.IsTrue(result == 4);
}
[TestMethod]
public void TestMethod40()
        {

            String s = "test3.json";
Board board = new Board();
int? result = board.simmetricfile(s);
Assert.IsTrue(result == 11);
        }
        [TestMethod]
public void TestMethod41()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.Bargefile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod42()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.Hivefile(s);
    Assert.IsTrue(result == 5);
}
[TestMethod]
public void TestMethod43()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.Blockfile(s);
    Assert.IsTrue(result == 2);
}
[TestMethod]
public void TestMethod44()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.Boxfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod45()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.Boatfile(s);
    Assert.IsTrue(result == 1);
}
[TestMethod]
public void TestMethod46()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.Shipfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod47()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.Pondfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod48()
{
    String s = "test3.json";
    Board board = new Board();
    int? result = board.Flasherfile(s);
    Assert.IsTrue(result == 4);
}
[TestMethod]
public void TestMethod49()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.stepstostablefile(s);
    Assert.IsTrue(result == 770);
}
[TestMethod]
public void TestMethod50()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.stablefile(s);
    Assert.IsTrue(result == 13);
}
[TestMethod]
public void TestMethod51()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.periodicfile(s);
    Assert.IsTrue(result == 14);
}
[TestMethod]
public void TestMethod52()
        { String s = "test4.json";
Board board = new Board();
int? result = board.simmetricfile(s);
Assert.IsTrue(result == 23);
        }
        [TestMethod]
public void TestMethod53()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.Bargefile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod54()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.Hivefile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod55()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.Blockfile(s);
    Assert.IsTrue(result == 9);
}
[TestMethod]
public void TestMethod56()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.Boxfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod57()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.Boatfile(s);
    Assert.IsTrue(result == 2);
}
[TestMethod]
public void TestMethod58()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.Shipfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod59()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.Pondfile(s);
    Assert.IsTrue(result == 0);
}
[TestMethod]
public void TestMethod60()
{
    String s = "test4.json";
    Board board = new Board();
    int? result = board.Flasherfile(s);
    Assert.IsTrue(result == 14);
}
    }
}

