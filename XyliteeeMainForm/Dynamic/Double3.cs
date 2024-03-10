using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotoKaze.Dynamic
{
    public class Double3(double X, double Y, double Z)
    {
        public double X = X;
        public double Y = Y;
        public double Z = Z;

        public double Length()
        {
            double length = Math.Sqrt(X * X + Y * Y + Z * Z);
            return length;
        }

        public static Double3 operator *(Double3 a, double b)
        {
            return new Double3(a.X * b, a.Y * b, a.Z * b);
        }
        public static Double3 operator *(double b, Double3 a)
        {
            return new Double3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Double3 operator /(Double3 a, double b)
        {
            return new Double3(a.X /b, a.Y / b, a.Z / b);
        }

        public static Double3 operator +(Double3 a, double b)
        {
            return new Double3(a.X + b, a.Y + b, a.Z + b);
        }

        public static Double3 operator +(double b, Double3 a)
        {
            return new Double3(a.X + b, a.Y + b, a.Z + b);
        }

        public static Double3 operator -(Double3 a, double b)
        {
            return new Double3(a.X - b, a.Y - b, a.Z - b);
        }

        public static Double3 operator +(Double3 a, Double3 b)
        {
            return new Double3 (a.X + b.X,a.Y + b.Y,a.Z + b.Z );
        }

        public static Double3 operator -(Double3 a, Double3 b)
        {
            return new Double3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
    }
}
