// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels
{
    public interface IDrawable
    {
        IBounds Bounds { get; }
        IShapeDecorator Decorator { get; }
        string StyleId { get; set; }
        IPaintEffects Effects { get; set; }
        void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r);
    }
}
