using System.Collections;
using System.Collections.Generic;
using Midity;
using Midity.Playable;
using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private MidiSignalReceiver midiSignalReceiver;
    [SerializeField] private MidiFileAsset[] midiFileAssets;
    [SerializeField] private GameObject prefab;
    [SerializeField] private string[] texts;

    [ContextMenu("Hello")]
    void Start()
    {
        byte maxNote = 0;
        byte minNote = 255;

        foreach (var midiFile in midiFileAssets)
        {
            foreach (var pair in midiFile.MidiFile.Tracks[0].NoteEventPairs)
            {
                if (pair.NoteNumber > maxNote)
                    maxNote = pair.NoteNumber;
                if (pair.NoteNumber < minNote)
                    minNote = pair.NoteNumber;
            }
        }

        midiSignalReceiver.onFireEvent = Instantiate;


        void Instantiate(MTrkEvent mTrkEvent)
        {
            if (!(mTrkEvent is OnNoteEvent onNoteEvent)) return;
            if (cubeParent is null) cubeParent = new GameObject("Cube Parent");
            var instance = GameObject.Instantiate(prefab, cubeParent.transform);
            instance.transform.localScale = new Vector3(1, onNoteEvent.NoteEventPair.LengthTick / 960f, 1);
            var offset = -(minNote + (maxNote - minNote) / 2f);
            instance.transform.position = new Vector3(offset + onNoteEvent.NoteNumber, 4);
        }
    }

    GameObject cubeParent = null;
    private int textCount = 0;
    [SerializeField] private Text text;
    public void DestroyCubeParent()
    {
        Destroy(cubeParent);
        cubeParent = null;
        text.text = texts[textCount++];
    }
}