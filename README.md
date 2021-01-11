# Playable Midi

![Ableton](https://i.imgur.com/yxJr18E.png)
![Unity](https://i.imgur.com/aTMgdnB.gif)

**Playable Midi** is a custom timeline/playables package that provides
functionality to control object properties based on sequence data contained
in a standard MIDI file (`.mid` file). This allows you to create musically
synchronized animation using a DAW (digital audio workstation) that is easy
to manage accurately synchronized timings compared to other non-musical
timeline editors like Unity's one. Playable Midi is an extension of the [MIDI Animation Track](https://github.com/keijiro/MidiAnimationTrack).


## System requirements

- Unity 2020.1 or later

## Dependency
- [Midity](https://github.com/goma-recorder/Midity)

## Installation

### A. OpenUPM-CLI
```
$ openupm add jp.goma_recorder.midity.playable
```
[OpenUpm](https://openupm.com/packages/jp.goma_recorder.midity.playable)

### B. Git url
`Window -> Package Manager -> +▼ -> Add package from git url`
 - `https://github.com/goma-recorder/PlayableMidi.git?#upm`
 
 ### C. Release page
[here](https://github.com/goma-recorder/PlayableMidi/releases)

## Importing .mid files

You can import a `.mid` file as an asset file. Simply drag and drop it to the
project view, or navigate to "Assets" - "Import New Asset...".

An imported asset may contain multiple tracks that are shown as sub-assets
under it.

![Sub-assets](https://i.imgur.com/tuBe3py.png)

To create a MIDI animation track, drag and drop one of these clip assets to
a timeline.

![Drag and drop](https://i.imgur.com/WVMaG6J.gif)

## Track controls

You can animate object properties from a MIDI animation track using **track
controls**. To create a new track control, select a MIDI animation track (not a
clip) in the Timeline editor. Then track control editor will appear in the
inspector.

![Track controls](https://i.imgur.com/uOwwWKR.gif)

A track control only can animate a single property. You can add multiple
controls to animate multiple different properties.

At the moment, a track control only supports `float`, `Vector3`, `Quaternion`
and `Color` properties. Please note that it requires a public *property* to
be animated; Just providing a public *variable* is not enough.

There are three modes in the track control: **Note Envelope**, **Note Curve**
and **CC**.

### Note Envelope mode

![Inspector](https://i.imgur.com/7SFMCk9.png)

**Note Envelope** is a mode where a property is animated by a classic [ADSR]
style envelope. This mode is useful when you want to make animation react to
key-off events.

You can specify which **Note/Octave** the control reacts to. Please note that
key velocity affects the envelope. It's simply multiplied to the envelope
output.

[ADSR]: https://en.wikipedia.org/wiki/Envelope_(music)

### Note Curve mode

![Inspector](https://i.imgur.com/YFATPN0.png)

**Note Curve** is a mode where a property is animated by an animation curve.
It starts playing animation on key-on events and keeps playing until it ends
(key-off events will be ignored).

You can specify which **Note/Octave** the control reacts to. Please note that
key velocity affects the animation curve. It's simply multiplied to the curve
value.

### CC mode

![Inspector](https://i.imgur.com/ERFtPKL.png)

**CC** is a mode where a property is animated by CC (control change) events
contained in a MIDI track.

You can specify which **CC Number** the control reacts to.

## MIDI signals

A MIDI animation track also supports sending [Timeline Signals] on key-on/off and some Meta
events. To receive MIDI events from a track, you can use the **MIDI Signal
Receiver** component.

[Timeline Signals]:
    https://blogs.unity3d.com/2019/05/21/how-to-use-timeline-signals/

![Inspector](https://i.imgur.com/LAwiWel.png)

1. Add the MIDI Signal Receiver component to a game object that receives MIDI
   signals.
2. Specify which **Note/Octave** the receiver reacts to.
3. Register methods to **Note On/Off Events**.
4. Set the receiver game object as the output destination of the track.

![Output destination](https://i.imgur.com/PqYi9cN.gif)
