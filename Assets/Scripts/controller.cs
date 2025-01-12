using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class controller : MonoBehaviour
{

    public WheelCollider[] wheels;
    public float torque = 100;
    public float steerPower = 100;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (var wheel in wheels)
        {
            wheel.motorTorque = Input.GetAxis("Vertical") * torque;
        }

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].steerAngle = Input.GetAxis("Horizontal") * steerPower;
        }

    }
}
