using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagsCloudVisualization
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var minW = 20;
            var maxW = 80;
            var minH = 10;
            var maxH = 40;
            string filename = @"C:\Users\Aidar\Pictures\test\res";
            for (int n = 1; n <= 5; n++)
            {
                var layouter = new CircularCloudLayouter(new Point(0, 0));
                var rectangles = new List<Rectangle>();

                var seed = DateTime.Now.Millisecond;
                Random r = new Random(seed);
                Console.WriteLine(seed);
                for (var i = 0; i < 20; i++)
                {
                    var size = new Size(r.Next(minW, maxW), r.Next(minH, maxH));
                    rectangles.Add(layouter.PutNextRectangle(size));
                }

                var bitmap = rectangles.Visualize();
                bitmap.Save(filename + n + ".bmp", ImageFormat.Bmp);
            }
            Console.WriteLine("Done!");
        }
    }

}
