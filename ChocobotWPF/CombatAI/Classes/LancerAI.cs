using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.CombatAI.Classes
{
    class LancerAI : GenericAI
    {

        private byte _step = 0;

        public LancerAI()
            : base()
        {
            IsRanged = false;
            Name = "Lancer";
        }

        public override void Fight(Character user, Character monster, Recast recast)
        {

            base.Fight(user, monster, recast);

            monster.Target();
            recast.Refresh();

            if (recast.Abilities.Count == 0 && user.Level >= 6)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D4); // Keen Flurry
            } else if (recast.WeaponSpecials.Count == 0)
            {
                if (_step == 0)
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1); // True Thrust
                    _step = 1;
                } else if(user.Level >= 4)
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D3); // Vorpal Thrust
                    _step = 0;
                }
            }

        }
 
    }
}
