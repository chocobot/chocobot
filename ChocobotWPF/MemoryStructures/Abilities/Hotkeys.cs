using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Chocobot.Datatypes;
using Chocobot.Utilities.Memory;
using Roslyn.Compilers.VisualBasic;

namespace Chocobot.MemoryStructures.Abilities
{
    public class Hotkeys
    {
        public class Hotkey
        {
            private readonly short _id;
            private short _slot;
            private bool _control;
            private byte _highlighted;
            private byte _inRange;
            private byte _percentReady;
            private bool _isInHotbar = true;
            private bool _activated;
            private DateTime _lastUseTime;
            
            public Hotkey(short id, short slot, bool control, byte highlighted, byte inRange, byte percentReady, bool activated)
            {
                _id = id;
                _slot = slot;
                _control = control;
                _highlighted = highlighted;
                _inRange = inRange;
                _percentReady = percentReady;
                _activated = activated;
                _lastUseTime = new DateTime(1999, 12, 1, 9, 9, 59);
            }

            public TimeSpan TimeSinceLastUse
            {
                get
                {
                    return DateTime.Now - _lastUseTime;
                }
            }
            public DateTime LastUseTime
            {
                get { return _lastUseTime; }
            }
            public short ID
            {
                get { return _id; }
            }

            public short Slot
            {
                get { return _slot; }
                set { _slot = value;  }
            }

            public bool IsInHotbar
            {
                get { return _isInHotbar; }
                set { _isInHotbar = value; }
            }

            public bool Control
            {
                get { return _control; }
                set { _control = value; }
            }

            public bool Highlighted
            {
                get { return _highlighted == 1; }
                set
                {
                    if (value == true)
                        _highlighted = 1;
                    else
                    {
                        _highlighted = 2;
                    }
                }
            }

            public byte InRange
            {
                get { return _inRange; }
                set { _inRange = value; }
            }

            public byte PercentReady
            {
                get
                {
                    if (_percentReady == 0)
                        return 100;

                    return _percentReady;
                }
                set { _percentReady = value; }
            }

            public bool Activated
            {
                get { return _activated; }
                set { _activated = value; }
            }

            private Keys getKey(short slot)
            {
                switch (slot)
                {
                    case 1:
                        return Keys.D1;
                    case 2:
                        return Keys.D2;
                    case 3:
                        return Keys.D3;
                    case 4:
                        return Keys.D4;
                    case 5:
                        return Keys.D5;
                    case 6:
                        return Keys.D6;
                    case 7:
                        return Keys.D7;
                    case 8:
                        return Keys.D8;
                    case 9:
                        return Keys.D9;
                    case 10:
                        return Keys.D0;
                    case 11:
                        return Keys.Dash;
                    case 12:
                        return Keys.VK_OEM_PLUS;

                }

                return Keys.D1;
                
            }

            public void UseAbility()
            {
                if (_id == 0)
                    return;

                if (_control)
                {
                    //MemoryFunctions.ForceHotkey(_slot - 1, 1);
                    Utilities.Keyboard.KeyBoardHelper.Ctrl(getKey(_slot));
                }
                else
                {
                    //MemoryFunctions.ForceHotkey(_slot - 1, 0);
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(getKey(_slot));
                }

                _lastUseTime = DateTime.Now;
            }


        }

        private readonly long _address;
        private const int Structsize = 36;
        public Dictionary<short, Hotkey> Abilities = new Dictionary<short,Hotkey>();

        public Hotkey this[short i]
        {
            get
            {
                if (Abilities.ContainsKey(i))
                {
                    return Abilities[i];
                }
                else
                {
                    Hotkey result = new Hotkey(0, 0, false, 0, 0, 50, false);
                    result.IsInHotbar = false;

                    return result;
                }
            }
        }

        public Hotkeys()
        {
            _address = MemoryLocations.Database["hotkeys"];
            _address = MemoryHandler.Instance.GetUInt32(_address) + 0x20;
            _address = MemoryHandler.Instance.GetUInt32(_address) + 0xC;
            _address = MemoryHandler.Instance.GetUInt32(_address) + 0x18;
            _address = MemoryHandler.Instance.GetUInt32(_address) + 0x20;
            _address = MemoryHandler.Instance.GetUInt32(_address) + 0x20; // Start of Hotkeys

            RefreshAbilities();
        }

