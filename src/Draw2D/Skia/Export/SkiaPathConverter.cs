using System.Collections.Generic;
using System.Text;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Export;

public class SkiaPathConverter : IPathConverter
{
    private bool IsAcceptedShape(IBaseShape shape)
    {
        return !(shape is IPointShape || shape is FigureShape);
    }

    private IList<IBaseShape> GetShapes(ICollection<IBaseShape> selected)
    {
        if (selected == null || selected.Count <= 0)
        {
            return null;
        }

        var shapes = new List<IBaseShape>();

        foreach (var shape in selected)
        {
            if (IsAcceptedShape(shape))
            {
                shapes.Add(shape);
            }
        }

        return shapes;
    }

    private IList<SKPath> ToPaths(IToolContext context, IList<IBaseShape> shapes)
    {
        if (shapes == null || shapes.Count <= 0)
        {
            return null;
        }

        var paths = new List<SKPath>();

        for (int i = 0; i < shapes.Count; i++)
        {
            var fillType = SKPathFillType.Winding;
            if (shapes[i] is PathShape pathShape)
            {
                fillType = SkiaUtil.ToSKPathFillType(pathShape.FillType);
            }
            var path = new SKPath() { FillType = fillType };
            var result = SkiaUtil.AddShape(context, shapes[i], 0.0, 0.0, path);
            if (result == true && path.IsEmpty == false)
            {
                paths.Add(path);
            }
            else
            {
                path.Dispose();
            }
        }

        return paths;
    }

    private SKPath ToPath(IToolContext context, IBaseShape shape)
    {
        var fillType = SKPathFillType.Winding;
        if (shape is PathShape pathShape)
        {
            fillType = SkiaUtil.ToSKPathFillType(pathShape.FillType);
        }

        var geometry = new SKPath() { FillType = fillType };

        if (SkiaUtil.AddShape(context, shape, 0.0, 0.0, geometry) == true)
        {
            return geometry;
        }
        else
        {
            geometry.Dispose();
        }

        return null;
    }

    public PathShape ToPathShape(IToolContext context, IBaseShape shape)
    {
        using (var geometry = ToPath(context, shape))
        {
            if (geometry != null)
            {
                var style = context.DocumentContainer?.StyleLibrary?.Get(shape.StyleId);
                if (style == null)
                {
                    style = context.DocumentContainer?.StyleLibrary?.CurrentItem;
                }
                return SkiaUtil.ToPathShape(context, geometry, style, context?.DocumentContainer?.PointTemplate);
            }
        }
        return null;
    }

    public PathShape ToStrokePathShape(IToolContext context, IBaseShape shape)
    {
        using (var geometry = ToPath(context, shape))
        {
            if (geometry != null)
            {
                var style = context.DocumentContainer?.StyleLibrary?.Get(shape.StyleId);
                if (style == null)
                {
                    style = context.DocumentContainer?.StyleLibrary?.CurrentItem;
                }

                using var disposable = new CompositeDisposable();
                var path = SkiaUtil.ToStrokePath(context, style.StrokePaint, shape.Effects, geometry, disposable.Disposables);
                if (path != null)
                {
                    disposable.Disposables.Add(path);
                    var union = SkiaUtil.Op(SKPathOp.Union, new[] { path, path });
                    if (union != null && !union.IsEmpty)
                    {
                        disposable.Disposables.Add(union);
                        return SkiaUtil.ToPathShape(context, union, context.DocumentContainer?.StyleLibrary?.CurrentItem, context?.DocumentContainer?.PointTemplate);
                    }
                }
            }
        }
        return null;
    }

    public PathShape ToFillPathShape(IToolContext context, IBaseShape shape)
    {
        using (var geometry = ToPath(context, shape))
        {
            if (geometry != null)
            {
                var style = context.DocumentContainer?.StyleLibrary?.Get(shape.StyleId);
                if (style == null)
                {
                    style = context.DocumentContainer?.StyleLibrary?.CurrentItem;
                }

                using var disposable = new CompositeDisposable();
                var path = SkiaUtil.ToFillPath(context, style.FillPaint, shape.Effects, geometry, disposable.Disposables);
                if (path != null)
                {
                    disposable.Disposables.Add(path);
                    var union = SkiaUtil.Op(SKPathOp.Union, new[] { path, path });
                    if (union != null && !union.IsEmpty)
                    {
                        disposable.Disposables.Add(union);
                        return SkiaUtil.ToPathShape(context, union, context.DocumentContainer?.StyleLibrary?.CurrentItem, context?.DocumentContainer?.PointTemplate);
                    }

                }
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
            var paths = ToPaths(context, shapes);
            if (paths != null && paths.Count > 0)
            {
                var result = SkiaUtil.Op(SkiaUtil.ToSKPathOp(op), paths);
                if (result != null)
                {
                    if (!result.IsEmpty)
                    {
                        var style = context.DocumentContainer?.StyleLibrary?.Get(shapes[0].StyleId);
                        if (style == null)
                        {
                            style = context.DocumentContainer?.StyleLibrary?.CurrentItem;
                        }
                        path = SkiaUtil.ToPathShape(context, result, style, context?.DocumentContainer?.PointTemplate);
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
            using var path = SkiaUtil.ToPath(svgPathData);
            return SkiaUtil.ToPathShape(context, path, context.DocumentContainer?.StyleLibrary?.CurrentItem, context?.DocumentContainer?.PointTemplate);
        }
        return null;
    }

    public string ToSvgPathData(IToolContext context, ICollection<IBaseShape> selected)
    {
        var sb = new StringBuilder();
        var shapes = GetShapes(selected);
        if (shapes != null && shapes.Count > 0)
        {
            foreach (var shape in shapes)
            {
                SkiaUtil.ToSvgPathData(context, shape, sb);
            }
        }
        return sb.ToString();
    }

    public void Dispose()
    {
    }
}