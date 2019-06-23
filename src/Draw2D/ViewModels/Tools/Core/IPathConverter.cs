// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    public interface IPathConverter
    {
        PathShape ToPathShape(IToolContext context, IBaseShape shape);
        PathShape Op(IToolContext context, PathOp op, ICollection<IBaseShape> selected);
        PathShape ToPathShape(IToolContext context, string svgPathData);
        string ToSvgPathData(IToolContext context, ICollection<IBaseShape> selected);
    }
}
