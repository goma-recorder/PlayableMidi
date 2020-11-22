using System.Collections.Generic;

namespace Midity
{
    public class MidiTrack
    {
        #region Serialized variables

        public string name = "";
        public float tempo = 120;
        public uint duration;
        public uint ticksPerQuarterNote = 96;
        public List<MTrkEvent> events = new List<MTrkEvent>();
        public float DurationInSecond
            => duration / tempo * 60 / ticksPerQuarterNote;

        public uint ConvertSecondToTicks(float time)
            => (uint)(time * tempo / 60 * ticksPerQuarterNote);

        public float ConvertTicksToSecond(uint tick)
            => tick * 60 / (tempo * ticksPerQuarterNote);

        public bool GetAbstractTick(MTrkEvent mTrkEvent, out uint ticks)
        {
            ticks = 0u;
            foreach (var e in events)
            {
                ticks += e.ticks;
                if (e == mTrkEvent)
                    return true;
            }
            ticks = 0u;
            return false;
        }

        public bool GetAbstractTime(MTrkEvent mTrkEvent, out float time)
        {
            if (GetAbstractTick(mTrkEvent, out var tick))
            {
                time = ConvertTicksToSecond(tick);
                return true;
            }
            else
            {
                time = 0f;
                return false;
            }
        }

        #endregion
    }
}
