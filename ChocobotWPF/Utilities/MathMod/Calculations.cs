using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chocobot.Datatypes;

namespace Chocobot.Utilities.MathMod
{
    public static class Calculations
    {

        public static float PointDistance(Coordinate Coordinate1, Coordinate Coordinate2)
        {

            return (float) Math.Sqrt(Math.Pow(Coordinate1.X - Coordinate2.X, 2) + Math.Pow(Coordinate1.Y - Coordinate2.Y, 2) + Math.Pow(Coordinate1.Z - Coordinate2.Z, 2));

        }



    }
}
