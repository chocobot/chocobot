using System;
using System.Collections.Generic;
using System.Globalization;
using Chocobot.Utilities.Memory;


namespace Chocobot.MemoryStructures.Map
{
    public class Map
    {

        public static Map   Instance = new Map();
        public ushort       MapIndex;

        public Map()
        {
        }

        public void Refresh()
        {
            MapIndex = MemoryHandler.Instance.GetUInt16(MemoryLocations.Database["map"]);
        }

        public MapInfo GetMapInfo()
        {
            return new MapInfo(MapIndex);
        }
   
    }
}
