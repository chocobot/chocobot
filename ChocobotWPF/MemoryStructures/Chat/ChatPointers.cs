    // ChatPointers.cs


using System.Runtime.InteropServices;

namespace Chocobot.MemoryStructures.Chat
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct ChatPointers
    {
        public uint LineCount1;

        //public uint LineCount2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]

        public byte[] Unk1;
        public uint OffsetArrayStart;
        public uint OffsetArrayPos;
        public uint OffsetArrayEnd;
        public uint Unk2;
        public uint LogStart;
        public uint LogNext;
        public uint LogEnd;
        public uint Unk3;
        public uint OldLogStart;
        public uint OldLogNext;
        public uint OldLogEnd;
    }

    //[StructLayout(LayoutKind.Sequential)]
    //internal struct ChatPointers
    //{
    //    public uint LineCount1;
    //    //public uint LineCount2;

    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
    //    public byte[] Unk1;

    //    public uint OffsetArrayStart;
    //    public uint OffsetArrayPos;
    //    public uint OffsetArrayEnd;
    //    public uint Unk2;
    //    public uint Unk3;
    //    public uint Unk4;
    //    public uint LogStart;
    //    public uint LogNext;
    //    public uint LogEnd;
    //    public uint Unk5;
    //    public uint pOldChat2Unk1;
    //    public uint pOldChat2Unk2;
    //    public uint pOldChat2Unk3;
    //}
}