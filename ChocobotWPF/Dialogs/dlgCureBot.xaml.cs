using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BondTech.HotKeyManagement.WPF._4;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;
using Chocobot.Utilities.Memory;
using MahApps.Metro.Controls;
using Keys = Chocobot.Datatypes.Keys;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgCureBot.xaml
    /// </summary>
    public partial class dlgCureBot :  MetroWindow
    {
        private readonly List<String> _targets = new List<String>();
        private Character _user;
        private readonly DispatcherTimer _targetMonitor = new DispatcherTimer();
        private HotKeyManager _hotKeyManager;
        private GlobalHotKey _enableHK = new GlobalHotKey("Enable", ModifierKeys.None, BondTech.HotKeyManagement.WPF._4.Keys.Oem3, true);
            

        private class TargetStorage
        {
            public readonly bool MemDirect = false;
            public readonly Character Character;
            public readonly string Name;

            public TargetStorage(string name)
            {
                Name = name;
            }

            public TargetStorage(Character character)
            {
                Character = character;
                MemDirect = true;
            }

            public override string ToString()
            {
                if(MemDirect)
                {
                    return Character.Name;
                }

                return Name;
            }
        }

        public dlgCureBot()
        {
            InitializeComponent();

            _targetMonitor.Tick += thread_TargetMonitor_Tick;
            _targetMonitor.Interval = new TimeSpan(0, 0, 0, 0, 150);
            
        }

        private void thread_TargetMonitor_Tick(object sender, EventArgs e)
        {
            if (chk_CureAll.IsChecked == true)
                RefreshAllPlayers();
            else
                RefreshInformation();
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

        private void RefreshInformation()
        {

            Recast recast = new Recast();
            int curePotency = int.Parse(txt_CurePotency.Text);
            Character maxMissingTarget = null;
            int maxMissing = 0;
            int hurtPlayers = 0;

            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();
            List<Character> npcs = new List<Character>();
            List<Character> damagedTargets = new List<Character>();

            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);
            MemoryFunctions.GetNPCs(npcs);

            players.AddRange(npcs);

            List<Character> targets = (from player in players from target in _targets where player.Name.ToLower() == target.ToLower() select player).ToList();

            if (_user.Health_Current == 0 || _user.IsMoving)
                return;

            foreach (Character target in targets)
            {
                target.Refresh();

                if(target.Valid == false)
                {
                    continue;
                }

                if (target.Health_Current == 0)
                    continue;


                if (target.DistanceFrom(_user) >= 30)
                    continue;

                int healthMissing = target.Health_Max - target.Health_Current;

                if (healthMissing >= curePotency || target.Health_Percent < 70)
                {
                    if(healthMissing > maxMissing)
                    {
                        maxMissing = healthMissing;
                        maxMissingTarget = target;
                        damagedTargets.Add(target);
                    }

                    hurtPlayers++;
                }
            }

            if (damagedTargets.Count > 0)
            {
                damagedTargets.Sort((a,b) => a.Health_Percent.CompareTo(b.Health_Percent));
                damagedTargets.First().Target();

                //maxMissingTarget.Target();
                recast.Refresh();

                if (recast.WeaponSpecials.Count == 0)
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(hurtPlayers >= 3 ? Keys.Dash : Keys.D0);
                    Thread.Sleep(500);
                }

            }

        }

        private void btn_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
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
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();

            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);

            foreach (Character p in players)
            {
                _targets.Add(p.Name);
            }

            RefreshTargetList();
        }

        private void btn_AddSelectedTarget_Click(object sender, RoutedEventArgs e)
        {
            _targets.Add((new Character((uint) MemoryFunctions.GetTarget(), true)).Name);
            RefreshTargetList();
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
        }

        private void dlgCureBot_Loaded(object sender, RoutedEventArgs e)
        {

            _hotKeyManager = new HotKeyManager(this);
            _hotKeyManager.AddGlobalHotKey(_enableHK);

            _hotKeyManager.GlobalHotKeyPressed += new GlobalHotKeyEventHandler(hotKeyManager_GlobalHotKeyPressed);
            
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
    }
}
