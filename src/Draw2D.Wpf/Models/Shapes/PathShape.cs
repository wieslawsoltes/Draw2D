using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Models.Renderers;

namespace Draw2D.Models.Shapes
{
    public class PathShape : ConnectableShape
    {
        private ObservableCollection<FigureShape> _figures;
        private PathFillRule _fillRule;

        public ObservableCollection<FigureShape> Figures
        {
            get { return _figures; }
            set
            {
                if (value != _figures)
                {
                    _figures = value;
                    Notify("Figures");
                }
            }
        }

        public PathFillRule FillRule
        {
            get { return _fillRule; }
            set
            {
                if (value != _fillRule)
                {
                    _fillRule = value;
                    Notify("FillRule");
                }
            }
        }

        public PathShape()
            : base()
        {
            _figures = new ObservableCollection<FigureShape>();
        }

        public PathShape(ObservableCollection<FigureShape> figures)
            : base()
        {
            this.Figures = figures;
        }

        public PathShape(string name)
            : this()
        {
            this.Name = name;
        }

        public PathShape(string name, ObservableCollection<FigureShape> figures)
            : base()
        {
            this.Name = name;
            this.Figures = figures;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            r.DrawPath(dc, this, Style, dx, dy);

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<BaseShape> selected, double dx, double dy)
        {
            foreach (var figure in Figures)
            {
                if (!selected.Contains(figure))
                {
                    figure.Move(selected, dx, dy);
                }
            }

            base.Move(selected, dx, dy);
        }
    }
}
