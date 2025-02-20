using JetBrains.Annotations;
using M2MqttUnity.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public TelemetryRecorder telemetryRecorder; // Riferimento da assegnare nell'Inspector
    public M2MqttUnityTest mqtt;
    private string message;

    private void Awake()
    {
        if (telemetryRecorder == null)
        {
            Debug.LogError("TelemetryRecorder non assegnato in TrackCheckpoints! Assegna il riferimento nell'Inspector.");
        }

        Transform checkpointsTransform = transform.Find("Checkpoints");
        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
        }
    }

    public string PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
    {
        // Componi il messaggio per la pubblicazione MQTT
        message = checkpointSingle.transform.name + "," + checkpointSingle.gravit‡Curva + "," + checkpointSingle.applyFreno;
        mqtt.TestPublish(message);

        // Reset del timer al checkpoint Start/Finish
        if (checkpointSingle.transform.name == "Start/Finish" && telemetryRecorder != null)
        {
            //Debug.Log("Turco gay EZZY");
            telemetryRecorder.ResetTimer();
        }

        return checkpointSingle.transform.name;
    }
}
