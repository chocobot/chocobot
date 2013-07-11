using System;
using Chocobot.Utilities.FileIO;

namespace Chocobot.MemoryStructures.Map
{
    public class MapInfo
    {
        public string Name;
        public float XScale;
        public float YScale;
        public float XOffset;
        public float YOffset;

        public MapInfo(ushort index)
        {

            if (System.IO.File.Exists(@"Maps\" + index + ".ini"))
            {
                IniParser parser = new IniParser(@"Maps\" + index + ".ini");

                try
                {
                    Name = parser.GetSetting("map", "name");
                    XScale = float.Parse(parser.GetSetting("map", "x_scale"));
                    YScale = float.Parse(parser.GetSetting("map", "y_scale"));
                    XOffset = float.Parse(parser.GetSetting("map", "x_offset"));
                    YOffset = float.Parse(parser.GetSetting("map", "y_offset"));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print("Problem reading map ini file. " + ex.Message);
                }


            }

        }
    }
}
