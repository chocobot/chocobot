using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BondTech.HotKeyManagement.WPF._4;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Aggro;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.UIWindows;
using Chocobot.Utilities.Memory;
using Chocobot.Utilities.Misc;
using MahApps.Metro.Controls;
using Keys = Chocobot.Datatypes.Keys;
using System.Diagnostics;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgCureBot.xaml
    /// </summary>
    public partial class dlgCureBot :  MetroWindow
    {

        private const int CURE = 120;
        private const int CURE2 = 135;
        private const int CURE3 = 131;
        private const int MEDICA = 124;
        private const int MEDICA2 = 133;
        private const int DIVINESEAL = 138;
        private const int SHROUD = 130;
        private const int PRESENCEOFMIND = 136;
        private const int ESUNA = 126;
        private const int SWIFTCAST = 150;
        private const int RAISE = 125;
        private const int BENEDICTION = 140;
        private const int REGEN = 137;

        private readonly List<String> _targets = new List<String>();
        private Character _user;
        private readonly DispatcherTimer _targetMonitor = new DispatcherTimer();
        private HotKeyManager _hotKeyManager;
        private GlobalHotKey _enableHK = new GlobalHotKey("Enable", ModifierKeys.None, BondTech.HotKeyManagement.WPF._4.Keys.Oem3, true);
        private bool _spellCast = false;
        private Stopwatch _spellCastTimer;
        private int _castWaitTime = 400;
        private readonly Hotkeys _hotkeys = new Hotkeys();


        public dlgCureBot()
        {
            InitializeComponent();

            _targetMonitor.Tick += thread_TargetMonitor_Tick;
            _targetMonitor.Interval = new TimeSpan(0, 0, 0, 0, 200);
            
        }

        private void thread_TargetMonitor_Tick(object sender, EventArgs e)
        {


            UIWindow ui = new UIWindow();
            ui.RefreshPointers();

            if (ui.GetActiveWindowName() == "chatlog")
                return;

            if (chk_CureAll.IsChecked == true){
                RefreshAllPlayers();
            } else {
                if(_user.Job == JOB.WHM)
                    WhmCureMode();
                else
                    ScholarCureMode();
            }
        }


        private void RefreshTargetList()
        {
            lst_Targets.Items.Clear();

            foreach (string target in _targets)
            {
                lst_Targets.Items.Add(target);

            }
        }

        private void btn_AddTarget_Click(object sender, RoutedEventArgs e)
        {

            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();
            List<Character> npcs = new List<Character>();
             
            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);
            MemoryFunctions.GetNPCs(npcs);

            foreach (Character p in players)
            {
                if(p.Name.ToLower() == txt_TargetName.Text.ToLower())
                {
                    _targets.Add(p.Name);
                    RefreshTargetList();
                    return;
                }
            }


            foreach (Character p in npcs)
            {
                if (p.Name.ToLower() == txt_TargetName.Text.ToLower())
                {
                    _targets.Add(p.Name);
                    RefreshTargetList();
                    return;
                }
            }

            foreach (Character p in fate)
            {
                if (p.Name.ToLower() == txt_TargetName.Text.ToLower())
                {
                    _targets.Add(p.Name);
                    RefreshTargetList();
                    return;
                }
            }

            foreach (Character p in monsters)
            {
                if (p.Name.ToLower() == txt_TargetName.Text.ToLower())
                {
                    _targets.Add(p.Name);
                    RefreshTargetList();
                    return;
                }
            }

        }



        private void RefreshAllPlayers()
        {
            Recast recast = new Recast();
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();

            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);

            int curePotency = int.Parse(txt_CurePotency.Text);
            Character maxMissingTarget = null;
            int maxMissing = 0;


            if (_user.Health_Current == 0 || _user.IsMoving)
                return;

            foreach (Character target in players)
            {
                if(target.DistanceFrom(_user) >= 30)
                    continue;

                if(target.Health_Current == 0)
                    continue;

                int healthMissing = target.Health_Max - target.Health_Current;

                if (healthMissing >= curePotency)
                {
                    if (healthMissing > maxMissing)
                    {
                        maxMissing = healthMissing;
                        maxMissingTarget = target;
                    }
                }
            }

            int userhealthMissing = _user.Health_Max - _user.Health_Current;

            if (userhealthMissing >= curePotency || _user.Health_Percent < 70)
            {
                maxMissingTarget = _user;
            }


            if (maxMissingTarget != null)
            {

                recast.Refresh();
                maxMissingTarget.Target();

                if (recast.WeaponSpecials.Count == 0)
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D0);
                }
            }
        }


        private void WhmCureMode()
        {

            Recast recast = new Recast();
            int curePotency = int.Parse(txt_CurePotency.Text);
            int cure2Potency = int.Parse(txt_Cure2Potency.Text);
            int hurtPlayers = 0;

            Character tank = null, tank2 = null;
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();
            List<Character> npcs = new List<Character>();
            List<Character> damagedTargets = new List<Character>();
            List<Character> deadTargets = new List<Character>();

            Character statusAilment = null;

            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);
            MemoryFunctions.GetNPCs(npcs);

            _hotkeys.QuickRefreshAbilities();


            if (_user.ContainsStatusEffect(292, 0, false))
                return;

            players.AddRange(npcs);

            List<Character> targets = (from player in players from target in _targets where player.Name.ToLower() == target.ToLower() select player).ToList();

            targets = targets.Distinct().ToList();

            if (_user.Health_Current == 0)
                return;

            recast.Refresh();

            if (_hotkeys[SHROUD].PercentReady == 100 && _user.Mana_Percent <= 70 && chk_WHMMode.IsChecked == true) // Shroud
            {
                _hotkeys[SHROUD].UseAbility();
            }


            if (_hotkeys[SHROUD].PercentReady == 100)
            {
                AggroHelper ah = new AggroHelper();
                List<Tuple<int, byte>> aggro = ah.GetAggroListWithEmnity();
                foreach(Tuple<int, byte> a in aggro)
                {
                    // Check if hate level is above 95%
                    if(a.Item2 >= 95)
                    {
                        // Check if the monster has less than 95% life.
                        Character aggroMob = MemoryFunctions.GetCharacterFromID(a.Item1);
                        if(aggroMob == null)
                        {
                            continue;
                        }
                        if (aggroMob.Health_Percent <= 90 && aggroMob.Health_Percent >= 1 && aggroMob.Name.ToLower().Contains("plume") == false)
                        {
                            Debug.Print("Hate Detected: " + a.Item2.ToString() + "   " + aggroMob.Name + "\n");
                            _hotkeys[SHROUD].UseAbility();
                        }
                    }
                }
            }




            foreach (Character target in targets)
            {
                target.Refresh();

                if (target.Name == txt_Tank.Text && target.ContainsStatusEffect(432, 0, false) == false)
                    tank = target;

                if (target.Name == txt_Tank2.Text && target.ContainsStatusEffect(432, 0, false) == false)
                    tank2 = target;

                if(target.Valid == false)
                    continue;

                if (target.ContainsStatusEffect(292, 0, false))
                    continue;

                if (target.ContainsStatusEffect(432, 0, false)) // Leviathan debuff
                    continue;

                if (target.DistanceFrom(_user) >= 30)
                    continue;

                if (target.Health_Current == 0)
                {
                    deadTargets.Add(target);
                    continue;
                }
                
                int healthMissing = target.Health_Max - target.Health_Current;

                if (healthMissing >= curePotency || target.Health_Percent < 70)
                {
                    damagedTargets.Add(target);
                    hurtPlayers++;
                }

                foreach (var item in lst_StatusEffects.Items)
                {
                    if (target.ContainsStatusEffect(Int32.Parse(item.ToString()), 0, false) && target.ContainsStatusEffect(292, 0, false) == false)
                       statusAilment = target;
                }
            }

            if (chk_SwiftcastRaiseAll.IsChecked == true)
            {
                deadTargets.Clear();
                deadTargets.AddRange(from player in players where player.Valid where !player.ContainsStatusEffect(292, 0, false) where !(player.DistanceFrom(_user) >= 30) where player.Health_Current == 0 select player);
            }

            damagedTargets = damagedTargets.Distinct().ToList();

            // Raise Tanks and Healers first
            deadTargets.Sort((a, b) => a.JobPriority.CompareTo(b.JobPriority));

            // Check if tank needs healing first
            if(chk_TankPriority.IsChecked == true)
            {

                // Check who is main tank atm.
                if (tank != null && tank2 != null)
                {
                    Character tank1Target = MemoryFunctions.GetCharacterFromID(tank.TargetID);
                    Character tank2Target = MemoryFunctions.GetCharacterFromID(tank2.TargetID);
                    Character highestHp = null;

                    if (tank2Target != null)
                    {
                        highestHp = tank2Target;
                    }

                    if (tank1Target != null)
                    {
                        highestHp = tank1Target;
                    }

                    if (tank2Target != null && highestHp != null)
                    {
                        if (tank2Target.Health_Max > highestHp.Health_Max)
                            highestHp = tank2Target;
                    }

                    if (highestHp != null)
                    {
                        Character monstersTarget = MemoryFunctions.GetCharacterFromID(highestHp.TargetID);

                        if (monstersTarget != null)
                            if (monstersTarget.Name == tank2.Name)
                            {
                                Character ph = tank;
                                tank = tank2;
                                tank2 = ph;
                            }
                    }
                }

                if (tank != null)
                    this.Title = "Main Tank: " + tank.Name;
         
                if (CheckTankHealth(tank, deadTargets, recast, damagedTargets)) return;
                if (CheckTankHealth(tank2, deadTargets, recast, damagedTargets)) return;
            }



            if (_user.IsMoving || MemoryFunctions.GetGroundCursor() > 0)
            {
                if (_spellCastTimer != null)
                {
                    _spellCastTimer.Stop();
                    _spellCastTimer = null;
                }
                _spellCast = false;

                if (tank != null)
                {
                    if (tank.ContainsStatusEffect(158, _user.ID, true, (float) 2.5) == false &&
                        _hotkeys[REGEN].PercentReady == 100 && tank.Health_Percent != 100)
                    {
                        tank.Target();
                        _hotkeys[REGEN].UseAbility();

                        return;
                    }
                }

                if (damagedTargets.Count > 0)
                {
                    damagedTargets.Sort((a, b) => a.Health_Percent.CompareTo(b.Health_Percent));
                    damagedTargets.First().Target();
                }
                
                return;
            }

            double avgMissingHealth = 0;

            if(damagedTargets.Count > 0)
                avgMissingHealth = damagedTargets.Average(a => (a.Health_Max - a.Health_Current));

            if ((chk_SwiftcastRaise.IsChecked == true || chk_SwiftcastRaiseAll.IsChecked == true) && deadTargets.Count > 0 && _hotkeys[SWIFTCAST].PercentReady == 100 && recast.WeaponSpecials.Count == 0 && _user.Mana_Current >= 798)
            {

                deadTargets.First().Target();
                Thread.Sleep(150);

                if (ForceSwiftcast()) return;

                Thread.Sleep(150);

                _hotkeys[RAISE].UseAbility();
                _hotkeys[RAISE].UseAbility();
                _hotkeys[RAISE].UseAbility();
                Thread.Sleep(50);
                _hotkeys[RAISE].UseAbility();


                Thread.Sleep(150);
            } else if (damagedTargets.Count > 0)
            {

                damagedTargets.Sort((a,b) => a.Health_Percent.CompareTo(b.Health_Percent));
                damagedTargets.First().Target();
                Thread.Sleep(100);

                recast.Refresh();

                // Benediction Check
                if(chk_TankPriority.IsChecked == false)
                {
                    if (damagedTargets.First().Health_Percent <= 30 && _hotkeys[BENEDICTION].PercentReady == 100)
                    {

                        _hotkeys[BENEDICTION].UseAbility();

                        _spellCastTimer = new Stopwatch();
                        _spellCastTimer.Reset();
                        _spellCastTimer.Start();
                        _spellCast = true;
                        return;
                    }
                }

                if (recast.WeaponSpecials.Count == 0)
                {
                    if (_spellCast)
                    {
                        _spellCastTimer = new Stopwatch();
                        _spellCastTimer.Reset();
                        _spellCastTimer.Start();
                        _spellCast = false;
                    }
                    
                    // Make sure we are not spamming spells too quickly.
                    if (_spellCastTimer != null)
                    {
                        if (_spellCastTimer.ElapsedMilliseconds <= _castWaitTime)
                        {
                            return;
                        }
                        _spellCastTimer.Stop();
                        _spellCastTimer = null;
                    }

                    recast.Refresh();
                    Stopwatch timeout = new Stopwatch();
                    timeout.Reset();
                    timeout.Start();
                    //Debug.Print("Casting...");

                    bool useAoe = _hotkeys[MEDICA].TimeSinceLastUse.TotalSeconds > 6  &&
                                  _hotkeys[MEDICA2].TimeSinceLastUse.TotalSeconds > 6 &&
                                  _hotkeys[CURE3].TimeSinceLastUse.TotalSeconds > 6;

                    while (recast.WeaponSpecials.Count == 0)
                    {

                        UIWindow ui = new UIWindow();
                        ui.RefreshPointers();

                        if (ui.GetActiveWindowName() == "chatlog")
                            return;

                        if(timeout.ElapsedMilliseconds > 1000)
                        {
                            _spellCast = false;
                            break;
                        }

                        Whm_AI(hurtPlayers, damagedTargets, cure2Potency, useAoe);
                        recast.Refresh();
                        _hotkeys.QuickRefreshAbilities();
                        _spellCast = true;
                    }
                    
                } else {
                    if (damagedTargets.Count > 2 && avgMissingHealth > curePotency)
                    {
                        if (_hotkeys[PRESENCEOFMIND].PercentReady == 100 && _user.ContainsStatusEffect(159, 0, false) == false)
                        {
                            _hotkeys[PRESENCEOFMIND].UseAbility();
                        }
                        else if (_hotkeys[DIVINESEAL].PercentReady == 100 && _user.ContainsStatusEffect(157, 0, false) == false)
                        {
                            _hotkeys[DIVINESEAL].UseAbility();
                        }
                    }
                }

            }  else if (statusAilment != null)
            {
                statusAilment.Target();
                _hotkeys[ESUNA].UseAbility();
            }
        }




        private void ScholarCureMode()
        {

            Recast recast = new Recast();
            int curePotency = int.Parse(txt_CurePotency.Text);
            int cure2Potency = int.Parse(txt_Cure2Potency.Text);
            //int maxMissing = 0;
            int hurtPlayers = 0;

            Character tank = null, tank2 = null;
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();
            List<Character> npcs = new List<Character>();
            List<Character> damagedTargets = new List<Character>();
            List<Character> deadTargets = new List<Character>();

            Character statusAilment = null;

            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);
            MemoryFunctions.GetNPCs(npcs);

            _hotkeys.QuickRefreshAbilities();

            if (_user.ContainsStatusEffect(292, 0, false))
                return;

            players.AddRange(npcs);

            List<Character> targets = (from player in players from target in _targets where player.Name.ToLower() == target.ToLower() select player).ToList();
            targets = targets.Distinct().ToList();
            if (_user.Health_Current == 0)
                return;


            if (_user.ContainsStatusEffect(451, 0, false, (float)3.0))
            {

                SoundModule.PlayAsyncSound("cursedVoice.wav");

                while (_user.ContainsStatusEffect(451, 0, false))
                {
                    Thread.Sleep(100);
                }

                Thread.Sleep(200);
            }

            recast.Refresh();

            if (recast.Abilities.Contains((int)Recast.eAbilities.Aetherflow) == false && _user.ContainsStatusEffect(304, 0, false, 1) == false) // Aetherflow
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D5);
            }

           
            foreach (Character target in targets)
            {
                target.Refresh();

                if (target.Name == txt_Tank.Text)
                    tank = target;

                if (target.Name == txt_Tank2.Text)
                    tank2 = target;

                if (target.Valid == false)
                    continue;

                if (target.ContainsStatusEffect(292, 0, false))
                    continue;


                if (target.DistanceFrom(_user) >= 30)
                    continue;

                if (target.Health_Current == 0)
                {
                    deadTargets.Add(target);
                    continue;
                }

                int healthMissing = target.Health_Max - target.Health_Current;

                if (healthMissing >= curePotency || target.Health_Percent < 70)
                {
                    damagedTargets.Add(target);
                    hurtPlayers++;
                }

                foreach (var item in lst_StatusEffects.Items)
                {
                    if (target.ContainsStatusEffect(Int32.Parse(item.ToString()), 0, false) && target.ContainsStatusEffect(292, 0, false) == false)
                        statusAilment = target;
                }
            }

            if (chk_SwiftcastRaiseAll.IsChecked == true)
            {
                deadTargets.Clear();
                deadTargets.AddRange(from player in players where player.Valid where !player.ContainsStatusEffect(292, 0, false) where !(player.DistanceFrom(_user) >= 30) where player.Health_Current == 0 select player);
            }

            // Raise Tanks and Healers first
            deadTargets.Sort((a, b) => a.JobPriority.CompareTo(b.JobPriority));


            // Summon Eos only if the dead people do not include a tank or healer
            if (recast.SubAbilities.Contains((int)Recast.eSubAbilities.SwiftCast) == false && recast.WeaponSpecials.Count == 0)
            {
                List<Character> userPets = (from pet in monsters where pet.Owner == _user.ID select pet).ToList();
                if (userPets.Count == 0)
                {
                    if (deadTargets.Count > 0)
                    {
                        if (deadTargets.First().JobPriority > 2)
                        {
                            Thread.Sleep(150);

                            if (ForceSwiftcast()) return;

                            Thread.Sleep(350);

                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6);
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6);
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6);
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6);
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6);
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6);

                            Thread.Sleep(750);
                            return;

                        }
                    }
                    else
                    {
                        Thread.Sleep(150);

                        if (ForceSwiftcast()) return;

                        Thread.Sleep(200);

                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6);
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6);
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6);
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D6);

                        Thread.Sleep(750);
                        return;
                    }

                }
            }



            // Check if tank needs healing first
            if (chk_TankPriority.IsChecked == true)
            {

                // Check who is main tank atm.
                if (tank != null && tank2 != null)
                {
                    Character tank1Target = MemoryFunctions.GetCharacterFromID(tank.TargetID);
                    Character tank2Target = MemoryFunctions.GetCharacterFromID(tank2.TargetID);
                    Character highestHp = null;

                    if (tank2Target != null)
                    {
                        highestHp = tank2Target;
                    }

                    if (tank1Target != null)
                    {
                        highestHp = tank1Target;
                    }

                    if (tank2Target != null && highestHp != null)
                    {
                        if (tank2Target.Health_Max > highestHp.Health_Max)
                            highestHp = tank2Target;
                    }

                    if (highestHp != null)
                    {
                        Character monstersTarget = MemoryFunctions.GetCharacterFromID(highestHp.TargetID);

                        if (monstersTarget != null)
                            if (monstersTarget.Name == tank2.Name)
                            {
                                Character ph = tank;
                                tank = tank2;
                                tank2 = ph;
                            }
                    }
                }

                if (tank != null)
                    this.Title = "Main Tank: " + tank.Name;

                if (CheckTankHealth(tank, deadTargets, recast, damagedTargets)) return;
                if (CheckTankHealth(tank2, deadTargets, recast, damagedTargets)) return;
            }



            if (_user.IsMoving || MemoryFunctions.GetGroundCursor() > 0)
            {
                if (_spellCastTimer != null)
                {
                    _spellCastTimer.Stop();
                    _spellCastTimer = null;
                }
                _spellCast = false;

                if (damagedTargets.Count > 0)
                {
                    damagedTargets.Sort((a, b) => a.Health_Percent.CompareTo(b.Health_Percent));
                    damagedTargets.First().Target();
                }

                return;
            }

            if ((chk_SwiftcastRaise.IsChecked == true || chk_SwiftcastRaiseAll.IsChecked == true) && deadTargets.Count > 0 && recast.SubAbilities.Contains((int)Recast.eSubAbilities.SwiftCast) == false && recast.WeaponSpecials.Count == 0 && _user.Mana_Current >= 798)
            {

                deadTargets.First().Target();
                Thread.Sleep(150);

                if (ForceSwiftcast()) return;

                Thread.Sleep(150);

                Utilities.Keyboard.KeyBoardHelper.Ctrl(Keys.Dash);
                Utilities.Keyboard.KeyBoardHelper.Ctrl(Keys.Dash);
                Utilities.Keyboard.KeyBoardHelper.Ctrl(Keys.Dash);
                

                Thread.Sleep(150);
            } else if (damagedTargets.Count > 0)
            {

                damagedTargets.Sort((a, b) => a.Health_Percent.CompareTo(b.Health_Percent));
                damagedTargets.First().Target();
                Thread.Sleep(100);

                recast.Refresh();

                if (recast.WeaponSpecials.Count == 0)
                {
                    if (_spellCast)
                    {
                        _spellCastTimer = new Stopwatch();
                        _spellCastTimer.Reset();
                        _spellCastTimer.Start();
                        _spellCast = false;
                    }

                    // Make sure we are not spamming spells too quickly.
                    if (_spellCastTimer != null)
                    {
                        if (_spellCastTimer.ElapsedMilliseconds <= _castWaitTime)
                        {
                            return;
                        }
                        _spellCastTimer.Stop();
                        _spellCastTimer = null;
                    }

                   
                    recast.Refresh();
                    Stopwatch timeout = new Stopwatch();
                    timeout.Reset();
                    timeout.Start();

                    while (recast.WeaponSpecials.Count == 0)
                    {
                        if (timeout.ElapsedMilliseconds > 1000)
                        {
                            _spellCast = false;
                            break;
                        }

                        Utilities.Keyboard.KeyBoardHelper.KeyPress(hurtPlayers >= 3 ? Keys.Dash : Keys.D0);
                        recast.Refresh();
                        _spellCast = true;

                        _hotkeys.QuickRefreshAbilities();
                    }

                }
            }
            else if (statusAilment != null)
            {
                statusAilment.Target();
                Utilities.Keyboard.KeyBoardHelper.Ctrl(Keys.D0);
            }
        }

        private bool ForceSwiftcast()
        {


            Stopwatch timeout = new Stopwatch();
            timeout.Reset();
            timeout.Start();
            
            while (_hotkeys[SWIFTCAST].PercentReady == 100)
            {

                UIWindow ui = new UIWindow();
                ui.RefreshPointers();

                if (ui.GetActiveWindowName() == "chatlog")
                    return false;

                if (timeout.ElapsedMilliseconds > 5000)
                {
                    break;
                }

                _hotkeys[SWIFTCAST].UseAbility();
                _hotkeys.QuickRefreshAbilities();
            }

            return false;
        }

        private bool CheckTankHealth(Character tank, List<Character> deadTargets, Recast recast, List<Character> damagedTargets)
        {
            if (tank != null)
            {

                // Benediction Check
                if (chk_WHMMode.IsChecked == true)
                {

                    if (tank.Health_Percent <= 25 && _hotkeys[BENEDICTION].PercentReady == 100 && tank.Health_Current > 0)
                    {
                        tank.Target();
                        Thread.Sleep(100);

                        _hotkeys[BENEDICTION].UseAbility();

                        _spellCastTimer = new Stopwatch();
                        _spellCastTimer.Reset();
                        _spellCastTimer.Start();
                        _spellCast = true;
                        return true;
                    }

                    // Swiftcast Heal
                    if (tank.Health_Percent <= 35 && _hotkeys[SWIFTCAST].PercentReady == 100 && deadTargets.Count == 0)
                    {
                        _hotkeys[SWIFTCAST].UseAbility();
                    }
                }


                if (damagedTargets.Contains(tank) && tank.Health_Percent < 80)
                {
                    damagedTargets.Clear();
                    damagedTargets.Add(tank);

                    if (_user.Health_Percent < 30 && damagedTargets.Count < 3)
                    {
                        damagedTargets.Add(_user);
                    }
                }
            }

            return false;
        }

        private void Whm_AI(int hurtPlayers, List<Character> damagedTargets, int cure2Potency, bool useAOE)
        {

            int cure3Potency = int.Parse(txt_Cure3Potency.Text);

            if (_user.ContainsStatusEffect(155, 0, false) && hurtPlayers <= 3) // Free Cure 2
            {
                _hotkeys[CURE2].UseAbility();
                return;
            }


            double healthMissing = damagedTargets.First().Health_Max - damagedTargets.First().Health_Current;
            double avgMissingHealth = damagedTargets.Average(a => (a.Health_Max - a.Health_Current));
            int inMedicaRange = 0, inMedica2Range = 0, inCure3Range = 0;

            Character bestMedicaTarget = FindBestAOETarget(damagedTargets, true, _user, 15, out inMedicaRange);
            Character bestCure3Target = FindBestAOETarget(damagedTargets, false, _user, 6, out inCure3Range);
            FindBestAOETarget(damagedTargets, true, _user, 25, out inMedica2Range);
            
            if ((avgMissingHealth > cure3Potency && inCure3Range >= 4 && _user.Mana_Current >= 505) || (avgMissingHealth > 900 && inCure3Range >= 4 && _user.Mana_Current >= 253 && _user.ContainsStatusEffect(156, 0, false)) && useAOE)
            {
                Debug.Print("Cure3 In Range: " + inCure3Range + "  Missing Health: " + Math.Round(avgMissingHealth, 1) + "  Target: " + bestCure3Target.Name + "  STD: " + Math.Round(damagedTargets.StandardDeviation()) + " Dmged: " + damagedTargets.Count);
                // Use Cure 3
                bestCure3Target.Target();
                Thread.Sleep(50);
                _hotkeys[CURE3].UseAbility();
                Thread.Sleep(200);
                _castWaitTime = 800;
            }
            else if (avgMissingHealth > 900 && inMedica2Range >= 4 && bestMedicaTarget != null && useAOE)
            {
                // Use Medica 2 if the regen isnt already applied.
                if (_user.ContainsStatusEffect(150, 0, false) == false && _user.Mana_Current >= 452)
                {
                    _hotkeys[MEDICA2].UseAbility();
                    _castWaitTime = 800;
                }
                else if (_user.Mana_Current >= 372 && inMedicaRange >= 4) // Use Medica 1 if regen is already applied.
                {
                    _hotkeys[MEDICA].UseAbility();
                    _castWaitTime = 800;
                }
                else if (_user.Mana_Current >= 266)
                {
                    _hotkeys[CURE2].UseAbility();
                    _castWaitTime = healthMissing > cure2Potency * 2 ? 0 : 400;
                }
                else if (_user.Mana_Current >= 133)
                {
                    _hotkeys[CURE].UseAbility();
                    _castWaitTime = healthMissing > cure2Potency * 2 ? 0 : 400;
                }
            }
            else if (avgMissingHealth > 900 && inMedicaRange >= 3 && bestMedicaTarget != null && _user.Mana_Current >= 372 && useAOE)
            {
                _hotkeys[MEDICA].UseAbility();
                _castWaitTime = 800;
            }
            else if (_user.Mana_Current >= 266 && healthMissing > cure2Potency)
            {
                _hotkeys[CURE2].UseAbility();
                _castWaitTime = healthMissing > cure2Potency * 2 ? 0 : 400;

            } else if (_user.Mana_Current >= 133)
            {
                _hotkeys[CURE].UseAbility();
                _castWaitTime = healthMissing > cure2Potency * 2 ? 0 : 400;
            }

        }

        private static Character FindBestAOETarget(List<Character> damagedTargets, bool userOrigin, Character user, double range, out int bestRange)
        {
            Character bestTarget = null;
            bestRange = 0;

            if(userOrigin)
            {
                int withinRange = 0;
                foreach (Character checkTarget in damagedTargets)
                {
                    if (checkTarget.DistanceFrom(user) <= range)
                    {
                        withinRange++;
                    }
                }

                if (bestRange < withinRange)
                {
                    bestRange = withinRange;
                    bestTarget = user;
                }

            } else
            {
                foreach (Character damagedTarget in damagedTargets)
                {

                    int withinRange = 0;
                    foreach (Character checkTarget in damagedTargets)
                    {
                        if (checkTarget.DistanceFrom(damagedTarget) <= range && (checkTarget.Health_Max - checkTarget.Health_Current) > 400)
                        {
                            withinRange++;
                        }
                    }

                    if (bestRange < withinRange)
                    {
                        bestRange = withinRange;
                        bestTarget = damagedTarget;
                    }
                }  

            }

            return bestTarget;
        }

        private void btn_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _hotkeys.RefreshAbilities();
            _targetMonitor.Start();
            this.Title = "Cure Bot: Running...";
        }

        private void btn_Stop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _targetMonitor.Stop();
            this.Title = "Cure Bot: Stopped";
        }

      

        private void btn_AddSurroundingPlayers_Click(object sender, RoutedEventArgs e)
        {
            
            lst_Targets.Items.Clear();

            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();

            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);

            int TankCount = 0;
            foreach (Character p in players)
            {
                _targets.Add(p.Name);
                if (p.Job == JOB.PLD || p.Job == JOB.WAR)
                {
                    if (TankCount == 0)
                        txt_Tank.Text = p.Name;
                    else
                        txt_Tank2.Text = p.Name;

                    TankCount++;
                }
            }

            RefreshTargetList();
            gb_SelectedTargets.Header = "Selected Targets: " + lst_Targets.Items.Count;

        }

        private void btn_AddSelectedTarget_Click(object sender, RoutedEventArgs e)
        {
            _targets.Add((new Character((uint) MemoryFunctions.GetTarget(), true)).Name);
            RefreshTargetList();
            gb_SelectedTargets.Header = "Selected Targets: " + lst_Targets.Items.Count;

        }

        private void lst_Targets_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void lst_Targets_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lst_Targets.SelectedItem == null)
                return;

            _targets.RemoveAt(lst_Targets.SelectedIndex);


            RefreshTargetList();
            gb_SelectedTargets.Header = "Selected Targets: " + lst_Targets.Items.Count;

        }

        private void dlgCureBot_Loaded(object sender, RoutedEventArgs e)
        {

            _hotKeyManager = new HotKeyManager(this);
            _hotKeyManager.AddGlobalHotKey(_enableHK);

            _hotKeyManager.GlobalHotKeyPressed += new GlobalHotKeyEventHandler(hotKeyManager_GlobalHotKeyPressed);

            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();
            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);

            if (_user.Job == JOB.WHM)
            {
                chk_TankPriority.IsChecked = true;
                chk_WHMMode.IsChecked = true;

            }
            else
            {
                txt_CurePotency.Text = "750";
            }

            lst_StatusEffects.Items.Add("17");
            lst_StatusEffects.Items.Add("62");
            lst_StatusEffects.Items.Add("269");
            lst_StatusEffects.Items.Add("181");
            lst_StatusEffects.Items.Add("216");
            
        }

        private void hotKeyManager_GlobalHotKeyPressed(object sender, GlobalHotKeyEventArgs e)
        {
            switch (e.HotKey.Name.ToLower())
            {
                case "enable":
                    if (_targetMonitor.IsEnabled)
                    {
                        this.Title = "Cure Bot: Stopped";
                        _targetMonitor.Stop();
                    } else
                    {
                        _targetMonitor.Start();
                        this.Title = "Cure Bot: Running...";
                    }
                    break;
            }
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            _targets.Clear();
            RefreshTargetList();
            gb_SelectedTargets.Header = "Selected Targets: " + lst_Targets.Items.Count;

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _targetMonitor.Stop();
        }

        private void btn_AddStatusEffect_Click(object sender, RoutedEventArgs e)
        {
            lst_StatusEffects.Items.Add(txt_statusEffect.Text);
            txt_statusEffect.Text = "";
        }

        private void lst_StatusEffects_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lst_StatusEffects.SelectedItem == null)
                return;

            lst_StatusEffects.Items.RemoveAt(lst_StatusEffects.SelectedIndex);

        }

        private void txt_statusEffect_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                lst_StatusEffects.Items.Add(txt_statusEffect.Text);
                txt_statusEffect.Text = "";
            }

        }

        private void lst_Targets_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (lst_Targets.SelectedItems.Count == 0)
                return;

            txt_Tank.Text = lst_Targets.SelectedItem.ToString();

        }

        private void lst_Targets_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (lst_Targets.SelectedItems.Count == 0)
                return;

            txt_Tank2.Text = lst_Targets.SelectedItem.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string tank1 = txt_Tank.Text;
            string tank2 = txt_Tank2.Text;

            txt_Tank2.Text = tank1;
            txt_Tank.Text = tank2;

        }


    }
}
