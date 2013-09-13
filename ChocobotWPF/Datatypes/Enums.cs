
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
