// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Draw2D.Core.Containers;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class FigureShape : ShapeObject, IShapeContainer, ICopyable<FigureShape>
    {
        private double _width;
        private double _height;
        private ObservableCollection<LineShape> _guides;
        private ObservableCollection<ShapeObject> _shapes;
        private bool _isFilled;
        private bool _isClosed;

        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => Update(ref _height, value);
        }

        public ObservableCollection<LineShape> Guides
        {
            get => _guides;
            set => Update(ref _guides, value);
        }

        public ObservableCollection<ShapeObject> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }

        public FigureShape()
            : base()
        {
            _shapes = new ObservableCollection<ShapeObject>();
        }

        public FigureShape(ObservableCollection<ShapeObject> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public FigureShape(string name)
            : this()
        {
            this.Name = name;
        }

        public FigureShape(string name, ObservableCollection<ShapeObject> shapes)
            : base()
        {
            this.Name = name;
            this.Shapes = shapes;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            foreach (var shape in Shapes)
            {
                foreach (var point in shape.GetPoints())
                {
                    yield return point;
                }
            }
        }

        public override bool Invalidate(ShapeRenderer r, double dx, double dy)
        {
            bool result = base.Invalidate(r, dx, dy);

            if (Guides != null)
            {
                foreach (var guide in Guides)
                {
                    result |= guide.Invalidate(r, dx, dy);
                }
            }

            foreach (var shape in Shapes)
            {
                result |= shape.Invalidate(r, dx, dy);
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
            base.BeginTransform(dc, r);

            if (Guides != null)
            {
                foreach (var guide in Guides)
                {
                    guide.Draw(dc, r, dx, dy);
                }
            }

            foreach (var shape in Shapes)
            {
                shape.Draw(dc, r, dx, dy);
            }

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
        }

        public FigureShape Copy()
        {
            return new FigureShape()
            {
                Style = this.Style,
                Transform = this.Transform?.Copy(),
                Width = this.Width,
                Height = this.Height,
                IsFilled = this.IsFilled,
                IsClosed = this.IsClosed
            };
        }
    }
}
