using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Chocobot.Datatypes;
using Chocobot.Dialogs;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;
using Roslyn.Compilers.VisualBasic;
using Color = System.Windows.Media.Color;

namespace Chocobot.CombatAI.Classes
{
    class PugilistAI : GenericAI
    {

        private static dlgMnk _mnkHelper;
        private bool helperVisible = false;

        

        public PugilistAI()
            : base()
        {
            if (helperVisible == false)
            {
                _mnkHelper = new dlgMnk();
                _mnkHelper.Show();
            }
                
            IsRanged = false;
            Name = "Pugilist";

        }

        ~PugilistAI()
        {
            try
            {
                _mnkHelper.Dispatcher.Invoke(_mnkHelper.Close);
            }
            catch
            {
                

            }
            
        }

        public bool MonsterNear(Character monster, string monsterSearch)
        {
            List<Character> nearby = monster.MonstersNear(15);

            foreach (Character currmonster in nearby)
            {
                if (currmonster.Name.ToLower().Contains(monsterSearch.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }
        public override void Fight(Character user, Character monster, Recast recast)
        {

            base.Fight(user, monster, recast);
            bool dotNeeded = !monster.ContainsStatusEffect(246, user.ID, true);
            bool dragonkickNeeded = !monster.ContainsStatusEffect(98, 0, false);
            bool lockIcon;
            monster.Target();
            recast.Refresh(2.0);


            


            try
            {

                ePosition position = monster.Position(user);

                Hotkeys hotkeys = new Hotkeys();
                Hotkeys.Hotkey highlightedAbility = null;

                hotkeys.RefreshAbilities();

                _mnkHelper.txt_Title.Content = "Position: " + position.ToString();


                if (user.ContainsStatusEffect(103, 0, false) == false && hotkeys[63].PercentReady == 100)
                {
                    hotkeys[63].UseAbility();
                }

                int colorOffset = (int)((hotkeys.Abilities[53].PercentReady - 25) * 3.4);
                if (colorOffset > 255)
                    colorOffset = 255;

                if (hotkeys.Abilities[53].PercentReady < 75)
                {
                    _mnkHelper.txt_Current.Foreground =
                        new SolidColorBrush(Color.FromRgb(255, 0, 255));
                    lockIcon = true;
                }
                else
                {
                    lockIcon = false;
                    _mnkHelper.txt_Current.Foreground = new SolidColorBrush(Color.FromRgb((byte)(255 - colorOffset), (byte)colorOffset, 0));
                }
                

                if (user.ContainsStatusEffect(110, 0, false))
                {
                    if (position == ePosition.Side)
                        highlightedAbility = hotkeys.Abilities[56]; // Snap Punch
                    else if (position == ePosition.Back)
                        highlightedAbility = hotkeys.Abilities[54]; // True Strike
                    else
                        highlightedAbility = hotkeys.Abilities[56]; // Snap Punch


                    if (hotkeys.Abilities[53].PercentReady > 50 && lockIcon == false)
                    {
                        SetCurrentAbilityImage(highlightedAbility.ID, recast, true, dotNeeded, dragonkickNeeded);
                    }
                   
                }
                else
                {
                    foreach (KeyValuePair<short, Hotkeys.Hotkey> hk in hotkeys.Abilities)
                    {
                        if (hk.Value.Highlighted)
                        {
                            if (user.ContainsStatusEffect(101, 0, false) == false && hk.Value.ID == 61 && user.TP_Current >= 60) // Twin Snakes if damage buff is down
                            {
                                highlightedAbility = hk.Value;
                                break;
                            }
                            else if (hk.Value.ID == 58 && user.TP_Current >= 40) // Haymaker
                            {
                                highlightedAbility = hk.Value;
                                break;
                            }
                            else if (position == ePosition.Side)
                            {
                                if (hk.Value.ID == 61 && user.TP_Current >= 60) // Twin Snakes
                                {
                                    highlightedAbility = hk.Value;
                                }
                                else if (hk.Value.ID == 56 && user.TP_Current >= 50) // Snap Punch
                                {
                                    highlightedAbility = hk.Value;
                                }

                            }
                            else if (position == ePosition.Back)
                            {
                                if (hk.Value.ID == 54 && user.TP_Current >= 50) // True Strike
                                {
                                    highlightedAbility = hk.Value;
                                }
                                else if (hk.Value.ID == 66 && monster.ContainsStatusEffect(246, user.ID, true) == false && user.TP_Current >= 50) // Demolish DOT
                                {
                                    highlightedAbility = hk.Value;
                                }
                                else if (hk.Value.ID == 56 && user.TP_Current >= 50) // Snap Punch
                                {
                                    if (highlightedAbility != null)
                                        if (highlightedAbility.ID == 66)
                                            continue;

                                    highlightedAbility = hk.Value;
                                }
                            }
                            else
                            {
                                if (hk.Value.ID != 72 && hk.Value.ID != 36 && user.TP_Current >= 60)
                                {
                                    highlightedAbility = hk.Value;
                                }

                            }
                        }
                    }

                    // Check for highlighted abilities if there is no position combos available.
                    if (highlightedAbility == null)
                    {
                        foreach (KeyValuePair<short, Hotkeys.Hotkey> hk in hotkeys.Abilities)
                        {
                            if (hk.Value.Highlighted)
                            {
                                if (user.ContainsStatusEffect(101, 0, false) == false && hk.Value.ID == 61 && user.TP_Current >= 60)
                                    // Twin Snakes if damage buff is down
                                {
                                    highlightedAbility = hk.Value;
                                    break;
                                }

                                if (hk.Value.ID == 66 && monster.ContainsStatusEffect(246, user.ID, true) == false && user.TP_Current >= 50)
                                    // Demolish DOT
                                {
                                    highlightedAbility = hk.Value;
                                    break;
                                }

                                if (hk.Value.ID != 72 && hk.Value.ID != 36 && user.TP_Current >= 60)
                                    highlightedAbility = hk.Value;
                            }
                        }

                    }

                    if (highlightedAbility == null)
                    {
                        if (lockIcon == false)
                        {
                            if (monster.ContainsStatusEffect(106, user.ID, true) == false)
                            {
                                SetCurrentAbilityImage(68, recast, false, dotNeeded, dragonkickNeeded);
                                    // Touch of Death

                            }
                            else if (position == ePosition.Back)
                            {
                                SetCurrentAbilityImage(53, recast, false, dotNeeded, dragonkickNeeded);
                            }
                            else if (position == ePosition.Side)
                            {
                                SetCurrentAbilityImage(74, recast, false, dotNeeded, dragonkickNeeded);
                            }
                            else
                            {
                                SetCurrentAbilityImage(53, recast, false, dotNeeded, dragonkickNeeded);
                            }
                        }
                    }
                    else
                    {
                        if (hotkeys.Abilities[53].PercentReady > 75 && lockIcon == false)
                        {
                            SetCurrentAbilityImage(highlightedAbility.ID, recast, false, dotNeeded, dragonkickNeeded);
                        }
                    }
                }


                
                if (recast.SubAbilities.Contains(80) == false && user.TP_Current < 450)
                {
                    hotkeys.Abilities[80].UseAbility(); // Invigorate
                }
                else if (hotkeys.Abilities[53].PercentReady > 80 && hotkeys.Abilities[53].InRange == 1)
                {
                    if (highlightedAbility != null) // Use Combo
                    {
                        highlightedAbility.UseAbility();
                    }
                    else if (monster.ContainsStatusEffect(106, user.ID, true) == false && user.TP_Current >= 80) // Touch of Death
                    {
                        hotkeys.Abilities[68].UseAbility();
                    }
                    else if (position == ePosition.Back && user.TP_Current >= 60)
                    {
                        hotkeys.Abilities[53].UseAbility(); // Start Bootshine Combo
                    }
                    else if (position == ePosition.Side && user.TP_Current >= 60)
                    {
                        hotkeys.Abilities[74].UseAbility(); // Start Dragonkick
                    }
                    else if(user.TP_Current >= 60)
                    {
                        hotkeys.Abilities[53].UseAbility();
                    }
                }
                else if (hotkeys.Abilities[53].InRange == 1)
                {
                    if (recast.Abilities.Contains(3) == false || recast.SubAbilities.Contains(85) == false)
                    {
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.Dash);
                    }
                    else if (recast.Abilities.Contains(8) == false && user.ContainsStatusEffect(112, 0, false) == false && user.ContainsStatusEffect(113, 0, false) == false && user.Level >= 50)
                    {
                        hotkeys.Abilities[69].UseAbility(); // Perfect Balance
                    }
                    else if (recast.Abilities.Contains(5) == false) 
                    {
                        hotkeys.Abilities[64].UseAbility(); // Steel Peak
                    }
                    else if (recast.Abilities.Contains(7) == false)
                    {
                        hotkeys.Abilities[67].UseAbility(); // Howling Fist
                    }
                    else if (hotkeys[36].PercentReady >= 99 && hotkeys[36].IsInHotbar && hotkeys[36].Activated) // Mercy Stroke
                    {
                        hotkeys[36].UseAbility();
                    }

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.StackTrace.ToString());
            }


        }

        public override void Reset()
        {
            base.Reset();

            Recast recast = new Recast();
            recast.Refresh();
            SetCurrentAbilityImage(0, recast, false, false, false);
        }

        private void SetCurrentAbilityImage(short id, Recast recast, bool perfectBalance, bool dotNeeded, bool dragonkickNeeded)
        {

            if (perfectBalance)
            {
                _mnkHelper.txt_Recommendation.Content = "Perfect Balance Active";
            } else if (dotNeeded)
            {
                _mnkHelper.txt_Recommendation.Content = "Demolish Needed (BACK)";
            }
            else if (dragonkickNeeded)
            {
                _mnkHelper.txt_Recommendation.Content = "Dragon Kick Needed (SIDE)";
            }
            else
            {
                _mnkHelper.txt_Recommendation.Content = "";
            }

            switch (id)
            {
                case 53:
                     _mnkHelper.img_Current.Source = new BitmapImage(new Uri("pack://application:,,/Resources/bootshine.png"));
                    _mnkHelper.txt_Current.Content = "Bootshine";
                    break;
                case 54:
                    _mnkHelper.img_Current.Source = new BitmapImage(new Uri("pack://application:,,/Resources/true strike.png"));
                    _mnkHelper.txt_Current.Content = "True Strike";
                    break;
                case 74:
                    _mnkHelper.img_Current.Source = new BitmapImage(new Uri("pack://application:,,/Resources/dragon kick.png"));
                    _mnkHelper.txt_Current.Content = "Dragon Kick";
                    break;
                case 61:
                    _mnkHelper.img_Current.Source = new BitmapImage(new Uri("pack://application:,,/Resources/twinsnakes.png"));
                    _mnkHelper.txt_Current.Content = "Twin Snakes (BUFF)";
                    break;
                case 56:
                    _mnkHelper.img_Current.Source = new BitmapImage(new Uri("pack://application:,,/Resources/snap punch.png"));
                    _mnkHelper.txt_Current.Content = "Snap Punch";
                    break;
                case 66:
                    _mnkHelper.img_Current.Source = new BitmapImage(new Uri("pack://application:,,/Resources/demolish.png"));
                    _mnkHelper.txt_Current.Content = "Demolish (DOT)";
                    break;
                case 70:
                    _mnkHelper.img_Current.Source = new BitmapImage(new Uri("pack://application:,,/Resources/rockbreaker.png"));
                    _mnkHelper.txt_Current.Content = "Rock Breaker";
                    break;
                case 68:
                    _mnkHelper.img_Current.Source = new BitmapImage(new Uri("pack://application:,,/Resources/touch of death.png"));
                    _mnkHelper.txt_Current.Content = "Touch of Death (DOT)";
                    break;
                case 58:
                    //_mnkHelper.img_Current.Source = new BitmapImage(new Uri("pack://application:,,/Resources/haymaker.png"));
                    //_mnkHelper.txt_Current.Content = "Haymaker";
                    break;
                default:
                    _mnkHelper.img_Current.Source = null;
                    _mnkHelper.txt_Current.Content = "";
                    break;
            }

        }
    }
}
