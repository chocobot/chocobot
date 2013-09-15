using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocobot.Datatypes
{
    public class CraftingKey
    {

        public Keys Key { get; set; }
        public bool CPCondition = false;
        public bool DurabilityCondition = false;
        public bool ProgressCondition = false;
        public short CP = 0;
        public short Durability = 0;
        public short Progress = 0;

        public string ConditionString
        {
            get
            {
                string conditionstr = "";
                
                if(CPCondition)
                {
                    conditionstr += "CP >= " + CP.ToString() + " ";
                }

                if (DurabilityCondition)
                {
                    conditionstr += "Dura >= " + Durability.ToString() + " ";
                }

                if (ProgressCondition)
                {
                    conditionstr += "Progress <= " + Progress.ToString() + " ";
                }

                return conditionstr;
            }
        }

        public string KeyString
        {
            get
            {

                return Key.ToString();
            }
        }



    }
}
