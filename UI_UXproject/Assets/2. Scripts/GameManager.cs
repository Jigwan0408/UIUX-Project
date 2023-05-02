using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health = 5;

    public Transform spawnPoint;
    public Transform savePoint;

    public PlayerMove player;
    public GameObject UI_Enemy;
    public GameObject[] Stages;

    public Image[] hpUI;
    public Text pointUI;
    public Text stageUI;   

    void Awake()
    {
        spawnPoint.transform.position = new Vector3(0, 0, -1);
        savePoint.transform.position = new Vector3(0, 0, -1);
    }

    void Update()
    {
        pointUI.text = (totalPoint + stagePoint).ToString();

        if (Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.Alpha5) && Input.GetKey(KeyCode.Alpha1) && Input.GetKeyDown(KeyCode.Alpha7))
        {
            StartCoroutine(OnCheat());
            Debug.Log("Cheat!");
            player.PlaySound("FINISH");
        }
        else if (Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.Alpha0) && Input.GetKey(KeyCode.Alpha1) && Input.GetKeyDown(KeyCode.Alpha4))
        {
            StopAllCoroutines();
            Debug.Log("not Cheat..");
            player.PlaySound("DIE");
        }
    }

    public void NextStage()
    {
        // 스테이지 초기화
        if(stageIndex < Stages.Length - 1)
        {
            PlayerRePosition(spawnPoint);
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);

            stageUI.text = "STAGE " + (stageIndex + 1);
        }
        else // 게임 클리어
        {
            SceneManager.LoadScene("GameClear");
        }

        // 점수 갱신 (Total 점수)
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    // 치트 모드
    IEnumerator OnCheat()
    {
        yield return new WaitForSeconds(0);

        Debug.Log("Cheat");
        player.isJumping = false;

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (health < 5)
            {
                HealthUp();
                yield return new WaitForSeconds(1f);
            }
        }

        StartCoroutine(OnCheat());
    }

    // 플레이어 회복
    public void HealthUp()
    {
        if (health < 5)
        {
            health++;
            hpUI[health - 1].color = new Color(1, 1, 1, 1f);
        }
        else if(health >= 5)
        {
            stagePoint += 500;
            UI_Enemy.transform.position = hpUI[health - 1].transform.position;
            Instantiate(UI_Enemy);

            Destroy(GameObject.Find("UI_Enemy(Clone)").gameObject, 6f);
        }
    }

    // 플레이어 체력 감소
    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            hpUI[health].color = new Color(1, 1, 1, 0.2f);
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
    }
 
    // 낙사 체크
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (health > 1)
            {
                player.OnDamaged(collision.transform.position);
                PlayerRePosition(savePoint);
            }
            else 
                HealthDown();
        }
        
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
    }

    // 플레이어 재생성
    void PlayerRePosition(Transform point)
    {
        player.transform.position = point.position;
        player.VelocityZero();
    }

    // 게임 재시작
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("InGame");
    }

    // 점프 초기화
    public void Reset(GameObject gameObject)
    {
        StartCoroutine(ResetJump(gameObject));
    }

    IEnumerator ResetJump(GameObject gameObject)
    {
        yield return new WaitForSeconds(2);

        gameObject.SetActive(true);
    }
}
