using System;
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
            thread_Refresh.Interval = new TimeSpan(0, 0, 0, 0, 100);
            thread_Refresh.Start();

        }

        private void thread_Refresh_Tick(object sender, EventArgs e)
        {
            vp_map.Refresh();
        }

        private void btn_RecordNav_Click(object sender, System.Windows.RoutedEventArgs e)
        {

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
            if ((string)btn_PlayPath.Content == "Play")
            {
                btn_PlayPath.Content = "Stop";
                vp_map.PlaySelectedPath();
            }
            else
            {
                btn_PlayPath.Content = "Play";
                vp_map.StopSelectedPath();
            }
        }

        private void btn_SaveNav_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vp_map.SaveNav();
        }
    }
}
