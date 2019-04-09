// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels
{
    public interface ISelection
    {
        BaseShape Hover { get; set; }
        ISet<BaseShape> Selected { get; set; }
    }
}