        public void RefreshAbilities()
        {
            long address = _address;
            Abilities.Clear();

            for (int i = 0; i < 12; i++)
            {
                short id = MemoryHandler.Instance.GetInt16(address);
                short slot = (short)(i + 1);
                byte highlighted = MemoryHandler.Instance.GetByte(address + 9, false);
                byte inrange = MemoryHandler.Instance.GetByte(address + 20, false);
                byte percentReady = MemoryHandler.Instance.GetByte(address + 8, false);
                byte activated = MemoryHandler.Instance.GetByte(address + 12, false);

                //if (percentReady >= 60)
                //    MemoryHandler.Instance.SetByte(address + 8, 100);

                if (id != 0)
                {
                    Hotkey hk = new Hotkey(id, slot, false, highlighted, inrange, percentReady, activated == 1);
                    try
                    {
                        Abilities.Add(hk.ID, hk);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                address += Structsize;
            }

            // Control Keys
            address = _address + 0x240;
            for (int i = 0; i < 12; i++)
            {
                short id = MemoryHandler.Instance.GetInt16(address);
                short slot = (short)(i + 1);
                byte highlighted = MemoryHandler.Instance.GetByte(address + 9, false);
                byte inrange = MemoryHandler.Instance.GetByte(address + 20, false);
                byte percentReady = MemoryHandler.Instance.GetByte(address + 8, false);
                byte activated = MemoryHandler.Instance.GetByte(address + 12, false);

                Hotkey hk = new Hotkey(id, slot, true, highlighted, inrange, percentReady, activated == 1);

                if (hk.ID != 0)
                {
                    try
                    {
                        Abilities.Add(hk.ID, hk);
                    }
                    catch (Exception ex)
                    {
                       // MessageBox.Show("Problem Adding" + hk.ID.ToString() + "  " + ex.StackTrace.ToString());

                    }
                }
                address += Structsize;
            }

        }



        public void QuickRefreshAbilities()
        {
            long address = _address;

            for (int i = 0; i < 12; i++)
            {
                short id = MemoryHandler.Instance.GetInt16(address);
                short slot = (short)(i + 1);
                byte highlighted = MemoryHandler.Instance.GetByte(address + 9, false);
                byte inrange = MemoryHandler.Instance.GetByte(address + 20, false);
                byte percentReady = MemoryHandler.Instance.GetByte(address + 8, false);
                byte activated = MemoryHandler.Instance.GetByte(address + 12, false);

               // if(percentReady >= 60)
                //    MemoryHandler.Instance.SetByte(address + 8, 0);

                if (id != 0)
                {
                    //Hotkey hk = new Hotkey(id, slot, false, highlighted, inrange, percentReady, activated == 1);
                    try
                    {
                        Abilities[id].Highlighted = highlighted == 1;
                        Abilities[id].InRange = inrange;
                        Abilities[id].Activated = activated == 1;
                        Abilities[id].PercentReady = percentReady;
                        Abilities[id].Slot = slot;
                        Abilities[id].Control = false;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                address += Structsize;
            }

            // Control Keys
            address = _address + 0x240;
            for (int i = 0; i < 12; i++)
            {
                short id = MemoryHandler.Instance.GetInt16(address);
                short slot = (short)(i + 1);
                byte highlighted = MemoryHandler.Instance.GetByte(address + 9, false);
                byte inrange = MemoryHandler.Instance.GetByte(address + 20, false);
                byte percentReady = MemoryHandler.Instance.GetByte(address + 8, false);
                byte activated = MemoryHandler.Instance.GetByte(address + 12, false);

                //Hotkey hk = new Hotkey(id, slot, true, highlighted, inrange, percentReady, activated == 1);

                if (id != 0)
                {
                    try
                    {
                        Abilities[id].Highlighted = highlighted == 1;
                        Abilities[id].InRange = inrange;
                        Abilities[id].Activated = activated == 1;
                        Abilities[id].PercentReady = percentReady;
                        Abilities[id].Slot = slot;
                        Abilities[id].Control = true;
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show("Problem Adding" + hk.ID.ToString());

                    }
                }
                address += Structsize;
            }

        }
    }
}
