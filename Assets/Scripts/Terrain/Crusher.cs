using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [SerializeField]
    int damage = 100;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
