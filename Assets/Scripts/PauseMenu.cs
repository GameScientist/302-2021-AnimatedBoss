using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls the activation of the pause menu and slowing down of time.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// The pause menu paenl.
    /// </summary>
    public GameObject menu;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (menu.activeInHierarchy) { // If paused, unpause.
                menu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                menu.SetActive(true); // If not paused, pause.
                Time.timeScale = 0;
            }
        }
    }
}
