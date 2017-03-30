using System.Collections.Generic;
using System.Diagnostics;
using Draw2D.Models;

namespace Draw2D.Editor
{
    public abstract class ToolBase : ObservableObject
    {
        public abstract string Name { get; }

        public List<PointIntersection> Intersections { get; set; }

        public List<PointFilter> Filters { get; set; }

        public virtual void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            Debug.WriteLine(string.Format("[{0}] LeftDown X={1} Y={2}, Modifier {3}", Name, x, y, modifier));
        }

        public virtual void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            Debug.WriteLine(string.Format("[{0}] LeftUp X={1} Y={2}, Modifier {3}", Name, x, y, modifier));
        }

        public virtual void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            Debug.WriteLine(string.Format("[{0}] RightDown X={1} Y={2}, Modifier {3}", Name, x, y, modifier));
        }

        public virtual void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
            Debug.WriteLine(string.Format("[{0}] RightUp X={1} Y={2}, Modifier {3}", Name, x, y, modifier));
        }

        public virtual void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            Debug.WriteLine(string.Format("[{0}] Move X={1} Y={2}, Modifier {3}", Name, x, y, modifier));
        }

        public virtual void Clean(IToolContext context)
        {
            Debug.WriteLine(string.Format("[{0}] Clean", Name));
        }
    }
}
