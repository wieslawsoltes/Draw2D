// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Containers;

namespace Draw2D.Editor
{
    public interface IFactory
    {
        IToolContext CreateToolContext();
        IContainerView CreateContainerView(string title);
    }
}
