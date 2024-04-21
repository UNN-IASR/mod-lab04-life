using cli_life;

namespace UnitTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void BoardCreate()
    {
        Board b = new Board(30, 30, 0.5);
        Assert.AreEqual(30, b.Width);
        Assert.AreEqual(30, b.Height);
    }
    [TestMethod]
    public void BoardAdvance()
    {
        Board b = new Board(30, 30, 0.5);
        b.Advance();
        b.Advance();
        b.Advance();
        Assert.AreEqual(3, b.states.Count);
    }
    [TestMethod]
    public void BoardSaveTo()
    {
        Board b = new Board(30, 30, 0.5);
        b.SaveTo("savetest.txt");
        Assert.IsTrue(File.Exists("savetest.txt"));
    }
    [TestMethod]
    public void BoardLoadFrom()
    {
        Board b = new Board(1, 1);
        b.LoadFrom("../../../loadtest.txt");
        Assert.IsTrue(b.alive > 0);
        Assert.AreEqual(28, b.generation);
    }
    [TestMethod]
    public void ToolsLoadBoardFron()
    {
        Board b = Tools.LoadBoardFrom("../../../settings.json");
        Assert.AreEqual(110, b.Width);
        Assert.AreEqual(50, b.Height);
    }
    [TestMethod]
    public void ToolsMakeString()
    {
        Board b = new Board(1, 1);
        b.LoadFrom("../../../loadtest.txt");
        string result = Tools.MakeString(b, 5, 5, 5, 5);
        Assert.AreEqual(" *   *   *    *    *     ", result);
    }
    [TestMethod]
    public void ToolsRun()
    {
        Board b = new Board(30, 30, 0.3);
        var result = Tools.Run(b, every: 1);
        Assert.AreEqual(b.generation, result.Count());
    }
    [TestMethod]
    public void ToolsCountBlock()
    {
        Board b = new Board(1, 1);
        b.LoadFrom("../../../figures.txt");
        var result = Tools.CountFigures(b);
        Assert.AreEqual(11, result["block"]);
    }
    [TestMethod]
    public void ToolsCountBeehive()
    {
        Board b = new Board(1, 1);
        b.LoadFrom("../../../figures.txt");
        var result = Tools.CountFigures(b);
        Assert.AreEqual(2, result["beehive"]);
    }
    [TestMethod]
    public void ToolsCountBlinker()
    {
        Board b = new Board(1, 1);
        b.LoadFrom("../../../figures.txt");
        var result = Tools.CountFigures(b);
        Assert.AreEqual(2, result["blinker"]);
    }
    [TestMethod]
    public void BoardStable()
    {
        Board b = new Board(1, 1);
        b.LoadFrom("../../../stable.txt");
        for (int i = 0; i < 10; i++) b.Advance();
        Assert.IsTrue(b.stable);
    }
    [TestMethod]
    public void BoardUnstable()
    {
        Board b = new Board(1, 1);
        b.LoadFrom("../../../loadtest.txt");
        for (int i = 0; i < 10; i++) b.Advance();
        Assert.IsFalse(b.stable);
    }
    [TestMethod]
    public void BoardCountAlive()
    {
        Board b = new Board(1, 1);
        b.LoadFrom("../../../loadtest.txt");
        Assert.AreEqual(276, b.alive);
    }
    [TestMethod]
    public void BoardOverload()
    {
        Board b = new Board(50, 50, 0.99);
        for (int i = 0; i < 5; i++) b.Advance();
        Assert.IsTrue(b.stable);
        Assert.AreEqual(0, b.alive);
    }
    [TestMethod]
    public void BoardEmpty()
    {
        Board b = new Board(50, 50, 0.01);
        for (int i = 0; i < 5; i++) b.Advance();
        Assert.IsTrue(b.stable);
        Assert.AreEqual(0, b.alive);
    }
}