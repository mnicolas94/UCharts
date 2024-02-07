using UnityEngine;
using UnityEngine.UIElements;
using Utils.Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UCharts.Runtime.Charts.Renderers
{
    /*
     * Test class to visualize information about the spatial hash grid. Needs some members of ChartDataSpatialHashGrid
     * to be public.
     */
    public class SpatialHashGridRenderer : ImguiDataRenderer
    {
        public Vector2 mousePosition;
        
        public SpatialHashGridRenderer()
        {
#if UNITY_EDITOR
            onGUIHandler += OnGui;
#endif
        }

#if UNITY_EDITOR
        public void OnGui()
        {
//            var rect = contentRect;
//            var dataBounds = DataReference.Bounds;
//            
//            (int hIndex, int vIndex) = DataReference.HashGrid.GetContainerCell(mousePosition, DataReference.HashGrid.Bounds);
//            var neighborhood = DataReference.HashGrid.GetNeighborhood((hIndex, vIndex));
//            
//            Handles.color = Color.blue;
//            foreach (var neighborhoodIndices in neighborhood)
//            {
//                int hNIndex = neighborhoodIndices.Item1;
//                int vNIndex = neighborhoodIndices.Item2;
//
//                var points = DataReference.HashGrid.Grid[hNIndex, vNIndex];
//                foreach ((ChartSingleData curve, int index) in points)
//                {
//                    var point = curve.Points[index];
//                    bool insideBounds = point.IsInsideRect(dataBounds);
//
//                    if (insideBounds)
//                    {
//                        float radius = 3;
//                        point = point.RemapBounds(dataBounds, rect);
//                        point = point.MirrorVertically(rect);
//                        Handles.DrawSolidDisc(point, Vector3.back, radius);
//                    }
//                }
//            }
//
//            if (hIndex >= 0 && hIndex < DataReference.HashGrid.HorizontalCount &&
//                vIndex >= 0 && vIndex < DataReference.HashGrid.VerticalCount)
//            {
//                Handles.color = Color.green;
//                var cellPoints = DataReference.HashGrid.Grid[hIndex, vIndex];
//                foreach ((ChartSingleData curve, int index) in cellPoints)
//                {
//                    var point = curve.Points[index];
//                    bool insideBounds = point.IsInsideRect(dataBounds);
//
//                    if (insideBounds)
//                    {
//                        float radius = 3;
//                        point = point.RemapBounds(dataBounds, rect);
//                        point = point.MirrorVertically(rect);
//                        Handles.DrawSolidDisc(point, Vector3.back, radius);
//                    }
//                }
//            }
        }
#endif
    }
}