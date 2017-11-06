using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        private Point center;
        private readonly List<Rectangle> existingRectangles = new List<Rectangle>();

        public CircularCloudLayouter(Point center)
        {
            this.center = center;
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            var res = NextRectangle(rectangleSize);
            existingRectangles.Add(res);
            return res;
        }

        private Rectangle NextRectangle(Size rectangleSize)
        {
            var res = new Rectangle(Point.Empty, rectangleSize);
            if (existingRectangles.Count == 0)
            {
                var halfSize = new Size(rectangleSize.Width / 2, rectangleSize.Height / 2);
                res.Location = Point.Subtract(center, halfSize);
            }
            else
                res.Location = ChooseLocationForRectangle(res);

            return res;
        }

        private Point ChooseLocationForRectangle(Rectangle rectangle)
        {
            var bestLocation = default(Point);
            var bestDistance = Double.PositiveInfinity;
            var t = existingRectangles
                .SelectMany(rect => rect.Vertices())
                .ManhattanConvexHull()
                .Ngramms(4).ToList();
            foreach (var edge in t)
            {
                var curLocation = BestLocationAlignedEdge(rectangle, edge);
                rectangle.Location = curLocation;
                var curDistance = rectangle
                    .Vertices()
                    .Select(point => point.DistanceTo(center))
                    .Max();
                if (!(curDistance < bestDistance)) continue;
                bestDistance = curDistance;
                bestLocation = curLocation;
            }
            return bestLocation;
        }

        private Point BestLocationAlignedEdge(Rectangle rectangle,
            Point[] threeVertices)
        {
            var firstEdge = threeVertices[1].Substruct(threeVertices[0]);
            var middleEdge = threeVertices[2].Substruct(threeVertices[1]);
            var lastEdge = threeVertices[3].Substruct(threeVertices[2]);

            int x;
            int y;
            if (firstEdge.PseudoVectorProduct(middleEdge) > 0 &&
                middleEdge.PseudoVectorProduct(lastEdge) > 0)
                // case when three sequental edges make two turns clockwise each
                ResolveTwoClockwiseTurnsCase(rectangle, threeVertices, middleEdge, out y, out x);
            else
                ResolveClockwiseAndCounterClockwiseTurnsCase(rectangle, threeVertices, middleEdge, firstEdge, out x, out y);
            return new Point(x, y);
        }

        private void ResolveClockwiseAndCounterClockwiseTurnsCase(Rectangle rectangle, Point[] threeVertices,
            Point middleEdge, Point firstEdge, out int x, out int y)
        {
            if (middleEdge.X == 0)
            {
                y = BestCoordinate(
                    rectangle.Height,
                    threeVertices.Skip(1).Take(2).Select(p => p.Y).ToArray(),
                    firstEdge.PseudoVectorProduct(middleEdge) < 0,
                    middleEdge.Y);
                x = threeVertices[1].X - (middleEdge.Y > 0 ? 0 : rectangle.Width);
            }
            else
            {
                x = BestCoordinate(
                    rectangle.Width,
                    threeVertices.Skip(1).Take(2).Select(p => p.X).ToArray(),
                    firstEdge.PseudoVectorProduct(middleEdge) < 0,
                    middleEdge.X);
                y = threeVertices[1].Y - (middleEdge.X < 0 ? 0 : rectangle.Height);
            }
        }

        private void ResolveTwoClockwiseTurnsCase(Rectangle rectangle, Point[] threeVertices, Point middleEdge,
            out int y, out int x)
        {
            // if middle edge is horizontal
            if (middleEdge.Y == 0)
            {
                x = center.X - rectangle.Width / 2;
                y = threeVertices[1].Y;
                if (middleEdge.X > 0)
                    y -= rectangle.Height;
            }
            // if middle edge is vertical
            else
            {
                x = threeVertices[1].X;
                y = center.Y - rectangle.Height / 2;
                if (middleEdge.Y < 0)
                    x -= rectangle.Width;
            }
        }

        private int BestCoordinate(int offset, int[] possibleConstraints, bool firstTurnIsCounterClockwise,
            int middleEdgeDirection)
        {
            var lowerConstraint = int.MinValue;
            var upperConstraint = int.MaxValue;
            var desiredCoordinate = center.X - offset / 2;
            if (!firstTurnIsCounterClockwise)
            {
                if (middleEdgeDirection > 0)
                    upperConstraint = possibleConstraints.Max();
                else
                    lowerConstraint = possibleConstraints.Min();
            }
            else
            {
                if (middleEdgeDirection > 0)
                    lowerConstraint = possibleConstraints.Min();
                else
                    upperConstraint = possibleConstraints.Max();
            }
            if (upperConstraint != int.MaxValue)
                upperConstraint -= offset;
            return Math.Min(Math.Max(lowerConstraint, desiredCoordinate), upperConstraint);
        }
    }
}