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
            noteNameFilter = NoteNameFilter.All,
            octaveFilter = OctaveFilter.All
        };

        public UnityEvent noteOnEvent = new UnityEvent();
        public UnityEvent noteOffEvent = new UnityEvent();
        public Action<MTrkEvent> onFireEvent = null;

        public void OnNotify
            (UnityEngine.Playables.Playable origin, INotification notification, object context)
        {
            var mtrkEvent = ((MidiSignal) notification).Event;
            onFireEvent?.Invoke(mtrkEvent);
            if (noteFilter.Check(mtrkEvent, out var noteEvent))
                (noteEvent is OnNoteEvent ? noteOnEvent : noteOffEvent).Invoke();
        }
    }
}