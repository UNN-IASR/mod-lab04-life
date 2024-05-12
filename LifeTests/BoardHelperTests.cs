namespace cli_life;

[TestClass]
public class BoardHelperTests
{
    [TestMethod]
    public void GetAliveCellsCount_ReturnsCorrectAliveCellsCount()
    {
        var board = new Board(3, 3, 1, 1);
        var helper = new BoardHelper(board, null);

        Assert.AreEqual(9, helper.GetAliveCellsCount());
    }
}
