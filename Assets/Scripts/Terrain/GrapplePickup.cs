using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerGrapple>().enabled = true;
    }
}
