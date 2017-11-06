using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
            var bestLocation = default(Point);
            var bestDistance = double.PositiveInfinity;
            foreach (var existingRectangle in existingRectangles)
            {
                foreach (var location in existingRectangle.Vertices().SelectMany(p => new[]
                {
                    p,
                    Point.Subtract(p, rectangleSize),
                    new Point(p.X, p.Y - rectangleSize.Height),
                    new Point(p.X - rectangleSize.Width, p.Y)
                }))
                {
                    rectangle.Location = location;
                    var distance = rectangle
                        .Vertices()
                        .Select(point => point.DistanceTo(center))
                        .Max();
                    if (!(bestDistance > distance) ||
                        existingRectangles.Any(r => r.IntersectsWith(rectangle))) continue;
                    bestLocation = location;
                    bestDistance = distance;
                }
            }
            return bestLocation;
        }
    }
}