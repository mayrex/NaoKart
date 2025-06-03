using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class GhostPlayer : MonoBehaviour
{
    [Tooltip("Transform del prefab ghost (usare un Rigidbody isKinematic).")]
    public Transform ghostTransform;

    private GhostData data;
    private float startTime;
    private int prevIndex = 0;

    void Start()
    {
        // Carica i dati dal file
        string path = Path.Combine(Application.persistentDataPath, "facile.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning($"GhostPlayer: file non trovato in {path}.");
            return;
        }

        string json = File.ReadAllText(path);
        data = JsonUtility.FromJson<GhostData>(json);
        if (data == null || data.frames.Count < 2)
        {
            Debug.LogWarning("GhostPlayer: dati insufficienti per riproduzione.");
            data = null;
            return;
        }

        // Avvia il timer di riproduzione
        startTime = Time.time;
        Debug.Log($"GhostPlayer: caricati {data.frames.Count} frame. Riproduzione avviata.");
    }

    void FixedUpdate()
    {
        if (data == null) return;

        float t = Time.time - startTime;
        // Se oltre l'ultimo frame, fermati o loop are optional
        if (t > data.frames[data.frames.Count - 1].time)
            return;

        // Trova il frame corrente (ottimizzato con prevIndex)
        while (prevIndex < data.frames.Count - 2 && data.frames[prevIndex + 1].time <= t)
        {
            prevIndex++;
        }

        GhostFrame a = data.frames[prevIndex];
        GhostFrame b = data.frames[prevIndex + 1];
        float u = (t - a.time) / (b.time - a.time);

        // Interpola posizione e rotazione
        Vector3 pos = Vector3.Lerp(a.position, b.position, u);
        Quaternion rot = Quaternion.Slerp(a.rotation, b.rotation, u);

        ghostTransform.position = pos;
        ghostTransform.rotation = rot;
    }
}
