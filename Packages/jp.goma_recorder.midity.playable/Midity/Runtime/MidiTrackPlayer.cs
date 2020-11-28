using System;

namespace Midity
{
    public class MidiTrackPlayer
    {

        #region Parameters

        public MidiTrackPlayer(MidiTrack track,Action<MTrkEvent> onPush)
        {
            this.track = track;
            this.onPush = onPush;
        }
        public readonly MidiTrack track;
        public Action<MTrkEvent> onPush;

        #endregion


        #region MIDI signal emission

        int _headIndex = 0;
        uint _lastTick = 0;
        public void ResetHead(float time, bool loop = true)
        {
            var targetTick = (uint)(time * track.tempo / 60 * track.ticksPerQuarterNote);
            ResetHead(targetTick, loop);
        }
        public void ResetHead(uint targetTick, bool loop = true)
        {
            _lastTick = 0u;
            while (targetTick - _lastTick > track.events[_headIndex].ticks)
            {
                _lastTick += track.events[_headIndex].ticks;
                _headIndex++;
                if (loop && _headIndex == track.events.Count)
                    _headIndex = 0;
            }
        }
        public void Play(float currentTime, bool loop = true)
        {
            var currentTick = (uint)(currentTime * track.tempo / 60 * track.ticksPerQuarterNote);
            if (currentTick < _lastTick)
            {
                ResetHead(currentTick, loop);
                return;
            }

            var deltaTick = currentTick - _lastTick;
            while (track.events[_headIndex].ticks <= deltaTick)
            {
                _lastTick += track.events[_headIndex].ticks;
                deltaTick -= track.events[_headIndex].ticks;
                onPush?.Invoke(track.events[_headIndex]);
                _headIndex++;
                if (loop && _headIndex == track.events.Count)
                    _headIndex = 0;
            }
        }

        #endregion
    }
}
