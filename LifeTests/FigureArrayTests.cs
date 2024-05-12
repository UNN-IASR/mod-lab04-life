namespace cli_life;

[TestClass]
public class FigureArrayTests
{
    [TestMethod]
    public void SaveToFile_SavesToFile()
    {
        var filePath = "../../../figures.json";
        var figures = new Figure[]
        {
        new Figure("Block", 4, 4, "     **  **     ")
        };
        var figureArray = new FigureArray(figures);

        figureArray.SaveToFile(filePath);

        Assert.IsTrue(File.Exists(filePath));
    }

    [TestMethod]
    public void LoadFromFile_LoadsFromFile()
    {
        var filePath = "../../../figures.json";
        var figures = new Figure[]
        {
        new Figure("Block", 4, 4, "     **  **     ")
        };
        var figureArray = new FigureArray(figures);
        figureArray.SaveToFile(filePath);

        figureArray = (FigureArray)FigureArray.LoadFromFile(filePath);

        Assert.IsFalse(figureArray is null);
    }

    [TestMethod]
    public void LoadFromFile_LoadsCorrectFigures()
    {
        var filePath = "../../../figures.json";
        var figures = new Figure[]
        {
        new Figure("Block", 4, 4, "     **  **     "),
        new Figure("Box", 5, 5, "       *   * *   *       ")
        };
        var figureArray = new FigureArray(figures);
        figureArray.SaveToFile(filePath);

        var loadedFigureArray = (FigureArray)FigureArray.LoadFromFile(filePath);

        for (var index = 0; index < figures.Length; index++)
        {
            Assert.IsTrue(figures[index].Equals(loadedFigureArray.Figures[index]));
        }
    }
}
