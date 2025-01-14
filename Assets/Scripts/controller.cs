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
    public float radius = 10;
    public float DownForceValue = 50f;

    public GameObject centerOfMass;
    private Rigidbody rb;

    void Start()
    {
        iM = GetComponent<inputMananger>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.transform.localPosition;

    }

    private void steerVehicle()
    {
        {
            if (iM.horizontal > 0)
            {
                wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * iM.horizontal;
                wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * iM.horizontal;
            }
            else if (iM.horizontal < 0)
            {
                wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * iM.horizontal;
                wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * iM.horizontal;
            }
            else
            {
                wheels[0].steerAngle = 0;
                wheels[1].steerAngle = 0;
            }
        }
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

    private void applyDownForce()
    {
         rb.AddForce(-transform.up * DownForceValue * rb.velocity.magnitude);
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        moveVehicle();
        steerVehicle();
    }
}
