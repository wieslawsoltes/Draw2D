// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Containers
{
    public interface IContainerView : IDrawTarget, IHitTestable, ICopyable
    {
        string Title { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        ArgbColor PrintBackground { get; set; }
        ArgbColor WorkBackground { get; set; }
        ArgbColor InputBackground { get; set; }
        ICanvasContainer CurrentContainer { get; set; }
        ICanvasContainer WorkingContainer { get; set; }
        ISelectionState SelectionState { get; set; }
        IZoomServiceState ZoomServiceState { get; set; }
        IDrawContainerView DrawContainerView { get; set; }
        void Add(IBaseShape shape);
        void Remove(IBaseShape shape);
        void Reference(GroupShape group);
        void Style(string styleId);
    }
}
