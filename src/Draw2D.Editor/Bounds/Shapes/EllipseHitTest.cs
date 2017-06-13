// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.Core.Shapes;

namespace Draw2D.Editor.Bounds.Shapes
{
    public class EllipseHitTest : BoxHitTest
    {
        public override Type TargetType => typeof(EllipseShape);
    }
}
