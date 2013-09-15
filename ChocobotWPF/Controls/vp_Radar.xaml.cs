using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Aggro;
using Chocobot.MemoryStructures.Character;
using Chocobot.MemoryStructures.Gathering;
using Chocobot.Utilities.Memory;

using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Chocobot.Controls
{
    /// <summary>
    /// Interaction logic for vp_Radar.xaml
    /// </summary>
    public partial class vp_Radar : UserControl
    {

        private Character _user;
        private readonly List<Character> _players = new List<Character>();
        private readonly List<Character> _monsters = new List<Character>();
        private readonly List<Character> _npcs = new List<Character>();
        private readonly List<Character> _fate = new List<Character>();
        private readonly List<Gathering> _gathering = new List<Gathering>();

        private readonly AggroHelper _aggrohelper = new AggroHelper();

        private readonly ImageSource _monstericon;
        private readonly ImageSource _monsterclaimedicon;
        private readonly ImageSource _playericon;
        private readonly ImageSource _skullicon;
        private readonly ImageSource _fateicon;
        private readonly ImageSource _npcicon;
        private readonly ImageSource _woodicon;
        private readonly ImageSource _radarheading;

        // Toggles
        public bool ShowPlayers = true;
        public bool ShowMonsters = true;
        public bool ShowNPCs = true;
        public bool ShowGathering = true;
        public bool ShowFate = true;
        public bool ShowGatheringName = true;
        public bool ShowNPCName = false;
        public bool ShowMonsterName = false;
        public bool ShowPlayerName = false;
        public bool ShowHidden = false;
        public string Filter = "";
        public bool Test
        {
            get { return ShowPlayers; }
            set { ShowPlayers = value; }
        }



        public vp_Radar()
        {
            InitializeComponent();

            if (MemoryLocations.Database.Count == 0)
                return;

            try // Added this because the designer was going crazy without it.
            {
                _monstericon = new BitmapImage(new Uri("pack://application:,,/Resources/monster_16x16.png"));
                _monsterclaimedicon = new BitmapImage(new Uri("pack://application:,,/Resources/monsterclaimed_16x16.png"));
                _playericon = new BitmapImage(new Uri("pack://application:,,/Resources/player_16x16.png"));
                _fateicon = new BitmapImage(new Uri("pack://application:,,/Resources/fate_16x16.png"));
                _skullicon = new BitmapImage(new Uri("pack://application:,,/Resources/skull_16x16.png"));
                _npcicon = new BitmapImage(new Uri("pack://application:,,/Resources/npc_16x16.png"));
                _woodicon = new BitmapImage(new Uri("pack://application:,,/Resources/wood_16x16.png"));
                _radarheading = new BitmapImage(new Uri("pack://application:,,/Resources/radar_heading.png")); 

                
            } catch
            {
                
            }


            this.IsHitTestVisible = false;
            Background = null;

        }

        public void Refresh()
        {
            InvalidateVisual();
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (MemoryLocations.Database.Count == 0)
                return;

            Coordinate origin = new Coordinate((float) ((this.ActualWidth/2)),
                                               (float) ((this.ActualHeight/2)), 0);


            float scale = ((float) (this.ActualHeight / 2.0f) / 125.0f);
            int targetID = -1;

            List<int> aggroList = _aggrohelper.GetAggroList();

            Pen dashedPen = new Pen(new SolidColorBrush(Colors.White), 2);
            dashedPen.DashStyle = DashStyles.DashDotDot;

            Pen targetedPen = new Pen(new SolidColorBrush(Colors.Yellow), 1);
            targetedPen.DashStyle = DashStyles.DashDotDot;

            try // Added this because the designer was going crazy without it.
            {
                targetID = MemoryFunctions.GetTarget();
                MemoryFunctions.GetCharacters(_monsters, _fate, _players, ref _user);
                MemoryFunctions.GetNPCs(_npcs);
                MemoryFunctions.GetGathering(_gathering);
            } catch
            {
                return;
            }

           
            drawingContext.DrawImage(_radarheading,
                            new Rect(new Point(origin.X - 128, origin.Y - 256),
                                    new Size(256, 256)));


            if (ShowPlayers)
            {

                foreach (Character player in _players)
                {

                    if (player.Valid == false)
                        continue;

                    if (player.Name.ToLower().Contains(Filter) == false)
                        continue;

                    if (_user.Valid == false)
                        return;

                    if (_user.IsHidden)
                        continue;


                    Coordinate offset = _user.Coordinate.Subtract(player.Coordinate).Rotate2d(_user.Heading).Scale(scale);
                    Coordinate screenCoordinate = offset.Add(origin);

                    if (player.Address  == targetID)
                    {
                        drawingContext.DrawEllipse(Brushes.Transparent, new Pen(new SolidColorBrush(Colors.Cyan), 2),
                           new Point(screenCoordinate.X, screenCoordinate.Y), 13, 13);
                    }

                    if (player.Name == _user.Name)
                        continue;

                    if (player.TargetID == _user.Address)
                        drawingContext.DrawLine(targetedPen, new Point(origin.X, origin.Y), new Point(screenCoordinate.X, screenCoordinate.Y));
                    
                    screenCoordinate = screenCoordinate.Add(-8, -8, 0);

                    if (player.Health_Current == 0)
                        drawingContext.DrawImage(_skullicon,
                                                 new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                                                          new Size(16, 16)));
                    else
                        drawingContext.DrawImage(_playericon,
                                                 new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                                                          new Size(16, 16)));

                    if(ShowPlayerName)
                    {
                        FormattedText playerLabel = new FormattedText(player.ToString(), System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 8, Brushes.White);

                        drawingContext.DrawText(playerLabel, new Point(screenCoordinate.X - 15, screenCoordinate.Y - 13));
                    }
                }
            }



            if (ShowMonsters){
                foreach (Character monster in _monsters)
                {
                    if (monster.Name.ToLower().Contains(Filter) == false)
                        continue;

                    if (monster.IsHidden)
                        continue;

                    Coordinate offset;

                    try
                    {
                        offset = _user.Coordinate.Subtract(monster.Coordinate).Rotate2d(_user.Heading).Scale(scale);
                    } catch
                    {
                        return;
                    }
                    
                    Coordinate screenCoordinate = offset.Add(origin);

                    if (monster.Address == targetID)
                    {
                        drawingContext.DrawEllipse(Brushes.Transparent, new Pen(new SolidColorBrush(Colors.Cyan), 2),
                           new Point(screenCoordinate.X, screenCoordinate.Y), 12, 12);
                    }

                   // System.Diagnostics.Debug.Print(aggroList.Count.ToString());
                    // Check for aggro!
                    if (aggroList.Contains(monster.ID))
                    {
                        drawingContext.DrawLine(targetedPen, new Point(origin.X, origin.Y),
                                                new Point(screenCoordinate.X, screenCoordinate.Y));

                        drawingContext.DrawEllipse(Brushes.Red, new Pen(new SolidColorBrush(Colors.Red), 2),
                                                   new Point(screenCoordinate.X, screenCoordinate.Y), 8, 8);
                    }

                    screenCoordinate = screenCoordinate.Add(-8, -8, 0);


                    if (monster.Health_Current == 0)
                        drawingContext.DrawImage(_skullicon,
                                                 new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                                                          new Size(16, 16)));
                    else
                    {
                        drawingContext.DrawImage(monster.IsClaimed?_monsterclaimedicon:_monstericon,
                                                 new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                                                          new Size(16, 16)));
                    }

                    if(ShowMonsterName)
                    {
                    
                        FormattedText monsterLabel = new FormattedText(monster.ToString() + " " + monster.UsingAbilityID.ToString("X"),
                                                                       System.Globalization.CultureInfo.InvariantCulture,
                                                                       FlowDirection.LeftToRight, new Typeface("Arial"), 8,
                                                                       Brushes.White);

                        drawingContext.DrawText(monsterLabel, new Point(screenCoordinate.X - 15, screenCoordinate.Y - 13));
                    }
                }

            }



            if (ShowFate)
            {

                foreach (Character monster in _fate)
                {
                    if (monster.Name.ToLower().Contains(Filter) == false)
                        continue;

                    if (monster.IsHidden)
                        continue;

                    Coordinate offset;

                    try
                    {
                        offset = _user.Coordinate.Subtract(monster.Coordinate).Rotate2d(_user.Heading).Scale(scale);
                    }
                    catch
                    {
                        return;
                    }

                    Coordinate screenCoordinate = offset.Add(origin);

                    if (monster.Address == targetID)
                    {
                        drawingContext.DrawEllipse(Brushes.Transparent, new Pen(new SolidColorBrush(Colors.Cyan), 2),
                           new Point(screenCoordinate.X, screenCoordinate.Y), 12, 12);
                    }

                    // Check for aggro!
                    if (aggroList.Contains(monster.ID))
                    {
                        drawingContext.DrawLine(targetedPen, new Point(origin.X, origin.Y),
                                                new Point(screenCoordinate.X, screenCoordinate.Y));

                        drawingContext.DrawEllipse(Brushes.BlueViolet, new Pen(new SolidColorBrush(Colors.BlueViolet), 2),
                                                   new Point(screenCoordinate.X, screenCoordinate.Y), 8, 8);
                    }

                    screenCoordinate = screenCoordinate.Add(-8, -8, 0);


                    if (monster.Health_Current == 0)
                        drawingContext.DrawImage(_skullicon,
                                                 new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                                                          new Size(16, 16)));
                    else
                    {
                        drawingContext.DrawImage(_fateicon,
                                                 new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                                                          new Size(16, 16)));
                    }

                    if (ShowMonsterName)
                    {

                        FormattedText monsterLabel = new FormattedText(monster.ToString(),
                                                                       System.Globalization.CultureInfo.InvariantCulture,
                                                                       FlowDirection.LeftToRight, new Typeface("Arial"), 8,
                                                                       Brushes.White);

                        drawingContext.DrawText(monsterLabel, new Point(screenCoordinate.X - 15, screenCoordinate.Y - 13));
                    }
                }


            }

            if (ShowNPCs)
            {
                foreach (Character NPC in _npcs)
                {
                    if (NPC.Name.ToLower().Contains(Filter) == false)
                        continue;

                    if (NPC.Type == CharacterType.NPC && ShowNPCs == false)
                        continue;

                    if (NPC.IsHidden)
                        continue;

                    Coordinate screenCoordinate;

                    try
                    {
                        Coordinate offset = _user.Coordinate.Subtract(NPC.Coordinate).Rotate2d(_user.Heading).Scale(scale);
                        screenCoordinate = offset.Add(origin);
                    }
                    catch (Exception)
                    {
                        return;
                    }


                    if (NPC.Address == targetID)
                    {
                        drawingContext.DrawEllipse(Brushes.Transparent, new Pen(new SolidColorBrush(Colors.Cyan), 2),
                           new Point(screenCoordinate.X, screenCoordinate.Y), 13, 13);
                    }

                    screenCoordinate = screenCoordinate.Add(-8, -8, 0);

             
                    drawingContext.DrawImage(_npcicon,
                                                new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                                                        new Size(16, 16)));

                    if (ShowNPCName)
                    {
                        FormattedText npcLabel = new FormattedText(NPC.Name,
                                                                    System.Globalization.CultureInfo.InvariantCulture,
                                                                    FlowDirection.LeftToRight, new Typeface("Arial"),
                                                                    8,
                                                                    Brushes.Wheat);

                        drawingContext.DrawText(npcLabel,
                                                new Point(screenCoordinate.X - 15, screenCoordinate.Y - 13));
                    }
                    
                   

                }

            }



            if (ShowGathering)
            {
                foreach (Gathering gather in _gathering)
                {
                    if (gather.Name.ToLower().Contains(Filter) == false)
                        continue;

                    if (gather.IsHidden && ShowHidden == false)
                        continue;


                    gather.IsHidden = false;

                    Coordinate screenCoordinate;
                
                    try
                    {
                        Coordinate offset = _user.Coordinate.Subtract(gather.Coordinate).Rotate2d(_user.Heading).Scale(scale);
                        screenCoordinate = offset.Add(origin);
                    }
                    catch (Exception)
                    {
                        return;
                    }


                    if (gather.Address == targetID)
                    {
                        drawingContext.DrawEllipse(Brushes.Transparent, new Pen(new SolidColorBrush(Colors.Cyan), 2),
                           new Point(screenCoordinate.X, screenCoordinate.Y), 13, 13);
                    }

                    screenCoordinate = screenCoordinate.Add(-8, -8, 0);
                    
 
                    drawingContext.DrawImage(_woodicon,
                                                new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                                                        new Size(16, 16)));

                    if (ShowGatheringName)
                    {
                        FormattedText npcLabel = new FormattedText(gather.Name,
                                                                    System.Globalization.CultureInfo.InvariantCulture,
                                                                    FlowDirection.LeftToRight, new Typeface("Arial"),
                                                                    8,
                                                                    Brushes.DarkOrange);

                        drawingContext.DrawText(npcLabel,
                                                new Point(screenCoordinate.X - 15, screenCoordinate.Y - 13));
                    }
                    

                }

            }

            drawingContext.DrawEllipse(Brushes.Green, new Pen(new SolidColorBrush(Colors.Green), 1),
                           new Point(origin.X, origin.Y), 2, 2);
        }
    }
}
