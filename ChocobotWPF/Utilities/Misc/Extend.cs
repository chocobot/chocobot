using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chocobot.MemoryStructures.Character;

namespace Chocobot.Utilities.Misc
{
    public static class Extend
    {
        public static double StandardDeviation(this List<Character> values)
        {
            double avg = values.Average(a => (a.Health_Max - a.Health_Current));
            return Math.Sqrt(values.Average(v => Math.Pow((v.Health_Max - v.Health_Current) - avg, 2)));
        }
    }
}
