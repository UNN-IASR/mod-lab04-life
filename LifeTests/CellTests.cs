namespace cli_life;

[TestClass]
public class CellTests
{
    [TestMethod]
    public void Advance_NoAliveNeighbours_CellDies()
    {
        var cell = new Cell();
        cell.IsAlive = true;
        cell.neighbors.Add(new Cell());

        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.IsFalse(cell.IsAlive);
    }

    [TestMethod]
    public void Advance_ThreeNeighboursAliveCell_CellLives()
    {
        var cell = new Cell();
        var neighbour = new Cell();
        cell.IsAlive = true;
        neighbour.IsAlive = true;
        for (var index = 0; index < 3; index++)
        {
            cell.neighbors.Add(neighbour);
        }

        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.IsTrue(cell.IsAlive);
    }

    [TestMethod]
    public void Advance_ThreeNeighboursDeadCell_NewAliveCell()
    {
        var cell = new Cell();
        var neighbour = new Cell();
        neighbour.IsAlive = true;
        for (var index = 0; index < 3; index++)
        {
            cell.neighbors.Add(neighbour);
        }

        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.IsTrue(cell.IsAlive);
    }

    [TestMethod]
    public void Advance_TooManyNeighboursAliveCell_CellDies()
    {
        var cell = new Cell();
        var neighbour = new Cell();
        cell.IsAlive = true;
        neighbour.IsAlive = true;
        for (var index = 0; index < 8; index++)
        {
            cell.neighbors.Add(neighbour);
        }

        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.IsFalse(cell.IsAlive);
    }

    [TestMethod]
    public void Advance_TooManyNeighboursDeadCell_CellStaysDead()
    {
        var cell = new Cell();
        var neighbour = new Cell();
        neighbour.IsAlive = true;
        for (var index = 0; index < 8; index++)
        {
            cell.neighbors.Add(neighbour);
        }

        cell.DetermineNextLiveState();
        cell.Advance();

        Assert.IsFalse(cell.IsAlive);
    }
}
