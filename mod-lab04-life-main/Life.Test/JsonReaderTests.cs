using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;

namespace Life.Test {

[TestClass]
public class JsonReaderTests
{
    string solutionRootPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

    [TestMethod]
    public void ReadSettingsTest()
    {
        GameOfLife actual = JsonReader.ReadSettings($"{solutionRootPath}/Life.Test/TestSettings.json");
        GameOfLife expected = new GameOfLife()
        {
            boardWidth = 3,
            boardHeight = 3,
            liveDensity = 1.0,
            liveSymbol = "*",
            deadSymbol = " ",
            iterations = 3,
            delay = 100
        };
        Assert.AreEqual(expected, actual);
    }
}}
