namespace cli_life;

[TestClass]
public class FigureTests
{
    [TestMethod]
    public void Equals_DifferentObjectType_ReturnsFalse()
    {
        var obj = 1;
        var figure = new Figure("Block", 4, 4, "     **  **     ");

        Assert.IsFalse(figure.Equals(obj));
    }

    [TestMethod]
    public void Equals_UnequalFigure_ReturnsFalse()
    {
        var firstFigure = new
            Figure("Block", 4, 4, "     **  **     ");
        var secondFigure = new
            Figure("Cross", 5, 5, "       *   ***   *       ");

        Assert.IsFalse(firstFigure.Equals(secondFigure));
    }

    [TestMethod]
    public void Equals_EqualFigure_ReturnsTrue()
    {
        var firstFigure = new
            Figure("Block", 4, 4, "     **  **     ");
        var secondFigure = new
            Figure("bl0ck", 4, 4, "     **  **     ");

        Assert.IsTrue(firstFigure.Equals(secondFigure));
    }
}
