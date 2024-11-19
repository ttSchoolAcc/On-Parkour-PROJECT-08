using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{

    public Speech speech;
    public int speechStart;
    public int speechLength;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(speech != null)
            {
                speech.StartChat(speechStart, speechLength);
            }
        }
    }
}
