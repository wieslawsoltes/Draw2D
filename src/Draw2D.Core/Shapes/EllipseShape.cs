using System.Collections.Generic;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class EllipseShape : ConnectableShape
    {
        private PointShape _topLeft;
        private PointShape _bottomRight;

        public PointShape TopLeft
        {
            get => _topLeft;
            set => Update(ref _topLeft, value);
        }

        public PointShape BottomRight
        {
            get => _bottomRight;
            set => Update(ref _bottomRight, value);
        }

        public EllipseShape()
            : base()
        {
        }

        public EllipseShape(PointShape topLeft, PointShape bottomRight)
            : base()
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            if (Style != null)
            {
                r.DrawEllipse(dc, this, Style, dx, dy); 
            }

            if (r.Selected.Contains(_topLeft))
            {
                _topLeft.Draw(dc, r, dx, dy); 
            }

            if (r.Selected.Contains(_bottomRight))
            {
                _bottomRight.Draw(dc, r, dx, dy); 
            }

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            if (!selected.Contains(_topLeft))
            {
                _topLeft.Move(selected, dx, dy);
            }

            if (!selected.Contains(_bottomRight))
            {
                _bottomRight.Move(selected, dx, dy);
            }

            base.Move(selected, dx, dy);
        }

        public override void Select(ISet<ShapeObject> selected)
        {
            base.Select(selected);
            TopLeft.Select(selected);
            BottomRight.Select(selected);
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);
            TopLeft.Deselect(selected);
            BottomRight.Deselect(selected);
        }
    }
}
