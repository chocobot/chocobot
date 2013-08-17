using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Chocobot.Dialogs;
using Chocobot.MemoryStructures.Aggro;
using Chocobot.MemoryStructures.Character;
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

        private void RefreshCharacterList()
        {
            _monsters.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            _players.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            _npcs.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));

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

            this.Title = string.Format("Chocobot: {0}", _user.Name);
        }


        public MainWindow()
        {
            InitializeComponent();

            MemoryLocations.GetMemlocs();

            MemoryFunctions.GetCharacters(_monsters, _fate, _players, ref _user);
            MemoryFunctions.GetNPCs(_npcs);

            RefreshCharacterList();


            _refresh.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _refresh.Tick += RefreshUser_Tick;

            _refresh.IsEnabled = true;
            _refresh.Start();

        }

        private void RefreshUser_Tick(object sender, EventArgs e)
        {
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

            Character currChar = (Character)lst_Characters.SelectedItem;

            //System.Diagnostics.Debug.Print(currChar.Address.ToString("X"));
 
            currChar.Target();
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {

            //AggroHelper test = new AggroHelper();
            //List<int> newlist = test.GetAggroList();

            MemoryFunctions.GetCharacters(_monsters, _fate, _players, ref _user);
            MemoryFunctions.GetNPCs(_npcs);

            RefreshCharacterList();
        }

        private void btn_Radar_Click(object sender, RoutedEventArgs e)
        {
            dlgRadar dlg = new dlgRadar();
            dlg.Show();

        }

        private void btn_Navigation_Click(object sender, RoutedEventArgs e)
        {
            dlgNavigation dlg = new dlgNavigation();
            dlg.Show();
        }

        private void btn_ExpBot_Click(object sender, RoutedEventArgs e)
        {
            dlgExpBot dlg = new dlgExpBot();
            dlg.Show();

        }

        private void btn_OnEvent_Click(object sender, RoutedEventArgs e)
        {
            dlgOnEvent dlg = new dlgOnEvent();
            dlg.Show();
        }

        private void btn_ShowTarget_Click(object sender, RoutedEventArgs e)
        {
            int targetID = MemoryFunctions.GetTarget();
            System.Diagnostics.Debug.Print(targetID.ToString("X"));
            foreach (Character monster in _monsters)
            {
                if (monster.Address == targetID)
                    System.Diagnostics.Debug.Print("Target: " + monster.Address.ToString("X") + "   ID: " + monster.ID.ToString("X"));

            }

            foreach (Character player in _players)
            {
                if (player.Address == targetID)
                    System.Diagnostics.Debug.Print("Target: " + player.Address.ToString("X") + "   ID: " + player.ID.ToString("X"));

            }


            foreach (Character npc in _npcs)
            {
                if (npc.Address == targetID)
                    System.Diagnostics.Debug.Print("Target: " + npc.Address.ToString("X") + "   ID: " + npc.ID.ToString("X"));

            }

        }

        private void btn_Map_Click(object sender, RoutedEventArgs e)
        {
            dlgMap dlg = new dlgMap();
            dlg.Show();
        }
    }
}
