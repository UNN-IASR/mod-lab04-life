using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckJson()
        {
            Board board = Json.Change_setting("setting.json");
            Board expected_board = new Board(5, 5, 1, 0.5);
            Assert.AreEqual(expected_board, board);
        }

        [TestMethod]
        public void CheckRead_Cell_0_0()
        {
            string file = "block.txt";
            Board board = TextFile.Read_File(file);
            Assert.IsTrue(board.Cells[0, 0].IsAlive);
        }

        [TestMethod]
        public void CheckRead_Cell_0_1()
        {
            string file = "block.txt";
            Board board = TextFile.Read_File(file);
            Assert.IsTrue(board.Cells[0, 1].IsAlive);
        }

        [TestMethod]
        public void CheckRead_Cell_1_0()
        {
            string file = "block.txt";
            Board board = TextFile.Read_File(file);
            Assert.IsTrue(board.Cells[1, 0].IsAlive);
        }

        [TestMethod]
        public void CheckRead_Cell_1_1()
        {
            string file = "block.txt";
            Board board = TextFile.Read_File(file);
            Assert.IsTrue(board.Cells[1, 1].IsAlive);
        }

        [TestMethod]
        public void CheckSave_Cell_0_0()
        {
            string file = "test_save.txt";
            Board board = new Board(2, 2, 1, 0.5);
            board.Cells[0, 0].IsAlive = true;
            TextFile.Save_File(file, board);

            Board board_expected = TextFile.Read_File(file);
            Assert.AreEqual(board_expected.Cells[0, 0].IsAlive, board.Cells[0, 0].IsAlive);
        }

        [TestMethod]
        public void CheckSave_Cell_0_1()
        {
            string file = "test_save.txt";
            Board board = new Board(2, 2, 1, 0.5);
            board.Cells[0, 1].IsAlive = false;
            TextFile.Save_File(file, board);

            Board board_expected = TextFile.Read_File(file);
            Assert.AreEqual(board_expected.Cells[0, 1].IsAlive, board.Cells[0, 1].IsAlive);
        }

        [TestMethod]
        public void CheckSave_Cell_1_0()
        {
            string file = "test_save.txt";
            Board board = new Board(2, 2, 1, 0.5);
            board.Cells[1, 0].IsAlive = true;
            TextFile.Save_File(file, board);

            Board board_expected = TextFile.Read_File(file);
            Assert.AreEqual(board_expected.Cells[1, 0].IsAlive, board.Cells[1, 0].IsAlive);
        }

        [TestMethod]
        public void CheckSave_Cell_1_1()
        {
            string file = "test_save.txt";
            Board board = new Board(2, 2, 1, 0.5);
            board.Cells[1, 1].IsAlive = false;
            TextFile.Save_File(file, board);

            Board board_expected = TextFile.Read_File(file);
            Assert.AreEqual(board_expected.Cells[1, 1].IsAlive, board.Cells[1, 1].IsAlive);
        }

        [TestMethod]
        public void CheckModeling()
        {
            string file = "block.txt";
            Board board = TextFile.Read_File(file);
            board.Advance();

            Board board_expected = new Board(2, 2, 1, 0.5);
            board_expected.Cells[0, 0].IsAlive = false;
            board_expected.Cells[0, 1].IsAlive = false;
            board_expected.Cells[1, 0].IsAlive = false;
            board_expected.Cells[1, 1].IsAlive = false;

            Assert.AreEqual(board_expected.Cells[0, 0].IsAlive, board.Cells[0, 0].IsAlive);
            Assert.AreEqual(board_expected.Cells[0, 1].IsAlive, board.Cells[0, 1].IsAlive);
            Assert.AreEqual(board_expected.Cells[1, 0].IsAlive, board.Cells[1, 0].IsAlive);
            Assert.AreEqual(board_expected.Cells[1, 1].IsAlive, board.Cells[1, 1].IsAlive);
        }

        [TestMethod]
        public void CheckCountIsAlive()
        {
            string file = "boat.txt";
            Board board = TextFile.Read_File(file);
            int count_isAlive = board.Count_isAlive();
            int expected_count = 5;
            Assert.AreEqual(expected_count, count_isAlive);
        }

        [TestMethod]
        public void CheckTypeFigures_ReverseBoat()
        {
            string file1 = "reverse_boat.txt";
            Board board1 = TextFile.Read_File(file1);
            string type1 = TextFile.Type_Figures(file1, board1);
            string expected_type1 = "boat";
            Assert.AreEqual(expected_type1, type1);
        }

        [TestMethod]
        public void CheckTypeFigures_TestSave()
        {
            string file2 = "test_save.txt";
            Board board2 = TextFile.Read_File(file2);
            string type2 = TextFile.Type_Figures(file2, board2);
            string expected_type2 = "";
            Assert.AreEqual(expected_type2, type2);
        }

        [TestMethod]
        public void CheckStablePhase()
        {
            string file = "block.txt";
            int step_stable_phase = 0;
            int expected_count = 0;
            Board board = TextFile.Read_File(file);
            Board expected_board = new Board(2, 2, 1, 0.5);

            for (int step = 0; step < 10; step++)
            {
                board.Advance();
                if (board.Check_stable_phase(expected_board.Cells) == true) step_stable_phase++;
            }

            Assert.AreEqual(expected_count, step_stable_phase);
        }

        [TestMethod]
        public void CheckSimmetry_Pond()
        {
            string file1 = "pond.txt";
            Board board1 = TextFile.Read_File(file1);
            Assert.AreEqual(true, board1.Check_Simmetri_OX());
            Assert.AreEqual(true, board1.Check_Simmetri_OY());
        }

        [TestMethod]
        public void CheckSimmetry_Boat()
        {
            string file2 = "boat.txt";
            Board board2 = TextFile.Read_File(file2);
            Assert.AreEqual(false, board2.Check_Simmetri_OX());
            Assert.AreEqual(false, board2.Check_Simmetri_OY());
        }

        [TestMethod]
        public void CheckSimmetry_OxSimmetry()
        {
            string file3 = "ox_simmetry.txt";
            Board board3 = TextFile.Read_File(file3);
            Assert.AreEqual(true, board3.Check_Simmetri_OX());
            Assert.AreEqual(false, board3.Check_Simmetri_OY());
        }

        [TestMethod]
        public void CheckSimmetry_OySimmetry()
        {
            string file4 = "oy_simmetry.txt";
            Board board4 = TextFile.Read_File(file4);
            Assert.AreEqual(false, board4.Check_Simmetri_OX());
            Assert.AreEqual(true, board4.Check_Simmetri_OY());
        }
    }
}
