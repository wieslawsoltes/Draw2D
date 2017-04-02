// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Core.Containers
{
    public interface IShapesContainer
    {
        double Width { get; set; }
        double Height { get; set; }
        ObservableCollection<DrawStyle> Styles { get; set; }
        ObservableCollection<LineShape> Guides { get; set; }
        ObservableCollection<ShapeObject> Shapes { get; set; }
    }
}
