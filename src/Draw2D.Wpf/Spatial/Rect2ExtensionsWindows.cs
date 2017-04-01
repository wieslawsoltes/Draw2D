using System.Windows;
using Draw2D.Spatial;

namespace Draw2D.Core.Shapes
{
    public static class Rect2ExtensionsWindows
    {
        public static Rect AsRect(this Rect2 rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
