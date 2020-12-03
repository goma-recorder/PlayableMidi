namespace Midity
{
    public enum NoteKey
    {
        CFlatMajor = -7,
        AFlatMinor = (-7 & 0xff) | 0x100,
        GFlatMajor = -6,
        EFlatMinor = (-6 & 0xff) | 0x100,
        DFlatMajor = -5,
        BFlatMinor = (-5 & 0xff) | 0x100,
        AFlatMajor = -4,
        FMinor = (-4 & 0xff) | 0x100,
        EFlatMajor = -3,
        CMinor = (-3 & 0xff) | 0x100,
        BFlatMajor = -2,
        GMinor = (-2 & 0xff) | 0x100,
        FMajor = -1,
        DMinor = (-1 & 0xff) | 0x100,
        CMajor = 0,
        AMinor = (0 & 0xff) | 0x100,
        GMajor = 1,
        EMinor = (1 & 0xff) | 0x100,
        DMajor = 2,
        BMinor = (2 & 0xff) | 0x100,
        AMajor = 3,
        FSharpMinor = (3 & 0xff) | 0x100,
        EMajor = 4,
        CSharpMinor = (4 & 0xff) | 0x100,
        BMajor = 5,
        GSharpMinor = (5 & 0xff) | 0x100,
        FSharpMajor = 6,
        DSharpMinor = (6 & 0xff) | 0x100,
        CSharpMajor = 7,
        ASharpMinor = (7 & 0xff) | 0x100,
    }
    [System.Serializable]
    public sealed class KeyEvent : MTrkEvent
    {
        public const byte EventNumber = 0x59;
        public NoteKey key;
        
        public KeyEvent(){}

        public KeyEvent(uint ticks, NoteKey key):base(ticks)
        {
            this.key = key;
        }
    }
}
