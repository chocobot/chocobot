using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chocobot.Datatypes;
using Chocobot.Dialogs;

namespace Chocobot.Utilities.Input
{
    class clsCraftingKey
    {

        private static CraftingKey _result = new CraftingKey();
        private static dlgCraftKeyConfig _dlg = null;

        private static void ResultCallback(CraftingKey result)
        {
            _result = result;

        }

        public clsCraftingKey()
        {

            if (_dlg != null)
            {
                _dlg.Close();
                _dlg = null;
            }

            _result = new CraftingKey();
            _dlg = new dlgCraftKeyConfig(ResultCallback);

        }

        public CraftingKey Show()
        {
 
            _dlg.ShowDialog();
            return _result;
    
        }
    }
}
