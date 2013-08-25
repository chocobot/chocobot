using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Chocobot.Utilities.FileIO;
using Chocobot.Utilities.Input;
using System.Security.Cryptography;

namespace Chocobot.Utilities.Licensing
{
    class LicenseManager
    {
        public static LicenseManager Instance = new LicenseManager();
        private readonly string _user;
        private readonly string _pass;


        public class LicenseResult
        {
            public bool Valid;
            public string Error;
        }
        public LicenseManager()
        {
            //IniParser ini = new IniParser(@"chocobot.ini");
            IniParserLegacy.IniFile ini = new IniParserLegacy.IniFile(System.Windows.Forms.Application.StartupPath + "\\chocobot.ini");

            string user = "";
            string password = "";

            try
            {
                user = ini.IniReadValue("Credentials", "UserName");
                password = ini.IniReadValue("Credentials", "Password");
            }
            catch (Exception)
            {
            }

            if(user == "" || password == "")
            {

                clsCredentials userinput = new clsCredentials();
                clsCredentials.Credentials credentials = userinput.Show(); ;
                user = credentials.User;
                password = GetMd5Hash(credentials.Password);
            }

            _user = user;
            _pass = password;

        }

        public LicenseResult VerifyLicense()
        {
            Random rnd = new Random();
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            LicenseResult licenseResult = new LicenseResult();
            
            int randInt = rnd.Next(124, 18724);
            string randIntHash = GetMd5Hash("S!k@l" + (randInt + 1563).ToString(CultureInfo.InvariantCulture));
            string htmlResult = new System.Net.WebClient().DownloadString("http://www.chocobotxiv.com/forum/licensing/action.php?user=" + _user + "&pass=" + _pass + "&action=1&session=" + randInt + "&version=" + fvi.FileMajorPart + "." + fvi.FileMinorPart);

            bool result = htmlResult == randIntHash;

            if(result)
            {
                //IniParser ini = new IniParser(@"chocobot.ini");
                IniParserLegacy.IniFile ini = new IniParserLegacy.IniFile(System.Windows.Forms.Application.StartupPath + "\\chocobot.ini");

                ini.IniWriteValue("Credentials", "UserName", _user);
                ini.IniWriteValue("Credentials", "Password", _pass);
                //ini.SaveSettings();
            } else
            {
                licenseResult.Error = htmlResult.Trim();
            }

            licenseResult.Valid = result;

            return licenseResult;

        }


        static string GetMd5Hash(string input)
        {

            MD5 md5Hash = MD5.Create();
            
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

    }
}
