using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Character;
using Chocobot.Utilities.Memory;
using Chocobot.MemoryStructures.Map;
using Chocobot.Utilities.Navigation;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Chocobot.Controls
{
    /// <summary>
    /// Interaction logic for vp_Map.xaml
    /// </summary>
    public partial class vp_Map : UserControl
    {
        private BitmapImage _map;

        private Character _user;
        private readonly List<Character> _players = new List<Character>();
        private readonly List<Character> _monsters = new List<Character>();
        private readonly List<Character> _npcs = new List<Character>();
        private readonly List<Character> _fate = new List<Character>();
        private MapInfo _mapinfo;

        private readonly NavigationHelper _navigation = new NavigationHelper();

        public bool ShowMonsters = true;
        public bool ShowNpc = true;
        public bool ShowPlayers = true;

        private const short XPixelCount = 1024;
        private const short YPixelCount = 1024;
        private double _mapScaleX = 1.0;
        private double _mapScaleY = 1.0;

        private ushort _mapIndex;

        private void RefreshMap()
        {

            _map = null;

            Uri filePath = new Uri(@"Maps\" + _mapIndex + ".gif", UriKind.Relative);
            _map = System.IO.File.Exists(@"Maps\" + _mapIndex + ".gif") ? new BitmapImage(filePath) : new BitmapImage(new Uri(@"Maps\0.gif", UriKind.Relative));
            _mapinfo = Map.Instance.GetMapInfo();
        }

        public vp_Map()
        {
            InitializeComponent();

            if (MemoryLocations.Database.Count == 0)
                return;

            Map.Instance.Refresh();
            _mapIndex = Map.Instance.MapIndex;

            RefreshMap();
            System.Diagnostics.Debug.Print(_mapIndex.ToString(CultureInfo.InvariantCulture));

            _navigation.WaypointIndexChanged += WaypointIndex_Changed;

        }

        private void WaypointIndex_Changed(object sender, int index)
        {
            
        }

        public void StartRecord()
        {
            
            _navigation.Record();
        }

        public void StopRecording()
        {
            _navigation.StopRecording();
            _mapinfo.WaypointGroups.Add(_navigation.Waypoints.ToList());
            _navigation.Waypoints = new ObservableCollection<Coordinate>();

        }

        public void Refresh()
        {

            Map.Instance.Refresh();

            if(Map.Instance.MapIndex != _mapIndex)
            {
                _mapIndex = Map.Instance.MapIndex;
                RefreshMap();
            }

            InvalidateVisual();
        }

        private Coordinate WorldToMap(Coordinate coord)
        {

            Coordinate origin = new Coordinate((float)((XPixelCount / 2)),
                                   (float)((YPixelCount / 2) + 2), 0);
            Coordinate newCoord;

            try
            {
                newCoord = new Coordinate { X = (float)((coord.X * _mapinfo.XScale) + origin.X) * (float)_mapScaleX, Y = (float)((coord.Y * _mapinfo.YScale) + origin.Y) * (float)_mapScaleY };

            }
            catch (Exception)
            {

                newCoord = new Coordinate { X = (float)(0.0), Y = (float)((0.0)) };
            }
            
            return newCoord;

        }

        private Coordinate MapToWorld(Coordinate coord)
        {

            Coordinate origin = new Coordinate((float)((XPixelCount / 2)),
                                   (float)((YPixelCount / 2) + 2), 0);

            Coordinate newCoord;

            try
            {
                newCoord = new Coordinate { X = (float)(((coord.X / (float)_mapScaleX) - origin.X) / _mapinfo.XScale), Y = (float)(((coord.Y / (float)_mapScaleY) - origin.Y) / _mapinfo.YScale) };

            }
            catch (Exception)
            {

                newCoord = new Coordinate { X = (float)(0.0), Y = (float)((0.0)) };
            }

            return newCoord;

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (MemoryLocations.Database.Count == 0)
                return;

            _mapScaleX = this.ActualWidth / XPixelCount;
            _mapScaleY = this.ActualHeight / YPixelCount;

            MemoryFunctions.GetCharacters(_monsters, _fate, _players, ref _user);
            MemoryFunctions.GetNPCs(_npcs);

            // Calculate the 2d map coordinates from 3d world coordinates.
            Coordinate userMapCoord = WorldToMap(_user.Coordinate);

            // Draw the map
            drawingContext.DrawImage(_map, new Rect(new Point(0, 0), new Size(this.ActualWidth, this.ActualHeight)));


            DrawPaths(drawingContext);
            DrawMonsters(drawingContext);
            DrawPlayers(drawingContext);
            DrawNPCs(drawingContext);

            // Draw the user coordinates
            drawingContext.DrawEllipse(Brushes.Cyan, new Pen(new SolidColorBrush(Colors.Cyan), 2), new Point(userMapCoord.X, userMapCoord.Y), 3, 3);


        }

        private void DrawNPCs(DrawingContext drawingContext)
        {
//// Draw the NPCs
            if (ShowNpc)
            {
                foreach (Character npc in _npcs)
                {
                    Coordinate npcMapCoord = WorldToMap(npc.Coordinate);

                    drawingContext.DrawEllipse(Brushes.DarkGray, new Pen(new SolidColorBrush(Colors.DarkGray), 1),
                                               new Point(npcMapCoord.X, npcMapCoord.Y), 1, 1);
                }
            }
        }

        private void DrawPlayers(DrawingContext drawingContext)
        {
            // Draw the Players
            if (ShowPlayers)
            {
                foreach (Character player in _players)
                {
                    Coordinate playerMapCoord = WorldToMap(player.Coordinate);

                    drawingContext.DrawEllipse(Brushes.Blue, new Pen(new SolidColorBrush(Colors.Blue), 1),
                                               new Point(playerMapCoord.X, playerMapCoord.Y), 1, 1);
                }
            }
        }


        private void DrawMonsters(DrawingContext drawingContext)
        {
            // Draw the monsters
            if (ShowMonsters)
            {
                foreach (Character monster in _monsters)
                {
                    Coordinate monsterMapCoord = WorldToMap(monster.Coordinate);

                    if (monster.IsFate)
                        drawingContext.DrawEllipse(Brushes.Magenta, new Pen(new SolidColorBrush(Colors.Lime), 6),
                                                   new Point(monsterMapCoord.X, monsterMapCoord.Y), 1, 1);
                    else
                        drawingContext.DrawEllipse(Brushes.Red, new Pen(new SolidColorBrush(Colors.Red), 1),
                                                   new Point(monsterMapCoord.X, monsterMapCoord.Y), 1, 1);
                }
            }
        }

        private void DrawPaths(DrawingContext drawingContext)
        {
            Pen p = new Pen(Brushes.MediumAquamarine, 2);
            foreach (List<Coordinate> waypointgroup in _mapinfo.WaypointGroups)
            {
                for(int i = 1; i < waypointgroup.Count; i++)
                {

                    Coordinate pnt1 = WorldToMap(waypointgroup[i - 1]);
                    Coordinate pnt2 = WorldToMap(waypointgroup[i]);

                    drawingContext.DrawLine(p, new Point(pnt1.X, pnt1.Y), new Point(pnt2.X, pnt2.Y));
                }

            }


            for (int i = 1; i < _navigation.Waypoints.Count; i++){
                    Coordinate pnt1 = WorldToMap(_navigation.Waypoints[i - 1]);
                    Coordinate pnt2 = WorldToMap(_navigation.Waypoints[i]);

                    drawingContext.DrawLine(p, new Point(pnt1.X, pnt1.Y), new Point(pnt2.X, pnt2.Y));
            }
            
        }

        private void map_MouseUp(object sender, MouseButtonEventArgs e)
        {

            Point p = e.GetPosition(this);
            Coordinate coords = MapToWorld(new Coordinate((float) p.X, (float) p.Y, 0));
            System.Diagnostics.Debug.Print(coords.X + "," + coords.Y);
            
        }



    }
}
