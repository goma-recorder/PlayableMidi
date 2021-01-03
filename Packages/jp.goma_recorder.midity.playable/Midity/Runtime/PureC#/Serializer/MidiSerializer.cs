using System;
using System.Text;
using static Midity.NoteKey;

namespace Midity
{
    public sealed class MidiSerializer
    {
        private static byte[] Concat(byte[] bytes1, byte[] bytes2)
        {
            var mergedBytes = new byte[bytes1.Length + bytes2.Length];
            Buffer.BlockCopy(bytes1, 0, mergedBytes, 0, bytes1.Length);
            Buffer.BlockCopy(bytes2, 0, mergedBytes, bytes1.Length, bytes2.Length);
            return mergedBytes;
        }

        public static byte[] SerializeFile(MidiFile midiFile)
        {
            var offset = 0;
            var bytes = new byte[14];
            var stringBytes = midiFile.encoding.GetBytes("MThd");
            Buffer.BlockCopy(stringBytes, 0, bytes, offset, 4);
            offset += 4;
            // Chunk length
            WriteBEUint(6, 4);
            WriteBEUint(midiFile.format, 2);
            WriteBEUint((uint) midiFile.Tracks.Count, 2);
            WriteBEUint(midiFile.DeltaTime, 2);

            void WriteBEUint(uint value, int length)
            {
                for (var i = 0; i < length; i++)
                {
                    offset += i;
                    bytes[offset] = (byte) ((value >> (8 * (length - i - 1))) & 0xff);
                }
            }

            foreach (var midiTrack in midiFile.Tracks)
                bytes = Concat(bytes, SerializeTrack(midiTrack, midiFile.encoding));

            return bytes;
        }

        internal static byte[] SerializeTrack(MidiTrack midiTrack, Encoding encoding)
        {
            var bytes = new byte[8];
            var stringBytes = encoding.GetBytes("MTrk");
            Buffer.BlockCopy(stringBytes, 0, bytes, 0, 4);
            foreach (var mTrkEvent in midiTrack.Events) bytes = Concat(bytes, SerializeEvent(mTrkEvent, encoding));

            return bytes;
        }

