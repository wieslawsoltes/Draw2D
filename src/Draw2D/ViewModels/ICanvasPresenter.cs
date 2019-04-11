// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.ViewModels.Containers;

namespace Draw2D.ViewModels
{
    public interface ICanvasPresenter
    {
        IDictionary<Type, IShapeDecorator> Decorators { get; set; }
        void DrawContainer(object dc, CanvasContainer container, IShapeRenderer renderer, double dx, double dy, object db, object r);
        void DrawDecorators(object dc, CanvasContainer container, IShapeRenderer renderer, double dx, double dy);
    }
}
