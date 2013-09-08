using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Character;
using Chocobot.Utilities.Memory;

namespace Chocobot.Utilities.Navigation
{

    public delegate void WaypointChangedEventHandler(object sender, int index);
    public delegate void FinishedEventHandler(object sender);

    public class NavigationHelper
    {
        public ObservableCollection<Coordinate> Waypoints = new ObservableCollection<Coordinate>();

        private int _currentindex;
        private readonly DispatcherTimer _recordcoordinates = new DispatcherTimer();
        private readonly DispatcherTimer _navigate = new DispatcherTimer();

        private Character _user;

        public event WaypointChangedEventHandler WaypointIndexChanged;
        public event FinishedEventHandler NavigationFinished;
        public double Sensitivity = 1.0;
        public bool Loop = true;
        public bool IsPlaying = false;

        private readonly Random _randNum = new Random();
        private readonly Stopwatch _jumpTimer = new Stopwatch();
        private int _jumpRand;


        // Invoke the Changed event; called whenever list changes
        protected virtual void OnWaypointChanged()
        {

            if (_currentindex == Waypoints.Count)
            {
                if (Waypoints[0].ToggleSteatlh)
                {
                    Keyboard.KeyBoardHelper.KeyPress(Keys.D6);
                }

            } else
            {
                if (Waypoints[_currentindex].ToggleSteatlh)
                {
                    Keyboard.KeyBoardHelper.KeyPress(Keys.D6);
                }  
            }

            

            if (WaypointIndexChanged != null)
                WaypointIndexChanged(this, _currentindex);
        }


        private void GrabUser()
        {

            uint startAddress = MemoryLocations.Database["charmap"];
            _user = new Character(startAddress);

        }

        public void CleanWaypoints(double sensitivity)
        {
            for(int i = Waypoints.Count - 1; i > 0; i--)
            {
                if (Waypoints[i].Distance2D(Waypoints[i - 1]) < sensitivity)
                {
                    Waypoints.RemoveAt(i-1);
                }
            }
        }

        public NavigationHelper()
        {

            if (MemoryLocations.Database.Count == 0)
                return;

            _recordcoordinates.Tick += thread_Record_Tick;
            _recordcoordinates.Interval = new TimeSpan(0, 0, 0, 0, 100);

            _navigate.Tick += thread_Navigate_Tick;
            _navigate.Interval = new TimeSpan(0, 0, 0, 0, 100);

            GrabUser();
        }


        public int FindNearestIndex()
        {
            float minDistance = 9999.0f;
            int i = 0;

            _user.Refresh();

            foreach (Coordinate currcoordinate in Waypoints)
            {
                float currDistance = currcoordinate.Distance2D(_user.Coordinate);

                if (currDistance < minDistance)
                {
                    minDistance = currDistance;
                    i = Waypoints.IndexOf(currcoordinate);
                }
            }

            return i;
        }


        public void Record()
        {
            _navigate.Stop();
            _recordcoordinates.Start();
        }
        public void StopRecording()
        {

            _recordcoordinates.Stop();
        }

        public void Start()
        {
            _recordcoordinates.Stop();

            _jumpRand = _randNum.Next(25, 50);
            _jumpTimer.Start();

            IsPlaying = true;

            _currentindex = FindNearestIndex();
            OnWaypointChanged();

            Keyboard.KeyBoardHelper.KeyDown(Keys.W);
            _navigate.Start();

        }


        public void Stop()
        {

            _jumpTimer.Stop();
            IsPlaying = false;
            _navigate.Stop();
            Keyboard.KeyBoardHelper.KeyUp(Keys.W);
        }

        public void Resume()
        {

            _jumpTimer.Reset();
            _jumpTimer.Start();

            
            _jumpRand = _randNum.Next(25, 50);

            IsPlaying = true;
            Keyboard.KeyBoardHelper.KeyDown(Keys.W);
            _navigate.Start();

        }

        public void Save(string filePath )
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                foreach (Coordinate coord in Waypoints)
                {

                    if(coord.ToggleSteatlh)
                    {
                        file.WriteLine(coord.X + "," + coord.Y + "," + coord.Z + ",1");
                    } else
                    {
                        file.WriteLine(coord.X + "," + coord.Y + "," + coord.Z);
                    }
                    

                }
            }
        }

        public void Load(string filePath)
        {
            using (System.IO.StreamReader file = new System.IO.StreamReader(filePath))
            {
                string line;

                while((line = file.ReadLine()) != null)
                {
                    List<string> results = line.Split(Convert.ToChar(",")).ToList();

                    Coordinate newCoordinate = new Coordinate(float.Parse(results[0]), float.Parse(results[1]), float.Parse(results[2]));
                    newCoordinate.ToggleSteatlh = results.Count == 4;

                    Waypoints.Add(newCoordinate);

                }


            }
        }

        private void thread_Navigate_Tick(object sender, EventArgs e)
        {



            if(_jumpTimer.Elapsed.Seconds > _jumpRand )
            {
                Keyboard.KeyBoardHelper.KeyPress(Keys.Space);

                _jumpTimer.Reset();
                _jumpTimer.Start();

                _jumpRand = _randNum.Next(25, 50);
            }

            Keyboard.KeyBoardHelper.KeyDown(Keys.W);

            if (_currentindex >= Waypoints.Count)
            {
                if (Loop)
                {
                    _currentindex = 0;
                    OnWaypointChanged();
                } else
                {
                    Stop();

                    if (NavigationFinished != null)
                        NavigationFinished(this);

                    return;
                }
            }

            _user.Refresh();

            float newHeading = _user.Coordinate.AngleTo(Waypoints[_currentindex]);
            _user.Heading = newHeading;

            if (_user.Coordinate.Distance2D(Waypoints[_currentindex]) < Sensitivity)
            {
                _currentindex++;
                OnWaypointChanged();
            }
        }

        private void thread_Record_Tick(object sender, EventArgs e)
        {
            _user.Refresh();

            if (Waypoints.Count == 0)
            {
                Waypoints.Add(_user.Coordinate);
            }
            else
            {
                if (Waypoints.Last().Distance(_user.Coordinate) > 5)
                {
                    Waypoints.Add(_user.Coordinate);
                }

            }

        }


    }
}
