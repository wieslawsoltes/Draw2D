// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public class PathShape : GroupShape, ICanvasContainer
    {
        public static PathFillType[] PathFillTypeValues { get; } = (PathFillType[])Enum.GetValues(typeof(PathFillType));

        internal static new IBounds s_bounds = new PathBounds();
        internal static new IShapeDecorator s_decorator = new PathDecorator();

        private PathFillType _fillType;
        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathFillType FillType
        {
            get => _fillType;
            set => Update(ref _fillType, value);
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

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r)
        {
            if (StyleId != null)
            {
                renderer.DrawPath(dc, this, StyleId, dx, dy, scale);
            }
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);

            //var points = new List<IPointShape>();
            //GetPoints(points);
            //
            //foreach (var point in points)
            //{
            //    point.Select(selectionState);
            //}
            foreach (var shape in Shapes)
            {
                shape.Select(selectionState);
            }
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);

            //var points = new List<IPointShape>();
            //GetPoints(points);
            //
            //foreach (var point in points)
            //{
            //    point.Deselect(selectionState);
            //}
            foreach (var shape in Shapes)
            {
                shape.Deselect(selectionState);
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
                Effects = (IPaintEffects)this.Effects?.Copy(shared),
                FillType = this.FillType,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                foreach (var point in this.Points)
                {
                    var pointCopy = (IPointShape)shared[point];
                    pointCopy.Owner = copy;
                    copy.Points.Add(pointCopy);
                }

                foreach (var shape in this.Shapes)
                {
                    var shapeCopy = (IBaseShape)(shape.Copy(shared));
                    shapeCopy.Owner = copy;
                    copy.Shapes.Add(shapeCopy);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }
}
