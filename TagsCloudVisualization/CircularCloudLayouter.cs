using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MoreLinq;
using TagsCloudVisualization.Extensions;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        private readonly Point center;
        private readonly List<Rectangle> existingRectangles = new List<Rectangle>();

        public CircularCloudLayouter(Point center)
        {
            this.center = center;
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            var res = new Rectangle(Point.Empty, rectangleSize);
            if (existingRectangles.Count == 0)
            {
                var halfSize = new Size(rectangleSize.Width / 2, rectangleSize.Height / 2);
                res.Location = Point.Subtract(center, halfSize);
            }
            else
                res.Location = ChooseLocationForRectangle(rectangleSize);

            existingRectangles.Add(res);
            return res;
        }

        private Point ChooseLocationForRectangle(Size rectangleSize)
        {
            var rectangle = new Rectangle(Point.Empty, rectangleSize);

            return ExistingVertices
                .SelectMany(point => rectangle.LocationsIfHadVertex(point))
//                .Distinct()
                .Where(p =>
                {
                    rectangle.Location = p;
                    return existingRectangles.All(r => !r.IntersectsWith(rectangle));
                })
                .MinBy(p =>
                {
                    rectangle.Location = p;
                    return rectangle.MaxDistanceTo(center);
                });
        }

        private IEnumerable<Point> ExistingVertices => existingRectangles
                    .SelectMany(r => r.Vertices())
                    .Distinct();
    }
}