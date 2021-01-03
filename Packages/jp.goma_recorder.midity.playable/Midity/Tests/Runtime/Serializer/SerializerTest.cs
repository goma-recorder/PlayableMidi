using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Midity.Tests
{
    public class SerializerTest
    {
        private static T ReDeserialize<T>(T mTrkEvent) where T : MTrkEvent
        {
            var encoding = Encoding.GetEncoding("shift_jis");
            var bytes = MidiSerializer.SerializeEvent(mTrkEvent, encoding);
            var deserializer = new MidiDeserializer(bytes, encoding);
            byte status = 0;
            return (T) deserializer.ReadEvent(ref status);
        }

        private byte GetRandomByte()
        {
            return (byte) UnityEngine.Random.Range(0, 0xff);
        }

        private (uint ticks, byte channel) GetRandomValue()
        {
            var ticks = (uint) UnityEngine.Random.Range(0, 0xffffff);
            var channel = (byte) UnityEngine.Random.Range(0, 0xf);
            return (ticks, channel);
        }

        [Test]
        public void NoteEvent()
        {
            var ticks = 300u;
            var isNoteOn = true;
            var channel = (byte) 2;
            var noteName = NoteName.A;
            var noteOctave = NoteOctave.Zero;
            var velocity = (byte) 5;

            var x = new NoteEvent(ticks, isNoteOn, channel, noteName, noteOctave, velocity);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.isNoteOn == y.isNoteOn);
            Assert.That(x.Channel == y.Channel);
            Assert.That(x.NoteName == y.NoteName);
            Assert.That(x.NoteOctave == y.NoteOctave);
            Assert.That(x.Velocity == y.Velocity);
        }

        [Test]
        public void PolyphonicKeyPressureEvent()
        {
            var ticks = 45212u;
            byte channel = 15;
            byte noteNumber = 125;
            byte pressure = 21;

            var x = new PolyphonicKeyPressureEvent(ticks, channel, noteNumber, pressure);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.Channel == y.Channel);
            Assert.That(x.noteNumber == y.noteNumber);
            Assert.That(x.pressure == y.pressure);
        }

        [Test]
        public void ControlChangeEvent()
        {
            var ticks = 300u;
            byte channel = 3;

            var x = new ControlChangeEvent(ticks, channel, GetRandomByte(), GetRandomByte());
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.Status == y.Status);
            Assert.That(x.controlChangeNumber == y.controlChangeNumber);
            Assert.That(x.data == y.data);
        }

        [Test]
        public void ProgramChangeEvent()
        {
            var ticks = 6253u;
            byte channel = 7;

            var x = new ProgramChangeEvent(ticks, channel, GetRandomByte());
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.Channel == y.Channel);
            Assert.That(x.programNumber == y.programNumber);
        }

        [Test]
        public void ChannelPressureEvent()
        {
            var value = GetRandomValue();

            var x = new ChannelPressureEvent(value.ticks, value.channel, GetRandomByte());
            var y = ReDeserialize(x);

            Assert.That(x.pressure == y.pressure);
        }

        [Test]
        public void PitchBendEvent()
        {
            var value = GetRandomValue();

            var x = new PitchBendEvent(value.ticks, value.channel, GetRandomByte(), GetRandomByte());
            var y = ReDeserialize(x);

            Assert.That(x.byte1 == y.byte1);
            Assert.That(x.byte2 == y.byte2);
        }

        [Test]
        public void SequenceNumberEvent()
        {
            var ticks = 300u;
            var number = (ushort) 6000;

            var x = new SequenceNumberEvent(ticks, number);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.number == y.number);
        }

        [Test]
        public void TextEvent()
        {
            var ticks = 3000u;
            var text = "s123あいう機";

            var x = new TextEvent(ticks, text);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.text == y.text);
        }

        [Test]
        public void CopyrightEvent()
        {
            var ticks = 2929u;
            var text = "MIT license";

            var x = new CopyrightEvent(ticks, text);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.text == y.text);
        }

        [Test]
        public void TrackNameEvent()
        {
            var ticks = 10u;
            var trackName = "piano";

            var x = new TrackNameEvent(ticks, trackName);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.name == y.name);
        }

        [Test]
        public void InstrumentNameEvent()
        {
            var ticks = 200000u;
            var name = "guitar";

            var x = new InstrumentNameEvent(ticks, name);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.name == y.name);
        }

        [Test]
        public void LyricEvent()
        {
            var ticks = 29470u;
            var lyric = "歌詞です";

            var x = new LyricEvent(ticks, lyric);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.lyric == y.lyric);
        }

        [Test]
        public void MarkerEvent()
        {
            var ticks = 20202u;
            var text = "tetext";

            var x = new MarkerEvent(ticks, text);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.text == y.text);
        }

        [Test]
        public void QueueEvent()
        {
            var ticks = 20202u;
            var text = "queue";

            var x = new QueueEvent(ticks, text);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.text == y.text);
        }

        [Test]
        public void ChannelPrefixEvent()
        {
            var ticks = 200836u;
            byte data = 128;

            var x = new ChannelPrefixEvent(ticks, data);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.data == y.data);
        }

        [Test]
        public void EndPointEvent()
        {
            var ticks = 18246u;

            var x = new EndPointEvent(ticks);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
        }

        [Test]
        public void TempoEvent()
        {
            var ticks = 9843758u;
            uint tempo = 0xff_ff_ff;
            var x = new TempoEvent(ticks, tempo);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.TickTempo == y.TickTempo);
        }

        [Test]
        public void SmpteOffsetEvent()
        {
            var ticks = 6276540u;
            byte hr = 223;
            byte mn = 145;
            byte se = 126;
            byte fr = 36;
            byte ff = 255;

            var x = new SmpteOffsetEvent(ticks, hr, mn, se, fr, ff);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.hr == y.hr);
            Assert.That(x.mn == y.mn);
            Assert.That(x.se == y.se);
            Assert.That(x.fr == y.fr);
            Assert.That(x.ff == y.ff);
        }

        [Test]
        public void BeatEvent()
        {
            var ticks = 19864u;
            byte nn = 0;
            byte dd = 0;
            byte cc = 0;
            byte bb = 0;

            var x = new BeatEvent(ticks, nn, dd, cc, bb);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.nn == y.nn);
            Assert.That(x.dd == y.dd);
            Assert.That(x.cc == y.cc);
            Assert.That(x.bb == y.bb);
        }

        [Test]
        public void KeyEvent()
        {
            var ticks = 2534u;
            var key = NoteKey.AFlatMinor;

            var x = new KeyEvent(ticks, key);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.noteKey == y.noteKey);
        }

        [Test]
        public void SequencerUniqueEvent()
        {
            var ticks = 275684u;
            var data = new byte[] {0, 1, 2, 3, 4, 5, 6};

            var x = new SequencerUniqueEvent(ticks, data);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.data.SequenceEqual(y.data));
        }

        [Test]
        public void UnknownMetaEvent()
        {
            var ticks = 176846u;
            byte eventNumber = 0xf1;
            var data = new byte[] {0xf1, 1, 2, 3, 4, 5, 6};

            var x = new UnknownMetaEvent(ticks, eventNumber, data);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.eventNumber == y.eventNumber);
            Assert.That(x.data.SequenceEqual(y.data));
        }

        [Test]
        public void SysExEvent()
        {
            var ticks = 25463u;
            var data = new byte[] {255, 32, 7, 65, 35, 42};

            var x = new SysExEvent(ticks, data);
            var y = ReDeserialize(x);

            Assert.That(x.Ticks == y.Ticks);
            Assert.That(x.data.SequenceEqual(y.data));
        }
    }
}