        internal static byte[] SerializeEvent(MTrkEvent mTrkEvent, Encoding encoding)
        {
            var offset = 0;
            var length = 0;
            byte[] bytes;
            switch (mTrkEvent)
            {
                case NoteEvent noteEvent:
                    WriteTicks(3);
                    bytes[offset] = noteEvent.Status;
                    bytes[++offset] = noteEvent.NoteNumber;
                    bytes[++offset] = noteEvent.Velocity;
                    offset++;
                    return bytes;
                case PolyphonicKeyPressureEvent polyphonicKeyPressureEvent:
                    WriteTicks(3);
                    bytes[offset] = polyphonicKeyPressureEvent.Status;
                    bytes[++offset] = polyphonicKeyPressureEvent.noteNumber;
                    bytes[++offset] = polyphonicKeyPressureEvent.pressure;
                    offset++;
                    return bytes;
                case ControlChangeEvent controlChangeEvent:
                    WriteTicks(3);
                    bytes[offset] = controlChangeEvent.Status;
                    bytes[++offset] = controlChangeEvent.controlChangeNumber;
                    bytes[++offset] = controlChangeEvent.data;
                    offset++;
                    return bytes;
                case ProgramChangeEvent programChangeEvent:
                    WriteTicks(3);
                    bytes[offset] = programChangeEvent.Status;
                    bytes[++offset] = (byte) ((programChangeEvent.programNumber >> 8) & 0xff);
                    bytes[++offset] = (byte) (programChangeEvent.programNumber & 0xff);
                    offset++;
                    return bytes;
                case ChannelPressureEvent channelPressureEvent:
                    WriteTicks(2);
                    bytes[offset] = channelPressureEvent.Status;
                    bytes[++offset] = channelPressureEvent.pressure;
                    offset++;
                    return bytes;
                case PitchBendEvent pitchBendEvent:
                    WriteTicks(3);
                    bytes[offset] = pitchBendEvent.Status;
                    bytes[++offset] = pitchBendEvent.byte1;
                    bytes[++offset] = pitchBendEvent.byte2;
                    offset++;
                    return bytes;
                case SequenceNumberEvent sequenceNumberEvent:
                    WriteTicks(5);
                    bytes[offset] = 0xff;
                    bytes[++offset] = SequenceNumberEvent.EventNumber;
                    bytes[++offset] = 0x02;
                    bytes[++offset] = (byte) (sequenceNumberEvent.number >> 8);
                    bytes[++offset] = (byte) (sequenceNumberEvent.number & 0x00ff);
                    offset++;
                    return bytes;
                case TextEvent textEvent:
                    WriteTextEvent(TextEvent.EventNumber, textEvent.text);
                    return bytes;
                case CopyrightEvent copyrightEvent:
                    WriteTextEvent(CopyrightEvent.EventNumber, copyrightEvent.text);
                    return bytes;
                case TrackNameEvent trackNameEvent:
                    WriteTextEvent(TrackNameEvent.EventNumber, trackNameEvent.name);
                    return bytes;
                case InstrumentNameEvent instrumentNameEvent:
                    WriteTextEvent(InstrumentNameEvent.EventNumber, instrumentNameEvent.name);
                    return bytes;
                case LyricEvent lyricEvent:
                    WriteTextEvent(LyricEvent.EventNumber, lyricEvent.lyric);
                    return bytes;
                case MarkerEvent markerEvent:
                    WriteTextEvent(MarkerEvent.EventNumber, markerEvent.text);
                    return bytes;
                case QueueEvent queueEvent:
                    WriteTextEvent(QueueEvent.EventNumber, queueEvent.text);
                    return bytes;
                case ChannelPrefixEvent channelPrefixEvent:
                    WriteTicks(4);
                    bytes[offset] = 0xff;
                    bytes[++offset] = ChannelPrefixEvent.EventNumber;
                    bytes[++offset] = 1;
                    bytes[++offset] = channelPrefixEvent.data;
                    offset++;
                    return bytes;
                case EndPointEvent endPointEvent:
                    WriteTicks(3);
                    bytes[offset] = 0xff;
                    bytes[++offset] = EndPointEvent.EventNumber;
                    bytes[++offset] = 0;
                    offset++;
                    return bytes;
                case TempoEvent tempoEvent:
                    WriteTicks(6);
                    bytes[offset] = 0xff;
                    bytes[++offset] = TempoEvent.EventNumber;
                    bytes[++offset] = 3;
                    bytes[++offset] = (byte) ((tempoEvent.TickTempo >> 16) & 0xff);
                    bytes[++offset] = (byte) ((tempoEvent.TickTempo >> 8) & 0xff);
                    bytes[++offset] = (byte) (tempoEvent.TickTempo & 0xff);
                    offset++;
                    return bytes;
                case SmpteOffsetEvent smpteOffsetEvent:
                    WriteTicks(8);
                    bytes[offset] = 0xff;
                    bytes[++offset] = SmpteOffsetEvent.EventNumber;
                    bytes[++offset] = 5;
                    bytes[++offset] = smpteOffsetEvent.hr;
                    bytes[++offset] = smpteOffsetEvent.mn;
                    bytes[++offset] = smpteOffsetEvent.se;
                    bytes[++offset] = smpteOffsetEvent.fr;
                    bytes[++offset] = smpteOffsetEvent.ff;
                    offset++;
                    return bytes;
                case BeatEvent beatEvent:
                    WriteTicks(7);
                    bytes[offset] = 0xff;
                    bytes[++offset] = BeatEvent.EventNumber;
                    bytes[++offset] = 4;
                    bytes[++offset] = beatEvent.nn;
                    bytes[++offset] = beatEvent.dd;
                    bytes[++offset] = beatEvent.cc;
                    bytes[++offset] = beatEvent.bb;
                    offset++;
                    return bytes;
                case KeyEvent keyEvent:
                    sbyte keyNumber = 0;
                    switch (keyEvent.noteKey)
                    {
                        case CFlatMajor:
                        case AFlatMinor:
                            keyNumber = -7;
                            break;
                        case GFlatMajor:
                        case EFlatMinor:
                            keyNumber = -6;
                            break;
                        case DFlatMajor:
                        case BFlatMinor:
                            keyNumber = -5;
                            break;
                        case AFlatMajor:
                        case FMinor:
                            keyNumber = -4;
                            break;
                        case EFlatMajor:
                        case CMinor:
                            keyNumber = -3;
                            break;
                        case BFlatMajor:
                        case GMinor:
                            keyNumber = -2;
                            break;
                        case FMajor:
                        case DMinor:
                            keyNumber = -1;
                            break;
                        case CMajor:
                        case AMinor:
                            keyNumber = 0;
                            break;
                        case GMajor:
                        case EMinor:
                            keyNumber = 1;
                            break;
                        case DMajor:
                        case BMinor:
                            keyNumber = 2;
                            break;
                        case AMajor:
                        case FSharpMinor:
                            keyNumber = 3;
                            break;
                        case EMajor:
                        case CSharpMinor:
                            keyNumber = 4;
                            break;
                        case BMajor:
                        case GSharpMinor:
                            keyNumber = 5;
                            break;
                        case FSharpMajor:
                        case DSharpMinor:
                            keyNumber = 6;
                            break;
                        case CSharpMajor:
                        case ASharpMinor:
                            keyNumber = 7;
                            break;
                    }

                    WriteTicks(5);
                    bytes[offset] = 0xff;
                    bytes[++offset] = KeyEvent.EventNumber;
                    bytes[++offset] = 2;
                    bytes[++offset] = (byte) keyNumber;
                    bytes[++offset] = (byte) (keyEvent.noteKey.IsMajor() ? 0 : 1);
                    offset++;
                    return bytes;
                case SequencerUniqueEvent sequencerUniqueEvent:
                    WriteBytesDataEvent(sequencerUniqueEvent.data, 0xff, SequencerUniqueEvent.EventNumber);
                    return bytes;
                case UnknownMetaEvent unknownEvent:
                    WriteBytesDataEvent(unknownEvent.data, 0xff, unknownEvent.eventNumber);
                    return bytes;
                case SysExEvent sysExEvent:
                    var sysExData = new byte[sysExEvent.data.Length + 1];
                    Buffer.BlockCopy(sysExEvent.data, 0, sysExData, 0, sysExEvent.data.Length);
                    sysExData[sysExData.Length - 1] = 0xf7;
                    WriteBytesDataEvent(sysExData, 0xf0);
                    return bytes;
                default:
                    return null;
            }

            void WriteTicks(int dataLength)
            {
                var tickBytes = ToMultiBytes(mTrkEvent.Ticks);
                var tickBytesLength = tickBytes.Length;
                length = tickBytesLength + dataLength;
                bytes = new byte[length];
                Buffer.BlockCopy(tickBytes, 0, bytes, offset, tickBytesLength);
                offset += tickBytesLength;
            }

            void WriteTextEvent(byte eventNumber, string text)
            {
                var tickBytes = ToMultiBytes(mTrkEvent.Ticks);
                var tickBytesLength = tickBytes.Length;
                var textBytes = encoding.GetBytes(text);
                var textBytesLength = textBytes.Length;
                var textLengthBytes = ToMultiBytes((uint) textBytes.Length);
                var textLengthBytesLength = textLengthBytes.Length;
                length = tickBytesLength + 2 + textLengthBytesLength + textBytesLength;
                bytes = new byte[length];

                Buffer.BlockCopy(tickBytes, 0, bytes, offset, tickBytesLength);
                offset += tickBytesLength;


                bytes[offset] = 0xff;
                bytes[++offset] = eventNumber;
                offset++;

                Buffer.BlockCopy(textLengthBytes, 0, bytes, offset, textLengthBytesLength);
                offset += textLengthBytesLength;

                Buffer.BlockCopy(textBytes, 0, bytes, offset, textBytesLength);
                offset += textBytesLength;
            }

            void WriteBytesDataEvent(byte[] data, params byte[] statusData)
            {
                var dataLength = data.Length;
                var dataLengthBytes = ToMultiBytes((uint) dataLength);
                var dataLengthBytesLength = dataLengthBytes.Length;

                var statusDataLength = statusData.Length;

                WriteTicks(statusDataLength + dataLengthBytesLength + dataLength);

                Buffer.BlockCopy(statusData, 0, bytes, offset, statusDataLength);
                offset += statusDataLength;

                Buffer.BlockCopy(dataLengthBytes, 0, bytes, offset, dataLengthBytesLength);
                offset += dataLengthBytesLength;

                Buffer.BlockCopy(data, 0, bytes, offset, dataLength);
                offset += dataLength;
            }
        }

        private static byte[] ToMultiBytes(uint value)
        {
            var count = GetBytesCount(value);
            var bytes = new byte[count];
            var isLastByte = true;
            for (var i = count - 1; i >= 0; i--)
            {
                var num = value & 0b0111_1111;
                if (!isLastByte)
                    num |= 0b1000_0000;
                isLastByte = false;
                bytes[i] = (byte) num;
                value >>= 7;
            }

            return bytes;
        }

        private static int GetBytesCount(uint value)
        {
            var count = 1;
            uint n = 0b1000_0000;
            while (true)
            {
                if (value < n)
                    return count;
                n <<= 7;
                count++;
            }
        }
    }
}