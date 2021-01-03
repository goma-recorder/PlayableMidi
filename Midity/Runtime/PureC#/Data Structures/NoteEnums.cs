using static Midity.NoteKey;

namespace Midity
{
    public enum NoteName
    {
        C,
        CSharp,
        D,
        DSharp,
        E,
        F,
        FSharp,
        G,
        GSharp,
        A,
        ASharp,
        B
    }

    public enum NoteOctave
    {
        Minus2,
        Minus1,
        Zero,
        Plus1,
        Plus2,
        Plus3,
        Plus4,
        Plus5,
        Plus6,
        Plus7,
        Plus8
    }

    public enum NoteKey
    {
        CFlatMajor,
        AFlatMinor,
        GFlatMajor,
        EFlatMinor,
        DFlatMajor,
        BFlatMinor,
        AFlatMajor,
        FMinor,
        EFlatMajor,
        CMinor,
        BFlatMajor,
        GMinor,
        FMajor,
        DMinor,
        CMajor,
        AMinor,
        GMajor,
        EMinor,
        DMajor,
        BMinor,
        AMajor,
        FSharpMinor,
        EMajor,
        CSharpMinor,
        BMajor,
        GSharpMinor,
        FSharpMajor,
        DSharpMinor,
        CSharpMajor,
        ASharpMinor
    }

    public static class NoteEnumUtil
    {
        public static NoteName ToNoteName(byte noteNumber)
        {
            return (NoteName) (noteNumber % 12);
        }

        public static NoteOctave ToNoteOctave(byte noteNumber)
        {
            return (NoteOctave) (noteNumber / 12);
        }

        public static byte ToNoteNumber(NoteName noteName, NoteOctave noteOctave)
        {
            return (byte) ((int) noteName + (int) noteOctave * 12);
        }
    }

    public static class NoteKeyExtension
    {
        public static bool IsMajor(this NoteKey noteKey)
        {
            switch (noteKey)
            {
                case CFlatMajor:
                case GFlatMajor:
                case DFlatMajor:
                case AFlatMajor:
                case EFlatMajor:
                case BFlatMajor:
                case FMajor:
                case CMajor:
                case GMajor:
                case DMajor:
                case AMajor:
                case EMajor:
                case BMajor:
                case FSharpMajor:
                case CSharpMajor:
                    return true;
                default:
                    return false;
            }
        }
    }
}