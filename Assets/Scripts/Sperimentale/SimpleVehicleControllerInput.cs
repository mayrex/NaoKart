using UnityEngine;
using VehiclePhysics;

public class SimpleVehicleControllerInput : VehicleBehaviour
{
    SimpleVehicleController m_vehicle;
    public float steerInput;
    public float throttleAndBrakeAxisValue;
    public float throttleInput;
    public float brakeInput;

    public override void OnEnableVehicle()
    {
        // This component requires a SimpleVehicleController explicitly

        m_vehicle = vehicle.GetComponent<SimpleVehicleController>();
        if (m_vehicle == null)
        {
            DebugLogWarning("A vehicle based on SimpleVehicleController is required. Component disabled.");
            enabled = false;
        }
    }

    public override void UpdateVehicle()
    {
        // Read the input from the standard Unity Input

        steerInput = Mathf.Clamp(Input.GetAxis("Horizontal"), -1.0f, 1.0f);

        throttleAndBrakeAxisValue = Input.GetAxis("Vertical");
        throttleInput = Mathf.Clamp01(throttleAndBrakeAxisValue);
        brakeInput = Mathf.Clamp01(-throttleAndBrakeAxisValue);
     

        // Hold Ctrl and Brake for reverse

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            throttleInput = -brakeInput;
            brakeInput = 0.0f;
        }

        // Feed the vehicle input parameters with the result

        m_vehicle.steerInput = steerInput;
        m_vehicle.driveInput = throttleInput;
        m_vehicle.brakeInput = brakeInput;
    }
}