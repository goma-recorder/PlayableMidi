using System;
using System.Collections.Generic;
using UnityEngine;

namespace Midity
{
    // SMF file deserializer implementation
    public static class MidiFileDeserializer
    {
        #region Public members

        public static MidiTrack[] Load(byte[] data)
        {
            var reader = new MidiDataStreamReader(data,932);

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
                var mtrkEvent = ReadEvent(reader, ref stat);
                allTicks += mtrkEvent.ticks;
                events.Add(mtrkEvent);
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
                    var sf = reader.ReadByte();
                    var mi = reader.ReadByte();
                    var key = (NoteKey) (mi << 8 | sf);
                    return new KeyEvent(ticks, key);
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
            
            if(reader.ReadByte() == 0xf7)
                return new SysExEvent(ticks, bytes);
            throw new Exception();
        }
        
        static MTrkEvent ReadMidiEvent(uint ticks, byte stat, MidiDataStreamReader reader)
        {
            var channel = (byte)(stat & 0x0f);
            if ((stat & 0xf0) == 0xb0)
            {
                var controlChangeNumber = reader.ReadByte();
                var data = (stat & 0xe0u) == 0xc0u ? (byte) 0 : reader.ReadByte();
                return new ControlChangeEvent(ticks, channel, controlChangeNumber, data);
            }
            else
            {
                var isNoteOn = (stat & 0xf0) == 0x90;
                var noteNumber = reader.ReadByte();
                var noteName = (NoteName)(noteNumber % 12);
                var noteOctave = (NoteOctave)(noteNumber / 12);
                var velocity = (stat & 0xe0u) == 0xc0u ? (byte)0 : reader.ReadByte();
                return new NoteEvent(ticks,isNoteOn,channel,noteName,noteOctave,velocity);
            }
        }
    }
}
