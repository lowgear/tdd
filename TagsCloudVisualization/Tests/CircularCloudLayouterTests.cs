using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TagsCloudVisualization.Extensions;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class CircularCloudLayouterTests
    {
        private CircularCloudLayouter layouter;
        private Point center;
        private List<Rectangle> layout;
        private Random random;
        private const double TimeCoefficient = 0.0003; // adjusted experimentaly with resereve of about 10 times of measured execution time

        [SetUp]
        public void SetUp()
        {
            layout = new List<Rectangle>();
        }

        private void Arrange(int centerX, int centerY, int randomSeed = 0)
        {
            center = new Point(centerX, centerY);
            layouter = new CircularCloudLayouter(center);
            random = new Random(randomSeed);
        }

        [Timeout(1000)]
        [TestCase(0, 0)]
        [TestCase(2, 3)]
        public void PutNextRectangle_FirstRectangle_ShouldBeCentered(int centerX, int centerY)
        {
            Arrange(centerX, centerY);

            var halfSize = new Size(10, 13);
            var rectangle = layouter.PutNextRectangle(Size.Add(halfSize, halfSize));

            Point.Add(rectangle.Location, halfSize).ShouldBeEquivalentTo(center);
        }

        [Timeout(200)]
        [TestCase(0, 0, 2)]
        [TestCase(2, 3, 2)]
        [TestCase(2, 3, 3)]
        [TestCase(2, 3, 8)]
        [TestCase(2, 3, 100)]
        public void PutNextRextangle_ManyRectanglesOfFixedSize_ShouldNotIntersect(int centerX, int centerY,
            int rectanglesNumber)
        {
            Arrange(centerX, centerY);

            var size = new Size(10, 7);
            for (var i = 0; i < rectanglesNumber; i++)
                layout.Add(layouter.PutNextRectangle(size));

            layout.ShouldNotIntersect();
        }

        [Timeout(50)]
        [TestCase(2)]
        [TestCase(2, 3)]
        [TestCase(10, 5)]
        public void PutNextRectangle_ManyRandomRectangles_ShouldNotIntersect(int rectanglesNumber,
            int randomSeed = 0)
        {
            Arrange(0, 0);

            AddRandomRectangles(rectanglesNumber);

            layout.ShouldNotIntersect();
        }

        [Timeout(200)]
        [TestCase(1, 0.4)]
        [TestCase(2, 0.3)]
        [TestCase(3, 0.35)]
        [TestCase(4, 0.35)]
        [TestCase(5, 0.35)]
        [TestCase(6, 0.35)]
        [TestCase(7, 0.35)]
        [TestCase(8, 0.35)]
        [TestCase(100, 0.8)]
        public void PutNextRectangle_ManyRectangles_ShouldBeDenserThanMinDensity(int rectanglesNumber,
            double minDensity)
        {
            Arrange(0, 0);

            var size = new Size(10, 15);
            for (var i = 0; i < rectanglesNumber; i++)
                layout.Add(layouter.PutNextRectangle(size));

            LayoutDensity.Should().BeGreaterOrEqualTo(minDensity);
        }

        [Timeout(200)]
        [TestCase(5, 0.3, 5)]
        [TestCase(10, 0.5, 10)]
        [TestCase(20, 0.5, 20)]
        [TestCase(50, 0.6, 50)]
        [TestCase(100, 0.6, 100)]
        public void PutNextRectangle_ManyRandomRectangles_ShouldBeDenserThanMinDensity(int rectanglesNumber,
            double minDensity, int randomSeed)
        {
            Arrange(0, 0, randomSeed);

            AddRandomRectangles(rectanglesNumber);

            LayoutDensity.Should().BeGreaterOrEqualTo(minDensity);
        }

        [Timeout(200)]
        [TestCase(5, 0.3)]
        [TestCase(10, 0.4)]
        [TestCase(20, 0.5)]
        [TestCase(50, 0.6)]
        [TestCase(100, 0.6)]
        public void PutNextRectangle_ManyWideRectangles_ShouldBeDenserThanMinDensity(int rectanglesNumber,
            double minDensity)
        {
            Arrange(0, 0);

            var size = new Size(50, 10);
            for (var i = 0; i < rectanglesNumber; i++)
                layout.Add(layouter.PutNextRectangle(size));

            LayoutDensity.Should().BeGreaterOrEqualTo(minDensity);
        }

        [Timeout(200)]
        [TestCase(5, 0.2, 5)]
        [TestCase(10, 0.25, 10)]
        [TestCase(20, 0.3, 20)]
        [TestCase(50, 0.4, 50)]
        [TestCase(100, 0.5, 100)]
        public void PutNextRectangle_ManyRandomWideRectangles_ShouldBeDenserThanMinDensity(
            int rectanglesNumber,
            double minDensity, int randomSeed)
        {
            Arrange(0, 0, randomSeed);

            AddRandomRectangles(rectanglesNumber, 100, 10, 300, 20);

            LayoutDensity.Should().BeGreaterOrEqualTo(minDensity);
        }

        [Timeout(50000)]
        [TestCase(5, 0.15, 5)]
        [TestCase(10, 0.25, 10)]
        [TestCase(20, 0.3, 20)]
        [TestCase(50, 0.4, 50)]
        [TestCase(100, 0.5, 100)]
        [TestCase(1000, 0.7, 1000)]
        public void PutNextRectangle_ManyRandomHighRectangles_ShouldBeDenserThanMinDensity(
            int rectanglesNumber,
            double minDensity, int randomSeed)
        {
            Arrange(0, 0, randomSeed);

            AddRandomRectangles(rectanglesNumber, 10, 100, 20, 300);

            LayoutDensity.Should().BeGreaterOrEqualTo(minDensity);
        }

        [TestCase(100)]
        [TestCase(500)]
        [TestCase(1000)]
        public void PutNextRectangle_ShouldFitInTimeout(int number)
        {
            Arrange(0, 0);

            var expectedTime = new TimeSpan(0, 0, 0, 0,
                Math.Max(
                    (int) (Math.Pow(number, 3) * TimeCoefficient),
                    150));

            Action act = () => AddRectangleGivenTimes(number);
            act.ExecutionTime().ShouldNotExceed(expectedTime);
        }

        private void AddRectangleGivenTimes(int times)
        {
            var size = new Size(1, 3);
            for (int i = 0; i < times; i++)
                layouter.PutNextRectangle(size);
        }

        [TearDown]
        public void TearDown()
        {
            if (Equals(TestContext.CurrentContext.Result.Outcome, ResultState.Failure))
            {
                var path = FormImagePath();

                layout.Visualize().Save(path, ImageFormat.Jpeg);

                Console.WriteLine($"Tag cloud visualization saved to file \"{path}\"");
            }
        }

        private Size GetRandomSize(int minW, int minH, int maxW, int maxH)
        {
            return new Size(random.Next(minW, maxW), random.Next(minH, maxH));
        }

        private double LayoutDensity
        {
            get
            {
                double radius = 0;
                foreach (var rectangle in layout)
                foreach (var vertex in rectangle.Vertices())
                    radius = Math.Max(center.DistanceTo(vertex), radius);

                var rectanglesArea = layout.Sum(rectangle => rectangle.Area());
                var circleArea = Math.Pow(radius, 2) * Math.PI;

                return rectanglesArea / circleArea;
            }
        }

        private void AddRandomRectangles(int rectanglesNumber, int minW = 10, int minH = 10, int maxW = 100,
            int maxH = 100)
        {
            for (var i = 0; i < rectanglesNumber; i++)
            {
                var size = GetRandomSize(minW, minH, maxW, maxH);
                layout.Add(layouter.PutNextRectangle(size));
            }
        }

        private static string FormImagePath()
        {
            var path = TestContext.CurrentContext.WorkDirectory;
            var imageName = TestContext.CurrentContext.Test.ID + ".jpg";
            path += Path.DirectorySeparatorChar + imageName;
            return path;
        }
    }
}