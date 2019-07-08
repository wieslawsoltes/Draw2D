// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public class FigureShape : GroupShape, ICanvasContainer
    {
        internal static new IBounds s_bounds = new FigureBounds();
        internal static new IShapeDecorator s_decorator = new FigureDecorator();

        private bool _isFilled;
        private bool _isClosed;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }

        public FigureShape()
            : base()
        {
        }

        public FigureShape(IList<IBaseShape> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public FigureShape(string title)
            : this()
        {
            this.Title = title;
        }

        public FigureShape(string title, IList<IBaseShape> shapes)
            : base()
        {
            this.Title = title;
            this.Shapes = shapes;
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

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new FigureShape()
            {
                Name = this.Name,
                Title = this.Title,
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                StyleId = this.StyleId,
                IsFilled = this.IsFilled,
                IsClosed = this.IsClosed
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
