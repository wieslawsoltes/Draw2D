using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Draw2D.Models;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
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

        private Point FromPoint(PointShape point, double dx, double dy)
        {
            return new Point(point.X + dx, point.Y + dy);
        }

        private Rect FromPoints(double x1, double y1, double x2, double y2, double dx, double dy)
        {
            double x = Math.Min(x1 + dx, x2 + dx);
            double y = Math.Min(y1 + dy, y2 + dy);
            double width = Math.Abs(Math.Max(x1 + dx, x2 + dx) - x);
            double height = Math.Abs(Math.Max(y1 + dy, y2 + dy) - y);
            return new Rect(x, y, width, height);
        }

        private Rect FromRectnagle(RectangleShape rectangle, double dx, double dy)
        {
            return FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y,
                dx, dy);
        }

        private Rect FromEllipse(EllipseShape ellipse, double dx, double dy)
        {
            return FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y,
                dx, dy);
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

        public override void DrawLine(object dc, LineShape line, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            _dc.DrawLine(style.IsStroked ? cache.StrokePen : null, FromPoint(line.Start, dx, dy), FromPoint(line.End, dx, dy));
        }

        public override void DrawRectangle(object dc, RectangleShape rectangle, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var rect = FromRectnagle(rectangle, dx, dy);
            _dc.DrawRectangle(style.IsFilled ? cache.Fill : null, style.IsStroked ? cache.StrokePen : null, rect);
        }

        public override void DrawEllipse(object dc, EllipseShape ellipse, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var rect = FromEllipse(ellipse, dx, dy);
            var radiusX = rect.Width / 2;
            var radiusY = rect.Height / 2;
            var center = new Point(rect.TopLeft.X, rect.TopLeft.Y);
            center.Offset(radiusX, radiusY);
            _dc.DrawEllipse(style.IsFilled ? cache.Fill : null, style.IsStroked ? cache.StrokePen : null, center, radiusX, radiusY);
        }
    }
}
