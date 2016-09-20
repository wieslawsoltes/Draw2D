using System;
using System.Collections.Generic;
using Draw2D.Models.Renderers;

namespace Draw2D.Models.Shapes
{
    public class LineShape : ConnectableShape
    {
        private PointShape _start;
        private PointShape _end;

        public PointShape Start
        {
            get { return _start; }
            set
            {
                if (value != _start)
                {
                    _start = value;
                    Notify("Start");
                }
            }
        }

        public PointShape End
        {
            get { return _end; }
            set
            {
                if (value != _end)
                {
                    _end = value;
                    Notify("End");
                }
            }
        }

        public LineShape()
            : base()
        {
        }

        public LineShape(PointShape start, PointShape end)
            : base()
        {
            this.Start = start;
            this.End = end;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            r.DrawLine(dc, _start.X, _start.Y, _end.X, _end.Y, Style, dx, dy);

            _start.Draw(dc, r, dx, dy); 
            _end.Draw(dc, r, dx, dy);

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<BaseShape> selected, double dx, double dy)
        {
            if (!selected.Contains(_start))
            {
                _start.Move(selected, dx, dy);
            }

            if (!selected.Contains(_end))
            {
                _end.Move(selected, dx, dy);
            }

            base.Move(selected, dx, dy);
        }

        public override void Select(ISet<BaseShape> selected)
        {
            base.Select(selected);
            Start.Select(selected);
            End.Select(selected);
        }

        public override void Deselect(ISet<BaseShape> selected)
        {
            base.Deselect(selected);
            Start.Deselect(selected);
            End.Deselect(selected);
        }
    }
}
