using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Chocobot.Utilities.Memory
{
    public class MemoryLocations2
    {

        public static Dictionary<string, uint> Database = new Dictionary<string, uint>();

        public static void GetMemlocs()
        {
            Database.Add("charmap", FindByteString("00-DB-0F-C9-3F-6F-12-83-3A-..-..-..-..") + 13);  //+205
            Database.Add("npcmap", FindByteString("05-00-00-00-00-00-00-00-00-01-5B-70-61-5D-00") + 88);
            Database.Add("target", FindByteString("00-00-05-00-00-00-..-..-..-..-00-00-00-00-00-00-00-00-..-..-..-..-..-..-..-..-04") + 126);
            //Database.Add("chatlog", FindByteString("C0-F9-E4-00-6F-01-A8-00-00-00-00-00"));
            Database.Add("chatlog", FindByteString("40-00-00-00-06-00-00-00-00-00-00-00-00-01-02"));
            Database.Add("recast", FindByteString("00-DB-0F-C9-3F-6F-12-83-3A-52-07-0A-01") + 737);
            Database.Add("recast_ws", FindByteString("00-DB-0F-C9-3F-6F-12-83-3A-52-07-0A-01") + 1637);
            Database.Add("aggro", 13181908 + MemoryHandler.Instance.BaseAddress);
        }

        public static uint FindByteString(string search)
        {
            uint Address = 0;
            try
            {
                Match match1 = null;
                Regex regex1 = new Regex(search);
                const int SearchSize = 229376;

                int num2 = MemoryHandler.GetModuleEndAddress(MemoryHandler.Instance.Process);
                System.Diagnostics.Debug.Print(MemoryHandler.Instance.BaseAddress.ToString("X") + "+" + num2.ToString("X"));
                MemoryHandler.Instance.Address = MemoryHandler.Instance.BaseAddress;

                do
                {

                    string Pattern = MemoryHandler.Instance.GetOpcode(SearchSize);
                    
                    match1 = regex1.Match(Pattern);
                    if (match1.Success)
                    {
                        break; 
                    }

                    MemoryHandler.Instance.Address = (MemoryHandler.Instance.Address + SearchSize);

                } while ((!match1.Success & (MemoryHandler.Instance.Address < num2)));

                Address = (uint)((match1.Index / 3) + MemoryHandler.Instance.Address);
            }
            catch (Exception ex)
            {
                Address = 0;
            }
            return Address;
        }

        public static uint FindMemloc2(string strPrefix, string strSuffix)
        {
            uint Address = 0;
            try
            {
                Match match1 = null;
                Regex regex1 = new Regex((strPrefix + "-([0-9|A-F][0-9|A-F])-([0-9|A-F][0-9|A-F])-([0-9|A-F][0-9|A-F])-([0-9|A-F][0-9|A-F])-" + strSuffix));
                const int StartAddress = 229376;

                int num2 = MemoryHandler.GetModuleEndAddress(MemoryHandler.Instance.Process);
                MemoryHandler.Instance.Address = MemoryHandler.Instance.BaseAddress;

                do
                {
                    string Pattern = MemoryHandler.Instance.GetOpcode(StartAddress);
                    match1 = regex1.Match(Pattern);
                    if (match1.Success)
                    {
                        break; // TODO: might not be correct. Was : Exit Do
                    }
                    MemoryHandler.Instance.Address = (MemoryHandler.Instance.Address + StartAddress);

                } while ((!match1.Success & (MemoryHandler.Instance.Address < num2)));

                Address = (uint)((match1.Index / 3) + MemoryHandler.Instance.Address);
            }
            catch (Exception ex)
            {
                Address = 0;
            }
            return Address;
        }

        public static int FindMemloc(string strPrefix, string strSuffix)
        {
            Match m = null;
            int Address = 0;
            Regex regex1 = new Regex((strPrefix + "-([0-9|A-F][0-9|A-F])-([0-9|A-F][0-9|A-F])-([0-9|A-F][0-9|A-F])-([0-9|A-F][0-9|A-F])-" + strSuffix));
            const int blockSize = 229376;


            int endAddress = MemoryHandler.GetModuleEndAddress(MemoryHandler.Instance.Process);
            MemoryHandler.Instance.Address = MemoryHandler.Instance.BaseAddress;

            do
            {
                string text1 = MemoryHandler.Instance.GetOpcode(blockSize);
                m = regex1.Match(text1);
                MemoryHandler.Instance.Address = (MemoryHandler.Instance.Address + blockSize);
            } while ((!m.Success & (MemoryHandler.Instance.Address < endAddress)));
            try
            {
                Address = Int32.Parse((m.Groups[4].ToString() + m.Groups[3].ToString() + m.Groups[2].ToString() + m.Groups[1].ToString()), NumberStyles.HexNumber);
            }
            catch (Exception ex)
            {
                Address = 0;
            }
            return Address;
        }
    }

}
