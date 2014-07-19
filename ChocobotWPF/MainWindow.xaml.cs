using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Media;
using System.Threading;
using System.Web.ModelBinding;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Assembly.CallingConvention;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Native;
using BondTech.HotKeyManagement.WPF._4;
using Chocobot.Datatypes;
using Chocobot.Dialogs;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Aggro;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.Gathering;
using Chocobot.MemoryStructures.StatusEffect;
using Chocobot.MemoryStructures.UIWindows;
using Chocobot.MemoryStructures.UIWindows.Crafting;
using Chocobot.Utilities.Chatlog;
using Chocobot.Utilities.FileIO;
using Chocobot.Utilities.Keyboard;
using Chocobot.Utilities.Licensing;
using Chocobot.Utilities.Memory;
using Chocobot.Utilities.Misc;
using MahApps.Metro.Controls;
using Keys = Chocobot.Datatypes.Keys;

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
        //private HotKeyManager _hotKeyManager;
        //private GlobalHotKey _enableHK = new GlobalHotKey("Enable", ModifierKeys.None, BondTech.HotKeyManagement.WPF._4.Keys.Oem3, true);
        //private GlobalHotKey _enableHK2 = new GlobalHotKey("floatup", ModifierKeys.None, BondTech.HotKeyManagement.WPF._4.Keys.F1, true);
        //private GlobalHotKey _enableHK3 = new GlobalHotKey("floatdown", ModifierKeys.None, BondTech.HotKeyManagement.WPF._4.Keys.F2, true);

        private bool setfloat = false;
        private Coordinate floatingposition;

        private void RefreshCharacterList()
        {
            _monsters.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            _players.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            _npcs.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            _gathering.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));

            //_gathering.Clear();
            //_players.Clear();
            //_monsters.Clear();
            //_npcs.Clear();
            //MessageBox.Show(_npcs.Count.ToString());
            //_fate.Clear();

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
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            InitializeComponent();

            MemoryLocations.GetMemlocs();

            MemoryFunctions.GetCharacters(_monsters, _fate, _players, ref _user);
            MemoryFunctions.GetNPCs(_npcs);
            MemoryFunctions.GetGathering(_gathering);

            RefreshCharacterList();

            string debugMsg="";
            Recast test = new Recast();
            test.Refresh();
            foreach(int a in test.Abilities)
                debugMsg += "\n" + ("Ability: " + a.ToString());

            foreach (int a in test.WeaponSpecials)
                debugMsg += "\n" + ("WS: " + a.ToString());

            foreach (int a in test.SubAbilities)
                debugMsg += "\n" + ("Sub Ability: " + a.ToString());
            
            System.Diagnostics.Debug.Print(debugMsg);
            _refresh.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _refresh.Tick += RefreshUser_Tick;

            _refresh.IsEnabled = true;
            _refresh.Start();

        }

        private void RefreshUser_Tick(object sender, EventArgs e)
        {

            if(setfloat)
            {
                //_user.Coordinate = floatingposition;
            }

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

            try
            {
                int targetID = MemoryFunctions.GetTarget();
                Character selected = new Character((uint)targetID, true);
                Character selectedtargetTarget = MemoryFunctions.GetCharacterFromID(selected.TargetID);
                selected.ContainsStatusEffect(1, 1, false, (float) 1.0);
                Debug.Print(selected.Name);
                Debug.Print("Address: " + selected.Address.ToString("X") + "   ID: " +
                                   selected.ID.ToString("X") + "   Dist: " +
                                   _user.DistanceFrom(selected));
                if (selectedtargetTarget != null)
                    Debug.Print("Targets Target: " + selectedtargetTarget.Name);

                foreach (StatusEffect statusEffect in selected.StatusEffects())
                {
                    Debug.Print("SE: " + statusEffect.ID + " From: " + statusEffect.RecievedFrom.ToString("X"));
                }
                Debug.Print(selected.UsingAbilityID.ToString());
                Debug.Print(selected.Owner.ToString());

                List<Gathering> gathering = new List<Gathering>();

                MemoryFunctions.GetGathering(gathering);
                foreach (Gathering gather in gathering)
                {
                    if (gather.Address == targetID)
                        Debug.Print("Target: " + gather.Address.ToString("X") + "   ID: " + gather.ID.ToString("X") + "   Dist: " + _user.Coordinate.Distance2D(gather.Coordinate));

                }
            }
            catch (Exception ex)
            {
                
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
            
            
           
            //Debug.Print(MemoryFunctions.GetMapID().ToString());
            //LicenseManager.LicenseResult result = LicenseManager.Instance.VerifyLicense();

            //if (result.Valid == false)
            //{
            //    string errString = "";

            //    switch(result.Error)
            //    {
            //        case "ERR101":
            //            errString = "Invalid Username/Password. Please try again.";
            //            break;
            //        case "ERR102":
            //            errString = "Subscription Expired. Please renew your subscription at www.chocobotxiv.com";
            //            break;
            //        case "ERR103":
            //            errString = "This version of Chocobot has expired. Please download the newest release from the forums.";
            //            break;
            //        default:
            //            errString = "Unknown license error. Please report this on the forums.";
            //            break;
            //    }

            //    MessageBox.Show(errString, "Licensing", MessageBoxButton.OK, MessageBoxImage.Information);
            //    Application.Current.Shutdown();
            //}

            //_hotKeyManager = new HotKeyManager(this);
            //_hotKeyManager.AddGlobalHotKey(_enableHK);
            //_hotKeyManager.AddGlobalHotKey(_enableHK2);
            //_hotKeyManager.AddGlobalHotKey(_enableHK3);
            //_hotKeyManager.GlobalHotKeyPressed += new GlobalHotKeyEventHandler(hotKeyManager_GlobalHotKeyPressed);
            //System.Diagnostics.Debug.Print(MemoryFunctions.GetGameTime().ToString());


            //CraftWindow test = new CraftWindow();
            //test.RefreshPointers();
            //test.Refresh();


            //Chatlog c = Chatlog.getInstance();

            //while (true)
            //{
            //    if (c.isNewLine())
            //    {
            //        List<Chatlog.Entry> test = c.getChatLogLines();
            //        if (test.Count > 0)
            //            Console.WriteLine("{0} new log lines", test.Count.ToString());
            //        foreach (Chatlog.Entry line in test)
            //            Console.WriteLine("{0}[{1}] -> {2}", line.timestamp.ToString(), line.code, line.text);
            //    }
            //    Thread.Sleep(300);
            //}




            //Hotkeys hotkeys = new Hotkeys();
            //hotkeys.RefreshAbilities();
            //foreach (var ability in hotkeys.Abilities)
            //{
            //    Debug.Print("ID: " + ability.Value.ID + "  Activated: " + ability.Value.Activated.ToString());
            //}

            DateTime eorzaTime = MemoryFunctions.GetGameTime();

            Debug.Print(eorzaTime.ToString("hh:mm tt"));

            Debug.Print(_user.MonstersNear(15).Count.ToString());
        }

        private void hotKeyManager_GlobalHotKeyPressed(object sender, GlobalHotKeyEventArgs e)
        {
            //switch (e.HotKey.Name.ToLower())
            //{
            //    case "enable":
            //        if (setfloat == false)
            //        {
            //            _user.Refresh();
            //            floatingposition = _user.Coordinate;
            //            _refresh.Interval = new TimeSpan(0, 0, 0, 0, 1);
            //            setfloat = true;
            //        }
            //        else
            //        {
            //            setfloat = false;
            //        }
            //        break;
            //    case "floatup":
            //        floatingposition.Z += 18;
            //        break;
            //    case "floatdown":
            //        floatingposition.Z = floatingposition.Z - 18;
            //        break;
            //}
            
        }

        private void btn_ServerRetry_Click(object sender, RoutedEventArgs e)
        {
            MemoryFunctions.HackMaxZoomLevel();

            SoundModule.PlayAsyncSound("Shriek2.wav");
           SoundModule.PlayAsyncSound("Shriek.wav");

            //CHATLOG
            //SigScan ss = new SigScan(MemoryHandler.Instance.Process, new IntPtr(0x1000 + MemoryHandler.Instance.BaseAddress), 0xEE0000);
            //IntPtr result = ss.FindPattern(SigScan.SigToByte("85 c0 74 0e 8b 0d ? ? ? ? 6a 00 51 E8 ? ? ? ? A1"), "xxxxxx????xxxx????x", 19);
            //uint chatlogPtr = MemoryHandler.Instance.GetUInt32((uint)result);

            //System.Diagnostics.Debug.Print(chatlogPtr.ToString("X"));


            // HOTKEY HAX
            //SigScan ss = new SigScan(MemoryHandler.Instance.Process, new IntPtr(0x1000 + MemoryHandler.Instance.BaseAddress), 0xEE0000);
            //IntPtr result = ss.FindPattern(SigScan.SigToByte("55 8B EC 8B 45 08 C1 E0 04 03 45 0C 69 C0 ? ? ? ? 8D 54 08 48"), "xxxxxxxxxxxxxx????xxxx", 0);

            //System.Diagnostics.Debug.Print(result.ToString("X"));
            //MemoryFunctions.ForceHotkey(Convert.ToInt16(txt_Slot.Text), Convert.ToInt16(txt_bar.Text));

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

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            try
            {
//                if (MemoryFunctions.CodeCave.IsValid)

            }
            catch (Exception)
            {

            }

        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }


    }
}
