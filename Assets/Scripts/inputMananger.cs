using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputMananger : MonoBehaviour
{


    public float horizontal;
    public float vertical;

    // Update is called once per frame
    void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }
}
