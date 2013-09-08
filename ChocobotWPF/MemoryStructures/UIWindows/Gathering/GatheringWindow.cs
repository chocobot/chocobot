using System.Collections.Generic;
using Chocobot.Utilities.Memory;

namespace Chocobot.MemoryStructures.UIWindows.Gathering
{
    class GatheringWindow
    {

        private uint _address;
        private uint _windownameaddress;

        public List<GatheringItems> Items = new List<GatheringItems>();
        private string _windowName;
        private bool _validWindow = true;

        public class GatheringItems
        {
            public string Name;
            public byte Position;

            public GatheringItems(string name, byte position)
            {
                Name = name;
                Position = position;
            }
        }

        public bool RefreshPointers()
        {
            _address = MemoryLocations.Database["crafting"];
            _address = MemoryHandler.Instance.GetUInt32(_address) + 0x20;
            _address = MemoryHandler.Instance.GetUInt32(_address) + 0x1E4;
            _address = MemoryHandler.Instance.GetUInt32(_address) + 0x8;

            if (_address == 0)
                return false;

            _windownameaddress = MemoryHandler.Instance.GetUInt32(_address) + 4;

            _windowName = MemoryHandler.Instance.GetString(_windownameaddress, 16).ToLower();
            if (_windowName != "gathering")
            {
                _validWindow = false;
                return false;
            }

            _address = MemoryHandler.Instance.GetUInt32(_address) + 0xE0;
            _address = MemoryHandler.Instance.GetUInt32(_address);


            return true;
        }

        public bool ValidWindow
        {
            get { return _validWindow; }
        }

        public void Refresh()
        {
            uint currPos = 60;
            const uint itemSize = 48;
            Items.Clear();

            for (byte i = 1; i <= 8; i++ )
            {
                uint itemListAddress = MemoryHandler.Instance.GetUInt32(_address + currPos) + 9;
                byte itemflag = MemoryHandler.Instance.GetByte(_address + currPos + 24, false);
                string itemName = MemoryHandler.Instance.GetString(itemListAddress, 32);

                if (itemflag >= 1 && itemName.Length > 0)
                {
                    Items.Add(new GatheringItems(itemName, i));
                }

                currPos += itemSize;
            }
        }
    }
}
