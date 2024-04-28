using Life;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNewOpportunity
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateGraphic()
        {
            string pathSave = "EvolutionGraphic1.png";
            MapCGL board = new MapCGL(50, 50);

            Random rd = new Random();
            CGL cgl = new CGL(board, new SphereConnect());
            int maxIteration = 100;
            double stepDensity = 0.1;

            ScottPlotGraphic graphics = new Life.ScottPlotGraphic(rd, cgl, maxIteration, stepDensity);
            graphics.GetGraphic(pathSave);
        }

        [TestMethod]
        public void SaveAndLoad()
        {
            string pathSave = "SaveJson/";
            MapCGL figCub = SaveJson.LoadFromJSon(pathSave + "FigureCubeWithoutBord.txt");

            MapCGL board = new MapCGL(50, 50);
            board = FillMap.AddFigure(20, 20, board, figCub);
            board = FillMap.AddFigure(21, 21, board, figCub);
            CGL cgl = new CGL(board, new SphereConnect());

            int countAlive = AnalyzerMap.CountAlive(board);
            Assert.AreEqual(7, countAlive);
            cgl.Advance();
            countAlive = AnalyzerMap.CountAlive(board);
            Assert.AreEqual(4, countAlive);

            SaveJson.SaveToJson(pathSave + "TestSaveAndLoad.txt", "TestSaveAndLoad", board);
            MapCGL loadBoard = SaveJson.LoadFromJSon(pathSave + "TestSaveAndLoad.txt");

            countAlive = AnalyzerMap.CountAlive(loadBoard);
            Assert.AreEqual(4, countAlive);

            cgl = new CGL(loadBoard, new SphereConnect());
            cgl.Advance();

            countAlive = AnalyzerMap.CountAlive(loadBoard);
            Assert.AreEqual(0, countAlive);
        }

        [TestMethod]
        public void MapToString()
        {
            string pathSave = "SaveJson/";
            MapCGL figCub = SaveJson.LoadFromJSon(pathSave + "FigureCub.txt");
            MapCGL figStick = SaveJson.LoadFromJSon(pathSave + "FigureStick.txt");

            MapCGL board = new MapCGL(4, 4);
            board = FillMap.AddFigure(0, 0, board, figCub);

            string str = "0000\n0110\n0110\n0000\n";
            Assert.AreEqual(str, board.ToString());
        }

        [TestMethod]
        public void LoadFromStr()
        {
            string pathSave = "SaveJson/";
            MapCGL figCub = SaveJson.LoadFromJSon(pathSave + "FigureCub.txt");
            MapCGL figStick = SaveJson.LoadFromJSon(pathSave + "FigureStick.txt");

            string str = "0000\n0110\n0110\n0000\n";
            MapCGL board = MapCGL.LoadFromStr(str);

            Assert.AreEqual(str, board.ToString());
        }

        [TestMethod]
        public void RandomFill1()
        {
            double liveDensity = 0.5;
            Random rd = new Random(41);

            MapCGL board = FillMap.FillRandom(new MapCGL(50, 50), liveDensity, rd);
            int countAlive = AnalyzerMap.CountAlive(board);
            double alivePercent = countAlive / (board.Rows * board.Columns);
            Assert.AreEqual(0.4, alivePercent, 0.6);
        }

        [TestMethod]
        public void RandomFill2()
        {
            double liveDensity = 0.2;
            Random rd = new Random(41);

            MapCGL board = FillMap.FillRandom(new MapCGL(50, 50), liveDensity, rd);
            int countAlive = AnalyzerMap.CountAlive(board);
            double alivePercent = countAlive / (board.Rows * board.Columns);
            Assert.AreEqual(0.1, alivePercent, 0.3);
        }

        [TestMethod]
        public void RandomFill3()
        {
            double liveDensity = 1;
            Random rd = new Random(41);

            MapCGL board = FillMap.FillRandom(new MapCGL(50, 50), liveDensity, rd);
            int countAlive = AnalyzerMap.CountAlive(board);
            double alivePercent = countAlive / (board.Rows * board.Columns);
            Assert.AreEqual(1, alivePercent, 1);
        }

        [TestMethod]
        public void Classification1()
        {
            string pathSave = "SaveJson/";
            MapCGL figCub = SaveJson.LoadFromJSon(pathSave + "FigureCub.txt");
            MapCGL figStick = SaveJson.LoadFromJSon(pathSave + "FigureStick.txt");

            MapCGL board = new MapCGL(50, 50);
            board = FillMap.AddFigure(0, 40, board, figCub);
            board = FillMap.AddFigure(30, 20, board, figCub);

            int countFigCub = AnalyzerMap.Classification(board, figCub);

            Assert.AreEqual(2, countFigCub);
        }

        [TestMethod]
        public void Classification2()
        {
            string pathSave = "SaveJson/";
            MapCGL figCub = SaveJson.LoadFromJSon(pathSave + "FigureCub.txt");
            MapCGL figStick = SaveJson.LoadFromJSon(pathSave + "FigureStick.txt");

            MapCGL board = new MapCGL(50, 50);
            board = FillMap.AddFigure(0, 40, board, figCub);
            board = FillMap.AddFigure(30, 20, board, figCub);
            board = FillMap.AddFigure(50, 50, board, figStick);

            int countFigCub = AnalyzerMap.Classification(board, figCub);
            int countFigStick = AnalyzerMap.Classification(board, figStick);

            Assert.AreEqual(2, countFigCub);
            Assert.AreEqual(1, countFigStick);
        }

        [TestMethod]
        public void CountLive1()
        {
            string pathSave = "SaveJson/";
            MapCGL figCub = SaveJson.LoadFromJSon(pathSave + "FigureCub.txt");
            MapCGL figStick = SaveJson.LoadFromJSon(pathSave + "FigureStick.txt");

            MapCGL board = new MapCGL(50, 50);
            board = FillMap.AddFigure(50, 50, board, figStick);
            board = FillMap.AddFigure(19, 23, board, figCub);
            board = FillMap.AddFigure(11, 23, board, figCub);

            int countAlive = AnalyzerMap.CountAlive(board);
            Assert.AreEqual(11, countAlive);

            CGL cgl = new CGL(board, new SphereConnect());
            cgl.Advance();

            countAlive = AnalyzerMap.CountAlive(board);
            Assert.AreEqual(11, countAlive);
        }

        [TestMethod]
        public void CountLive2()
        {
            string pathSave = "SaveJson/";
            MapCGL figCub = SaveJson.LoadFromJSon(pathSave + "FigureCubeWithoutBord.txt");

            MapCGL board = new MapCGL(50, 50);
            board = FillMap.AddFigure(20, 20, board, figCub);
            board = FillMap.AddFigure(21, 21, board, figCub);
            CGL cgl = new CGL(board, new SphereConnect());

            int countAlive = AnalyzerMap.CountAlive(board);
            Assert.AreEqual(7, countAlive);

            cgl.Advance();
            countAlive = AnalyzerMap.CountAlive(board);
            Assert.AreEqual(4, countAlive);

            cgl.Advance();
            countAlive = AnalyzerMap.CountAlive(board);
            Assert.AreEqual(0, countAlive);
        }


        [TestMethod]
        public void Stability1()
        {
            string pathSave = "SaveJson/";
            MapCGL figCub = SaveJson.LoadFromJSon(pathSave + "FigureCub.txt");

            MapCGL board = new MapCGL(50, 50);
            board = FillMap.AddFigure(15, 23, board, figCub);
            board = FillMap.AddFigure(19, 23, board, figCub);
            board = FillMap.AddFigure(11, 23, board, figCub);

            int countInStability = AnalyzerMap.StabilitySystem(new CGL(board, new SphereConnect()), 10);

            Assert.AreEqual(1, countInStability);
        }

        [TestMethod]
        public void Stability2()
        {
            string pathSave = "SaveJson/";
            MapCGL figCub = SaveJson.LoadFromJSon(pathSave + "FigureCub.txt");

            MapCGL board = new MapCGL(50, 50);

            int countInStability = AnalyzerMap.StabilitySystem(new CGL(board, new SphereConnect()), 10);

            Assert.AreEqual(1, countInStability);
        }

        [TestMethod]
        public void Stability3()
        {
            string pathSave = "SaveJson/";
            MapCGL figStick = SaveJson.LoadFromJSon(pathSave + "FigureStick.txt");

            MapCGL board = new MapCGL(5, 5);
            board = FillMap.AddFigure(0, 1, board, figStick);

            int countInStability = AnalyzerMap.StabilitySystem(new CGL(board, new SphereConnect()), 10);

            Assert.AreEqual(2, countInStability);
        }

        [TestMethod]
        public void Stability4()
        {
            string pathSave = "SaveJson/";
            MapCGL figCub = SaveJson.LoadFromJSon(pathSave + "FigureCubeWithoutBord.txt");

            MapCGL board = new MapCGL(50, 50);
            board = FillMap.AddFigure(20, 20, board, figCub);
            board = FillMap.AddFigure(21, 21, board, figCub);
            CGL cgl = new CGL(board, new SphereConnect());

            int countInStability = AnalyzerMap.StabilitySystem(cgl, 100);
            Assert.AreEqual(-1, countInStability);
        }
    }
}