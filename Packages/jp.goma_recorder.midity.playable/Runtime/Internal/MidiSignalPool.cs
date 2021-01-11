using System.Collections.Generic;

namespace Midity.Playable
{
    // Object pool class for MIDI signals
    internal sealed class MidiSignalPool
    {
        private readonly Stack<MidiSignal> _freeSignals = new Stack<MidiSignal>();
        private readonly Stack<MidiSignal> _usedSignals = new Stack<MidiSignal>();

        public MidiSignal Allocate(MTrkEvent data)
        {
            var signal = _freeSignals.Count > 0 ? _freeSignals.Pop() : new MidiSignal();
            signal.Event = data;
            _usedSignals.Push(signal);
            return signal;
        }

        public void ResetFrame()
        {
            while (_usedSignals.Count > 0) _freeSignals.Push(_usedSignals.Pop());
        }
    }
}