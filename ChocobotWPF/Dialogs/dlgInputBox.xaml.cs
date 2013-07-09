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
using MahApps.Metro.Controls;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgInputBox.xaml
    /// </summary>
    public partial class dlgInputBox : MetroWindow
    {
        public delegate void ResultDelegate(string result);

        private readonly ResultDelegate _callback;


        public dlgInputBox(string question, ResultDelegate callback)
        {
            InitializeComponent();
            _callback = callback;
            lbl_Question.Content = question;


        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            _callback.Invoke(txt_Input.Text);
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
    }
}
