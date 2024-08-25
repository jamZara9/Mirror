using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MonsterMirror : MonsterFSM
{
    public GameObject Mirror;                                           // 공격 후 부서질 거울 오브젝트
    [SerializeField]
    private bool isMirrorAttacked = false;                               // 거울이 부서졌는지 여부
    [SerializeField]
    private float dashDistance = 5f;                                     // 돌진 거리
    [SerializeField]
    private float dashDuration = 0.5f;                                   // 돌진 시간
    private bool isDashing = false;
    
    
    public override void Attack()        //  거울사제2(몬스터)의 플레이어 공격
    {                           //  만약 플레이어와 거울사제2(몬스터)의 거리가 공격 범위 내라면
        if (Vector3.Distance(_player.position, transform.position) < attackDistance)
        {                       //  currentTime 카운트 시작
            _currentTime += Time.deltaTime;
            if (_currentTime > attackDelay)// currentTime이 attackDelay만큼 카운트 했다면 공격 진행
            {
                if (!isMirrorAttacked && !isDashing) //   거울 부수기 공격을 하지 않았고 지금 대쉬 중이 아니라면
                {
                    StartCoroutine(MirrorAttack());    //  거울 부수기 공격
                }
                else
                {
                    int randValue = Random.Range(0, 10);
                    if (randValue < 5) //50%로 방향을 구함
                    {
                        Debug.Log("공격");    
                    }
                    else
                    {
                        Debug.Log("깨물기 공격");
                    }
                }
                _currentTime = 0;       // currentTime 초기화
            }
        }
        else
        {               //  플레이어와 거울사제2(몬스터)의 거리가 공격 범위 보다 멀다면
            if (!isDashing)
            {
                m_State = MonsterState.Move;    // 거울사제2(몬스터)의 상태를 Move로 변경
                _currentTime = 0;   // currentTime 초기화
            }
        }
    }
    private IEnumerator MirrorAttack()               // 거울 부수기 공격
    {
        Debug.Log("거울 공격");
        isDashing = true;                            // 대쉬중
        Vector3 dashStartPosition = transform.position;
        Vector3 targetPosition = dashStartPosition + transform.forward * dashDistance; // 현재 방향 + 앞 만큼 * 돌진거리
        float elapsedTime = 0f;                                                         // 누적 시간

        while (elapsedTime < dashDuration)                                              
        {                                                                               // 돌진 거리만큼을 돌진 시간에 걸려서 도착
            transform.position = Vector3.Lerp(dashStartPosition, targetPosition, (elapsedTime / dashDuration)); // 선형 보간으로 부드럽게 이동
            elapsedTime += Time.deltaTime;  
            yield return null;                                                          // 다음 프레임까지 대기
        }
        transform.position = targetPosition;                                            // 최종 위치 설정
        isDashing = false;
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (isDashing)
        {
            Debug.Log(other.gameObject.name+" 부딪힘");
            isMirrorAttacked = true; // 거울을 안보이게 함
            Mirror.SetActive(false); // 거울 부수기 공격을 하면 거울 오브젝트가 안보이도록
        }
    }
}