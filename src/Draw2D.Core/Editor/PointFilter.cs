using System;
using System.Collections.Generic;
using Draw2D.Models;

namespace Draw2D.Editor
{
    public abstract class PointFilter
    {
        public abstract string Name { get; }
        public List<ShapeObject> Guides { get; set; }

        protected PointFilter()
        {
            Guides = new List<ShapeObject>();
        }

        public abstract bool Process(IToolContext context, ref double x, ref double y);

        public virtual void Clear(IToolContext context)
        {
            foreach (var guide in Guides)
            {
                context.WorkingContainer.Shapes.Remove(guide);
                context.Selected.Remove(guide);
            }
            Guides.Clear();
        }
    }
}
