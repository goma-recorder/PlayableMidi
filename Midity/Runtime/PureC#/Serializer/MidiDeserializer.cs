using System;
using System.Collections.Generic;
using System.Text;
using static Midity.NoteKey;

namespace Midity
{
    public class MidiDeserializer
    {
        private readonly MidiDataStreamReader _reader;

        public MidiDeserializer(byte[] data, Encoding encoding)
        {
            _reader = new MidiDataStreamReader(data, encoding ?? Encoding.ASCII);
        }

        public MidiDeserializer(byte[] data, int codePage)
            : this(data, Encoding.GetEncoding(codePage))
        {
        }

        public MidiDeserializer(byte[] data, string codeName)
            : this(data, Encoding.GetEncoding(codeName))
        {
        }

        private (MidiFile, uint) LoadMidiFile()
        {
            // Chunk type
            if (_reader.ReadChars(4) != "MThd")
                throw new FormatException("Can't find header chunk.");

            // Chunk length
            if (_reader.ReadBEUInt(4) != 6u)
                throw new FormatException("Length of header chunk must be 6.");

            // Format
            var format = (byte) _reader.ReadBEUShort();

            // Number of tracks
            var trackCount = _reader.ReadBEUInt(2);

            // Ticks per quarter note
            var tpqn = _reader.ReadBEUInt(2);
            if ((tpqn & 0x8000u) != 0)
                throw new FormatException("SMPTE time code is not supported.");
            var midiFile = new MidiFile(tpqn, _reader.encoding, format);

            return (midiFile, trackCount);
        }

        public MidiFile Load()
        {
            var (midiFile, trackCount) = LoadMidiFile();
            for (var i = 0; i < trackCount; i++)
                ReadTrack(i, midiFile);
            return midiFile;
        }

        public (MidiFile midiFile, byte[] trackBytes) LoadTrackBytes()
        {
            var (midiFile, trackCount) = LoadMidiFile();
            var byteCount = _reader.data.Length - _reader.Position;
            var bytes = new byte[byteCount];
            Buffer.BlockCopy(_reader.data, (int) _reader.Position, bytes, 0, (int) byteCount);

            for (var i = 0; i < trackCount; i++)
                ReadTrack(i, midiFile);
            return (midiFile, bytes);
        }

        public void ReadTrack(int trackNumber, MidiFile midiFile)
        {
            // Chunk type
            if (_reader.ReadChars(4) != "MTrk")
                throw new FormatException("Can't find track chunk.");

            // Chunk length
            var chunkEnd = _reader.ReadBEUInt(4);
            chunkEnd += _reader.Position;

            // MIDI event sequence
            var events = new List<MTrkEvent>();
            byte stat = 0;
            while (_reader.Position < chunkEnd)
            {
                var mtrkEvent = ReadEvent(ref stat);
                events.Add(mtrkEvent);
            }

            midiFile.AddTrack(trackNumber, events);
        }

        internal MTrkEvent ReadEvent(ref byte stat)
        {
            // Delta time
            var ticks = _reader.ReadMultiByteValue();

            // Status byte
            if ((_reader.PeekByte() & 0x80u) != 0)
                stat = _reader.ReadByte();

            switch (stat)
            {
                case 0xff:
                    var eventNumber = _reader.ReadByte();
                    var length = _reader.ReadMultiByteValue();
                    return ReadMetaEvent(ticks, eventNumber, length);
                case 0xf0:
                    return ReadSysExEvent(ticks);
                default:
                    var channel = (byte) (stat & 0x0f);
                    return ReadMidiEvent(ticks, stat, channel);
            }
        }

