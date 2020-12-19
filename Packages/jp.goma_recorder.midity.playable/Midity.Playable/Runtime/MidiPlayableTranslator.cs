using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Midity.Playable
{
    public static class MidiPlayableTranslator
    {
        public static (MidiFileAsset, MidiAnimationAsset[]) Translate(MidiFile midiFile)
        {
            var midiFileAsset = MidiFileAsset.Instantiate(midiFile);
            var animations = new MidiAnimationAsset[midiFile.Tracks.Count];

            for (var i = 0; i < midiFile.Tracks.Count; i++)
            {
                var track = midiFile.Tracks[i];
                var anim = ScriptableObject.CreateInstance<MidiAnimationAsset>();
                anim.name = $"{i}:{track.Name}";
                anim.trackNumber = i;
                anim.midiFileAsset = midiFileAsset;

                var index = 0;
                var noteEvents = new List<MTrkEventHolder<NoteEvent>>();
                var controlChangeEvents = new List<MTrkEventHolder<ControlChangeEvent>>();
                var sequenceNumberEvents = new List<MTrkEventHolder<SequenceNumberEvent>>();
                var textEvents = new List<MTrkEventHolder<TextEvent>>();
                var copyrightEvents = new List<MTrkEventHolder<CopyrightEvent>>();
                var trackNameEvents = new List<MTrkEventHolder<TrackNameEvent>>();
                var instrumentNameEvents = new List<MTrkEventHolder<InstrumentNameEvent>>();
                var lyricEvents = new List<MTrkEventHolder<LyricEvent>>();
                var markerEvents = new List<MTrkEventHolder<MarkerEvent>>();
                var queueEvents = new List<MTrkEventHolder<QueueEvent>>();
                var channelPrefixEvents = new List<MTrkEventHolder<ChannelPrefixEvent>>();
                var endPointEvents = new List<MTrkEventHolder<EndPointEvent>>();
                var tempoEvents = new List<MTrkEventHolder<TempoEvent>>();
                var smpteOffsetEvents = new List<MTrkEventHolder<SmpteOffsetEvent>>();
                var beatEvents = new List<MTrkEventHolder<BeatEvent>>();
                var keyEvents = new List<MTrkEventHolder<KeyEvent>>();
                var sequencerUniqueEvents = new List<MTrkEventHolder<SequencerUniqueEvent>>();

                foreach (var t in track.Events)
                {
                    switch (t)
                    {
                        case NoteEvent noteEvent:
                            AddList(noteEvent, noteEvents);
                            break;
                        case ControlChangeEvent controlChangeEvent:
                            AddList(controlChangeEvent, controlChangeEvents);
                            break;
                        case SequenceNumberEvent sequenceNumberEvent:
                            AddList(sequenceNumberEvent, sequenceNumberEvents);
                            break;
                        case TextEvent textEvent:
                            AddList(textEvent, textEvents);
                            break;
                        case CopyrightEvent copyrightEvent:
                            AddList(copyrightEvent, copyrightEvents);
                            break;
                        case TrackNameEvent trackNameEvent:
                            AddList(trackNameEvent, trackNameEvents);
                            break;
                        case InstrumentNameEvent instrumentNameEvent:
                            AddList(instrumentNameEvent, instrumentNameEvents);
                            break;
                        case LyricEvent lyricEvent:
                            AddList(lyricEvent, lyricEvents);
                            break;
                        case MarkerEvent markerEvent:
                            AddList(markerEvent, markerEvents);
                            break;
                        case QueueEvent queueEvent:
                            AddList(queueEvent, queueEvents);
                            break;
                        case ChannelPrefixEvent channelPrefixEvent:
                            AddList(channelPrefixEvent, channelPrefixEvents);
                            break;
                        case EndPointEvent endPointEvent:
                            AddList(endPointEvent, endPointEvents);
                            break;
                        case TempoEvent tempoEvent:
                            AddList(tempoEvent, tempoEvents);
                            break;
                        case SmpteOffsetEvent smpteOffsetEvent:
                            AddList(smpteOffsetEvent, smpteOffsetEvents);
                            break;
                        case BeatEvent beatEvent:
                            AddList(beatEvent, beatEvents);
                            break;
                        case KeyEvent keyEvent:
                            AddList(keyEvent, keyEvents);
                            break;
                        case SequencerUniqueEvent sequencerUniqueEvent:
                            AddList(sequencerUniqueEvent, sequencerUniqueEvents);
                            break;
                    }
                }

                anim.eventCount = index;
                anim.noteEvents = noteEvents.ToArray();
                anim.controlChangeEvents = controlChangeEvents.ToArray();
                anim.textEvents = textEvents.ToArray();
                anim.lyricEvents = lyricEvents.ToArray();
                anim.markerEvents = markerEvents.ToArray();
                anim.queueEvents = queueEvents.ToArray();
                anim.beatEvents = beatEvents.ToArray();
                anim.keyEvents = keyEvents.ToArray();

                animations[i] = anim;

                void AddList<T>(T mEvent, List<MTrkEventHolder<T>> list) where T : MTrkEvent
                {
                    list.Add(new MTrkEventHolder<T>(index, mEvent));
                    index++;
                }
            }

            midiFileAsset.animationAssets = animations;
            return (midiFileAsset, animations);
        }
    }
}