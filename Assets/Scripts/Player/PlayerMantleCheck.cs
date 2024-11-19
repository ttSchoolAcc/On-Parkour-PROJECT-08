using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMantleCheck : MonoBehaviour
{

    public bool mantleAreaClear;
    public LayerMask layerMask;

    void OnTriggerStay(Collider other)
    {
        if((layerMask & (1 << other.gameObject.layer)) != 0)
        {
            mantleAreaClear = false;
        }

        // if(other.tag == "Ground")
        // {
        // }
    }

    void OnTriggerExit(Collider other)
    {
        if((layerMask & (1 << other.gameObject.layer)) != 0)
        {
            mantleAreaClear = true;
        }

        // if(other.tag == "Ground")
        // {
        //     mantleAreaClear = true;
        // }
    }
}
