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

        private int _headIndex;
        private uint _lastTick;
        private float _lastTime;
        public float LastTime => IsFinished ? track.AllSeconds : _lastTime;
        public bool canLoop = true;
        public bool IsFinished => _headIndex >= track.Events.Count;

        public void ResetHead(float time)
        {
            if (!canLoop && time >= track.AllSeconds)
            {
                _headIndex = track.Events.Count;
                return;
            }

            _lastTime = time % track.AllSeconds;
            var targetTick = track.ConvertSecondToTicks(_lastTime);
            _lastTick = 0u;
            _headIndex = 0;
            while (targetTick - _lastTick > track.Events[_headIndex].Ticks)
            {
                _lastTick += track.Events[_headIndex].Ticks;
                _headIndex++;
                if (_headIndex == track.Events.Count)
                    if (canLoop)
                        _headIndex = 0;
                    else
                        return;
            }
        }

        private void ResetHead(uint targetTick)
        {
            _lastTick = 0u;
            _headIndex = 0;
            while (targetTick - _lastTick > track.Events[_headIndex].Ticks)
            {
                _lastTick += track.Events[_headIndex].Ticks;
                _headIndex++;
                if (_headIndex == track.Events.Count)
                    if (canLoop)
                        _headIndex = 0;
                    else
                        return;
            }
        }

        public void PlayByDeltaTime(float deltaTime)
        {
            if (IsFinished || deltaTime < 0) return;

            _lastTime += deltaTime;
            var currentTick = track.ConvertSecondToTicks(_lastTime);


            if (!canLoop && _headIndex > track.Events.Count - 1)
                return;

            var deltaTick = currentTick - _lastTick;
            while (track.Events[_headIndex].Ticks <= deltaTick)
            {
                _lastTick += track.Events[_headIndex].Ticks;
                deltaTick -= track.Events[_headIndex].Ticks;
                onPush?.Invoke(track.Events[_headIndex]);
                _headIndex++;
                if (_headIndex == track.Events.Count)
                    if (canLoop)
                    {
                        deltaTime = _lastTime - track.AllSeconds;
                        ResetHead(0f);
                        PlayByDeltaTime(deltaTime);
                    }
                    else
                    {
                        return;
                    }
            }
        }

        #endregion
    }
}