using System;
using System.Collections.Generic;
using UnityEngine;

namespace UCharts.Runtime.Charts
{
    [Serializable]
    public class ChartData
    {
        private List<ChartSingleData> _data;
        private Rect _bounds;
        private float _scale;
        private Vector2 _offset;

        public List<ChartSingleData> Data => _data;
        
        public Rect Bounds
        {
            get
            {
                Rect transformedBounds = _bounds;

                float extraWidth = transformedBounds.width * (_scale - 1);
                float extraHeight = transformedBounds.height * (_scale - 1);
                transformedBounds.width = transformedBounds.width * _scale;
                transformedBounds.height = transformedBounds.height * _scale;
                transformedBounds.x -= extraWidth * 0.5f;
                transformedBounds.y -= extraHeight * 0.5f;
                
                transformedBounds.x += _offset.x;
                transformedBounds.y += _offset.y;
                
                return transformedBounds;
            }
        }

        public float Scale
        {
            get => _scale;
            set => _scale = Mathf.Clamp(value, 0.05f, 2);
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

        public int Count => _data.Count;

        public ChartData()
        {
            _data = new List<ChartSingleData>();
            _bounds = new Rect(0, 0, 0, 0);
            _scale = 1;
            _offset = Vector2.zero;
        }

        public void AddData(ChartSingleData singleData)
        {
            _data.Add(singleData);
            if (_data.Count == 1)  // if added first data
                _bounds.Set(0, 0, 0, 0);
            _bounds = RectContainer(_bounds, singleData.Bounds);
        }

        public void ClearData()
        {
            _data.Clear();
            _bounds.Set(0, 0, 10, 10);
        }

        public void AddZoom()
        {
            Scale *= 0.9f;
        }

        public void ReduceZoom()
        {
            Scale *= 1.1f;
        }

        public void AddOffset(Vector2 offset)
        {
            Offset += offset;
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