using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Chocobot.Datatypes;
using Chocobot.Dialogs;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Aggro;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.Gathering;
using Chocobot.MemoryStructures.UIWindows.Crafting;
using Chocobot.Utilities.FileIO;
using Chocobot.Utilities.Licensing;
using Chocobot.Utilities.Memory;
using MahApps.Metro.Controls;

namespace Chocobot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        private Character _user;
        private readonly List<Character> _players = new List<Character>();
        private readonly List<Character> _monsters = new List<Character>();
        private readonly List<Character> _fate = new List<Character>();
        private readonly List<Character> _npcs = new List<Character>();
        private readonly DispatcherTimer _refresh = new DispatcherTimer();
        private readonly List<Gathering> _gathering = new List<Gathering>();

        private dlgExpBot _dlgExpBot = null;
        private dlgFishingBot _dlgFishBot = null;
        private dlgRadar _dlgRadar = null;
        private dlgNavigation _dlgNavigation = null;
        private dlgMap _dlgMap = null;
        private dlgCureBot _dlgCureBot = null;
        private dlgCrafting _dlgCraftBot = null;
        private dlgGathering _dlgGatheringBot = null;
        private dlgStunBot _dlgStunBot = null;

        private bool _spamServer = false;

        private void RefreshCharacterList()
        {
            _monsters.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            _players.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            _npcs.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            _gathering.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));


            lst_Characters.Items.Clear();
           
            foreach(Character monster in _monsters)
            {
                lst_Characters.Items.Add(monster);

            }

            foreach (Character monster in _fate)
            {
                lst_Characters.Items.Add(monster);

            }

            foreach (Character player in _players)
            {
                lst_Characters.Items.Add(player);

            }


            foreach (Character NPC in _npcs)
            {
                lst_Characters.Items.Add(NPC);

            }

            foreach (Gathering gather in _gathering)
            {
                lst_Characters.Items.Add(gather);

            }

            this.Title = string.Format("Chocobot: {0}", _user.Name);
        }


        public MainWindow()
        {
            InitializeComponent();

            MemoryLocations.GetMemlocs();

            MemoryFunctions.GetCharacters(_monsters, _fate, _players, ref _user);
            MemoryFunctions.GetNPCs(_npcs);
            MemoryFunctions.GetGathering(_gathering);

            RefreshCharacterList();

            Recast test = new Recast();
            test.Refresh();
            foreach(int a in test.Abilities)
                System.Diagnostics.Debug.Print("Ability: " + a.ToString());

            foreach (int a in test.WeaponSpecials)
                System.Diagnostics.Debug.Print("WS: " + a.ToString());

            _refresh.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _refresh.Tick += RefreshUser_Tick;

            _refresh.IsEnabled = true;
            _refresh.Start();

        }

        private void RefreshUser_Tick(object sender, EventArgs e)
        {


            if(_spamServer)
            {
                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad0);
                //Thread.Sleep(150);
                return;
            }

            if (_user == null || _user.Valid == false)
                return;

            _user.Refresh();

            lbl_Name.Content = "Name: " + _user.Name;
            lbl_Health.Content = "Health: " + _user.Health_Current + "/" + _user.Health_Max;
            lbl_TP.Content = "TP: " + _user.TP_Current + "/" + _user.TP_Max;
            lbl_Coords.Content = "Coords: " + Math.Round(_user.Coordinate.X, 2) + " " + Math.Round(_user.Coordinate.Y, 2) + " " + Math.Round(_user.Coordinate.Z, 2);
        }


        private void lst_Characters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lst_Characters.SelectedItem == null)
                return;

            if(typeof(Gathering) == lst_Characters.SelectedItem.GetType())
            {

                Gathering currChar = (Gathering)lst_Characters.SelectedItem;

                System.Diagnostics.Debug.Print(currChar.Address.ToString("X"));

                currChar.Target();
            } else
            {

                Character currChar = (Character)lst_Characters.SelectedItem;

                System.Diagnostics.Debug.Print(currChar.Address.ToString("X"));

                currChar.Target();
            }

        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {

            //AggroHelper test = new AggroHelper();
            //List<int> newlist = test.GetAggroList();

            MemoryFunctions.GetCharacters(_monsters, _fate, _players, ref _user);
            MemoryFunctions.GetNPCs(_npcs);
            MemoryFunctions.GetGathering(_gathering);

            RefreshCharacterList();
        }

        private void btn_Radar_Click(object sender, RoutedEventArgs e)
        {
            if (_dlgRadar != null)
                _dlgRadar.Close();

            _dlgRadar = new dlgRadar();
            _dlgRadar.Show();

        }

        private void btn_Navigation_Click(object sender, RoutedEventArgs e)
        {

            if (_dlgNavigation != null)
                _dlgNavigation.Close();

            _dlgNavigation = new dlgNavigation();
            _dlgNavigation.Show();
            
        }

        private void btn_ExpBot_Click(object sender, RoutedEventArgs e)
        {

            if(_dlgExpBot != null)
                _dlgExpBot.Close();

            _dlgExpBot = new dlgExpBot();
            _dlgExpBot.Show();

        }

    
        private void btn_ShowTarget_Click(object sender, RoutedEventArgs e)
        {
            int targetID = MemoryFunctions.GetTarget();
            System.Diagnostics.Debug.Print(targetID.ToString("X"));

            foreach (Character monster in _monsters)
            {
                if (monster.Address == targetID)
                    System.Diagnostics.Debug.Print("Target: " + monster.Address.ToString("X") + "   ID: " + monster.ID.ToString("X") + "   Dist: " + _user.Coordinate.Distance2D(monster.Coordinate));

            }

            foreach (Character player in _players)
            {
                if (player.Address == targetID)
                    System.Diagnostics.Debug.Print("Target: " + player.Address.ToString("X") + "   ID: " + player.ID.ToString("X") + "   Dist: " + _user.Coordinate.Distance2D(player.Coordinate));

            }


            foreach (Character npc in _npcs)
            {
                if (npc.Address == targetID)
                    System.Diagnostics.Debug.Print("Target: " + npc.Address.ToString("X") + "   ID: " + npc.ID.ToString("X") + "   Dist: " + _user.Coordinate.Distance2D(npc.Coordinate));

            }


            List<Gathering> gathering = new List<Gathering>();

            MemoryFunctions.GetGathering(gathering);
            foreach (Gathering gather in gathering)
            {
                if (gather.Address == targetID)
                    System.Diagnostics.Debug.Print("Target: " + gather.Address.ToString("X") + "   ID: " + gather.ID.ToString("X") + "   Dist: " + _user.Coordinate.Distance2D(gather.Coordinate));

            }

        }

        private void btn_Map_Click(object sender, RoutedEventArgs e)
        {
            if(_dlgMap != null)
                _dlgMap.Close();

            _dlgMap = new dlgMap();
            _dlgMap.Show();
        }

        private void btn_CureBot_Click(object sender, RoutedEventArgs e)
        {

            if (_dlgCureBot != null)
                _dlgCureBot.Close();

            _dlgCureBot = new dlgCureBot();
            _dlgCureBot.Show();

            
        }

        private void btn_FishBot_Click(object sender, RoutedEventArgs e)
        {
            if (_dlgFishBot != null)
                _dlgFishBot.Close();

            _dlgFishBot = new dlgFishingBot();
            _dlgFishBot.Show();

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            LicenseManager.LicenseResult result = LicenseManager.Instance.VerifyLicense();

            if (result.Valid == false)
            {
                string errString = "";

                switch(result.Error)
                {
                    case "ERR101":
                        errString = "Invalid Username/Password. Please try again.";
                        break;
                    case "ERR102":
                        errString = "Subscription Expired. Please renew your subscription at www.chocobotxiv.com";
                        break;
                    case "ERR103":
                        errString = "This version of Chocobot has expired. Please download the newest release from the forums.";
                        break;
                    default:
                        errString = "Unknown license error. Please report this on the forums.";
                        break;
                }

                MessageBox.Show(errString, "Licensing", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }


        }

        private void btn_ServerRetry_Click(object sender, RoutedEventArgs e)
        {
            _spamServer = !_spamServer;
        }

        private void btn_CraftBot_Click(object sender, RoutedEventArgs e)
        {

            if (_dlgCraftBot != null)
                _dlgCraftBot.Close();

            _dlgCraftBot = new dlgCrafting() ;
            _dlgCraftBot.Show();

        }

        private void btn_GatheringBot_Click(object sender, RoutedEventArgs e)
        {
            if (_dlgGatheringBot != null)
                _dlgGatheringBot.Close();

            _dlgGatheringBot = new dlgGathering();
            _dlgGatheringBot.Show();
        }

        private void btn_StunBot_Click(object sender, RoutedEventArgs e)
        {
            if (_dlgStunBot != null)
                _dlgStunBot.Close();

            _dlgStunBot = new dlgStunBot();
            _dlgStunBot.Show();
        }


    }
}
