using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UCharts.Runtime.Charts.Renderers
{
    public class ChartLegend : VisualElement, IDataRenderer
    {
        public new class UxmlFactory : UxmlFactory<ChartLegend>{}

        public Action eventRepaintRequest;
        
        private ChartData _chartData;
        
        private VisualTreeAsset _legendElementAsset;
        
        public ChartLegend()
        {
            _legendElementAsset = Resources.Load<VisualTreeAsset>("LegendElement");
        }
        
        public void SetChartDataReference(ChartData data)
        {
            _chartData = data;
        }

        public void UpdateLegend()
        {
            if (_chartData == null)
                return;
            ClearLegend();
            foreach (var chartSingleData in _chartData.Data)
            {
                AddLegendElement(chartSingleData);
            }
        }

        private void AddLegendElement(ChartSingleData data)
        {
            var element = _legendElementAsset.Instantiate();
            var color = element.Q("color");
            color.style.backgroundColor = new StyleColor(data.Color);
            var label = element.Q<Label>("label");
            label.text = data.DataName;
            var toggle = element.Q<Toggle>("toggle");
            toggle.value = data.Enabled;
            toggle.RegisterValueChangedCallback((changeEvent) =>
            {
                data.Enabled = changeEvent.newValue;
                eventRepaintRequest?.Invoke();
                Debug.Log($"toggle changed to: {changeEvent.newValue}");
            });
            
            Add(element);
        }

        private void ClearLegend()
        {
            Clear();
        }
    }
}