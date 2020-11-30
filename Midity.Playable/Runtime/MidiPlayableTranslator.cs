using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Midity.Playable
{
    public static class MidiPlayableTranslator
    {
        public static MidiFileAsset Translate(MidiTrack[] tracks)
        {
            var animations = tracks.Select(track =>
            {
                var anim = ScriptableObject.CreateInstance<MidiAnimationAsset>();
                anim.name = anim.template.trackName = track.name;
                anim.template.tempo = track.tempo;
                anim.template.duration = track.duration;
                anim.template.ticksPerQuarterNote = track.ticksPerQuarterNote;


                var offset = 0u;
                var index = 0;

                var noteEvents = new List<MTrkEventHolder<NoteEvent>>();
                var controlChangeEvents = new List<MTrkEventHolder<ControlChangeEvent>>();
                var textEvents = new List<MTrkEventHolder<TextEvent>>();
                var lyricEvents = new List<MTrkEventHolder<LyricEvent>>();
                var markerEvents = new List<MTrkEventHolder<MarkerEvent>>();
                var queueEvents = new List<MTrkEventHolder<QueueEvent>>();
                var beatEvents = new List<MTrkEventHolder<BeatEvent>>();
                var keyEvents = new List<MTrkEventHolder<KeyEvent>>();
                foreach (var t in track.events)
                {
                    switch (t)
                    {
                        case NoteEvent noteEvent:
                            AddList(noteEvent,noteEvents);
                            break;
                        case ControlChangeEvent controlChangeEvent:
                            AddList(controlChangeEvent,controlChangeEvents);
                            break;
                        case TextEvent textEvent:
                            AddList(textEvent, textEvents);
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
                        case BeatEvent beatEvent:
                            AddList(beatEvent, beatEvents);
                            break;
                        case KeyEvent keyEvent:
                            AddList(keyEvent, keyEvents);
                            break;
                        default:
                            offset += t.ticks;
                            break;
                    }
                }
                anim.template.eventCount = index;
                anim.template.noteEvents = noteEvents.ToArray();
                anim.template.controlChangeEvents = controlChangeEvents.ToArray();
                anim.template.textEvents = textEvents.ToArray();
                anim.template.lyricEvents = lyricEvents.ToArray();
                anim.template.markerEvents = markerEvents.ToArray();
                anim.template.queueEvents = queueEvents.ToArray();
                anim.template.beatEvents = beatEvents.ToArray();
                anim.template.keyEvents = keyEvents.ToArray();
                return anim;

                void AddList<T>(T mEvent, List<MTrkEventHolder<T>> list) where T : MTrkEvent
                {
                    mEvent.ticks += offset;
                    list.Add(new MTrkEventHolder<T>(index, mEvent));
                    offset = 0u;
                    index++;
                }
            }).ToArray();
            // Asset instantiation
            var asset = ScriptableObject.CreateInstance<MidiFileAsset>();
            asset.tracks = animations;
            return asset;
        }
    }
}
