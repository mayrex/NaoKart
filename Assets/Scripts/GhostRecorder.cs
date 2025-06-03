using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VehiclePhysics;

[System.Serializable]
public struct GhostFrame
{
    public float time;           
    public Vector3 position;     
    public Quaternion rotation;  
}

[System.Serializable]
public class GhostData
{
    public List<GhostFrame> frames = new List<GhostFrame>();
}


public class GhostRecorder : MonoBehaviour
{
    [Tooltip("Componente Vehicle Physics Pro del veicolo da registrare.")]
    public VehicleBehaviour vehicle;

    private GhostData data = new GhostData();
    private float startTime;
    private bool recording = false;

    void Update()
    {
        // Avvia registrazione
        if (Input.GetKeyDown(KeyCode.R) && !recording)
        {
            StartRecording();
        }
        // Ferma e salva dati
        if (Input.GetKeyDown(KeyCode.S) && recording)
        {
            StopAndSave();
        }
    }

    void FixedUpdate()
    {
        // Durante la registrazione, campiona ogni physics step
        if (!recording) return;

        float t = Time.time - startTime;
        data.frames.Add(new GhostFrame
        {
            time = t,
            position = vehicle.transform.position,
            rotation = vehicle.transform.rotation
        });
    }

    /// <summary>
    /// Inizia la registrazione: reset dei dati e azzeramento timer.
    /// </summary>
    public void StartRecording()
    {
        data.frames.Clear();
        startTime = Time.time;
        recording = true;
        Debug.Log("GhostRecorder: registrazione avviata.");
    }

    /// <summary>
    /// Ferma la registrazione e salva i dati in formato JSON.
    /// </summary>
    public void StopAndSave()
    {
        recording = false;
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "facile.json");
        File.WriteAllText(path, json);
        Debug.Log($"GhostRecorder: registrazione salvata in {path} con {data.frames.Count} frame.");
    }
}
