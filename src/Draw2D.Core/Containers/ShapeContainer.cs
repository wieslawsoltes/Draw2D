// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Core.Containers
{
    public class ShapeContainer : NamedObject, IShapeContainer, ICopyable
    {
        private double _width;
        private double _height;
        private ObservableCollection<LineShape> _guides;
        private ObservableCollection<ShapeObject> _shapes;
        private ObservableCollection<DrawStyle> _styles;

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

        public ObservableCollection<DrawStyle> Styles
        {
            get => _styles;
            set => Update(ref _styles, value);
        }

        public ShapeContainer()
        {
            _guides = new ObservableCollection<LineShape>();
            _shapes = new ObservableCollection<ShapeObject>();
            _styles = new ObservableCollection<DrawStyle>();
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

        public bool Invalidate(ShapeRenderer r, double dx, double dy)
        {
            bool result = false;

            var points = GetPoints();

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

            foreach (var point in points)
            {
                point.IsDirty = false;
            }

            return result;
        }

        public void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            if (Guides != null)
            {
                foreach (var shape in Guides)
                {
                    shape.Draw(dc, r, dx, dy);
                }
            }

            foreach (var shape in Shapes)
            {
                shape.Draw(dc, r, dx, dy);
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
                        copy.Shapes.Add((ShapeObject)copyable.Copy(shared));
                    }
                }
            }

            return copy;
        }
    }
}
