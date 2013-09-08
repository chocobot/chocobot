using System;
using System.Threading;
using Chocobot.Datatypes;
using Chocobot.Utilities.Memory;
using Chocobot.Utilities.MathMod;
using System.Globalization;

namespace Chocobot.MemoryStructures.Gathering
{
    public class Gathering
    {
        private string _name;
        private Coordinate _coordinate;
        private int _id;
        private byte _type;
        private byte _icon;
        private bool _hidden;
        private bool _valid = true;
        private byte _level;
        private byte _status;

        private readonly uint _address;
        //Get the culture property of the thread.
        private static readonly CultureInfo CultureInfo = Thread.CurrentThread.CurrentCulture;
        private static readonly TextInfo Textinfo = CultureInfo.TextInfo;

        #region "Properties"

        public float DistanceFrom(Gathering otherChar)
        {
            //Refresh()
            return Calculations.PointDistance(Coordinate, otherChar.Coordinate);
        }

        public CharacterStatus Status
        {
            get
            {
                try
                {
                    return (CharacterStatus)_status;
                }
                catch (Exception)
                {
                    return CharacterStatus.Unknown;
                }

            }
        }

        public byte Level
        {
            get { return _level; }
        }


        public bool IsHidden
        {
            get { return _hidden; }
            set
            {
                //MemoryHandler.Instance.SetInt32(Address + 284, 0);
            }
        }

        public byte Icon
        {
            get { return _icon; }
        }
        public uint Address
        {
            get { return _address; }
        }
        public bool Valid
        {
            get { return _valid; }
        }
        public int ID
        {
            get { return _id; }
        }

        public Coordinate Coordinate
        {
            get { return _coordinate; }
        }

        public CharacterType Type
        {
            get
            {
                switch (_type)
                {
                    case 1:
                        return CharacterType.PC;

                    case 2:
                        return CharacterType.Monster;

                    case 3:
                        return CharacterType.NPC;

                    case 6:
                        return CharacterType.Gathering;
                }

                return CharacterType.PC;
            }
        }

        public string Name
        {
            get { return _name; }
        }
        #endregion

        public override string ToString()
        {
            return _name;
        }


        public Gathering(uint address, bool forceAddress = false)
        {

            if (forceAddress == false)
                address = MemoryHandler.Instance.GetUInt32(address);

            if (address == 0)
            {
                _valid = false;
                return;
            }
            _address = address;

            Refresh();

        }

        public void Target()
        {
            uint targetAddress = MemoryLocations.Database["target"];

            MemoryHandler.Instance.SetUInt32(targetAddress, _address);
            MemoryHandler.Instance.SetUInt32(targetAddress + 12, _address);
            MemoryHandler.Instance.SetUInt32(targetAddress + 16, _address);
            MemoryHandler.Instance.SetInt32(targetAddress + 72, _id);
        }

        public void Refresh()
        {

            // Fixed
            _id = MemoryHandler.Instance.GetInt32(Address + 116);
            _name = Textinfo.ToTitleCase(MemoryHandler.Instance.GetString(Address + 48, 24));

            // Check if the name doesnt look right.... went to far on npc list?
            if (_name.Trim().Length == 0)
            {
                _valid = false;
                return;
            }

            _coordinate = new Coordinate(
                       MemoryHandler.Instance.GetFloat(Address + 160),
                       MemoryHandler.Instance.GetFloat(Address + 168),
                       MemoryHandler.Instance.GetFloat(Address + 164)
                  );


            _type = MemoryHandler.Instance.GetByte(Address + 138, false);
            _hidden = MemoryHandler.Instance.GetInt32(Address + 284) != 0;

            //_level = MemoryHandler.Instance.GetByte(Address + 5769, false);
            _icon = MemoryHandler.Instance.GetByte(Address + 394, false);
            _status = MemoryHandler.Instance.GetByte(Address + 405, false);
            // Needs Work
        }

    }
}
