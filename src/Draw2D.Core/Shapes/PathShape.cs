﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class PathShape : ConnectableShape
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

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            var isPathSelected = r.Selected.Contains(this);

            if (Style != null)
            {
                r.DrawPath(dc, this, Style, dx, dy);
            }

            foreach (var figure in Figures)
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

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
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
    }
}