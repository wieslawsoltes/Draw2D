// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels
{
    public interface IShapeRenderer : IDisposable
    {
        ISelectionState SelectionState { get; }
        void DrawLine(object dc, LineShape line, string styleId, double dx, double dy, double scale);
        void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, string styleId, double dx, double dy, double scale);
        void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, string styleId, double dx, double dy, double scale);
        void DrawConic(object dc, ConicShape conic, string styleId, double dx, double dy, double scale);
        void DrawPath(object dc, PathShape path, string styleId, double dx, double dy, double scale);
        void DrawRectangle(object dc, RectangleShape rectangle, string styleId, double dx, double dy, double scale);
        void DrawEllipse(object dc, EllipseShape ellipse, string styleId, double dx, double dy, double scale);
        void DrawText(object dc, TextShape text, string styleId, double dx, double dy, double scale);
    }
}
