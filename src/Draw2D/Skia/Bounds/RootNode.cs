using System;
using System.Collections.Generic;
using Draw2D.ViewModels;
using SkiaSharp;

namespace Draw2D.Bounds
{
    public readonly struct RootNode : IDisposable
    {
        public readonly IBaseShape Shape;
        public readonly IList<ChildNode> Children;

        public RootNode(IBaseShape shape)
        {
            this.Shape = shape;
            this.Children = new List<ChildNode>();
        }

        public bool Contains(float x, float y, ContainsMode mode, out IBaseShape rootShape, out IBaseShape childShape)
        {
            rootShape = null;
            childShape = null;
            for (int i = 0; i < Children.Count; i++)
            {
                if (mode == ContainsMode.Geometry)
                {
                    if (Children[i].Geometry.Contains(x, y))
                    {
                        rootShape = this.Shape;
                        childShape = Children[i].Shape;
                        return true;
                    }
                }
                else if (mode == ContainsMode.Bounds)
                {
                    if (Children[i].Bounds.Contains(x, y))
                    {
                        rootShape = this.Shape;
                        childShape = Children[i].Shape;
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool Intersects(SKPath geometry, out IBaseShape rootShape, out IBaseShape childShape)
        {
            rootShape = null;
            childShape = null;
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Intersects(geometry))
                {
                    rootShape = this.Shape;
                    childShape = Children[i].Shape;
                    return true;
                }
            }
            return false;
        }

        public bool Intersects(ref SKRect rect, out IBaseShape rootShape, out IBaseShape childShape)
        {
            rootShape = null;
            childShape = null;
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Bounds.IntersectsWith(rect))
                {
                    rootShape = this.Shape;
                    childShape = Children[i].Shape;
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            for (int i = 0; i < Children?.Count; i++)
            {
                Children[i].Dispose();
            }
        }
    }
}
