using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneController : MonoBehaviour
{
    private GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.Find("/PauseMenu");
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = (Time.timeScale + 1) % 2;
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        }
    }

    public void Resume()
    {
        Time.timeScale = (Time.timeScale + 1) % 2;
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
    }
}
