// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Draw2D.ViewModels.Shapes
{
    public class PathShape : ConnectableShape, ICopyable
    {
        private ObservableCollection<FigureShape> _figures;
        private PathFillRule _fillRule;

        public ObservableCollection<FigureShape> Figures
        {
            get => _figures;
            set => Update(ref _figures, value);
        }

        public PathFillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
        }

        public PathShape()
            : base()
        {
            _figures = new ObservableCollection<FigureShape>();
        }

        public PathShape(ObservableCollection<FigureShape> figures)
            : base()
        {
            this.Figures = figures;
        }

        public PathShape(string name)
            : this()
        {
            this.Name = name;
        }

        public PathShape(string name, ObservableCollection<FigureShape> figures)
            : base()
        {
            this.Name = name;
            this.Figures = figures;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            foreach (var point in Points)
            {
                yield return point;
            }

            foreach (var figure in Figures)
            {
                foreach (var point in figure.GetPoints())
                {
                    yield return point;
                }
            }
        }

        public override bool Invalidate(IShapeRenderer renderer, double dx, double dy)
        {
            bool result = base.Invalidate(renderer, dx, dy);

            foreach (var figure in Figures)
            {
                result |= figure.Invalidate(renderer, dx, dy);
            }

            if (this.IsDirty || result == true)
            {
                renderer.InvalidateCache(this, Style, dx, dy);
                this.IsDirty = false;
                result |= true;
            }

            return result;
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            var state = base.BeginTransform(dc, renderer);

            var isPathSelected = renderer.Selection.Selected.Contains(this);

            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawPath(dc, this, Style, dx, dy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                foreach (var figure in Figures)
                {
                    DrawPoints(dc, renderer, dx, dy, db, mode, r, figure, isPathSelected);
                }
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
            base.EndTransform(dc, renderer, state);
        }

        private void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r, FigureShape figure, bool isPathSelected)
        {
            foreach (var shape in figure.Shapes)
            {
                switch (shape)
                {
                    case LineShape line:
                        {
                            var isSelected = renderer.Selection.Selected.Contains(line);

                            if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(line.StartPoint))
                            {
                                line.StartPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(line.Point))
                            {
                                line.Point.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            foreach (var point in line.Points)
                            {
                                if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(point))
                                {
                                    point.Draw(dc, renderer, dx, dy, mode, db, r);
                                }
                            }
                        }
                        break;
                    case CubicBezierShape cubic:
                        {
                            var isSelected = renderer.Selection.Selected.Contains(cubic);

                            if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(cubic.StartPoint))
                            {
                                cubic.StartPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(cubic.Point1))
                            {
                                cubic.Point1.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(cubic.Point2))
                            {
                                cubic.Point2.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(cubic.Point3))
                            {
                                cubic.Point3.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            foreach (var point in cubic.Points)
                            {
                                if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(point))
                                {
                                    point.Draw(dc, renderer, dx, dy, mode, db, r);
                                }
                            }
                        }
                        break;
                    case QuadraticBezierShape quadratic:
                        {
                            var isSelected = renderer.Selection.Selected.Contains(quadratic);

                            if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(quadratic.StartPoint))
                            {
                                quadratic.StartPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(quadratic.Point1))
                            {
                                quadratic.Point1.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(quadratic.Point2))
                            {
                                quadratic.Point2.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            foreach (var point in quadratic.Points)
                            {
                                if (isPathSelected || isSelected || renderer.Selection.Selected.Contains(point))
                                {
                                    point.Draw(dc, renderer, dx, dy, mode, db, r);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public override void Move(ISelection selection, double dx, double dy)
        {
            var points = GetPoints().Distinct();

            foreach (var point in points)
            {
                if (!selection.Selected.Contains(point))
                {
                    point.Move(selection, dx, dy);
                }
            }

            base.Move(selection, dx, dy);
        }

        public bool Validate(bool removeEmptyFigures)
        {
            if (_figures.Count > 0 && _figures[0].Shapes.Count > 0)
            {
                var figures = _figures.ToList();

                if (removeEmptyFigures == true)
                {
                    foreach (var figure in figures)
                    {
                        if (figure.Shapes.Count <= 0)
                        {
                            _figures.Remove(figure);
                        }
                    }
                }

                if (_figures.Count > 0 && _figures[0].Shapes.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public object Copy(IDictionary<object, object> shared)
        {
            var copy = new PathShape()
            {
                Style = this.Style,
                Transform = (MatrixObject)this.Transform?.Copy(shared),
                FillRule = this.FillRule
            };

            if (shared != null)
            {
                foreach (var figure in this.Figures)
                {
                    copy.Figures.Add((FigureShape)figure.Copy(shared));
                }

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }
}
