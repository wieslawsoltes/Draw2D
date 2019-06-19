// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels.Tools
{
    public interface ISelection : IDirty
    {
        void Cut(IToolContext context);
        void Copy(IToolContext context);
        void Paste(IToolContext context);
        void Delete(IToolContext context);
        void Group(IToolContext context);
        void Ungroup(IToolContext context);
        void Break(IToolContext context);
        void Reference(IToolContext context);
        void SelectAll(IToolContext context);
        void DeselectAll(IToolContext context);
        void Connect(IToolContext context, IPointShape point);
        void Disconnect(IToolContext context, IPointShape point);
        void Disconnect(IToolContext context, IBaseShape shape);
    }
}
