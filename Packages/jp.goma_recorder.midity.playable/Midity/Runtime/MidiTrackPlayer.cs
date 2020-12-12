using System;

namespace Midity
{
    public class MidiTrackPlayer
    {
        #region Parameters

        public MidiTrackPlayer(MidiTrack track, Action<MTrkEvent> onPush, bool canLoop)
        {
            this.track = track;
            this.onPush = onPush;
            this.canLoop = canLoop;
        }

        public readonly MidiTrack track;
        public Action<MTrkEvent> onPush;

        #endregion


        #region MIDI signal emission

        private int _headIndex = 0;
        private uint _lastTick = 0;
        private float _lastTime = 0f;
        public float LastTime => _lastTime;
        public bool canLoop = true;

        public void ResetHead(float time)
        {
            _lastTime = time;
            var targetTick = (uint) (time * track.tempo / 60 * track.ticksPerQuarterNote);
            ResetHead(targetTick);
        }

        void ResetHead(uint targetTick)
        {
            _lastTick = 0u;
            while (targetTick - _lastTick > track.events[_headIndex].ticks)
            {
                _lastTick += track.events[_headIndex].ticks;
                _headIndex++;
                if (_headIndex == track.events.Count)
                    if (canLoop)
                        _headIndex = 0;
                    else
                        return;
            }
        }

        public void PlayByDeltaTime(float deltaTime)
        {
            PlayByAbstractTime(_lastTime + deltaTime);
        }

        public void PlayByAbstractTime(float currentTime)
        {
            _lastTime = currentTime;
            var currentTick = (uint) (_lastTime * track.tempo / 60 * track.ticksPerQuarterNote);
            if (currentTick < _lastTick)
            {
                ResetHead(currentTick);
                return;
            }

            if (!canLoop && _headIndex > track.events.Count - 1)
                return;

            var deltaTick = currentTick - _lastTick;
            while (track.events[_headIndex].ticks <= deltaTick)
            {
                _lastTick += track.events[_headIndex].ticks;
                deltaTick -= track.events[_headIndex].ticks;
                onPush?.Invoke(track.events[_headIndex]);
                _headIndex++;
                if (_headIndex == track.events.Count)
                    if (canLoop)
                        _headIndex = 0;
                    else
                        return;
            }
        }

        #endregion
    }
}