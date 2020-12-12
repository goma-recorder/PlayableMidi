using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Midity.Playable
{
    // Runtime playable class that calculates MIDI based animation
    [System.Serializable]
    public sealed class MidiAnimation : PlayableBehaviour
    {
        #region Serialized variables

        public string trackName;
        public float tempo = 120;
        public uint duration;
        public uint ticksPerQuarterNote = 96;
        public int eventCount;
        public MTrkEventHolder<NoteEvent>[] noteEvents;
        public MTrkEventHolder<ControlChangeEvent>[] controlChangeEvents;
        public MTrkEventHolder<TextEvent>[] textEvents;
        public MTrkEventHolder<LyricEvent>[] lyricEvents;
        public MTrkEventHolder<MarkerEvent>[] markerEvents;
        public MTrkEventHolder<QueueEvent>[] queueEvents;
        public MTrkEventHolder<BeatEvent>[] beatEvents;
        public MTrkEventHolder<KeyEvent>[] keyEvents;

        MidiTrack _track;

        public MidiTrack Track
        {
            get
            {
                if (_track != null)
                    return _track;
                var mtrkEvents = new List<MTrkEvent>();
                return _track = new MidiTrack(trackName, tempo, duration, ticksPerQuarterNote, Translate());
            }
        }

        List<MTrkEvent> Translate()
        {
            var list = new List<MTrkEvent>();
            var listIndex = 0;
            var noteIndex = 0;
            var controlChangeIndex = 0;
            var textIndex = 0;
            var lyricIndex = 0;
            var markerIndex = 0;
            var queueIndex = 0;
            var beatIndex = 0;
            var keyIndex = 0;
            for (; listIndex < eventCount; listIndex++)
            {
                Search(ref noteIndex, noteEvents);
                Search(ref controlChangeIndex, controlChangeEvents);
                Search(ref textIndex, textEvents);
                Search(ref lyricIndex, lyricEvents);
                Search(ref markerIndex, markerEvents);
                Search(ref queueIndex, queueEvents);
                Search(ref beatIndex, beatEvents);
                Search(ref keyIndex, keyEvents);
            }

            return list;

            void Search<T>(ref int index, MTrkEventHolder<T>[] events) where T : MTrkEvent
            {
                if (events.Length == index) return;
                if (events[index].index == listIndex)
                {
                    list.Add((events[index].Event));
                    index++;
                }
            }
        }

        MidiTrackPlayer _player;

        MidiTrackPlayer Player
        {
            get
            {
                if (_player != null)
                    return _player;
                return _player = new MidiTrackPlayer(Track, PushSignal, true);
            }
        }

        List<MTrkEvent> MtrkEvents => Track.events;

        #endregion

        #region Public properties and methods

        public float DurationInSecond => Track.DurationInSecond;

        public float GetValue(UnityEngine.Playables.Playable playable, MidiControl control)
        {
            if (MtrkEvents == null) return 0;
            var t = (float) playable.GetTime() % DurationInSecond;
            if (control.mode == MidiControl.Mode.NoteEnvelope)
                return GetNoteEnvelopeValue(control, t);
            else if (control.mode == MidiControl.Mode.NoteCurve)
                return GetNoteCurveValue(control, t);
            else // CC
                return GetCCValue(control, t);
        }

        #endregion

        #region PlayableBehaviour implementation

        float _previousTime;

        public override void OnGraphStart(UnityEngine.Playables.Playable playable)
        {
            _previousTime = (float) playable.GetTime();
            Player.ResetHead(_previousTime);
        }

        public override void OnBehaviourPause(UnityEngine.Playables.Playable playable, FrameData info)
        {
            // When the playable is being finished, signals laying in the rest
            // of the clip should be all triggered.
            if (!playable.IsDone()) return;
            _playable = playable;
            _frameData = info;
            Player.PlayByAbstractTime((float) playable.GetDuration());
        }

        public override void PrepareFrame(UnityEngine.Playables.Playable playable, FrameData info)
        {
            _playable = playable;
            _frameData = info;
            var currentTime = (float) playable.GetTime();
            if (info.evaluationType == FrameData.EvaluationType.Playback)
                Player.PlayByAbstractTime(currentTime);
            _previousTime = currentTime;
        }

        #endregion

        #region MIDI signal emission

        private MidiSignalPool _signalPool = new MidiSignalPool();
        private UnityEngine.Playables.Playable _playable;
        private FrameData _frameData;

        void PushSignal(MTrkEvent mTrkEvent)
        {
            _frameData.output.PushNotification(_playable, _signalPool.Allocate(mTrkEvent));
        }

        #endregion

        #region Private variables and methods

        (ControlChangeEvent i0, ControlChangeEvent i1) GetCCEventIndexAroundTick(uint tick, int ccNumber)
        {
            var time = 0u;
            ControlChangeEvent lastEvent = null;
            foreach (var mEvent in MtrkEvents)
            {
                time += mEvent.ticks;
                if (!(mEvent is ControlChangeEvent e)) continue;
                if (e.controlChangeNumber != ccNumber) continue;
                if (time > tick) return (lastEvent, e);
                lastEvent = e;
            }

            return (lastEvent, lastEvent);
        }

        (NoteEvent iOn, NoteEvent iOff) GetNoteEventsBeforeTick(uint tick, MidiNoteFilter note)
        {
            NoteEvent eOn = null;
            NoteEvent eOff = null;
            var time = 0u;
            foreach (var mEvent in MtrkEvents)
            {
                time += mEvent.ticks;
                if (!(mEvent is NoteEvent e)) continue;
                if (time > tick) break;
                if (!note.Check(e)) continue;
                if (e.isNoteOn)
                {
                    eOn = e;
                    eOff = null;
                }
                else eOff = e;
            }

            return (eOn, eOff);
        }

        #endregion

        #region Envelope generator

        float CalculateEnvelope(MidiEnvelope envelope, float onTime, float offTime)
        {
            var attackTime = envelope.AttackTime;
            var attackRate = 1 / attackTime;

            var decayTime = envelope.DecayTime;
            var decayRate = 1 / decayTime;

            var level = -offTime / envelope.ReleaseTime;

            if (onTime < attackTime)
            {
                level += onTime * attackRate;
            }
            else if (onTime < attackTime + decayTime)
            {
                level += 1 - (onTime - attackTime) * decayRate * (1 - envelope.SustainLevel);
            }
            else
            {
                level += envelope.SustainLevel;
            }

            return Mathf.Max(0, level);
        }

        #endregion

        #region Value calculation methods

        float GetNoteEnvelopeValue(MidiControl control, float time)
        {
            var tick = Track.ConvertSecondToTicks(time);
            var (eOn, eOff) = GetNoteEventsBeforeTick(tick, control.noteFilter);

            if (eOn == null) return 0;

            // Note-on time
            Track.GetAbstractTime(eOn, out var onTime);

            // Note-off time
            var offTime = 0f;
            if (eOff != null)
                Track.GetAbstractTime(eOff, out offTime);
            else
                offTime = time;

            var envelope = CalculateEnvelope(
                control.envelope,
                Mathf.Max(0, offTime - onTime),
                Mathf.Max(0, time - offTime)
            );

            var velocity = eOn.velocity / 127.0f;

            return envelope * velocity;
        }

        float GetNoteCurveValue(MidiControl control, float time)
        {
            var tick = Track.ConvertSecondToTicks(time);
            var (iOn, iOff) = GetNoteEventsBeforeTick(tick, control.noteFilter);

            if (iOn == null) return 0;

            // Note-on time
            Track.GetAbstractTime(iOn, out var onTime);

            var curve = control.curve.Evaluate(Mathf.Max(0, time - onTime));
            var velocity = iOn.velocity / 127.0f;

            return curve * velocity;
        }

        float GetCCValue(MidiControl control, float time)
        {
            var tick = Track.ConvertSecondToTicks(time);
            var (i0, i1) = GetCCEventIndexAroundTick(tick, control.ccNumber);

            if (i0 == null) return 0;
            if (i1 == null) return i0.data / 127.0f;

            Track.GetAbstractTime(i0, out var t0);
            Track.GetAbstractTime(i1, out var t1);

            var v0 = i0.data / 127.0f;
            var v1 = i1.data / 127.0f;

            return Mathf.Lerp(v0, v1, Mathf.Clamp01((time - t0) / (t1 - t0)));
        }

        #endregion
    }
}