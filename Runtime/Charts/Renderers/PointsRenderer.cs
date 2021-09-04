using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Runtime.Extensions;

namespace UCharts.Runtime.Charts.Renderers
{
    public class PointsRenderer : ImguiDataRenderer
    {
        public new class UxmlFactory : UxmlFactory<PointsRenderer>{}

        public PointsRenderer()
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
                for (int j = 0; j < points.Count; j++)
                {
                    var point = points[j];
                    bool insideBounds = point.IsInsideRect(dataBounds);

                    if (insideBounds)
                    {
                        point = point.RemapBounds(dataBounds, rect);
                        point = point.MirrorVertically(rect);
                        Handles.DrawSolidDisc(point, Vector3.back, 3);
                    }
                }
            }
        }
    }
}