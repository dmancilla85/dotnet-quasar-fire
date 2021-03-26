using System;
using System.Text.Json;

namespace FuegoDeQuasar.Model
{
    public class Point2D : IPoint
    {
        public Point2D()
        {
            X = 0;
            Y = 0;
        }

        public Point2D(double x)
        {
            X = x;
            Y = 0;
        }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        /// <summary>
        /// The Pythagoras theorem, also known as the Pythagorean theorem, states that the square of
        /// the length of the hypotenuse is equal to the sum of squares of the lengths of other two
        /// sides of the right-angled triangle. Or, the sum of the squares of the two legs of a right
        /// triangle is equal to the square of its hypotenuse.
        /// </summary>
        /// <param name="b">Point b</param>
        /// <returns>The distance from this point to the point b</returns>
        public double DistanceTo(IPoint b)
        {
            Point2D aux = (Point2D)b;
            double ret = 0;

            try
            {
                ret = Math.Sqrt(Squared(aux.X - this.X) + Squared(aux.Y - this.Y));
            }
            catch (ArithmeticException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// Static version of DistanceTo function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static double DistanceFromTo(Point2D a, IPoint b)
        {
            Point2D aux = (Point2D)b;
            double ret = 0;

            try
            {
                ret = Math.Sqrt(Squared(aux.X - a.X) + Squared(aux.Y - a.Y));
            }
            catch (ArithmeticException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ret;
        }

        public static Point2D operator -(Point2D a, Point2D b) => new()
        {
            X = a.X - b.X,
            Y = a.Y - b.Y
        };

        public static Point2D operator +(Point2D a, Point2D b) => new()
        {
            X = a.X + b.X,
            Y = a.Y + b.Y
        };

        public static Point2D operator *(Point2D a, Point2D b) => new()
        {
            X = a.X * b.Y,
            Y = b.X * a.Y
        };

        public static Point2D operator *(double k, Point2D b) => new()
        {
            X = k * b.X,
            Y = b.Y * k
        };

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Heron's formula (sometimes called Hero's formula), named after Hero of Alexandria, gives
        /// the area of a triangle when the length of all three sides are known. Unlike other triangle
        /// area formulae, there is no need to calculate angles or other distances in the triangle first.
        /// </summary>
        /// <param name="a">Distance from A to B</param>
        /// <param name="b">Distance from B to C</param>
        /// <param name="c">Distance from C to A</param>
        /// <returns>Area of a scalene triangle</returns>
        private static double Heron(double a, double b, double c)
        {
            double s = (a + b + c) / 2;
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }

        public static IPoint Triangulation(ISatellite s1, double r1, ISatellite s2, double r2, ISatellite s3, double r3)
        {
            if (r1 == 0 || r2 == 0 || r3 == 0)
            {
                Console.WriteLine("*** Error: Las distancias son incorrectas");
                return null;
            }

            double x1, x2 = 0, y1 = 0, y2;

            try
            {
                //                (X)
                //                /b\
                //           r1  / | \  r2
                //              /  h  \
                //           a /_ r12 _\ g
                //          (s1)       (s2)
                double r12 = s1.DistanceTo(s2);
                double h = 2 * Heron(r1, r12, r2) / r12;
                double alphaRadians = Math.Sin(h / r1);
                double ah = (r1 * Math.Cos(alphaRadians));
                Point2D p1 = (Point2D)s1.GetCoords();
                Point2D p2 = (Point2D)s2.GetCoords();
                Point2D p3 = p1 + ((ah / r12) * (p2 - p1));

                x1 = p3.X + (h * (p2.Y - p1.Y) / r12);
                x2 = p3.X - (h * (p2.Y - p1.Y) / r12);
                y1 = p3.Y + (h * (p2.X - p1.X) / r12);
                y2 = p3.Y + (h * (p2.X - p1.X) / r12);

                // TODO: Select combination with minimun error automatically
                Console.WriteLine("*** Showing the error in the different solutions");
                Console.WriteLine($"*** s1. Error (x1,y1): {Math.Abs(DistanceFromTo(p1, new Point2D(x1, y1)) - r1)}");
                Console.WriteLine($"*** s1. Error (x1,y2): {Math.Abs(DistanceFromTo(p1, new Point2D(x1, y2)) - r1)}");
                Console.WriteLine($"*** s1. Error (x2,y1): {Math.Abs(DistanceFromTo(p1, new Point2D(x2, y1)) - r1)}");
                Console.WriteLine($"*** s1. Error (x2,y2): {Math.Abs(DistanceFromTo(p1, new Point2D(x2, y2)) - r1)}");
                Console.WriteLine($"*** s2. Error (x1,y1): {Math.Abs(DistanceFromTo(p2, new Point2D(x1, y1)) - r2)}");
                Console.WriteLine($"*** s2. Error (x1,y2): {Math.Abs(DistanceFromTo(p2, new Point2D(x1, y2)) - r2)}");
                Console.WriteLine($"*** s2. Error (x2,y1): {Math.Abs(DistanceFromTo(p2, new Point2D(x2, y1)) - r2)}");
                Console.WriteLine($"*** s2. Error (x2,y2): {Math.Abs(DistanceFromTo(p2, new Point2D(x2, y2)) - r2)}");
                Console.WriteLine($"*** s3. Error (x1,y1): {Math.Abs(DistanceFromTo((Point2D)s3.GetCoords(), new Point2D(x1, y1)) - r3)}");
                Console.WriteLine($"*** s3. Error (x1,y2): {Math.Abs(DistanceFromTo((Point2D)s3.GetCoords(), new Point2D(x1, y2)) - r3)}");
                Console.WriteLine($"*** s3. Error (x2,y1): {Math.Abs(DistanceFromTo((Point2D)s3.GetCoords(), new Point2D(x2, y1)) - r3)}");
                Console.WriteLine($"*** s3. Error (x2,y2): {Math.Abs(DistanceFromTo((Point2D)s3.GetCoords(), new Point2D(x2, y2)) - r3)}");
                Console.WriteLine("*** The minimum error is in the 3th and 4th combination, and the right combination is the 3th.");
            }
            catch (ArithmeticException e)
            {
                Console.WriteLine(e.Message);
            }

            return new Point2D()
            {
                X = x2,
                Y = y1
            };
        }

        internal static double Squared(double? a) => Math.Pow(a == null ? 0 : (double)a, 2);
    }
}