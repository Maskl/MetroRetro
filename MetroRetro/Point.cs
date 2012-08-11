using System;
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

        public Point Add(Point point)
        {
            return new Point(X + point.X, Y + point.Y);
        }

        public Point Add(float v)
        {
            return new Point(X + v, Y + v);
        }

        public Point Sub(Point point)
        {
            return new Point(X - point.X, Y - point.Y);
        }

        public Point Sub(float v)
        {
            return new Point(X - v, Y - v);
        }

        public Point Mul(Point point)
        {
            return new Point(X * point.X, Y * point.Y);
        }

        public Point Mul(float v)
        {
            return new Point(X * v, Y * v);
        }

        public Point Div(Point point)
        {
            return new Point(X / point.X, Y / point.Y);
        }

        public Point Div(float v)
        {
            return new Point(X / v, Y / v);
        }

        public Point Half()
        {
            return new Point(X / 2, Y / 2);
        }

        public Point Clamp(Point margin0, Point margin1)
        {
            return new Point(
                Math.Min(Math.Max(X, margin0.X), margin1.X),
                Math.Min(Math.Max(Y, margin0.Y), margin1.Y));
        }
        public RectangleF ApplyTo(RectangleF rectangle)
        {
            rectangle.Left *= X;
            rectangle.Right *= X;
            rectangle.Top *= Y;
            rectangle.Bottom *= Y;
            return rectangle;
        }

        public RectangleF ToBox(Point size)
        {
            return new RectangleF(X - size.X / 2, Y - size.Y / 2, X + size.X / 2, Y + size.Y / 2);
        }

        public RectangleF ToRectangleWith(Point point)
        {
            return new RectangleF(X, Y, point.X, point.Y);
        }
    }
}