using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;
    public AudioSource audioSource;
    public Lane[] lanes; //list of lanes
    public float songDelayInSeconds;
    public int inputDelayInMilliseconds;
    public double marginOfError; //in seconds

    public string fileLocation;
    public float noteTime; //How long it will be on the screen
    public float noteSpawnX; //How far does it start spawning
    public float noteTapX; //Where to tap/ hit the target

    public float noteDespawnX
    {
        get
        {
            return noteTapX - (noteSpawnX - noteTapX);
        }
    }

    public static MidiFile midiFile; //load MIDI

    void Start()
    {
        Instance = this;
        ReadFromFile();
    }

    private void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        GetDataFromMidi();
    }

    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes(); //get the notes
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count]; //Create an array for the notes
        notes.CopyTo(array, 0);
        foreach (var lane in lanes) lane.SetTimeStamps(array); //Set time stamps for each lane

        Invoke(nameof(StartSong), songDelayInSeconds); //start the song
    }

    public void StartSong()
    {
        audioSource.Play();
    }

    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency; //get audio time in double
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
