// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core.Containers;
using Draw2D.Core.Renderer;

namespace Draw2D.Core.Presenters
{
    public abstract class ShapePresenter
    {
        public IDictionary<Type, ShapeHelper> Helpers { get; set; }
        public abstract void DrawContainer(object dc, IShapeContainer container, ShapeRenderer r, double dx, double dy);
        public abstract void DrawHelpers(object dc, IShapeContainer container, ShapeRenderer r, double dx, double dy);
    }
}
