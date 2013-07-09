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

    class NavigationHelper
    {
        public ObservableCollection<Coordinate> Waypoints = new ObservableCollection<Coordinate>();

        private int _currentindex = 0;
        private readonly DispatcherTimer _recordcoordinates = new DispatcherTimer();
        private readonly DispatcherTimer _navigate = new DispatcherTimer();

        private Character _user;

        public event WaypointChangedEventHandler WaypointIndexChanged;

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnWaypointChanged()
        {
            if (WaypointIndexChanged != null)
                WaypointIndexChanged(this, _currentindex);
        }


        private void GrabUser()
        {

            uint StartAddress = MemoryLocations.Database["charmap"];
            _user = new Character(StartAddress);

        }

        public NavigationHelper()
        {
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
                float CurrDistance = currcoordinate.Distance(_user.Coordinate);

                if (CurrDistance < MinDistance)
                {
                    MinDistance = CurrDistance;
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

            _currentindex = FindNearestIndex();
            OnWaypointChanged();

            Keyboard.KeyBoardHelper.KeyDown(Keys.W);
            _navigate.Start();
        }


        public void Stop()
        {
            _navigate.Stop();
            Keyboard.KeyBoardHelper.KeyUp(Keys.W);
        }

        public void Resume()
        {

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
                _currentindex = 0;
                OnWaypointChanged();
            }

            _user.Refresh();

            float NewHeading = _user.Coordinate.AngleTo(Waypoints[_currentindex]);
            _user.Heading = NewHeading;

            if (_user.Coordinate.Distance(Waypoints[_currentindex]) < 1.0)
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
