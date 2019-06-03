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
    public class ReferenceShape : BaseShape
    {
        internal static new IBounds s_bounds = new ReferenceBounds();
        internal static new IShapeDecorator s_decorator = new ReferenceDecorator();

        private string _title;
        private double _x;
        private double _y;
        private IBaseShape _template;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Title
        {
            get => _title;
            set => Update(ref _title, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double X
        {
            get => _x;
            set => Update(ref _x, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Y
        {
            get => _y;
            set => Update(ref _y, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IBaseShape Template
        {
            get => _template;
            set => Update(ref _template, value);
        }

        public ReferenceShape()
        {
        }

        public ReferenceShape(string title, double x, double y, IBaseShape template)
        {
            this.Title = title;
            this.X = x;
            this.Y = y;
            this.Template = template;
            this.Points = new ObservableCollection<IPointShape>();

            if (template is IConnectable connectable)
            {
                foreach (var point in connectable.Points)
                {
                    Points.Add(new ReferencePointShape(point, this));
                }
            }
        }

        public override void Invalidate()
        {
            if (_template != null)
            {
                _template.Invalidate();
            }

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r)
        {
            if (_template != null)
            {
                double offsetX = X;
                double offsetY = Y;
                _template.Draw(dc, renderer, dx + offsetX, dy + offsetY, scale, db, r);
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new ReferenceShape()
            {
                StyleId = this.StyleId,
                Owner = this.Owner,
                Title = this.Title,
                X = this.X,
                Y = this.Y,
                Template = this.Template,
                Points = new ObservableCollection<IPointShape>()
            };

            if (shared != null)
            {
                foreach (var point in this.Points)
                {
                    if (point is ReferencePointShape referencePoint)
                    {
                        var referencePointCopy = (ReferencePointShape)shared[referencePoint];
                        referencePointCopy.Owner = copy;
                        referencePointCopy.Reference = copy;
                        copy.Points.Add(referencePointCopy);
                    }
                    else
                    {
                        copy.Points.Add((IPointShape)shared[point]);
                    }
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }
}
