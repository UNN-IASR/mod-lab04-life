using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ClassificationTK;
using PropertyFileController;
using StatusFileController;
using cli_life;


namespace NET
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            LifeProperty aLifeProperty = new LifeProperty(30, 30, 1, 1);
            JSONController.SerializeToJSON(aLifeProperty);

            LifeProperty anOtherLifeProperty = JSONController.DeserializeFromJSON();

            Assert.IsTrue(anOtherLifeProperty.BoardWidth == anOtherLifeProperty.BoardWidth &&
                          anOtherLifeProperty.BoardHeight == anOtherLifeProperty.BoardHeight &&
                          anOtherLifeProperty.LifeDensity == anOtherLifeProperty.LifeDensity &&
                          anOtherLifeProperty.BoardCellSize == anOtherLifeProperty.BoardCellSize);
        }
        [TestMethod]
        public void TestMethod2()
        {
            Board aBoard = new Board(30, 30, 1, 0.5);
            TextController.SaveLifeStatus(aBoard.Cells);

            Cell [,] aCells = new Cell[30,30];
            for(int col = 0; col < 30; ++col)
            {
                for (int row = 0; row < 30; ++row)
                {
                    aCells[col,row] = new Cell();
                }
            }

            TextController.ReadLifeStatus(aCells);

            bool flag = true;

            for(int col = 0; col < 30; ++col)
            {
                for (int row = 0; row < 30; ++row)
                {
                    if (aCells[row, col].IsAlive != aBoard.Cells[row, col].IsAlive)
                    {
                        flag = false;
                    }
                }
            }

            Assert.IsTrue(flag == true);
        }     
        [TestMethod]        
        public void TestMethod3()
        {
            Board aBoard = new Board(20, 20, 1, 0.5);
            TextController.ReadLifeStatus(aBoard.Cells, "Test3.txt");
            aBoard.ConnectNeighbors();

            Assert.IsTrue(Classification.getClassificationBoardState(aBoard)[0] == 1);
        }     
       [TestMethod]      
        public void TestMethod4()
        {
            Board aBoard = new Board(20, 20, 1, 0.5);
            TextController.ReadLifeStatus(aBoard.Cells, "Test4.txt");
            aBoard.ConnectNeighbors();

            Assert.IsTrue(Classification.getClassificationBoardState(aBoard)[1] == 1);
        }     
        [TestMethod]      
        public void TestMethod5()
        {
            Board aBoard = new Board(20, 20, 1, 0.5);
            TextController.ReadLifeStatus(aBoard.Cells, "Test5.txt");
            aBoard.ConnectNeighbors();

            Assert.IsTrue(Classification.getClassificationBoardState(aBoard)[2] == 1);
        }     
        [TestMethod]      
        public void TestMethod6()
        {
            Board aBoard = new Board(20, 20, 1, 0.5);
            TextController.ReadLifeStatus(aBoard.Cells, "Test6.txt");
            aBoard.ConnectNeighbors();

            Assert.IsTrue(Classification.getClassificationBoardState(aBoard)[3] == 1);
        }     
        [TestMethod]      
        public void TestMethod7()
        {
            Board aBoard = new Board(20, 20, 1, 0.5);
            TextController.ReadLifeStatus(aBoard.Cells, "Test7.txt");
            aBoard.ConnectNeighbors();

            Assert.IsTrue(Classification.getClassificationBoardState(aBoard)[4] == 1);
        }                     
        [TestMethod]      
        public void TestMethod8()
        {
            Board aBoard = new Board(20, 20, 1, 0.5);
            TextController.ReadLifeStatus(aBoard.Cells, "Test9.txt");
            aBoard.ConnectNeighbors();

            Assert.IsTrue(Classification.getClassificationBoardState(aBoard)[6] == 1);
        }

        [TestMethod]      
        public void TestMethod9()
        {
            Board aBoard = new Board(20, 20, 1, 0.5);
            TextController.ReadLifeStatus(aBoard.Cells, "Test10.txt");
            aBoard.ConnectNeighbors();

            Assert.IsTrue(Classification.getClassificationBoardState(aBoard)[7] == 1);
        }     

       [TestMethod]      
        public void TestMethod10()
        {
            Board aBoard = new Board(20, 20, 1, 0.5);
            TextController.ReadLifeStatus(aBoard.Cells, "Test11.txt");
            aBoard.ConnectNeighbors();

            Assert.IsTrue(Classification.StabilityPhase(ref aBoard) == 3);
        }

       [TestMethod]   
        public void TestMethod11()
        {
            Board aBoard = new Board(20, 20, 1, 0.5);
            TextController.ReadLifeStatus(aBoard.Cells, "Test12.txt");
            aBoard.ConnectNeighbors();

            Assert.IsTrue(Classification.getSymmetricalElements(aBoard) == 6);
        }     
    }
}
