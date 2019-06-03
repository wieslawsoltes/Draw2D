// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Spatial;
using Spatial.ConvexHull;
using Spatial.DouglasPeucker;
using Spatial.Sat;

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

        internal void DrawLine(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale, DrawMode mode)
        {
            _line.StyleId = _strokeStyleId;
            _line.StartPoint.X = a.X;
            _line.StartPoint.Y = a.Y;
            _line.Point.X = b.X;
            _line.Point.Y = b.Y;
            _line.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawLine(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale, DrawMode mode)
        {
            _line.StyleId = _strokeStyleId;
            _line.StartPoint.X = ax;
            _line.StartPoint.Y = ay;
            _line.Point.X = bx;
            _line.Point.Y = by;
            _line.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void FillEllipse(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale, DrawMode mode)
        {
            _ellipse.StyleId = _fillStyleId;
            _ellipse.TopLeft.X = s.X - radius;
            _ellipse.TopLeft.Y = s.Y - radius;
            _ellipse.BottomRight.X = s.X + radius;
            _ellipse.BottomRight.Y = s.Y + radius;
            _ellipse.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void FillEllipse(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale, DrawMode mode)
        {
            _ellipse.StyleId = _fillStyleId;
            _ellipse.TopLeft.X = sx - radius;
            _ellipse.TopLeft.Y = sy - radius;
            _ellipse.BottomRight.X = sx + radius;
            _ellipse.BottomRight.Y = sy + radius;
            _ellipse.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawEllipse(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale, DrawMode mode)
        {
            _ellipse.StyleId = _strokeStyleId;
            _ellipse.TopLeft.X = s.X - radius;
            _ellipse.TopLeft.Y = s.Y - radius;
            _ellipse.BottomRight.X = s.X + radius;
            _ellipse.BottomRight.Y = s.Y + radius;
            _ellipse.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawEllipse(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale, DrawMode mode)
        {
            _ellipse.StyleId = _strokeStyleId;
            _ellipse.TopLeft.X = sx - radius;
            _ellipse.TopLeft.Y = sy - radius;
            _ellipse.BottomRight.X = sx + radius;
            _ellipse.BottomRight.Y = sy + radius;
            _ellipse.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void FillEllipse(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale, DrawMode mode)
        {
            _ellipse.StyleId = _fillStyleId;
            _ellipse.TopLeft.X = a.X;
            _ellipse.TopLeft.Y = a.Y;
            _ellipse.BottomRight.X = b.X;
            _ellipse.BottomRight.Y = b.Y;
            _ellipse.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void FillEllipse(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale, DrawMode mode)
        {
            _ellipse.StyleId = _fillStyleId;
            _ellipse.TopLeft.X = ax;
            _ellipse.TopLeft.Y = ay;
            _ellipse.BottomRight.X = bx;
            _ellipse.BottomRight.Y = by;
            _ellipse.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawEllipse(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale, DrawMode mode)
        {
            _ellipse.StyleId = _strokeStyleId;
            _ellipse.TopLeft.X = a.X;
            _ellipse.TopLeft.Y = a.Y;
            _ellipse.BottomRight.X = b.X;
            _ellipse.BottomRight.Y = b.Y;
            _ellipse.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawEllipse(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale, DrawMode mode)
        {
            _ellipse.StyleId = _strokeStyleId;
            _ellipse.TopLeft.X = ax;
            _ellipse.TopLeft.Y = ay;
            _ellipse.BottomRight.X = bx;
            _ellipse.BottomRight.Y = by;
            _ellipse.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale, DrawMode mode)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.TopLeft.X = s.X - radius;
            _rectangle.TopLeft.Y = s.Y - radius;
            _rectangle.BottomRight.X = s.X + radius;
            _rectangle.BottomRight.Y = s.Y + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale, DrawMode mode)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.TopLeft.X = sx - radius;
            _rectangle.TopLeft.Y = sy - radius;
            _rectangle.BottomRight.X = sx + radius;
            _rectangle.BottomRight.Y = sy + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, IPointShape s, double radius, double dx, double dy, double scale, DrawMode mode)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.TopLeft.X = s.X - radius;
            _rectangle.TopLeft.Y = s.Y - radius;
            _rectangle.BottomRight.X = s.X + radius;
            _rectangle.BottomRight.Y = s.Y + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, double sx, double sy, double radius, double dx, double dy, double scale, DrawMode mode)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.TopLeft.X = sx - radius;
            _rectangle.TopLeft.Y = sy - radius;
            _rectangle.BottomRight.X = sx + radius;
            _rectangle.BottomRight.Y = sy + radius;
            _rectangle.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale, DrawMode mode)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.TopLeft.X = a.X;
            _rectangle.TopLeft.Y = a.Y;
            _rectangle.BottomRight.X = b.X;
            _rectangle.BottomRight.Y = b.Y;
            _rectangle.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void FillRectangle(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale, DrawMode mode)
        {
            _rectangle.StyleId = _fillStyleId;
            _rectangle.TopLeft.X = ax;
            _rectangle.TopLeft.Y = ay;
            _rectangle.BottomRight.X = bx;
            _rectangle.BottomRight.Y = by;
            _rectangle.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, IPointShape a, IPointShape b, double dx, double dy, double scale, DrawMode mode)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.TopLeft.X = a.X;
            _rectangle.TopLeft.Y = a.Y;
            _rectangle.BottomRight.X = b.X;
            _rectangle.BottomRight.Y = b.Y;
            _rectangle.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawRectangle(object dc, IShapeRenderer renderer, double ax, double ay, double bx, double by, double dx, double dy, double scale, DrawMode mode)
        {
            _rectangle.StyleId = _strokeStyleId;
            _rectangle.TopLeft.X = ax;
            _rectangle.TopLeft.Y = ay;
            _rectangle.BottomRight.X = bx;
            _rectangle.BottomRight.Y = by;
            _rectangle.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawText(object dc, IShapeRenderer renderer, string text, IPointShape a, IPointShape b, double dx, double dy, double scale, DrawMode mode)
        {
            _text.StyleId = _strokeStyleId;
            _text.TopLeft.X = a.X;
            _text.TopLeft.Y = a.Y;
            _text.BottomRight.X = b.X;
            _text.BottomRight.Y = b.Y;
            _text.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawText(object dc, IShapeRenderer renderer, string text, double ax, double ay, double bx, double by, double dx, double dy, double scale, DrawMode mode)
        {
            _text.StyleId = _strokeStyleId;
            _text.TopLeft.X = ax;
            _text.TopLeft.Y = ay;
            _text.BottomRight.X = bx;
            _text.BottomRight.Y = by;
            _text.Draw(dc, renderer, dx, dy, scale, mode, null, null);
        }

        internal void DrawBoxFromPoints(object dc, IShapeRenderer renderer, IBaseShape shape, double dx, double dy, double scale, DrawMode mode)
        {
            var points = new List<IPointShape>();
            shape.GetPoints(points);

            if (points.Count >= 2)
            {
                points.GetBox(out double ax, out double ay, out double bx, out double by);
                DrawRectangle(dc, renderer, ax, ay, bx, by, dx, dy, scale, mode);
            }
        }

        public abstract void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode);
    }

    [DataContract(IsReference = true)]
    public class ConicDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, ConicShape conic, double dx, double dy, double scale, DrawMode mode)
        {
            DrawLine(dc, renderer, conic.StartPoint, conic.Point1, dx, dy, scale, mode);
            DrawLine(dc, renderer, conic.Point1, conic.Point2, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is ConicShape conic)
            {
                Draw(dc, renderer, conic, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class ContainerDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, ICanvasContainer container, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (selectionState.IsSelected(container))
            {
                DrawBoxFromPoints(dc, renderer, container, dx, dy, scale, mode);
            }
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is ICanvasContainer container)
            {
                Draw(dc, renderer, container, selectionState, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class CubicBezierDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, CubicBezierShape cubicBezier, double dx, double dy, double scale, DrawMode mode)
        {
            DrawLine(dc, renderer, cubicBezier.StartPoint, cubicBezier.Point1, dx, dy, scale, mode);
            DrawLine(dc, renderer, cubicBezier.Point3, cubicBezier.Point2, dx, dy, scale, mode);
            DrawLine(dc, renderer, cubicBezier.Point1, cubicBezier.Point2, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is CubicBezierShape cubicBezier)
            {
                Draw(dc, renderer, cubicBezier, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, EllipseShape ellipseShape, double dx, double dy, double scale, DrawMode mode)
        {
            DrawRectangle(dc, renderer, ellipseShape.TopLeft, ellipseShape.BottomRight, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is EllipseShape ellipseShape)
            {
                Draw(dc, renderer, ellipseShape, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class LineDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, LineShape lineShape, double dx, double dy, double scale, DrawMode mode)
        {
            DrawRectangle(dc, renderer, lineShape.StartPoint, lineShape.Point, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is LineShape lineShape)
            {
                Draw(dc, renderer, lineShape, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class GroupDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, GroupShape group, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (selectionState.IsSelected(group))
            {
                DrawBoxFromPoints(dc, renderer, group, dx, dy, scale, mode);
            }
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is GroupShape group)
            {
                Draw(dc, renderer, group, selectionState, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class FigureDecorator : CommonDecorator
    {
        private readonly LineDecorator _lineDecorator;
        private readonly CubicBezierDecorator _cubiceBezierDecorator;
        private readonly QuadraticBezierDecorator _quadraticBezierDecorator;
        private readonly ConicDecorator _conicDecorator;

        public FigureDecorator()
        {
            _lineDecorator = new LineDecorator();
            _cubiceBezierDecorator = new CubicBezierDecorator();
            _quadraticBezierDecorator = new QuadraticBezierDecorator();
            _conicDecorator = new ConicDecorator();
        }

        public void DrawShape(object dc, IShapeRenderer renderer, IBaseShape shape, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is LineShape line)
            {
                if (selectionState.IsSelected(line))
                {
                    _lineDecorator.Draw(dc, line, renderer, selectionState, dx, dy, scale, mode);
                }
            }
            else if (shape is CubicBezierShape cubicBezier)
            {
                if (selectionState.IsSelected(cubicBezier))
                {
                    _cubiceBezierDecorator.Draw(dc, cubicBezier, renderer, selectionState, dx, dy, scale, mode);
                }
            }
            else if (shape is QuadraticBezierShape quadraticBezier)
            {
                if (selectionState.IsSelected(quadraticBezier))
                {
                    _quadraticBezierDecorator.Draw(dc, quadraticBezier, renderer, selectionState, dx, dy, scale, mode);
                }
            }
            else if (shape is ConicShape conicShape)
            {
                if (selectionState.IsSelected(conicShape))
                {
                    _conicDecorator.Draw(dc, conicShape, renderer, selectionState, dx, dy, scale, mode);
                }
            }
        }

        public void DrawFigure(object dc, IShapeRenderer renderer, FigureShape figure, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (selectionState.IsSelected(figure))
            {
                DrawBoxFromPoints(dc, renderer, figure, dx, dy, scale, mode);
            }

            foreach (var shape in figure.Shapes)
            {
                DrawShape(dc, renderer, shape, selectionState, dx, dy, scale, mode);
            }
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is FigureShape figure)
            {
                DrawFigure(dc, renderer, figure, selectionState, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class PathDecorator : CommonDecorator
    {
        public PathDecorator()
        {
        }

        public void Draw(object dc, IShapeRenderer renderer, PathShape path, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (selectionState.IsSelected(path))
            {
                DrawBoxFromPoints(dc, renderer, path, dx, dy, scale, mode);
            }

            foreach (var shape in path.Shapes)
            {
                shape.Decorator?.Draw(dc, shape, renderer, selectionState, dx, dy, scale, mode);
            }
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is PathShape path)
            {
                Draw(dc, renderer, path, selectionState, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class PointDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, IPointShape pointShape, double dx, double dy, double scale, DrawMode mode)
        {
            DrawRectangle(dc, renderer, pointShape, 10, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
#if USE_POINT_DECORATOR
            if (shape is IPointShape pointShape)
            {
                Draw(dc, renderer, pointShape, dx, dy, scale, mode);
            }
#endif
        }
    }

    [DataContract(IsReference = true)]
    public class QuadraticBezierDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, QuadraticBezierShape quadraticBezier, double dx, double dy, double scale, DrawMode mode)
        {
            DrawLine(dc, renderer, quadraticBezier.StartPoint, quadraticBezier.Point1, dx, dy, scale, mode);
            DrawLine(dc, renderer, quadraticBezier.Point1, quadraticBezier.Point2, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is QuadraticBezierShape quadraticBezier)
            {
                Draw(dc, renderer, quadraticBezier, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, RectangleShape rectangleShape, double dx, double dy, double scale, DrawMode mode)
        {
            DrawRectangle(dc, renderer, rectangleShape.TopLeft, rectangleShape.BottomRight, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is RectangleShape rectangleShape)
            {
                Draw(dc, renderer, rectangleShape, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class ReferenceDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, ReferenceShape reference, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (selectionState.IsSelected(reference) && reference.Template != null)
            {
                DrawBoxFromPoints(dc, renderer, reference.Template, dx + reference.X, dy + reference.Y, scale, mode);
            }
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is ReferenceShape reference)
            {
                Draw(dc, renderer, reference, selectionState, dx, dy, scale, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class TextDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, TextShape textShape, double dx, double dy, double scale, DrawMode mode)
        {
            DrawRectangle(dc, renderer, textShape.TopLeft, textShape.BottomRight, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is TextShape textShape)
            {
                Draw(dc, renderer, textShape, dx, dy, scale, mode);
            }
        }
    }
}
