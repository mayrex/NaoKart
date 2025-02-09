using JetBrains.Annotations;
using M2MqttUnity.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{

    public M2MqttUnityTest mqtt;
    private void Awake()
    {
        Transform checkpointsTransform = transform.Find("Checkpoints");
        foreach(Transform checkpointSingleTransform  in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
        }
    }

    public string PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
    {
        mqtt.TestPublish(checkpointSingle.transform.name);
        Debug.Log(checkpointSingle.transform.name);
        return checkpointSingle.transform.name;
    }

    
}
