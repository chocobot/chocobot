using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Character;
using Chocobot.Utilities.Memory;
using Chocobot.MemoryStructures.Map;

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

        public bool ShowMonsters = true;
        public bool ShowNpc = true;
        public bool ShowPlayers = true;


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

            Map.Instance.Refresh();
            _mapIndex = Map.Instance.MapIndex;

            RefreshMap();
            System.Diagnostics.Debug.Print(_mapIndex.ToString());

            //_map = new BitmapImage(new Uri("pack://application:,,/Resources/Maps/map.gif"));
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

            Coordinate origin = new Coordinate((float)((this.ActualWidth / 2)),
                                   (float)((this.ActualHeight / 2) + 2), 0);
            Coordinate newCoord;

            try
            {
                newCoord = new Coordinate { X = (float)((coord.X * _mapinfo.XScale) + origin.X), Y = (float)((coord.Y * _mapinfo.YScale) + origin.Y) };

            }
            catch (Exception)
            {

                newCoord = new Coordinate { X = (float)(0.0), Y = (float)((0.0)) };
            }
            
            return newCoord;

        }

        private Coordinate MapToWorld(Coordinate coord)
        {

            Coordinate origin = new Coordinate((float)((this.ActualWidth / 2)),
                                   (float)((this.ActualHeight / 2) + 2), 0);
            Coordinate newCoord;

            try
            {
                newCoord = new Coordinate { X = (float)((coord.X - origin.X) / _mapinfo.XScale), Y = (float)((coord.Y - origin.Y) / _mapinfo.YScale) };

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

            MemoryFunctions.GetCharacters(_monsters, _fate, _players, ref _user);
            MemoryFunctions.GetNPCs(_npcs);

            // Calculate the 2d map coordinates from 3d world coordinates.
            Coordinate userMapCoord = WorldToMap(_user.Coordinate);

            // Draw the map
            drawingContext.DrawImage(_map, new Rect(new Point(0, 0), new Size(1024, 1024)));



            //// Draw the monsters
            if (ShowMonsters){
                foreach (Character monster in _monsters)
                {
                    Coordinate monsterMapCoord = WorldToMap(monster.Coordinate);

                    if(monster.IsFate)
                        drawingContext.DrawEllipse(Brushes.Magenta, new Pen(new SolidColorBrush(Colors.Lime), 6),
                           new Point(monsterMapCoord.X, monsterMapCoord.Y), 1, 1);
                    else
                        drawingContext.DrawEllipse(Brushes.Red, new Pen(new SolidColorBrush(Colors.Red), 1),
                           new Point(monsterMapCoord.X, monsterMapCoord.Y), 1, 1);
                    

                }
            }

            //// Draw the Players
            if (ShowPlayers)
            {
                foreach (Character player in _players)
                {
                    Coordinate playerMapCoord = WorldToMap(player.Coordinate);

                    drawingContext.DrawEllipse(Brushes.Blue, new Pen(new SolidColorBrush(Colors.Blue), 1),
                                               new Point(playerMapCoord.X, playerMapCoord.Y), 1, 1);

                }
            }


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


            // Draw the user coordinates
            drawingContext.DrawEllipse(Brushes.Cyan, new Pen(new SolidColorBrush(Colors.Cyan), 2), new Point(userMapCoord.X, userMapCoord.Y), 3, 3);


        }

        private void map_MouseUp(object sender, MouseButtonEventArgs e)
        {

            Point p = e.GetPosition(this);
            Coordinate coords = MapToWorld(new Coordinate((float) p.X, (float) p.Y, 0));
            System.Diagnostics.Debug.Print(coords.X + "," + coords.Y);
            
        }
    }
}
