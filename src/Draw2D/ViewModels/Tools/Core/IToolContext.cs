// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Input;
using Draw2D.ViewModels.Containers;

namespace Draw2D.ViewModels.Tools
{
    public interface IToolContext : IInputTarget, IDisposable
    {
        IStyleLibrary StyleLibrary { get; set; }
        IGroupLibrary GroupLibrary { get; set; }
        IBaseShape PointTemplate { get; set; }
        IHitTest HitTest { get; set; }
        IPathConverter PathConverter { get; set; }
        IList<IContainerView> ContainerViews { get; set; }
        IContainerView ContainerView { get; set; }
        IList<ITool> Tools { get; set; }
        ITool CurrentTool { get; set; }
        EditMode EditMode { get; set; }
        void SetTool(string name);
    }
}
