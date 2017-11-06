using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using MoreLinq;

namespace TagsCloudVisualization
{
    public static class EnumerablePointsExtensions
    {
        public static IEnumerable<Point> ManhattanConvexHull(this IEnumerable<Point> points)
        {
            var sortedPoints = points.OrderBy(p => p.X).ToList();
            var maxY = points.Max(p => p.Y);
            var minY = points.Min(p => p.Y);

            var segmLeftDown = new List<Point>();
            foreach (var point in sortedPoints)
                if (segmLeftDown.Count == 0)
                    segmLeftDown.Add(point);
                else if (point.Y > segmLeftDown[segmLeftDown.Count - 1].Y)
                {
                    while (segmLeftDown.Count != 0 && segmLeftDown[segmLeftDown.Count - 1].X == point.X)
                        segmLeftDown.RemoveAt(segmLeftDown.Count - 1);
                    if (segmLeftDown.Count != 0)
                        segmLeftDown.Add(new Point(point.X, segmLeftDown[segmLeftDown.Count - 1].Y));
                        segmLeftDown.Add(point);
                    if (point.Y == minY)
                        break;
                }
            var segmLeftUp = new List<Point>();
            foreach (var point in sortedPoints)
                if (segmLeftUp.Count == 0)
                    segmLeftUp.Add(point);
                else if (point.Y < segmLeftUp[segmLeftUp.Count - 1].Y)
                {
                    while (segmLeftUp.Count != 0 && segmLeftUp[segmLeftUp.Count - 1].X == point.X)
                        segmLeftUp.RemoveAt(segmLeftUp.Count - 1);
                    if (segmLeftUp.Count != 0)
                        segmLeftUp.Add(new Point(point.X, segmLeftUp[segmLeftUp.Count - 1].Y));
                    segmLeftUp.Add(point);
                    if (point.Y == minY)
                        break;
                }
            sortedPoints.Reverse();
            var segmRightDown = new List<Point>();
            foreach (var point in sortedPoints)
                if (segmRightDown.Count == 0)
                    segmRightDown.Add(point);
                else if (point.Y > segmRightDown[segmRightDown.Count - 1].Y)
                {
                    while (segmRightDown.Count != 0 && segmRightDown[segmRightDown.Count - 1].X == point.X)
                        segmRightDown.RemoveAt(segmRightDown.Count - 1);
                    if (segmRightDown.Count != 0)
                        segmRightDown.Add(new Point(point.X, segmRightDown[segmRightDown.Count - 1].Y));
                    segmRightDown.Add(point);
                    if (point.Y == maxY)
                        break;
                }
            var segmRightUp = new List<Point>();
            foreach (var point in sortedPoints)
                if (segmRightUp.Count == 0)
                    segmRightUp.Add(point);
                else if (point.Y < segmRightUp[segmRightUp.Count - 1].Y)
                {
                    while (segmRightUp.Count != 0 && segmRightUp[segmRightUp.Count - 1].X == point.X)
                        segmRightUp.RemoveAt(segmRightUp.Count - 1);
                    if (segmRightUp.Count != 0)
                        segmRightUp.Add(new Point(point.X, segmRightUp[segmRightUp.Count - 1].Y));
                    segmRightUp.Add(point);
                    if (point.Y == minY)
                        break;
                }
            segmRightUp.Reverse();
            segmLeftDown.Reverse();
            return segmLeftUp.Concat(segmRightUp).Concat(segmRightDown).Concat(segmLeftDown);
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private static IEnumerable<Point> ConvexHull(this IEnumerable<Point> points)
        {
            var referencePoint = points.MinBy(p => Tuple.Create(p.Y, p.X));
            var hull = new List<Point>();
            hull.Add(referencePoint);
            var e = new Point(0, 1);
            foreach (var point in points.Where(p => !p.Equals(referencePoint))
                .OrderBy(p => p.Substruct(referencePoint).AngleTo(e)))
            {
                while (hull.Count >= 2 &&
                       point.Substruct(hull[hull.Count - 1])
                           .PseudoVectorProduct(
                               hull[hull.Count - 1].Substruct(hull[hull.Count - 2])) > -double.Epsilon)
                    hull.RemoveAt(hull.Count - 1);

                hull.Add(point);
            }
            return hull;
        }
    }
}