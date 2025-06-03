using JetBrains.Annotations;
using M2MqttUnity.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrackCheckpoints : MonoBehaviour
{
    public TelemetryRecorder telemetryRecorder; // Riferimento da assegnare nell'Inspector
    public M2MqttUnityTest mqtt;
    [SerializeField] string message;
    private string lap;
    private string AiMessage;
    public TextMeshProUGUI sottotitoli;

    public float BestLapTime { get; private set; } = Mathf.Infinity; //  you can GET the laptime value from outside the class. (because its public) private set means you only can set the value inside the class.
    public float LastLapTime { get; private set; } = 0;
    public float CurrentLapTime { get; private set; } = 0;
    public float CurrentLap { get; private set; } = 0;

    public float TotalLapTime { get; private set; } = 0;

    private List<float> bestLapSplits = new List<float>(); // tempi parziali best lap
    private List<float> currentLapSplits = new List<float>(); // tempi parziali giro attuale
    private float lapTimer;
    private int lastCheckpointPassed = 0;
    public TextMeshProUGUI bestLapTime;
    public TextMeshProUGUI lastLapTime;
    public TextMeshProUGUI currentLapTime;
    public TextMeshProUGUI totalLapTime;
    public GhostRecorder ghostRecorder;


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
        message = checkpointSingle.gravitàCurva;
        sottotitoli.text = message;
        mqtt.Checkpoint(message);


        // Aggiorna la curva corrente e la posizione ideale
        if (telemetryRecorder != null)
        {
            telemetryRecorder.AggiornaCurva(checkpointSingle.gravitàCurva, checkpointSingle.transform.position);
        }

        // Se è il traguardo
        if (checkpointSingle.transform.name == "Start/Finish" && telemetryRecorder != null)
        {
            string BestLapformatted;
            string LastLapformatted;
            LastLapTime = lapTimer;
            
      
            // Se è il miglior tempo, salviamo anche i parziali
            if (LastLapTime < BestLapTime)
            {
                BestLapTime = LastLapTime;
                bestLapSplits = new List<float>(currentLapSplits);
                Debug.Log("Nuovo giro più veloce! Tempo: " + BestLapTime.ToString("F2") + " secondi");
                lap = "FUCSIA, GIRO VELOCE." + BestLapTime.ToString("F2") + " secondi";
                mqtt.Best_giro(lap);
                BestLapformatted= FormatTime(BestLapTime);
                LastLapformatted= FormatTime(LastLapTime);
                bestLapTime.text = "Miglior giro:" + BestLapformatted.ToString();
                lastLapTime.text = "Ultimo giro:" + LastLapformatted.ToString();
                
            }
            else
            {
                Debug.Log("Giro completato. Tempo: " + LastLapTime + " secondi");
                lap = "Il tuo ultimo giro è stato di: " + LastLapTime + " secondi";
                mqtt.Checkpoint(lap);
                LastLapformatted = FormatTime(LastLapTime);
                lastLapTime.text = "Ultimo giro:" + LastLapformatted.ToString();

            }

            lapTimer = 0;
            telemetryRecorder.ResetTimer();
            CurrentLap++;
            currentLapSplits.Clear();
            AiMessage = "Miglior Giro: " + BestLapTime + "; Ultimo Giro: " + LastLapTime + "; Numero Giri: " + CurrentLap + ";";
            mqtt.AiMessage(AiMessage);
          
           /* if (CurrentLap == 2) { 
            
                ghostRecorder.StopAndSave();
            }
           */
        }

        return checkpointSingle.transform.name;
     
    }
    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
    private void Update()
    {
        TotalLapTime += Time.deltaTime;
        lapTimer += Time.deltaTime;
        CurrentLapTime = lapTimer;
        string CurrentLapformatted = FormatTime(CurrentLapTime);
        string TotalLapFormatted = FormatTime(TotalLapTime);
        currentLapTime.text = "Tempo:      " + CurrentLapformatted.ToString();
        totalLapTime.text = "Totale:      " + TotalLapFormatted.ToString();
        

    }

}





