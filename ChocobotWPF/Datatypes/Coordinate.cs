using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chocobot.Datatypes
{
    public class Coordinate
    {
        public float X;
        public float Y;
        public float Z;

        public bool setHeading = false;
        public float Heading = (float)0.0;

        public Coordinate Rotate2d(float Angle)
        {
            Coordinate newCoordinate = new Coordinate();

            newCoordinate.Z = Z;
            newCoordinate.X = (float)(X * Math.Cos(Angle) - Y * Math.Sin(Angle));
            newCoordinate.Y = (float)(Y * Math.Cos(Angle) + X * Math.Sin(Angle));

            return newCoordinate;

        }

        public double Angle()
        {
            double angle = Math.Atan(Y / X);
            return angle;
        }

        public Coordinate Subtract(Coordinate b)
        {
            return new Coordinate(X - b.X, Y - b.Y, Z - b.Z);
        }

        public Coordinate Add(Coordinate b)
        {
            return new Coordinate(X + b.X, Y + b.Y, Z + b.Z);
        }

        public Coordinate Add(float dx, float dy, float dz)
        {
            return new Coordinate(X + dx, Y + dy, Z + dz);
        }

        public Coordinate Scale(float s)
        {
            return new Coordinate(X * s, Y * s, Z * s);
        }

        public Coordinate Normalize()
        {
            float length = (float) Math.Sqrt(Math.Pow(X,2) + Math.Pow(Y,2) + Math.Pow(Z,2));
            return new Coordinate(X / length, Y / length, Z / length);
        }

        public Coordinate Normalize(Coordinate origin)
        {
            Coordinate nCoordinate = this.Subtract(origin);

            return nCoordinate.Normalize();
        }

        public float AngleTo(Coordinate b)
        {
            Coordinate tmp = b.Subtract(this);
            return (float) Math.Atan2(tmp.X, tmp.Y);
        }

        public float Distance(Coordinate coordinate2)
        {

            return (float)Math.Sqrt(Math.Pow(X - coordinate2.X, 2) + Math.Pow(Y - coordinate2.Y, 2) + Math.Pow(Z - coordinate2.Z, 2));

        }

        public float Distance2D(Coordinate coordinate2)
        {

            return (float)Math.Sqrt(Math.Pow(X - coordinate2.X, 2) + Math.Pow(Y - coordinate2.Y, 2));

        }

        public Coordinate()
        {
        }

        public Coordinate(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public override string ToString()
        {
            return X + ", " + Y + ", " + Z;
        }


    }
}
