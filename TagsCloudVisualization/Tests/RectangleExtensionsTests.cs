using System.Drawing;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Extensions;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class RectangleExtensionsTests
    {
        [Test]
        public void LocationsIfHadVertex_ReturnsProperLocations()
        {
            new Rectangle(Point.Empty, new Size(1, 2)).LocationsIfHadVertex(new Point(2, 3))
                .ShouldBeEquivalentTo(new[]
                {
                    new Point(2, 3),
                    new Point(1, 3),
                    new Point(2, 1),
                    new Point(1, 1)
                });
        }

        [Test]
        public void RectangleVertices_ShouldReturnAllItsVertices()
        {
            new Rectangle(Point.Empty, new Size(1, 2))
                .Vertices()
                .ShouldBeEquivalentTo(new[]
                {
                    new Point(0, 0),
                    new Point(0, 2),
                    new Point(1, 0),
                    new Point(1, 2)
                });
        }

        [TestCase(0, -1, 1, -1, TestName = "ShouldReturnDistanceToUpperRigthCorner")]
        [TestCase(-1, -1, -1, -1, TestName = "ShouldReturnDistanceToUpperLeftCorner")]
        [TestCase(0, 0, 1, 1, TestName = "ShouldReturnDistanceToLowerRigthCorner")]
        [TestCase(-1, 0, -1, 1, TestName = "ShouldReturnDistanceToLowerLeftCorner")]
        public void MaxDistanceTo_Origin(int rectX, int rectY, int cornerX, int cornerY)
        {
            new Rectangle(rectX, rectY, 1, 1).MaxDistanceTo(Point.Empty)
                .Should()
                .Be(new Point(cornerX, cornerY).DistanceTo(Point.Empty));
        }
    }
}