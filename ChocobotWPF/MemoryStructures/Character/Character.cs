using System;
using System.Threading;
using Chocobot.Datatypes;
using Chocobot.Utilities.Memory;
using Chocobot.Utilities.MathMod;
using System.Globalization;

namespace Chocobot.MemoryStructures.Character
{
    public class Character
    {
        private string _name;
        private Coordinate _coordinate;
        private float _heading;
        private int _id;
        private byte _type;
        private byte _claimed;
        private byte _icon;
        private int _currenthealth;
        private int _maxhealth;
        private int _currentmp;
        private int _maxmp;
        private int _currenttp;
        private int _target;
        private int _maxtp;
        private bool _valid = true;
        private uint _fate;
        private byte _level;
        private byte _status;
        private bool _hidden;
        private bool _isUser = false;
        private bool _isMoving;
        private short _currentcp;
        private short _maxcp;
        private int _usingAbilityID;
        private bool _usingAbility;
        private short _currentGp;
        private short _maxgp;
        private int _craftbool;

        private readonly uint _address;
        //Get the culture property of the thread.
        private static readonly CultureInfo CultureInfo = Thread.CurrentThread.CurrentCulture;
        private static readonly TextInfo Textinfo = CultureInfo.TextInfo;

        
        #region "Properties"

        public bool IsUser
        {
            get { return _isUser; }
            set { 
                _isUser = value;
                int targAddress = MemoryFunctions.GetTarget();
                if (targAddress == -1)
                {
                    _target = -1;
                }
                else
                {
                    Character tmptarget = new Character((uint) targAddress, true);
                    _target = tmptarget.ID;
                }
            
            }
        }
        public bool IsFate
        {
            get
            {
                return _fate == 0x801AFFFF && Type == CharacterType.Monster;
            }
        }

