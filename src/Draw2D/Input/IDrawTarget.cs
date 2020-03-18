// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.Input
{
    public interface IDrawTarget
    {
        IInputService InputService { get; set; }
        IZoomService ZoomService { get; set; }
        void Draw(object context, double width, double height, double dx, double dy, double zx, double zy);
    }
}
