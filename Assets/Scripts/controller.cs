using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class controller : MonoBehaviour
{

    private inputMananger iM;
    public WheelCollider[] wheels;
    public float torque = 100;
    public float steerPower = 30;

    void Start()
    {
        iM = GetComponent<inputMananger>();
    }


    private void moveVehicle()
    {
        for (int i = 2; i < wheels.Length; i++) 
        {
            wheels[i].motorTorque = -iM.vertical * torque;
        
        
        }

        for (int i = 0; i < wheels.Length - 2; i++)
        {
            wheels[i].steerAngle = iM.horizontal * steerPower;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        moveVehicle();
    }
}
