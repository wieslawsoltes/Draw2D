// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Draw2D.ViewModels.Tools
{
    public interface IPointIntersection : INode, IDirty
    {
        string Title { get; }
        IList<IPointShape> Intersections { get; set; }
        void Find(IToolContext context, IBaseShape shape);
        void Clear(IToolContext context);
    }
}
