using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.CombatAI.Classes
{
    class BasicAI : GenericAI
    {
        public BasicAI()
            : base()
        {
            IsRanged = false;
            Name = "Basic Fighter";
        }

        public override void Fight(Character user, Character monster, Recast recast)
        {

            base.Fight(user, monster, recast);

            monster.Target();
            recast.Refresh();

            if (recast.WeaponSpecials.Count == 0)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1);
            }

        }
 
    }
}
