using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private short _usingAbilityID;
        private bool _usingAbility;
        private short _currentGp;
        private short _maxgp;
        private int _craftbool;
        private uint _statusEffectsAddress;
        private byte _job;
        private int _owner;
        private byte _distance;

        private readonly long _address;
        //Get the culture property of the thread.
        private static readonly CultureInfo CultureInfo = Thread.CurrentThread.CurrentCulture;
        private static readonly TextInfo Textinfo = CultureInfo.TextInfo;

        
        #region "Properties"

        public int Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

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

        public byte Distance
        {
            get { return _distance; }
        }
        public byte Level
        {
            get { return _level;  }
        }
        public bool IsClaimed
        {
            get { return _claimed != 0; }
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
                    if (_maxhealth > 0 && _currenthealth > 0)
                        return Convert.ToByte(Math.Ceiling((Convert.ToDouble(_currenthealth) / Convert.ToDouble(_maxhealth)) * 100.0));
                    
                    return 0;
                    
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
        public long Address
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

        public ePosition Position(Character person)
        {
            double angle = Coordinate.AngleTo(person.Coordinate) - Heading;

            angle = (angle * 57.2957795);
            if(angle < 0)
                angle += 360;

           // Debug.Print(angle.ToString());

            if(angle >= 125 && angle <= 230)
                return ePosition.Back;
            if ((angle < 125 && angle >= 60) || (angle > 230 && angle < 300))
                return ePosition.Side;
            

            return ePosition.Front;

        
            
        }


        public Coordinate Coordinate
        {
            get { return _coordinate; }
            set
            {

                //MemoryHandler.Instance.SetFloat(Address + 160, value.X);
                //MemoryHandler.Instance.SetFloat(Address + 164, value.Z);
                //MemoryHandler.Instance.SetFloat(Address + 168, value.Y);
                //UInt32 addy = MemoryHandler.Instance.GetUInt32(Address + 0xEC);

                //MemoryHandler.Instance.SetFloat(addy + 0x30, value.X);
                //MemoryHandler.Instance.SetFloat(addy + 0x34, value.Z);
                //MemoryHandler.Instance.SetFloat(addy + 0x38, value.Y);

            }
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


        public List<Character> MonstersNear(double distance)
        {
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();
            Character user = null;

            MemoryFunctions.GetCharacters(monsters, fate, players, ref user);

            return monsters.Where(monster => DistanceFrom(monster) < distance && monster.Health_Current > 0 && monster.ID != ID && monster.IsHidden == false && monster.Owner == -536870912).Distinct().ToList();
        }


        public bool ContainsStatusEffect(Int32 abilityID, Int32 casterID, bool checkCaster, float time)
        {

            for (uint i = 0; i < 24; i++)
            {
                Int32 id = MemoryHandler.Instance.GetInt16(Address + _statusEffectsAddress + (i * 12));
                float remaining = MemoryHandler.Instance.GetFloat(Address + _statusEffectsAddress + (i * 12) + 4);
                if (id == 0 || id != abilityID)
                    continue;

                if (time > remaining)
                    return false;

                Int32 casterId = MemoryHandler.Instance.GetInt32(Address + _statusEffectsAddress + (i * 12) + 8);

                if (checkCaster && casterId == casterID)
                {
                    return true;
                }

                if (checkCaster == false)
                {
                    return true;
                }
            }

            return false;

        }


        public bool ContainsStatusEffect2(Int32 abilityID, float time)
        {

            for (uint i = 0; i < 24; i++)
            {
                Int32 id = MemoryHandler.Instance.GetInt16(Address + _statusEffectsAddress + (i * 12));
                float remaining = MemoryHandler.Instance.GetFloat(Address + _statusEffectsAddress + (i * 12) + 4);
                if (id == 0 || id != abilityID)
                    continue;

                if (time >= remaining)
                {
                    return true;
                }
                    
                return false;
            }

            return false;

        }

        public override bool Equals(object obj)
        {

            Character c = obj as Character;
            if (c == null)
                return false;

            return c.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public bool ContainsStatusEffect(Int32 abilityID, Int32 casterID, bool checkCaster)
        {

            for (uint i = 0; i < 24; i++)
            {
                Int32 id = MemoryHandler.Instance.GetInt16(Address + _statusEffectsAddress + (i * 12));

                if (id == 0 || id != abilityID)
                    continue;

                Int32 casterId = MemoryHandler.Instance.GetInt32(Address + _statusEffectsAddress + (i * 12) + 8);

                if (checkCaster && casterId == casterID)
                {
                    return true;
                }
               
                if(checkCaster == false)
                {
                    return true;
                }
            }

            return false;

        }

        public bool ContainsStatusEffect(Int32 abilityID, Int32 casterID, bool checkCaster, byte countLimit)
        {

            for (uint i = 0; i < 24; i++)
            {
                Int32 id = MemoryHandler.Instance.GetInt16(Address + _statusEffectsAddress + (i * 12));

                if (id == 0 || id != abilityID)
                    continue;

                Int32 casterId = MemoryHandler.Instance.GetInt32(Address + _statusEffectsAddress + (i * 12) + 8);
                Byte count = MemoryHandler.Instance.GetByte(Address + _statusEffectsAddress + (i * 12) + 2, false);

                if (countLimit >= count)
                {
                    return false;
                }

                if (checkCaster && casterId == casterID)
                {
                    return true;
                }

                if (checkCaster == false)
                {
                    return true;
                }
            }

            return false;

        }


        public byte BuffCount(Int32 abilityID, Int32 casterID, bool checkCaster)
        {

            for (uint i = 0; i < 24; i++)
            {
                Int32 id = MemoryHandler.Instance.GetInt16(Address + _statusEffectsAddress + (i * 12));

                if (id == 0 || id != abilityID)
                    continue;

                Int32 casterId = MemoryHandler.Instance.GetInt32(Address + _statusEffectsAddress + (i * 12) + 8);
                Byte count = MemoryHandler.Instance.GetByte(Address + _statusEffectsAddress + (i * 12) + 2, false);

                return count;

            }

            return 0;

        }

        public List<StatusEffect.StatusEffect> StatusEffects()
        {
            List<StatusEffect.StatusEffect> statuseffects = new List<StatusEffect.StatusEffect>();

            for (uint i = 0; i < 24; i++ )
            {
                Int32 id = MemoryHandler.Instance.GetInt16(Address + _statusEffectsAddress + (i * 12));

                if (id == 0)
                    continue;

                Int32 casterId = MemoryHandler.Instance.GetInt32(Address + _statusEffectsAddress + (i * 12) + 8);
                Byte count = MemoryHandler.Instance.GetByte(Address + _statusEffectsAddress + (i*12) + 2, false);

                statuseffects.Add(new StatusEffect.StatusEffect(casterId, id, count));
            }



            return statuseffects;

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

        public short UsingAbilityID
        {
            get { return _usingAbilityID; }
        }

        public bool UsingAbility
        {
            get { return _usingAbility; }
        }

        public JOB Job
        {
            get { return (JOB)_job; }
        }
        public int JobPriority
        {
            get
            {
                if (Job == JOB.WHM || Job == JOB.SCH)
                {
                    return 1;

                }
                if (Job == JOB.PLD || Job == JOB.WAR)
                {
                    return 2;
                }
                
                return 3;
            }

        }

        #endregion

        public override string ToString()
        {
            return Job + " " + Health_Percent + "% " + _name;
        }


        public Character(long address, bool forceAddress = false, bool isUser = false)
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
            long targetAddress = MemoryLocations.Database["target"];

            //MemoryHandler.Instance.SetUInt32(targetAddress, _address);
            //MemoryHandler.Instance.SetUInt32(targetAddress + 12, _address);
            //MemoryHandler.Instance.SetUInt32(targetAddress + 16, _address);
            //MemoryHandler.Instance.SetInt32(targetAddress + 104, _id);
        }

        public void Refresh()
        {

            // Fixed
            _id = MemoryHandler.Instance.GetInt32(Address + 0x74);
            _name = Textinfo.ToTitleCase(MemoryHandler.Instance.GetString(Address + 0x30, 32));
    
            // Check if the name doesnt look right.... went to far on npc list?
            if(_name.Trim().Length == 0)
            {
                _valid = false;
                return;
            }

            _coordinate = new Coordinate(
                       MemoryHandler.Instance.GetFloat(Address + 0xB0),
                       MemoryHandler.Instance.GetFloat(Address + 0xB8),
                       MemoryHandler.Instance.GetFloat(Address + 0xB4)
                  );

            _heading = MemoryHandler.Instance.GetFloat(Address + 0xC0);

            _type = MemoryHandler.Instance.GetByte(Address + 0x8A, false);//Check

            _owner = MemoryHandler.Instance.GetInt32(Address + 0x84);//Check
            _distance = MemoryHandler.Instance.GetByte(Address + 0x8D, false); //Check
    

            _currenthealth = MemoryHandler.Instance.GetInt32(Address + 0x16f8);
            _maxhealth = MemoryHandler.Instance.GetInt32(Address + 0x16fc);
            if (_maxhealth == 0)
                _maxhealth = 1;

            _currentmp = MemoryHandler.Instance.GetInt32(Address + 0x1700);
            _maxmp = MemoryHandler.Instance.GetInt32(Address + 0x1704);
            _currenttp = MemoryHandler.Instance.GetInt16(Address + 0x1708);
            _maxtp = 1000;

            _currentcp = 0;//MemoryHandler.Instance.GetInt16(Address + 0x17FE);
            _maxcp = 0;//MemoryHandler.Instance.GetInt16(Address + 0x1800);
            _craftbool = 0;//MemoryHandler.Instance.GetInt32(Address + 0x17A8); 

            _currentGp = 0;//MemoryHandler.Instance.GetInt16(Address + 0x17FA);
            _maxgp = 0;// MemoryHandler.Instance.GetInt16(Address + 0x17FC);
            _level = MemoryHandler.Instance.GetByte(Address + 0x16f1, false);
            _job = MemoryHandler.Instance.GetByte(Address + 0x16f0, false);



            _claimed = MemoryHandler.Instance.GetByte(Address + 0x1C4, false);//Check
            _fate = MemoryHandler.Instance.GetUInt32(Address + 0xE4); //Check


            _icon = MemoryHandler.Instance.GetByte(Address + 0x1B0, false);
            _status = MemoryHandler.Instance.GetByte(Address + 0x1B2, false);
            _hidden = MemoryHandler.Instance.GetInt32(Address + 0x130) != 0; 

            _isMoving = MemoryHandler.Instance.GetByte(Address + 0x3E8, false) == 1;



            _usingAbilityID = MemoryHandler.Instance.GetInt16(Address + 0x38C4);
            _usingAbility = MemoryHandler.Instance.GetByte(Address + 0x618, false) == 1;

            _statusEffectsAddress = 0x3740;

            // Needs Work

            if (_isUser == false)
            {
                _target = MemoryHandler.Instance.GetInt32(Address + 0x1CC);
            }
            else
            {
                int targAddress = MemoryFunctions.GetTarget();
                if (targAddress == -1)
                {
                    _target = -1;
                }
                else
                {
                    _target = new Character((uint)targAddress, true).ID;
                }
            }
            
            if (_target == -536870912)
                _target = -1;

            //_coordinate = new Coordinate(
            //           MemoryHandler.Instance.GetFloat(Address + 0xA0),
            //           MemoryHandler.Instance.GetFloat(Address + 0xA8),
            //           MemoryHandler.Instance.GetFloat(Address + 0xA4)
            //      );

            //_heading = MemoryHandler.Instance.GetFloat(Address + 0xB0);

            //_type = MemoryHandler.Instance.GetByte(Address + 0x8A, false);

            //_currenthealth = MemoryHandler.Instance.GetInt32(Address + 0x17E8);
            //_maxhealth = MemoryHandler.Instance.GetInt32(Address + 0x17EC); 
            //if (_maxhealth == 0)
            //    _maxhealth = 1;

            //_currentmp = MemoryHandler.Instance.GetInt32(Address + 0x17F0);
            //_maxmp = MemoryHandler.Instance.GetInt32(Address + 0x17F4);
            //_currenttp = MemoryHandler.Instance.GetInt16(Address + 0x17F8); 
            //_maxtp = 1000;

            //_currentcp = MemoryHandler.Instance.GetInt16(Address + 0x17FE);
            //_maxcp = MemoryHandler.Instance.GetInt16(Address + 0x1800);
            //_craftbool = MemoryHandler.Instance.GetInt32(Address + 0x17A8); //?

            //_currentGp = MemoryHandler.Instance.GetInt16(Address + 0x17FA);
            //_maxgp = MemoryHandler.Instance.GetInt16(Address + 0x17FC);
            //_level = MemoryHandler.Instance.GetByte(Address + 0x17E1, false);
            //_job = MemoryHandler.Instance.GetByte(Address + 0x17E0, false);
            //_icon = MemoryHandler.Instance.GetByte(Address + 0x18C, false);
            //_status = MemoryHandler.Instance.GetByte(Address + 0x18E, false);
            //_hidden = MemoryHandler.Instance.GetInt32(Address + 0x10C) != 0; //?

            //_isMoving = MemoryHandler.Instance.GetByte(Address + 0x224, false) == 1;
            //_owner = MemoryHandler.Instance.GetInt32(Address + 0x84);

            //_distance = MemoryHandler.Instance.GetByte(Address + 0x8D, false);
            ////UInt32 abilityAddress = MemoryHandler.Instance.GetUInt32(Address + 2300);

            //_usingAbilityID = MemoryHandler.Instance.GetInt16(Address + 0x3334);
            //_usingAbility = MemoryHandler.Instance.GetByte(Address + 0x7D4, false) == 1;
            //_statusEffectsAddress = 0x3168;

            //// Needs Work

            //if (_isUser == false)
            //{
            //    _target = MemoryHandler.Instance.GetInt32(Address + 0x1A8);
            //} else
            //{
            //    int targAddress = MemoryFunctions.GetTarget();
            //    if(targAddress == -1)
            //    {
            //        _target = -1;
            //    } else
            //    {
            //        _target = new Character((uint)targAddress, true).ID;
            //    }
            //}

            //_claimed = MemoryHandler.Instance.GetByte(Address + 0x1A0, false);

            //_fate = MemoryHandler.Instance.GetUInt32(Address + 0xE4);

            //if (_target == -536870912)
            //    _target = -1;
        }

    }
}
