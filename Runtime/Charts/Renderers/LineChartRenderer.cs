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
                if (!line.Enabled)
                    continue;
                
                Handles.color = line.Color;
                var points = line.Points;
                for (int j = 0; j < points.Count - 1; j++)
                {
                    var pointA = points[j];
                    var pointB = points[j + 1];
                    bool aInside = pointA.IsInsideRect(dataBounds);
                    bool bInside = pointB.IsInsideRect(dataBounds);

                    if (aInside && bInside)
                    {
                        pointA = pointA.RemapBounds(dataBounds, rect);
                        pointA = pointA.MirrorVertically(rect);
                        pointB = pointB.RemapBounds(dataBounds, rect);
                        pointB = pointB.MirrorVertically(rect);
                        Handles.DrawLine(pointA, pointB);
                    }
                }
            }
        }
    }
}