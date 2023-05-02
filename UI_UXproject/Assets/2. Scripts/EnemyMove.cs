using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid; // 리지드바디
    Animator anim; // 애니메이터
    SpriteRenderer spriteRenderer; // 스프레이트 렌더러
    CapsuleCollider2D capsuleCollider; // 콜라이더

    public int nextMove;
    public float thinkTime;

    void Awake()
    {
        // 변수 초기화
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        // 5초 후 Think 함수 호출
        Invoke("Think", 5f);
    }

    
    void Update()
    {
        // 이동
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // 플랫폼 탐색
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        { 
            Turn();
        }
    }

    // 랜덤 이동 함수 (재귀 함수)
    void Think()
    {
        // 랜덤 설정
        nextMove = Random.Range(-1, 2);
        thinkTime = Random.Range(2f, 5f);
        
        // 2~5초(랜덤)후 재귀 호출
        Invoke("Think",  thinkTime);

        // 애니메이션
        anim.SetInteger("WalkSpeed", nextMove);
        
        // 방향 전환
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == -1;
    }

    // 방향 전환 함수
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == -1;

        CancelInvoke();
        Invoke("Think", 2);
    }

    // 피격 함수
    public void OnDamaged()
    {
        // 피격 색
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 상하 반전
        spriteRenderer.flipY = true;

        // 레이어 변경
        gameObject.layer = 9;
        
        // 죽음 모션
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    // 삭제 함수
    public void Destroy()
    {
        Debug.Log("die");
        gameObject.SetActive(false);
    }
}
