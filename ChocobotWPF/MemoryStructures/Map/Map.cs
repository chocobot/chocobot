using System;
using System.Collections.Generic;
using System.Globalization;
using Chocobot.Utilities.Memory;

namespace Chocobot.MemoryStructures.Map
{
    public class Map
    {
        private readonly Dictionary<short, string> _mapIndex = new Dictionary<short, string>();

        public static Map Instance = new Map();
        public ushort MapIndex;

        public Map()
        {
            _mapIndex.Add(145, "Eastern Thanalan");
            _mapIndex.Add(153, "South Shroud");
            _mapIndex.Add(130, "Uldah");
            _mapIndex.Add(134, "Middle La Noscea");
        }

        public void Refresh()
        {
            MapIndex = MemoryHandler.Instance.GetUInt16(MemoryLocations.Database["map"]);
        }

        public string Index2Name(short index)
        {

            try
            {
                return _mapIndex[index];
            }
            catch (Exception)
            {
                return "Unknown - " + index.ToString(CultureInfo.InvariantCulture);
            }


        }
    }
}
