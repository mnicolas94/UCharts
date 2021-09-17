using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UCharts.Runtime.Charts.Renderers
{
    public class AxisNameRenderer : TextElement, IDataRenderer
    {
        public new class UxmlFactory : UxmlFactory<AxisNameRenderer, UxmlTraits>{}

        public new class UxmlTraits : TextElement.UxmlTraits
        {
            UxmlBoolAttributeDescription m_horizontal = new UxmlBoolAttributeDescription { name = "horizontal" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var axisRenderer = (AxisNameRenderer) ve;
                axisRenderer._horizontal = m_horizontal.GetValueFromBag(bag, cc);
                axisRenderer.UpdateText();
            }
        }
        private ChartData _dataReference;
        private bool _horizontal;

        public AxisNameRenderer()
        {
        }
        
        public void SetChartDataReference(ChartData data)
        {
            _dataReference = data;
            UpdateText();
        }

        public void UpdateText()
        {
//            float halfWidth = contentRect.width * 0.5f;
//            float halfHeight = contentRect.height * 0.5f;
//            Debug.Log($"{contentRect.width}; {contentRect.height}");
//            Debug.Log($"before: {transform.position}");
//            transform.position += new Vector3(-halfWidth, halfHeight, 0);
//            Debug.Log($"after: {transform.position}");
//            var angleZ = _horizontal ? 0 : -90;
//            transform.rotation = Quaternion.Euler(0,0,angleZ);
            string xAxisName = "";
            string yAxisName = "";
            if (_dataReference != null)
            {
                xAxisName = _dataReference.XAxisName;
                yAxisName = _dataReference.YAxisName;
            }
            if (_horizontal)
            {
                text = xAxisName;
            }
            else
            {
                text = yAxisName;
            }
        }
    }
}