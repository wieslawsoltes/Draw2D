// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Tools
{
    public interface IContainerFactory
    {
        IStyleLibrary CreateStyleLibrary();
        IGroupLibrary CreateGroupLibrary();
        IToolContext CreateToolContext();
        IContainerView CreateContainerView(string title);
        IDocumentContainer CreateDocumentContainer(string title);
    }
}
