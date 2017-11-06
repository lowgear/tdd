using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization
{
    public static class RectangleExtensions
    {
        public static IEnumerable<Point> Vertices(this Rectangle rectangle)
        {
            foreach (var y in new[] { rectangle.Top, rectangle.Bottom })
            foreach (var x in new[] { rectangle.Left, rectangle.Right })
                yield return new Point(x, y);
        }

        public static double Area(this Rectangle rectangle)
        {
            return rectangle.Height * rectangle.Width;
        }
    }
}
