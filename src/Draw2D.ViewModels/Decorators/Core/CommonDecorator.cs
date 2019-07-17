// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public abstract class CommonDecorator : ViewModelBase, IShapeDecorator
    {
        private readonly string _strokeStyleId;
        private readonly string _fillStyleId;
        private readonly LineShape _line;
        private readonly OvalShape _oval;
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

            _oval = new OvalShape(new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<IPointShape>(),
            };
            _oval.StartPoint.Owner = _oval;
            _oval.Point.Owner = _oval;

            _rectangle = new RectangleShape(new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<IPointShape>(),
            };
            _rectangle.StartPoint.Owner = _rectangle;
            _rectangle.Point.Owner = _rectangle;

            _text = new TextShape(new Text(), new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<IPointShape>(),
            };
            _text.StartPoint.Owner = _text;
            _text.Point.Owner = _text;
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

        internal void FillOval(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale)
        {
            _oval.StyleId = _fillStyleId;
            _oval.StartPoint.X = s.X - radius;
            _oval.StartPoint.Y = s.Y - radius;
            _oval.Point.X = s.X + radius;
            _oval.Point.Y = s.Y + radius;
            _oval.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillOval(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale)
        {
            _oval.StyleId = _fillStyleId;
            _oval.StartPoint.X = sx - radius;
            _oval.StartPoint.Y = sy - radius;
            _oval.Point.X = sx + radius;
            _oval.Point.Y = sy + radius;
            _oval.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawOval(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale)
        {
            _oval.StyleId = _strokeStyleId;
            _oval.StartPoint.X = s.X - radius;
            _oval.StartPoint.Y = s.Y - radius;
            _oval.Point.X = s.X + radius;
            _oval.Point.Y = s.Y + radius;
            _oval.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawOval(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale)
        {
            _oval.StyleId = _strokeStyleId;
            _oval.StartPoint.X = sx - radius;
            _oval.StartPoint.Y = sy - radius;
            _oval.Point.X = sx + radius;
            _oval.Point.Y = sy + radius;
            _oval.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillOval(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _oval.StyleId = _fillStyleId;
            _oval.StartPoint.X = a.X;
            _oval.StartPoint.Y = a.Y;
            _oval.Point.X = b.X;
            _oval.Point.Y = b.Y;
            _oval.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillOval(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _oval.StyleId = _fillStyleId;
            _oval.StartPoint.X = ax;
            _oval.StartPoint.Y = ay;
            _oval.Point.X = bx;
            _oval.Point.Y = by;
            _oval.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawOval(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _oval.StyleId = _strokeStyleId;
            _oval.StartPoint.X = a.X;
            _oval.StartPoint.Y = a.Y;
            _oval.Point.X = b.X;
            _oval.Point.Y = b.Y;
            _oval.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawOval(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _oval.StyleId = _strokeStyleId;
            _oval.StartPoint.X = ax;
            _oval.StartPoint.Y = ay;
            _oval.Point.X = bx;
            _oval.Point.Y = by;
            _oval.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.StartPoint.X = s.X - radius;
            _rectangle.StartPoint.Y = s.Y - radius;
            _rectangle.Point.X = s.X + radius;
            _rectangle.Point.Y = s.Y + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.StartPoint.X = sx - radius;
            _rectangle.StartPoint.Y = sy - radius;
            _rectangle.Point.X = sx + radius;
            _rectangle.Point.Y = sy + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.StartPoint.X = s.X - radius;
            _rectangle.StartPoint.Y = s.Y - radius;
            _rectangle.Point.X = s.X + radius;
            _rectangle.Point.Y = s.Y + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.StartPoint.X = sx - radius;
            _rectangle.StartPoint.Y = sy - radius;
            _rectangle.Point.X = sx + radius;
            _rectangle.Point.Y = sy + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.StartPoint.X = a.X;
            _rectangle.StartPoint.Y = a.Y;
            _rectangle.Point.X = b.X;
            _rectangle.Point.Y = b.Y;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.StartPoint.X = ax;
            _rectangle.StartPoint.Y = ay;
            _rectangle.Point.X = bx;
            _rectangle.Point.Y = by;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.StartPoint.X = a.X;
            _rectangle.StartPoint.Y = a.Y;
            _rectangle.Point.X = b.X;
            _rectangle.Point.Y = b.Y;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.StartPoint.X = ax;
            _rectangle.StartPoint.Y = ay;
            _rectangle.Point.X = bx;
            _rectangle.Point.Y = by;
            _rectangle.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawText(object dc, IShapeRenderer renderer, string text, IPointShape a, IPointShape b, double dx, double dy, double scale)
        {
            _text.StyleId = _strokeStyleId;
            _text.StartPoint.X = a.X;
            _text.StartPoint.Y = a.Y;
            _text.Point.X = b.X;
            _text.Point.Y = b.Y;
            _text.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawText(object dc, IShapeRenderer renderer, string text, double ax, double ay, double bx, double by, double dx, double dy, double scale)
        {
            _text.StyleId = _strokeStyleId;
            _text.StartPoint.X = ax;
            _text.StartPoint.Y = ay;
            _text.Point.X = bx;
            _text.Point.Y = by;
            _text.Draw(dc, renderer, dx, dy, scale, null, null);
        }

        internal void DrawBoxFromPoints(object dc, IShapeRenderer renderer, IBaseShape shape, double dx, double dy, double scale)
        {
            var box = new Box(shape);
            if (box.points.Count >= 2)
            {
                DrawRectangle(dc, renderer, box.ax, box.ay, box.bx, box.by, dx, dy, scale);
            }
        }

        public abstract void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale);
    }
}
