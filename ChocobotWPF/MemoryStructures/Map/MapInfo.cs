using System;
using System.Collections.Generic;
using System.IO;
using Chocobot.Datatypes;
using Chocobot.Utilities.FileIO;

namespace Chocobot.MemoryStructures.Map
{
    public class MapInfo
    {
        public string           Name;
        public float            XScale;
        public float            YScale;
        public float            XOffset;
        public float            YOffset;
        public bool             HasNavCoordinates = false;
        public ushort           Index;
        public List<List<Coordinate>> WaypointGroups = new List<List<Coordinate>>();
        public bool Valid = false;
        
        public MapInfo(ushort index)
        {

            Index = index;
            if (File.Exists(@"Maps\" + index + ".ini"))
            {
                IniParser parser = new IniParser(@"Maps\" + index + ".ini");

                try
                {
                    Name = parser.GetSetting("map", "name");
                    XScale = float.Parse(parser.GetSetting("map", "x_scale"));
                    YScale = float.Parse(parser.GetSetting("map", "y_scale"));
                    XOffset = float.Parse(parser.GetSetting("map", "x_offset"));
                    YOffset = float.Parse(parser.GetSetting("map", "y_offset"));


                    if (File.Exists(@"Maps\" + index + ".nav"))
                    {
                        LoadNav();

                        HasNavCoordinates = true;
                    }
                    Valid = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print("Problem reading map ini file. " + ex.Message);
                    Valid = false;
                }


            }

        }

        public void SaveNav()
        {

            

            using (StreamWriter file = new StreamWriter(@"Maps\" + Index + ".nav"))
            {
                foreach (List<Coordinate> waypointgroup in WaypointGroups)
                {
                    foreach (Coordinate coord in waypointgroup)
                    {
                        file.WriteLine(coord.X + "," + coord.Y + "," + coord.Z);
                    }
                    file.WriteLine("NG");
                }

                file.Close();
            }
        }


        public void LoadNav()
        {

            WaypointGroups.Clear();

            using (StreamReader file = new StreamReader(@"Maps\" + Index + ".nav"))
            {

                List<Coordinate> group = new List<Coordinate>();

                while (file.Peek() >= 0)
                {
                    string currLine = file.ReadLine();

                    if (currLine.Contains("NG"))
                    {
                        WaypointGroups.Add(group);

                        group = new List<Coordinate>();
                    }
                    else
                    {
                        List<String> str = new List<string>(currLine.Split(','));
                        group.Add(new Coordinate(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2])));

                    }
                }
                file.Close();

            }
        }
    }
}
