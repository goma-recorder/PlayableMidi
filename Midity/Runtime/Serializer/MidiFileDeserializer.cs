using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Midity.NoteKey;

namespace Midity
{
    // SMF file deserializer implementation
    public static class MidiFileDeserializer
    {
        #region Public members

        public static MidiFile Load(byte[] data, string codeName)
        {
            var encoding = Encoding.GetEncoding(codeName);
            return Load(data, encoding);
        }

        public static MidiFile Load(byte[] data, int codePage)
        {
            var encoding = Encoding.GetEncoding(codePage);
            return Load(data, encoding);
        }

        public static MidiFile Load(byte[] data, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.ASCII;
            var reader = new MidiDataStreamReader(data, encoding);

            // Chunk type
            if (reader.ReadChars(4) != "MThd")
                throw new FormatException("Can't find header chunk.");

            // Chunk length
            if (reader.ReadBEUInt(4) != 6u)
                throw new FormatException("Length of header chunk must be 6.");

            // Format (unused)
            var format = (byte) reader.ReadBEUShort();

            // Number of tracks
            var trackCount = reader.ReadBEUInt(2);

            // Ticks per quarter note
            var tpqn = reader.ReadBEUInt(2);
            if ((tpqn & 0x8000u) != 0)
                throw new FormatException("SMPTE time code is not supported.");

            // Tracks
            var midiFile = new MidiFile(format, tpqn, encoding.CodePage);
            for (var i = 0; i < trackCount; i++)
                ReadTrack(i, midiFile, reader);

            return midiFile;
        }

        #endregion

        #region Private members

        static void ReadTrack(int trackNumber, MidiFile midiFile, MidiDataStreamReader reader)
        {
            // Chunk type
            if (reader.ReadChars(4) != "MTrk")
                throw new FormatException("Can't find track chunk.");

            // Chunk length
            var chunkEnd = reader.ReadBEUInt(4);
            var l = reader.Position - chunkEnd;
            chunkEnd += reader.Position;

            // MIDI event sequence
            var events = new List<MTrkEvent>();
            byte stat = 0;

            while (reader.Position < chunkEnd)
            {
                var mtrkEvent = ReadEvent(reader, ref stat);
                events.Add(mtrkEvent);
            }

            // Quantize duration with bars.
            var trackName = "";
            foreach (var e in events)
            {
                switch (e)
                {
                    case TrackNameEvent trackNameEvent:
                        trackName = trackNameEvent.name;
                        break;
                }
            }

            midiFile.AddTrack(trackNumber, events);
        }

        #endregion

        public static MTrkEvent ReadEvent(MidiDataStreamReader reader, ref byte stat)
        {
            // Delta time
            var ticks = reader.ReadMultiByteValue();

            // Status byte
            if ((reader.PeekByte() & 0x80u) != 0)
                stat = reader.ReadByte();

            switch (stat)
            {
                case 0xff:
                    return ReadMetaEvent(ticks, reader);
                case 0xf0:
                    return ReadSysExEvent(ticks, reader);
                default:
                    return ReadMidiEvent(ticks, stat, reader);
            }
        }

