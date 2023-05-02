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
            //�Ͻ����� Ȱ��ȭ
            if(isPause == false)
            {
                Time.timeScale = 0;
                isPause = true;
                pause.SetActive(isPause);
            }
            //�Ͻ����� ��Ȱ��ȭ
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
