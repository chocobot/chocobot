using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgInputBox.xaml
    /// </summary>
    public partial class dlgCredentials : MetroWindow
    {
        public delegate void ResultDelegate(string username, string password);

        private readonly ResultDelegate _callback;


        public dlgCredentials(ResultDelegate callback)
        {
            InitializeComponent();
            _callback = callback;
            txt_Username.Focus();

        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            _callback.Invoke(txt_Username.Text, txt_Password.Password);
            this.DialogResult = true;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void dlg_InputBox_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txt_Input_Keyup(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _callback.Invoke(txt_Username.Text, txt_Password.Password);
                this.DialogResult = true;
            }
        }



    }
}
