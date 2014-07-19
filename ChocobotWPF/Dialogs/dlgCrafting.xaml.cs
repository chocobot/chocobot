using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BondTech.HotKeyManagement.WPF._4;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.UIWindows.Crafting;
using Chocobot.Scripting;
using Chocobot.Utilities.FileIO;
using Chocobot.Utilities.Input;
using Chocobot.Utilities.Memory;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Keys = Chocobot.Datatypes.Keys;

namespace Chocobot.Dialogs
{


    public class CraftWorker
    {

        private enum StatusEnum
        {
            Initializing,
            Crafting,
        }

        private StatusEnum _status;
        private ObservableCollection<CraftingKey> _keyConditions;
        public int MaxCrafts;
        public bool ScriptMode = false;
        public CraftingAI CraftAi = null;
        public bool Paused = false;




        public CraftWorker(ObservableCollection<CraftingKey> keyConditions)
        {
            _keyConditions = keyConditions;
        }

        public void DoWork()
        {
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();
            Character user = null;
            int craftAmount = 0;

            _status = StatusEnum.Initializing;

            MemoryFunctions.GetCharacters(monsters, fate, players, ref user);

            while (craftAmount < MaxCrafts)
            {
                user.Refresh();
                Debug.Print(user.Status.ToString() + "  -  " + user.Status.ToString("X"));

                if(Paused)
                    continue;

                if ((user.Status == CharacterStatus.Idle || user.Status == CharacterStatus.Crafting_Idle || user.Status == CharacterStatus.Crafting_Idle2) && user.IsCrafting == false)
                {
                    CraftWindow craftwindow = new CraftWindow();

                    while (craftwindow.RefreshPointers() == false)
                    {
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad0);
                        Thread.Sleep(350);
                    }

                    Thread.Sleep(200);
                    
                    if (ScriptMode)
                    {
                        CraftAi.Craftwindow = craftwindow;
                        CraftAi.Synth();
                    }
                    else
                    {
                        foreach (CraftingKey keyCondition in _keyConditions)
                        {


                            while(Paused){
                                Thread.Sleep(250);
                            }

                            if (craftwindow.RefreshPointers() == false)
                                break;

                            user.Refresh();
                            craftwindow.Refresh();

                            if (craftwindow.CurrProgress == craftwindow.MaxProgress)
                                break;

                            if (keyCondition.CPCondition)
                            {
                                if (user.CurrentCP < keyCondition.CP)
                                    continue;
                            }

                            if (keyCondition.DurabilityCondition)
                            {
                                if (craftwindow.CurrDurability <= keyCondition.Durability)
                                    continue;
                            }

                            if (keyCondition.ProgressCondition)
                            {
                                if (craftwindow.CurrProgress > keyCondition.Progress)
                                    continue;
                            }

                            if (keyCondition.ConditionCondition)
                            {
                                if (craftwindow.Condition.Trim().ToLower() == keyCondition.Condition.Trim().ToLower())
                                    continue;
                            }

                            // Utilities.Keyboard.KeyBoardHelper.KeyPress(keyCondition.Key);
                            WaitForAbility(user, keyCondition.Key, keyCondition.ControlKey);

                        }
                    }

                    while (craftwindow.RefreshPointers())
                    {
                        Thread.Sleep(250);
                    }

                    craftAmount++;

                }

                Thread.Sleep(500);
            }
        }

        private static void WaitForAbility(Character user, Keys key, bool controlKey)
        {

            user.Refresh();

            //Debug.Print("Before: " + user.Status.ToString() + " " + user.IsCrafting.ToString());
            Stopwatch timer = new Stopwatch();

            timer.Reset();
            timer.Start();

            while ((user.UsingAbility) && user.IsCrafting && timer.Elapsed.Seconds < 3)
            {
                user.Refresh();
            }


            while ((user.UsingAbility == false) && user.IsCrafting && timer.Elapsed.Seconds < 3)
            {
                if (controlKey)
                {
                    Utilities.Keyboard.KeyBoardHelper.Ctrl(key);
                }
                else
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(key);
                }
                user.Refresh();
            }

            Debug.Print("Waiting for start: " + timer.Elapsed.Milliseconds.ToString());
            //while ((user.Status == CharacterStatus.Idle || user.Status == CharacterStatus.Crafting_Idle) && user.IsCrafting && timer.Elapsed.Seconds < 4)
            //{                
            //    user.Refresh();
            //}

