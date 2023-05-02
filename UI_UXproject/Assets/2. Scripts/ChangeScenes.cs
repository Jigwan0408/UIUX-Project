using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
  
    public void ToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void ToGame()
    {
        SceneManager.LoadScene("InGame");
    }

    public void ToHTP1()
    {
        SceneManager.LoadScene("HTP1");
    }

    public void ToHTP2()
    {
        SceneManager.LoadScene("HTP2");
    }
}
