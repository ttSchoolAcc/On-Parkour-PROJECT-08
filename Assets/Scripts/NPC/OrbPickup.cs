using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPickup : MonoBehaviour
{
    public Speech orbSpeech;
    public MeshRenderer meshRenderer;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            orbSpeech.StartChat(2, 3);

            Destroy(gameObject);
        }
    }
}
