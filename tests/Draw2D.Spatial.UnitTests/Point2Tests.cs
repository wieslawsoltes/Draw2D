using System;
using NUnit.Framework;

namespace Draw2D.Spatial.UnitTests
{
    [TestFixture]
    public class Point2Tests
    {
        [TestCase]
        public void Construtor_Sets_All_Fields()
        {
            var target = new Point2(10, 20);
            Assert.AreEqual(10.0, target.X);
            Assert.AreEqual(20.0, target.Y);
        }

        [TestCase]
        public void FromXY_Returns_Valid_Point()
        {
            var target = Point2.FromXY(30, 50);
            Assert.NotNull(target);
            Assert.AreEqual(30.0, target.X);
            Assert.AreEqual(50.0, target.Y);
        }

        [TestCase]
        public void Operator_Plus_Adds_Field_Values()
        {
            var point1 = new Point2(5, 10);
            var point2 = new Point2(20, 7);
            var target = point1 + point2;
            Assert.AreEqual(25.0, target.X);
            Assert.AreEqual(17.0, target.Y);
        }

        [TestCase]
        public void Operator_Minus_Subtracts_Field_Values()
        {
            var point1 = new Point2(21, 9);
            var point2 = new Point2(8, 15);
            var target = point1 - point2;
            Assert.AreEqual(13.0, target.X);
            Assert.AreEqual(-6.0, target.Y);
        }

        [TestCase]
        public void DistanceTo_Calculates_Horizontal_Distance_Between_Points()
        {
            var point1 = new Point2(10, 10);
            var point2 = new Point2(30, 10);
            var target = point1.DistanceTo(point2);
            Assert.AreEqual(20.0, target);
        }

        [TestCase]
        public void DistanceTo_Calculates_Vertical_Distance_Between_Points()
        {
            var point1 = new Point2(10, 10);
            var point2 = new Point2(10, 40);
            var target = point1.DistanceTo(point2);
            Assert.AreEqual(30.0, target);
        }

        [TestCase]
        public void DistanceTo_Calculates_Diagonal_Distance_Between_Points()
        {
            var point1 = new Point2(10, 10);
            var point2 = new Point2(35, 20);
            var target = point1.DistanceTo(point2);
            Assert.AreEqual(26.92582403567252, target);
        }

        [TestCase]
        public void AngleBetween_Calculates_Angle_In_Degrees()
        {
            var origin = new Point2(30, 30);
            Assert.AreEqual(0.0, origin.AngleBetween(new Point2(30, 30)));
            Assert.AreEqual(0.0, origin.AngleBetween(new Point2(60, 30)));
            Assert.AreEqual(45.0, origin.AngleBetween(new Point2(60, 60)));
            Assert.AreEqual(90.0, origin.AngleBetween(new Point2(30, 60)));
            Assert.AreEqual(135.0, origin.AngleBetween(new Point2(0, 60)));
            Assert.AreEqual(180.0, origin.AngleBetween(new Point2(0, 30)));
            Assert.AreEqual(225.0, origin.AngleBetween(new Point2(0, 0)));
            Assert.AreEqual(270.0, origin.AngleBetween(new Point2(30, 0)));
            Assert.AreEqual(315.0, origin.AngleBetween(new Point2(60, 0)));
        }

        [TestCase]
        public void RotateAt_Rotates_Point_Around_Center_Point_By_Specified_Angle_In_Degrees()
        {
            var center = new Point2(0, 0);
            var point = new Point2(0, 30);

            var delta = 8.0E-15;

            var target0 = point.RotateAt(center, 0);
            Assert.AreEqual(0.0, target0.X, delta);
            Assert.AreEqual(30.0, target0.Y, delta);

            var target90 = point.RotateAt(center, 90);
            Assert.AreEqual(-30.0, target90.X, delta);
            Assert.AreEqual(0.0, target90.Y, delta);

            var target180 = point.RotateAt(center, 180);
            Assert.AreEqual(0.0, target180.X, delta);
            Assert.AreEqual(-30.0, target180.Y);

            var target270 = point.RotateAt(center, 270);
            Assert.AreEqual(30.0, target270.X, delta);
            Assert.AreEqual(0.0, target270.Y, delta);

            var target360 = point.RotateAt(center, 360);
            Assert.AreEqual(0.0, target360.X, delta);
            Assert.AreEqual(30.0, target360.Y, delta);
        }

