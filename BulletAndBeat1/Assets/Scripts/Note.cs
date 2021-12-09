﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime; //Tap time

    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2)); //

        if(t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            //Position the notes and spawn targets
            transform.localPosition = Vector3.Lerp(Vector3.forward * SongManager.Instance.noteSpawnX, Vector3.forward * SongManager.Instance.noteDespawnX, t);
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
