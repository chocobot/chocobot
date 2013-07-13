using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chocobot.Datatypes;

namespace Chocobot.MemoryStructures.Map
{
    class MapNavArr
    {
        public const int ArrSize = 4096;
        public readonly byte[,] MapArr;
        public const float ArrScale = 2;
        public CoordinateInt Min = new CoordinateInt();

        private static T[,] GetNew2DArray<T>(int x, int y, T initialValue)
        {
            T[,] nums = new T[x, y];
            for (int i = 0; i < x * y; i++) nums[i % x, i / x] = initialValue;
            return nums;
        }


        public MapNavArr(List<List<Coordinate>> waypointGroups)
        {
            MapArr = GetNew2DArray(MapNavArr.ArrSize, MapNavArr.ArrSize, (byte)0); //new byte[16384,16384];
            Coordinate minCoord = new Coordinate((float)9999.0, (float)9999.0, (float)9999.0);


            foreach (List<Coordinate> waypointgroup in waypointGroups)
            {
                foreach (Coordinate waypoint in waypointgroup)
                {
                    if (minCoord.X > waypoint.X)
                        minCoord.X = waypoint.X;

                    if (minCoord.Y > waypoint.Y)
                        minCoord.Y = waypoint.Y;
                }
            }

            int minX = (int)(minCoord.X * ArrScale);
            int minY = (int)(minCoord.Y * ArrScale);

            Min.X = (int)(minCoord.X);
            Min.Y = (int)(minCoord.Y);

            foreach (List<Coordinate> waypointgroup in waypointGroups)
            {

                int prevX = 0, prevY = 0;
                bool initial = true;

                foreach (Coordinate waypoint in waypointgroup)
                {
                    int xCoord = (int)(waypoint.X * ArrScale);
                    int yCoord = (int)(waypoint.Y * ArrScale);

                    if (xCoord >= MapNavArr.ArrSize || yCoord >= MapNavArr.ArrSize)
                    {
                        System.Diagnostics.Debug.Print("Coordinate to large. " + xCoord + " " + yCoord);
                        continue;
                    }

                    MapArr[xCoord - minX, yCoord - minY] = 1;

                    if (initial)
                    {
                        initial = false;
                        prevX = xCoord;
                        prevY = yCoord;

                    }
                    else
                    {
                        double a = Math.Abs(xCoord - prevX);
                        double b = Math.Abs(yCoord - prevY);
                        double blendVal = 0.05;
                        double dist = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));

                        if (dist > 1.0)
                        {
                            while (blendVal < 1)
                            {
                                int tmpX = (int)(xCoord + (blendVal * (prevX - xCoord)));
                                int tmpY = (int)(yCoord + (blendVal * (prevY - yCoord)));

                                MapArr[tmpX - minX, tmpY - minY] = 1;
                                blendVal += 0.05;
                            }

                        }

                        prevX = xCoord;
                        prevY = yCoord;
                    }

                }

            }

        }




        public CoordinateInt GetClosestIndex(Coordinate pos)
        {
            CoordinateInt minIndex = new CoordinateInt();
            float minDistance = (float) 9999.0;

            for (int i = 0; i < ArrSize; i++)
            {
                for (int j = 0; j < ArrSize; j++)
                {

                    if (MapArr[i, j] == 0)
                        continue;

                    Coordinate arrPos = new Coordinate(i / ArrScale + Min.X, j / ArrScale + Min.Y, 0);
                    float currDist = pos.Distance(arrPos);

                    if (currDist < minDistance)
                    {
                        minDistance = currDist;
                        minIndex.X = i;
                        minIndex.Y = j;
                    }

                }
            }

   
             return minIndex;
    
        }



        public void Save(CoordinateInt Start, CoordinateInt End)
        {
            FileStream fs = new FileStream("D:\\path.astar", FileMode.Create, FileAccess.Write);

            fs.WriteByte((byte)(Start.X >> 8));
            fs.WriteByte((byte)(Start.X & 0x000000FF));
            fs.WriteByte((byte)(Start.Y >> 8));
            fs.WriteByte((byte)(Start.Y & 0x000000FF));
            fs.WriteByte((byte)(End.X >> 8));
            fs.WriteByte((byte)(End.X & 0x000000FF));
            fs.WriteByte((byte)(End.Y >> 8));
            fs.WriteByte((byte)(End.Y & 0x000000FF));
            fs.WriteByte((byte)(1));
            fs.WriteByte((byte)(0));
            fs.WriteByte((byte)(0));
            fs.WriteByte((byte)2);
            fs.WriteByte((byte)0);
            fs.WriteByte((byte)(0));
            fs.WriteByte((byte)(((int)50000) >> 24));
            fs.WriteByte((byte)(((int)50000) >> 16));
            fs.WriteByte((byte)(((int)50000) >> 8));
            fs.WriteByte((byte)(((int)50000) & 0x000000FF));
            fs.WriteByte((byte)50);
            fs.WriteByte((byte)1);

            for (int y = 0; y < 1000; y++)
                for (int x = 0; x < 1000; x++)
                    fs.WriteByte(MapArr[x, y]);

            fs.Close();

        }
    }
}
