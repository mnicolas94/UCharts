using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UCharts.Runtime.Charts
{
    [Serializable]
    public class ChartSingleData
    {
        private List<Vector2> _points;
        private string _dataName;
        private Color _color;
        private bool _hasSpecificColor;
        private bool _enabled;
        
        private Rect _bounds;

        public List<Vector2> Points
        {
            get => _points;
            set => _points = value;
        }

        public string DataName
        {
            get => _dataName;
            set => _dataName = value;
        }

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                _hasSpecificColor = true;
            }
        }

        public bool HasSpecificColor => _hasSpecificColor;

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public Rect Bounds => _bounds;

        public ChartSingleData(List<Vector2> points, string dataName, Color color)
        {
            _points = points;
            _dataName = dataName;
            _color = color;
            _hasSpecificColor = true;
            _enabled = true;
            _bounds = new Rect();
            RecomputeBounds();
        }
        
        public ChartSingleData(List<Vector2> points, string dataName="")
        {
            _points = points;
            _dataName = dataName;
            _color = Color.white;
            _hasSpecificColor = false;
            _enabled = true;
            _bounds = new Rect();
            RecomputeBounds();
        }
        
        public ChartSingleData(Func<float, float> function, float min, float max, float step, string dataName, Color color)
        {
            _points = PointsFromFunction(function, min, max, step);
            _dataName = dataName;
            _color = color;
            _hasSpecificColor = true;
            _enabled = true;
            _bounds = new Rect();
            RecomputeBounds();
        }
        
        public ChartSingleData(Func<float, float> function, float min, float max, float step, string dataName="")
        {
            _points = PointsFromFunction(function, min, max, step);
            _dataName = dataName;
            _color = Color.white;
            _hasSpecificColor = false;
            _enabled = true;
            _bounds = new Rect();
            RecomputeBounds();
        }
        
        private void RecomputeBounds()
        {
            float minX = Single.MaxValue;
            float maxX = Single.MinValue;
            float minY = Single.MaxValue;
            float maxY = Single.MinValue;

            for (int i = 0; i < _points.Count; i++)
            {
                var point = _points[i];
                minX = Mathf.Min(minX, point.x);
                maxX = Mathf.Max(maxX, point.x);
                minY = Mathf.Min(minY, point.y);
                maxY = Mathf.Max(maxY, point.y);
            }

            float width = maxX - minX;
            float height = maxY - minY;
            
            _bounds = new Rect(minX, minY, width, height);
        }

        public static ChartSingleData RandomData(int pointsCount)
        {
            var points = new List<Vector2>();
            float sum = 0;
            for (int i = 0; i < 100; i++)
            {
                points.Add(new Vector2(i, sum));
                sum += Random.Range(-1, 3);
            }

            return new ChartSingleData(points);
        }

        public static ChartSingleData RandomData(int pointsCount, Color color)
        {
            var data = RandomData(pointsCount);
            data.Color = color;
            return data;
        }

        private static List<Vector2> PointsFromFunction(Func<float, float> function, float min, float max, float step)
        {
            var points = new List<Vector2>();
            for (float x = min; x <= max; x += step)
            {
                float y = function(x);
                points.Add(new Vector2(x, y));
            }

            return points;
        }
    }
}