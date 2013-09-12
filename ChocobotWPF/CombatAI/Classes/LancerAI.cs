using System.Threading;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.CombatAI.Classes
{
    class LancerAI : GenericAI
    {

        private byte _step = 0;
        private bool _initial;

        public LancerAI()
            : base()
        {
            IsRanged = false;
            _initial = true;
            Name = "Lancer";
        }

        public override void Reset()
        {
            base.Reset();
            _initial = true;
            _step = 0;

        }

        public override void Fight(Character user, Character monster, Recast recast)
        {

            base.Fight(user, monster, recast);

            monster.Target();
            recast.Refresh();

            if (user.Level < 2)
            {
                _step = 1;
                _initial = false;
            }

            if (recast.Abilities.Contains((int)Recast.eAbilities.KeenFlurry) == false && user.Level >= 6 && (monster.Health_Percent > 15 || monster.Health_Current > 5000))
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D4); // Keen Flurry
            }
            else if (recast.Abilities.Contains((int)Recast.eAbilities.LegSweep) == false && user.Level >= 12 && (monster.Health_Percent > 15 || monster.Health_Current > 5000))
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6); // Keen Flurry
            } else if (recast.WeaponSpecials.Count == 0)
            {

                if (_initial)
                {
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D2); // Feint
                        _step = 1;
                        return;
                }

                if (_step == 1)
                {
                    if (user.Level >= 1)
                    {
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1); // True Thrust
                        _step = 2;
                        Thread.Sleep(300);

                        return;
                    }

                    _step = 2;
                }
                
                if(_step == 2)
                {

                    if (user.Level >= 4)
                    {
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D3); // Vorpal Thrust
                        _step = 1;
                        Thread.Sleep(300);

                        return;
                    }

                    _step = 1;
                }
            } else
            {
                _initial = false;
                if(_step == 0)
                    _step = 1;
            }

        }
 
    }
}
