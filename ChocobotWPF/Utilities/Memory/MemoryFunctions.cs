using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.Utilities.Memory
{
    class MemoryFunctions
    {

        //public static void SetTarget(Character character)
        //{
        //    uint targetAddress = MemoryLocations.Database["target"];

        //    MemoryHandler.Instance.SetUInt32(targetAddress, character.Address);
        //    MemoryHandler.Instance.SetUInt32(targetAddress + 12, character.Address);
        //    MemoryHandler.Instance.SetUInt32(targetAddress + 16, character.Address);
        //    MemoryHandler.Instance.SetInt32(targetAddress + 72, character.ID);
            
        //}

        public static int GetTarget()
        {
            uint targetAddress = MemoryLocations.Database["target"];
            
            int result = MemoryHandler.Instance.GetInt32(targetAddress);

            //2206790
            if (result == 0)
                result = -1;

            return result;

        }

        public static void GetCharacters(List<Character> Monsters, List<Character> Fate, List<Character> Players, ref Character User)
        {
            uint startAddress = MemoryLocations.Database["charmap"];
            const uint length = 396;

            Players.Clear();
            Monsters.Clear();
            Fate.Clear();

            for (uint i = 0; i <= length; i += 4)
            {
                Character newChar = new Character(startAddress + i);

                if (i == 0)
                    User = newChar;

                if (newChar.Valid)
                {
                    if (newChar.Type == Datatypes.CharacterType.PC)
                        Players.Add(newChar);
                    else
                    {
                        if(newChar.IsFate)
                            Fate.Add(newChar);
                        else
                            Monsters.Add(newChar);
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
    }
}
