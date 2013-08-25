using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chocobot.Utilities.Memory;

namespace Chocobot.MemoryStructures.Abilities
{
    class Availability
    {
        public int ID { get; set; }
        public bool Available { get; set; }

        public Availability(int id, bool available)
        {
            ID = id;
            Available = available;
        }

        public Availability(uint address)
        {
            ID = MemoryHandler.Instance.GetInt16(address + 19);
            Available = MemoryHandler.Instance.GetByte(address + 27, false) == 1?true:false; 
        }
    }
}
