using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeArea : MonoBehaviour
{


    [SerializeField]
    string sceneToChangeTo;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(sceneToChangeTo);
        }
    }
}
