using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pause;
    public bool isPause = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //일시정지 활성화
            if(isPause == false)
            {
                Time.timeScale = 0;
                isPause = true;
                pause.SetActive(isPause);
            }
            //일시정지 비활성화
            else if(isPause == true)
            {
                Time.timeScale = 1;
                isPause = false;
                pause.SetActive(isPause);
            }
        }
    }
    public void Resume()
    {
        Time.timeScale = 1;
        isPause = false;
        pause.SetActive(isPause);
    }


}