        public float DistanceFrom(Character otherChar)
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
                } catch(Exception)
                {
                    return CharacterStatus.Unknown;
                }
                
            }
        }

        public byte Level
        {
            get { return _level;  }
        }
        public bool IsClaimed
        {
            get { return _claimed == 1; }
        }

        public bool IsHidden
        {
            get { return _hidden; }
        }

        public byte Health_Percent
        {
            get
            {
                try
                {
                    return Convert.ToByte(Math.Ceiling((Convert.ToDouble(_currenthealth) / Convert.ToDouble(_maxhealth)) * 100.0));
                } catch
                {
                    return 0;
                }
                    
            }
        }

        public byte Mana_Percent
        {
            get
            {
                try
                {
                    return Convert.ToByte(Math.Ceiling((Convert.ToDouble(_currentmp) / Convert.ToDouble(_maxmp)) * 100.0));
                }
                catch
                {
                    return 0;
                }

            }
        }

        public int Health_Current
        {
            get { return _currenthealth; }
        }

        public int Health_Max
        {
            get { return _maxhealth; }
        }

        public int Mana_Current
        {
            get { return _currentmp; }
        }

        public int Mana_Max
        {
            get { return _maxmp; }
        }

        public int TP_Current
        {
            get { return _currenttp; }
        }

        public int TP_Max
        {
            get { return _maxtp; }
        }

        public int TargetID
        {
            get { return _target; }
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
        public float Heading
        {
            get { return _heading; }
            set { 
                
                // Server side change
                MemoryHandler.Instance.SetFloat(Address + 176, value);
                _heading = value;

                // Client side change
                UInt32 addy = MemoryHandler.Instance.GetUInt32(Address + 0xEC);
                Coordinate headingvector = new Coordinate(1, 0, 0);
                headingvector = headingvector.Rotate2d(_heading / 2);

                MemoryHandler.Instance.SetFloat(addy + 0x4C, headingvector.X);
                MemoryHandler.Instance.SetFloat(addy + 0x44, headingvector.Y);

            }
        }

        public Coordinate Coordinate
        {
            get { return _coordinate; }
            //set
            //{

            //    MemoryHandler.Instance.SetFloat(Address + 160, value.X);
            //    MemoryHandler.Instance.SetFloat(Address + 164, value.Z);
            //    MemoryHandler.Instance.SetFloat(Address + 168, value.Y);
            //    UInt32 addy = MemoryHandler.Instance.GetUInt32(Address + 0xEC);

            //    //const int CLI_SIDE_X = 0x30;
            //    //const int CLI_SIDE_Z = 0x34;
            //    //const int CLI_SIDE_Y = 0x38;

            //    MemoryHandler.Instance.SetFloat(addy + 0x30, value.X);
            //    MemoryHandler.Instance.SetFloat(addy + 0x34, value.Z);
            //    MemoryHandler.Instance.SetFloat(addy + 0x38, value.Y);

            //}
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

        public short CurrentCP
        {
            get { return _currentcp; }
        }

        public short MaxCP
        {
            get { return _maxcp; }
        }

        public bool IsCrafting
        {
            get { return _craftbool != 0; }
        }

        public short CurrentGP
        {
            get { return _currentGp; }
        }

        public short MaxGP
        {
            get { return _maxgp; }
        }

        public bool IsMoving
        {
            get { return _isMoving; }
        }

        public int UsingAbilityID
        {
            get { return _usingAbilityID; }
        }

        public bool UsingAbility
        {
            get { return _usingAbility; }
        }

        #endregion

        public override string ToString()
        {
            return Health_Percent + "% " + _name;
        }


        public Character(uint address, bool forceAddress = false, bool isUser = false)
        {

            if (forceAddress == false)
                address = MemoryHandler.Instance.GetUInt32(address);

            if (address == 0)
            {
                _valid = false;
                return;
            }
            _address = address;
            _isUser = isUser;

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
            if(_name.Trim().Length == 0)
            {
                _valid = false;
                return;
            }

            _coordinate = new Coordinate(
                       MemoryHandler.Instance.GetFloat(Address + 160), 
                       MemoryHandler.Instance.GetFloat(Address + 168),
                       MemoryHandler.Instance.GetFloat(Address + 164)
                  );

            _heading = MemoryHandler.Instance.GetFloat(Address + 176);
            //UInt32 addy = MemoryHandler.Instance.GetUInt32(Address + 0xEC);
            //float vec1 = MemoryHandler.Instance.GetFloat(addy + 0x4C);
            //float vec2 = MemoryHandler.Instance.GetFloat(addy + 0x44);

            //Coordinate test = new Coordinate(1, 0, 0);
            //test = test.Rotate2d(_heading/2);
            //System.Diagnostics.Debug.Print(test.X.ToString() +  " " + test.Y.ToString());
            //System.Diagnostics.Debug.Print(vec1.ToString() + "    " + vec2.ToString());
            ////System.Diagnostics.Debug.Print((test.Angle() * 57.2957795 * 2).ToString() + " == " + (_heading * 57.2957795).ToString());





            _type = MemoryHandler.Instance.GetByte(Address + 138,false);
            _currenthealth = MemoryHandler.Instance.GetInt32(Address + 5776);
            _maxhealth = MemoryHandler.Instance.GetInt32(Address + 5780); 
            if (_maxhealth == 0)
                _maxhealth = 1;

            _currentmp = MemoryHandler.Instance.GetInt32(Address + 5784); 
            _maxmp = MemoryHandler.Instance.GetInt32(Address + 5788); 
            _currenttp = MemoryHandler.Instance.GetInt32(Address + 5792); 
            _maxtp = 1000;

            _currentcp = MemoryHandler.Instance.GetInt16(Address + 5798);
            _maxcp = MemoryHandler.Instance.GetInt16(Address + 5800);
            _craftbool = MemoryHandler.Instance.GetInt32(Address + 5712);

            _currentGp = MemoryHandler.Instance.GetInt16(Address + 5794);
            _maxgp = MemoryHandler.Instance.GetInt16(Address + 5796);

            _level = MemoryHandler.Instance.GetByte(Address + 5769, false);
            _icon = MemoryHandler.Instance.GetByte(Address + 394, false);
            _status = MemoryHandler.Instance.GetByte(Address + 405, false);
            _hidden = MemoryHandler.Instance.GetInt32(Address + 284) != 0;
            _isMoving = MemoryHandler.Instance.GetByte(Address + 532, false) == 1;
            _usingAbilityID = MemoryHandler.Instance.GetInt32(Address + 424);  //2300
            _usingAbility = MemoryHandler.Instance.GetByte(Address + 1956, false) == 1;
            // Needs Work

            if (_isUser == false)
            {
                _target = MemoryHandler.Instance.GetInt32(Address + 416);
            } else
            {
                int targAddress = MemoryFunctions.GetTarget();
                if(targAddress == -1)
                {
                    _target = -1;
                } else
                {
                    _target = new Character((uint)targAddress, true).ID;
                }
            }

            _claimed = MemoryHandler.Instance.GetByte(Address + 405, false);

            _fate = MemoryHandler.Instance.GetUInt32(Address + 228);

            if (_target == -536870912)
                _target = -1;
        }

    }
}
