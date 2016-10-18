
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace BezierDemo
{
    public partial class Window1 : Window
    {
        public Window1()
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
    
    
    
    public static class CubicBezier
    {
        public static double[] BezierCoeffs(double P0, double P1, double P2, double P3)
        {
            var Z = new double[4];
            Z[0] = -P0 + 3*P1 + -3*P2 + P3; 
            Z[1] = 3*P0 - 6*P1 + 3*P2;
            Z[2] = -3*P0 + 3*P1;
            Z[3] = P0;
            return Z;
        }
        
        // sign of number
        public static int Sign(double x)
        {
            if (x < 0.0) return -1;
            return 1;
        }
        
        public static void SortSpecial(double[] a)
        {
            bool flip;
            double temp;
            
            do
            {
                flip=false;
                for (var i=0;i<a.Length-1;i++)
                {
                    if ((a[i+1]>=0 && a[i]>a[i+1]) || (a[i]<0 && a[i+1]>=0))
                    {
                        flip=true;
                        temp=a[i];
                        a[i]=a[i+1];
                        a[i+1]=temp;
                    }
                }
            } while (flip);
        }
        
        // based on http://mysite.verizon.net/res148h4j/javascript/script_exact_cubic.html#the%20source%20code
        public static double[] CubicRoots(double[] P)
        {
            double a=P[0];
            double b=P[1];
            double c=P[2];
            double d=P[3];
            
            double A=b/a;
            double B=c/a;
            double C=d/a;
        

            double Im;

            double Q = (3*B - Math.Pow(A, 2))/9;
            double R = (9*A*B - 27*C - 2*Math.Pow(A, 3))/54;
            double D = Math.Pow(Q, 3) + Math.Pow(R, 2);    // polynomial discriminant
        
            var t = new double[3];
            
            if (D >= 0)                                 // complex or duplicate roots
            {
                double S = Sign(R + Math.Sqrt(D)) * Math.Pow(Math.Abs(R + Math.Sqrt(D)),(1/3));
                double T = Sign(R - Math.Sqrt(D)) * Math.Pow(Math.Abs(R - Math.Sqrt(D)),(1/3));
        
                t[0] = -A/3 + (S + T);                    // real root
                t[1] = -A/3 - (S + T)/2;                  // real part of complex root
                t[2] = -A/3 - (S + T)/2;                  // real part of complex root
                Im = Math.Abs(Math.Sqrt(3)*(S - T)/2);    // complex part of root pair   
                
                //discard complex roots
                if (Im!=0)
                {
                    t[1]=-1;
                    t[2]=-1;
                }
           
            }
            else                                          // distinct real roots
            {
                double th = Math.Acos(R/Math.Sqrt(-Math.Pow(Q, 3)));
                t[0] = 2*Math.Sqrt(-Q)*Math.Cos(th/3) - A/3;
                t[1] = 2*Math.Sqrt(-Q)*Math.Cos((th + 2*Math.PI)/3) - A/3;
                t[2] = 2*Math.Sqrt(-Q)*Math.Cos((th + 4*Math.PI)/3) - A/3;
                Im = 0.0;
            }
            
            //discard out of spec roots
            for (var i=0;i<3;i++) 
                if (t[i]<0 || t[i]>1.0) t[i]=-1;
                        
            //sort but place -1 at the end
            SortSpecial(t);
            
            Debug.WriteLine(t[0]+" "+t[1]+" "+t[2]);
            return t;
        }
        
        // computes intersection between a cubic spline and a line segment
        public static double[] ComputeIntersections(double[] px, double[] py, double[] lx, double[] ly)
        {
            var A=ly[1]-ly[0];        //A=y2-y1
            var B=lx[0]-lx[1];        //B=x1-x2
            var C=lx[0]*(ly[0]-ly[1]) + 
                  ly[0]*(lx[1]-lx[0]);    //C=x1*(y1-y2)+y1*(x2-x1)
        
            var bx = BezierCoeffs(px[0],px[1],px[2],px[3]);
            var by = BezierCoeffs(py[0],py[1],py[2],py[3]);
            
            var P = new double[4];
            P[0] = A*bx[0]+B*by[0];        //t^3
            P[1] = A*bx[1]+B*by[1];        //t^2
            P[2] = A*bx[2]+B*by[2];        //t
            P[3] = A*bx[3]+B*by[3] + C;    //1
            
            var r = CubicRoots(P);
            
            var X = new double[2];
            var I = new double[6];
            
            //verify the roots are in bounds of the linear segment
            for (var i=0;i<3;i++)
            {
                var t=r[i];
                
                X[0]=bx[0]*t*t*t+bx[1]*t*t+bx[2]*t+bx[3];
                X[1]=by[0]*t*t*t+by[1]*t*t+by[2]*t+by[3];            
                    
                // above is intersection point assuming infinitely long line segment,
                // make sure we are also in bounds of the line
                double s;
                if ((lx[1]-lx[0])!=0)           // if not vertical line
                    s=(X[0]-lx[0])/(lx[1]-lx[0]);
                else
                    s=(X[1]-ly[0])/(ly[1]-ly[0]);
                
                // in bounds?   
                //*
                if (t<0 || t>1.0 || s<0 || s>1.0)
                {
                    X[0]=-100;  // move off screen
                    X[1]=-100;
                }
                //*/
               
                // move intersection point
                I[i*2] = X[0];
                I[i*2+1] = X[1];
            }
            return I;
        }
    }
}