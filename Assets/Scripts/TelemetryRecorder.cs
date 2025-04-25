using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using VehiclePhysics;

public class TelemetryRecorder : MonoBehaviour
{
    private List<string> records = new List<string>();
    private Rigidbody rb;
    private VehicleBase vehicle;
    private float lapTimer = 0f;
    private Vector3 previousVelocity = Vector3.zero;
    // Nuovi campi
    public string curvaAttuale = "None";
    private Vector3 idealCheckpointPosition = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        vehicle = GetComponent<VehicleBase>();

        if (rb == null)
            Debug.LogWarning("Rigidbody non trovato su " + gameObject.name);

        if (vehicle == null)
            Debug.LogWarning("VehicleBase non trovato su " + gameObject.name);
    }

    void FixedUpdate()
    {
        lapTimer += Time.fixedDeltaTime;

        float t = Time.time;
        Vector3 pos = transform.position;
        Vector3 vel = rb != null ? rb.velocity : Vector3.zero;

        // Calcolo dell'accelerazione
        Vector3 acceleration = (vel - previousVelocity) / Time.fixedDeltaTime;
        previousVelocity = vel;

        // Velocità locale
        Vector3 localVelocity = vehicle.transform.InverseTransformDirection(vel);
        Vector3 localAcceleration = vehicle.transform.InverseTransformDirection(acceleration);

        // Input del pilota
        float steering = GetInput(InputData.Steer);
        float throttle = GetInput(InputData.Throttle);
        float brake = GetInput(InputData.Brake);

        // Rotazioni (in gradi)
        Vector3 rotationEuler = transform.rotation.eulerAngles;

        // Distanza dal checkpoint ideale (se impostato)
        float distanceFromIdeal = (idealCheckpointPosition == Vector3.zero) ? 0f : Vector3.Distance(pos, idealCheckpointPosition);

        // Record CSV
        string record = $"{t:F2},{pos.x:F3},{pos.y:F3},{pos.z:F3}," +
                        $"{vel.x:F3},{vel.y:F3},{vel.z:F3}," +
                        $"{steering:F3},{throttle:F3},{brake:F3}," +
                        $"{localVelocity.x:F3},{localVelocity.y:F3},{localVelocity.z:F3}," +
                        $"{localAcceleration.x:F3},{localAcceleration.y:F3},{localAcceleration.z:F3}," +
                        $"{rotationEuler.y:F3},{rotationEuler.x:F3},{rotationEuler.z:F3}," + // yaw, pitch, roll
                        $"{distanceFromIdeal:F3},{curvaAttuale},{lapTimer:F2}";

        records.Add(record);
    }

    float GetInput(int inputChannel)
    {
        return vehicle != null ? vehicle.data.Get(Channel.Input, inputChannel) / 10000f : 0f;
    }

    void OnApplicationQuit()
    {
        WriteTelemetryToCSV();
    }

    private static int lapCounter = 1;

    private void WriteTelemetryToCSV()
    {
        string fileName = $"Telemetry_Giro_{lapCounter:D3}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Time,PosX,PosY,PosZ,VelX,VelY,VelZ,Steering,Throttle,Brake," +
                      "LocalVelX,LocalVelY,LocalVelZ,AccLat,AccLong,AccVert," +
                      "Yaw,Pitch,Roll,DistFromIdeal,Curva,LapTime");

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

        lapCounter++;
        records.Clear();
    }

    public void ResetTimer()
    {
        WriteTelemetryToCSV();
        lapTimer = 0f;
        Debug.Log("Timer giro resettato e dati salvati!");
    }

    // Metodo chiamato da TrackCheckpoints per aggiornare info curva
    public void AggiornaCurva(string curva, Vector3 idealPos)
    {
        Debug.Log($"Aggiornato a {curva} con posizione ideale {idealPos}");
        curvaAttuale = curva;
        idealCheckpointPosition = idealPos;
    }

}
