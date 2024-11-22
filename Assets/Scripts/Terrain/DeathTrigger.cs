using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    PlayerHealth playerHealth;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player")
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(100);
        }
    }
}