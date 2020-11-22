using System;

namespace Midity
{
    public class MidiTrackPlayer
    {

        #region Parameters

        public MidiTrackPlayer(MidiTrack track)
        {
            this.track = track;
        }
        public MidiTrack track;

        #endregion


        #region MIDI signal emission

        int headIndex = 0;
        uint lastTick = 0;
        public void ResetHead(float time, bool loop = true)
        {
            var targetTick = (uint)(time * track.tempo / 60 * track.ticksPerQuarterNote);
            ResetHead(targetTick, loop);
        }
        public void ResetHead(uint targetTick, bool loop = true)
        {
            lastTick = 0u;
            while (targetTick - lastTick > track.events[headIndex].ticks)
            {
                lastTick += track.events[headIndex].ticks;
                headIndex++;
                if (loop && headIndex == track.events.Count)
                    headIndex = 0;
            }
        }
        public void Play(float currentTime, Action<MTrkEvent> onPushEvent = null, bool loop = true)
        {
            var currentTick = (uint)(currentTime * track.tempo / 60 * track.ticksPerQuarterNote);
            if (currentTick < lastTick)
            {
                ResetHead(currentTick, loop);
                return;
            }

            var deltatick = currentTick - lastTick;
            while (track.events[headIndex].ticks <= deltatick)
            {
                lastTick += track.events[headIndex].ticks;
                deltatick -= track.events[headIndex].ticks;
                if (onPushEvent != null)
                    onPushEvent(track.events[headIndex]);
                headIndex++;
                if (loop && headIndex == track.events.Count)
                    headIndex = 0;
            }
        }

        #endregion
    }
}
