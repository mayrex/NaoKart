using JetBrains.Annotations;
using M2MqttUnity.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{

    public RecordData record;

    public M2MqttUnityTest mqtt;
    private string message;

    
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

        message = checkpointSingle.transform.name + "," + checkpointSingle.gravit‡Curva + "," + checkpointSingle.applyFreno;

        mqtt.TestPublish(message);
        if (checkpointSingle.transform.name == "Curva1 ENTRY") 
        {
            record.InCurva = true;
            record.WriteCSV(record.InCurva);
        }
        else if(checkpointSingle.transform.name == "Curva1 EXIT")
        {
            record.InCurva = false;
        }
        return checkpointSingle.transform.name;
    }

    
}
