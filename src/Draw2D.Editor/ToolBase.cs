using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Draw2D.Editor
{
    public abstract class ToolBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public abstract string Name { get; }

        public List<PointIntersection> Intersections { get; set; }

        public List<PointFilter> Filters { get; set; }

        public virtual void LeftDown(IToolContext context, double x, double y)
        {
            Debug.WriteLine(string.Format("[{0}] LeftDown X={1} Y={2}", Name, x, y));
        }

        public virtual void LeftUp(IToolContext context, double x, double y)
        {
            Debug.WriteLine(string.Format("[{0}] LeftUp X={1} Y={2}", Name, x, y));
        }

        public virtual void RightDown(IToolContext context, double x, double y)
        {
            Debug.WriteLine(string.Format("[{0}] RightDown X={1} Y={2}", Name, x, y));
        }

        public virtual void RightUp(IToolContext context, double x, double y)
        {
            Debug.WriteLine(string.Format("[{0}] RightUp X={1} Y={2}", Name, x, y));
        }

        public virtual void Move(IToolContext context, double x, double y)
        {
            Debug.WriteLine(string.Format("[{0}] Move X={1} Y={2}", Name, x, y));
        }

        public virtual void Clean(IToolContext context)
        {
            Debug.WriteLine(string.Format("[{0}] Clean", Name));
        }
    }
}
