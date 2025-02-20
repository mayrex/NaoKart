using UnityEngine;

public class TelemetryRecord
{
    public float timeStamp;
    public Vector3 position;
    public Vector3 velocity;
    public float lapTimer;  // Tempo trascorso dall'ultimo reset del timer

    public TelemetryRecord(float timeStamp, Vector3 position, Vector3 velocity, float lapTimer)
    {
        this.timeStamp = timeStamp;
        this.position = position;
        this.velocity = velocity;
        this.lapTimer = lapTimer;
    }
}
