using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.CombatAI.Ranger
{
    class RangerAI : GenericAI
    {
        public RangerAI(Character user, Character monster, Recast recast) : base(user, monster, recast)
        {
            IsRanged = true;
            Name = "Ranger";
        }

        public override void Fight()
        {
            Monster.Target();
            Recast.Refresh();

            if (Recast.WeaponSpecials.Count == 0)
            {

                if(Monster.Health_Percent >= 20)
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1);
                } else
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D5);
                }

            }

        }
 
    }
}
