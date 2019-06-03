// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Draw2D.ViewModels.Containers
{
    public interface ICanvasContainer : IBaseShape
    {
        IList<IBaseShape> Shapes { get; set; }
    }
}
