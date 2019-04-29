// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.Input
{
    public interface IInputTarget
    {
        IInputService InputService { get; set; }
        void LeftDown(double x, double y, Modifier modifier);
        void LeftUp(double x, double y, Modifier modifier);
        void RightDown(double x, double y, Modifier modifier);
        void RightUp(double x, double y, Modifier modifier);
        void Move(double x, double y, Modifier modifier);
        double GetWidth();
        double GetHeight();
    }
}
