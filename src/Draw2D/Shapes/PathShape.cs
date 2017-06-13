// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Draw2D.Core.Renderer;
using Draw2D.Core.Shape;

namespace Draw2D.Core.Shapes
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

        public override bool Invalidate(ShapeRenderer r, double dx, double dy)
        {
            bool result = base.Invalidate(r, dx, dy);

            foreach (var figure in Figures)
            {
                result |= figure.Invalidate(r, dx, dy);
            }

            if (this.IsDirty || result == true)
            {
                r.InvalidateCache(this, Style, dx, dy);
                this.IsDirty = false;
                result |= true;
            }

            return result;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            var state = base.BeginTransform(dc, r);

            var isPathSelected = r.Selected.Contains(this);

            if (Style != null)
            {
                r.DrawPath(dc, this, Style, dx, dy);
            }

            foreach (var figure in Figures)
            {
                Draw(dc, r, dx, dy, figure, isPathSelected);
            }

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r, state);
        }

        private void Draw(object dc, ShapeRenderer r, double dx, double dy, FigureShape figure, bool isPathSelected)
        {
            foreach (var shape in figure.Shapes)
            {
                switch (shape)
                {
                    case LineShape line:
                        {
                            var isSelected = r.Selected.Contains(line);

                            if (isPathSelected || isSelected || r.Selected.Contains(line.StartPoint))
                            {
                                line.StartPoint.Draw(dc, r, dx, dy);
                            }

                            if (isPathSelected || isSelected || r.Selected.Contains(line.Point))
                            {
                                line.Point.Draw(dc, r, dx, dy);
                            }

                            foreach (var point in line.Points)
                            {
                                if (isPathSelected || isSelected || r.Selected.Contains(point))
                                {
                                    point.Draw(dc, r, dx, dy);
                                }
                            }
                        }
                        break;
                    case CubicBezierShape cubic:
                        {
                            var isSelected = r.Selected.Contains(cubic);

                            if (isPathSelected || isSelected || r.Selected.Contains(cubic.StartPoint))
                            {
                                cubic.StartPoint.Draw(dc, r, dx, dy);
                            }

                            if (isPathSelected || isSelected || r.Selected.Contains(cubic.Point1))
                            {
                                cubic.Point1.Draw(dc, r, dx, dy);
                            }

                            if (isPathSelected || isSelected || r.Selected.Contains(cubic.Point2))
                            {
                                cubic.Point2.Draw(dc, r, dx, dy);
                            }

                            if (isPathSelected || isSelected || r.Selected.Contains(cubic.Point3))
                            {
                                cubic.Point3.Draw(dc, r, dx, dy);
                            }

                            foreach (var point in cubic.Points)
                            {
                                if (isPathSelected || isSelected || r.Selected.Contains(point))
                                {
                                    point.Draw(dc, r, dx, dy);
                                }
                            }
                        }
                        break;
                    case QuadraticBezierShape quadratic:
                        {
                            var isSelected = r.Selected.Contains(quadratic);

                            if (isPathSelected || isSelected || r.Selected.Contains(quadratic.StartPoint))
                            {
                                quadratic.StartPoint.Draw(dc, r, dx, dy);
                            }

                            if (isPathSelected || isSelected || r.Selected.Contains(quadratic.Point1))
                            {
                                quadratic.Point1.Draw(dc, r, dx, dy);
                            }

                            if (isPathSelected || isSelected || r.Selected.Contains(quadratic.Point2))
                            {
                                quadratic.Point2.Draw(dc, r, dx, dy);
                            }

                            foreach (var point in quadratic.Points)
                            {
                                if (isPathSelected || isSelected || r.Selected.Contains(point))
                                {
                                    point.Draw(dc, r, dx, dy);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public override void Move(ISet<BaseShape> selected, double dx, double dy)
        {
            var points = GetPoints().Distinct();

            foreach (var point in points)
            {
                if (!selected.Contains(point))
                {
                    point.Move(selected, dx, dy);
                }
            }

            base.Move(selected, dx, dy);
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
