// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Editor
{
    public interface IEditorToolContext : IToolContext
    {
        IContainerFactory ContainerFactory { get; set; }
        ISelection Selection { get; set; }
        string CurrentDirectory { get; set; }
        IList<string> Files { get; set; }
    }
}
