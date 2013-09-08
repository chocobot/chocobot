using System;
using System.Collections.Generic;
using System.Windows;

namespace Chocobot.MemoryStructures.Abilities
{
    class Recast
    {
        public List<int> Abilities = new List<int>();
        public List<int> WeaponSpecials = new List<int>();

        private readonly uint _address;
        private readonly uint _address2;


        //public enum eAbilities:int
        //{
        //    MiserysEnd = 16708,
        //    RagingStrikes = 17138,
        //    HawksEye = 17077,
        //    Barrage = 17082,
        //    Bloodletter = 16761,

        //    SecondWind = 17138,
        //    FeatherFoot = 17077,
        //    InternalRelease = 17011,

        //    LegSweep = 16883,
        //    KeenFlurry = 17077,
            
        //}

        public enum eAbilities : int
        {
            MiserysEnd = 4,
            RagingStrikes = 2,
            HawksEye = 1,
            Barrage = 6,
            Bloodletter = 9,

            SecondWind = 2,
            FeatherFoot = 1,
            InternalRelease = 3,

            LegSweep = 3,
            KeenFlurry = 1,

        }


        public Recast()
        {
            _address = Utilities.Memory.MemoryLocations.Database["recast"];
            _address2 = Utilities.Memory.MemoryLocations.Database["recast_ws"];

        }

        public void Refresh()
        {

            uint counter = 0;
            const int maxAbilities = 12;
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

                //MessageBox.Show(i.ToString());
                Abilities.Add(i);

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
