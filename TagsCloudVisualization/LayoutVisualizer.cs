using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagsCloudVisualization
{
    public static class LayoutVisualizer
    {
        public static Bitmap Visualize(this IEnumerable<Rectangle> rectangles)
        {
            const int margin = 5;

            var minX = rectangles.Min(p => p.X);
            var minY = rectangles.Min(p => p.Y);
            var width = rectangles.Max(p => p.X + p.Width) - minX + 2 * margin;
            var height = rectangles.Max(p => p.Y + p.Height) - minY + 2 * margin;

            var bitmap = new Bitmap(width, height);

            var offsetX = - minX + margin;
            var offsetY = - minY + margin;
            var offset = new Size(offsetX, offsetY);

            var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.White, 0, 0, width, height);
            //graphics.RenderingOrigin = new Point(offsetX, offsetY);
            var pen = new Pen(Color.Black);
            var brush = Brushes.DarkGray;

            //graphics.DrawRectangle(pen, new Rectangle(0, 0, 100, 100));
            foreach (var rectangle in rectangles)
            {
                var r = new Rectangle(Point.Add(rectangle.Location, offset), rectangle.Size);
                graphics.FillRectangle(brush, r);
                graphics.DrawRectangle(pen, r);
            }

            return bitmap;
        }
    }
}