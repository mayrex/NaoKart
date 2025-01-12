using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{


    public GameObject Player;
    public float speed = 1f;
    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
    }

    private void Follow()
    {
        gameObject.transform.position = Vector3.Lerp(transform.position, Player.transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt(Player.transform.position);
    }
}
