using System;
using System.Collections.Generic;
using System.Windows;

namespace Chocobot.MemoryStructures.Abilities
{
    public class Recast
    {
        public List<int> Abilities = new List<int>();
        public List<int> SubAbilities = new List<int>();
        public List<int> WeaponSpecials = new List<int>();

        private readonly long _address;
        private readonly long _address2;

        public enum eAbilities : int
        {
            MiserysEnd = 4,
            RagingStrikes = 2,
            HawksEye = 1,
            Barrage = 6,
            BluntArrow = 8,
            Bloodletter = 9,

            SecondWind = 2,
            FeatherFoot = 1,
            InternalRelease = 3,

            LifeSurge = 4,
            LegSweep = 3,
            Invigorate = 2,
            KeenFlurry = 1,

            ShroudOfSaints = 2,
            Benediction = 6,

            PresenceOfMind = 4,
            DivineSeal = 5,

            Aetherflow = 1
        }


        public enum eSubAbilities : int
        {
            MiserysEnd = 103,
            RagingStrikes = 101,
            Barrage = 107,
            Bloodletter = 110,

            SecondWind = 57,
            FeatherFoot = 55,
            InternalRelease = 59,

            LifeSurge = 83,
            LegSweep = 82,
            Invigorate = 80,
            KeenFlurry = 77,
            BloodForBlood = 85,

            SwiftCast = 150,
            SureCast = 143,
        }

        public Recast()
        {
            _address = Utilities.Memory.MemoryLocations.Database["recast"];
            _address2 = Utilities.Memory.MemoryLocations.Database["recast_ws"];

        }

        public void Refresh(double toleranceWS = 0.0)
        {

            uint counter = 0;
            const int maxAbilities = 12;
            const int maxWS = 3;
            int i = 0;

            Abilities.Clear();
            SubAbilities.Clear();
            WeaponSpecials.Clear();

            while(i < maxAbilities)
            {
                float recasttimer = Utilities.Memory.MemoryHandler.Instance.GetFloat(_address + counter);

                if (Math.Abs(recasttimer) < 0.00001)
                {
                    

                    i++;
                    counter += 20;
                    continue;
                }

                Abilities.Add(i + 1);

                counter += 20;
                i++;
            }


            List<int> AbilityIDs = new List<int>();
            i = 0;
            while (i < 5)
            {
                int id = Utilities.Memory.MemoryHandler.Instance.GetInt32((uint)(_address - 0x30 + (i * 4)));
                AbilityIDs.Add(id);
                i += 1;
            }




            i = 0;
            counter = 0;
            // Get Sub Abilities
            while (i < 5)
            {
                float recasttimer = Utilities.Memory.MemoryHandler.Instance.GetFloat(_address + 0x320 + counter);
                int recastid = AbilityIDs[i];

                if (Math.Abs(recasttimer) < 0.00001)
                {
                    i++;
                    counter += 20;
                    continue;
                }

                SubAbilities.Add(recastid);
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

                if (Math.Abs(recasttimer) < 0.00001 || ((toleranceWS - recasttimer) < 0.00001 && toleranceWS > 0.1))
                    break;

                WeaponSpecials.Add(recastid);

                counter += 16;
                i++;
            }

        }
    }
}
