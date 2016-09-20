using System;
using NUnit.Framework;

namespace Draw2D.Spatial.UnitTests
{
    [TestFixture]
    public class Rect2Tests
    {
        [TestCase]
        public void Construtor_Sets_All_Fields()
        {
            var target = new Rect2(15, 20, 40, 35);
            Assert.AreEqual(15.0, target.X);
            Assert.AreEqual(20.0, target.Y);
            Assert.AreEqual(40.0, target.Width);
            Assert.AreEqual(35.0, target.Height);
        }

        [TestCase]
        public void FromPoints_Returns_Valid_Rectangle()
        {
            var target1 = Rect2.FromPoints(0, 0, 10, 10);
            Assert.AreEqual(0.0, target1.X);
            Assert.AreEqual(0.0, target1.Y);
            Assert.AreEqual(10.0, target1.Width);
            Assert.AreEqual(10.0, target1.Height);

            var target2 = Rect2.FromPoints(20, 20, 5, 5);
            Assert.AreEqual(5.0, target2.X);
            Assert.AreEqual(5.0, target2.Y);
            Assert.AreEqual(15.0, target2.Width);
            Assert.AreEqual(15.0, target2.Height);

            var target3 = Rect2.FromPoints(20, 20, 5, 5, 3, 2);
            Assert.AreEqual(8.0, target3.X);
            Assert.AreEqual(7.0, target3.Y);
            Assert.AreEqual(15.0, target3.Width);
            Assert.AreEqual(15.0, target3.Height);
        }

        [TestCase]
        public void FromPoints_Point_Returns_Valid_Rectangle()
        {
            var target1 = Rect2.FromPoints(new Point2(0, 0), new Point2(10, 10));
            Assert.AreEqual(0.0, target1.X);
            Assert.AreEqual(0.0, target1.Y);
            Assert.AreEqual(10.0, target1.Width);
            Assert.AreEqual(10.0, target1.Height);

            var target2 = Rect2.FromPoints(new Point2(20, 20), new Point2(5, 5));
            Assert.AreEqual(5.0, target2.X);
            Assert.AreEqual(5.0, target2.Y);
            Assert.AreEqual(15.0, target2.Width);
            Assert.AreEqual(15.0, target2.Height);

            var target3 = Rect2.FromPoints(new Point2(20, 20), new Point2(5, 5), 3, 2);
            Assert.AreEqual(8.0, target3.X);
            Assert.AreEqual(7.0, target3.Y);
            Assert.AreEqual(15.0, target3.Width);
            Assert.AreEqual(15.0, target3.Height);
        }

        [TestCase]
        public void Top_Property_Returns_Valid_Value()
        {
            var target = new Rect2(15, 20, 40, 35);
            Assert.AreEqual(20.0, target.Top);
        }

        [TestCase]
        public void Left_Property_Returns_Valid_Value()
        {
            var target = new Rect2(15, 20, 40, 35);
            Assert.AreEqual(15.0, target.Left);
        }

        [TestCase]
        public void Bottom_Property_Returns_Valid_Value()
        {
            var target = new Rect2(15, 20, 40, 35);
            Assert.AreEqual(20.0 + 35.0, target.Bottom);
        }

        [TestCase]
        public void Right_Property_Returns_Valid_Value()
        {
            var target = new Rect2(15, 20, 40, 35);
            Assert.AreEqual(15.0 + 40.0, target.Right);
        }

        [TestCase]
        public void Center_Property_Returns_Valid_Value()
        {
            var rect = new Rect2(15, 20, 40, 35);
            var target = rect.Center;
            Assert.AreEqual(15.0 + 40.0 / 2, target.X);
            Assert.AreEqual(20.0 + 35.0 / 2, target.Y);
        }

        [TestCase]
        public void TopLeft_Property_Returns_Valid_Value()
        {
            var rect = new Rect2(15, 20, 40, 35);
            var target = rect.TopLeft;
            Assert.AreEqual(15.0, target.X);
            Assert.AreEqual(20.0, target.Y);
        }

        [TestCase]
        public void BottomRight_Property_Returns_Valid_Value()
        {
            var rect = new Rect2(15, 20, 40, 35);
            var target = rect.BottomRight;
            Assert.AreEqual(15.0 + 40.0, target.X);
            Assert.AreEqual(20.0 + 35.0, target.Y);
        }

        [TestCase]
        public void Contains_Point_Returns_True_If_Point_Is_Inside_Rectangle()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.True(target.Contains(new Point2(15, 15)));
            Assert.True(target.Contains(new Point2(11, 10)));
            Assert.True(target.Contains(new Point2(29, 30)));
        }

        [TestCase]
        public void Contains_Point_Returns_False_If_Point_Is_Outside_Rectangle()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.False(target.Contains(new Point2(9, 15)));
            Assert.False(target.Contains(new Point2(15, 31)));
            Assert.False(target.Contains(new Point2(15, 9)));
            Assert.False(target.Contains(new Point2(15, 31)));
        }

        [TestCase]
        public void Contains_Returns_True_If_Point_Is_Inside_Rectangle()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.True(target.Contains(15, 15));
            Assert.True(target.Contains(11, 10));
            Assert.True(target.Contains(29, 30));
        }

        [TestCase]
        public void Contains_Returns_False_If_Point_Is_Outside_Rectangle()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.False(target.Contains(9, 15));
            Assert.False(target.Contains(15, 31));
            Assert.False(target.Contains(15, 9));
            Assert.False(target.Contains(15, 31));
        }

        [TestCase]
        public void IntersectsWith_Returns_True_If_Rectangles_Intersect()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.True(target.IntersectsWith(new Rect2(12, 12, 16, 16)));
            Assert.True(target.IntersectsWith(new Rect2(5, 10, 20, 20)));
            Assert.True(target.IntersectsWith(new Rect2(20, 10, 20, 20)));
            Assert.True(target.IntersectsWith(new Rect2(10, 5, 20, 20)));
            Assert.True(target.IntersectsWith(new Rect2(10, 20, 20, 20)));
        }

        [TestCase]
        public void IntersectsWith_Returns_False_If_Rectangles_Do_Not_Intersect()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.False(target.IntersectsWith(new Rect2(0, 9, 9, 9)));
            Assert.False(target.IntersectsWith(new Rect2(31, 9, 9, 9)));
            Assert.False(target.IntersectsWith(new Rect2(10, 0, 9, 9)));
            Assert.False(target.IntersectsWith(new Rect2(10, 31, 9, 9)));
        }
    }
}
