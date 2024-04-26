using cli_life;
using ScottPlot;

namespace Life
{
    public class RenderInScottPlot : IRender
    {
        public void Render()
        {
            throw new System.NotImplementedException();
        }

        public void RenderStep()
        {
            throw new System.NotImplementedException();
        }

        public void SavePict(Board board, SettingsRender settings, string path)
        {
            var plt = new Plot(settings.WidthMap, settings.HeightMap);

            double[,] cellsInDouble = new double[board.Rows, board.Colums];

            for (int y = 0; y < board.Rows; y++)
                for (int x = 0; x < board.Colums; x++)
                    cellsInDouble[y, x] = board.Cells[x, y].IsAlive ? 1 : 0;

            var hm = plt.AddHeatmap(cellsInDouble, lockScales: false);

            plt.SaveFig(path);
        }
    }
}