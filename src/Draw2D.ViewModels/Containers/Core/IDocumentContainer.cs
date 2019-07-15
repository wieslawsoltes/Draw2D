// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Draw2D.ViewModels.Containers
{
    public interface IDocumentContainer : ICopyable, IDisposable
    {
        string Title { get; set; }
        IStyleLibrary StyleLibrary { get; set; }
        IGroupLibrary GroupLibrary { get; set; }
        IBaseShape PointTemplate { get; set; }
        IList<IContainerView> ContainerViews { get; set; }
        IContainerView ContainerView { get; set; }
    }
}
