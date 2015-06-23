using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Documents;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.UIWindows;
using Chocobot.Utilities.Memory;
using System.Collections.Generic;
using Chocobot.Utilities.Misc;

namespace Chocobot.CombatAI.Classes
{
    internal class RangerAI : GenericAI
    {

        private const short HEAVYSHOT = 97;
        private const short STRAIGHTSHOT = 98;
        private const short BLOODLETTER = 110;
        private const short WINDBITE = 113;
        private const short VENOMOUSBITE = 100;
        private const short MISERYSEND = 103;
        private const short RAGINGSTRIKES = 101;
        private const short BLUNTARROW = 109;
        private const short BARRAGE = 107;
        private const short INTERNALRELEASE = 59;
        private const short HAWKSEYE = 99;
        private const short BLOODFORBLOOD = 85;
        private const short INVIGORATE = 80;
        private const short HAYMAKER = 58;

        private List<string> _ignoreDOTMobs = new List<string>();

        public RangerAI() : base()
        {
            IsRanged = true;
            Name = "Ranger";

            _ignoreDOTMobs.Add("granite gaol");
            _ignoreDOTMobs.Add("infernal nail");
            _ignoreDOTMobs.Add("razor plume");
            _ignoreDOTMobs.Add("conflagration");
            _ignoreDOTMobs.Add("renaud");
        }

        public override void Reset()
        {
            base.Reset();
        }


