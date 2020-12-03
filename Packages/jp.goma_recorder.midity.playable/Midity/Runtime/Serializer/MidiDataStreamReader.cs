using System;
using System.Collections.Generic;

namespace Midity
{
    // MIDI binary data stream reader
    public sealed class MidiDataStreamReader
    {
        #region Internal members

        readonly byte[] _data;
        private readonly int _codepage;

        #endregion

        #region Constructor

        public MidiDataStreamReader(byte[] data,int codepage)
        {
            _data = data;
            _codepage = codepage;
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
            Buffer.BlockCopy(_data, (int)Position, bytes, 0, (int)len);
            Position += len;
            return bytes;
        }

        public string ReadChars(uint length)
        {
            return System.Text.Encoding.GetEncoding(_codepage).GetString(ReadBytes(length));
        }

        public uint ReadBEUInt(byte length)
        {
            var number = 0u;
            for (byte i = 0; i < length; i++)
            {
                number += (uint)ReadByte() << (length - i - 1) * 8;
            }
            return number;
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
