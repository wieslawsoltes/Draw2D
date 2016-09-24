using System;
using System.Collections.Generic;
using Draw2D.Models;
using Draw2D.Models.Shapes;

namespace Draw2D.Editor
{
    public abstract class PointIntersection
    {
        public abstract string Name { get; }
        public List<PointShape> Intersections { get; set; }

        protected PointIntersection()
        {
            Intersections = new List<PointShape>();
        }

        public abstract void Find(IToolContext context, BaseShape shape);

        public virtual void Clear(IToolContext context)
        {
            foreach (var point in Intersections)
            {
                context.WorkingContainer.Shapes.Remove(point);
                context.Renderer.Selected.Remove(point);
            }
            Intersections.Clear();
        }
    }
}
