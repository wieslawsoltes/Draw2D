using System;
using System.Collections.Generic;
using Draw2D.Models.Style;

namespace Draw2D.Models.Renderers
{
    public abstract class ShapeRenderer : BaseObject
    {
        public abstract HashSet<BaseShape> Selected { get; set; }
        public abstract void PushMatrix(object dc, MatrixObject matrix);
        public abstract void PopMatrix(object dc);
        public abstract void DrawLine(object dc, double x0, double y0, double x1, double y1, DrawStyle style, double dx, double dy);
        public abstract void DrawRectangle(object dc, double tlx, double tly, double brx, double bry, DrawStyle style, double dx, double dy);
        public abstract void DrawEllipse(object dc, double tlx, double tly, double brx, double bry, DrawStyle style, double dx, double dy);
    }
}
