using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BondTech.HotKeyManagement.WPF._4;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;
using Chocobot.Utilities.Memory;
using MahApps.Metro.Controls;
using Keys = Chocobot.Datatypes.Keys;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgStunBot.xaml
    /// </summary>
    public partial class dlgStunBot : MetroWindow
    {

        private readonly DispatcherTimer _stunner = new DispatcherTimer();
        private HotKeyManager _hotKeyManager;
        private GlobalHotKey _enableHK = new GlobalHotKey("Enable", ModifierKeys.None, BondTech.HotKeyManagement.WPF._4.Keys.Oem3, true);

        public dlgStunBot()
        {
            InitializeComponent();

            _stunner.Tick += Stunner_Tick;
            _stunner.Interval = new TimeSpan(0,0,0,0,100);
        }

        private void Stunner_Tick(object sender, EventArgs e)
        {

            Hotkeys hotkeys = new Hotkeys();
            hotkeys.RefreshAbilities();

            int targetID = MemoryFunctions.GetTarget();
            if(targetID == -1)
                return;

            Character selected = new Character((uint)targetID, true);


            if (selected.UsingAbilityID != 0)
                lbl_CurrAbility.Content = selected.UsingAbilityID.ToString("X");

            bool found = false;

            foreach (var item in lst_StunID.Items)
            {
                if (selected.UsingAbilityID.ToString("X").ToLower() == item.ToString().ToLower())
                {
                    found = true;
                    break;
                }
            }

            if(found)
            {
                if (hotkeys[109].PercentReady == 100)
                {
                    Global.PauseInput = true;
                    Utilities.Keyboard.KeyBoardHelper.Ctrl(Keys.D3);
                }

            } else
            {
                Global.PauseInput = false;
            }

        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            _stunner.Start();
            this.Title = "Started";
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            _stunner.Stop();
            this.Title = "Stopped";
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _hotKeyManager = new HotKeyManager(this);
            _hotKeyManager.AddGlobalHotKey(_enableHK);

            _hotKeyManager.GlobalHotKeyPressed += new GlobalHotKeyEventHandler(hotKeyManager_GlobalHotKeyPressed);

            Global.StunBotOpen = true;

        }

        private void hotKeyManager_GlobalHotKeyPressed(object sender, GlobalHotKeyEventArgs e)
        {


            switch (e.HotKey.Name.ToLower())
            {
                case "enable":
                    if (_stunner.IsEnabled)
                    {
                        this.Title = "Stopped";
                        _stunner.Stop();
                    }
                    else
                    {
                        _stunner.Start();
                        this.Title = "Running...";
                    }
                    break;
            }
        }

        private void txt_Ability_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                lst_StunID.Items.Add(txt_Ability.Text);
                txt_Ability.Text = "";
            }
        }

        private void lst_StunID_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lst_StunID.SelectedItem == null)
                return;

            lst_StunID.Items.RemoveAt(lst_StunID.SelectedIndex);
        }

        private void MetroWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Global.StunBotOpen = false;
        }
    }
}
