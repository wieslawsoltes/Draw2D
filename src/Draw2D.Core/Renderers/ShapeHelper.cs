// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Draw2D.Core.Renderers
{
    public abstract class ShapeHelper
    {
        public abstract void Draw(object dc, ShapeRenderer r, ShapeObject shape, ISet<ShapeObject> selected, double dx, double dy);
    }
}
