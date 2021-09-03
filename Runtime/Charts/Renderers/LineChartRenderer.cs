using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Runtime.Extensions;

namespace UCharts.Runtime.Charts.Renderers
{
    public class LineChartRenderer : ImguiDataRenderer
    {
        public new class UxmlFactory : UxmlFactory<LineChartRenderer>{}

        public LineChartRenderer()
        {
            onGUIHandler += OnGui;
        }

        private void OnGui()
        {
            if (DataReference == null)
                return;
            
            var rect = contentRect;
            var curves = DataReference.Data;
            var dataBounds = DataReference.Bounds;
            for (int i = 0; i < curves.Count; i++)
            {
                var line = curves[i];
                Handles.color = line.HasSpecificColor ?
                    line.Color : 
                    Color.HSVToRGB((float) i / curves.Count, 1, 1);
                var points = line.Points;
                for (int j = 0; j < points.Count - 1; j++)
                {
                    var pointA = points[j].RemapBounds(dataBounds, rect);
                    pointA.y = rect.height - pointA.y;
                    var pointB = points[j + 1].RemapBounds(dataBounds, rect);
                    pointB.y = rect.height - pointB.y;
                    Handles.DrawLine(pointA, pointB);
                }
            }
        }
    }
}