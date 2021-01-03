using UnityEngine;
using UnityEngine.Playables;

namespace Midity.Playable
{
    // Payload for MIDI event notifications
    public sealed class MidiSignal : INotification
    {
        // MIDI event
        public MTrkEvent Event { get; set; }

        // Notification ID (not in use)
        PropertyName INotification.id => default;
    }
}