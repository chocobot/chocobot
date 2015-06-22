using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chocobot.Utilities.Memory;

namespace Chocobot.MemoryStructures.UIWindows
{
    class UIWindow
    {
        private long _address;
        private uint _windownameaddress;

        public UIWindow()
        {

        }

        public bool RefreshPointers()
        {
            _address = MemoryLocations.Database["crafting"];

            _address = MemoryHandler.Instance.GetUInt32(_address) + 0x54;
            _address = MemoryHandler.Instance.GetUInt32(_address) + 0x78C;
            _address = MemoryHandler.Instance.GetUInt32(_address) + 0x270;

            if (_address == 0)
                return false;

            _windownameaddress = MemoryHandler.Instance.GetUInt32(_address) + 4;

            _address = MemoryHandler.Instance.GetUInt32(_address) + 0xDC;
            _address = MemoryHandler.Instance.GetUInt32(_address);

            return true;
        }


        public string GetActiveWindowName()
        {
            string windowName = MemoryHandler.Instance.GetString(_windownameaddress, 16).ToLower();

            //Debug.Print(windowName);
            return windowName;
        }

    }
}
