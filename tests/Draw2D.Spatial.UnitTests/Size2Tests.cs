using System;
using NUnit.Framework;

namespace Draw2D.Spatial.UnitTests
{
    [TestFixture]
    public class Size2Tests
    {
        [TestCase]
        public void Construtor_Sets_All_Fields()
        {
            var target = new Size2(600, 400);
            Assert.AreEqual(600.0, target.Width);
            Assert.AreEqual(400.0, target.Height);
        }
    }
}
