
namespace Chocobot.Datatypes
{

    public enum CharacterType
    {
        NPC,
        PC,
        Monster,
        Gathering
    }

    public enum CharacterStatus
    {
        Idle=2,
        Dead = 4,
        Fighting = 6,

        Fishing_Idle=0x23,

        Fishing_Cast1 = 0x26,
        Fishing_Cast6 = 0x27,
        Fishing_Cast2 = 0x28,
        Fishing_Cast3 = 0x29,
        Fishing_Cast4 = 0x2C,
        Fishing_Cast5 = 0x2A,
        Fishing_Cast7 = 0x2F,

        Fishing_FishOnHook = 0x39,
        Fishing_FishOnHook4 = 0x38,
        Fishing_FishOnHook5 = 0x3A,

        Fishing_ReelingIn = 0x30,
        Fishing_ReelingIn2 = 0x31,
        Fishing_ReelingIn3 = 0x33,
        Fishing_ReelingInBig = 0x32,
        Fishing_ReelingInBig2 = 0x34,

        Fishing_NoBite = 0x2F,
        Unknown=0

    }

}
