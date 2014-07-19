using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Chocobot.Datatypes;
using Chocobot.Utilities.Memory;

namespace Chocobot.Utilities.Chatlog
{
    public partial class Chatlog
    {
        private readonly List<Entry> buffer = new List<Entry>();
        private readonly List<int> offsets_list = new List<int>();
        private int currentOffset;
        private int previousArrayIndex;
        private int previousOffset;
        private CHATLOGINFO structure;
        private IntPtr address;

        public Chatlog(CHATLOGINFO _structure, IntPtr _address)
        {
            structure = _structure;
            address = _address;
        }

        /// <summary>
        ///     Updates our offsets list
        /// </summary>
        private void updateOffsetArray()
        {
            offsets_list.Clear();

            for (int i = 0; i < Constants.CHATLOG_ARRAY_SIZE; i++)
                offsets_list.Add((int) MemoryHandler.Instance.ResolvePointer((IntPtr) (structure.ArrayStart + (i*0x4))));
        }

        /// <summary>
        ///     Reads a single line and create an Entry out of it
        /// </summary>
        /// <param name="start">Start offset of the line</param>
        /// <param name="end">End offset of the line</param>
        /// <returns>Entry instance</returns>
        private Entry ReadEntry(int start, int end)
        {
            int bytesread;

            var cle =
                new Entry(MemoryHandler.Instance.ReadAdress((IntPtr)(structure.LogStart + start), (uint)(end - start)));
            return cle;
        }

        /// <summary>
        ///     This is a wrapper around ReadEntry
        ///     in order to create a List of Entry out
        ///     of all the lines we haven't processed yet.
        /// </summary>
        /// <param name="start">Array index to start at</param>
        /// <param name="end">Array index to stop at</param>
        /// <returns>List of Entry instances</returns>
        private List<Entry> ReadEntries(int start, int end)
        {
            var ret = new List<Entry>();
            for (int i = start; i < end; i++)
                {
                    currentOffset = offsets_list[i];
                    ret.Add(ReadEntry(previousOffset, currentOffset));
                    previousOffset = currentOffset;
                }
            return ret;
        }

        /// <summary>
        ///     Is there a new line?
        /// </summary>
        /// <returns></returns>
        public bool isNewLine()
        {
            updateBuffer();
            return (buffer.Count > 0);
        }

        /// <summary>
        ///     This updates our internal buffer.
        /// </summary>
        private void updateBuffer()
        {
            updateOffsetArray();
            structure = MemoryHandler.Instance.CreateStructFromAddress<CHATLOGINFO>(address);
            int currentArrayIndex = (structure.ArrayCurrent - structure.ArrayStart)/4;

            // I forgot why we did this
            if (currentArrayIndex < previousArrayIndex)
                {
                    buffer.AddRange(ReadEntries(previousArrayIndex, (int) Constants.CHATLOG_ARRAY_SIZE));
                    previousOffset = 0;
                    previousArrayIndex = 0;
                }
            if (previousArrayIndex < currentArrayIndex)
                buffer.AddRange(ReadEntries(previousArrayIndex, currentArrayIndex));
            previousArrayIndex = currentArrayIndex;
        }

        /// <summary>
        ///     This returns a copy of our buffer, and clear it.
        /// </summary>
        /// <returns>List of Entry instances</returns>
        public List<Entry> getChatLogLines()
        {
            updateBuffer();
            List<Entry> newList = buffer.ToList();
            buffer.Clear();
            return newList;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct CHATLOGINFO
        {
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0)] public int Count;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x20)] public int ArrayStart;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x24)] public int ArrayCurrent;
            [MarshalAs(UnmanagedType.I4)] [FieldOffset(0x30)] public int LogStart;
        };

        /// <summary>
        ///     This function instantiates a Chatlog object
        /// </summary>
        /// <returns>Chatlog instance</returns>
        public static Chatlog getInstance()
        {
            IntPtr pointer = MemoryHandler.Instance.ResolvePointerPath(Constants.CHATPTR);
            var c = new Chatlog(MemoryHandler.Instance.CreateStructFromAddress<Chatlog.CHATLOGINFO>(pointer), pointer);
            return c;
        }
    }


}