using System.Windows;
using Chocobot.Utilities.Navigation;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgNavigation.xaml
    /// </summary>
    public partial class dlgNavigation : MetroWindow
    {

        private readonly NavigationHelper _navigation = new NavigationHelper();

        public dlgNavigation()
        {
            InitializeComponent();

            _navigation.WaypointIndexChanged += WaypointIndex_Changed;

            lst_Coordinates.ItemsSource = _navigation.Waypoints;

        }

        private void WaypointIndex_Changed(object sender, int index)
        {
            lst_Coordinates.SelectedIndex = index;
        }


        private void btn_Record_Click(object sender, RoutedEventArgs e)
        {

            if((string) btn_Record.Content == "Record")
            {
                btn_Record.Content = "Stop";
                _navigation.Record();
            } else
            {
                btn_Record.Content = "Record";
                _navigation.StopRecording();
            }


        }

        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            if ((string)btn_Play.Content == "Play")
            {
                _navigation.Start();
                btn_Play.Content = "Stop";
            }
            else
            {
                _navigation.Stop();
                btn_Play.Content = "Play";
            }

        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Title = "Save Navigation";
            dlg.Filter = "Navigation File (*.nav)|*.nav";

            if (dlg.ShowDialog() == false)
                return;

            _navigation.Save(dlg.FileName);

        }

        private void btn_Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Title = "Open Navigation";
            dlg.Filter = "Navigation File (*.nav)|*.nav";

            if (dlg.ShowDialog() == false)
                return;

            _navigation.Load(dlg.FileName);
        }
    }
}
