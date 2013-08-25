using System;
using System.Windows;
using System.Windows.Threading;
using Chocobot.Datatypes;
using MahApps.Metro.Controls;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgMap.xaml
    /// </summary>
    public partial class dlgMap : MetroWindow
    {

        private readonly DispatcherTimer thread_Refresh;

        public dlgMap()
        {
            InitializeComponent();
            thread_Refresh = new DispatcherTimer();

            thread_Refresh.Tick += thread_Refresh_Tick;
            thread_Refresh.Interval = new TimeSpan(0, 0, 0, 0, 500);
            thread_Refresh.Start();

            vp_map.ControlNavigationFinished += NavigationFinished;

        }

        private void NavigationFinished(object sender)
        {
            btn_PlayPath.Content = "Run";
        }

        private void thread_Refresh_Tick(object sender, EventArgs e)
        {
            vp_map.Refresh();
        }

        private void btn_RecordNav_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            vp_map.ShowPaths = true;

            if ((string)btn_RecordNav.Content == "Record")
            {
                btn_RecordNav.Content = "Stop";
                vp_map.StartRecord();
            }
            else
            {
                btn_RecordNav.Content = "Record";
                vp_map.StopRecording();
            }


            

        }

        private void btn_PlayPath_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((string)btn_PlayPath.Content == "Run")
            {
                btn_PlayPath.Content = "Stop";
                vp_map.PlaySelectedPath();
            }
            else
            {
                btn_PlayPath.Content = "Run";
                vp_map.StopSelectedPath();
            }
        }

        private void btn_SaveNav_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vp_map.SaveNav();
        }

        private void dlgMap_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            thread_Refresh.Stop();
        }

        private void mnu_ToggleSmallMaps_Clicked(object sender, RoutedEventArgs e)
        {

            vp_map.SmallMarkers = !vp_map.SmallMarkers;

        }

        private void mnu_TogglePaths_Clicked(object sender, RoutedEventArgs e)
        {

            vp_map.ShowPaths = !vp_map.ShowPaths;

        }
    }
}
