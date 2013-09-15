using Chocobot.Utilities.Memory;

namespace Chocobot.MemoryStructures.UIWindows.Crafting
{
    class CraftWindow
    {
        private uint _address;
        private uint _windownameaddress;
        private short _currDurability;
        private short _maxDurability;
        private short _currQuality;
        private short _maxQuality;
        private short _currProgress;
        private short _maxProgress;
        private short _currHQ;
        private string _condition;
        private string _windowName;
        private bool _validWindow = true;

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
            if (_windowName != "synthesis")
            {
                _validWindow = false;
                return false;
            }

            _address = MemoryHandler.Instance.GetUInt32(_address) + 0xE0;
            _address = MemoryHandler.Instance.GetUInt32(_address);

            _address += 0x24;

            return true;
        }

        public short CurrDurability
        {
            get { return _currDurability; }
        }

        public short MaxDurability
        {
            get { return _maxDurability; }
        }

        public short CurrQuality
        {
            get { return _currQuality; }
        }

        public short MaxQuality
        {
            get { return _maxQuality; }
        }

        public short CurrProgress
        {
            get { return _currProgress; }
        }

        public short MaxProgress
        {
            get { return _maxProgress; }
        }

        public short CurrHq
        {
            get { return _currHQ; }
        }

        public string Condition
        {
            get { return _condition; }
        }

        public void Refresh()
        {

            uint conditionAddress = MemoryHandler.Instance.GetUInt32(_address + 56);
            _condition = MemoryHandler.Instance.GetString(conditionAddress, 8);

            _currProgress = MemoryHandler.Instance.GetInt16(_address);
            _maxProgress = MemoryHandler.Instance.GetInt16(_address + 8);

            _currDurability = MemoryHandler.Instance.GetInt16(_address + 16);
            _maxDurability = MemoryHandler.Instance.GetInt16(_address + 24);

            _currQuality = MemoryHandler.Instance.GetInt16(_address + 32);
            _maxQuality = MemoryHandler.Instance.GetInt16(_address + 80);

            _currHQ = MemoryHandler.Instance.GetInt16(_address + 40);

        }
    }
}
