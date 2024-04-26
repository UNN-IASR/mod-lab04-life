using Life;

namespace TestNewOpportunity
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {
            SettingsMap settingsMap = new SettingsMap(50, 50);
            SettingsRender settingsRender = new SettingsRender();
            string pathSave = "SaveJson/";


            SettingsMap settingsFigureCub = SaveJson<SettingsMap>.LoadFromJSon(pathSave + "FigureCub.txt");
            Board figCub = new Board(settingsFigureCub, new SphereConnect());


            Board board = new Board(settingsMap, new SphereConnect(), 0);
            board.AddFigure(48, 48, settingsFigureCub);

            RenderInScottPlot renderInScott = new RenderInScottPlot();
            renderInScott.SavePict(board, settingsRender, pathSave + "ImageTest3.png");


            double alivePercent = AnalizerMap.AlivePercent(board);
            bool verticalSymmetry = AnalizerMap.VerticalSymmetry(board);
            bool horizontalSymmetry = AnalizerMap.HorizontalSymmetry(board);
            int countInStability = AnalizerMap.StabilitySystem(board, 10);
            int countFigCub = AnalizerMap.Classification(board, figCub);

            Assert.AreEqual(0.0016, alivePercent, 0.0001);
            Assert.IsTrue(verticalSymmetry);
            Assert.IsTrue(horizontalSymmetry);
            Assert.AreEqual(1, countFigCub);
            Assert.AreEqual(1, countInStability);
        }

        [TestMethod]
        public void TestMethod2()
        {
            SettingsMap settingsMap = new SettingsMap(50, 50);
            SettingsRender settingsRender = new SettingsRender();
            string pathSave = "SaveJson/";


            SettingsMap settingsFigureCub = SaveJson<SettingsMap>.LoadFromJSon(pathSave + "FigureCub.txt");
            Board figCub = new Board(settingsFigureCub, new SphereConnect());


            Board board = new Board(settingsMap, new SphereConnect(), 0);
            board.AddFigure(15, 23, settingsFigureCub);
            board.AddFigure(19, 23, settingsFigureCub);
            board.AddFigure(11, 23, settingsFigureCub);

            RenderInScottPlot renderInScott = new RenderInScottPlot();
            renderInScott.SavePict(board, settingsRender, pathSave + "ImageTest4.png");


            double alivePercent = AnalizerMap.AlivePercent(board);
            bool verticalSymmetry = AnalizerMap.VerticalSymmetry(board);
            bool horizontalSymmetry = AnalizerMap.HorizontalSymmetry(board);
            int countFigCub = AnalizerMap.Classification(board, figCub);
            int countInStability = AnalizerMap.StabilitySystem(board, 10);

            Assert.AreEqual(0.0048, alivePercent, 0.0001);
            Assert.IsFalse(verticalSymmetry);
            Assert.IsTrue(horizontalSymmetry);
            Assert.AreEqual(3, countFigCub);
            Assert.AreEqual(1, countInStability);
        }

        [TestMethod]
        public void TestMethod3()
        {
            SettingsMap settingsMap = new SettingsMap(6, 6);
            SettingsRender settingsRender = new SettingsRender();
            string pathSave = "SaveJson/";


            SettingsMap settingsFigureCub = SaveJson<SettingsMap>.LoadFromJSon(pathSave + "FigureCub.txt");
            Board figCub = new Board(settingsFigureCub, new SphereConnect());


            Board board = new Board(settingsMap, new SphereConnect(), 0);
            board.AddFigure(1, 1, settingsFigureCub);

            RenderInScottPlot renderInScott = new RenderInScottPlot();
            renderInScott.SavePict(board, settingsRender, pathSave + "ImageTest5.png");


            double alivePercent = AnalizerMap.AlivePercent(board);
            bool verticalSymmetry = AnalizerMap.VerticalSymmetry(board);
            bool horizontalSymmetry = AnalizerMap.HorizontalSymmetry(board);
            int countInStability = AnalizerMap.StabilitySystem(board, 10);
            int countFigCub = AnalizerMap.Classification(board, figCub);

            Assert.AreEqual(0.125, alivePercent, 0.01);
            Assert.IsTrue(verticalSymmetry);
            Assert.IsTrue(horizontalSymmetry);
            Assert.AreEqual(1, countInStability);
            Assert.AreEqual(1, countFigCub);
        }

        [TestMethod]
        public void TestMethod4()
        {
            SettingsMap settingsMap = new SettingsMap(5, 5);
            SettingsRender settingsRender = new SettingsRender();
            string pathSave = "SaveJson/";


            SettingsMap settingsFigureStick = SaveJson<SettingsMap>.LoadFromJSon(pathSave + "FigureStick.txt");
            Board figStick = new Board(settingsFigureStick, new SphereConnect());


            Board board = new Board(settingsMap, new SphereConnect(), 0);
            board.AddFigure(0, 1, settingsFigureStick);

            RenderInScottPlot renderInScott = new RenderInScottPlot();
            renderInScott.SavePict(board, settingsRender, pathSave + "ImageTest6.png");


            double alivePercent = AnalizerMap.AlivePercent(board);
            bool verticalSymmetry = AnalizerMap.VerticalSymmetry(board);
            bool horizontalSymmetry = AnalizerMap.HorizontalSymmetry(board);
            int countInStability = AnalizerMap.StabilitySystem(board, 10);
            int countFigStick = AnalizerMap.Classification(board, figStick);

            Assert.AreEqual(0.13, alivePercent, 0.01);
            Assert.IsTrue(verticalSymmetry);
            Assert.IsTrue(horizontalSymmetry);
            Assert.AreEqual(2, countInStability);
            Assert.AreEqual(1, countFigStick);
        }
    }
}