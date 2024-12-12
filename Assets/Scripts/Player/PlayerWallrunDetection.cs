using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallrunDetection : MonoBehaviour
{
    public int leftOrRight;
    public PlayerMovement playerMovement;
    public LayerMask layerMask;

    // void OnTriggerStay(Collider other) //Ontrigger enter results in weird corner interactions
    // {
    //     //if(other.tag == "Ground")
    //     if((layerMask & (1 << other.gameObject.layer)) != 0)
    //     {
    //         if(leftOrRight == 0)
    //         {
    //             playerMovement.leftWallRun = true;
    //             playerMovement.WallrunLeft();
    //         }
    //         else if(leftOrRight == 1)
    //         {
    //             playerMovement.rightWallRun = true;
    //             playerMovement.WallrunRight();
    //         }
    //     }
    // }

    void OnTriggerEnter(Collider other)
    {
        if((layerMask & (1 << other.gameObject.layer)) != 0)
        {
            if(leftOrRight == 0)
            {
                playerMovement.WallrunStart();
            }
            else if(leftOrRight == 1)
            {
                playerMovement.WallrunStart();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if((layerMask & (1 << other.gameObject.layer)) != 0)
        {
            if(leftOrRight == 0)
            {
                playerMovement.leftWallRun = true;
                playerMovement.WallrunLeft();
            }
            else if(leftOrRight == 1)
            {
                playerMovement.rightWallRun = true;
                playerMovement.WallrunRight();
            }
        }
        

        Vector3 wallrunForceDir = new Vector3(0, 0, 0);
        if(leftOrRight == 0)
        {
            wallrunForceDir = -transform.right;
        }
        else if(leftOrRight == 1)
        {
            wallrunForceDir = transform.right;
        }
        playerMovement.WallRunForce(wallrunForceDir);
    }

    // void OnTriggerStay(Collider other)
    // {
    //     Vector3 collisionPoint = other.ClosestPoint(transform.position);
    //     Vector3 wallNormal = transform.position - collisionPoint;

    //     playerMovement.WallRunForce(wallNormal);
    // }

    void OnTriggerExit(Collider other)
    {
        //if(other.tag == "Ground")
        if((layerMask & (1 << other.gameObject.layer)) != 0)
        {
            if(leftOrRight == 0)
            {
                playerMovement.leftWallRun = false;
                playerMovement.WallrunLeft();
            }
            else if(leftOrRight == 1)
            {
                playerMovement.rightWallRun = false;
                playerMovement.WallrunRight();
            }
        }
        
        //playerMovement.Wallrun();
    }
}
