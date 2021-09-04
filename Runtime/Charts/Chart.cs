﻿using System.Collections.Generic;
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
        private ChartLegend _chartLegend;
        
        private VisualElement _renderersContainer;
        private VisualElement _renderersSelectorsContainer;
        private Dictionary<string, VisualElement> _optionalRenderers;
        
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

        private void AddDataInternal(ChartSingleData singleData)
        {
            _chartData.AddData(singleData);
        }

        public void AddData(ChartSingleData singleData)
        {
            AddDataInternal(singleData);
            DistributeColors();
            _chartLegend.UpdateLegend();
            MarkDirtyRepaint();
        }

        public void AddDatas(params ChartSingleData[] datas)
        {
            foreach (var data in datas)
            {
                AddDataInternal(data);
            }
            DistributeColors();
            _chartLegend.UpdateLegend();
            MarkDirtyRepaint();
        }
        
        public void ClearData()
        {
            _chartData.ClearData();
            _chartLegend.UpdateLegend();
            MarkDirtyRepaint();
        }
        
        private void InitializeRenderers()
        {
            _renderers = new List<IDataRenderer>();
            _optionalRenderers = new Dictionary<string, VisualElement>();
            
            _backgroundRenderer = this.Q<ChartBackgroundRenderer>("ChartBackground");
            _horizontalScale = this.Q<ScaleRenderer>("HorizontalScale");
            _verticalScale = this.Q<ScaleRenderer>("VerticalScale");
            _chartLegend = this.Q<ChartLegend>("ChartLegend");

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
            _renderers.Add(_horizontalScale);
            _renderers.Add(_verticalScale);
            _renderers.Add(_chartLegend);
            _renderers.Add(lineRenderer);
            _renderers.Add(pointsRenderer);

            foreach (var renderer in _renderers)
            {
                renderer.SetChartDataReference(_chartData);
            }
            
            _optionalRenderers.Add("Background", _backgroundRenderer);
            _optionalRenderers.Add("Horizontal Scale", _horizontalScale);
            _optionalRenderers.Add("Vertical Scale", _verticalScale);
            _optionalRenderers.Add("Legend", _chartLegend);
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
                    var displayStyle = changeEvent.newValue
                        ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex)
                        : new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    renderer.style.display = displayStyle;
                });
                _renderersSelectorsContainer.Add(toggle);
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
            Debug.Log("wheel");
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

        private void DistributeColors()
        {
            int colorNotSpecifiedCount = 0;
            for (int i = 0; i < _chartData.Count; i++)
            {
                var data = _chartData.Data[i];
                if (!data.HasSpecificColor)
                {
                    data.Color = Color.HSVToRGB((float) colorNotSpecifiedCount / _chartData.Count, 1, 1);
                    colorNotSpecifiedCount++;
                }
            }
        }
    }
}