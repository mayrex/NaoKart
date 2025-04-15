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
    public DeltaTimeUI deltaTimeUI;


    public float BestLapTime { get; private set; } = Mathf.Infinity; //  you can GET the laptime value from outside the class. (because its public) private set means you only can set the value inside the class.
    public float LastLapTime { get; private set; } = 0;
    public float CurrentLapTime { get; private set; } = 0;
    public float CurrentLap { get; private set; } = 0;
    private List<float> bestLapSplits = new List<float>(); // tempi parziali best lap
    private List<float> currentLapSplits = new List<float>(); // tempi parziali giro attuale
    private float lapTimer;
    private int lastCheckpointPassed = 0;


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
        // Componi il messaggio per la pubblicazione MQTT
        message = checkpointSingle.gravitàCurva;
        mqtt.TestPublish(message);

        // Tempo attuale al checkpoint
        float currentSplit = CurrentLapTime;
        currentLapSplits.Add(currentSplit);

        // Calcolo del Delta Time (solo se esiste un best lap valido)
        if (bestLapSplits.Count == currentLapSplits.Count)
        {
            float bestSplit = bestLapSplits[currentLapSplits.Count - 1];
            float delta = currentSplit - bestSplit;
            if (deltaTimeUI != null)
            {
                deltaTimeUI.UpdateDeltaBar(delta);
            }

            string colore = delta < 0 ? "verde" : "rosso";
            Debug.Log($"{colore} Delta Time al checkpoint '{checkpointSingle.transform.name}': {delta:+0.00;-0.00} s");
        }
        else
        {
            Debug.Log($"Checkpoint '{checkpointSingle.transform.name}' - Tempo: {currentSplit:F2} s");
        }

        // Se è il traguardo
        if (checkpointSingle.transform.name == "Start/Finish" && telemetryRecorder != null)
        {
            LastLapTime = lapTimer;

            // Se è il miglior tempo, salviamo anche i parziali
            if (LastLapTime < BestLapTime)
            {
                BestLapTime = LastLapTime;
                bestLapSplits = new List<float>(currentLapSplits); // copia
                Debug.Log("Nuovo giro più veloce! Tempo: " + BestLapTime.ToString("F2") + " secondi");
            }
            else
            {
                Debug.Log("Giro completato. Tempo: " + LastLapTime.ToString("F2") + " secondi");
            }
            if (deltaTimeUI != null)
            {
                deltaTimeUI.UpdateLapTimes(BestLapTime, LastLapTime);
            }
            lapTimer = 0;
            telemetryRecorder.ResetTimer();
            CurrentLap++;
            currentLapSplits.Clear(); // reset per il prossimo giro
        }

        return checkpointSingle.transform.name;
    }
    private void Update()
    {
        lapTimer += Time.deltaTime;
        CurrentLapTime = lapTimer;
    }

}





