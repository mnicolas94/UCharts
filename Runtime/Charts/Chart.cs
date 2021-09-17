using System.Collections.Generic;
using UCharts.Runtime.Charts.Renderers;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using Utils.Runtime.Extensions;

namespace UCharts.Runtime.Charts
{
    public class Chart : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Chart>{}

        private ChartData _chartData;

        private List<IDataRenderer> _renderers;

        private ChartBackgroundRenderer _backgroundRenderer;
        private AxisNameRenderer _xAxisRenderer;
        private AxisNameRenderer _yAxisRenderer;
        private ScaleRenderer _horizontalScale;
        private ScaleRenderer _verticalScale;
        private ChartLegend _chartLegend;
        private HighlightedValueRenderer _valueRenderer;

        private VisualElement _renderersContainer;
        private VisualElement _renderersSelectorsContainer;
        private Dictionary<string, VisualElement> _optionalRenderers;
        
        public string Tittle
        {
            get => _chartData.Tittle;
            set => _chartData.Tittle = value;
        }

        public string XAxisName
        {
            get => _chartData.XAxisName;
            set
            {
                _chartData.XAxisName = value;
                _xAxisRenderer.UpdateText();
                _yAxisRenderer.UpdateText();
            }
        }

        public string YAxisName
        {
            get => _chartData.YAxisName;
            set
            {
                _chartData.YAxisName = value;
                _xAxisRenderer.UpdateText();
                _yAxisRenderer.UpdateText();
            }
        }
        
        
        public Chart()
        {
            _chartData = new ChartData();

            var visualTree = Resources.Load<VisualTreeAsset>("Chart");
            var uxmlContent = visualTree.Instantiate();
            uxmlContent.style.flexGrow = 1;
            Add(uxmlContent);
            _renderersContainer = uxmlContent.Q("ChartContainer");
            _renderersSelectorsContainer = uxmlContent.Q("RenderersSelectionLayout");
                
            InitializeRenderers();
            PopulateRenderersSelectors();
            
            _renderersContainer.RegisterCallback<WheelEvent>(OnWheel);
            _renderersContainer.RegisterCallback<MouseMoveEvent>(OnMouseEvent);
            _chartLegend.eventRepaintRequest += MarkDirtyRepaint;
        }

        public void AddData(ChartSingleData singleData)
        {
            _chartData.AddData(singleData);
            _chartLegend.UpdateLegend();
            MarkDirtyRepaint();
        }

        public void AddDatas(params ChartSingleData[] datas)
        {
            _chartData.AddDatas(datas);
            _chartLegend.UpdateLegend();
            MarkDirtyRepaint();
        }
        
        public void ClearData()
        {
            _chartData.ClearData();
            _chartLegend.UpdateLegend();
            MarkDirtyRepaint();
        }

        public void RecomputeMetadata()
        {
            _chartData.RecomputeBounds();
            _chartData.RecomputeSpatialHashGrid();
        }
        
        private void InitializeRenderers()
        {
            _renderers = new List<IDataRenderer>();
            _optionalRenderers = new Dictionary<string, VisualElement>();
            
            _backgroundRenderer = this.Q<ChartBackgroundRenderer>("ChartBackground");
            _xAxisRenderer = this.Q<AxisNameRenderer>("AxisX");
            _yAxisRenderer = this.Q<AxisNameRenderer>("AxisY");
            _horizontalScale = this.Q<ScaleRenderer>("HorizontalScale");
            _verticalScale = this.Q<ScaleRenderer>("VerticalScale");
            _chartLegend = this.Q<ChartLegend>("ChartLegend");
            _valueRenderer = this.Q<HighlightedValueRenderer>("ValueRenderer");

            var lineRenderer = new LineChartRenderer();
            lineRenderer.style.position = new StyleEnum<Position>(Position.Absolute);
            lineRenderer.style.left = new StyleLength(Length.Percent(0));
            lineRenderer.style.right = new StyleLength(Length.Percent(0));
            lineRenderer.style.top = new StyleLength(Length.Percent(0));
            lineRenderer.style.bottom = new StyleLength(Length.Percent(0));
            _renderersContainer.Add(lineRenderer);
            
            var pointsRenderer = new PointsRenderer();
            pointsRenderer.style.position = new StyleEnum<Position>(Position.Absolute);
            pointsRenderer.style.left = new StyleLength(Length.Percent(0));
            pointsRenderer.style.right = new StyleLength(Length.Percent(0));
            pointsRenderer.style.top = new StyleLength(Length.Percent(0));
            pointsRenderer.style.bottom = new StyleLength(Length.Percent(0));
            _renderersContainer.Add(pointsRenderer);
            
            _renderers.Add(_backgroundRenderer);
            _renderers.Add(_xAxisRenderer);
            _renderers.Add(_yAxisRenderer);
            _renderers.Add(_horizontalScale);
            _renderers.Add(_verticalScale);
            _renderers.Add(_chartLegend);
            _renderers.Add(_valueRenderer);
            _renderers.Add(lineRenderer);
            _renderers.Add(pointsRenderer);

            foreach (var renderer in _renderers)
            {
                renderer.SetChartDataReference(_chartData);
            }
            
            _optionalRenderers.Add("Background", _backgroundRenderer);
            _optionalRenderers.Add("X axis name", _xAxisRenderer);
            _optionalRenderers.Add("Y axis name", _yAxisRenderer);
            _optionalRenderers.Add("Horizontal Scale", _horizontalScale);
            _optionalRenderers.Add("Vertical Scale", _verticalScale);
            _optionalRenderers.Add("Legend", _chartLegend);
            _optionalRenderers.Add("Pop-up value", _valueRenderer);
            _optionalRenderers.Add("Lines Renderer", lineRenderer);
            _optionalRenderers.Add("Points Renderer", pointsRenderer);
        }

        private void PopulateRenderersSelectors()
        {
            foreach (var rendererName in _optionalRenderers.Keys)
            {
                var renderer = _optionalRenderers[rendererName];
                
                var toggle = new Toggle(rendererName);
                toggle.value = true;
                toggle.RegisterValueChangedCallback((changeEvent) =>
                {
                    var visibilityStyle = changeEvent.newValue
                        ? new StyleEnum<Visibility>(Visibility.Visible)
                        : new StyleEnum<Visibility>(Visibility.Hidden);
                    renderer.style.visibility = visibilityStyle;
                });
                _renderersSelectorsContainer.Add(toggle);
            }
        }

        private void OnMouseEvent(MouseMoveEvent evt)
        {
            var rect = _backgroundRenderer.contentRect;
            var dataBounds = _chartData.Bounds;
            // handle drag
            if (evt.pressedButtons == 1)
            {
                var delta = evt.mouseDelta;
                delta.x = -1 * (delta.x / rect.width) * dataBounds.width;
                delta.y = (delta.y / rect.height) * dataBounds.height;
                _chartData.AddOffset(delta);
                MarkDirtyRepaint();
            }
            
            // handle highlighting
            var position = evt.localMousePosition;
            float pixelsDistanceThreshold = 10;
            bool previouslyHighlighted = _chartData.ExistsHighlightedPoint;
            _chartData.HandleClosestPointHighlighting(position, pixelsDistanceThreshold, rect, dataBounds);
            
            if (_chartData.ExistsHighlightedPoint || previouslyHighlighted)
                MarkDirtyRepaint();
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
    }
}