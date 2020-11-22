using System;

namespace Midity
{
    // MIDI binary data stream reader
    sealed class MidiDataStreamReader
    {
        #region Internal members

        readonly byte[] _data;

        #endregion

        #region Constructor

        public MidiDataStreamReader(byte[] data)
        {
            _data = data;
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

        public string ReadChars(uint length, int codepage = 932)
        {
            var bytesData = new byte[length];
            for (var i = 0; i < length; i++)
                bytesData[i] = ReadByte();
            return System.Text.Encoding.GetEncoding(codepage).GetString(bytesData);
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

        #endregion
    }
}
