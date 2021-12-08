using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Play3DGame ()
    {
        SceneManager.LoadScene("playScene");
    }
    public void Play2DGame()
    {
        SceneManager.LoadScene("playScene2d");
    }
    public void QuitGame()
    {
        UnityEngine.Debug.Log("Quit!!");
        UnityEngine.Application.Quit();
    }
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
