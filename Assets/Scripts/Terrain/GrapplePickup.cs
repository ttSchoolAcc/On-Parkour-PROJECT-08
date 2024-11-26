using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerGrapple>().enabled = true;
            Destroy(gameObject);
        }
    }
}
