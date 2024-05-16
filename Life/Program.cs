using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Text.RegularExpressions;
using ScottPlot.Plottables;
using System.Runtime.CompilerServices;
using ScottPlot;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;

        public readonly List<Cell> neighbours = new();

        private bool IsAliveNext;

        public bool DetermineNextLiveState()
        {
            int liveNeighbors = neighbours.Where(x => x.IsAlive).Count();

            if (IsAlive)
            {
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            }
            else
            {
                IsAliveNext = liveNeighbors == 3;
            }

            return IsAliveNext;
        }

        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        private int _width;

        private int _heigth;

        public Cell[,] Cells;

        public int CellSize;

        public int Columns { get { return Cells.GetLength(1); } }

        public int Rows { get { return Cells.GetLength(0); } }

        public int Width
        {
            get
            {
                return _width;
            }
            private set
            {
                _width = value;
            }
        }

        public int Height
        {
            get
            {
                return _heigth;
            }
            private set
            {
                _heigth = value;
            }
        }

        public int Generation { get; set; }

        public Board(BoardConfigurations configurations)
        {
            Generation = 0;

            CellSize = configurations.CellSize;

            Cells = new Cell[configurations.Height / CellSize, configurations.Width / CellSize];

            Width = Columns * CellSize;
            Height = Rows * CellSize;

            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    Cells[row, col] = new Cell();
                }
            }

            ConnectNeighbours();
            Randomize(configurations.LiveDensity);
        }

        public Board(BoardConfigurations configurations, int generation, Cell[,] cells)
        {
            Generation = generation;

            CellSize = configurations.CellSize;

            Cells = cells;

            Width = configurations.Width;
            Height = configurations.Height;

            ConnectNeighbours();
        }

        public Board(double density = 0.5) : this(new BoardConfigurations(100, 50, 1, density))
        {
        }

        public void Randomize(double liveDensity)
        {
            var rand = new Random();

            foreach (var cell in Cells)
            {
                cell.IsAlive = rand.NextDouble() < liveDensity;
            }
        }

        public void Advance()
        {
            foreach (var cell in Cells)
            {
                cell.DetermineNextLiveState();
            }

            foreach (var cell in Cells)
            {
                cell.Advance();
            }

            Generation++;
        }

        private void ConnectNeighbours()
        {
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Columns; col++)
                {
                    var rowBottom = (row > 0)
                        ? row - 1
                        : Rows - 1;
                    var rowTop = (row < Rows - 1)
                        ? row + 1
                        : 0;

                    var colLeft = (col > 0)
                        ? col - 1
                        : Columns - 1;
                    var colRight = (col < Columns - 1)
                        ? col + 1
                        : 0;

                    Cells[row, col].neighbours.Add(Cells[rowTop, colLeft]);
                    Cells[row, col].neighbours.Add(Cells[rowTop, col]);
                    Cells[row, col].neighbours.Add(Cells[rowTop, colRight]);
                    Cells[row, col].neighbours.Add(Cells[row, colLeft]);
                    Cells[row, col].neighbours.Add(Cells[row, colRight]);
                    Cells[row, col].neighbours.Add(Cells[rowBottom, colLeft]);
                    Cells[row, col].neighbours.Add(Cells[rowBottom, col]);
                    Cells[row, col].neighbours.Add(Cells[rowBottom, colRight]);
                }
            }
        }
    }

    public class Entity
    {
        public string Identifier { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string EntityDesign { get; set; }

        public Entity(string identifier, int width, int height, string entityDesign)
        {
            Identifier = identifier;
            Width = width;
            Height = height;
            EntityDesign = entityDesign;
        }

        public Entity() : this(string.Empty, 0, 0, string.Empty) { }

        public override bool Equals(object obj)
        {
            if (obj is Entity entity)
            {
                return entity.Width == Width
                && entity.Height == Height
                && entity.EntityDesign == EntityDesign;
            }
            return false;
        }
    }

    public class BoardConfigurations
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }

        public BoardConfigurations(int width, int height, int cellSize, double liveDensity = 0.5)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            LiveDensity = liveDensity;
        }

        public BoardConfigurations() : this(0, 0, 0, 0) { }
    }

    public class DocumentHandler
    {
        private static T LoadJsonFormat<T>(string pathToFile)
        {
            var jsonFormat = File.ReadAllText(pathToFile);
            return JsonSerializer.Deserialize<T>(jsonFormat);
        }

        public static BoardConfigurations LoadBoardConfiguration(string pathToFile)
            => LoadJsonFormat<BoardConfigurations>(pathToFile);

        public static void SaveToJsonFormat<T>(T data, string pathToFile, bool writeIndented = false)
        {
            var jsonFormat = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = writeIndented });
            File.WriteAllText(pathToFile, jsonFormat);
        }

        public static void SaveBoardConfiguration(BoardConfigurations configurations, string pathToFile)
            => SaveToJsonFormat(configurations, pathToFile);

        public static void SaveBoardCondition(Board board, string pathToFile)
        {
            StringBuilder boardCondition = new StringBuilder();
            for (var row = 0; row < board.Height; row++)
            {
                for (var col = 0; col < board.Width; col++)
                {
                    boardCondition.Append(
                       board.Cells[row, col].IsAlive
                       ? '*'
                       : ' ');
                }
                boardCondition.Append('\n');
            }
            boardCondition.AppendLine($"cellSize={board.CellSize}")
                       .AppendLine($"generation={board.Generation}");

            File.WriteAllText(pathToFile, boardCondition.ToString());
        }

        public static Board LoadBoardCondition(string pathToFile)
        {
            var boardStringArrayRepr = File.ReadAllLines(pathToFile);

            int.TryParse(
                boardStringArrayRepr[boardStringArrayRepr.Length - 2]
                    .Substring(
                        boardStringArrayRepr[boardStringArrayRepr.Length - 2]
                        .IndexOf('=') + 1),
                out int cellSize);

            int.TryParse(
                boardStringArrayRepr[boardStringArrayRepr.Length - 1]
                    .Substring(
                        boardStringArrayRepr[boardStringArrayRepr.Length - 1]
                        .IndexOf('=') + 1),
                out int generation);

            cellSize = cellSize > 1
                ? cellSize
                : 1;

            generation = generation > 0
                ? generation
                : 0;

            var height = boardStringArrayRepr.Length - 2;
            var width = boardStringArrayRepr[0].Length;

            var configurations = new BoardConfigurations(width, height, cellSize);

            var cells = new Cell[height / cellSize, width / cellSize];

            for (var row = 0; row < height; row += cellSize)
            {
                for (var col = 0; col < width; col += cellSize)
                {
                    cells[row / cellSize, col / cellSize] = new Cell
                    {
                        IsAlive = (boardStringArrayRepr[row][col] == '*')
                    };
                }
            }

            return new Board(configurations, generation, cells);
        }

        public static Entity[] LoadEntities(string pathToFile)
            => LoadJsonFormat<Entity[]>(pathToFile);

        public static void SaveEntities(Entity[] entities, string pathToFile)
            => SaveToJsonFormat(entities, pathToFile, true);

        public static void SavePlotPng(Plot plot, string pathToFile, int width = 1920, int heigth = 1080)
            => plot.SavePng(pathToFile, 1920, 1080);
    }
    public class TableEvaluationStrategy
    {
        public static Entity[] Entities;

        private Board _board;

        public TableEvaluationStrategy(Board board)
        {
            _board = board;
        }

        public int FindAliveCellsCount()
        {
            return FindAliveCellsCount(_board);
        }

        public static int FindAliveCellsCount(Board board)
        {
            var result = 0;

            foreach (var cell in board.Cells)
            {
                if (cell.IsAlive)
                {
                    result++;
                }
            }

            return result;
        }

        private Entity CreateEntity(int row, int col, int width, int height)
        {
            string entityDesign = GenerateEntityDesign(row, col, width, height);

            return new Entity("entity", width, height, entityDesign);
        }

        private string GenerateEntityDesign(int row, int col, int width, int height)
        {
            return string.Join("",
                from rowIndex in Enumerable.Range(row, height)
                from colIndex in Enumerable.Range(col, width)
                select GetEntityCell(rowIndex, colIndex));
        }

        private char GetEntityCell(int rowIndex, int colIndex)
        {
            return IsInsideBoard(rowIndex, colIndex)
                ? (_board.Cells[rowIndex, colIndex].IsAlive ? '*' : ' ')
                : ' ';
        }

        private bool IsInsideBoard(int rowIndex, int colIndex)
        {
            return rowIndex >= 0 && rowIndex < _board.Rows
                && colIndex >= 0 && colIndex < _board.Columns;
        }

        public Dictionary<string, int> GetEntitiesCount()
        {
            Dictionary<string, int> entitiesCount = InstantiateEntitiesDictionary();

            for (int row = 0; row < _board.Rows; row++)
            {
                for (int col = 0; col < _board.Columns; col++)
                {
                    UpdateEntitiesCount(entitiesCount, row, col);
                }
            }

            return entitiesCount;
        }

        private Dictionary<string, int> InstantiateEntitiesDictionary()
        {
            return Entities.ToDictionary(entity => entity.Identifier, _ => 0);
        }

        private void UpdateEntitiesCount(Dictionary<string, int> entitiesCount, int row, int col)
        {
            var entityToIncrement = Entities
                .FirstOrDefault(entity => entity.Equals(CreateEntity(row, col, entity.Width, entity.Height)));

            if (entityToIncrement != null)
            {
                entitiesCount[entityToIncrement.Identifier]++;
            }
        }
    }

    class Program
    {

        const string ConfigurationsJsonPath = "../../../boardConfigurations.json";

        const string BoardConditionPathToFile = "../../../resultGameOfLife.txt";

        const string EntitiesJsonPath = "../../../entities.json";

        const string PlotPngPath = "../../../plot.png";

        static int IterationsNum = 50;

        static bool LoadFromFile = false;

        static bool SaveToFile = false;

        static Board SimulationBoard;
        static TableEvaluationStrategy TableEvaluationStrategy;
        static BoardConfigurations Configurations = new BoardConfigurations(50, 20, 1, 0.5);



        static void ConfigureSimulationStart()
        {
            Console.Write("The number of iterations of the simulation: ");
            IterationsNum = ParseNumericInput();

            Console.Write("Download from a file? (1 - yes, 0 - no): ");
            LoadFromFile = ParseYesNoInput();

            Console.Write("Save the result to a file? (1 - yes, 0 - no): ");
            SaveToFile = ParseYesNoInput();
        }

        static int ParseNumericInput()
        {
            int.TryParse(Console.ReadLine(), out int userResponse);
            return userResponse;
        }

        static bool ParseYesNoInput()
        {
            var userResponse = ParseNumericInput();
            return userResponse != 0;
        }

        static void StandardSimulationProcedure()
        {
            ConfigureSimulationStart();
            Reset();

            TableEvaluationStrategy.Entities = DocumentHandler.LoadEntities(EntitiesJsonPath);

            LoadBoardCondition();

            ExecuteGameCycle();

            SaveBoardCondition();
        }

        private static void LoadBoardCondition()
        {
            if (!LoadFromFile) return;

            try
            {
                SimulationBoard = DocumentHandler.LoadBoardCondition(BoardConditionPathToFile);
                TableEvaluationStrategy = new(SimulationBoard);
                Console.WriteLine("Successfully reading data from a file.");
            }
            catch
            {
                Console.WriteLine("Error when reading data from a file.");
                Reset();
            }

            Thread.Sleep(2000);
        }

        private static void ExecuteGameCycle()
        {
            for (var index = 0; index < IterationsNum; index++)
            {
                Console.Clear();
                Render();
                ShowGameStatistics();
                SimulationBoard.Advance();
                Thread.Sleep(1000);
            }
        }

        private static void ShowGameStatistics()
        {
            Console.WriteLine($"Current generation: {SimulationBoard.Generation}");
            Console.WriteLine($"The number of living cells: {TableEvaluationStrategy.FindAliveCellsCount()}");
            Console.WriteLine("The number of each of the entities:");
            foreach (var entity in TableEvaluationStrategy.GetEntitiesCount())
            {
                Console.WriteLine($"Entity {entity.Key}: Count {entity.Value}");
            }
        }

        private static void SaveBoardCondition()
        {
            if (!SaveToFile) return;

            try
            {
                DocumentHandler.SaveBoardCondition(SimulationBoard, BoardConditionPathToFile);
                Console.WriteLine("Successfully saving the result to a file.");
            }
            catch
            {
                Console.WriteLine("Error when saving the result to a file.");
            }
        }

        static void AnalysisSimulation()
        {
            var plot = SetupGraph();

            double[] density = CreateVitalityDensitySequence();

            var generations = CreateGenerationsCollection();
            var configurations = new BoardConfigurations(Configurations.Width, Configurations.Height, Configurations.CellSize);

            for (int boardIndex = 0; boardIndex < density.Length; boardIndex++)
            {
                Board board = CreateNewBoard(density, boardIndex, configurations);
                TableEvaluationStrategy tableEvaluationStrategy = new TableEvaluationStrategy(board);

                List<int> aliveCells = CalculateAliveCells(board, tableEvaluationStrategy);

                AddScatterToPlot(plot, generations, aliveCells, density, boardIndex);
            }

            DocumentHandler.SavePlotPng(plot, PlotPngPath);
        }

        private static Plot SetupGraph()
        {
            var plot = new Plot();
            plot.XLabel("Generation");
            plot.YLabel("Living cells");
            plot.ShowLegend();
            return plot;
        }

        private static double[] CreateVitalityDensitySequence() => new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };

        private static List<int> CreateGenerationsCollection() => Enumerable.Repeat(0, IterationsNum).Select((index, generation) => generation + index).ToList();

        private static Board CreateNewBoard(double[] density, int boardIndex, BoardConfigurations configurations)
        {
            configurations.LiveDensity = density[boardIndex];
            return new Board(configurations);
        }

        private static List<int> CalculateAliveCells(Board board, TableEvaluationStrategy tableEvaluationStrategy)
        {
            List<int> aliveCells = new();
            int generation = 0;

            while (generation < IterationsNum)
            {
                aliveCells.Add(tableEvaluationStrategy.FindAliveCellsCount());
                board.Advance();
                generation++;
            }

            return aliveCells;
        }

        private static void AddScatterToPlot(Plot plot, List<int> generations, List<int> aliveCells, double[] density, int boardIndex)
        {
            var rand = new Random();
            Scatter scatter = plot.Add.Scatter(generations, aliveCells);
            scatter.LegendText = density[boardIndex].ToString();
            scatter.LineWidth = 3;
            scatter.Color = new(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
        }

        static void RetrieveGameplayMode()
        {
            Console.Write("To run: \n1. Simulation\n2. Plotting\nChoice: ");
            int userChoice = ParseNumericInput();

            switch (userChoice)
            {
                case 1:
                    StandardSimulationProcedure();
                    break;
                case 2:
                    AnalysisSimulation();
                    break;
                default:
                    Console.WriteLine("A non-existent option is selected");
                    RetrieveGameplayMode(); // повторный запрос типа симуляции у пользователя при некорректном вводе
                    break;
            }
        }
        static private void Reset()
        {
            try
            {
                var configurations = DocumentHandler.LoadBoardConfiguration(ConfigurationsJsonPath);

                SimulationBoard = new Board(configurations);
            }
            catch
            {
                SimulationBoard = new Board(Configurations);
            }

            TableEvaluationStrategy = new(SimulationBoard);
        }

        static void Render()
        {
            for (var row = 0; row < SimulationBoard.Rows; row++)
            {
                for (var col = 0; col < SimulationBoard.Columns; col++)
                {
                    var cell = SimulationBoard.Cells[row, col];

                    Console.Write(cell.IsAlive
                        ? '*'
                        : ' ');
                }

                Console.Write('\n');
            }
        }

        static void Main(string[] args)
        {
            RetrieveGameplayMode();
        }
    }
}