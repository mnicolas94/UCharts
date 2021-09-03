using UnityEngine.UIElements;

namespace UCharts.Runtime.Charts.Renderers
{
    public abstract class ImguiDataRenderer : IMGUIContainer, IDataRenderer
    {
        protected ChartData DataReference;

        public void SetChartDataReference(ChartData dataReference)
        {
            DataReference = dataReference;
        }
    }
}