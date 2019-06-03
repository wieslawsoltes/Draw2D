// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Decorators;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public class EllipseShape : BoxShape
    {
        internal static new IBounds s_bounds = new EllipseBounds();
        internal static new IShapeDecorator s_decorator = new EllipseDecorator();

        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public EllipseShape()
            : base()
        {
        }

        public EllipseShape(IPointShape topLeft, IPointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public override void Invalidate()
        {
            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (StyleId != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawEllipse(dc, this, StyleId, dx, dy, scale);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState?.IsSelected(TopLeft) ?? false)
                {
                    TopLeft.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(BottomRight) ?? false)
                {
                    BottomRight.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, scale, mode, db, r);
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new EllipseShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StyleId = this.StyleId,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (IPointShape)shared[this.TopLeft];
                copy.BottomRight = (IPointShape)shared[this.BottomRight];

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
