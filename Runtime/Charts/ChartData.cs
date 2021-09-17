using System;
using System.Collections.Generic;
using UnityEngine;

namespace UCharts.Runtime.Charts
{
    [Serializable]
    public class ChartData
    {
        private string _tittle;
        private string _xAxisName;
        private string _yAxisName;
        private List<ChartSingleData> _data;
        private ChartDataSpatialHashGrid _hashGrid;
        private Rect _bounds;
        private float _scaleHorizontal;
        private float _scaleVertical;
        private Vector2 _offset;

        private (ChartSingleData, int) _highlightedPoint;
        private bool _existsHighlightedPoint;

        public string Tittle
        {
            get => _tittle;
            set => _tittle = value;
        }

        public string XAxisName
        {
            get => _xAxisName;
            set => _xAxisName = value;
        }

        public string YAxisName
        {
            get => _yAxisName;
            set => _yAxisName = value;
        }

        public List<ChartSingleData> Data => _data;

        public Rect Bounds
        {
            get
            {
                Rect transformedBounds = _bounds;

                float extraWidth = transformedBounds.width * (_scaleHorizontal - 1);
                float extraHeight = transformedBounds.height * (_scaleVertical - 1);
                transformedBounds.width = transformedBounds.width * _scaleHorizontal;
                transformedBounds.height = transformedBounds.height * _scaleVertical;
                transformedBounds.x -= extraWidth * 0.5f;
                transformedBounds.y -= extraHeight * 0.5f;
                
                transformedBounds.x += _offset.x;
                transformedBounds.y += _offset.y;
                
                return transformedBounds;
            }
        }

        public float ScaleHorizontal
        {
            get => _scaleHorizontal;
            set => _scaleHorizontal = Mathf.Clamp(value, 0.001f, 2);
        }

        public float ScaleVertical
        {
            get => _scaleVertical;
            set => _scaleVertical = Mathf.Clamp(value, 0.001f, 2);
        }

        public Vector2 Offset
        {
            get => _offset;
            set
            {
                float halfWidth = _bounds.width * 0.5f;
                float halfHeight = _bounds.height * 0.5f;
                _offset.x = Mathf.Clamp(value.x, -halfWidth, halfWidth);
                _offset.y = Mathf.Clamp(value.y, -halfHeight, halfHeight);
            }
        }

        public (ChartSingleData, int) HighlightedPoint => _highlightedPoint;

        public bool ExistsHighlightedPoint => _existsHighlightedPoint;

        public int Count => _data.Count;

        public ChartData(string tittle = "", string xAxisName = "", string yAxisName = "")
        {
            _tittle = tittle;
            _xAxisName = xAxisName;
            _yAxisName = yAxisName;
            _data = new List<ChartSingleData>();
            _hashGrid = new ChartDataSpatialHashGrid();
            _bounds = new Rect(0, 0, 0, 0);
            _scaleHorizontal = 1;
            _scaleVertical = 1;
            _offset = Vector2.zero;
        }

        public void AddData(ChartSingleData singleData)
        {
            if (_data.Count == 0)  // if there is no data
                _bounds.Set(0, 0, 0, 0);
            _data.Add(singleData);
            _bounds = RectContainer(_bounds, singleData.Bounds);
            _hashGrid.ConstructGrid(_data, _bounds);
            DistributeColors();
        }
        
        public void AddDatas(params ChartSingleData[] datas)
        {
            if (_data.Count == 0)  // if there is no data
                _bounds.Set(0, 0, 0, 0);
            var bounds = new Rect[datas.Length + 1];
            bounds[0] = _bounds;
            for (int i = 0; i < datas.Length; i++)
            {
                var singleData = datas[i];
                _data.Add(singleData);
                bounds[i + 1] = singleData.Bounds;
            }

            _bounds = RectContainer(bounds);
            _hashGrid.ConstructGrid(_data, _bounds);
            DistributeColors();
        }

        public void ClearData()
        {
            _data.Clear();
            _bounds.Set(0, 0, 10, 10);
        }

        public void ResetOffsetScale()
        {
            _scaleHorizontal = 1;
            _scaleVertical = 1;
            _offset = Vector2.zero;
        }
        
        public void AddZoom()
        {
            AddHorizontalZoom();
            AddVerticalZoom();
        }
        
        public void AddHorizontalZoom()
        {
            ScaleHorizontal *= 0.9f;
        }
        
        public void AddVerticalZoom()
        {
            ScaleVertical *= 0.9f;
        }

        public void ReduceZoom()
        {
            ReduceHorizontalZoom();
            ReduceVerticalZoom();
        }
        
        public void ReduceHorizontalZoom()
        {
            ScaleHorizontal *= 1.1f;
        }
        
        public void ReduceVerticalZoom()
        {
            ScaleVertical *= 1.1f;
        }

        public void AddOffset(Vector2 offset)
        {
            Offset += offset;
        }

        public void RecomputeBounds()
        {
            Rect[] dataBounds = new Rect[_data.Count];
            for (int i = 0; i < _data.Count; i++)
            {
                var data = _data[i];
                data.RecomputeBounds();
                dataBounds[i] = data.Bounds;
            }

            _bounds = RectContainer(dataBounds);
        }

        public void RecomputeSpatialHashGrid()
        {
            _hashGrid.ConstructGrid(_data, _bounds);
        }

        public void HandleClosestPointHighlighting(Vector2 targetPoint, float distanceThreshold, Rect uiBounds, Rect dataBounds)
        {
            ((var curve, int pointIndex), float sqrDistance, bool exists) = _hashGrid.GetClosestPoint(targetPoint, uiBounds, dataBounds);
            if (exists)
            {
                bool withinThreshold = sqrDistance <= distanceThreshold * distanceThreshold;
                _highlightedPoint = (curve, pointIndex);
                _existsHighlightedPoint = withinThreshold;
            }
            else
            {
                _existsHighlightedPoint = false;
            }
        }
        
        private void DistributeColors()
        {
            int colorNotSpecifiedCount = 0;
            for (int i = 0; i < Count; i++)
            {
                var data = _data[i];
                if (!data.HasSpecificColor)
                {
                    data.ColorWithoutSpecifying = Color.HSVToRGB((float) colorNotSpecifiedCount / Count, 1, 1);
                    colorNotSpecifiedCount++;
                }
            }
        }
        
        private Rect RectContainer(params Rect[] rects)
        {
            float minX = Single.MaxValue;
            float maxX = Single.MinValue;
            float minY = Single.MaxValue;
            float maxY = Single.MinValue;

            for (int i = 0; i < rects.Length; i++)
            {
                var rect = rects[i];
                minX = Mathf.Min(minX, rect.xMin);
                maxX = Mathf.Max(maxX, rect.xMax);
                minY = Mathf.Min(minY, rect.yMin);
                maxY = Mathf.Max(maxY, rect.yMax);
            }

            float width = maxX - minX;
            float height = maxY - minY;
            
            var containerRect = new Rect(minX, minY, width, height);
            return containerRect;
        }
    }
}