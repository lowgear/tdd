using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MoreLinq;

namespace TagsCloudVisualization.Extensions
{
    public static class RectangleExtensions
    {
        public static IEnumerable<Point> Vertices(this Rectangle rectangle)
        {
            return new[] {rectangle.Left, rectangle.Right}
                .Cartesian(new[] {rectangle.Top, rectangle.Bottom},
                    (x, y) => new Point(x, y));
        }

        public static double Area(this Rectangle rectangle)
        {
            return rectangle.Height * rectangle.Width;
        }

        public static IEnumerable<Point> LocationsIfHadVertex(this Rectangle rectangle, Point p)
        {
            yield return p;
            yield return Point.Subtract(p, rectangle.Size);
            yield return new Point(p.X, p.Y - rectangle.Size.Height);
            yield return new Point(p.X - rectangle.Size.Width, p.Y);
        }

        public static double MaxDistanceTo(this Rectangle rectangle, Point point)
        {
            return rectangle.Vertices()
                .Max(p => p.DistanceTo(point));
        }
    }
}