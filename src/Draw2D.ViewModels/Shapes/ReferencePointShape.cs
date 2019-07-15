// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Decorators;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public class ReferencePointShape : BaseShape, IPointShape
    {
        internal static new IBounds s_bounds = new PointBounds();
        internal static new IShapeDecorator s_decorator = new PointDecorator();

        private IPointShape _point;
        private ReferenceShape _reference;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [IgnoreDataMember]
        public double X
        {
            get
            {
                if (_point != null && _reference != null)
                {
                    return _point.X + _reference.X;
                }
                return double.NaN;
            }
            set
            {
                if (_point != null && _reference != null)
                {
                    _point.X = value;
                }
            }
        }

        [IgnoreDataMember]
        public double Y
        {
            get
            {
                if (_point != null && _reference != null)
                {
                    return _point.Y + _reference.Y;
                }
                return double.NaN;
            }
            set
            {
                if (_point != null && _reference != null)
                {
                    _point.Y = value;
                }
            }
        }

        [IgnoreDataMember]
        public HAlign HAlign
        {
            get
            {
                if (_point != null && _reference != null)
                {
                    return _point.HAlign;
                }
                return HAlign.Auto;
            }
            set
            {
                if (_point != null && _reference != null)
                {
                    _point.HAlign = value;
                }
            }
        }

        [IgnoreDataMember]
        public VAlign VAlign
        {
            get
            {
                if (_point != null && _reference != null)
                {
                    return _point.VAlign;
                }
                return VAlign.Auto;
            }
            set
            {
                if (_point != null && _reference != null)
                {
                    _point.VAlign = value;
                }
            }
        }

        [IgnoreDataMember]
        public IBaseShape Template
        {
            get
            {
                if (_point != null && _reference != null)
                {
                    return _point.Template;
                }
                return null;
            }
            set
            {
                if (_point != null && _reference != null)
                {
                    _point.Template = value;
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ReferenceShape Reference
        {
            get => _reference;
            set => Update(ref _reference, value);
        }

        public ReferencePointShape()
        {
        }

        public ReferencePointShape(IPointShape point, ReferenceShape reference)
        {
            this.Point = point;
            this.Reference = reference;
            this.Owner = reference;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(this);
        }

        public override void Invalidate()
        {
            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r)
        {
            if (_point != null && _reference != null)
            {
                double offsetX = _reference.X;
                double offsetY = _reference.Y;
                _point.Draw(dc, renderer, dx + offsetX, dy + offsetY, scale, db, r);
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            return new ReferencePointShape()
            {
                StyleId = this.StyleId,
                Owner = this.Owner,
                Point = this.Point,
                Reference = this.Reference,
            };
        }
    }
}
