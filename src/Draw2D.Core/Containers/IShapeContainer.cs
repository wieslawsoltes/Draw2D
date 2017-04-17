// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Containers
{
    public interface IShapeContainer
    {
        double Width { get; set; }
        double Height { get; set; }
        ObservableCollection<LineShape> Guides { get; set; }
        ObservableCollection<ShapeObject> Shapes { get; set; }
        IEnumerable<PointShape> GetPoints();
        bool Invalidate(ShapeRenderer r, double dx, double dy);
        void Draw(object dc, ShapeRenderer r, double dx, double dy);
    }
}
