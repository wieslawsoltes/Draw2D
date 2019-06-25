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
        void CreateGroup(IToolContext context);
        void CreateReference(IToolContext context);
        void CreatePath(IToolContext context);
        void AlignLeft(IToolContext context);
        void AlignCentered(IToolContext context);
        void AlignRight(IToolContext context);
        void AlignTop(IToolContext context);
        void AlignCenter(IToolContext context);
        void AlignBottom(IToolContext context);
        void ArangeBringToFront(IToolContext context);
        void ArangeBringForward(IToolContext context);
        void ArangeSendBackward(IToolContext context);
        void ArangeSendToBack(IToolContext context);
        void Break(IToolContext context);
        void Connect(IToolContext context);
        void Disconnect(IToolContext context);
        void SelectAll(IToolContext context);
        void DeselectAll(IToolContext context);
    }
}
