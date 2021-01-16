using System;
using UnityEngine;

namespace Midity.Playable
{
    // MIDI control class and related structs and enums

    #region Control parameter types

    public enum NoteNameFilter
    {
        All,
        C,
        CSharp,
        D,
        DSharp,
        E,
        F,
        FSharp,
        G,
        GSharp,
        A,
        ASharp,
        B
    }

    public enum OctaveFilter
    {
        All,
        Minus2,
        Minus1,
        Zero,
        Plus1,
        Plus2,
        Plus3,
        Plus4,
        Plus5,
        Plus6,
        Plus7,
        Plus8
    }

    [Serializable]
    public struct MidiNoteFilter
    {
        public NoteNameFilter noteNameFilter;
        public OctaveFilter octaveFilter;

        public bool Check(MTrkEvent e)
        {
            if (e is NoteEvent midiEvent)
                return Check(midiEvent);
            return false;
        }

        public bool Check(MTrkEvent e, out NoteEvent noteEvent)
        {
            if (e is NoteEvent ne)
            {
                noteEvent = ne;
                return Check(ne);
            }

            noteEvent = null;
            return false;
        }

        public bool Check(NoteEvent e)
        {
            return (octaveFilter == OctaveFilter.All || (int) e.NoteOctave == (int) octaveFilter - 1) &&
                   (noteNameFilter == NoteNameFilter.All || (int) e.NoteName == (int) noteNameFilter - 1);
        }
    }

    [Serializable]
    public struct MidiEnvelope
    {
        // ADSR parameters
        public float attack;
        public float decay;
        public float sustain;
        public float release;

        // Times in seconds
        public float AttackTime => Mathf.Max(1e-5f, attack / 10);
        public float DecayTime => Mathf.Max(1e-5f, decay / 10);
        public float ReleaseTime => Mathf.Max(1e-5f, release / 10);

        // Normalized sustain level value
        public float SustainLevel => Mathf.Clamp01(sustain);
    }

    #endregion

    #region Serializable MIDI control class

    [Serializable]
    public sealed class MidiControl
    {
        // Control mode (Note/CC)
        public enum Mode
        {
            NoteEnvelope,
            NoteCurve,
            CC
        }

        // Is this control enabled?
        public bool enabled = true;
        public Mode mode = Mode.NoteEnvelope;

        // (Note mode) Note filter
        public MidiNoteFilter noteFilter = new MidiNoteFilter
        {
            noteNameFilter = NoteNameFilter.All,
            octaveFilter = OctaveFilter.All
        };

        // (Note Envelope mode) Envelope parameters
        public MidiEnvelope envelope = new MidiEnvelope
        {
            attack = 0,
            decay = 1,
            sustain = 0.5f,
            release = 1
        };

        // (Note Curve mode) Envelope curve
        public AnimationCurve curve = new AnimationCurve(
            new Keyframe(0, 0, 90, 90),
            new Keyframe(0.02f, 1),
            new Keyframe(0.5f, 0)
        );

        // (CC mode) CC number
        public Controller ccController = Controller.ModulationWheelCourse;

        // Component/property options
        public ExposedReference<Component> targetComponent;
        public string propertyName;
        public string fieldName;

        // Value options
        public Vector4 vector0;
        public Vector4 vector1 = Vector3.one;
    }

    #endregion
}