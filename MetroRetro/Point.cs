using SharpDX;

namespace MetroRetro
{
    public class Point
    {
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public RectangleF ToBox(Point size)
        {
            return new RectangleF(X - size.X / 2, Y - size.Y / 2, X + size.X / 2, Y + size.Y / 2);
        }

        public RectangleF ApplyTo(RectangleF rectangle)
        {
            rectangle.Left *= X;
            rectangle.Right *= X;
            rectangle.Top *= Y;
            rectangle.Bottom *= Y;
            return rectangle;
        }

        public Point Add(Point point)
        {
            return new Point(X + point.X, Y + point.Y);
        }

        public Point Add(float v)
        {
            return new Point(X +v, Y + v);
        }

        public Point Mul(Point point)
        {
            return new Point(X * point.X, Y * point.Y);
        }

        public Point Mul(float v)
        {
            return new Point(X * v, Y * v);
        }
    }
}