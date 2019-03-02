using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenuController : MonoBehaviour
{
    private GameObject settings;


	// Use this for initialization
	void Start ()
    {
        settings = GameObject.Find("/Settings");
        settings.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void StartGame()
    {
        SceneManager.LoadScene(""); //change to the next scene
    }

    public void OpenSettings()
    {
        settings.SetActive(!settings.active);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
