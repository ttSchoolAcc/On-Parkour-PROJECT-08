using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public Speech speech;
    public int speechStart;
    public int speechLength;
    [SerializeField]
    bool triggerOnceEnable;
    bool triggerOnce;
    [SerializeField]
    Animator anim;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && triggerOnce == false)
        {
            if(speech != null)
            {
                speech.StartChat(speechStart, speechLength);
            }

            if(triggerOnceEnable)
            {
                triggerOnce = true;
            }

            if(anim != null)
            {
                anim.Play("Mech Walk");
            }
        }
    }
}
