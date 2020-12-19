using System;
using System.Text;

namespace Midity
{
    // MIDI binary data stream reader
    public sealed class MidiDataStreamReader
    {
        #region Internal members

        private readonly byte[] _data;
        private readonly Encoding _encoding;

        #endregion

        #region Constructor

        public MidiDataStreamReader(byte[] data, Encoding encoding)
        {
            _data = data;
            _encoding = encoding;
        }

        #endregion

        #region Current reading position

        public uint Position { get; private set; }

        public void Advance(uint delta)
        {
            Position += delta;
        }

        #endregion

        #region Reader methods

        public byte PeekByte()
        {
            return _data[Position];
        }

        public byte ReadByte()
        {
            return _data[Position++];
        }

        public byte[] ReadBytes(uint len)
        {
            var bytes = new byte[len];
            Buffer.BlockCopy(_data, (int) Position, bytes, 0, (int) len);
            Position += len;
            return bytes;
        }

        public string ReadChars(uint length)
        {
            return _encoding.GetString(ReadBytes(length));
        }

        public uint ReadBEUInt(byte length)
        {
            var number = 0u;
            for (byte i = 0; i < length; i++)
            {
                number += (uint) ReadByte() << (length - i - 1) * 8;
            }

            return number;
        }

        public short ReadBEShort()
        {
            var bytes = ReadBytes(2);
            return (short) (bytes[0] << 8 + bytes[1]);
        }

        public ushort ReadBEUShort()
        {
            var bytes = ReadBytes(2);
            return (ushort) (bytes[0] << 8 + bytes[1]);
        }

        public uint ReadMultiByteValue()
        {
            var v = 0u;
            while (true)
            {
                uint b = ReadByte();
                v += b & 0x7fu;
                if (b < 0x80u) break;
                v <<= 7;
            }

            return v;
        }

        public static uint ReadMultiByteValue(byte[] bytes)
        {
            var i = 0;
            var v = 0u;
            while (true)
            {
                uint b = bytes[i];
                i++;
                v += b & 0x7fu;
                if (b < 0x80u) break;
                v <<= 7;
            }

            return v;
        }

        #endregion
    }
}