
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
        Fighting = 6,
        Fishing_Idle=0x23,

        Fishing_Cast1 = 0x26,
        Fishing_Cast2 = 0x28,
        Fishing_Cast3 = 0x29,
        Fishing_Cast4 = 0x2C,

        Fishing_FishOnHook = 0x39,
        Fishing_FishOnHook2 = 0x30,
        Fishing_FishOnHook3 = 0x33,
        Fishing_FishOnHook4 = 0x38,
        Fishing_FishOnHook5 = 0x3A,
        Unknown=0

    }

}
