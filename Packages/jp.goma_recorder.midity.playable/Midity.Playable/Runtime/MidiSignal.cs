using UnityEngine;
using UnityEngine.Playables;

namespace Midity.Playable
{
    // Payload for MIDI event notifications
    public sealed class MidiSignal : INotification
    {
        // Notification ID (not in use)
        PropertyName INotification.id { get { return default(PropertyName); } }

        // MIDI event
        public MTrkEvent Event { get; set; }
    }
}
