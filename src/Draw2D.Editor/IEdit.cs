// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core;
using Draw2D.Core.Shapes;

namespace Draw2D.Editor
{
    public interface IEdit
    {
        void Cut(IToolContext context);
        void Copy(IToolContext context);
        void Paste(IToolContext context);
        void Delete(IToolContext context);
        void Group(IToolContext context);
        void SelectAll(IToolContext context);
        void Hover(IToolContext context, ShapeObject shape);
        void DeHover(IToolContext context);
        void Connect(IToolContext context, PointShape point);
        void Disconnect(IToolContext context, PointShape point);
        void Disconnect(IToolContext context, ShapeObject shape);
    }
}
