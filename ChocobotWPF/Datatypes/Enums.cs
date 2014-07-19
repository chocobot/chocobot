
namespace Chocobot.Datatypes
{

    public enum CharacterType
    {
        NPC,
        PC,
        Monster,
        Gathering
    }
    
    //1 crafting   1A basic synth   1B basic Touch
    public enum ePosition
    {
        Front,
        Back,
        Side
    }
    public enum JOB : byte
    {
        // ReSharper disable InconsistentNaming
        NPC = 0x0,
        GLD = 0x1,
        PGL = 0x2,
        MRD = 0x3,
        LNC = 0x4,
        ARC = 0x5,
        CNJ = 0x6,
        THM = 0x7,
        CPT = 0x8,
        BSM = 0x9,
        ARM = 0xA,
        GSM = 0xB,
        LTW = 0xC,
        WVR = 0xD,
        ALC = 0xE,
        CUL = 0xF,
        MIN = 0x10,
        BOT = 0x11,
        FSH = 0x12,
        PLD = 0x13,
        MNK = 0x14,
        WAR = 0x15,
        DRG = 0x16,
        BRD = 0x17,
        WHM = 0x18,
        BLM = 0x19,
        ACN = 0x1A,
        SMN = 0x1B,
        SCH = 0x1C,
        Chocobo = 0x1D,
        Pet = 0x1E
    }

    public enum CharacterStatus
    {
        Crafting_Idle = 1,
        Crafting_Idle2 = 5,

        Idle = 2,
        Dead = 4,
        Fighting = 6,

        Crafting_BasicSynth = 0x1A,
        Crafting_BasicTouch = 0x1B,

        Fishing_Idle=0x23,

        Fishing_Cast1 = 0x26,
        Fishing_Cast6 = 0x27,
        Fishing_Cast2 = 0x28,
        Fishing_Cast3 = 0x29,
        Fishing_Cast4 = 0x2C,
        Fishing_Cast5 = 0x2A,
        Fishing_Cast7 = 0x2F,
        Fishing_Cast8 = 0x2B,
        Fishing_Cast9 = 0x2E,
        Fishing_Cast10 = 0x2D,

        Fishing_FishOnHook = 0x39,
        Fishing_FishOnHook4 = 0x38,
        Fishing_FishOnHook5 = 0x3A,

        Fishing_ReelingIn = 0x30,
        Fishing_ReelingIn2 = 0x31,
        Fishing_ReelingIn3 = 0x33,
        Fishing_ReelingInBig = 0x32,
        Fishing_ReelingInBig2 = 0x34,
        Fishing_ReelingInBig3 = 0x35,

        Fishing_NoBite = 0x2F,
        Unknown = 0

    }

}
