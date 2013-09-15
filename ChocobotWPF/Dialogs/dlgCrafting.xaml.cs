using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.UIWindows.Crafting;
using Chocobot.Utilities.FileIO;
using Chocobot.Utilities.Input;
using Chocobot.Utilities.Memory;
using MahApps.Metro.Controls;
using Microsoft.Win32;

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
            int craftAmount = 7;

            _status = StatusEnum.Initializing;

            MemoryFunctions.GetCharacters(monsters, fate, players, ref user);

            while (true)
            {
                user.Refresh();
                System.Diagnostics.Debug.Print(user.Status.ToString() + "  -  " + user.Status.ToString("X"));

                if ((user.Status == CharacterStatus.Idle || user.Status == CharacterStatus.Crafting_Idle || user.Status == CharacterStatus.Crafting_Idle2) && user.IsCrafting == false)
                {
                    CraftWindow craftwindow = new CraftWindow();

                    while (craftwindow.RefreshPointers() == false)
                    {
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad0);
                        Thread.Sleep(250);
                    }

                    Thread.Sleep(200);

                    //WaitForCraft(user);
                    
                    bool initialCraft = true;

                restart:


                    foreach (CraftingKey keyCondition in _keyConditions)
                    {
                        while (craftwindow.RefreshPointers())
                        {
                            user.Refresh();
                            craftwindow.Refresh();

                            if (craftwindow.CurrProgress == craftwindow.MaxProgress)
                                break;

                            if(keyCondition.CPCondition)
                            {
                                if (keyCondition.CP < user.CurrentCP)
                                    break;
                            }

                            if (keyCondition.DurabilityCondition)
                            {
                                if (keyCondition.Durability < craftwindow.CurrDurability)
                                    break;
                            }

                            if (keyCondition.ProgressCondition)
                            {
                                if (keyCondition.Progress < craftwindow.CurrProgress)
                                    break;
                            }

                            Utilities.Keyboard.KeyBoardHelper.KeyPress(keyCondition.Key);
                            WaitForAbility(user);

                            if (keyCondition.CPCondition == false && keyCondition.DurabilityCondition == false && keyCondition.ProgressCondition == false)
                                break;

                        }

                    }


                    while (craftwindow.RefreshPointers())
                    {
                        Thread.Sleep(250);
                    }

                }

                Thread.Sleep(500);
            }
        }

        private static void WaitForAbility(Character user)
        {

            user.Refresh();

            Debug.Print("Before: " + user.Status.ToString() + " " + user.IsCrafting.ToString());
            Stopwatch timer = new Stopwatch();

            timer.Reset();
            timer.Start();

            while ((user.UsingAbility == false) && user.IsCrafting && timer.Elapsed.Seconds < 4)
            {
                user.Refresh();
            }

            //while ((user.Status == CharacterStatus.Idle || user.Status == CharacterStatus.Crafting_Idle) && user.IsCrafting && timer.Elapsed.Seconds < 4)
            //{                
            //    user.Refresh();
            //}

            Debug.Print("After: " + user.Status.ToString() + " " + user.IsCrafting.ToString());
            timer.Restart();

            while ((user.UsingAbility == true) && user.IsCrafting && timer.Elapsed.Seconds < 4)
            {
                user.Refresh();
            }

            //while (user.Status != CharacterStatus.Idle && user.Status != CharacterStatus.Crafting_Idle2 && user.IsCrafting && timer.Elapsed.Seconds < 7)
            //{
            //    user.Refresh();
            //}

            //Debug.Print("After2: " + user.Status.ToString() + " " + user.IsCrafting.ToString());
            user.Refresh();
            //Debug.Print("End: " + user.Status.ToString() + " " + user.IsCrafting.ToString());
            Thread.Sleep(600);
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
        public ObservableCollection<CraftingKey> KeyConditions
        { get { return _keyConditions; } }

        public dlgCrafting()
        {
            InitializeComponent();
            
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


            _craftWorker = new CraftWorker(_keyConditions);
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

                newkey.Progress = short.Parse(ini.IniReadValue("Key" + (i + 1), "progress"));
                newkey.ProgressCondition = bool.Parse(ini.IniReadValue("Key" + (i + 1), "progresscondition"));

                _keyConditions.Add(newkey);
            }


        }
    }
}