        public override void Fight(Character user, Character monster, Recast recast)
        {

            base.Fight(user, monster, recast);
            bool ignoreDOT = false;
            Hotkeys hotkeys = new Hotkeys();
            hotkeys.RefreshAbilities();

            if (hotkeys[STRAIGHTSHOT].InRange == 0)
                return;

            //UIWindow ui = new UIWindow();
            //ui.RefreshPointers();

            //  if (ui.GetActiveWindowName() == "chatlog")
            //      return;

            monster.Target();
            recast.Refresh(2.1);
            user.Refresh();

            if (user.ContainsStatusEffect2(451, (float) 3.5))
            {
                Debug.Print("Cursed Voice");
                SoundModule.PlayAsyncSound("cursedVoice.wav");

                while (user.ContainsStatusEffect(451, 0, false))
                {
                    Thread.Sleep(100);
                }

                Thread.Sleep(200);
                user.Refresh();
            }

            if (monster.Name.ToLower().Contains("renaud"))
                return;


            // Stop for turn 6 ability
            if (monster.UsingAbilityID == 1949)
            {
                //Utilities.Keyboard.KeyBoardHelper.Ctrl(Keys.D6);
                return;
            }

            List<Character> nearMobs = monster.MonstersNear(100.0);

            // Blight Turn 6
            if (nearMobs.Any(mob => mob.UsingAbilityID == 1949))
            {
                //Utilities.Keyboard.KeyBoardHelper.Ctrl(Keys.D6);
                return;
            }



            // Turn 7 Petrification
            if (nearMobs.Any(mob => mob.UsingAbilityID == 1979))
            {
                SoundModule.PlayAsyncSound("petrification.wav");
                //Utilities.Keyboard.KeyBoardHelper.Ctrl(Keys.D6);
                return;
            }
            if (nearMobs.Any(mob => mob.UsingAbilityID == 1969))
            {
                SoundModule.PlayAsyncSound("petrification.wav");
                //Utilities.Keyboard.KeyBoardHelper.Ctrl(Keys.D6);
                return;
            }

            if (user.ContainsStatusEffect(452, 0, false))
            {
                SoundModule.PlayAsyncSound("Shriek.wav");
            }

            //if (nearMobs.Any(mob => mob.UsingAbilityID == 1967))
            //{
            //    if (user.ContainsStatusEffect(452, 0, false))
            //    {
            //        SoundModule.PlayAsyncSound("Shriek.wav");
            //    }
            //    else
            //    {
            //        SoundModule.PlayAsyncSound("Shriek2.wav");
            //    }
            //}

            if (MemoryFunctions.GetGroundCursor() > 0)
                return;

            foreach (string mobToIgnore in _ignoreDOTMobs)
            {
                if (monster.Name.ToLower().Contains(mobToIgnore.ToLower()) &&
                    monster.Name.ToLower().Contains("gaoler") == false)
                {
                    ignoreDOT = true;
                }
            }

            if (MemoryFunctions.GetMapID() == 364 && monster.Name.Contains("Good King") == false)
                ignoreDOT = true;


            if (hotkeys[INTERNALRELEASE].PercentReady >= 99 &&
                user.Level >= 26 && Global.DisableBuffs == false && user.TP_Current >= 40 && hotkeys[HEAVYSHOT].PercentReady <= 78)
            {
                //MemoryFunctions.ForceAction(INTERNALRELEASE, 21, MemoryFunctions.ActionType.Ability);
                hotkeys[INTERNALRELEASE].UseAbility();
            }
            else if (recast.WeaponSpecials.Count == 0)
            {
                if ((user.ContainsStatusEffect(130, 0, false, (float) 3.0) == false ||
                     user.ContainsStatusEffect(122, 0, false)) && user.Level >= 2 && user.TP_Current >= 70)
                {
                    //MemoryFunctions.ForceAction(STRAIGHTSHOT, 20, MemoryFunctions.ActionType.WS);
                    hotkeys[STRAIGHTSHOT].UseAbility();
                }
                else if (monster.ContainsStatusEffect(124, user.ID, true, (float) 2.5) == false &&
                         user.Level >= 6 && user.TP_Current >= 100 && ignoreDOT == false && monster.StatusEffects().Count < 29)
                {
                    //MemoryFunctions.ForceAction(VENOMOUSBITE, 20, MemoryFunctions.ActionType.WS);
                    hotkeys[VENOMOUSBITE].UseAbility();
                }
                else if (monster.ContainsStatusEffect(129, user.ID, true, (float) 2.5) == false &&
                         user.Level >= 30 && user.TP_Current >= 100 && monster.StatusEffects().Count < 29 && 
                         (ignoreDOT == false || monster.Name.ToLower().Contains("infernal nail")))
                {
                    //MemoryFunctions.ForceAction(WINDBITE, 20, MemoryFunctions.ActionType.WS);
                    hotkeys[WINDBITE].UseAbility();
                }
                else if (hotkeys[HAYMAKER].Highlighted && user.Level >= 10 && user.TP_Current >= 40)
                {
                    //MemoryFunctions.ForceAction(HAYMAKER, 20, MemoryFunctions.ActionType.WS);
                    hotkeys[HAYMAKER].UseAbility();
                }
                else if (user.TP_Current >= 60)
                {
                    //MemoryFunctions.ForceAction(HEAVYSHOT, 20, MemoryFunctions.ActionType.WS);
                    hotkeys[HEAVYSHOT].UseAbility();
                }
            }
            else if (hotkeys[INVIGORATE].PercentReady >= 100 && user.Level >= 4 &&
                     user.TP_Current <= 550 && hotkeys[HEAVYSHOT].PercentReady <= 78)
            {
                //MemoryFunctions.ForceAction(INVIGORATE, 20, MemoryFunctions.ActionType.Ability);
                hotkeys[INVIGORATE].UseAbility();

            }
            else if (hotkeys[RAGINGSTRIKES].PercentReady >= 100 && user.Level >= 4 && Global.DisableBuffs == false && hotkeys[HEAVYSHOT].PercentReady <= 78)
            {
                //MemoryFunctions.ForceAction(RAGINGSTRIKES, 19, MemoryFunctions.ActionType.Ability);
                hotkeys[RAGINGSTRIKES].UseAbility();
            }
            else if (hotkeys[BLOODFORBLOOD].PercentReady >= 100 && user.Level >= 38 &&
                     monster.Name.Contains("Twintania") == false &&
                     monster.Name.Contains("Conflagration") == false && Global.DisableBuffs == false && hotkeys[HEAVYSHOT].PercentReady <= 78)
                // && user.ContainsStatusEffect(125, 0, false) == false
            {
                //MemoryFunctions.ForceAction(BLOODFORBLOOD, 20, MemoryFunctions.ActionType.Ability);
                hotkeys[BLOODFORBLOOD].UseAbility();
            }
            else if (
                hotkeys[BLOODFORBLOOD].PercentReady >= 100 && user.Level >= 38 && monster.Name.Contains("Twintania") &&
                monster.Health_Percent <= 55 && Global.DisableBuffs == false && hotkeys[HEAVYSHOT].PercentReady <= 78)
                // && user.ContainsStatusEffect(125, 0, false) == false
            {
                //MemoryFunctions.ForceAction(BLOODFORBLOOD, 20, MemoryFunctions.ActionType.Ability);
                hotkeys[BLOODFORBLOOD].UseAbility();
            }
            else if (hotkeys[HAWKSEYE].PercentReady >= 100 && user.Level >= 26 && Global.DisableBuffs == false && hotkeys[HEAVYSHOT].PercentReady <= 78)
            {
                //MemoryFunctions.ForceAction(HAWKSEYE, 15, MemoryFunctions.ActionType.Ability);
                hotkeys[HAWKSEYE].UseAbility();
            }
            else if (hotkeys[BARRAGE].PercentReady >= 100 &&
                     user.Level >= 38 && monster.Name.Contains("Twintania") == false &&
                     monster.Name.Contains("Conflagration") == false && Global.DisableBuffs == false && hotkeys[HEAVYSHOT].PercentReady <= 78)
            {
                //MemoryFunctions.ForceAction(BARRAGE, 12, MemoryFunctions.ActionType.Ability);
                hotkeys[BARRAGE].UseAbility();
            }
            else if (hotkeys[BARRAGE].PercentReady >= 100 &&
                     user.Level >= 38 && monster.Name.Contains("Twintania") &&
                     monster.Health_Percent <= 55 && Global.DisableBuffs == false && hotkeys[HEAVYSHOT].PercentReady <= 78)
            {
                //MemoryFunctions.ForceAction(BARRAGE, 12, MemoryFunctions.ActionType.Ability);
                hotkeys[BARRAGE].UseAbility();
            }

            else if (hotkeys[BLOODLETTER].PercentReady >= 99 && user.Level >= 12 && hotkeys[HEAVYSHOT].PercentReady <= 78)
            {
                //MemoryFunctions.ForceAction(BLOODLETTER, 16, MemoryFunctions.ActionType.Ability);
                hotkeys[BLOODLETTER].UseAbility();
            }
            else if (monster.Health_Percent <= 20 &&
                     hotkeys[MISERYSEND].PercentReady >= 99 && user.Level >= 8 && hotkeys[HEAVYSHOT].PercentReady <= 78)
            {
                //MemoryFunctions.ForceAction(MISERYSEND, 16, MemoryFunctions.ActionType.Ability);
                hotkeys[MISERYSEND].UseAbility();
            }
            else if (
                hotkeys[BLUNTARROW].PercentReady >= 99 && user.Level >= 42 && Global.StunBotOpen == false && hotkeys[HEAVYSHOT].PercentReady <= 78)
            {
                //MemoryFunctions.ForceAction(BLUNTARROW, 16, MemoryFunctions.ActionType.Ability);
                hotkeys[BLUNTARROW].UseAbility();
            }
        }

    }
}
    
