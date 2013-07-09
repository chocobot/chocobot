using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chocobot.MemoryStructures.Abilities
{
    class Ability
    {
        public int ID { get; set; }
        public float Timer { get; set; }
        public string Name { get; set; }

        public Ability(int ID, string Name, float Timer)
        {
            this.ID = ID;
            this.Name = Name;
            this.Timer = Timer;
        }

    }
}
