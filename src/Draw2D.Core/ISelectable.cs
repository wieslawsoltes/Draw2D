// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Shape;

namespace Draw2D.Core
{
    public interface ISelectable
    {
        void Move(ISet<BaseShape> selected, double dx, double dy);
        void Select(ISet<BaseShape> selected);
        void Deselect(ISet<BaseShape> selected);
    }
}
