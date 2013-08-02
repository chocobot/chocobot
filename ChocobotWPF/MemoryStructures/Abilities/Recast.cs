using System;
using System.Collections.Generic;

namespace Chocobot.MemoryStructures.Abilities
{
    class Recast
    {
        public List<int> Abilities = new List<int>();
        public List<int> WeaponSpecials = new List<int>();

        private readonly uint _address;
        private readonly uint _address2;

        public enum eAbilities:int
        {
            MiserysEnd = 16704,
            RagingStrikes = 17204
        }



        public Recast()
        {
            _address = Utilities.Memory.MemoryLocations.Database["recast"];
            _address2 = Utilities.Memory.MemoryLocations.Database["recast_ws"];

        }

        public void Refresh()
        {

            uint counter = 0;
            const int maxAbilities = 6;
            const int maxWS = 3;
            int i = 0;

            Abilities.Clear();
            WeaponSpecials.Clear();

            while(i < maxAbilities)
            {
                float recasttimer = Utilities.Memory.MemoryHandler.Instance.GetFloat(_address + counter);
                int recastid = Utilities.Memory.MemoryHandler.Instance.GetInt32(_address + counter + 6);

                if (Math.Abs(recasttimer) < 0.00001)
                {
                    i++;
                    counter += 20;
                    continue;
                }

                Abilities.Add(recastid);

                counter += 20;
                i++;
            }

            

            // Get weapon specials
            i = 0;
            counter = 0;

            while (i < maxWS)
            {
                float recasttimer = Utilities.Memory.MemoryHandler.Instance.GetFloat(_address2 + counter);
                int recastid = Utilities.Memory.MemoryHandler.Instance.GetInt32(_address2 + counter + 6);

                if (Math.Abs(recasttimer) < 0.00001)
                    break;

                WeaponSpecials.Add(recastid);

                counter += 16;
                i++;
            }

        }
    }
}
