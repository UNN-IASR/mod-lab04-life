namespace LifeTests;
using cli_life;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        int[,] mat = {{0,0,0,0},
                      {0,1,1,0},
                      {0,1,1,0},
                      {0,0,0,1}};
        int [,] m = Program.Rotate90(mat);
        m = Program.Rotate90(m);
        m = Program.Rotate90(m);
        m = Program.Rotate90(m);
        Assert.AreEqual(mat[mat.GetLength(0)-1,mat.GetLength(1)-1], m[m.GetLength(0)-1,m.GetLength(1)-1]);
    }
    [TestMethod]
    public void TestMethod2()
    {
        Board board = Program.Load("hive.txt");
        var out_dict = Program.Classify(board);
        Assert.AreEqual(1, out_dict["Hive"]);

    }
    [TestMethod]
    public void TestMethod3()
    {
        Board board = Program.Load("flashing.txt");
        var out_dict = Program.Classify(board);
        Assert.AreEqual(1, out_dict["Flashing"]);
    }
    [TestMethod]
    public void TestMethod4()
    {
        Board board = Program.Load("caravay.txt");
        var out_dict = Program.Classify(board);
        Assert.AreEqual(1, out_dict["Caravay"]);
    }
    [TestMethod]
    public void TestMethod5()
    {
        Board board = Program.Load("box.txt");
        var out_dict = Program.Classify(board);
        Assert.AreEqual(1, out_dict["Box"]);
    }
    [TestMethod]
    public void TestMethod6()
    {
        Board board = Program.Load("block.txt");
        var out_dict = Program.Classify(board);
        Assert.AreEqual(1, out_dict["Block"]);
    }
    [TestMethod]
    public void TestMethod7()
    {
        Board board = Program.Load("smt.txt");
        var out_dict = Program.Classify(board);
        int sum = 0;
        foreach (string key in out_dict.Keys) {
            sum+=out_dict[key];
        }
        Assert.AreEqual(0, sum);
    }
    [TestMethod]
    public void TestMethod8()
    {
        Board board = Program.Load("smeared_block.txt");
        var out_dict = Program.Classify(board);
        Assert.AreEqual(1, out_dict["Block"]);
    }
    [TestMethod]
    public void TestMethod9()
    {   
        Board board = Program.Load("smeared_box.txt");
        var out_dict = Program.Classify(board);
        Assert.AreEqual(1, out_dict["Box"]);
    }
    [TestMethod]
    public void TestMethod10()
    {
        Board board = Program.Load("smeared_caravay.txt");
        var out_dict = Program.Classify(board);
        Assert.AreEqual(1, out_dict["Caravay"]);
    }
    [TestMethod]
    public void TestMethod11()
    {
        Board board = Program.Load("smeared_hive.txt");
        var out_dict = Program.Classify(board);
        Assert.AreEqual(1, out_dict["Hive"]);
    }
    [TestMethod]
    public void TestMethod12()
    {
        Board board = Program.Load("smeared_flashing.txt");
        var out_dict = Program.Classify(board);
        Assert.AreEqual(1, out_dict["Flashing"]);
    }
    [TestMethod]
    public void TestMethod13()
    {
        Board board = Program.Load("7_alive_celles.txt");
        int num_of_alive_celles = board.Number_of_alive_cells(); 
        Assert.AreEqual(7, num_of_alive_celles);
    }
    [TestMethod]
    public void TestMethod14()
    {
        Board board = Program.Load("0_alive_celles.txt");
        int num_of_alive_celles = board.Number_of_alive_cells(); 
        Assert.AreEqual(0, num_of_alive_celles);
    }
    [TestMethod]
    public void TestMethod15()
    {
        Board board = Program.Load("3_alive_celles.txt");
        board.Advance();
        int num_of_alive_celles = board.Number_of_alive_cells(); 
        Assert.AreEqual(0, num_of_alive_celles);
    }
}