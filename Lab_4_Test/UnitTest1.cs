using Xunit;
using LifeSimulation;
using System.IO;
using System.Text.Json;

namespace LifeSimulationTests
{
    public class LifeSimulationTests
    {
        [Fact]
        public void Initialize_ValidFilePathAndSettingsPath_ReturnsCorrectGridSize()
        {
            Directory.SetCurrentDirectory(@"C:\Users\rshah\Desktop\4 курс\2 семестр\Shtanyk\Lab_4\Lab_4_Test");
            // Arrange
            string settingsPath = @"setting\\user_settings.json";
            string filePath = "setting/example1.txt";
            var simulation = new LifeSimulation.LifeSimulation();

            // Act
            int gridSize = simulation.Initialize(filePath, settingsPath);

            // Assert
            Assert.True(gridSize == 0);
        }
        [Fact]
        public void ReturnsCorrectIterationsize()
        {
            Directory.SetCurrentDirectory(@"C:\Users\rshah\Desktop\4 курс\2 семестр\Shtanyk\Lab_4\Lab_4_Test");
            // Arrange
            string settingsPath = @"setting\\user_settings.json";
            string filePath = "setting/example1.txt";
            var simulation = new LifeSimulation.LifeSimulation();

            // Act
            var IterSize = simulation.Run(filePath, settingsPath);

            // Assert
            Assert.True(IterSize.Iters == 47);
        }
       
        [Fact]
        public void Run_ValidFilePathAndSettingsPath_ReturnsCorrectNumberOfAliveCells()
        {
            Directory.SetCurrentDirectory(@"C:\Users\rshah\Desktop\4 курс\2 семестр\Shtanyk\Lab_4\Lab_4_Test");
            // Arrange
            string settingsPath = @"setting\user_settings.json";
            string filePath = "setting/example4.txt";
            var simulation = new LifeSimulation.LifeSimulation();

            // Act
            var cells = simulation.Run(filePath, settingsPath);
            
            // Assert
            Assert.True(cells.aliveCells > 0);
        }

        [Fact]
        public void SaveToFile_SavesGridToCorrectFile()
        {
            Directory.SetCurrentDirectory(@"C:\Users\rshah\Desktop\4 курс\2 семестр\Shtanyk\Lab_4\Lab_4_Test");
            // Arrange
            string settingsPath = @"setting\user_settings.json";
            string filePath = "setting/example1.txt";
            var simulation = new LifeSimulation.LifeSimulation();
            var savefile = "setting/result4.txt";
            // Act
            simulation.Run(filePath, settingsPath);
            simulation.SaveToFile(savefile);

            // Assert
            Assert.True(File.Exists("setting/result4.txt"));
        }
    }
}