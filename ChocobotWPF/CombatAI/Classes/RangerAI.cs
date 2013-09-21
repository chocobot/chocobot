using System.Threading;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.CombatAI.Classes
{
    class RangerAI : GenericAI
    {
        private int _attackStep = 0;


        public RangerAI() : base()
        {
            IsRanged = true;
            Name = "Ranger";
        }

        public override void Reset()
        {
            _attackStep = 0;
            base.Reset();
        }

        public override void Fight(Character user, Character monster, Recast recast)
        {

            base.Fight(user, monster, recast);

            monster.Target();
            recast.Refresh();


            if (recast.Abilities.Contains((int)Recast.eAbilities.RagingStrikes) == false && user.Level >= 4 && (monster.Health_Percent > 15 || monster.Health_Current > 5000))
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D3); // Raging Strikes
            }
            else if (recast.Abilities.Contains((int)Recast.eAbilities.HawksEye) == false && user.Level >= 26 && (monster.Health_Percent > 15 || monster.Health_Current > 5000))
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D8); // Hawks Eye
            }
            else if (monster.Health_Percent <= 20 && recast.Abilities.Contains((int)Recast.eAbilities.MiserysEnd) == false && user.Level >= 8)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D5); // Miserys End
            }
            else if (recast.Abilities.Contains((int)Recast.eAbilities.Bloodletter) == false && user.Level >= 12)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D7); // Bloodletter
            }
            else if (recast.Abilities.Contains((int)Recast.eAbilities.Barrage) == false && user.Level >= 38)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D9); // Barrage
            }
            else if (recast.WeaponSpecials.Count == 0)
            {

                if (_attackStep == 0 && user.Level >= 2){
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D2); // Straight Shot
                    Thread.Sleep(300);
                } else if (_attackStep == 1 && user.Level >= 6){
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6); // Venomous Bite
                    Thread.Sleep(300);
                } else if (_attackStep == 2 && user.Level >= 30){
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D4); // Windbite

                } else{
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1); // Heavy Shot
                }

                _attackStep++;

                if(_attackStep >= 15)
                    _attackStep = 0;
                

            }

        }
 
    }
}
