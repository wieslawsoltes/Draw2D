using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public class PointShape : BaseShape, IPointShape
    {
        public static HAlign[] HAlignValues { get; } = (HAlign[])Enum.GetValues(typeof(HAlign));
        public static VAlign[] VAlignValues { get; } = (VAlign[])Enum.GetValues(typeof(VAlign));

        internal static new IBounds s_bounds = new PointBounds();
        internal static new IShapeDecorator s_decorator = new PointDecorator();

        private double _x;
        private double _y;
        private HAlign _hAlign;
        private VAlign _vAlign;
        private IBaseShape _template;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

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
        public HAlign HAlign
        {
            get => _hAlign;
            set => Update(ref _hAlign, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public VAlign VAlign
        {
            get => _vAlign;
            set => Update(ref _vAlign, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IBaseShape Template
        {
            get => _template;
            set => Update(ref _template, value);
        }

        public PointShape()
        {
        }

        public PointShape(double x, double y, IBaseShape template)
        {
            this.X = x;
            this.Y = y;
            this.Template = template;
        }

        public PointShape(double x, double y, HAlign halign, VAlign valign, IBaseShape template)
        {
            this.X = x;
            this.Y = y;
            this.HAlign = halign;
            this.VAlign = valign;
            this.Template = template;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(this);
        }

        public override bool IsTreeDirty()
        {
            if (base.IsTreeDirty())
            {
                return true;
            }

            if (_template?.IsTreeDirty() ?? false)
            {
                return true;
            }

            return false;
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
            return new PointShape()
            {
                StyleId = this.StyleId,
                Effects = (IPaintEffects)this.Effects?.Copy(shared),
                Owner = this.Owner,
                X = this.X,
                Y = this.Y,
                Template = this.Template
            };
        }
    }
}
