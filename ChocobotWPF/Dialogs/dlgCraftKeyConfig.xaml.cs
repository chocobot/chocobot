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

            //lst_Key.ItemsSource = Enum.GetValues(typeof(Keys)).Cast<Keys>();
            lst_Key.Items.Add("D1");
            lst_Key.Items.Add("D2");
            lst_Key.Items.Add("D3");
            lst_Key.Items.Add("D4");
            lst_Key.Items.Add("D5");
            lst_Key.Items.Add("D6");
            lst_Key.Items.Add("D7");
            lst_Key.Items.Add("D8");
            lst_Key.Items.Add("D9");
            lst_Key.Items.Add("D0");
            lst_Key.Items.Add("Dash");

            lst_Condition.Items.Add("Poor");
            lst_Condition.Items.Add("Normal");
            lst_Condition.Items.Add("Good");
            lst_Condition.Items.Add("Excellent");

            lst_Condition.SelectedIndex = 3;
            lst_Key.SelectedIndex = 0;
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            CraftingKey craftkey = new CraftingKey();

            craftkey.ControlKey = chk_ControlKey.IsChecked == true;
            craftkey.Key = (Keys)Enum.Parse(typeof(Keys), lst_Key.Text, true);
            craftkey.CPCondition = chk_CP.IsChecked == true;
            craftkey.DurabilityCondition = chk_Durability.IsChecked == true;
            craftkey.ProgressCondition = chk_Progress.IsChecked == true;
            craftkey.ConditionCondition = chk_Condition.IsChecked == true;

            craftkey.CP = short.Parse(txt_CP.Text);
            craftkey.Progress = short.Parse(txt_Progress.Text);
            craftkey.Durability = short.Parse(txt_Durability.Text);
            craftkey.Condition = lst_Condition.Text;

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
