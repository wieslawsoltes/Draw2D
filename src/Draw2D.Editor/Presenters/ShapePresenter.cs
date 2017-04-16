// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Editor.Presenters
{
    public abstract class ShapePresenter
    {
        public IDictionary<Type, ShapeHelper> Helpers { get; set; }
        public abstract void DrawContent(object dc, IToolContext context, double dx, double dy);
        public abstract void DrawWorking(object dc, IToolContext context, double dx, double dy);
        public abstract void DrawHelpers(object dc, IToolContext context, double dx, double dy);
    }
}
