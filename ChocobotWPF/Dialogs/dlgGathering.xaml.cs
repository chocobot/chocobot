using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.Gathering;
using Chocobot.MemoryStructures.UIWindows.Gathering;
using Chocobot.Utilities.Memory;
using Chocobot.Utilities.Navigation;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgGathering.xaml
    /// </summary>
    public partial class dlgGathering : MetroWindow
    {
        private GatheringWorker _gatheringWorker = new GatheringWorker();
        private Thread _gatheringThread = null;


        public class GatheringWorker
        {
            private BotStage _botstage;
            public NavigationHelper _navigation = new NavigationHelper();
            public List<String> SearchItems = new List<string>();
            public double SearchDistance;
            public double DistanceThreshold;

            private bool _initial = true;

            private enum BotStage
            {
                Nothing,
                Detection,
                Initialize,
                Gather
            }


            private Gathering FindClosestGatheringSpot()
            {

                float minDistance = 99999.0f;

                Gathering closestSpot = null;
                List<Character> players = new List<Character>();
                List<Character> monsters = new List<Character>();
                List<Character> fate = new List<Character>();
                List<Gathering> gathering = new List<Gathering>();
                Character user = null;

                MemoryFunctions.GetCharacters(monsters, fate, players, ref user);
                MemoryFunctions.GetGathering(gathering);

                foreach (Gathering spot in gathering)
                {
                    if (spot.Valid == false)
                        continue;

                    if (spot.IsHidden)
                        continue;

                    if (spot.Name.ToLower().Contains("mineral") || spot.Name.ToLower().Contains("outcrop") || spot.Name.ToLower().Contains("mature tree") || spot.Name.ToLower().Contains("lush vegetation"))
                    {
                        float curDistance = user.Coordinate.Distance(spot.Coordinate);

                        if (curDistance < minDistance && curDistance < SearchDistance)
                        {
                            minDistance = curDistance;
                            closestSpot = spot;
                        }
                    }
                }
                
                // If there is no monster nearby, exit out and try again.
                if (closestSpot == null)
                    return null;

                Debug.Print(minDistance.ToString());

                return closestSpot;

            }


            private void GatherSpot(Character user, Gathering currentSpot)
            {
                currentSpot.Refresh();
                user.Refresh();

                if (user.Health_Current == 0)
                {
                    _botstage = BotStage.Nothing;
                    _navigation.Stop();
                    return;
                }

                if (user.Coordinate.Distance(currentSpot.Coordinate) < 0.5)
                {
                    user.Heading = user.Coordinate.AngleTo(currentSpot.Coordinate);

                    Utilities.Keyboard.KeyBoardHelper.KeyDown(Keys.S);
                    Thread.Sleep(150);
                    Utilities.Keyboard.KeyBoardHelper.KeyUp(Keys.S);
                }
                
                // We are close enough.. stop running
                //Utilities.Keyboard.KeyBoardHelper.KeyUp(Keys.W);


                if (currentSpot.IsHidden == false)
                {
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad0);
                    Thread.Sleep(350);
                }
                else
                {
                    //_ignorelist.Clear();
                    _navigation.Resume();
                    _botstage = BotStage.Detection;
                }

                //// Check if we should ignore the monster because it has not been claimed for a while..
                //// This typically happens if monster is out of line of sight.
                //if (_claimwatch.Elapsed.Seconds > 7 && currentSpot.IsClaimed == false)
                //{
                //    _claimwatch.Stop();
                //    _ignorelist.Add(currentSpot.ID);

                //    _navigation.Resume();
                //    _botstage = BotStage.Detection;
                //}
            }

 
            public void DoWork()
            {

                try
                {

                
                    List<Character> monsters = new List<Character>();
                    List<Character> fate = new List<Character>();
                    List<Character> players = new List<Character>();
                    List<Gathering> gathering = new List<Gathering>();
                    GatheringWindow uiWindow = new GatheringWindow();
                    long gathercount = 0;

                    Character user = null;
                    Gathering currentTarget = null;

                    Stopwatch _claimwatch;
                    byte lastPosition = 1;

                    MemoryFunctions.GetCharacters(monsters, fate, players, ref user);
                    MemoryFunctions.GetGathering(gathering);
                    _botstage = BotStage.Detection;
                    _navigation.Start();

                    _initial = true;
                    while (true)
                    {
                        //Debug.Print(_botstage.ToString());
                        switch (_botstage)
                        {
                            case BotStage.Detection:

                                currentTarget = FindClosestGatheringSpot();
                                if (currentTarget != null)
                                {
                                    _navigation.Stop();
                                    _botstage = BotStage.Initialize;
                                    // Start timer to see if we have run into a problem claiming the monster.
                                    _claimwatch = new Stopwatch();
                                    _claimwatch.Start();
                                }

                                break;

                            case BotStage.Initialize:

                                InitializeGatheringSpot(user, uiWindow, currentTarget);
                            
                                Debug.Print("Window open...");
                                uiWindow.Refresh();

                                Debug.Print("Items Found: " + uiWindow.Items.Count.ToString());
                                GatheringWindow.GatheringItems idealItem = null;

                                bool found = false;
                                foreach (string lstitem in SearchItems)
                                {
                                    foreach (GatheringWindow.GatheringItems item in uiWindow.Items)
                                    {
                                        if (item.Name.ToLower().Contains(lstitem.ToLower()))
                                        {
                                            idealItem = item;
                                            found = true;
                                            break;
                                        }
                                    }


                                    if (found)
                                        break;

                                }


                                //foreach (GatheringWindow.GatheringItems item in uiWindow.Items)
                                //{
                                //    foreach (string lstitem in SearchItems)
                                //    {
                                //        if (item.Name.ToLower().Contains(lstitem.ToLower()))
                                //        {
                                //            idealItem = item;
                                //            found = true;
                                //            break;
                                //        }
                                //    }

                                //    if (found)
                                //        break;
                                //}

                                
                                if (idealItem == null)
                                    idealItem = uiWindow.Items.First();

                                Thread.Sleep(1000);
                                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad6);
                                Thread.Sleep(150);
                                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad6);
                                Thread.Sleep(150);
                                Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad6);
                                Thread.Sleep(200);

                                Debug.Print("Found Item: " + idealItem.Name + " Pos: " + idealItem.Position.ToString() + " " + found);
                                int positionShift = idealItem.Position - lastPosition;
                                Debug.Print("Shift: " + positionShift);
                                for (int i = 1; i <= Math.Abs(positionShift); i++)
                                {
                                    if (positionShift > 0)
                                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad2);
                                    else
                                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad8);


                                    Thread.Sleep(350);
                                }

                                lastPosition = idealItem.Position;


                                if (_initial)
                                {
                                    MessageBox.Show("Make sure your current selection is: " + idealItem.Name +
                                                    "... Click OK to continue.", "Chocobot", MessageBoxButton.OK, MessageBoxImage.Information);
                                    _initial = false;
                                }

                                user.Refresh();

                                if (idealItem.Name.ToLower().Contains("shard") || idealItem.Name.ToLower().Contains("crystal") || idealItem.Name.ToLower().Contains("cluster"))
                                {
                                    if (user.CurrentGP >= 400 && user.Level >= 20)
                                    {

                                        if (idealItem.Name.ToLower().Contains("fire"))
                                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.Dash);
                                        else if (idealItem.Name.ToLower().Contains("lightning"))
                                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D0);
                                        else if (idealItem.Name.ToLower().Contains("water"))
                                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.VK_OEM_PLUS);
                                        else if (user.Level >= 30)
                                            Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1);

                                        Thread.Sleep(500);
                                    }
                                } else {

                                    if (user.CurrentGP >= 100 && user.Level >= 15)
                                    {
                                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D4);
                                        Thread.Sleep(500);
                                    }

                                    //if (user.CurrentGP >= 400 && user.Level >= 30)
                                    //{

                                    //    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D1);
                                    //    Thread.Sleep(500);
                                    //}

                                    //if (user.CurrentGP >= 100 && user.Level >= 15)
                                    //{
                                    //    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D9);
                                    //    Thread.Sleep(500);
                                    //}

                                    //user.Refresh();
                                    //if (user.CurrentGP >= 250 && user.Level >= 10)
                                    //{


                                    //    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D7);
                                    //    Thread.Sleep(500);
                                    //}
                                }
                                gathercount += 1;
                                _botstage = BotStage.Gather;

                                break;

                            case BotStage.Gather:
                                GatherSpot(user, currentTarget);

                                break;


                        }

        

                        Thread.Sleep(250);
                    }
                }
                catch (Exception)
                {
                    _navigation.Stop();
                    throw;
                }
            }

            private void InitializeGatheringSpot(Character user, GatheringWindow uiWindow, Gathering currentSpot)
            {

                user.Refresh();
                Debug.Print("User Status: " + user.Status.ToString() + "  -  " + user.Status.ToString("X"));
                currentSpot.Target();
                Thread.Sleep(100);

                while (uiWindow.RefreshPointers() == false)
                {

                    user.Refresh();

                    user.Heading = user.Coordinate.AngleTo(currentSpot.Coordinate);

                    Utilities.Keyboard.KeyBoardHelper.KeyDown(Keys.W);

                    if (user.Coordinate.Distance2D(currentSpot.Coordinate) > DistanceThreshold)
                    {
                        user.Heading = user.Coordinate.AngleTo(currentSpot.Coordinate);

                        Utilities.Keyboard.KeyBoardHelper.KeyDown(Keys.W);
                        continue;
                    }

                    Utilities.Keyboard.KeyBoardHelper.KeyUp(Keys.W);

                    Thread.Sleep(350);
                    user.Refresh();
                    user.Heading = user.Coordinate.AngleTo(currentSpot.Coordinate);

                    Debug.Print("Waiting for window...");
                    Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.NumPad0);
                    Thread.Sleep(750);
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

        }

        public dlgGathering()
        {
            InitializeComponent();

            vp_map.ShowMonsters = false;
            vp_map.ShowNpc = false;
            vp_map.ShowPlayers = false;
            vp_map.ShowSelf = true;
            vp_map.SmallSelfIcon = true;
            vp_map.SmallMarkers = true;
            vp_map.ShowPaths = true;
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {

            if(_gatheringWorker._navigation.Waypoints.Count == 0)
            {
                MessageBox.Show("Please load a waypoint list.", "Chocobot", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            if (_gatheringThread != null)
                if (_gatheringThread.IsAlive)
                {
                    _gatheringThread.Abort();
                    while (_gatheringThread.IsAlive)
                    {
                    }
                }


            _gatheringWorker.SearchItems.Clear();
            foreach (string item in lst_Items.Items)
            {
                _gatheringWorker.SearchItems.Add(item);
            }

            _gatheringWorker.DistanceThreshold = double.Parse(txt_GatheringThreshhold.Text);
            _gatheringWorker.SearchDistance = double.Parse(txt_SearchDistance.Text);


            _gatheringThread = new Thread(new ThreadStart(_gatheringWorker.DoWork));

            _gatheringThread.Start();

            while (!_gatheringThread.IsAlive)
            {
            }

            System.Diagnostics.Debug.Print("Thread Started");
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (_gatheringThread.IsAlive)
            {
                _gatheringThread.Abort();
                while (_gatheringThread.IsAlive)
                {
                }

                Debug.Print("Thread Stopped");
            }
        }

        private void btn_LoadPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Navigation (*.nav)|*.nav";
            if (dlg.ShowDialog() == false)
                return;


            _gatheringWorker._navigation.Waypoints.Clear();
            _gatheringWorker._navigation.Load(dlg.FileName);

            lbl_PathWaypoints.Content = "Waypoints: " + _gatheringWorker._navigation.Waypoints.Count.ToString(CultureInfo.InvariantCulture);

            List<Coordinate> waypoints = new List<Coordinate>(_gatheringWorker._navigation.Waypoints);
            vp_map.SetPath(waypoints);


            vp_map.Refresh();
        }

        private void btn_AddItem_Click(object sender, RoutedEventArgs e)
        {
            lst_Items.Items.Add(txt_Item.Text);

        }

        private void lst_Items_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lst_Items.SelectedItem != null)
                lst_Items.Items.Remove(lst_Items.SelectedItem);
        }
    }
}
