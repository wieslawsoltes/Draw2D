using System;
using System.Collections.Generic;
using Draw2D.Models;

namespace Draw2D.Editor
{
    public abstract class PointFilter
    {
        public List<BaseShape> Guides { get; set; }

        protected PointFilter()
        {
            Guides = new List<BaseShape>();
        }

        public abstract bool Process(IToolContext context, ref double x, ref double y);

        public virtual void Clear(IToolContext context)
        {
            foreach (var guide in Guides)
            {
                context.WorkingContainer.Shapes.Remove(guide);
                context.Renderer.Selected.Remove(guide);
            }
            Guides.Clear();
        }
    }
}
