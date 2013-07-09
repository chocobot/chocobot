using System;
using System.Windows.Threading;
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
    }
}
