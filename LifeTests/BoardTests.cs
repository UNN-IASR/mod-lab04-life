namespace cli_life;

[TestClass]
public class BoardTests
{
    [TestMethod]
    public void SaveToFile_SavesToFile()
    {
        var filePath = "../../../boardSettings.json";
        var boardSettings = new BoardSettings
        {
            Width = 50,
            Height = 20,
            CellSize = 1,
            LiveDensity = 0.5
        };

        BoardSettings.SaveToFile(boardSettings, filePath);

        Assert.IsTrue(File.Exists(filePath));
    }

    [TestMethod]
    public void LoadFromFile_LoadsFromFile()
    {
        var filePath = "../../../boardSettings.json";
        var boardSettings = new BoardSettings
        {
            Width = 50,
            Height = 20,
            CellSize = 1,
            LiveDensity = 0.5
        };

        BoardSettings.SaveToFile(boardSettings, filePath);
        boardSettings = (BoardSettings)BoardSettings.LoadFromFile(filePath);

        Assert.IsTrue(boardSettings is not null);
    }

    [TestMethod]
    public void LoadFromFile_LoadsCorrectSettings()
    {
        var filePath = "../../../boardSettings.json";
        var boardSettings = new BoardSettings
        {
            Width = 50,
            Height = 20,
            CellSize = 1,
            LiveDensity = 0.5
        };
        BoardSettings.SaveToFile(boardSettings, filePath);
        
        var loadedSettings = (BoardSettings)BoardSettings.LoadFromFile(filePath);

        Assert.IsTrue(boardSettings.Width == loadedSettings.Width 
            && boardSettings.Height == loadedSettings.Height
            && boardSettings.CellSize == loadedSettings.CellSize 
            && boardSettings.LiveDensity == loadedSettings.LiveDensity);
    }
}