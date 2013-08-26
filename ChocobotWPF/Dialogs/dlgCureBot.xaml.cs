using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;
using Chocobot.Utilities.Memory;
using MahApps.Metro.Controls;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgCureBot.xaml
    /// </summary>
    public partial class dlgCureBot :  MetroWindow
    {
        private readonly List<Character> _targets = new List<Character>();
        private Character _user;
        private readonly DispatcherTimer _targetMonitor = new DispatcherTimer();

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

            foreach (Character target in _targets)
            {
                lst_Targets.Items.Add(target.Name);

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
                    _targets.Add(p);
                    RefreshTargetList();
                    return;
                }
            }


            foreach (Character p in npcs)
            {
                if (p.Name.ToLower() == txt_TargetName.Text.ToLower())
                {
                    _targets.Add(p);
                    RefreshTargetList();
                    return;
                }
            }

            foreach (Character p in fate)
            {
                if (p.Name.ToLower() == txt_TargetName.Text.ToLower())
                {
                    _targets.Add(p);
                    RefreshTargetList();
                    return;
                }
            }

            foreach (Character p in monsters)
            {
                if (p.Name.ToLower() == txt_TargetName.Text.ToLower())
                {
                    _targets.Add(p);
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
            bool foundInvalid = false;

            if (_user == null)
            {
                List<Character> monsters = new List<Character>();
                List<Character> fate = new List<Character>();
                List<Character> players = new List<Character>();

                MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);
            }


            _user.Refresh();

            foreach (Character target in _targets)
            {
                target.Refresh();

                if(target.Valid == false)
                {
                    foundInvalid = true;
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
                    }

                    hurtPlayers++;
                }
            }

            if(maxMissingTarget != null)
            {
                maxMissingTarget.Target();
                recast.Refresh();

                if (recast.WeaponSpecials.Count == 0)
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(hurtPlayers >= 3 ? Keys.Dash : Keys.D0);
                }

            }

            if (foundInvalid)
                RefreshTargets();

        }

        private void btn_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _targetMonitor.Start();
        }

        private void btn_Stop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _targetMonitor.Stop();
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshTargets();
        }

        private void RefreshTargets()
        {

            for (int i = 0; i < lst_Targets.Items.Count; i++)
            {
                RefreshTarget(lst_Targets.Items[i].ToString(), _targets[i]);
            }


            //List<Character> monsters = new List<Character>();
            //List<Character> fate = new List<Character>();
            //List<Character> players = new List<Character>();

            //MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);

            //players.AddRange(monsters);

            //for (int i = 0; i < _targets.Count; i++)
            //{
            //    foreach (Character p in players)
            //    {
            //        if (p.Name == _targets[i].Name)
            //        {
            //            _targets.RemoveAt(i);
            //            _targets.Insert(i, p);
            //        }
            //    }
            //}
        }

        private void RefreshTarget(string name, Character target)
        {


            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();

            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);

            players.AddRange(monsters);

            
            foreach (Character p in players)
            {
                if (p.Name == name)
                {
                    int i = _targets.IndexOf(target);
                    _targets.Remove(target);
                    _targets.Insert(i, p);

                    return;
                }
            }
            
        }

        private void btn_AddSurroundingPlayers_Click(object sender, RoutedEventArgs e)
        {
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();

            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);

            foreach (Character p in players)
            {
                _targets.Add(p);
            }

            RefreshTargetList();
        }

        private void btn_AddSelectedTarget_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.Print(((uint)MemoryFunctions.GetTarget()).ToString("X"));
            _targets.Add(new Character((uint) MemoryFunctions.GetTarget(), true));
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

            //foreach (Character target in _targets)
            //{
            //    if(target.name.ToLower() == lst_Targets.SelectedItem.ToString().ToLower())
            //    {
            //        _targets.Remove(target);
            //        RefreshTargetList();
            //        return;
            //    }
            //}


            RefreshTargetList();
        }
    }
}
