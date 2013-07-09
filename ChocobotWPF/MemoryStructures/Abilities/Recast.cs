using System;
using System.Collections.Generic;

namespace Chocobot.MemoryStructures.Abilities
{
    class Recast
    {
        public List<Ability> Abilities = new List<Ability>();
        public List<Ability> WeaponSpecials = new List<Ability>();

        private readonly uint _address;
        private readonly uint _address2;

        public Recast()
        {
            _address = Utilities.Memory.MemoryLocations.Database["recast"];
            _address2 = Utilities.Memory.MemoryLocations.Database["recast_ws"];
            System.Diagnostics.Debug.Print("Recast2: " + _address2.ToString("X"));
        }

        public void Refresh()
        {

            uint Counter = 0;
            const int MaxAbilities = 3;
            const int MaxWS = 3;
            int i = 0;

            Abilities.Clear();
            WeaponSpecials.Clear();

            while(i < MaxAbilities)
            {
                float recasttimer = Utilities.Memory.MemoryHandler.Instance.GetFloat(_address + Counter);
                int recastid = Utilities.Memory.MemoryHandler.Instance.GetInt32(_address + Counter + 4);

                if (Math.Abs(recasttimer) < 0.00001)
                    break;

                Abilities.Add(new Ability(recastid, "Unknown", recasttimer));

                Counter += 16;
                i++;
            }


            // Get weapon specials
            i = 0;
            Counter = 0;

            while (i < MaxWS)
            {
                float recasttimer = Utilities.Memory.MemoryHandler.Instance.GetFloat(_address2 + Counter);
                int recastid = Utilities.Memory.MemoryHandler.Instance.GetInt32(_address2 + Counter + 4);

                if (Math.Abs(recasttimer) < 0.00001)
                    break;

                WeaponSpecials.Add(new Ability(recastid, "Unknown WS", recasttimer));

                Counter += 16;
                i++;
            }

        }
    }
}
