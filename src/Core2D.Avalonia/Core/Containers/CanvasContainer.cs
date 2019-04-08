// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    public class CanvasContainer : BaseShape, ICopyable
    {
        private double _width;
        private double _height;
        private ArgbColor _printBackground;
        private ArgbColor _workBackground;
        private ArgbColor _inputBackground;
        private ObservableCollection<LineShape> _guides;
        private ObservableCollection<BaseShape> _shapes;
        private ObservableCollection<ShapeStyle> _styles;

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

        public ArgbColor PrintBackground
        {
            get => _printBackground;
            set => Update(ref _printBackground, value);
        }

        public ArgbColor WorkBackground
        {
            get => _workBackground;
            set => Update(ref _workBackground, value);
        }

        public ArgbColor InputBackground
        {
            get => _inputBackground;
            set => Update(ref _inputBackground, value);
        }

        public ObservableCollection<LineShape> Guides
        {
            get => _guides;
            set => Update(ref _guides, value);
        }

        public ObservableCollection<BaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public ObservableCollection<ShapeStyle> Styles
        {
            get => _styles;
            set => Update(ref _styles, value);
        }

        public CanvasContainer()
        {
            _guides = new ObservableCollection<LineShape>();
            _shapes = new ObservableCollection<BaseShape>();
            _styles = new ObservableCollection<ShapeStyle>();
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

        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var state = BeginTransform(dc, renderer);

            if (Guides != null)
            {
                foreach (var shape in Guides)
                {
                    shape.Draw(dc, renderer, dx, dy, db, r);
                }
            }

            foreach (var shape in Shapes)
            {
                shape.Draw(dc, renderer, dx, dy, db, r);
            }

            EndTransform(dc, renderer, state);
        }

        public override bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            bool result = false;

            var points = GetPoints();

            if (Guides != null)
            {
                foreach (var guide in Guides)
                {
                    result |= guide.Invalidate(renderer, dx, dy);
                }
            }

            foreach (var shape in Shapes)
            {
                result |= shape.Invalidate(renderer, dx, dy);
            }

            foreach (var point in points)
            {
                point.IsDirty = false;
            }

            return result;
        }

        public virtual object Copy(IDictionary<object, object> shared)
        {
            var copy = new CanvasContainer()
            {
                Name = this.Name,
                Style = this.Style,
                Transform = (MatrixObject)this.Transform?.Copy(shared),
                Width = this.Width,
                Height = this.Height,
            };

            if (shared != null)
            {
                foreach (var guide in this.Guides)
                {
                    copy.Guides.Add((LineShape)guide.Copy(shared));
                }

                foreach (var shape in this.Shapes)
                {
                    if (shape is ICopyable copyable)
                    {
                        copy.Shapes.Add((BaseShape)copyable.Copy(shared));
                    }
                }
            }

            return copy;
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
        }
    }
}
