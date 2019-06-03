// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Draw2D.ViewModels.Containers
{
    public interface IDrawContainerView : IDisposable
    {
        void Draw(object context, double width, double height, double dx, double dy, double zx, double zy);
    }
}
