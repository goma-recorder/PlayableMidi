using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Midity.Playable
{
    // Runtime playable class that calculates MIDI based animation
    public sealed class MidiAnimation : PlayableBehaviour
    {
        private readonly MidiTrackPlayer _trackPlayer;
        private readonly MidiAnimationAsset midiAnimationAsset;
        private FrameData _frameData;
        private UnityEngine.Playables.Playable _playable;

        private float _previousTime;

        private readonly MidiSignalPool _signalPool = new MidiSignalPool();

        public MidiAnimation()
        {
        }

        internal MidiAnimation(MidiAnimationAsset midiAnimationAsset)
        {
            this.midiAnimationAsset = midiAnimationAsset;
            _trackPlayer = new MidiTrackPlayer(midiTrack, PushSignal, true);
        }

        private MidiTrack midiTrack => midiAnimationAsset.MidiTrack;

        public float GetValue(UnityEngine.Playables.Playable playable, MidiControl control)
        {
            var t = (float) (playable.GetTime() % midiTrack.TotalSeconds);
            switch (control.mode)
            {
                case MidiControl.Mode.NoteEnvelope:
                    return GetNoteEnvelopeValue(control, t);
                case MidiControl.Mode.NoteCurve:
                    return GetNoteCurveValue(control, t);
                case MidiControl.Mode.CC:
                    return GetCCValue(control, t);
                default:
                    return 0;
            }
        }

        public override object Clone()
        {
            return new MidiAnimation(midiAnimationAsset);
        }

        public override void OnGraphStart(UnityEngine.Playables.Playable playable)
        {
            _previousTime = (float) playable.GetTime();
            _trackPlayer.ResetHead(_previousTime);
        }

        public override void OnBehaviourPause(UnityEngine.Playables.Playable playable, FrameData info)
        {
            // When the playable is being finished, signals laying in the rest
            // of the clip should be all triggered.
            if (!playable.IsDone()) return;
            _playable = playable;
            _frameData = info;
            var currentTime = (float) playable.GetDuration();
            _trackPlayer.PlayByDeltaTime(_previousTime - currentTime);
            _previousTime = currentTime;
        }

        public override void PrepareFrame(UnityEngine.Playables.Playable playable, FrameData info)
        {
            _playable = playable;
            _frameData = info;
            var currentTime = (float) playable.GetTime();
            if (info.evaluationType == FrameData.EvaluationType.Playback)
                _trackPlayer.PlayByDeltaTime(currentTime - _previousTime);
            _previousTime = currentTime;
        }

        private void PushSignal(MTrkEvent mTrkEvent)
        {
            _frameData.output.PushNotification(_playable, _signalPool.Allocate(mTrkEvent));
        }

        private (ControlChangeEvent i0, ControlChangeEvent i1) GetCCEventIndexAroundTick(uint tick,
            Controller controller)
        {
            var time = 0u;
            ControlChangeEvent lastEvent = null;
            foreach (var mEvent in midiTrack.Events)
            {
                time += mEvent.Ticks;
                if (!(mEvent is ControlChangeEvent e)) continue;
                if (e.controller != controller) continue;
                if (time > tick) return (lastEvent, e);
                lastEvent = e;
            }

            return (lastEvent, lastEvent);
        }

        private NoteEventPair GetNoteEventsBeforeTick(uint tick, MidiNoteFilter note)
        {
            NoteEventPair pair = null;
            foreach (var noteEventPair in midiTrack.NoteEventPairs)
            {
                if (noteEventPair.OnTick > tick) break;
                if (!note.Check(noteEventPair.onNoteEvent)) continue;
                pair = noteEventPair;
            }

            return pair;
        }

        private float CalculateEnvelope(MidiEnvelope envelope, float onTime, float offTime)
        {
            var attackTime = envelope.AttackTime;
            var attackRate = 1 / attackTime;

            var decayTime = envelope.DecayTime;
            var decayRate = 1 / decayTime;

            var level = -offTime / envelope.ReleaseTime;

            if (onTime < attackTime)
                level += onTime * attackRate;
            else if (onTime < attackTime + decayTime)
                level += 1 - (onTime - attackTime) * decayRate * (1 - envelope.SustainLevel);
            else
                level += envelope.SustainLevel;

            return Mathf.Max(0, level);
        }

        private float GetNoteEnvelopeValue(MidiControl control, float time)
        {
            var tick = midiTrack.ConvertSecondToTicks(time);
            var pair = GetNoteEventsBeforeTick(tick, control.noteFilter);

            if (pair is null) return 0;

            // Note-off time
            var offTime = Math.Min(midiTrack.ConvertTicksToSecond(pair.OffTick), time);

            var envelope = CalculateEnvelope(
                control.envelope,
                Mathf.Max(0, offTime - midiTrack.ConvertTicksToSecond(pair.OnTick)),
                Mathf.Max(0, time - offTime)
            );

            var velocity = pair.Velocity / 127.0f;

            return envelope * velocity;
        }

        private float GetNoteCurveValue(MidiControl control, float time)
        {
            var tick = midiTrack.ConvertSecondToTicks(time);
            var pair = GetNoteEventsBeforeTick(tick, control.noteFilter);

            if (pair is null) return 0;

            var curve = control.curve.Evaluate(Mathf.Max(0, time - midiTrack.ConvertTicksToSecond(pair.OnTick)));
            var velocity = pair.Velocity / 127.0f;

            return curve * velocity;
        }

        private float GetCCValue(MidiControl control, float time)
        {
            var tick = midiTrack.ConvertSecondToTicks(time);
            var (i0, i1) = GetCCEventIndexAroundTick(tick, control.ccController);

            if (i0 is null) return 0;
            if (i1 is null) return i0.data / 127.0f;

            var v0 = i0.data / 127.0f;
            var v1 = i1.data / 127.0f;

            var t0 = midiTrack.ConvertTicksToSecond(i0.Ticks);
            var t1 = midiTrack.ConvertTicksToSecond(i1.Ticks);
            return Mathf.Lerp(v0, v1, Mathf.Clamp01((time - t0) / (t1 - t0)));
        }
    }
}