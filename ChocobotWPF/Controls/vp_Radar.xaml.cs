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

        private readonly ImageSource _playerBrd;
        private readonly ImageSource _playerBlm;
        private readonly ImageSource _playerDrg;
        private readonly ImageSource _playerMrd;
        private readonly ImageSource _playerMnk;
        private readonly ImageSource _playerPld;
        private readonly ImageSource _playerSch;
        private readonly ImageSource _playerSmn;
        private readonly ImageSource _playerWhm;

        // Toggles
        public bool ShowPlayers = true;
        public bool ShowMonsters = true;
        public bool ShowHunts = true;
        public bool ShowNPCs = true;
        public bool ShowGathering = true;
        public bool ShowFate = true;
        public bool ShowGatheringName = true;
        public bool ShowNPCName = false;
        public bool ShowMonsterName = false;
        public bool ShowHuntsName = false;
        public bool ShowPlayerName = false;
        public bool ShowHidden = false;
        public bool CompassMode = false;
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

                _playerBrd = new BitmapImage(new Uri("pack://application:,,/Resources/Radar/bard.png"));
                _playerBlm = new BitmapImage(new Uri("pack://application:,,/Resources/Radar/blackmage.png"));
                _playerDrg = new BitmapImage(new Uri("pack://application:,,/Resources/Radar/dragoon.png"));
                _playerMrd = new BitmapImage(new Uri("pack://application:,,/Resources/Radar/marauder.png"));
                _playerMnk = new BitmapImage(new Uri("pack://application:,,/Resources/Radar/monk.png"));
                _playerPld = new BitmapImage(new Uri("pack://application:,,/Resources/Radar/paladin.png"));
                _playerSch = new BitmapImage(new Uri("pack://application:,,/Resources/Radar/scholar.png"));
                _playerWhm = new BitmapImage(new Uri("pack://application:,,/Resources/Radar/whitemage.png"));
                _playerSmn = new BitmapImage(new Uri("pack://application:,,/Resources/Radar/summoner.png")); 

        //                private readonly ImageSource _playerBrd;
        //private readonly ImageSource _playerBlm;
        //private readonly ImageSource _playerDrg;
        //private readonly ImageSource _playerMrd;
        //private readonly ImageSource _playerMnk;
        //private readonly ImageSource _playerPld;
        //private readonly ImageSource _playerSch;
        //private readonly ImageSource _playerSmn;
        //private readonly ImageSource _playerWhm;
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

           
            float rotationAmount;
            if (CompassMode)
            {
                rotationAmount = _user.Heading;

                drawingContext.DrawImage(_radarheading,
                new Rect(new Point(origin.X - 128, origin.Y - 256),
                        new Size(256, 256)));
            }
            else
                rotationAmount = (float)3.14159265;
                
            
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


                    Coordinate offset = _user.Coordinate.Subtract(player.Coordinate).Rotate2d(rotationAmount).Scale(scale);
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
                        DrawPlayerIcon(drawingContext, player, screenCoordinate);

                    


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
                        offset = _user.Coordinate.Subtract(monster.Coordinate).Rotate2d(rotationAmount).Scale(scale);
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

        if (ShowHunts)
            {
                foreach (Character monster in _monsters)
                {
                    if (monster.Name.Contains("Bloody Mary") == false &&
                        monster.Name.Contains("Hellsclaw") == false &&
                        monster.Name.Contains("Garlok") == false &&
                        monster.Name.Contains("Barbastelle") == false &&
                        monster.Name.Contains("Unktehi") == false &&
                        monster.Name.Contains("Croakadile") == false &&
                        monster.Name.Contains("Skogs Fru") == false &&
                        monster.Name.Contains("Vogaal Ja") == false &&
                        monster.Name.Contains("Croque-Mitaine") == false &&
                        monster.Name.Contains("Vuokho") == false &&
                        monster.Name.Contains("Cornu") == false &&
                        monster.Name.Contains("Mahisha") == false &&
                        monster.Name.Contains("Myradrosh") == false &&
                        monster.Name.Contains("Marberry") == false &&
                        monster.Name.Contains("Nandi") == false &&
                        monster.Name.Contains("Dark Helmet") == false &&
                        monster.Name.Contains("Nahn") == false &&
                        monster.Name.Contains("Bonnacon") == false &&
                        monster.Name.Contains("White Joker") == false &&
                        monster.Name.Contains("Forneus") == false &&
                        monster.Name.Contains("Laideronnette") == false &&
                        monster.Name.Contains("Stinging Sophie") == false &&
                        monster.Name.Contains("Melt") == false &&
                        monster.Name.Contains("Wulgaru") == false &&
                        monster.Name.Contains("Phecda") == false &&
                        monster.Name.Contains("Girtab") == false &&
                        monster.Name.Contains("Thousand-cast Theda") == false &&
                        monster.Name.Contains("Ghede Ti Malice") == false &&
                        monster.Name.Contains("Mindflayer") == false &&
                        monster.Name.Contains("Naul") == false &&
                        monster.Name.Contains("Marraco") == false &&
                        monster.Name.Contains("Safat") == false &&
                        monster.Name.Contains("Ovjang") == false &&
                        monster.Name.Contains("Sabotender Bailarina") == false &&
                        monster.Name.Contains("Brontes") == false &&
                        monster.Name.Contains("Gatling") == false &&
                        monster.Name.Contains("Maahes") == false &&
                        monster.Name.Contains("Lampalagua") == false &&
                        monster.Name.Contains("Flame Sergeant Dalvag") == false &&
                        monster.Name.Contains("Dalvag's Final Flame") == false &&
                        monster.Name.Contains("Minhocao") == false &&
                        monster.Name.Contains("Albin the Ashen") == false &&
                        monster.Name.Contains("Zanig'oh") == false &&
                        monster.Name.Contains("Nunyunuwi") == false &&
                        monster.Name.Contains("Sewer Syrup") == false &&
                        monster.Name.Contains("Alectyron") == false &&
                        monster.Name.Contains("Zona Seeker") == false &&
                        monster.Name.Contains("Leech King") == false &&
                        monster.Name.Contains("Kurrea") == false &&
                        monster.Name.Contains("Agrippa") == false)
                        continue;
                  

                    if (monster.IsHidden)
                        continue;

                    Coordinate offset;

                    try
                    {
                        offset = _user.Coordinate.Subtract(monster.Coordinate).Rotate2d(rotationAmount).Scale(scale);
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
                        drawingContext.DrawImage(monster.IsClaimed ? _monsterclaimedicon : _monstericon,
                                                 new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                                                          new Size(16, 16)));
                    }

                    if (ShowHuntsName)
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
                        offset = _user.Coordinate.Subtract(monster.Coordinate).Rotate2d(rotationAmount).Scale(scale);
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
                        Coordinate offset = _user.Coordinate.Subtract(NPC.Coordinate).Rotate2d(rotationAmount).Scale(scale);
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
                        Coordinate offset = _user.Coordinate.Subtract(gather.Coordinate).Rotate2d(rotationAmount).Scale(scale);
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
                           new Point(origin.X, origin.Y), 4, 4);

             if (!CompassMode)
             {

                 Coordinate heading = new Coordinate(0, 8, 0);
                 heading = heading.Rotate2d(-_user.Heading);
                 heading = heading.Add(origin);

                 drawingContext.DrawLine(new Pen(new SolidColorBrush(Colors.Green), 5),
                                         new Point(origin.X, origin.Y),
                                         new Point(heading.X, heading.Y));

             }
        }

        private void DrawPlayerIcon(DrawingContext drawingContext, Character player, Coordinate screenCoordinate)
        {
            switch (player.Job)
            {
                case JOB.ACN:
                case JOB.SCH:
                    drawingContext.DrawImage(_playerSch,
                        new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                            new Size(16, 16)));
                    break;
                case JOB.ARC:
                case JOB.BRD:
                    drawingContext.DrawImage(_playerBrd,
                        new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                            new Size(16, 16)));
                    break;
                case JOB.THM:
                case JOB.BLM:
                    drawingContext.DrawImage(_playerBlm,
                        new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                            new Size(16, 16)));
                    break;
                case JOB.LNC:
                case JOB.DRG:
                    drawingContext.DrawImage(_playerDrg,
                        new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                            new Size(16, 16)));
                    break;
                case JOB.WAR:
                case JOB.MRD:
                    drawingContext.DrawImage(_playerMrd,
                        new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                            new Size(16, 16)));
                    break;

                case JOB.PGL:
                case JOB.MNK:
                    drawingContext.DrawImage(_playerMnk,
                        new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                            new Size(16, 16)));
                    break;

                case JOB.GLD:
                case JOB.PLD:
                    drawingContext.DrawImage(_playerPld,
                        new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                            new Size(16, 16)));
                    break;

                case JOB.CNJ:
                case JOB.WHM:
                    drawingContext.DrawImage(_playerWhm,
                        new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                            new Size(16, 16)));
                    break;

                case JOB.SMN:
                    drawingContext.DrawImage(_playerSmn,
                        new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                            new Size(16, 16)));
                    break;
                default:
                    drawingContext.DrawImage(_playericon,
                        new Rect(new Point(screenCoordinate.X, screenCoordinate.Y),
                            new Size(16, 16)));
                    break;
            }
        }
    }
}
