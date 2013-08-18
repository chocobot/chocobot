using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Chocobot.Datatypes;
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
                lst_Targets.Items.Add(target);

            }
        }

        private void btn_AddTarget_Click(object sender, System.Windows.RoutedEventArgs e)
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

            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();

            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);

            int curePotency = int.Parse(txt_CurePotency.Text);
            Character maxMissingTarget = null;
            int maxMissing = 0;
            int hurtPlayers = 0;

            foreach (Character target in players)
            {
                if(target.DistanceFrom(_user) >= 30)
                    continue;

                int healthMissing = target.Health_Max - target.Health_Current;

                if (healthMissing >= curePotency)
                {
                    if (healthMissing > maxMissing)
                    {
                        maxMissing = healthMissing;
                        maxMissingTarget = target;
                    }

                    hurtPlayers++;
                }
            }

            int userhealthMissing = _user.Health_Max - _user.Health_Current;

            if (userhealthMissing >= curePotency || _user.Health_Percent < 70)
            {
                maxMissingTarget = _user;
                hurtPlayers = 1;
            }


            if (maxMissingTarget != null)
            {
                maxMissingTarget.Target();

                Utilities.Keyboard.KeyBoardHelper.KeyPress( Keys.D0);
            }
        }

        private void RefreshInformation()
        {
            int curePotency = int.Parse(txt_CurePotency.Text);
            Character maxMissingTarget = null;
            int maxMissing = 0;
            int hurtPlayers = 0;

            _user.Refresh();

            foreach (Character target in _targets)
            {
                target.Refresh();

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

                Utilities.Keyboard.KeyBoardHelper.KeyPress(hurtPlayers >= 2 ? Keys.Dash : Keys.D0);
            }
        }

        private void btn_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _targetMonitor.Start();
        }

        private void btn_Stop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _targetMonitor.Stop();
        }

    }
}
