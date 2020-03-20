using System;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels
{
    public interface IShapeRenderer : IDisposable
    {
        void DrawLine(object dc, LineShape line, string styleId, double dx, double dy, double scale);
        void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, string styleId, double dx, double dy, double scale);
        void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, string styleId, double dx, double dy, double scale);
        void DrawConic(object dc, ConicShape conic, string styleId, double dx, double dy, double scale);
        void DrawPath(object dc, PathShape path, string styleId, double dx, double dy, double scale);
        void DrawRectangle(object dc, RectangleShape rectangle, string styleId, double dx, double dy, double scale);
        void DrawCircle(object dc, CircleShape circle, string styleId, double dx, double dy, double scale);
        void DrawArc(object dc, ArcShape arc, string styleId, double dx, double dy, double scale);
        void DrawOval(object dc, OvalShape oval, string styleId, double dx, double dy, double scale);
        void DrawText(object dc, TextShape text, string styleId, double dx, double dy, double scale);
        void DrawImage(object dc, ImageShape image, string styleId, double dx, double dy, double scale);
    }
}
