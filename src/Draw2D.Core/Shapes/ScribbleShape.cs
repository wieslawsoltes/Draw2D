// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class ScribbleShape : ConnectableShape, ICopyable<ScribbleShape>
    {
        public ScribbleShape()
            : base()
        {
        }

        public ScribbleShape(ObservableCollection<PointShape> points)
            : base()
        {
            this.Points = points;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            foreach (var point in Points)
            {
                yield return point;
            }
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            if (Points.Count >= 2 && Style != null)
            {
                r.DrawPolyLine(dc, Points, Style, dx, dy);
            }

            foreach (var point in Points)
            {
                if (r.Selected.Contains(point))
                {
                    point.Draw(dc, r, dx, dy);
                }
            }

            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            foreach (var point in Points)
            {
                if (!selected.Contains(point))
                {
                    point.Move(selected, dx, dy);
                }
            }
        }

        public ScribbleShape Copy()
        {
            return new ScribbleShape()
            {
                Style = this.Style,
                Transform = this.Transform?.Copy()
            };
        }
    }
}
