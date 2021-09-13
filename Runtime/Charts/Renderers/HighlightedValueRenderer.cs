using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Runtime.Extensions;

namespace UCharts.Runtime.Charts.Renderers
{
    public class HighlightedValueRenderer : ImguiDataRenderer
    {
        public new class UxmlFactory : UxmlFactory<HighlightedValueRenderer>{}

        public HighlightedValueRenderer()
        {
            onGUIHandler += OnGui;
        }

        private void OnGui()
        {
            if (DataReference == null)
                return;
            
            var rect = contentRect;
            var dataBounds = DataReference.Bounds;

            if (DataReference.ExistsHighlightedPoint)
            {
                (var curve, int pointIndex) = DataReference.HighlightedPoint;
                var point = curve.Points[pointIndex];
                var uiPoint = point.RemapBounds(dataBounds, rect).MirrorVertically(rect);
                uiPoint.y -= 40;
                
                string xAxisName = "x";
                string yAxisName = "y";
                var guiStyle = GUI.skin.label;
                guiStyle.alignment = TextAnchor.MiddleCenter;
                var labelRect = new Rect(uiPoint + new Vector2(-2500, -2500), Vector2.one * 5000);
                GUI.Label(labelRect, $"{xAxisName}: {point.x:0.000}\n{yAxisName}: {point.y:0.000}");
            }
        }
    }
}