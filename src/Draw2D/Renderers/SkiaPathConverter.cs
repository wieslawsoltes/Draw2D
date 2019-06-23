// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Renderers
{
    public class SkiaPathConverter : IPathConverter
    {
        public PathShape ToPathShape(IToolContext context, IBaseShape shape)
        {
            return SkiaHelper.ToPathShape(context, shape);
        }

        public PathShape Op(IToolContext context, PathOp op, ICollection<IBaseShape> selected)
        {
            return SkiaHelper.Op(context, op, selected);
        }

        public PathShape ToPathShape(IToolContext context, string svgPathData)
        {
            return SkiaHelper.ToPathShape(context, svgPathData);
        }

        public string ToSvgPathData(IToolContext context, ICollection<IBaseShape> selected)
        {
            return SkiaHelper.ToSvgPathData(context, selected);
        }
    }
}