        static MTrkEvent ReadMetaEvent(uint ticks, MidiDataStreamReader reader)
        {
            var eventNumber = reader.ReadByte();
            var length = reader.ReadMultiByteValue();
            byte[] bytes;
            switch (eventNumber)
            {
                // 00
                case SequenceNumberEvent.EventNumber:
                    bytes = reader.ReadBytes(length);
                    var number = (ushort) (bytes[0] << 8 | bytes[1]);
                    return new SequenceNumberEvent(ticks, number);
                // 01
                case TextEvent.EventNumber:
                    return new TextEvent(ticks, reader.ReadChars(length));
                // 02
                case CopyrightEvent.EventNumber:
                    return new CopyrightEvent(ticks, reader.ReadChars(length));
                // 03
                case TrackNameEvent.EventNumber:
                    return new TrackNameEvent(ticks, reader.ReadChars(length));
                // 04
                case InstrumentNameEvent.EventNumber:
                    return new InstrumentNameEvent(ticks, reader.ReadChars(length));
                // 05
                case LyricEvent.EventNumber:
                    return new LyricEvent(ticks, reader.ReadChars(length));
                // 06
                case MarkerEvent.EventNumber:
                    return new MarkerEvent(ticks, reader.ReadChars(length));
                // 07
                case QueueEvent.EventNumber:
                    return new QueueEvent(ticks, reader.ReadChars(length));
                // 20
                case ChannelPrefixEvent.EventNumber:
                    return new ChannelPrefixEvent(ticks, reader.ReadByte());
                // 2f
                case EndPointEvent.EventNumber:
                    return new EndPointEvent(ticks);
                // 51
                case TempoEvent.EventNumber:
                    return new TempoEvent(ticks, reader.ReadBEUInt((byte) length));
                // 54
                case SmpteOffsetEvent.EventNumber:
                    bytes = reader.ReadBytes(length);
                    return new SmpteOffsetEvent(ticks, bytes[0], bytes[1], bytes[2], bytes[3], bytes[4]);
                // 58
                case BeatEvent.EventNumber:
                    bytes = reader.ReadBytes(length);
                    return new BeatEvent(ticks, bytes[0], bytes[1], bytes[2], bytes[3]);
                // 59
                case KeyEvent.EventNumber:
                    var sf = (sbyte) reader.ReadByte();
                    var mi = reader.ReadByte() == 0;

                    if ((sf, mi) == (-7, true))
                        return new KeyEvent(ticks, CFlatMajor);
                    if ((sf, mi) == (-7, false))
                        return new KeyEvent(ticks, AFlatMinor);
                    if ((sf, mi) == (-6, true))
                        return new KeyEvent(ticks, GFlatMajor);
                    if ((sf, mi) == (-6, false))
                        return new KeyEvent(ticks, EFlatMinor);
                    if ((sf, mi) == (-5, true))
                        return new KeyEvent(ticks, DFlatMajor);
                    if ((sf, mi) == (-5, false))
                        return new KeyEvent(ticks, BFlatMinor);
                    if ((sf, mi) == (-4, true))
                        return new KeyEvent(ticks, AFlatMajor);
                    if ((sf, mi) == (-4, false))
                        return new KeyEvent(ticks, FMinor);
                    if ((sf, mi) == (-3, true))
                        return new KeyEvent(ticks, EFlatMajor);
                    if ((sf, mi) == (-3, false))
                        return new KeyEvent(ticks, CMinor);
                    if ((sf, mi) == (-2, true))
                        return new KeyEvent(ticks, BFlatMajor);
                    if ((sf, mi) == (-2, false))
                        return new KeyEvent(ticks, GMinor);
                    if ((sf, mi) == (-1, true))
                        return new KeyEvent(ticks, FMajor);
                    if ((sf, mi) == (-1, false))
                        return new KeyEvent(ticks, DMinor);
                    if ((sf, mi) == (0, true))
                        return new KeyEvent(ticks, CMajor);
                    if ((sf, mi) == (0, false))
                        return new KeyEvent(ticks, AMinor);
                    if ((sf, mi) == (1, true))
                        return new KeyEvent(ticks, GMajor);
                    if ((sf, mi) == (1, false))
                        return new KeyEvent(ticks, EMinor);
                    if ((sf, mi) == (2, true))
                        return new KeyEvent(ticks, DMajor);
                    if ((sf, mi) == (2, false))
                        return new KeyEvent(ticks, BMinor);
                    if ((sf, mi) == (3, true))
                        return new KeyEvent(ticks, AMajor);
                    if ((sf, mi) == (3, false))
                        return new KeyEvent(ticks, FSharpMinor);
                    if ((sf, mi) == (4, true))
                        return new KeyEvent(ticks, EMajor);
                    if ((sf, mi) == (4, false))
                        return new KeyEvent(ticks, CSharpMinor);
                    if ((sf, mi) == (5, true))
                        return new KeyEvent(ticks, BMajor);
                    if ((sf, mi) == (5, false))
                        return new KeyEvent(ticks, GSharpMinor);
                    if ((sf, mi) == (6, true))
                        return new KeyEvent(ticks, FSharpMajor);
                    if ((sf, mi) == (6, false))
                        return new KeyEvent(ticks, DSharpMinor);
                    if ((sf, mi) == (7, true))
                        return new KeyEvent(ticks, CSharpMajor);
                    if ((sf, mi) == (7, false))
                        return new KeyEvent(ticks, ASharpMinor);
                    return null;
                // 7f
                case SequencerUniqueEvent.EventNumber:
                    return new SequencerUniqueEvent(ticks, reader.ReadBytes(length));
                // Unknown
                default:
                    return new UnknownMetaEvent(ticks, eventNumber, reader.ReadBytes(length));
            }
        }

        static MTrkEvent ReadSysExEvent(uint ticks, MidiDataStreamReader reader)
        {
            var length = reader.ReadMultiByteValue() - 1;
            var bytes = new byte[length];

            for (var i = 0; i < length; i++)
                bytes[i] = reader.ReadByte();

            if (reader.ReadByte() == 0xf7)
                return new SysExEvent(ticks, bytes);
            throw new Exception();
        }

        static MTrkEvent ReadMidiEvent(uint ticks, byte stat, MidiDataStreamReader reader)
        {
            var channel = (byte) (stat & 0x0f);
            if ((stat & 0xf0) == 0xb0)
            {
                var controlChangeNumber = reader.ReadByte();
                var data = (stat & 0xe0u) == 0xc0u ? (byte) 0 : reader.ReadByte();
                return new ControlChangeEvent(ticks, channel, controlChangeNumber, data);
            }
            else
            {
                var noteNumber = reader.ReadByte();
                var velocity = (stat & 0xe0u) == 0xc0u ? (byte) 0 : reader.ReadByte();
                var isNoteOn = (stat & 0xf0) == 0x90 && velocity != 0;
                return new NoteEvent(ticks, isNoteOn, channel, noteNumber, velocity);
            }
        }
    }
}