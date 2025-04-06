using UnityEngine;
using VehiclePhysics;

public class SimpleVehicleController : VehicleBase
{
    [Header("Simple Vehicle Controller")]
    public VPWheelCollider wheelFL;
    public VPWheelCollider wheelFR;
    public VPWheelCollider wheelRL;
    public VPWheelCollider wheelRR;
    public TireFriction tireFriction = new TireFriction();

    public float maxDriveTorque = 500.0f;
    public float maxBrakeTorque = 1000.0f;
    public float maxSteerAngle = 45.0f;
    public float maxDriveRpm = 50.0f;

    [Range(-1, 1)]
    public float driveInput = 0.0f;
    [Range(0, 1)]
    public float brakeInput = 0.0f;
    [Range(-1, 1)]
    public float steerInput = 0.0f;

    // Blocchi interni del veicolo
    DirectDrive m_directDrive;
    SimpleGear m_simpleGear;  // AGGIUNTO
    SimpleOpenDifferential m_differential;  // Sostituisce il differenziale


    private void Start()
    {
        if (wheelFL == null || wheelFR == null || wheelRL == null || wheelRR == null)
        {
            Debug.LogError("[ERROR] Uno o più VPWheelCollider non sono assegnati! Controlla l'Inspector.");
            return;
        }

        Debug.Log("[DEBUG] Tutti i VPWheelCollider sono assegnati correttamente.");
    }

    protected override void OnInitialize()
    {
        SetNumberOfWheels(4);

        if (wheelFL == null || wheelFR == null || wheelRL == null || wheelRR == null)
        {
            Debug.LogError("Missing VPWheelCollider");
            return;
        }
        if (m_directDrive == null || m_simpleGear == null || m_differential == null)
{
    Debug.LogError("[ERROR] Uno o più blocchi del veicolo non sono stati inizializzati correttamente.");
    return;
}

        ConfigureWheelData(wheelState[0], wheels[0], wheelFL, true);
        ConfigureWheelData(wheelState[1], wheels[1], wheelFR, true);
        ConfigureWheelData(wheelState[2], wheels[2], wheelRL);
        ConfigureWheelData(wheelState[3], wheels[3], wheelRR);

        // Inizializza i blocchi
        m_directDrive = new DirectDrive();
        m_simpleGear = new SimpleGear();  // AGGIUNTO
        m_simpleGear.ratio = 3.5f;  // Imposta il rapporto del cambio

        m_differential = new SimpleOpenDifferential();  // Sostituisce il differenziale predefinito

        // Collegare i blocchi: DriveLine -> Cambio -> Differenziale -> Ruote
        Block.Connect(wheels[2], 0, m_differential, 0);
        Block.Connect(wheels[3], 0, m_differential, 1);
        Block.Connect(m_differential, 0, m_simpleGear, 0);
        Block.Connect(m_simpleGear, 0, m_directDrive, 0);
    }

    void ConfigureWheelData(WheelState ws, Wheel wheel, VPWheelCollider wheelCol, bool steerable = false)
    {
        if (wheelCol == null)
        {
            Debug.LogError("VPWheelCollider non assegnato per " + wheelCol.gameObject.name);
            return;
        }

        // Debug extra per capire il problema
        Debug.Log($"[DEBUG] Wheel {wheelCol.gameObject.name}: Radius={wheelCol.radius}, Mass={wheelCol.mass}");

        // Se i valori sono invalidi, assegniamo quelli di default
        if (wheelCol.radius <= 0)
        {
            Debug.LogWarning($"[WARNING] Radius di {wheelCol.gameObject.name} non valido, impostato a 0.3");
            wheelCol.radius = 0.3f;
        }

        if (wheelCol.mass <= 0)
        {
            Debug.LogWarning($"[WARNING] Mass di {wheelCol.gameObject.name} non valido, impostato a 20.0");
            wheelCol.mass = 20.0f;
        }

        // Assegniamo i valori alla ruota
        wheel.radius = wheelCol.radius;
        wheel.mass = wheelCol.mass;

        ws.wheelCol = wheelCol;
        ws.steerable = steerable;
        wheel.tireFriction = tireFriction;

        Debug.Log($"[DEBUG] Dopo assegnazione: Wheel {wheelCol.gameObject.name}: Radius={wheel.radius}");
    }



    protected override void DoUpdateBlocks()
    {
        m_directDrive.motorInput = driveInput;
        m_directDrive.maxMotorTorque = maxDriveTorque;
        m_directDrive.maxRpm = maxDriveRpm;

        float angle = steerInput * maxSteerAngle;
        wheelState[0].steerAngle = angle;
        wheelState[1].steerAngle = angle;

        float brakeTorque = brakeInput * maxBrakeTorque;
        wheels[0].AddBrakeTorque(brakeTorque);
        wheels[1].AddBrakeTorque(brakeTorque);
        wheels[2].AddBrakeTorque(brakeTorque);
        wheels[3].AddBrakeTorque(brakeTorque);
    }
}
