using UnityEngine;
using VehiclePhysics;

public class TelemetryDataCollector : MonoBehaviour
{
    public VehicleBase vehicle;

    void Start()
    {
        vehicle = GetComponent<VehicleBase>();
    }

    void FixedUpdate()
    {
        // Leggi i valori dal Data Bus
        float steering = vehicle.data.Get(Channel.Input, InputData.Steer) / 10000.0f;
        float throttle = vehicle.data.Get(Channel.Input, InputData.Throttle) / 10000.0f;
        float brake = vehicle.data.Get(Channel.Input, InputData.Brake) / 10000.0f;

        Debug.Log(steering + "+" + throttle + "+" + brake);
        // Leggi la velocità locale del veicolo
        Vector3 localVelocity = vehicle.localVelocity;

        // Ora puoi utilizzare questi dati per il tuo modello AI
    }
}
