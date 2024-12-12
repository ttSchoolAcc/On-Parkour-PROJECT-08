using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenu : MonoBehaviour
{

    [SerializeField]
    GameObject menu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            MenuOpen();
        }
    }

    public void MenuOpen()
    {
        if(!menu.activeInHierarchy) //If not active, enable
        {
            Cursor.lockState = CursorLockMode.None;
            menu.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            menu.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
