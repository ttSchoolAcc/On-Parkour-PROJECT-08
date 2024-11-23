using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    int health = 100;
    PlayerMovement playerMovement;
    [SerializeField]
    GameObject deathScreen;

    void Awake()
    {

        playerMovement = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        playerMovement.enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        deathScreen.SetActive(true);
        Invoke("Respawn", 3);
    }

    void Respawn()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
