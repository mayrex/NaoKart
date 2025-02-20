using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class TelemetryRecorder : MonoBehaviour
{
    private List<string> records = new List<string>();
    private Rigidbody rb;
    private float lapTimer = 0f; // Aggiunto il timer per i giri

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody non trovato su " + gameObject.name);
        }
    }

    void FixedUpdate()
    {
        lapTimer += Time.fixedDeltaTime; // Incrementa il timer giro
        float t = Time.time;
        Vector3 pos = transform.position;
        Vector3 vel = rb != null ? rb.velocity : Vector3.zero;

        // Salva il record in stringa CSV
        string record = $"{t:F2},{pos.x:F3},{pos.y:F3},{pos.z:F3},{vel.x:F3},{vel.y:F3},{vel.z:F3},{lapTimer:F2}";
        records.Add(record);
    }

    void OnApplicationQuit()
    {
        WriteTelemetryToCSV();
    }

    private void WriteTelemetryToCSV()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "TelemetryData.csv");

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Time,PosX,PosY,PosZ,VelX,VelY,VelZ,LapTime");

        foreach (var record in records)
        {
            sb.AppendLine(record);
        }

        try
        {
            File.WriteAllText(filePath, sb.ToString());
            Debug.Log("Dati di telemetria salvati in: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Errore nella scrittura del file CSV: " + e.Message);
        }
    }

    // **Metodo ResetTimer()**
    public void ResetTimer()
    {
        lapTimer = 0f;
        Debug.Log("Timer giro resettato!");
    }
}
