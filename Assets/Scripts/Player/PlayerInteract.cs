using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    public LayerMask interactMask;
    public float interactRange = 10;
    bool validInteraction;
    //public GameObject upgradeSelectionUI; //This needs to be referenced because it is inactive. I put it on this script because it is attatched to player by default
    //public InventoryUIManager inventoryUIManager;
    public GameObject interactedObj;
    //public InteractionScript interactionScript;
    public TextMeshProUGUI interactText;
    public GameObject objectBeingLookedAt;


    public Transform cam;

    void Awake()
    {
        cam = Camera.main.transform;
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, interactRange, interactMask)/* && objectBeingLookedAt != hit.collider.gameObject*/)
        {
            objectBeingLookedAt = hit.collider.gameObject;

            if(interactText.enabled == false)
            {
                interactText.enabled = true;
                validInteraction = true;
            }
        }
        else
        {
            if(interactText.enabled == true)
            {
                interactText.enabled = false;
            }
            validInteraction = false; //leave this out here to make sure it falses
        }

        if(validInteraction)
        {
            //------------------------------------------------------------------------------------THIS IS WHERE YOU PUT NEW STUFF
            //PUT UI STUFF SAYING TO INTERACT
            if(Input.GetKeyDown(KeyCode.E))
            {
                interactedObj = hit.collider.gameObject;
                GetCustomComponent(interactedObj);
                //interactionScript = hit.collider.GetComponent<InteractionScript>();

                // if(door != null)
                // {
                //     if(door.opened.Value == false)
                //     {
                //         door.OpenServerRpc();
                //     }
                //     else
                //     {
                //         door.CloseServerRpc();
                //     }
                // }
            }
        }
    }

    void GetCustomComponent(GameObject interactedObj)
    {

    }
}
