// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Draw2D.ViewModels
{
    public interface IBaseShape : INode, ICopyable, IDirty, ISelectable, IDrawable
    {
        void GetPoints(IList<IPointShape> points);
        bool IsPointsTreeDirty();
    }
}
