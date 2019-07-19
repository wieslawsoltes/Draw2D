// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels
{
    public interface IDirty
    {
        bool IsDirty { get; set; }
        void MarkAsDirty(bool isDirty);
        bool IsTreeDirty();
        void Invalidate();
    }
}
