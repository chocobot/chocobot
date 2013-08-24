using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chocobot.Dialogs;

namespace Chocobot.Utilities.Input
{
    class clsInput
    {

        private static string _result;
        private static dlgInputBox _dlg = null;

        private static void ResultCallback(string result)
        {
            _result = result;
        }

        public clsInput(string question)
        {

            if (_dlg != null)
            {
                _dlg.Close();
                _dlg = null;
            }

            _result = "";
            _dlg = new dlgInputBox(question, ResultCallback);

        }

        public string Show()
        {
            _result = "";
            _dlg.ShowDialog();
            return _result;
    
        }
    }
}
