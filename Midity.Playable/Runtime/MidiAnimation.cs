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
        public MTrkEventHolder<MidiEvent>[] midiEvents;
        public MTrkEventHolder<TextEvent>[] textEvents;
        public MTrkEventHolder<LyricEvent>[] lyricEvents;
        public MTrkEventHolder<MarkerEvent>[] markerEvents;
        public MTrkEventHolder<QueueEvent>[] queueEvents;
        public MTrkEventHolder<BeatEvent>[] beatEvents;
        public MTrkEventHolder<KeyEvent>[] keyEvents;

        MidiTrack _track;
        MidiTrack track
        {
            get
            {
                if (_track != null)
                    return _track;
                var mtrkEvents = new List<MTrkEvent>();
                return _track = new MidiTrack()
                {
                    name = trackName,
                    tempo = tempo,
                    duration = duration,
                    ticksPerQuarterNote = ticksPerQuarterNote,
                    events = Translate(),
                };
            }
        }

        List<MTrkEvent> Translate()
        {
            var list = new List<MTrkEvent>();
            var listIndex = 0;
            var midiIndex = 0;
            var textIndex = 0;
            var lyricIndex = 0;
            var markerIndex = 0;
            var queueIndex = 0;
            var beatIndex = 0;
            var keyIndex = 0;
            for (; listIndex < eventCount; listIndex++)
            {
                Search(ref midiIndex, midiEvents);
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
        MidiTrackPlayer player
        {
            get
            {
                if (_player != null)
                    return _player;
                return _player = new MidiTrackPlayer(track);
            }
        }
        List<MTrkEvent> mtrkEvents => track.events;

        #endregion

        #region Public properties and methods

        public float DurationInSecond => track.DurationInSecond;

        public float GetValue(UnityEngine.Playables.Playable playable, MidiControl control)
        {
            if (mtrkEvents == null) return 0;
            var t = (float)playable.GetTime() % DurationInSecond;
            if (control.mode == MidiControl.Mode.NoteEnvelope)
                return GetNoteEnvelopeValue(control, t);
            else if (control.mode == MidiControl.Mode.NoteCurve)
                return GetNoteCurveValue(control, t);
            else // CC
                return GetCCValue(control, t);
        }

        #endregion

        #region PlayableBehaviour implementation

        float previousTime;

        public override void OnGraphStart(UnityEngine.Playables.Playable playable)
        {
            previousTime = (float)playable.GetTime();
            player.ResetHead(previousTime);
        }

        public override void OnBehaviourPause(UnityEngine.Playables.Playable playable, FrameData info)
        {
            // When the playable is being finished, signals laying in the rest
            // of the clip should be all triggered.
            if (!playable.IsDone()) return;
            var pushAction = GetPushAction(playable, info);
            player.Play((float)playable.GetDuration(), pushAction);
        }

        public override void PrepareFrame(UnityEngine.Playables.Playable playable, FrameData info)
        {
            var pushAction = GetPushAction(playable, info);
            var currentTime = (float)playable.GetTime();
            if (info.evaluationType == FrameData.EvaluationType.Playback)
                player.Play(currentTime, pushAction);
            previousTime = currentTime;
        }

        #endregion

        #region MIDI signal emission

        MidiSignalPool _signalPool = new MidiSignalPool();

        Action<MTrkEvent> GetPushAction(UnityEngine.Playables.Playable playable, FrameData info)
        {
            return e =>
                info.output.PushNotification(playable, _signalPool.Allocate(e));
        }

        #endregion

        #region Private variables and methods

        (MidiEvent i0, MidiEvent i1) GetCCEventIndexAroundTick(uint tick, int ccNumber)
        {
            var time = 0u;
            MidiEvent lastEvent = null;
            foreach (var mEvent in mtrkEvents)
            {
                time += mEvent.ticks;
                if (!(mEvent is MidiEvent e)) continue;
                if (!e.IsCC || e.data1 != ccNumber) continue;
                if (time > tick) return (lastEvent, e);
                lastEvent = e;
            }
            return (lastEvent, lastEvent);
        }

        (MidiEvent iOn, MidiEvent iOff) GetNoteEventsBeforeTick(uint tick, MidiNoteFilter note)
        {
            MidiEvent iOn = null;
            MidiEvent iOff = null;
            var time = 0u;
            foreach (var mEvent in mtrkEvents)
            {
                time += mEvent.ticks;
                if (!(mEvent is MidiEvent e)) continue;
                if (time > tick) break;
                if (!note.Check(e)) continue;
                if (e.IsNoteOn)
                {
                    iOn = e;
                    iOff = null;
                } 
                else iOff = e;
            }
            return (iOn, iOff);
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
            var tick = track.ConvertSecondToTicks(time);
            var (eOn,eOff) = GetNoteEventsBeforeTick(tick, control.noteFilter);

            if (eOn == null) return 0;

            // Note-on time
             track.GetAbstractTime(eOn,out var onTime);

            // Note-off time
            var offTime = 0f;
            if (eOff != null)
                track.GetAbstractTime(eOff, out offTime);
            else
                offTime = time;

            var envelope = CalculateEnvelope(
                control.envelope,
                Mathf.Max(0, offTime - onTime),
                Mathf.Max(0, time - offTime)
            );

            var velocity = eOn.data2 / 127.0f;

            return envelope * velocity;
        }

        float GetNoteCurveValue(MidiControl control, float time)
        {
            var tick = track.ConvertSecondToTicks(time);
            var (iOn,iOff) = GetNoteEventsBeforeTick(tick, control.noteFilter);

            if (iOn == null) return 0;

            // Note-on time
            track.GetAbstractTime(iOn ,out var onTime);

            var curve = control.curve.Evaluate(Mathf.Max(0, time - onTime));
            var velocity = iOn.data2 / 127.0f;

            return curve * velocity;
        }

        float GetCCValue(MidiControl control, float time)
        {
            var tick = track.ConvertSecondToTicks(time);
            var (i0,i1) = GetCCEventIndexAroundTick(tick, control.ccNumber);

            if (i0 == null) return 0;
            if (i1 == null) return i0.data2 / 127.0f;

            track.GetAbstractTime(i0, out var t0);
            track.GetAbstractTime(i1, out var t1);

            var v0 = i0.data2 / 127.0f;
            var v1 = i1.data2 / 127.0f;

            return Mathf.Lerp(v0, v1, Mathf.Clamp01((time - t0) / (t1 - t0)));
        }

        #endregion
    }
}
