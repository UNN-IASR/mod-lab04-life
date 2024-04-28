using System;
using System.Collections.Generic;
using System.Linq;

namespace Life
{
    public class ScottPlotGraphic
    {
        public ScottPlot.Plot Plot { get; private set; } = new ScottPlot.Plot(600, 300);
        public Random Rd { get; }
        public CGL Cgl { get; }
        public int MaxIteration { get; }
        public double StepDensity { get; }

        public ScottPlotGraphic(Random rd, CGL cgl, int maxIteration, double stepDensity)
        {
            Rd = rd;
            Cgl = cgl;
            MaxIteration = maxIteration;
            StepDensity = stepDensity;
        }

        public void GetGraphic(string savePath)
        {
            Plot = new ScottPlot.Plot();
            Plot.Title("Transition to Stable State");
            Plot.XLabel("Epoch");
            Plot.YLabel("Live cell");
            Plot.Legend();

            for (double density = StepDensity; density < 1; density += StepDensity)
                Step(density);

            Plot.SaveFig(savePath);
        }

        private void Step(double density)
        {
            var cgl = new CGL(FillMap.FillRandom(Cgl.Map, density, Rd), Cgl.ConnectNeighbors);
            var countsLive = new List<double>();
            var iterations = Enumerable.Range(0, MaxIteration).Select(item => (double)item).ToArray();

            foreach (var iteration in iterations)
            {
                countsLive.Add(AnalyzerMap.CountAlive(cgl.Map));
                cgl.Advance();
            }

            Plot.AddScatter(iterations, countsLive.ToArray(), label: density.ToString());
        }

    }
}
