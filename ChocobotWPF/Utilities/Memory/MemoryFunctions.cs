using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.Gathering;

namespace Chocobot.Utilities.Memory
{
    class MemoryFunctions
    {

        public static int GetTarget()
        {
            uint targetAddress = MemoryLocations.Database["target"];
            
            int result = MemoryHandler.Instance.GetInt32(targetAddress);

            //2206790
            if (result == 0)
                result = -1;

            return result;

        }

        public static void GetCharacters(List<Character> monsters, List<Character> fate, List<Character> players, ref Character user)
        {
            uint startAddress = MemoryLocations.Database["charmap"];
            const uint length = 396;

            players.Clear();
            monsters.Clear();
            fate.Clear();

            for (uint i = 0; i <= length; i += 4)
            {
                Character newChar = new Character(startAddress + i);

                if (i == 0)
                    user = newChar;

                if (newChar.Valid)
                {
                    if (newChar.Type == Datatypes.CharacterType.PC)
                        players.Add(newChar);
                    else
                    {
                        if(newChar.IsFate)
                            fate.Add(newChar);
                        else
                            monsters.Add(newChar);
                    }

                }

            }


        }

        public static void GetNPCs(List<Character> NPCs)
        {

            try
            {
                uint startAddress = MemoryLocations.Database["npcmap"];
                const uint length = 256;

                NPCs.Clear();

                for (uint i = 0; i <= length; i += 4)
                {
                    Character newChar = new Character(startAddress + i);

                    if (newChar.Valid)
                        NPCs.Add(newChar);
                }

            } catch(Exception ex)
            {
                System.Diagnostics.Debug.Print("Problem getting NPCs");
            }

        }

        public static void GetGathering(List<Gathering> gather)
        {

            try
            {
                uint startAddress = MemoryLocations.Database["gathermap"];
                const uint length = 156;

                gather.Clear();

                for (uint i = 0; i <= length; i += 4)
                {
                    Gathering newChar = new Gathering(startAddress + i);

                    if (newChar.Valid)
                        gather.Add(newChar);
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Problem getting gather");
            }

        }
    }
}
