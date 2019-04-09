// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels
{
    public interface IEdit
    {
        void Cut(IToolContext context);
        void Copy(IToolContext context);
        void Paste(IToolContext context);
        void Delete(IToolContext context);
        void Group(IToolContext context);
        void SelectAll(IToolContext context);
        void Hover(IToolContext context, BaseShape shape);
        void DeHover(IToolContext context);
        void Connect(IToolContext context, PointShape point);
        void Disconnect(IToolContext context, PointShape point);
        void Disconnect(IToolContext context, BaseShape shape);
    }
}
