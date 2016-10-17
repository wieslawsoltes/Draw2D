using System;
using System.Windows;

namespace PathDemo
{
    public abstract class ToolBase
    {
        public string Name { get; set; }

        public virtual void LeftDown(IToolContext context, Point point)
        {
        }

        public virtual void LeftUp(IToolContext context, Point point)
        {
        }

        public virtual void RightDown(IToolContext context, Point point)
        {
        }

        public virtual void RightUp(IToolContext context, Point point)
        {
        }

        public virtual void Move(IToolContext context, Point point)
        {
        }
    }
}
