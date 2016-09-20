using System;
using System.Diagnostics;
using Draw2D.Models.Shapes;

namespace Draw2D.Editor.Filters
{
    public class GridSnapPointFilter : PointFilter
    {
        public GridSnapSettings Settings { get; set; }

        public override bool Process(IToolContext context, ref double x, ref double y)
        {
            if (Settings.Mode != GridSnapMode.None)
            {
                bool haveSnapToGrid = false;

                if (Settings.Mode.HasFlag(GridSnapMode.Horizontal))
                {
                    x = SnapGrid(x, Settings.GridSizeX);
                    haveSnapToGrid = true;
                }

                if (Settings.Mode.HasFlag(GridSnapMode.Vertical))
                {
                    y = SnapGrid(y, Settings.GridSizeY);
                    haveSnapToGrid = true;
                }

                if (Settings.EnableGuides && haveSnapToGrid)
                {
                    PointGuides(context, x, y);
                }

                Debug.WriteLineIf(haveSnapToGrid, string.Format("Grid Snap {0}", Settings.Mode));
                return haveSnapToGrid;
            }
            Clear(context);
            return false;
        }

        private void PointGuides(IToolContext context, double x, double y)
        {
            var horizontal = new LineShape(
                new PointShape(0, y, null),
                new PointShape(context.Container.Width, y, null));
            horizontal.Style = Settings.GuideStyle;

            var vertical = new LineShape(
                new PointShape(x, 0, null),
                new PointShape(x, context.Container.Height, null));
            vertical.Style = Settings.GuideStyle;

            Guides.Add(horizontal);
            context.WorkingContainer.Shapes.Add(horizontal);
            context.Renderer.Selected.Add(horizontal);

            Guides.Add(vertical);
            context.WorkingContainer.Shapes.Add(vertical);
            context.Renderer.Selected.Add(vertical);
        }

        public static double SnapGrid(double value, double size)
        {
            double r = value % size;
            return r >= size / 2.0 ? value + size - r : value - r;
        }
    }
}
