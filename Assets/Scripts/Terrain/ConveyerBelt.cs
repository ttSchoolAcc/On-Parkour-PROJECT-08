using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{

    Rigidbody detectedRb; //Should be player
    [SerializeField]
    bool playerOnBelt = false;
    [SerializeField]
    float speed = 5;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            detectedRb = other.GetComponent<Rigidbody>();
            playerOnBelt = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerOnBelt = false;
        }
    }

    void FixedUpdate()
    {
        if(playerOnBelt)
        {
            detectedRb.AddForce(transform.forward * speed);
        }
    }
}
