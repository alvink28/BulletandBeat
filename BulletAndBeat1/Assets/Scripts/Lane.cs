using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction; //limit to the key we want
    public KeyCode input;
    public GameObject notePrefab;
    List<Note> notes = new List<Note>(); //List the notes
    public List<double> timeStamps = new List<double>(); //List the time stamps

    int spawnIndex = 0;
    int inputIndex = 0;


    void Start()
    {
        
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array) 
    {
        foreach (var note in array)
        {
            if(note.NoteName == noteRestriction)
            {
                //filter the notes
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap()); //Convert tempo map to metric time
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f); //Get midi time in seconds
            }
        }
    }

    void Update()
    {
        if(spawnIndex < timeStamps.Count)
        {
            if(SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime) //time for this note to spawn
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<Note>());
                note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }

        if(inputIndex < timeStamps.Count)
        {
            //simplify codes
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (Input.GetKeyDown(input)) //2D mode key press
            {
                if (Math.Abs(audioTime - timeStamp) < marginOfError) //hit within margine of error
                {
                    Hit();
                    print($"Hit on {inputIndex} note");
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                    Score.scoreValue += 50;
                }
                else
                {
                    print($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
                }
            }
            if (timeStamp + marginOfError <= audioTime) //Missed 
            {
                Miss();
                print($"Missed {inputIndex} note");
                inputIndex++;
            }
        }
    }
    private void Hit()
    {
        ScoreManager.Hit();
    }
    private void Miss()
    {
        ScoreManager.Miss();
    }
}
