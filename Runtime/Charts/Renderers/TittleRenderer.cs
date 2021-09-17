using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UCharts.Runtime.Charts.Renderers
{
    public class TittleRenderer : TextElement, IDataRenderer
    {
        public new class UxmlFactory : UxmlFactory<TittleRenderer, UxmlTraits>{}
        
        private ChartData _dataReference;

        public TittleRenderer()
        {
        }
        
        public void SetChartDataReference(ChartData data)
        {
            _dataReference = data;
            UpdateText();
        }

        public void UpdateText()
        {
            string tittleName = _dataReference != null ? _dataReference.Tittle : "";
            text = tittleName;
        }
    }
}