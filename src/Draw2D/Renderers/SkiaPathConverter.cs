﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Text;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Renderers
{
    public class SkiaPathConverter : IPathConverter
    {
        private IList<IBaseShape> GetShapes(ICollection<IBaseShape> selected)
        {
            if (selected == null || selected.Count <= 0)
            {
                return null;
            }

            var shapes = new List<IBaseShape>();

            foreach (var shape in selected)
            {
                if (!(shape is IPointShape))
                {
                    shapes.Add(shape);
                }
            }

            return shapes;
        }

        public PathShape ToPathShape(IToolContext context, IBaseShape shape)
        {
            using (var geometry = new SKPath())
            {
                if (SkiaHelper.ToGeometry(context, shape, 0.0, 0.0, geometry) == true)
                {
                    return SkiaHelper.ToPathShape(context, geometry, context.StyleLibrary?.CurrentStyle, context.PointTemplate);
                }
            }
            return null;
        }

        public PathShape Op(IToolContext context, PathOp op, ICollection<IBaseShape> selected)
        {
            var path = default(PathShape);
            var shapes = GetShapes(selected);
            if (shapes != null && shapes.Count > 0)
            {
                var paths = SkiaHelper.ToGeometries(context, shapes);
                if (paths != null && paths.Count > 0)
                {
                    var result = SkiaHelper.Op(SkiaHelper.ToPathOp(op), paths);
                    if (result != null)
                    {
                        if (!result.IsEmpty)
                        {
                            path = SkiaHelper.ToPathShape(context, result, context.StyleLibrary?.CurrentStyle, context.PointTemplate);
                        }
                        result.Dispose();
                    }

                    for (int i = 0; i < paths.Count; i++)
                    {
                        paths[i].Dispose();
                    }
                }
            }
            return path;
        }

        public PathShape ToPathShape(IToolContext context, string svgPathData)
        {
            if (!string.IsNullOrWhiteSpace(svgPathData))
            {
                using (var path = SkiaHelper.ToGeometry(svgPathData))
                {
                    return SkiaHelper.ToPathShape(context, path, context.StyleLibrary?.CurrentStyle, context.PointTemplate);
                }
            }
            return null;
        }

        public string ToSvgPathData(IToolContext context, ICollection<IBaseShape> selected)
        {
            var sb = new StringBuilder();

            foreach (var shape in selected)
            {
                SkiaHelper.ToSvgPathData(context, shape, sb);
            }

            return sb.ToString();
        }
    }
}
