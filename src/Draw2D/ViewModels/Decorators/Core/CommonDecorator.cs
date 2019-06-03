// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public abstract class CommonDecorator : ViewModelBase, IShapeDecorator
    {
        private readonly string _strokeStyleId;
        private readonly string _fillStyleId;
        private readonly LineShape _line;
        private readonly EllipseShape _ellipse;
        private readonly RectangleShape _rectangle;
        private readonly TextShape _text;

        public CommonDecorator()
        {
            _strokeStyleId = "Decorator-Stroke";
            _fillStyleId = "Decorator-Fill";

            _line = new LineShape(new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<IPointShape>()
            };
            _line.StartPoint.Owner = _line;
            _line.Point.Owner = _line;

            _ellipse = new EllipseShape(new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<IPointShape>(),
            };
            _ellipse.TopLeft.Owner = _ellipse;
            _ellipse.BottomRight.Owner = _ellipse;

            _rectangle = new RectangleShape(new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<IPointShape>(),
            };
            _rectangle.TopLeft.Owner = _rectangle;
            _rectangle.BottomRight.Owner = _rectangle;

            _text = new TextShape(new Text(), new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<IPointShape>(),
            };
            _text.TopLeft.Owner = _text;
            _text.BottomRight.Owner = _text;
        }

        internal void DrawLine(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _line.StyleId = _strokeStyleId;
            _line.StartPoint.X = a.X;
            _line.StartPoint.Y = a.Y;
            _line.Point.X = b.X;
            _line.Point.Y = b.Y;
            _line.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawLine(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _line.StyleId = _strokeStyleId;
            _line.StartPoint.X = ax;
            _line.StartPoint.Y = ay;
            _line.Point.X = bx;
            _line.Point.Y = by;
            _line.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillEllipse(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale)
        {
            _ellipse.StyleId = _fillStyleId;
            _ellipse.TopLeft.X = s.X - radius;
            _ellipse.TopLeft.Y = s.Y - radius;
            _ellipse.BottomRight.X = s.X + radius;
            _ellipse.BottomRight.Y = s.Y + radius;
            _ellipse.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillEllipse(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale)
        {
            _ellipse.StyleId = _fillStyleId;
            _ellipse.TopLeft.X = sx - radius;
            _ellipse.TopLeft.Y = sy - radius;
            _ellipse.BottomRight.X = sx + radius;
            _ellipse.BottomRight.Y = sy + radius;
            _ellipse.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawEllipse(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale)
        {
            _ellipse.StyleId = _strokeStyleId;
            _ellipse.TopLeft.X = s.X - radius;
            _ellipse.TopLeft.Y = s.Y - radius;
            _ellipse.BottomRight.X = s.X + radius;
            _ellipse.BottomRight.Y = s.Y + radius;
            _ellipse.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawEllipse(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale)
        {
            _ellipse.StyleId = _strokeStyleId;
            _ellipse.TopLeft.X = sx - radius;
            _ellipse.TopLeft.Y = sy - radius;
            _ellipse.BottomRight.X = sx + radius;
            _ellipse.BottomRight.Y = sy + radius;
            _ellipse.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillEllipse(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _ellipse.StyleId = _fillStyleId;
            _ellipse.TopLeft.X = a.X;
            _ellipse.TopLeft.Y = a.Y;
            _ellipse.BottomRight.X = b.X;
            _ellipse.BottomRight.Y = b.Y;
            _ellipse.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillEllipse(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _ellipse.StyleId = _fillStyleId;
            _ellipse.TopLeft.X = ax;
            _ellipse.TopLeft.Y = ay;
            _ellipse.BottomRight.X = bx;
            _ellipse.BottomRight.Y = by;
            _ellipse.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawEllipse(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _ellipse.StyleId = _strokeStyleId;
            _ellipse.TopLeft.X = a.X;
            _ellipse.TopLeft.Y = a.Y;
            _ellipse.BottomRight.X = b.X;
            _ellipse.BottomRight.Y = b.Y;
            _ellipse.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawEllipse(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _ellipse.StyleId = _strokeStyleId;
            _ellipse.TopLeft.X = ax;
            _ellipse.TopLeft.Y = ay;
            _ellipse.BottomRight.X = bx;
            _ellipse.BottomRight.Y = by;
            _ellipse.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.TopLeft.X = s.X - radius;
            _rectangle.TopLeft.Y = s.Y - radius;
            _rectangle.BottomRight.X = s.X + radius;
            _rectangle.BottomRight.Y = s.Y + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.TopLeft.X = sx - radius;
            _rectangle.TopLeft.Y = sy - radius;
            _rectangle.BottomRight.X = sx + radius;
            _rectangle.BottomRight.Y = sy + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.TopLeft.X = s.X - radius;
            _rectangle.TopLeft.Y = s.Y - radius;
            _rectangle.BottomRight.X = s.X + radius;
            _rectangle.BottomRight.Y = s.Y + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.TopLeft.X = sx - radius;
            _rectangle.TopLeft.Y = sy - radius;
            _rectangle.BottomRight.X = sx + radius;
            _rectangle.BottomRight.Y = sy + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.TopLeft.X = a.X;
            _rectangle.TopLeft.Y = a.Y;
            _rectangle.BottomRight.X = b.X;
            _rectangle.BottomRight.Y = b.Y;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.TopLeft.X = ax;
            _rectangle.TopLeft.Y = ay;
            _rectangle.BottomRight.X = bx;
            _rectangle.BottomRight.Y = by;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.TopLeft.X = a.X;
            _rectangle.TopLeft.Y = a.Y;
            _rectangle.BottomRight.X = b.X;
            _rectangle.BottomRight.Y = b.Y;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.TopLeft.X = ax;
            _rectangle.TopLeft.Y = ay;
            _rectangle.BottomRight.X = bx;
            _rectangle.BottomRight.Y = by;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawText(object dc, IShapeRenderer renderer, string text, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _text.StyleId = _strokeStyleId;
            _text.TopLeft.X = a.X;
            _text.TopLeft.Y = a.Y;
            _text.BottomRight.X = b.X;
            _text.BottomRight.Y = b.Y;
            _text.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawText(object dc, IShapeRenderer renderer, string text, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _text.StyleId = _strokeStyleId;
            _text.TopLeft.X = ax;
            _text.TopLeft.Y = ay;
            _text.BottomRight.X = bx;
            _text.BottomRight.Y = by;
            _text.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawBoxFromPoints(object dc, IShapeRenderer renderer, IBaseShape shape, double dx, double dy, double scale)
        {
            var points = new List<IPointShape>();
            shape.GetPoints(points);

            if (points.Count >= 2)
            {
                points.GetBox(out double ax, out double ay, out double bx, out double by);
                DrawRectangle(dc, renderer, ax, ay, bx, by, dx, dy, scale);
            }
        }

        public abstract void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale);
    }
}
