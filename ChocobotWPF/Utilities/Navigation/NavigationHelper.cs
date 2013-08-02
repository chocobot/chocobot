using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Character;
using Chocobot.Utilities.Memory;
using Microsoft.Win32;

namespace Chocobot.Utilities.Navigation
{

    public delegate void WaypointChangedEventHandler(object sender, int Index);
    public delegate void FinishedEventHandler(object sender);

    class NavigationHelper
    {
        public ObservableCollection<Coordinate> Waypoints = new ObservableCollection<Coordinate>();

        private int _currentindex = 0;
        private readonly DispatcherTimer _recordcoordinates = new DispatcherTimer();
        private readonly DispatcherTimer _navigate = new DispatcherTimer();

        private Character _user;

        public event WaypointChangedEventHandler WaypointIndexChanged;
        public event FinishedEventHandler NavigationFinished;
        public double Sensitivity = 1.0;
        public bool Loop = true;
        public bool IsPlaying = false;


        // Invoke the Changed event; called whenever list changes
        protected virtual void OnWaypointChanged()
        {
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
            float MinDistance = 9999.0f;
            int i = 0;

            _user.Refresh();

            foreach (Coordinate currcoordinate in Waypoints)
            {
                float currDistance = currcoordinate.Distance2D(_user.Coordinate);

                if (currDistance < MinDistance)
                {
                    MinDistance = currDistance;
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

            IsPlaying = true;

            _currentindex = FindNearestIndex();
            OnWaypointChanged();

            Keyboard.KeyBoardHelper.KeyDown(Keys.W);
            _navigate.Start();
        }


        public void Stop()
        {

            IsPlaying = false;
            _navigate.Stop();
            Keyboard.KeyBoardHelper.KeyUp(Keys.W);
        }

        public void Resume()
        {

            IsPlaying = true;
            Keyboard.KeyBoardHelper.KeyDown(Keys.W);
            _navigate.Start();

        }

        public void Save(string FilePath )
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath))
            {
                foreach (Coordinate coord in Waypoints)
                {

                    file.WriteLine(coord.X + "," + coord.Y + "," + coord.Z);

                }
            }
        }

        public void Load(string FilePath)
        {
            using (System.IO.StreamReader file = new System.IO.StreamReader(FilePath))
            {
                string line;

                while((line = file.ReadLine()) != null)
                {
                    List<string> results = line.Split(Convert.ToChar(",")).ToList();

                    Coordinate newCoordinate = new Coordinate(float.Parse(results[0]), float.Parse(results[1]), float.Parse(results[2]));

                    Waypoints.Add(newCoordinate);

                }


            }
        }

        private void thread_Navigate_Tick(object sender, EventArgs e)
        {

            //if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.W) == false)
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
