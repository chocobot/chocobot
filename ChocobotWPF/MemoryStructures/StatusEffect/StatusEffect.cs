using System;

namespace Chocobot.MemoryStructures.StatusEffect
{
    public class StatusEffect
    {
        public Int32 RecievedFrom;
        public Int32 ID;
        public byte Count;

        public StatusEffect(Int32 recievedFrom, Int32 id, byte count)
        {
            RecievedFrom = recievedFrom;
            ID = id;
            Count = count;
        }
    }
}
