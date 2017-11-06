using System;
using System.Drawing;

namespace TagsCloudVisualization
{
    public static class PointExtensions
    {
        public static double DistanceTo(this Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        public static Point Substruct(this Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static double PseudoVectorProduct(this Point a, Point b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static double ScalarProduct(this Point a, Point b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static double AngleTo(this Point a, Point b)
        {
            return Math.Atan2(a.ScalarProduct(b), a.PseudoVectorProduct(b));
        }
    }
}