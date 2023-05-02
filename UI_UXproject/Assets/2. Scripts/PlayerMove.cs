using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    
    public GameObject fakeBGM;
    public GameObject BGM;

    public float maxSpeed = 5; // 최고 속도
    public float jumpPower = 18; // 점프 크기 
    public bool isJumping; // 점프 여부
    
    // 오디오 클립
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioHeal;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    AudioSource audioSource; // 오디오 소스
    AudioSource fakeAudio; // 함정 오디오 소스
    Rigidbody2D rigid; // 리지드바디
    SpriteRenderer spriteRenderer; // 스프라이트 렌더러
    Animator anim; // 애니메이터
    CapsuleCollider2D capsuleCollider; // 콜라이더

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
       
    }

    void Update()
    {
        // 만약, 점프 중이 아닐 때 W or 위 화살표를 눌렀을 경우
        if (!isJumping && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {            
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isJumping = true;

            // 점프 사운드 재생
            PlaySound("JUMP");
            jumpPower = 18;
        }

        // 이동 감속
        if (Input.GetButtonUp("Horizontal"))
        {            
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // 이동 애니메이션
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == 1;
        }

        // 걷기 애니메이션 
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("isWalking", true);
        }
    }

    void FixedUpdate()
    {
        // 이동
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // �ִ이동 속도 제한
        if (rigid.velocity.x > maxSpeed)  // 오른쪽 이동일 때
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1)) // 왼쪽 이동일 때
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // 점프 체크 (무한 점프 제한)
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance <= 0.8)
                {
                    isJumping = false;
                }
            }
        }
    }

    // 적 충돌 체크
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y - collision.transform.position.y > 0.5 && collision.gameObject.name != "Spike")
            {
                // 타격   
                OnAttack(collision.transform);
            }
            else
            {
                // 피격
                OnDamaged(collision.transform.position);
            }
        }
    }

    // 아이템, 상호작용 충돌 체크 
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 세이브 포인트
        if (collision.gameObject.tag == "Save")
        {
            gameManager.savePoint.position = gameObject.transform.position;
            Debug.Log(gameManager.savePoint.position);
        }

        // 아이템
        if (collision.gameObject.tag == "Item" && gameObject.layer != 11)
        {
            // 코인
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            // 아이템
            bool isHeal = collision.gameObject.name.Contains("Heal");
            bool isJump = collision.gameObject.name.Contains("Jump");
            bool isEnemy = collision.gameObject.name.Contains("Enemy");
            bool isFake = collision.gameObject.name.Contains("Fake");

            if (isBronze) gameManager.stagePoint += 50;
            else if (isSilver) gameManager.stagePoint += 150;
            else if (isGold) gameManager.stagePoint += 300;

            // 함정 시스템
            if (isFake)
            {
                gameManager.stageUI.text = "It's Fake! LOL";
                BGM.SetActive(false);
                fakeBGM.SetActive(true);

                fakeAudio = GameObject.Find("FakeBGM").GetComponentInChildren<AudioSource>();
                fakeAudio.Play();

                Invoke("FakeOff", 10f);
                Destroy(collision.gameObject);

                return;
            }

            if (isEnemy)
            {
                OnDamaged(collision.transform.position);
                Destroy(collision.gameObject);
                return;
            }

            if (isHeal)
            {
                gameManager.HealthUp();
                PlaySound("HEAL");

                collision.gameObject.SetActive(false);
                return;
            }

            if (isJump)
            {
                isJumping = false;
                jumpPower = 25;

                gameManager.Reset(collision.gameObject);
            }

            // 충돌체 활성화 해제
            collision.gameObject.SetActive(false);

            // 아이템 사운드
            PlaySound("ITEM");
        }
        else if (collision.gameObject.tag == "Finish")
        {
            // 다음 스테이지
            gameManager.NextStage();

            // 스테이지 클리어 사운드
            PlaySound("FINISH");
        }
    }

    void FakeOff()
    {
        gameManager.stageUI.text = "STAGE " + (gameManager.stageIndex + 1);

        fakeBGM.SetActive(false);
        BGM.SetActive(true);
    }

    void OnAttack(Transform enemy)
    {
        // 점수 획득
        gameManager.stagePoint += 100;

        // 타격 사운드
        PlaySound("ATTACK");

        // 타격 반동
        rigid.AddForce(Vector2.up * 18, ForceMode2D.Impulse);

        // 적 사망
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    public void OnDamaged(Vector2 targetPos)
    {
        // 체력 감소
        gameManager.HealthDown();

        // 피격 사운드
        PlaySound("DAMAGED");

        // 무적 시스템 (Player => PlayerDamaged) (플레이어 => 피격된플레이어 레이어 변경)
        gameObject.layer = 11;

        // Alpha 값 조정 (반투명화)
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 피격 반동
        int dir = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dir, 1) * 5 , ForceMode2D.Impulse);

        // 무적 시스템 Off
        Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        // 사망 애니메이션 (캐릭터 상하반전)
        spriteRenderer.flipY = true;

        // 콜라이더 비활성화
        capsuleCollider.enabled = false;

        // 받동
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    // 사운드 재생
    public void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "HEAL":
                audioSource.clip = audioHeal;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        
        audioSource.Play();
    }

}
