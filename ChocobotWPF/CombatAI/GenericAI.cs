using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.CombatAI
{
    public class GenericAI
    {
        public bool IsRanged = false;
        public bool HasCure = false;
        public string Name = "Generic Fighter";

        public GenericAI()
        {
            IsRanged = true;
        }

        public virtual void Fight(Character user, Character monster, Recast recast)
        {
            if (HasCure)
                if (user.Health_Percent < 25)
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D0);
                }
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual void Reset()
        {
            
        }

    }
}
