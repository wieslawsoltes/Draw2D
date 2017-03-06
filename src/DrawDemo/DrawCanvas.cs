using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DrawDemo
{
    public class DrawCanvas : Canvas
    {
        bool _draw = false;
        Point _point;

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            //Debug.WriteLine("Enter");
            _point = e.GetPosition(this);
            _draw = true;
            //Cursor = Cursors.None;
            this.InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            //Debug.WriteLine("Leave");
            _draw = false;
            //Cursor = Cursors.Arrow;
            this.InvalidateVisual();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            //Debug.WriteLine("Move");
            _point = e.GetPosition(this);
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_draw)
            {
                //Debug.WriteLine("Draw");
                var pen = new Pen(Brushes.Cyan, 2);

                var a0 = new Point(0, _point.Y);
                var b0 = new Point(this.ActualWidth, _point.Y);
                var a1 = new Point(_point.X, 0);
                var b1 = new Point(_point.X, this.ActualHeight);

                dc.DrawLine(pen, a0, b0);
                dc.DrawLine(pen, a1, b1);

                //dc.DrawEllipse(Brushes.Yellow, null, _point, 5, 5);

            }
        }
    }
}
