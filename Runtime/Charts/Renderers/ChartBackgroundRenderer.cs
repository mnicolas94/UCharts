using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Runtime.Extensions;

namespace UCharts.Runtime.Charts.Renderers
{
    public class ChartBackgroundRenderer : ImguiDataRenderer
    {
        public new class UxmlFactory : UxmlFactory<ChartBackgroundRenderer>{}

        public ChartBackgroundRenderer()
        {
            onGUIHandler += OnGui;
        }

        private void OnGui()
        {
            if (DataReference == null)
                return;
            
            var rect = contentRect;
            var dataBounds = DataReference.Bounds;
            Handles.color = Color.gray;
            float dottedLinesSpace = 4f;

            // vertical lines along X axis
            var horizontalSplits = dataBounds.HorizontalEvenlySplit();
            foreach (var x in horizontalSplits)
            {
                var pointA = new Vector2(x, dataBounds.yMin);
                var pointB = new Vector2(x, dataBounds.yMax);
                pointA = pointA.RemapBounds(dataBounds, rect);
                pointB = pointB.RemapBounds(dataBounds, rect);
                Handles.DrawDottedLine(pointA, pointB, dottedLinesSpace);
            }
            
            // horizontal lines along Y axis
            var verticalSplits = dataBounds.VerticalEvenlySplit();
            foreach (var y in verticalSplits)
            {
                var pointA = new Vector2(dataBounds.xMin, y);
                var pointB = new Vector2(dataBounds.xMax, y);
                pointA = pointA.RemapBounds(dataBounds, rect).MirrorVertically(rect);
                pointB = pointB.RemapBounds(dataBounds, rect).MirrorVertically(rect);
                Handles.DrawDottedLine(pointA, pointB, dottedLinesSpace);
            }
            
            // border lines
            float left = rect.xMin;
            float right = rect.xMax;
            float top = rect.yMin;
            float bottom = rect.yMax;
            Vector2 leftTop = new Vector2(left, top);
            Vector2 leftBottom = new Vector2(left, bottom);
            Vector2 rightTop = new Vector2(right, top);
            Vector2 rightBottom = new Vector2(right, bottom);
            Handles.DrawPolyLine(leftBottom, rightBottom, rightTop, leftTop, leftBottom);
        }
    }
}