using cli_life;

namespace Life_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Cell_IsAlive_IsFalseByDefault()
        {
            var cell = new Cell();
            Assert.IsFalse(cell.IsAlive);
        }

        [TestMethod]
        public void Cell_GetNeighborCount_ReturnsCorrectCount()
        {
            var cell = new Cell();
            var neighbor1 = new Cell();
            var neighbor2 = new Cell();
            neighbor1.IsAlive = true;
            neighbor2.IsAlive = true;
            cell.neighbors.Add(neighbor1);
            cell.neighbors.Add(neighbor2);

            var neighborCount = cell.neighbors.Where(x => x.IsAlive).Count();
            Assert.AreEqual(2, neighborCount);
        }

        [TestMethod]
        public void Cell_DetermineNextLiveState_CorrectlyUpdatesState()
        {
            var cell = new Cell();
            var neighbor1 = new Cell { IsAlive = true };
            var neighbor2 = new Cell { IsAlive = true };
            var neighbor3 = new Cell { IsAlive = true };
            cell.neighbors.Add(neighbor1);
            cell.neighbors.Add(neighbor2);
            cell.neighbors.Add(neighbor3);

            cell.DetermineNextLiveState();

            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        public void Cell_Advance_UpdatesIsAlive()
        {
            var cell = new Cell { IsAlive = true };
            cell.Advance();
            Assert.IsFalse(cell.IsAlive);
        }

        [TestMethod]
        public void Read_Settings_From_File()
        {
            var settings = File.ReadAllText("../../../test_settings.json");
            var board = Program.CreateBoardWithSettings(settings);
            Assert.AreEqual(board.Width, 50);
            Assert.AreEqual(board.Height, 20);
        }
        [TestMethod]
        public void Read_State_From_File()
        {
            var board = new Board(3, 3, 1, 0.5, true);
            Assert.IsTrue(board.Cells[0,0].IsAlive);
            Assert.IsFalse(board.Cells[0, 1].IsAlive);
        }
    }
}