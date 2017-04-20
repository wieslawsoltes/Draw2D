// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Core.Renderers
{
    public abstract class ShapeRenderer : ObservableObject
    {
        public abstract ISet<ShapeObject> Selected { get; set; }
        public abstract void InvalidateCache(DrawStyle style);
        public abstract void InvalidateCache(MatrixObject matrix);
        public abstract void InvalidateCache(ShapeObject shape, DrawStyle style, double dx, double dy);
        public abstract object PushMatrix(object dc, MatrixObject matrix);
        public abstract void PopMatrix(object dc, object state);
        public abstract void DrawLine(object dc, IPoint start, IPoint point, DrawStyle style, double dx, double dy);
        public abstract void DrawCubicBezier(object dc, IPoint start, IPoint point1, IPoint point2, IPoint poin3, DrawStyle style, double dx, double dy);
        public abstract void DrawQuadraticBezier(object dc, IPoint start, IPoint point1, IPoint point2, DrawStyle style, double dx, double dy);
        public abstract void DrawPath(object dc, IPath path, DrawStyle style, double dx, double dy);
        public abstract void DrawRectangle(object dc, IPoint tl, IPoint br, DrawStyle style, double dx, double dy);
        public abstract void DrawEllipse(object dc, IPoint tl, IPoint br, DrawStyle style, double dx, double dy);
        public abstract void DrawText(object dc, IPoint tl, IPoint br, string text, DrawStyle style, double dx, double dy);
    }
}
