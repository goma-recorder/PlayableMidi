using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Midity.Playable
{
    // Receives MIDI signals (MIDI event notifications) from a timeline and
    // invokes assigned events.
    [ExecuteInEditMode]
    public sealed class MidiSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        public MidiNoteFilter noteFilter = new MidiNoteFilter
        {
            note = MidiNote.All,
            octave = MidiOctave.All
        };

        public UnityEvent noteOnEvent = new UnityEvent();
        public UnityEvent noteOffEvent = new UnityEvent();
        public Action<MTrkEvent> onFireEvent = null;

        public void OnNotify
            (UnityEngine.Playables.Playable origin, INotification notification, object context)
        {
            var mtrkEvent = ((MidiSignal)notification).Event;
            if (onFireEvent != null)
                onFireEvent(mtrkEvent);
            if (noteFilter.Check(mtrkEvent, out var noteEvent))
                (noteEvent.IsNoteOn ? noteOnEvent : noteOffEvent).Invoke();
        }
    }
}
