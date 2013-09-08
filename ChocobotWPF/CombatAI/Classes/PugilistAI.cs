using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.CombatAI.Classes
{
    class PugilistAI : GenericAI
    {

        public PugilistAI()
            : base()
        {
            IsRanged = false;
            Name = "Pugilist";
        }

        public override void Fight(Character user, Character monster, Recast recast)
        {

            base.Fight(user, monster, recast);

            monster.Target();
            recast.Refresh();


            if (user.Level >= 8 && user.Health_Percent < 75 && recast.Abilities.Contains((int)Recast.eAbilities.SecondWind) == false)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D5); // Second wind
            }
            else if (monster.Health_Percent > 20 && recast.Abilities.Contains((int)Recast.eAbilities.FeatherFoot) == false && user.Level >= 4)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D3); // Feather Foot
            }
            else if (monster.Health_Percent > 20 && recast.Abilities.Contains((int)Recast.eAbilities.InternalRelease) == false && user.Level >= 12)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D7); // Internal Release
            }
            else if (recast.WeaponSpecials.Count == 0)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1); // Bootshine
            }

        }
 
    }
}
