// ChatEntry.cs


using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Chocobot.MemoryStructures.Chat
{
    public class ChatEntry
    {
        public bool JP;

        /// <summary>
        /// </summary>
        /// <param name="raw"> </param>
        public ChatEntry(byte[] raw)
        {
            Bytes = raw;
            Raw = Encoding.UTF8.GetString(raw.ToArray());
            var cut = (Raw.Substring(13, 1) == ":") ? 14 : 13;
            var cleaner = new ChatCleaner(raw, CultureInfo.CurrentUICulture, out JP);
            var cleaned = cleaner.Result;
            Line = XmlHelper.SanitizeXmlString(cleaned.Substring(cut));
            Code = Raw.Substring(8, 4);
            Combined = String.Format("{0}:{1}", Code, Line);
            TimeStamp = UnixTimeStampToDateTime(Int32.Parse(Raw.Substring(0, 8), NumberStyles.HexNumber));
        }

        public DateTime TimeStamp { get; private set; }
        public string Code { get; private set; }
        public string Line { get; private set; }
        public string Combined { get; private set; }
        public string Raw { get; private set; }
        public byte[] Bytes { get; private set; }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp)
                                   .ToLocalTime();
            return dtDateTime;
        }
    }
}