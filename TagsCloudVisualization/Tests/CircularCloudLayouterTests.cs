using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;

namespace TagsCloudVisualization
{
    [TestFixture]
    public class CircularCloudLayouterTests
    {
        private CircularCloudLayouter layouter;
        private Point center;

        private void Arrange(int centerX, int centerY)
        {
            center = new Point(centerX, centerY);
            layouter = new CircularCloudLayouter(center);
        }

        [TestCase(0, 0)]
        [TestCase(2, 3)]
        public void PutNextRectangle_FirstRectangle_ShouldBeCentered(int centerX, int centerY)
        {
            Arrange(centerX, centerY);

            var halfSize = new Size(10, 10);
            var rectangle = layouter.PutNextRectangle(Size.Add(halfSize, halfSize));

            Point.Add(rectangle.Location, halfSize).ShouldBeEquivalentTo(center);
        }

        [TestCase(0, 0, 2)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 3)]
        [TestCase(2, 3, 8)]
        [TestCase(2, 3, 100)]
        public void PutNextRextangle_ManyRectangles_ShouldNotIntersect(int centerX, int centerY, int rectanglesNumber)
        {
            Arrange(centerX, centerY);

            var size = new Size(10, 7);
            var rectangles = new List<Rectangle>();
            for (var i = 0; i < rectanglesNumber; i++)
                rectangles.Add(layouter.PutNextRectangle(size));

            for (var i = 1; i < rectanglesNumber; i++)
            for (var j = 0; j < i; j++)
                rectangles[i].IntersectsWith(rectangles[j]).Should().BeFalse();
        }

        [TestCase(1, 0.4)]
        [TestCase(2, 0.3)]
        [TestCase(3, 0.35)]
        [TestCase(4, 0.35)]
        [TestCase(5, 0.35)]
        [TestCase(6, 0.35)]
        [TestCase(7, 0.35)]
        [TestCase(8, 0.35)]
        [TestCase(100, 0.8)]
        public void PutNextRectangle_ManyRectangles_ShouldBeDenserThanMinDensity(int rectanglesNumber, double minDensity)
        {
            Arrange(0, 0);

            var size = new Size(10, 15);
            List<Rectangle> rectangles = new List<Rectangle>();
            for (var i = 0; i < rectanglesNumber; i++)
                rectangles.Add(layouter.PutNextRectangle(size));

            CalculateRectanglesDensity(rectangles, center).Should().BeGreaterOrEqualTo(minDensity);
        }

        private double CalculateRectanglesDensity(List<Rectangle> rectangles, Point center)
        {
            double radius = 0;
            foreach (var rectangle in rectangles)
            foreach (var vertex in rectangle.Vertices())
                radius = Math.Max(center.DistanceTo(vertex), radius);

            var rectanglesArea = rectangles.Sum(rectangle => rectangle.Area());
            var circleArea = Math.Pow(radius, 2) * Math.PI;

            return rectanglesArea / circleArea;
        }
    }
}