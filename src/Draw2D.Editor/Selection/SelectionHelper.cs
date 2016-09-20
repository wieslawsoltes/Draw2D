using System;
using System.Diagnostics;
using Draw2D.Spatial;

namespace Draw2D.Editor.Selection
{
    public static class SelectionHelper
    {
        public static bool TryToHover(IToolContext context, SelectionMode mode, SelectionTargets targets, Point2 target, double radius)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest.TryToGetPoint(context.Container.Shapes, target, radius) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest.TryToGetShape(context.Container.Shapes, target, radius) : null;

            var guidePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest.TryToGetPoint(context.Container.Guides, target, radius) : null;

            var guide =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest.TryToGetShape(context.Container.Guides, target, radius) : null;

            if (shapePoint != null || shape != null || guide != null)
            {
                if (shapePoint != null)
                {
                    Debug.WriteLine("Hover Shape Point: {0}", shapePoint.GetType());
                    context.Renderer.Selected.Clear();
                    shapePoint.Select(context.Renderer.Selected);
                    return true;
                }
                else if (shape != null)
                {
                    Debug.WriteLine("Hover Shape: {0}", shape.GetType());
                    context.Renderer.Selected.Clear();
                    shape.Select(context.Renderer.Selected);
                    return true;
                }
                else if (guidePoint != null)
                {
                    Debug.WriteLine("Hover Guide Point: {0}", guidePoint.GetType());
                    context.Renderer.Selected.Clear();
                    guidePoint.Select(context.Renderer.Selected);
                    return true;
                }
                else if (guide != null)
                {
                    Debug.WriteLine("Hover Guide: {0}", guide.GetType());
                    context.Renderer.Selected.Clear();
                    guide.Select(context.Renderer.Selected);
                    return true;
                }
            }

            Debug.WriteLine("No Hover");
            context.Renderer.Selected.Clear();
            return false;
        }

        public static bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Point2 point, double radius)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest.TryToGetPoint(context.Container.Shapes, point, radius) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest.TryToGetShape(context.Container.Shapes, point, radius) : null;

            var guidePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest.TryToGetPoint(context.Container.Guides, point, radius) : null;

            var guide =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest.TryToGetShape(context.Container.Guides, point, radius) : null;

            if (shapePoint != null || shape != null || guidePoint != null || guide != null)
            {
                bool haveNewSelection =
                    (shapePoint != null && !context.Renderer.Selected.Contains(shapePoint))
                    || (shape != null && !context.Renderer.Selected.Contains(shape))
                    || (guidePoint != null && !context.Renderer.Selected.Contains(guidePoint))
                    || (guide != null && !context.Renderer.Selected.Contains(guide));

                if (context.Renderer.Selected.Count >= 1 && !haveNewSelection)
                {
                    return true;
                }
                else
                {
                    if (shapePoint != null)
                    {
                        context.Renderer.Selected.Clear();
                        Debug.WriteLine("Selected Shape Point: {0}", shapePoint.GetType());
                        shapePoint.Select(context.Renderer.Selected);
                        return true;
                    }
                    else if (shape != null)
                    {
                        context.Renderer.Selected.Clear();
                        Debug.WriteLine("Selected Shape: {0}", shape.GetType());
                        shape.Select(context.Renderer.Selected);
                        return true;
                    }
                    else if (guidePoint != null)
                    {
                        context.Renderer.Selected.Clear();
                        Debug.WriteLine("Selected Guide Point: {0}", guidePoint.GetType());
                        guidePoint.Select(context.Renderer.Selected);
                        return true;
                    }
                    else if (guide != null)
                    {
                        context.Renderer.Selected.Clear();
                        Debug.WriteLine("Selected Guide: {0}", guide.GetType());
                        guide.Select(context.Renderer.Selected);
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Rect2 rect, double radius)
        {
            var shapes =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest.TryToGetShapes(context.Container.Shapes, rect, radius) : null;

            var guides =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest.TryToGetShapes(context.Container.Guides, rect, radius) : null;

            if (shapes != null || guides != null)
            {
                if (shapes != null)
                {
                    Debug.WriteLine("Selected Shapes: {0}", shapes != null ? shapes.Count : 0);
                    context.Renderer.Selected.Clear();
                    foreach (var shape in shapes)
                    {
                        shape.Select(context.Renderer.Selected);
                    }
                    return true;
                }
                else if (guides != null)
                {
                    Debug.WriteLine("Selected Guides: {0}", guides != null ? guides.Count : 0);
                    context.Renderer.Selected.Clear();
                    foreach (var guide in guides)
                    {
                        guide.Select(context.Renderer.Selected);
                    }
                    return true;
                }
            }

            return false;
        }
    }
}
