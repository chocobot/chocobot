using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
//using Binarysharp.MemoryManagement;
//using Binarysharp.MemoryManagement.Assembly.CallingConvention;
//using Binarysharp.MemoryManagement.Memory;
//using Binarysharp.MemoryManagement.Native;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.Gathering;

namespace Chocobot.Utilities.Memory
{
    public class MemoryFunctions
    {
        public enum ActionType
        {
            Spell,WS,Ability
        }
      //  private static MemorySharp Sharp = new MemorySharp(MemoryHandler.Instance.Process);
       // public static RemoteAllocation CodeCave = Sharp.Memory.Allocate(512);

        public static void ForceHotkey(int slot, int bar)
        {


            long esi, ecx;
            esi = MemoryLocations.Database["forceHotkey"];

            esi = MemoryHandler.Instance.GetUInt32(esi) + 0x20;
            esi = MemoryHandler.Instance.GetUInt32(esi) + 0x148;
            esi = MemoryHandler.Instance.GetUInt32(esi) + 0x414;
            esi = MemoryHandler.Instance.GetUInt32(esi) + 0xC;
            esi = MemoryHandler.Instance.GetUInt32(esi);
            ecx = MemoryHandler.Instance.GetUInt32(esi + 0x233B4);

            // 55 8B EC 8B 45 08 C1 E0 04 03 45 0C 69 C0 ? ? ? ? 8D 54 08 48

            // Initialize Ability: 55 8B EC 83 EC 20 53 56 8B 75 08 0F B6 46 08
            IntPtr address = new IntPtr(0x91870 + MemoryHandler.Instance.BaseAddress);

            //var asm = new[]
            //          {
            //              "mov esi, " + esi, // Probably not needed...

            //              "mov ecx, " + slot, // Parameter 1
            //              "push ecx",
            //              "mov ecx, " + ecx, // Set *this
                          
            //              "push " + bar, // Parameter 2

            //              "call " + address, // Call function
            //              "retn"
            //          };

        //    Sharp.Assembly.Execute(address, CallingConventions.Thiscall, ecx, bar, slot);
   
        }

        public static void HackMaxZoomLevel()
        {
            long esi;
            esi = MemoryLocations.Database["zoomHax"];
            esi = MemoryHandler.Instance.GetUInt32(esi) + 0xF0;

            MemoryHandler.Instance.SetFloat(esi, (float)100.0);
        }


        public static Character GetCharacterFromID(int ID)
        {
            List<Character> players = new List<Character>();
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            Character user = null;

            GetCharacters(monsters, fate, players, ref user);
            monsters.AddRange(players);

            foreach (Character monster in monsters.Where(monster => monster.ID == ID))
            {
                return monster;
            }

            return null;
        }

        public static int GetTarget()
        {
            long targetAddress = MemoryLocations.Database["target"];
            
            int result = MemoryHandler.Instance.GetInt32(targetAddress);

            //2206790
            if (result == 0)
                result = -1;

            return result;

        }

        public static int GetGroundCursor()
        {
            long targetAddress = MemoryLocations.Database["recast"] - 0x140;

            int result = MemoryHandler.Instance.GetInt16(targetAddress);

            return result;

        }

        public static DateTime GetGameTime()
        {
            long startAddress = MemoryLocations.Database["time"];

          //  byte hour = MemoryHandler.Instance.GetByte(startAddress, false);
          //  byte minute = MemoryHandler.Instance.GetByte(startAddress + 4, false);

            return new DateTime(); //(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);

        }

        public static ushort GetMapID()
        {
            return MemoryHandler.Instance.GetUInt16(MemoryLocations.Database["map"]);
        }

        public static void GetCharacters(List<Character> monsters, List<Character> fate, List<Character> players, ref Character user)
        {
            long startAddress = MemoryLocations.Database["charmap"];
            const uint length = 396 * 2;

            players.Clear();
            monsters.Clear();
            fate.Clear();

            for (uint i = 0; i <= length; i += 8)
            {
                Character newChar = new Character(startAddress + i);

                if (i == 0)
                {
                    user = newChar;
                    user.IsUser = true;
                }

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
                long startAddress = MemoryLocations.Database["npcmap"];
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
                long startAddress = MemoryLocations.Database["gathermap"];
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
