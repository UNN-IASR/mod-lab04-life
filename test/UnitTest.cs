using Xunit;
using LifeSimulation;
using System.IO;
using System.Text.Json;

namespace SimulationTests
{
    public class SimulationTests
    {
        [Fact]
        public void Initialize_ValidFilePathAndSettingsPath_ReturnsCorrectGridSize()
        {
            Directory.SetCurrentDirectory(@"C:\1\Lab4\Lab4Test");
            // Arrange
            string settingsPath = @"User settings chd\user_settings.json";
            string filePath = "User settings chd/example1.txt";
            var simulation = new Simulation.Simulation();

            // Act
            int CellSize = simulation.Initialize(filePath, settingsPath);

            // Assert
            Assert.True(CellSize == 0);
        }
        [Fact]
        public void ReturnsCorrectIterationsize()
        {
            Directory.SetCurrentDirectory(@"C:\1\Lab4\Lab4_Test");
            // Arrange
            string settingsPath = @"User settings chd\User settings chd.json";
            string filePath = "User settings chd\User settings chd.json";
            var simulation = new Simulation.Simulation();

            // Act
            var IterSize = simulation.Run(filePath, settingsPath);

            // Assert
            Assert.True(IterSize.Iters == 47);
        }
       
        [Fact]
        public void Run_ValidFilePathAndSettingsPath_ReturnsCorrectNumberOfAliveCells()
        {
            Directory.SetCurrentDirectory(@"C:\1\Lab4\Lab4_Test");
            // Arrange
            string settingsPath = @"User settings chd\User settings chd.json";
            string filePath = "User settings chd/example1.txt";
            var simulation = new Simulation.Simulation();

            // Act
            var cells = simulation.Run(filePath, settingsPath);
            
            // Assert
            Assert.True(cells.aliveCells > 0);
        }

        [Fact]
        public void SaveToFile_SavesBoardToCorrectFile()
        {
            Directory.SetCurrentDirectory(@"C:\1\Lab4\Lab4_Test");
            // Arrange
            string settingsPath = @"User settings chd\User settings chd.json";
            string filePath = "User settings chd/example1.txt";
            var simulation = new Simulation.Simulation();
            var savefile = "User settings chd/result1.txt";
            // Act
            simulation.Run(filePath, settingsPath);
            simulation.SaveToFile(savefile);

            // Assert
            Assert.True(File.Exists("User settings chd/result1.txt"));
        }
    }
}
