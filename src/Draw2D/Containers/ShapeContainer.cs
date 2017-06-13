// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Renderer;
using Draw2D.Shape;
using Draw2D.Shapes;
using Draw2D.Style;

namespace Draw2D.Containers
{
    public class ShapeContainer : ObservableObject, IShapeContainer, ICopyable
    {
        private double _width;
        private double _height;
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

        public ShapeContainer()
        {
            _guides = new ObservableCollection<LineShape>();
            _shapes = new ObservableCollection<BaseShape>();
            _styles = new ObservableCollection<ShapeStyle>();
        }

        public IEnumerable<PointShape> GetPoints()
        {
            foreach (var shape in Shapes)
            {
                foreach (var point in shape.GetPoints())
                {
                    yield return point;
                }
            }
        }

        public bool Invalidate(ShapeRenderer renderer, double dx, double dy)
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

        public void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
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
        }

        public object Copy(IDictionary<object, object> shared)
        {
            var copy = new ShapeContainer()
            {
                Name = this.Name,
                Width = this.Width,
                Height = this.Height
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
    }
}
