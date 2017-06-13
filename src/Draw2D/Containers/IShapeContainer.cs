// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Renderer;
using Draw2D.Shape;
using Draw2D.Shapes;

namespace Draw2D.Containers
{
    public interface IShapeContainer
    {
        double Width { get; set; }
        double Height { get; set; }
        ObservableCollection<LineShape> Guides { get; set; }
        ObservableCollection<BaseShape> Shapes { get; set; }
        IEnumerable<PointShape> GetPoints();
        bool Invalidate(ShapeRenderer renderer, double dx, double dy);
        void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r);
    }
}
