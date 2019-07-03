// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Draw2D.ViewModels.Tools
{
    public interface IPointFilter : INode, IDirty
    {
        IList<IBaseShape> Guides { get; set; }
        bool Process(IToolContext context, ref double x, ref double y);
        void Clear(IToolContext context);
    }
}
