using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Aggro;
using Chocobot.MemoryStructures.Character;
using Chocobot.Utilities.Memory;
using Chocobot.Utilities.Navigation;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgExpBot.xaml
    /// </summary>
    public partial class dlgExpBot : MetroWindow
    {
        private enum BotStage
        {
            Nothing,
            Detection,
            Fighting,
            Healing
        }

        private readonly NavigationHelper _navigation = new NavigationHelper();
        private readonly DispatcherTimer _monsterdetection = new DispatcherTimer();

        //private readonly DispatcherTimer _monsterfighter = new DispatcherTimer();

        private Character _user;
        private Character _currentmonster = null;
        private readonly AggroHelper _aggrohelper = new AggroHelper();
        private Stopwatch _claimwatch = new Stopwatch();
        private readonly List<int> _ignorelist = new List<int>();

        private readonly Recast _recast = new Recast();

        private BotStage _botstage = BotStage.Nothing;

        public dlgExpBot()
        {
            InitializeComponent();

            _monsterdetection.Tick += thread__MonsterDetection_Tick;
            _monsterdetection.Interval = new TimeSpan(0, 0, 0, 0, 150);

        }



        private void FightMonster()
        {
            _currentmonster.Refresh();
            _user.Refresh();

            _user.Heading = _user.Coordinate.AngleTo(_currentmonster.Coordinate);


            if (chk_IsMage.IsChecked == false)
            {
                if (_user.Coordinate.Distance(_currentmonster.Coordinate) > 3)
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyDown(Keys.W);
                }
                else
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyUp(Keys.W);
                }
            } else
            {
                Utilities.Keyboard.KeyBoardHelper.KeyUp(Keys.W);
            }


            if (_user.Health_Percent < 15)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D0);
            }

            if (_currentmonster.Health_Current > 0)
            {
                _currentmonster.Target();

                _recast.Refresh();
                //Debug.Print("Recast: " + _recast.WeaponSpecials.Count);
                if (_recast.WeaponSpecials.Count == 0)
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1);
            }
            else
            {
                _ignorelist.Clear();
                _navigation.Resume();
                _botstage = BotStage.Detection;
            }

            // Check if we should ignore the monster because it has not been claimed for a while..
            // This typically happens if monster is out of line of sight.
            if(_claimwatch.Elapsed.Seconds > 7 && _currentmonster.IsClaimed == false)
            {
                _claimwatch.Stop();
                _ignorelist.Add(_currentmonster.ID);

                _navigation.Resume();
                _botstage = BotStage.Detection;
            }
        }

        private bool CheckHealth()
        {
            if (_user == null)
                return false;

            _user.Refresh();

            if (_user.Health_Percent < 65 || _user.TP_Current < 250 || _user.Mana_Percent < 30)
            {
                _navigation.Stop();
                _botstage = BotStage.Healing;
                return true;
            }

            return false;
        }

        private void DetectMonster()
        {

            Character closestMonster = null;
            List<int> aggroList = _aggrohelper.GetAggroList();

            // Check if we have aggro
            if (aggroList.Count > 0)
            {
                foreach(int aggroID in aggroList){
                    Character aggroMob = GetMonsterFromID(aggroID);
                    if (aggroMob.Health_Percent > 0)
                        closestMonster = aggroMob;
                }
                
            }

            // Make sure we do not have any aggro...
            if (closestMonster == null)
            {
                // Check if our Hp/Tp is OK
                if(CheckHealth())
                    return;

                // If our health is fine, check for the next closest montser.
                closestMonster = GetClosestMonster();

                // If there is no monster nearby, exit out and try again.
                if (closestMonster == null)
                    return;
            }

            Debug.Print("Found Monster");
            // We have a valid monster, so stop navigating and enable Fight mode!
            _navigation.Stop();

            _currentmonster = closestMonster;

            _botstage = BotStage.Fighting;

            // Start timer to see if we have run into a problem claiming the monster.
            _claimwatch = new Stopwatch();
            _claimwatch.Start();

        }

        private void PlayerRest()
        {

            if (_user == null)
                return;

            _user.Refresh();
            List<int> aggroList = _aggrohelper.GetAggroList();

            if ((_user.Health_Percent > 95 && _user.TP_Current > 700 && _user.Mana_Percent > 95) || aggroList.Count > 0)
            {
                _navigation.Resume();
                _botstage = BotStage.Detection;
            }

        }


        private void thread__MonsterDetection_Tick(object sender, EventArgs e)
        {
            this.Title = _botstage.ToString();

            switch(_botstage)
            {
                case BotStage.Detection:
                    DetectMonster();
                    break;

                case BotStage.Fighting:
                    FightMonster();
                    break;

                case BotStage.Healing:
                    PlayerRest();
                    break;
            }
        }



        private Character GetMonsterFromID(int ID)
        {
            List<Character> players = new List<Character>();
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);

            foreach (Character monster in monsters)
            {
                if (monster.ID == ID)
                    return monster;

            }

            return null;
        }


        private Character GetClosestMonster()
        {
            float minDistance = 99999.0f;

            Character closestMonster = null;
            List<Character> players = new List<Character>();
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();

            MemoryFunctions.GetCharacters(monsters, fate, players, ref _user);

            foreach (Character monster in monsters)
            {
                if (monster.IsClaimed || monster.Health_Percent < 100 || _ignorelist.Contains(monster.ID))
                    continue;

                if (lst_Monsters.Items.Contains(monster.Name))
                {
                    float curDistance = _user.Coordinate.Distance(monster.Coordinate);
                    if (curDistance < minDistance && curDistance < 15)
                    {
                        minDistance = curDistance;
                        closestMonster = monster;
                    }
                }
            }

            return closestMonster;
        }

        private void btn_LoadPath_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == false)
                return;

            _navigation.Load(dlg.FileName);

            lbl_PathWaypoints.Content = "Waypoints: " + _navigation.Waypoints.Count.ToString(CultureInfo.InvariantCulture);
            
        }

        private void btn_AddMonster_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            lst_Monsters.Items.Add(txt_MonsterName.Text);
        }

        private void lst_Monsters_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lst_Monsters.SelectedIndex == -1)
                return;

            lst_Monsters.Items.RemoveAt(lst_Monsters.SelectedIndex);
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            _botstage = BotStage.Detection;
            _navigation.Start();
            _monsterdetection.Start();

        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            _botstage = BotStage.Nothing;
            _monsterdetection.Stop();
            _navigation.Stop();
        }
    }
}
