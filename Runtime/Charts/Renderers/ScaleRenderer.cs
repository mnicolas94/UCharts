using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Runtime.Extensions;

namespace UCharts.Runtime.Charts.Renderers
{
    public class ScaleRenderer : ImguiDataRenderer
    {
        public new class UxmlFactory : UxmlFactory<ScaleRenderer, UxmlTraits>{}
        public new class UxmlTraits : ImguiDataRenderer.UxmlTraits
        {
            UxmlBoolAttributeDescription m_horizontal = new UxmlBoolAttributeDescription { name = "horizontal" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((ScaleRenderer)ve)._horizontal = m_horizontal.GetValueFromBag(bag, cc);
            }
        }
        
        private bool _horizontal;

        public bool Horizontal
        {
            get => _horizontal;
            set => _horizontal = value;
        }

        public ScaleRenderer()
        {
            onGUIHandler += OnGui;
        }

        private void OnGui()
        {
            if (DataReference == null)
                return;
            
            var rect = contentRect;
            var dataBounds = DataReference.Bounds;
            
            if (_horizontal)
            {
                var horizontalSplits = dataBounds.HorizontalEvenlySplit();
                foreach (var x in horizontalSplits)
                {
                    float rangePercent = (x - dataBounds.xMin) / dataBounds.width;
                    float uiXPosition = Mathf.Lerp(rect.xMin, rect.xMax, rangePercent);
                    float labelWidth = 40;
                    var labelRect = new Rect(uiXPosition - labelWidth * 0.5f,  0, labelWidth, rect.height);
                    string text = $"{x}";
                    var labelStyle = GUI.skin.label;
                    labelStyle.alignment = TextAnchor.MiddleCenter;
                    GUI.Label(labelRect, text, labelStyle);
                }
            }
            else
            {
                var verticalSplits = dataBounds.VerticalEvenlySplit();
                foreach (var y in verticalSplits)
                {
                    float rangePercent = (y - dataBounds.yMin) / dataBounds.height;
                    float uiYPosition = Mathf.Lerp(rect.yMin, rect.yMax, 1 - rangePercent);
                    float labelHeight = 25;
                    var labelRect = new Rect(0, uiYPosition - labelHeight * 0.5f, rect.width, labelHeight);
                    string text = $"{y}";
                    var labelStyle = GUI.skin.label;
                    labelStyle.alignment = TextAnchor.MiddleCenter;
                    GUI.Label(labelRect, text, labelStyle);
                }
            }
        }
    }
}