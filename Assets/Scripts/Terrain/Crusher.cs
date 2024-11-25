using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [SerializeField]
    Animator anim;

    [SerializeField]
    int startDelay;
    [SerializeField]
    int waitTime;

    void Start()
    {
        if(startDelay >= 0)
        {
            Invoke("StartCrush", startDelay);
        }
    }

    void StartCrush()
    {
        StartCoroutine(CrushCoroutine());
    }

    IEnumerator CrushCoroutine()
    {
        while(true)
        {
            anim.Play("Crusher");
            yield return new WaitForSeconds(waitTime);
        }
    }
}
