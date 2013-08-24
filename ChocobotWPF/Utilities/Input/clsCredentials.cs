using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chocobot.Dialogs;

namespace Chocobot.Utilities.Input
{
    class clsCredentials
    {

        public class Credentials
        {
            public string User;
            public string Password;
        }

        private static Credentials _result = new Credentials();
        private static dlgCredentials _dlg = null;

        private static void ResultCallback(string username, string password)
        {
            _result.User = username;
            _result.Password = password;
        }

        public clsCredentials()
        {

            if (_dlg != null)
            {
                _dlg.Close();
                _dlg = null;
            }

            _result = new Credentials();
            _dlg = new dlgCredentials(ResultCallback);

        }

        public Credentials Show()
        {
 
            _dlg.ShowDialog();
            return _result;
    
        }
    }
}
