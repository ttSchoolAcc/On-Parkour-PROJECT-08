using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallCheck : MonoBehaviour
{
    public bool lookingAtWall;

    public LayerMask layerMask;

    void OnTriggerEnter(Collider other)
    {
        if((layerMask & (1 << other.gameObject.layer)) != 0)
        {
            lookingAtWall = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if((layerMask & (1 << other.gameObject.layer)) != 0)
        {
            lookingAtWall = false;
        }
    }
}
