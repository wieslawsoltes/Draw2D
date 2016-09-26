using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Draw2D.Models;
using Draw2D.Models.Renderers;
using Draw2D.Models.Style;

namespace Draw2D.Wpf.Renderers
{
    public class WpfShapeRenderer : ShapeRenderer
    {
        private readonly IDictionary<DrawStyle, WpfBrushCache> _brushCache;

        private HashSet<BaseShape> _selected;

        public override HashSet<BaseShape> Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    Notify("Selected");
                }
            }
        }

        public WpfShapeRenderer()
        {
            _brushCache = new Dictionary<DrawStyle, WpfBrushCache>();
            _selected = new HashSet<BaseShape>();
        }

        private Rect FromPoints(double x1, double y1, double x2, double y2, double dx, double dy)
        {
            double x = Math.Min(x1 + dx, x2 + dx);
            double y = Math.Min(y1 + dy, y2 + dy);
            double width = Math.Abs(Math.Max(x1 + dx, x2 + dx) - x);
            double height = Math.Abs(Math.Max(y1 + dy, y2 + dy) - y);
            return new Rect(x, y, width, height);
        }

        private WpfBrushCache GetOrCreateCache(DrawStyle style)
        {
            WpfBrushCache cache;
            if (!_brushCache.TryGetValue(style, out cache))
            {
                _brushCache[style] = WpfBrushCache.FromDrawStyle(style);
                return _brushCache[style];
            }
            return cache;
        }

        private MatrixTransform ToMatrixTransform(MatrixObject matrix)
        {
            return new MatrixTransform(
                matrix.M11, matrix.M12, 
                matrix.M21, matrix.M22, 
                matrix.OffsetX, matrix.OffsetY);
        }

        public override void PushMatrix(object dc, MatrixObject matrix)
        {
            var _dc = dc as DrawingContext;
            _dc.PushTransform(ToMatrixTransform(matrix));
        }

        public override void PopMatrix(object dc)
        {
            var _dc = dc as DrawingContext;
            _dc.Pop();
        }

        public override void DrawLine(object dc, double x0, double y0, double x1, double y1, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            _dc.DrawLine(style.IsStroked ? cache.StrokePen : null, new Point(x0 + dx, y0 + dy), new Point(x1 + dx, y1 + dy));
        }

        public override void DrawRectangle(object dc, double tlx, double tly, double brx, double bry, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var rect = FromPoints(tlx, tly, brx, bry, dx, dy);
            _dc.DrawRectangle(style.IsFilled ? cache.Fill : null, style.IsStroked ? cache.StrokePen : null, rect);
        }

        public override void DrawEllipse(object dc, double tlx, double tly, double brx, double bry, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var rect = FromPoints(tlx, tly, brx, bry, dx, dy);
            var radiusX = rect.Width / 2;
            var radiusY = rect.Height / 2;
            var center = new Point(rect.TopLeft.X, rect.TopLeft.Y);
            center.Offset(radiusX, radiusY);
            _dc.DrawEllipse(style.IsFilled ? cache.Fill : null, style.IsStroked ? cache.StrokePen : null, center, radiusX, radiusY);
        }
    }
}
