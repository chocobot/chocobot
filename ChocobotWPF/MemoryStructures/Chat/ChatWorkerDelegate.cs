// ChatWorkerDelegate.cs


using System;
using ChocobotWPF;

namespace Chocobot.MemoryStructures.Chat
{
    internal static class ChatWorkerDelegate
    {
        /// <summary>
        /// </summary>
        public static void OnNewLine(ChatEntry chatEntry)
        {
            var entry = new object[] {chatEntry.Bytes, chatEntry.Code, chatEntry.Combined, chatEntry.JP, chatEntry.Line, chatEntry.Raw, chatEntry.TimeStamp};
        }
    }
}