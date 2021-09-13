using System;
using System.Collections.Generic;
using UnityEngine;

namespace UCharts.Runtime.Charts
{
    public class ChartDataSpatialHashGrid
    {
        private int _horizontalCount;
        private int _verticalCount;
        private List<(ChartSingleData, int)>[,] _grid;
        private Rect _bounds;
        
        public ChartDataSpatialHashGrid(int horizontalCount = 10, int verticalCount = 10)
        {
            _horizontalCount = horizontalCount;
            _verticalCount = verticalCount;
            _grid = new List<(ChartSingleData, int)>[horizontalCount, verticalCount];
            
            for (int i = 0; i < horizontalCount; i++)
            {
                for (int j = 0; j < verticalCount; j++)
                {
                    _grid[i, j] = new List<(ChartSingleData, int)>();
                }
            }
        }

        public ((ChartSingleData, int), float, bool) GetClosestPoint(Vector2 point)
        {
            var cellIndices = GetContainerCell(point, _bounds);
            var neighborhoodCellsIndices = GetNeighborhood(cellIndices);

            bool assigned = false;
            (ChartSingleData, int) closestPoint = (null, -1);
            float closestSqrDistance = Single.MaxValue;
            foreach (var neighborhoodIndices in neighborhoodCellsIndices)
            {
                int hIndex = neighborhoodIndices.Item1;
                int vIndex = neighborhoodIndices.Item2;

                var cellPoints = _grid[hIndex, vIndex];
                for (int i = 0; i < cellPoints.Count; i++)
                {
                    (ChartSingleData curve, int pointIndex) = cellPoints[i];
                    if (curve.Enabled)
                    {
                        var curvePoint = curve.Points[pointIndex];
                        float sqrDistance = Vector2.SqrMagnitude(point - curvePoint);
                        if (sqrDistance < closestSqrDistance)
                        {
                            closestSqrDistance = sqrDistance;
                            closestPoint = (curve, pointIndex);
                            assigned = true;
                        }
                    }    
                }
            }

            return (closestPoint, closestSqrDistance, assigned);
        }

        public void ConstructGrid(List<ChartSingleData> curves, Rect bounds)
        {
            Clear();
            _bounds = bounds;
            foreach (var curve in curves)
            {
                for (int i = 0; i < curve.Points.Count; i++)
                {
                    var point = curve.Points[i];
                    (int hIndex, int vIndex) = GetContainerCell(point, bounds);
                    hIndex = Mathf.Clamp(hIndex, 0, _horizontalCount - 1);
                    vIndex = Mathf.Clamp(vIndex, 0, _verticalCount - 1);
                    _grid[hIndex, vIndex].Add((curve, i));
                }
            }
        }

        private void Clear()
        {
            for (int i = 0; i < _horizontalCount; i++)
            {
                for (int j = 0; j < _verticalCount; j++)
                {
                    _grid[i, j].Clear();
                }
            }
        }

        private (int, int) GetContainerCell(Vector2 point, Rect bounds)
        {
            float percentX = (point.x - bounds.xMin) / bounds.width;
            float percentY = (point.y - bounds.yMin) / bounds.height;

            int horizontalIndex = (int )(_horizontalCount * percentX);
            int verticalIndex = (int )(_verticalCount * percentY);
            return (horizontalIndex, verticalIndex);
        }

        private IEnumerable<(int, int)> GetNeighborhood((int hIndex, int vIndex) containerCellIndices)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int neighborHorizontalIndex = containerCellIndices.hIndex + i;
                    int neighborVerticalIndex = containerCellIndices.vIndex + j;
                    bool insideHorizontalBounds = neighborHorizontalIndex >= 0 && neighborHorizontalIndex < _horizontalCount;
                    bool insideVerticalBounds = neighborVerticalIndex >= 0 && neighborVerticalIndex < _verticalCount;
                    if (insideHorizontalBounds && insideVerticalBounds)
                    {
                        yield return (neighborHorizontalIndex, neighborVerticalIndex);
                    }
                }
            }
        }
    }
}