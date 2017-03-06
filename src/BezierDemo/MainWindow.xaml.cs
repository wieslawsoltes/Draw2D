using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BezierDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            p0.DragDelta += (sender, e) => 
            {
                MoveThumb(p0, e.HorizontalChange, e.VerticalChange);
                MoveBezier();
                MoveIntersections();
            };
            
            p1.DragDelta += (sender, e) => 
            {
                MoveThumb(p1, e.HorizontalChange, e.VerticalChange);
                MoveBezier();
                MoveIntersections();
            };
            
            p2.DragDelta += (sender, e) => 
            {
                MoveThumb(p2, e.HorizontalChange, e.VerticalChange);
                MoveBezier();
                MoveIntersections();
            };
            
            p3.DragDelta += (sender, e) => 
            {
                MoveThumb(p3, e.HorizontalChange, e.VerticalChange);
                MoveBezier();
                MoveIntersections();
            };
            
            l0.DragDelta += (sender, e) => 
            {
                MoveThumb(l0, e.HorizontalChange, e.VerticalChange);
                MoveLine();
                MoveIntersections();
            };
            
            l1.DragDelta += (sender, e) => 
            {
                MoveThumb(l1, e.HorizontalChange, e.VerticalChange);
                MoveLine();
                MoveIntersections();
            };
        }
        
        public void MoveThumb(Thumb thumb, double dx, double dy)
        {
            double x = Canvas.GetLeft(thumb);
            double y = Canvas.GetTop(thumb);
            Canvas.SetLeft(thumb, x + dx);
            Canvas.SetTop(thumb, y + dy);
        }
        
        public void MoveLine()
        {
            double lx0 = Canvas.GetLeft(l0);
            double ly0 = Canvas.GetTop(l0);
            double lx1 = Canvas.GetLeft(l1);
            double ly1 = Canvas.GetTop(l1);
            line.X1 = lx0;
            line.Y1 = ly0;
            line.X2 = lx1;
            line.Y2 = ly1;
        }
        
        public void MoveBezier()
        {
            double px0 = Canvas.GetLeft(p0);
            double py0 = Canvas.GetTop(p0);
            double px1 = Canvas.GetLeft(p1);
            double py1 = Canvas.GetTop(p1);
            double px2 = Canvas.GetLeft(p2);
            double py2 = Canvas.GetTop(p2);
            double px3 = Canvas.GetLeft(p3);
            double py3 = Canvas.GetTop(p3);
            figure.StartPoint = new Point(px0, py0);
            bezier.Point1 = new Point(px1, py1);
            bezier.Point2 = new Point(px2, py2);
            bezier.Point3 = new Point(px3, py3);
        }
        
        public void MoveIntersections()
        {
            double lx0 = Canvas.GetLeft(l0);
            double ly0 = Canvas.GetTop(l0);
            double lx1 = Canvas.GetLeft(l1);
            double ly1 = Canvas.GetTop(l1);
            double px0 = Canvas.GetLeft(p0);
            double py0 = Canvas.GetTop(p0);
            double px1 = Canvas.GetLeft(p1);
            double py1 = Canvas.GetTop(p1);
            double px2 = Canvas.GetLeft(p2);
            double py2 = Canvas.GetTop(p2);
            double px3 = Canvas.GetLeft(p3);
            double py3 = Canvas.GetTop(p3);
            
            double[] px = { px0, px1, px2, px3 };
            double[] py = { py0, py1, py2, py3 };
            double[] lx = { lx0, lx1 };
            double[] ly = { ly0, ly1 };
            
            double [] i = CubicBezier.ComputeIntersections(px, py, lx, ly);
            
            Canvas.SetLeft(i0, i[0]);
            Canvas.SetTop(i0, i[1]);
            Canvas.SetLeft(i1, i[2]);
            Canvas.SetTop(i1, i[3]);
            Canvas.SetLeft(i2, i[4]);
            Canvas.SetTop(i2, i[5]);
        }
    }
}
