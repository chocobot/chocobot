using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Chocobot.Datatypes;

namespace Chocobot.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgInputBox.xaml
    /// </summary>
    public partial class dlgCraftKeyConfig : MetroWindow
    {
        public delegate void ResultDelegate(CraftingKey result);

        private readonly ResultDelegate _callback;


        public dlgCraftKeyConfig(ResultDelegate callback)
        {
            InitializeComponent();
            _callback = callback;

            lst_Key.ItemsSource = Enum.GetValues(typeof(Keys)).Cast<Keys>();
            lst_Key.SelectedIndex = 0;
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            CraftingKey craftkey = new CraftingKey();

            craftkey.Key = (Keys)Enum.Parse(typeof(Keys), lst_Key.Text, true);
            craftkey.CPCondition = chk_CP.IsChecked == true;
            craftkey.DurabilityCondition = chk_Durability.IsChecked == true;
            craftkey.ProgressCondition = chk_Progress.IsChecked == true;
            craftkey.CP = short.Parse(txt_CP.Text);
            craftkey.Progress = short.Parse(txt_Progress.Text);
            craftkey.Durability = short.Parse(txt_Durability.Text);

            _callback.Invoke(craftkey);
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
