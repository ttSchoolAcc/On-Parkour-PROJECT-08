using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveStartSceneManager : MonoBehaviour
{
    public GameObject playerBase;
    public GameObject player;
    public GameObject orb;

    PlayerMovement playerMovement;
    PlayerGrapple playerGrapple;
    

    // Start is called before the first frame update
    void Start()
    {
        playerBase.GetComponent<Animator>().Play("BASE Cave Wake Up");

        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.enabled = false;

        playerGrapple = player.GetComponent<PlayerGrapple>();
        playerGrapple.enabled = false;

        orb.GetComponent<Speech>().StartChat(0, 1);
    }

    public void StartWakeUpEnd() //Called by anim
    {
        playerMovement.enabled = true;
        playerGrapple.enabled = true;
        player.transform.parent = null; //This is a bandaid fix. I should've just animated the camera
    }
}
