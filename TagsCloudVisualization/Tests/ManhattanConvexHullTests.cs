using System.Drawing;
using FluentAssertions;
using MoreLinq;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    [TestFixture]
    class ManhattanConvexHullTests
    {
        private Rectangle rectangle;

        [SetUp]
        public void SetUp()
        {
            rectangle = new Rectangle(0, 0, 1, 1);
        }


        [Test]
        public void ManhattanConvexHull_ReturnRectangleSameAs_Given()
        {
            rectangle.Vertices().ManhattanConvexHull().ShouldAllBeEquivalentTo(rectangle.Vertices());
        }
    }
}
