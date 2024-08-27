using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterStudent : MonsterFSM
{
    public override void Attack()        //  학생(몬스터)의 플레이어 공격
    {                           //  만약 플레이어와 학생(몬스터)의 거리가 공격 범위 내라면
        if (Vector3.Distance(_player.position, transform.position) < attackDistance)
        {                       //  currentTime 카운트 시작
            _currentTime += Time.deltaTime;
            if (_currentTime > attackDelay)// currentTime이 attackDelay만큼 카운트 했다면 공격 진행
            {
                int randValue = Random.Range(0, 10);
                if (randValue < 5) //50%로 방향을 구함
                {
                    // Debug.Log("공격");    

                    // 플레이어의 체력을 감소시킴 (테스트 코드)
                    _player.GetComponent<PlayerStatus>().AdjustStatus(StatusType.Health, -attackPower);
                    Debug.Log($"플레이어의 체력 : {_player.GetComponent<PlayerStatus>().CurrentHealth}");
                }
                else
                {
                    Debug.Log("깨물기 공격");
                    _player.GetComponent<PlayerStatus>().AdjustStatus(StatusType.Health, -(attackPower+5.0f));
                }
                _currentTime = 0;       // currentTime 초기화
            }
        }
        else
        {               //  플레이어와 학생(몬스터)의 거리가 공격 범위 보다 멀다면
            m_State = MonsterState.Move;    // 학생(몬스터)의 상태를 Move로 변경
            _currentTime = 0;   // currentTime 초기화
        }
    }
   
}