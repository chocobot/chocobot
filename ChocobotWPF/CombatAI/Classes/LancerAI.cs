using System.Collections.Generic;
using System.Threading;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.CombatAI.Classes
{
    class LancerAI : GenericAI
    {


        public LancerAI()
            : base()
        {
            IsRanged = false;
            Name = "Lancer";
        }

        public override void Reset()
        {
            base.Reset();

        }

        public override void Fight(Character user, Character monster, Recast recast)
        {

            base.Fight(user, monster, recast);

            //byte distance = monster.Distance;
            monster.Target();
            recast.Refresh(2.0);
            user.Refresh();

            ePosition position = monster.Position(user);

            Hotkeys hotkeys = new Hotkeys();
            Hotkeys.Hotkey highlightedAbility = null;

            hotkeys.RefreshAbilities();

            if (hotkeys.Abilities[92].InRange == 0)
                return;

            foreach (KeyValuePair<short, Hotkeys.Hotkey> hk in hotkeys.Abilities)
            {
                if (hk.Value.Highlighted && hk.Value.ID != 89)
                {
                    highlightedAbility = hk.Value;
                }
            }


            if (recast.Abilities.Contains(2) == false && user.TP_Current < 450)
            {
                hotkeys.Abilities[80].UseAbility(); // Invigorate
            } else if (recast.WeaponSpecials.Count == 0 && hotkeys.Abilities[75].InRange == 1)
            {

                //if (distance > 7)
                //{
                //    hotkeys.Abilities[90].UseAbility(); // Piercing Talon
                //} else 
                if (highlightedAbility != null)
                {
                    if (highlightedAbility.ID == 84 && recast.Abilities.Contains(4) == false)
                    {
                        hotkeys.Abilities[83].UseAbility(); // Life Surge for Full Thrust
                    }
                    else
                    {
                        highlightedAbility.UseAbility();
                    }

                }
                else if (user.ContainsStatusEffect(115, 0, false) == false && position == ePosition.Side)
                    //Heavy Thrust Dmg Buff From Side
                {
                    hotkeys.Abilities[79].UseAbility();
                }
                else if (monster.ContainsStatusEffect(119, user.ID, true) == false) //Phlebotomize DOT
                {
                    hotkeys.Abilities[91].UseAbility();
                }
                else if (position == ePosition.Back && monster.ContainsStatusEffect(121, user.ID, true) == false)
                {
                    hotkeys.Abilities[81].UseAbility(); // Start Impulse Drive Combo
                }
                else
                {
                    hotkeys.Abilities[75].UseAbility(); // Start True Thrust Combo
                }

            }
            else
            {

                if ((recast.Abilities.Contains(5) == false || recast.SubAbilities.Contains(59) == false) && hotkeys.Abilities[75].InRange == 1)
                    // Blood for Blood or Internal Release
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D0);
                }

                if (monster.Name.Contains("Titan") == false)
                {
                    if (recast.Abilities.Contains(7) == false && recast.Abilities.Contains(10) == false)
                    {
                        hotkeys.Abilities[93].UseAbility(); // Power Surge
                    }
                    else if (recast.Abilities.Contains(10) == false)
                    {
                        hotkeys.Abilities[96].UseAbility(); // Dragonfire Dive
                    }
                    else if (recast.Abilities.Contains(6) == false)
                    {
                        hotkeys.Abilities[92].UseAbility(); // Jump
                    }
                    else if (recast.Abilities.Contains(9) == false)
                    {
                        hotkeys.Abilities[95].UseAbility(); // Spineshatter drive
                    }
                }
            }
  



        }
 
    }
}
