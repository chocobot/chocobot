using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocobot.Datatypes
{
    class CoordinateInt
    {

        public int X;
        public int Y;


        public CoordinateInt()
        {
            X = 0;
            Y = 0;
        }

        public CoordinateInt(int x, int y)
        {
            X = x;
            Y = y;
        }

        public CoordinateInt(float x, float y)
        {
            X = (int) x;
            Y = (int) y;
        }
    }
}
