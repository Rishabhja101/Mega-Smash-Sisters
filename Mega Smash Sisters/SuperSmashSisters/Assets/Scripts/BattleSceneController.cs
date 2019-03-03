using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleSceneController : MonoBehaviour
{
    private GameObject pauseMenu;
    public GameObject player1;
    public GameObject player2;
    public GameObject endGameScreen;
    public Text endGameText;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.Find("/PauseMenu");
        Invoke("Delayed", 0.1f);
    }

    void Delayed()
    {
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

        if (player1.GetComponent<playerController>().lives == 0)
        {
            player1.SetActive(false);
            player2.SetActive(false);
            endGameScreen.SetActive(true);
            endGameText.text = "Congratulations!\n Player 2 Won!!!";
        }
        else if (player2.GetComponent<playerController>().lives == 0)
        {
            player1.SetActive(false);
            player2.SetActive(false);
            endGameScreen.SetActive(true);
            endGameText.text = "Congratulations!\n Player 1 Won!!!";
        }

    }

    public void Resume()
    {
        Time.timeScale = (Time.timeScale + 1) % 2;
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
