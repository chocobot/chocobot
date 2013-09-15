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
using System.Windows.Shapes;
using System.Windows.Threading;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Character;
using Chocobot.Utilities.Memory;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgStunBot.xaml
    /// </summary>
    public partial class dlgStunBot : Window
    {

        private readonly DispatcherTimer _stunner = new DispatcherTimer();

        public dlgStunBot()
        {
            InitializeComponent();

            _stunner.Tick += Stunner_Tick;
            _stunner.Interval = new TimeSpan(0,0,0,0,100);
        }

        private void Stunner_Tick(object sender, EventArgs e)
        {
            List<Character> monsters = new List<Character>();
            List<Character> fate = new List<Character>();
            List<Character> players = new List<Character>();
            Character user = null;

            MemoryFunctions.GetCharacters(monsters, fate, players, ref user);

            monsters.AddRange(fate);


            foreach (Character currmonster in monsters)
            {
                if(user.TargetID == currmonster.ID)
                {
                    lbl_CurrAbility.Content = currmonster.UsingAbilityID.ToString("X");

                    if(currmonster.UsingAbilityID.ToString("X").ToLower() == txt_Ability.Text.ToLower())
                    {
                        Utilities.Keyboard.KeyBoardHelper.KeyDown(Keys.ControlKey);
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(Keys.D0);
                        Utilities.Keyboard.KeyBoardHelper.KeyUp(Keys.ControlKey);
                    }
                }
            }


        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            _stunner.Start();
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            _stunner.Stop();
        }
    }
}
