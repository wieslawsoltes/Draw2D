using System;
using System.Collections.Generic;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace Draw2D.Models.Renderers
{
    public abstract class ShapeRenderer : ObservableObject
    {
        public abstract ISet<ShapeObject> Selected { get; set; }
        public abstract void PushMatrix(object dc, MatrixObject matrix);
        public abstract void PopMatrix(object dc);
        public abstract void DrawLine(object dc, LineShape line, DrawStyle style, double dx, double dy);
        public abstract void DrawPolyLine(object dc, PointShape start, IList<PointShape> points, DrawStyle style, double dx, double dy);
        public abstract void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, DrawStyle style, double dx, double dy);
        public abstract void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, DrawStyle style, double dx, double dy);
        public abstract void DrawPath(object dc, PathShape path, DrawStyle style, double dx, double dy);
        public abstract void DrawRectangle(object dc, RectangleShape rectangle, DrawStyle style, double dx, double dy);
        public abstract void DrawEllipse(object dc, EllipseShape ellipse, DrawStyle style, double dx, double dy);
    }
}