        protected virtual MTrkEvent ReadMetaEvent(uint ticks, byte eventNumber, uint length)
        {
            byte[] bytes;
            switch (eventNumber)
            {
                // 00
                case SequenceNumberEvent.EventNumber:
                    bytes = _reader.ReadBytes(length);
                    var number = (ushort) ((bytes[0] << 8) | bytes[1]);
                    return new SequenceNumberEvent(ticks, number);
                // 01
                case TextEvent.EventNumber:
                    return new TextEvent(ticks, _reader.ReadChars(length));
                // 02
                case CopyrightEvent.EventNumber:
                    return new CopyrightEvent(ticks, _reader.ReadChars(length));
                // 03
                case TrackNameEvent.EventNumber:
                    return new TrackNameEvent(ticks, _reader.ReadChars(length));
                // 04
                case InstrumentNameEvent.EventNumber:
                    return new InstrumentNameEvent(ticks, _reader.ReadChars(length));
                // 05
                case LyricEvent.EventNumber:
                    return new LyricEvent(ticks, _reader.ReadChars(length));
                // 06
                case MarkerEvent.EventNumber:
                    return new MarkerEvent(ticks, _reader.ReadChars(length));
                // 07
                case QueueEvent.EventNumber:
                    return new QueueEvent(ticks, _reader.ReadChars(length));
                // 20
                case ChannelPrefixEvent.EventNumber:
                    return new ChannelPrefixEvent(ticks, _reader.ReadByte());
                // 2f
                case EndPointEvent.EventNumber:
                    return new EndPointEvent(ticks);
                // 51
                case TempoEvent.EventNumber:
                    return new TempoEvent(ticks, _reader.ReadBEUInt((byte) length));
                // 54
                case SmpteOffsetEvent.EventNumber:
                    bytes = _reader.ReadBytes(length);
                    return new SmpteOffsetEvent(ticks, bytes[0], bytes[1], bytes[2], bytes[3], bytes[4]);
                // 58
                case BeatEvent.EventNumber:
                    bytes = _reader.ReadBytes(length);
                    return new BeatEvent(ticks, bytes[0], bytes[1], bytes[2], bytes[3]);
                // 59
                case KeyEvent.EventNumber:
                    var sf = (sbyte) _reader.ReadByte();
                    var mi = _reader.ReadByte() == 0;

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
                    return new SequencerUniqueEvent(ticks, _reader.ReadBytes(length));
                // Unknown
                default:
                    return new UnknownMetaEvent(ticks, eventNumber, _reader.ReadBytes(length));
            }
        }

        protected virtual MTrkEvent ReadSysExEvent(uint ticks)
        {
            var length = _reader.ReadMultiByteValue() - 1;
            var bytes = new byte[length];

            for (var i = 0; i < length; i++)
                bytes[i] = _reader.ReadByte();

            if (_reader.ReadByte() == 0xf7)
                return new SysExEvent(ticks, bytes);
            throw new Exception();
        }

        protected virtual MTrkEvent ReadMidiEvent(uint ticks, byte status, byte channel)
        {
            switch (status & 0xf0)
            {
                // note event
                case 0x80:
                case 0x90:
                    var noteNumber = _reader.ReadByte();
                    var velocity = (status & 0xe0u) == 0xc0u ? (byte) 0 : _reader.ReadByte();
                    var isNoteOn = (status & 0xf0) == 0x90 && velocity != 0;
                    return new NoteEvent(ticks, isNoteOn, channel, noteNumber, velocity);
                // a0
                case PolyphonicKeyPressureEvent.StatusHead:
                    return new PolyphonicKeyPressureEvent(ticks, channel, _reader.ReadByte(), _reader.ReadByte());
                // b0
                case ControlChangeEvent.StatusHead:
                    var controlChangeNumber = _reader.ReadByte();
                    var bytes = (status & 0xe0u) == 0xc0u ? (byte) 0 : _reader.ReadByte();
                    return new ControlChangeEvent(ticks, channel, controlChangeNumber, bytes);
                // c0
                case ProgramChangeEvent.StatusHead:
                    var programNumber = _reader.ReadBEUShort();
                    return new ProgramChangeEvent(ticks, channel, programNumber);
                // d0
                case ChannelPressureEvent.StatusHead:
                    return new ChannelPressureEvent(ticks, channel, _reader.ReadByte());
                // e0
                case PitchBendEvent.StatusHead:
                    var byte1 = _reader.ReadByte();
                    var byte2 = _reader.ReadByte();
                    return new PitchBendEvent(ticks, channel, byte1, byte2);
                default:
                    throw new Exception("Unknown midi event");
            }
        }
    }
}