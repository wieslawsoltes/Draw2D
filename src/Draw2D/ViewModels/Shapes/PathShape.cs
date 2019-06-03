// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public class PathShape : GroupShape, ICanvasContainer
    {
        internal static new IBounds s_bounds = new PathBounds();
        internal static new IShapeDecorator s_decorator = new PathDecorator();

        private PathFillRule _fillRule;
        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathFillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public PathShape()
            : base()
        {
        }

        public PathShape(IList<IBaseShape> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public PathShape(string title, IList<IBaseShape> shapes)
            : base()
        {
            this.Title = title;
            this.Shapes = shapes;
        }

        public IPointShape GetFirstPoint()
        {
            if (Shapes.Count > 0)
            {
                var lastShape = Shapes[Shapes.Count - 1];
                if (lastShape is FigureShape lastFigure)
                {
                    var shapes = lastFigure.Shapes;
                    if (shapes.Count > 0)
                    {
                        switch (shapes[0])
                        {
                            case LineShape line:
                                return line.StartPoint;
                            case CubicBezierShape cubicBezier:
                                return cubicBezier.StartPoint;
                            case QuadraticBezierShape quadraticBezier:
                                return quadraticBezier.StartPoint;
                            case ConicShape conic:
                                return conic.StartPoint;
                            default:
                                throw new Exception("Could not find last path point.");
                        }
                    }
                }
            }
            return null;
        }

        public IPointShape GetLastPoint()
        {
            if (Shapes.Count > 0)
            {
                var shape = Shapes[Shapes.Count - 1];
                if (shape is FigureShape lastFigure)
                {
                    var lastFigureShapes = lastFigure.Shapes;
                    if (lastFigureShapes.Count > 0)
                    {
                        switch (lastFigureShapes[lastFigureShapes.Count - 1])
                        {
                            case LineShape line:
                                return line.Point;
                            case CubicBezierShape cubicBezier:
                                return cubicBezier.Point3;
                            case QuadraticBezierShape quadraticBezier:
                                return quadraticBezier.Point2;
                            case ConicShape conic:
                                return conic.Point2;
                            default:
                                throw new Exception("Could not find last path point.");
                        }
                    }
                }
            }
            return null;
        }

        public override void Invalidate()
        {
            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            var isPathSelected = renderer.SelectionState?.IsSelected(this) ?? false;

            if (StyleId != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawPath(dc, this, StyleId, dx, dy, scale);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                foreach (var shape in Shapes)
                {
                    if (shape is FigureShape figure)
                    {
                        DrawPoints(dc, renderer, dx, dy, scale, mode, db, r, figure, isPathSelected);
                    }
                }
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                foreach (var point in Points)
                {
                    if (renderer.SelectionState?.IsSelected(point) ?? false)
                    {
                        point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                    }
                }
            }
        }

        private void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r, FigureShape figure, bool isPathSelected)
        {
            foreach (var shape in figure.Shapes)
            {
                switch (shape)
                {
                    case LineShape line:
                        {
                            var isSelected = renderer.SelectionState?.IsSelected(line) ?? false;

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(line.StartPoint) ?? false))
                            {
                                line.StartPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(line.Point) ?? false))
                            {
                                line.Point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            foreach (var point in line.Points)
                            {
                                if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(point) ?? false))
                                {
                                    point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                                }
                            }
                        }
                        break;
                    case CubicBezierShape cubic:
                        {
                            var isSelected = renderer.SelectionState?.IsSelected(cubic) ?? false;

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(cubic.StartPoint) ?? false))
                            {
                                cubic.StartPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(cubic.Point1) ?? false))
                            {
                                cubic.Point1.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(cubic.Point2) ?? false))
                            {
                                cubic.Point2.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(cubic.Point3) ?? false))
                            {
                                cubic.Point3.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            foreach (var point in cubic.Points)
                            {
                                if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(point) ?? false))
                                {
                                    point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                                }
                            }
                        }
                        break;
                    case QuadraticBezierShape quadratic:
                        {
                            var isSelected = renderer.SelectionState?.IsSelected(quadratic) ?? false;

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(quadratic.StartPoint) ?? false))
                            {
                                quadratic.StartPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(quadratic.Point1) ?? false))
                            {
                                quadratic.Point1.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(quadratic.Point2) ?? false))
                            {
                                quadratic.Point2.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            foreach (var point in quadratic.Points)
                            {
                                if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(point) ?? false))
                                {
                                    point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                                }
                            }
                        }
                        break;
                    case ConicShape conic:
                        {
                            var isSelected = renderer.SelectionState?.IsSelected(conic) ?? false;

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(conic.StartPoint) ?? false))
                            {
                                conic.StartPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(conic.Point1) ?? false))
                            {
                                conic.Point1.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(conic.Point2) ?? false))
                            {
                                conic.Point2.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            foreach (var point in conic.Points)
                            {
                                if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(point) ?? false))
                                {
                                    point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);

            var points = new List<IPointShape>();
            GetPoints(points);

            foreach (var point in points)
            {
                point.Select(selectionState);
            }
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);

            var points = new List<IPointShape>();
            GetPoints(points);

            foreach (var point in points)
            {
                point.Deselect(selectionState);
            }
        }

        public bool Validate(bool removeEmptyFigures)
        {
            var figures = new List<FigureShape>();

            foreach (var shape in Shapes)
            {
                if (shape is FigureShape figure)
                {
                    figures.Add(figure);
                }
            }

            if (figures.Count > 0 && figures[0].Shapes.Count > 0)
            {
                if (removeEmptyFigures == true)
                {
                    foreach (var figure in figures)
                    {
                        if (figure.Shapes.Count <= 0)
                        {
                            Shapes.Remove(figure);
                            this.MarkAsDirty(true);
                        }
                    }
                }

                if (Shapes.Count > 0 && Shapes[0] is FigureShape figureShape && figureShape.Shapes.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new PathShape()
            {
                Name = this.Name,
                Title = this.Title,
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                StyleId = this.StyleId,
                FillRule = this.FillRule,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                foreach (var shape in this.Shapes)
                {
                    copy.Shapes.Add((IBaseShape)(shape.Copy(shared)));
                }

                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }
}
