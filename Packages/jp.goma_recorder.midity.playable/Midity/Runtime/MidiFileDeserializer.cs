using System;
using System.Collections.Generic;

namespace Midity
{
    // SMF file deserializer implementation
    public static class MidiFileDeserializer
    {
        #region Public members

        public static MidiTrack[] Load(byte[] data)
        {
            var reader = new MidiDataStreamReader(data);

            // Chunk type
            if (reader.ReadChars(4) != "MThd")
                throw new FormatException("Can't find header chunk.");

            // Chunk length
            if (reader.ReadBEUInt(4) != 6u)
                throw new FormatException("Length of header chunk must be 6.");

            // Format (unused)
            reader.Advance(2);

            // Number of tracks
            var trackCount = reader.ReadBEUInt(2);

            // Ticks per quarter note
            var tpqn = reader.ReadBEUInt(2);
            if ((tpqn & 0x8000u) != 0)
                throw new FormatException("SMPTE time code is not supported.");

            // Tracks
            var tracks = new MidiTrack[trackCount];
            float? tempo = null;
            for (var i = 0; i < trackCount; i++)
                tracks[i] = ReadTrack(reader, tpqn, ref tempo);

            return tracks;
        }

        #endregion

        #region Private members

        static MidiTrack ReadTrack(MidiDataStreamReader reader, uint tpqn, ref float? tempo)
        {
            // Chunk type
            if (reader.ReadChars(4) != "MTrk")
                throw new FormatException("Can't find track chunk.");

            // Chunk length
            var chunkEnd = reader.ReadBEUInt(4);
            chunkEnd += reader.Position;

            // MIDI event sequence
            var events = new List<MTrkEvent>();
            var allTicks = 0u;
            byte stat = 0;

            while (reader.Position < chunkEnd)
            {
                // Delta time
                var ticks = reader.ReadMultiByteValue();
                allTicks += ticks;

                // Status byte
                if ((reader.PeekByte() & 0x80u) != 0)
                    stat = reader.ReadByte();

                switch (stat)
                {
                    case 0xff:
                        events.Add(ReadMetaEvent(allTicks, ticks, reader));
                        break;
                    case 0xf0:
                        events.Add(ReadSysExEvent(allTicks, ticks, stat, reader));
                        break;
                    default:
                        events.Add(ReadMidiEvent(allTicks, ticks, stat, reader));
                        break;
                }
            }

            // Quantize duration with bars.
            var bars = (allTicks + tpqn * 4 - 1) / (tpqn * 4);
            var trackName = "";
            foreach (var e in events)
            {
                switch (e)
                {
                    case TrackNameEvent trackNameEvent:
                        trackName = trackNameEvent.name;
                        break;
                    case TempoEvent tempoEvent:
                        if (tempo == null)
                            tempo = tempoEvent.tempo;
                        break;
                }
            }
            // Asset instantiation
            return new MidiTrack
            {
                name = trackName,
                tempo = tempo ?? 120f,
                duration = bars * tpqn * 4,
                ticksPerQuarterNote = tpqn,
                events = events,
            };
        }

        #endregion

        static MTrkEvent ReadMetaEvent(uint allTicks, uint ticks, MidiDataStreamReader reader)
        {
            var eventType = reader.ReadByte();
            var length = reader.ReadMultiByteValue();
            switch (eventType)
            {
                // 00
                case SequenceNumberEvent.status:
                    return new SequenceNumberEvent
                    {
                        ticks = ticks,
                        number = reader.ReadBytes(length),
                    };
                // 01
                case TextEvent.status:
                    return new TextEvent
                    {
                        ticks = ticks,
                        text = reader.ReadChars(length),
                    };
                // 02
                case CopyrightEvent.status:
                    return new CopyrightEvent
                    {
                        ticks = ticks,
                        text = reader.ReadChars(length),
                    };
                // 03
                case TrackNameEvent.status:
                    return new TrackNameEvent
                    {
                        ticks = ticks,
                        name = reader.ReadChars(length),
                    };
                // 04
                case InstrumentNameEvent.status:
                    return new InstrumentNameEvent
                    {
                        ticks = ticks,
                        name = reader.ReadChars(length),
                    };
                // 05
                case LyricEvent.status:
                    return new LyricEvent
                    {
                        ticks = ticks,
                        lyric = reader.ReadChars(length),
                    };
                // 06
                case MarkerEvent.status:
                    return new MarkerEvent
                    {
                        ticks = ticks,
                        text = reader.ReadChars(length),
                    };
                // 07
                case QueueEvent.status:
                    return new QueueEvent
                    {
                        ticks = ticks,
                        text = reader.ReadChars(length),
                    };
                // 20
                case ChannelPrefixEvent.status:
                    return new ChannelPrefixEvent
                    {
                        ticks = ticks,
                        data = reader.ReadByte(),
                    };
                // 2f
                case EndPointEvent.status:
                    return new EndPointEvent
                    {
                        ticks = ticks,
                    };
                // 51
                case TempoEvent.status:
                    var tickTempo = reader.ReadBEUInt((byte)length);
                    return new TempoEvent
                    {
                        ticks = ticks,
                        tickTempo = tickTempo,
                    };
                // 54
                case SmpteOffsetEvent.status:
                    return new SmpteOffsetEvent
                    {
                        ticks = ticks,
                        data = reader.ReadBytes(length),
                    };
                // 58
                case BeatEvent.status:
                    return new BeatEvent
                    {
                        ticks = ticks,
                        data = reader.ReadBytes(length),
                    };
                // 59
                case KeyEvent.status:
                    var sf = reader.ReadByte();
                    var mi = reader.ReadByte();
                    return new KeyEvent
                    {
                        ticks = ticks,
                        sf = sf,
                        isMajor = mi == 1,
                    };
                // 7f
                case SequencerUniqueEvent.status:
                    return new SequencerUniqueEvent
                    {
                        ticks = ticks,
                        data = reader.ReadBytes(length),
                    };
                // Ignore
                default:
                    return new UnknownEvent
                    {
                        ticks = ticks,
                        bytes = reader.ReadBytes(length),
                    };
            }
        }
        static MTrkEvent ReadSysExEvent(uint allTicks, uint ticks, byte stat, MidiDataStreamReader reader)
        {
            var bytes = new List<byte>();
            while (true)
            {
                var data = reader.ReadByte();
                bytes.Add(data);
                if (data == 0xf7u)
                    return new UnknownEvent
                    {
                        ticks = ticks,
                        bytes = bytes.ToArray(),
                    };
            }
        }
        static MidiEvent ReadMidiEvent(uint allTicks, uint ticks, byte stat, MidiDataStreamReader reader)
        {
            var b1 = reader.ReadByte();
            var b2 = (stat & 0xe0u) == 0xc0u ? (byte)0 : reader.ReadByte();
            return new MidiEvent
            {
                ticks = ticks,
                status = stat,
                data1 = b1,
                data2 = b2
            };
        }
    }
}
