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
        public void Cell_IsAliveNext_IsFalseByDefault()
        {
            var cell = new Cell();
            Assert.IsFalse(cell.IsAliveNext);
        }

        [TestMethod]
        public void Neighbours_Are_Added_Correctly()
        {
            var cell = new Cell { IsAlive = true };
            var neighbor = new Cell { IsAlive = true };
            cell.neighbors.Add(neighbor);
            cell.neighbors.Add(neighbor);
            cell.neighbors.Add(neighbor);
            cell.Advance();
            Assert.AreEqual(cell.neighbors.Count, 3);
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
        public void Cell_DetermineNextLiveState_CorrectlyUpdatesState1()
        {
            var cell = new Cell { IsAlive = true };
            var neighbor = new Cell { IsAlive = true };
            cell.neighbors.Add(neighbor);
            cell.neighbors.Add(neighbor);
            cell.neighbors.Add(neighbor);

            cell.DetermineNextLiveState();

            Assert.IsTrue(cell.IsAliveNext);
        }
        [TestMethod]
        public void Cell_DetermineNextLiveState_CorrectlyUpdatesState2()
        {
            var cell = new Cell { IsAlive = true };
            var neighbor = new Cell { IsAlive = true };
            cell.neighbors.Add(neighbor);
            cell.neighbors.Add(neighbor);

            cell.DetermineNextLiveState();

            Assert.IsTrue(cell.IsAliveNext);
        }
        [TestMethod]
        public void Cell_DetermineNextLiveState_CorrectlyUpdatesState3()
        {
            var cell = new Cell { IsAlive = true };
            var neighbor = new Cell { IsAlive = true };
            cell.neighbors.Add(neighbor);

            cell.DetermineNextLiveState();

            Assert.IsFalse(cell.IsAliveNext);
        }
        [TestMethod]
        public void Cell_DetermineNextLiveState_CorrectlyUpdatesState4()
        {
            var cell = new Cell { IsAlive = true };
            var neighbor = new Cell { IsAlive = true };
            cell.neighbors.Add(neighbor);
            cell.neighbors.Add(neighbor);
            cell.neighbors.Add(neighbor);
            cell.neighbors.Add(neighbor);

            cell.DetermineNextLiveState();

            Assert.IsFalse(cell.IsAliveNext);
        }

        [TestMethod]
        public void Cell_Advance_UpdatesIsAlive_To_False_With_No_Neigbours()
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

        [TestMethod]
        public void Board_Is_Created_Correctly_With_Args()
        {
            var board = new Board(3, 3, 1, 0.5, true);
            Assert.AreEqual(board.Width, 3);
            Assert.AreEqual(board.Width, 3);
        }

        [TestMethod]
        public void Board_Is_Created_Correctly_Without_useState_Arg()
        {
            var board = new Board(3, 3, 1, 0.5);
            Assert.AreEqual(board.Width, 3);
            Assert.AreEqual(board.Width, 3);
        }

        [TestMethod]
        public void Board_Has_Alive_Cells()
        {
            var board = new Board(3, 3, 1, 0.5, true);
            Assert.AreNotEqual(board.AliveCellsCount(), 0);
        }

        [TestMethod]
        public void Board_RecordPosition_Works_Correctly()
        {
            var board = new Board(3, 3, 1, 0.5, true);
            string str = board.RecordPosition(board);
            int count = str.Count(c => c == '1');
            Assert.AreEqual(board.AliveCellsCount(), count);
        }
    }
}