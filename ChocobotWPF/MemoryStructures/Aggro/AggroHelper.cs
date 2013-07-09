using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chocobot.Utilities.Memory;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Aggro;
using Chocobot.MemoryStructures.Character;


namespace Chocobot.MemoryStructures.Aggro
{
    class AggroHelper
    {
        private readonly uint _address = MemoryLocations.Database["aggro"];
        private const uint Structsize = 72; //64

       public List<int> GetAggroList()
        {

           List<int> monstersAggroed = new List<int>();
           uint address = _address;

           byte aggroCount = MemoryHandler.Instance.GetByte(address + 2304, false);

          
           // Removed for phase 3... Value turns to 0 when target unselected...
           //if (MemoryHandler.Instance.GetByte(_address - 8) == 0)
           //    return monstersAggroed;

           for(int i = 0; i < aggroCount; i++)
           {
               monstersAggroed.Add(MemoryHandler.Instance.GetInt32(address + 64));
               address += Structsize;
           }

           return monstersAggroed;
        }

    }
}
