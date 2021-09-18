using UnityEngine;
using UnityEngine.UIElements;
using Utils.Runtime.Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UCharts.Runtime.Charts.Renderers
{
    public class PointsRenderer : ImguiDataRenderer
    {
        public new class UxmlFactory : UxmlFactory<PointsRenderer>{}

        public PointsRenderer()
        {
#if UNITY_EDITOR
            onGUIHandler += OnGui;
#endif
        }

#if UNITY_EDITOR
        private void OnGui()
        {
            if (DataReference == null)
                return;
            
            var rect = contentRect;
            var curves = DataReference.Data;
            var dataBounds = DataReference.Bounds;
            
            // draw points
            for (int i = 0; i < curves.Count; i++)
            {
                var curve = curves[i];
                if (!curve.Enabled)
                    continue;
                
                Handles.color = curve.Color;
                var points = curve.Points;
                
                for (int j = 0; j < points.Count; j++)
                {
                    var point = points[j];
                    bool insideBounds = point.IsInsideRect(dataBounds);

                    if (insideBounds)
                    {
                        float radius = 3;
                        point = point.RemapBounds(dataBounds, rect);
                        point = point.MirrorVertically(rect);
                        Handles.DrawSolidDisc(point, Vector3.back, radius);
                    }
                }
            }
            
            // draw the highlighted point
            if (DataReference.ExistsHighlightedPoint)
            {
                (var highlightedCurve, int pointIndex) = DataReference.HighlightedPoint;
                if (highlightedCurve.Enabled)
                {
                    Handles.color = highlightedCurve.Color;
                    var point = highlightedCurve.Points[pointIndex];
                    float radius = 5;
                    point = point.RemapBounds(dataBounds, rect);
                    point = point.MirrorVertically(rect);
                    Handles.DrawSolidDisc(point, Vector3.back, radius);
                }
            }
        }
#endif
    }
}