using System;
using System.Windows;
using System.Windows.Threading;
using Chocobot.Utilities.Input;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgRadar.xaml
    /// </summary>
    public partial class dlgRadar : Window
    {

        private readonly DispatcherTimer thread_Refresh;
        private string _filter = "";

        public dlgRadar()
        {
            InitializeComponent();

            thread_Refresh = new DispatcherTimer();

            thread_Refresh.Tick += thread_Refresh_Tick;
            thread_Refresh.Interval = new TimeSpan(0,0,0,0,100);
            thread_Refresh.Start();
            Background = null;

        }

        private void thread_Refresh_Tick(object sender, EventArgs e)
        {
            vp_radar.Refresh();
        }

        private void btn_Config_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            } catch
            {
                
            }
        }

        private void mnu_TogglePlayers(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowPlayers = !vp_radar.ShowPlayers;
        }

        private void mnu_ToggleMonsters(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowMonsters = !vp_radar.ShowMonsters;
        }

        private void mnu_ToggleHunts(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowHunts = !vp_radar.ShowHunts;
        }
        
        private void mnu_ToggleGathering(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowGathering = !vp_radar.ShowGathering;
        }

        private void mnu_ToggleNPCs(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowNPCs = !vp_radar.ShowNPCs;
        }

        private void mnu_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mnu_ToggleMonsterName(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowMonsterName = !vp_radar.ShowMonsterName;
        }
        
        private void mnu_ToggleHuntsName(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowHuntsName = !vp_radar.ShowHuntsName;
        }
        
        private void mnu_TogglePlayerNames(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowPlayerName = !vp_radar.ShowPlayerName;
        }

        private void mnu_ToggleGatheringNames(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowGatheringName = !vp_radar.ShowGatheringName;
        }

        private void mnu_ToggleNPCNames(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowNPCName = !vp_radar.ShowNPCName;
        }

        private void mnu_Filter_Click(object sender, RoutedEventArgs e)
        {
            clsInput input = new clsInput("Please enter the name to filter.");

            vp_radar.Filter = input.Show().ToLower();
        }


        private void mnu_Scale50_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 510 / 2;
            this.Height = 510 / 2;
        }

        private void mnu_Scale100_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 510;
            this.Height = 510;
        }

        private void mnu_Scale200_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 510 * 2;
            this.Height = 510 * 2;
        }

        private void dlgRadar_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            thread_Refresh.Stop();
        }


        private void mnu_ToggleHidden(object sender, RoutedEventArgs e)
        {
            vp_radar.ShowHidden = !vp_radar.ShowHidden;

        }

        private void mnu_ToggleCompass(object sender, RoutedEventArgs e)
        {
            vp_radar.CompassMode = !vp_radar.CompassMode;
        }
    }
}
