// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core.Renderers;
using Draw2D.Core.Editor;

namespace Draw2D.Core.Presenters
{
    public abstract class ShapePresenter
    {
        public IDictionary<Type, ShapeHelper> Helpers { get; set; }
        public abstract void Draw(object dc, IToolContext context);
        public abstract void DrawHelpers(object dc, IToolContext context);
    }
}
