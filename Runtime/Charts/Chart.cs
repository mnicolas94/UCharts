using System.Collections.Generic;
using UCharts.Runtime.Charts.Renderers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UCharts.Runtime.Charts
{
    public class Chart : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Chart>{}

        private ChartData _chartData;

        private List<IDataRenderer> _renderers;

        private ChartBackgroundRenderer _backgroundRenderer;
        private ScaleRenderer _horizontalScale;
        private ScaleRenderer _verticalScale;
        private VisualElement _renderersContainer;
        private VisualElement _legendContainer;
        
        public Chart()
        {
            _chartData = new ChartData();

            var visualTree = Resources.Load<VisualTreeAsset>("Chart");
            var uxmlContent = visualTree.Instantiate();
            uxmlContent.style.flexGrow = 1;
            Add(uxmlContent);
            _renderersContainer = uxmlContent.Q("ChartContainer");
            
            InitializeRenderers();
            
            _backgroundRenderer.RegisterCallback<WheelEvent>(OnWheel);
            _backgroundRenderer.RegisterCallback<MouseMoveEvent>(OnMouseEvent);
        }

        private void InitializeRenderers()
        {
            _renderers = new List<IDataRenderer>();
            
            _backgroundRenderer = this.Q<ChartBackgroundRenderer>("ChartBackground");
            _horizontalScale = this.Q<ScaleRenderer>("HorizontalScale");
            _verticalScale = this.Q<ScaleRenderer>("VerticalScale");

            var lineRenderer = new LineChartRenderer();
            lineRenderer.style.flexGrow = 1;
            _renderersContainer.Add(lineRenderer);
            
            _renderers.Add(_backgroundRenderer);
            _renderers.Add(_horizontalScale);
            _renderers.Add(_verticalScale);
            _renderers.Add(lineRenderer);

            foreach (var renderer in _renderers)
            {
                renderer.SetChartDataReference(_chartData);
            }
        }

        private void OnMouseEvent(MouseMoveEvent evt)
        {
            if (evt.pressedButtons == 1)
            {
                var delta = evt.mouseDelta;
                delta.x = -1 * (delta.x / _backgroundRenderer.contentRect.width) * _chartData.Bounds.width;
                delta.y = (delta.y / _backgroundRenderer.contentRect.height) * _chartData.Bounds.height;
                _chartData.AddOffset(delta);
                MarkDirtyRepaint();
            }
        }

        private void OnWheel(WheelEvent evt)
        {
            if (evt.delta.y > 0)
            {
                _chartData.ReduceZoom();
                MarkDirtyRepaint();
            }
            else if (evt.delta.y < 0)
            {
                _chartData.AddZoom();
                MarkDirtyRepaint();
            }
        }
        
        public void AddData(ChartSingleData singleData)
        {
            _chartData.AddData(singleData);
        }

        public void ClearData()
        {
            _chartData.ClearData();
        }
    }
}