        [TestCase]
        public void ProjectOnLine_Projects_Point_On_Vertical_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(0, 50);
            var target = new Point2(25, 25).ProjectOnLine(a, b);
            Assert.AreEqual(0.0, target.X);
            Assert.AreEqual(25.0, target.Y);
        }

        [TestCase]
        public void ProjectOnLine_Projects_Point_On_Horizontal_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(50, 0);
            var target = new Point2(25, 25).ProjectOnLine(a, b);
            Assert.AreEqual(25.0, target.X);
            Assert.AreEqual(0.0, target.Y);
        }

        [TestCase]
        public void ProjectOnLine_Projects_Point_On_Diagonal_Line()
        {
            var a = new Point2(0, 50);
            var b = new Point2(50, 0);
            var target = new Point2(0, 0).ProjectOnLine(a, b);
            Assert.AreEqual(25.0, target.X);
            Assert.AreEqual(25.0, target.Y);
        }

        [TestCase]
        public void NearestOnLine_Finds_Nearest_Point_On_Vertical_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(0, 50);
            var target = new Point2(25, 25).NearestOnLine(a, b);
            Assert.AreEqual(0.0, target.X);
            Assert.AreEqual(25.0, target.Y);
        }

        [TestCase]
        public void NearestOnLine_Finds_Nearest_Point_On_Horizontal_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(50, 0);
            var target = new Point2(25, 25).NearestOnLine(a, b);
            Assert.AreEqual(25.0, target.X);
            Assert.AreEqual(0.0, target.Y);
        }

        [TestCase]
        public void NearestOnLine_Finds_Nearest_Point_On_Diagonal_Line()
        {
            var a = new Point2(0, 50);
            var b = new Point2(50, 0);
            var target = new Point2(0, 0).NearestOnLine(a, b);
            Assert.AreEqual(25.0, target.X);
            Assert.AreEqual(25.0, target.Y);
        }

        [TestCase]
        public void IsOnLine_Returns_True_If_Point_Is_On_Vertical_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(0, 50);
            var target = new Point2(0, 25).IsOnLine(a, b);
            Assert.True(target);
        }

        [TestCase]
        public void IsOnLine_Returns_True_If_Point_Is_On_Horizontal_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(50, 0);
            var target = new Point2(25, 0).IsOnLine(a, b);
            Assert.True(target);
        }

        [TestCase]
        public void IsOnLine_Returns_True_If_Point_Is_On_Diagonal_Line()
        {
            var a = new Point2(0, 50);
            var b = new Point2(50, 0);
            var target = new Point2(25, 25).IsOnLine(a, b);
            Assert.True(target);
        }

        [TestCase]
        public void IsOnLine_Returns_False_If_Point_Is_Not_On_Vertical_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(0, 50);
            var target = new Point2(0.1, 25).IsOnLine(a, b);
            Assert.False(target);
        }

        [TestCase]
        public void IsOnLine_Returns_False_If_Point_Is_Not_On_Horizontal_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(50, 0);
            var target = new Point2(25, 0.1).IsOnLine(a, b);
            Assert.False(target);
        }

        [TestCase]
        public void IsOnLine_Returns_False_If_Point_X_Is_Not_On_Diagonal_Line()
        {
            var a = new Point2(0, 50);
            var b = new Point2(50, 0);
            var target = new Point2(50.1, 50.0).IsOnLine(a, b);
            Assert.False(target);
        }

        [TestCase]
        public void IsOnLine_Returns_False_If_Point_Y_Is_Not_On_Diagonal_Line()
        {
            var a = new Point2(0, 50);
            var b = new Point2(50, 0);
            var target = new Point2(50.0, 50.1).IsOnLine(a, b);
            Assert.False(target);
        }

        [TestCase]
        public void ExpandToRect_Expands_Point_To_Rectangle_By_Specified_Radius()
        {
            var point = new Point2(0, 0);
            var target = point.ExpandToRect(10);
            Assert.AreEqual(-10.0, target.Left);
            Assert.AreEqual(10.0, target.Right);
            Assert.AreEqual(-10.0, target.Top);
            Assert.AreEqual(10.0, target.Bottom);
        }
    }
}
