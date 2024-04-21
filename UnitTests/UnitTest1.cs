using cli_life;

namespace UnitTests;

[TestClass]
public class UnitTest1
{
    readonly string path = "../../../../";
    [TestMethod]
    public void BoardRows() {
        Board board = new Board(5, 10, 1);
        Assert.AreEqual(10, board.Rows);
    }
    [TestMethod]
    public void BoardColumns() {
        Board board = new Board(5, 10, 1);
        Assert.AreEqual(5, board.Columns);
    }
    [TestMethod]
    public void BoardCellSize() {
        Board board = new Board(5, 10, 1);
        Assert.AreEqual(1, board.CellSize);
    }
    [TestMethod]
    public void BoardHeight() {
        Board board = new Board(5, 10, 1);
        Assert.AreEqual(10, board.Height);
    }
    [TestMethod]
    public void BoardWidth() {
        Board board = new Board(5, 10, 1);
        Assert.AreEqual(5, board.Width);
    }
    [TestMethod]
    public void BoardFromJsonRows() {
        Board board = Board.ResetFromFile(path + "settings.json");
        Assert.AreEqual(50, board.Rows);
    }
    [TestMethod]
    public void BoardFromJsonColumns() {
        Board board = Board.ResetFromFile(path + "settings.json");
        Assert.AreEqual(40, board.Columns);
    }
    [TestMethod]
    public void SaveBoard() {
        Board board = new Board(50, 50, 1);
        board.SaveToFile("savedBoard.txt");
        Assert.IsTrue(File.Exists("savedBoard.txt"));
    }
    [TestMethod]
    public void LoadFromFile() {
        Board board = Board.LoadFromFile(path + "boardToLoad.txt");
        Assert.AreEqual(6, board.Columns);
    }
    [TestMethod]
    public void CountLivingCells() {
        Board board = Board.LoadFromFile(path + "boardToLoad.txt");
        Assert.AreEqual(4, board.CountLivingCells());
    }
    [TestMethod]
    public void GetCellsInString() {
        Board board = Board.LoadFromFile(path + "boardToLoad.txt");
        Assert.AreEqual("...*....**................*...", board.GetCellsInString());
    }
    [TestMethod]
    public void LoadPatterns() {
        BoardAnalyzer.LoadPatterns(path +"patterns.json");
        Assert.AreEqual(11, BoardAnalyzer.patterns.Count());
    }
    [TestMethod]
    public void CheckPatterns() {
        Board board = Board.LoadFromFile(path + "boardToCheckPatterns.txt");
        BoardAnalyzer.LoadPatterns(path +"patterns.json");
        Dictionary<string, int> res = BoardAnalyzer.CheckPatterns(board);
        Assert.AreEqual(1, res["HiveHorizontal"]);
        Assert.AreEqual(1, res["Block"]);
        Assert.AreEqual(1, res["Snake"]);
        Assert.AreEqual(0, res["Canoe"]);
    }
    [TestMethod]
    public void Simulate() {
        Board board = Board.LoadFromFile(path + "boardToCheckSimulate.txt");
        BoardAnalyzer.LoadPatterns(path +"patterns.json");
        Dictionary<int, int> res = BoardAnalyzer.Simulate(board);
        Assert.AreEqual(67, res[16]);
    }
    [TestMethod]
    public void CreateGraph() {
        BoardAnalyzer.CreateGraph(new List<double>() { 0.2, 0.4, 0.6, 0.8}, 40, 40);
        Assert.IsTrue(File.Exists("plot.png"));
    }

}