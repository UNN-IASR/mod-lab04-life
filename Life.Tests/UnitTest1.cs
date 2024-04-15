namespace Life.Tests;
using cli_life;

[TestClass]
public class UnitTest1
{
    //клетка родилась
    //клетка умерла от перенаселения
    //клетка умерла от одиночества
    //клетки живы в стабильном
    //клетки живы в спинере
    //глайдер катается
    //нормально считает живых
    //нормально считает блоки
    //нормально считает спинеры
    //нормально считает буханки
    //нормально считает улей
    //нормально считает пруд
    //считает коробки
    //считает лодки
    //считает корабли
    //нормально завершает на пустом
    //нормально завершает большой пример

    [TestMethod]
    public void TestMethod1()
    {
        int w = 10;
        int h = 10;
        int cz = 1;
        Board board = new Board(w, h, cz, 1);

        Assert.AreEqual(w, board.Width);
        Assert.AreEqual(h, board.Height);
        Assert.AreEqual(cz, board.CellSize);
    }

    [TestMethod]
    public void TestMethod2()
    {
    }

    [TestMethod]
    public void TestMethod3()
    {
    }

    [TestMethod]
    public void TestMethod4()
    {
    }

    [TestMethod]
    public void TestMethod5()
    {
    }

    [TestMethod]
    public void TestMethod6()
    {
    }

    [TestMethod]
    public void TestMethod7()
    {
    }

    [TestMethod]
    public void TestMethod8()
    {
    }

    [TestMethod]
    public void TestMethod9()
    {
    }

    [TestMethod]
    public void TestMethod10()
    {
    }

    [TestMethod]
    public void TestMethod11()
    {
    }

    [TestMethod]
    public void TestMethod12()
    {
    }

    [TestMethod]
    public void TestMethod13()
    {
    }

    [TestMethod]
    public void TestMethod14()
    {
    }

    [TestMethod]
    public void TestMethod15()
    {
    }
}