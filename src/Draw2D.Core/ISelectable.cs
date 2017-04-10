// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Draw2D.Core
{
    public interface ISelectable
    {
        void Move(ISet<ShapeObject> selected, double dx, double dy);
        void Select(ISet<ShapeObject> selected);
        void Deselect(ISet<ShapeObject> selected);
    }
}
