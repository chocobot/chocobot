using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.CombatAI.Classes
{
    class RangerAI : GenericAI
    {

        public RangerAI() : base()
        {
            IsRanged = true;
            Name = "Ranger";
        }

        public override void Fight(Character user, Character monster, Recast recast)
        {

            base.Fight(user, monster, recast);

            monster.Target();
            recast.Refresh();

            if (recast.Abilities.Count == 0)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D3); // Raging Strikes
            }
            else if (monster.Health_Percent < 20 && recast.Abilities.Contains((int)Recast.eAbilities.MiserysEnd) == false)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D5); // Miserys End
            }
            else if (recast.WeaponSpecials.Count == 0)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1); // Heavy Shot
            }

        }
 
    }
}