           // Debug.Print("After: " + user.Status.ToString() + " " + user.IsCrafting.ToString());
            timer.Restart();

            while ((user.UsingAbility == true) && user.IsCrafting && timer.Elapsed.Seconds < 3)
            {
                user.Refresh();
            }

            Debug.Print("Ability Finished: " + timer.Elapsed.Milliseconds.ToString());
            //while (user.Status != CharacterStatus.Idle && user.Status != CharacterStatus.Crafting_Idle2 && user.IsCrafting && timer.Elapsed.Seconds < 7)
            //{
            //    user.Refresh();
            //}

            //Debug.Print("After2: " + user.Status.ToString() + " " + user.IsCrafting.ToString());
            user.Refresh();
            //Debug.Print("End: " + user.Status.ToString() + " " + user.IsCrafting.ToString());
            Thread.Sleep(300);
        }

        private static void WaitForCraft(Character user)
        {

            Thread.Sleep(700);
            user.Refresh();
            while (user.IsCrafting == false)
            {
                user.Refresh();
            }

            Thread.Sleep(700);
        }
    }
    /// <summary>
    /// Interaction logic for dlgCrafting.xaml
    /// </summary>
    public partial class dlgCrafting :  MetroWindow
    {

        private CraftWorker _craftWorker;
        private Thread _craftThread = null;
        private readonly ObservableCollection<CraftingKey> _keyConditions = new ObservableCollection<CraftingKey>();
        private CraftingAI _craftingAI = null;
        private HotKeyManager _hotKeyManager;
        private GlobalHotKey _enableHK = new GlobalHotKey("Enable", ModifierKeys.None, BondTech.HotKeyManagement.WPF._4.Keys.Oem3, true);
            
        public ObservableCollection<CraftingKey> KeyConditions
        { get { return _keyConditions; } }

        public dlgCrafting()
        {
            InitializeComponent();
            lst_SynthMode.Items.Add("Recipe");
            lst_SynthMode.Items.Add("Script");

            lst_SynthMode.SelectedIndex = 0;


        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {

            if (_craftThread != null)
                if (_craftThread.IsAlive)
                {
                    _craftThread.Abort();
                    while (_craftThread.IsAlive)
                    {
                    }
                }

            this.Title = "Crafting Bot";

            _craftWorker = new CraftWorker(_keyConditions) {MaxCrafts = int.Parse(txt_SynthLimit.Text), Paused = false};

            if (lst_SynthMode.SelectedIndex == 1)
            {
                _craftWorker.ScriptMode = true;
                _craftWorker.CraftAi = _craftingAI;
                _craftWorker.CraftAi.Initialize();
            }

            _craftThread = new Thread(new ThreadStart(_craftWorker.DoWork));
            
            _craftThread.Start();

            while (!_craftThread.IsAlive)
            {
            }

            Debug.Print("Thread Started");
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (_craftThread.IsAlive)
            {
                _craftThread.Abort();
                while (_craftThread.IsAlive)
                {
                }

                Debug.Print("Thread Stopped");
            }
        }

        private void btn_AddKey_Click(object sender, RoutedEventArgs e)
        {
            clsCraftingKey keyinput = new clsCraftingKey();
            CraftingKey result = keyinput.Show();

            _keyConditions.Add(result);


        }

        private void lst_KeyConditions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void lst_KeyConditions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lst_KeyConditions.SelectedItems.Count == 0)
                return;

            _keyConditions.RemoveAt(lst_KeyConditions.SelectedIndex);
        }

        private void btn_SaveRecipe_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Crafting Recipe (*.crft)|*.crft";
            if (dlg.ShowDialog() == false)
                return;

            IniParserLegacy.IniFile ini = new IniParserLegacy.IniFile(dlg.FileName);

            ini.IniWriteValue("Main", "KeyCount", _keyConditions.Count.ToString());

            int currkey = 1;

            foreach (CraftingKey keyCondition in _keyConditions)
            {
                ini.IniWriteValue("Key" + currkey, "key", keyCondition.KeyString);
                ini.IniWriteValue("Key" + currkey, "cp", keyCondition.CP.ToString());
                ini.IniWriteValue("Key" + currkey, "cpcondition", keyCondition.CPCondition.ToString());
                ini.IniWriteValue("Key" + currkey, "durability", keyCondition.Durability.ToString());
                ini.IniWriteValue("Key" + currkey, "durabilitycondition", keyCondition.DurabilityCondition.ToString());
                ini.IniWriteValue("Key" + currkey, "progress", keyCondition.Progress.ToString());
                ini.IniWriteValue("Key" + currkey, "progresscondition", keyCondition.ProgressCondition.ToString());
                ini.IniWriteValue("Key" + currkey, "condition", keyCondition.Condition.ToString());
                ini.IniWriteValue("Key" + currkey, "conditioncondition", keyCondition.ConditionCondition.ToString());
                ini.IniWriteValue("Key" + currkey, "controlkey", keyCondition.ControlKey.ToString());
                currkey++;
            }
            

        }

        private void btn_LoadRecipe_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Crafting Recipe (*.crft)|*.crft";
            if (dlg.ShowDialog() == false)
                return;

            IniParserLegacy.IniFile ini = new IniParserLegacy.IniFile(dlg.FileName);

            int keycount = int.Parse(ini.IniReadValue("Main", "KeyCount"));

            _keyConditions.Clear();

            for(int i = 0 ; i < keycount ; i++)
            {
                CraftingKey newkey = new CraftingKey();
                newkey.Key = (Keys)Enum.Parse(typeof(Keys), ini.IniReadValue("Key" + (i + 1), "key"), true);
                newkey.CP = short.Parse(ini.IniReadValue("Key" + (i + 1), "cp"));
                newkey.CPCondition = bool.Parse(ini.IniReadValue("Key" + (i + 1), "cpcondition"));

                newkey.Durability = short.Parse(ini.IniReadValue("Key" + (i + 1), "durability"));
                newkey.DurabilityCondition = bool.Parse(ini.IniReadValue("Key" + (i + 1), "durabilitycondition"));

                try
                {
                    newkey.ControlKey = bool.Parse(ini.IniReadValue("Key" + (i + 1), "controlkey"));
                } catch (Exception ex)
                {
                    newkey.ControlKey = false;
                }
                
                
                newkey.Condition = ini.IniReadValue("Key" + (i + 1), "condition");
                try
                {
                    newkey.ConditionCondition = bool.Parse(ini.IniReadValue("Key" + (i + 1), "conditioncondition"));
                } catch (Exception ex)
                {
                    newkey.ConditionCondition = false;
                }
                


                newkey.Progress = short.Parse(ini.IniReadValue("Key" + (i + 1), "progress"));
                newkey.ProgressCondition = bool.Parse(ini.IniReadValue("Key" + (i + 1), "progresscondition"));

                _keyConditions.Add(newkey);
            }


        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {

                if (_craftThread.IsAlive)
                {
                    _craftThread.Abort();
                    while (_craftThread.IsAlive)
                    {
                    }

                    Debug.Print("Thread Stopped");
                }
            } catch (Exception ex)
            {
                
            }

        }

        private void lst_SynthMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(lst_SynthMode.SelectedIndex == 1)
            {
                List<Character> monsters = new List<Character>();
                List<Character> fate = new List<Character>();
                List<Character> players = new List<Character>();
                Character user = null;
                MemoryFunctions.GetCharacters(monsters, fate, players, ref user);

                _craftingAI = new CraftingAI(user);
                

            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _hotKeyManager = new HotKeyManager(this);

            try
            {
                _hotKeyManager.AddGlobalHotKey(_enableHK);

                _hotKeyManager.GlobalHotKeyPressed += new GlobalHotKeyEventHandler(hotKeyManager_GlobalHotKeyPressed);
            }
            catch (Exception)
            {

                MessageBox.Show("Unable to activate hotkeys. Make sure other bots are closed.", "Chocobot",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void hotKeyManager_GlobalHotKeyPressed(object sender, GlobalHotKeyEventArgs e)
        {
            _craftWorker.Paused = !_craftWorker.Paused;

            if(_craftWorker.Paused)
            {
                this.Title = "Paused..";
            } else
            {
                this.Title = "Crafting Bot";
            }

        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear?", "Chocobot", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            _keyConditions.Clear();

        }
    }
}
