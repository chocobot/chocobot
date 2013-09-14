using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.UIWindows.Crafting;
using Chocobot.Utilities.Memory;
using MahApps.Metro.Controls;

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
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad0);
                    Thread.Sleep(100);
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad0);
                    Thread.Sleep(350);
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad0);
                    Thread.Sleep(150);
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad0);
                    Thread.Sleep(250);
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad0);

                    Thread.Sleep(750);

                    WaitForCraft(user);

                    Thread.Sleep(500);

                    CraftWindow craftwindow = new CraftWindow();
                    bool initialCraft = true;

                restart:
                    user.Refresh();

                    System.Diagnostics.Debug.Print("Bumping up the craft " + craftwindow.MaxProgress.ToString());
                    while (craftwindow.MaxProgress - craftwindow.CurrProgress > craftAmount && craftwindow.CurrDurability > 10 && user.IsCrafting)
                    {

                        if (user.Level >= 31 && user.CurrentCP > 15)
                        {
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D0);
                        } else
                        {
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1);
                        }

                        WaitForAbility(user);
                        Thread.Sleep(250);
                        craftwindow.Refresh();

                        if (initialCraft)
                        {
                            craftAmount = craftwindow.CurrProgress;
                        }

                        initialCraft = false;
                    }



                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D9);
                    WaitForAbility(user);
                    Thread.Sleep(500);
                    craftwindow.Refresh();
                    user.Refresh();

                    System.Diagnostics.Debug.Print("Checking Durability " + craftwindow.CurrDurability +  "   CP: " + user.CurrentCP);

                    if (user.Level >= 31 && user.CurrentCP >= 32)
                    {
                        while (craftwindow.CurrDurability > 10 && user.CurrentCP >= 47 && user.IsCrafting)
                        {
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D8);
                            WaitForAbility(user);
                            Thread.Sleep(250);
                            craftwindow.Refresh();
                            user.Refresh();
                        }
                    } else
                    {
                        while (craftwindow.CurrDurability > 10 && user.CurrentCP >= 18 && user.IsCrafting && user.Level >= 5)
                        {
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D2);
                            WaitForAbility(user);
                            Thread.Sleep(250);
                            craftwindow.Refresh();
                            user.Refresh();
                        }
                    }



                    //if (user.CurrentCP >= 92 && user.IsCrafting && user.Level >= 7)
                    //{
                    //    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D3);
                    //    WaitForAbility(user);
                    //    Thread.Sleep(1250);
                    //    craftwindow.Refresh();
                    //    user.Refresh();
                    //    System.Diagnostics.Debug.Print("Refreshing Durability " + craftwindow.CurrDurability + "   CP: " + user.CurrentCP);

                    //    goto restart;
                    //}



                    while(user.IsCrafting){
                        System.Diagnostics.Debug.Print("Finishing...");
                        // Finish it up
                        if (user.Level >= 31 && user.CurrentCP > 15)
                        {
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D0);
                        }
                        else
                        {
                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1);
                        }

                        Thread.Sleep(500);
                        user.Refresh();
                    }

                    Thread.Sleep(3000);
                    


                }

                Thread.Sleep(250);
            }
        }

        private static void WaitForAbility(Character user)
        {

            Thread.Sleep(700);
            user.Refresh();
            while (user.Status != CharacterStatus.Idle && user.Status != CharacterStatus.Crafting_Idle && user.Status != CharacterStatus.Crafting_Idle && user.IsCrafting == true)
            {
                user.Refresh();
            }

            Thread.Sleep(700);
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

        private CraftWorker _craftWorker = new CraftWorker();
        private Thread _craftThread = null;
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

            _craftThread = new Thread(new ThreadStart(_craftWorker.DoWork));
            
            _craftThread.Start();

            while (!_craftThread.IsAlive)
            {
            }

            System.Diagnostics.Debug.Print("Thread Started");
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (_craftThread.IsAlive)
            {
                _craftThread.Abort();
                while (_craftThread.IsAlive)
                {
                }

                System.Diagnostics.Debug.Print("Thread Stopped");
            }
        }
    }
}
