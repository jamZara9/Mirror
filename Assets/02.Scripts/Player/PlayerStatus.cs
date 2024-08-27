using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum StatusType
{
    Health,
    Mental
}

/// <summary>
/// 플레이어의 상태(체력, 스태미나)를 관리하는 클래스
/// </summary>
public class PlayerStatus : MonoBehaviour, IDamage
{
    [Serializable]
    public class PlayerBasicSettings
    {   
        [Header("Player Maximum Settings")]
        public readonly float maxHealth = 100.0f;       // 최대 체력
        public readonly float maxMental = 100.0f;       // 최대 정신력

        [Header("Player Movement Settings")]
        public readonly float walkSpeed = 2.0f;         // 걷기 속도
        public readonly float runSpeed = 4.0f;          // 달리기 속도
        public readonly float speedChangeRate = 10.0f;  // 속도 변경 비율(가속도)
        public readonly float jumpHeight = 1.2f;        // 점프 높이

        [Header("Player Attack Settings")]
        public readonly float attackRange = 0.5f;       // 공격 사정거리
        public readonly float attackDamage = 10.0f;     // 공격 데미지
        public readonly float attackDelay = 1.0f;       // 공격 딜레이(공속)
    }

    public PlayerBasicSettings settings = new();     // 기본 설정

    public float CurrentHealth { get; private set; }
    public float CurrentMental { get; private set; }
    public float CurrentAttackRange { get; private set; }   
    public float CurrentAttackDamage { get; private set; }

    void Awake()
    {
        CurrentHealth = settings.maxHealth;
        CurrentMental = settings.maxMental;
        CurrentAttackDamage = settings.attackDamage;
        CurrentAttackRange = settings.attackRange;
    }

    /// <summary>
    /// Player의 Status를 조절하는 함수
    /// </summary>
    /// <param name="statusType">조절할 Status</param>
    /// <param name="amount">조절 양</param>
    public void AdjustStatus(StatusType statusType, float amount){
        switch(statusType){
            case StatusType.Health:
                CurrentHealth += amount;
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0, settings.maxHealth);
                break;
            case StatusType.Mental:
                CurrentMental += amount;
                CurrentMental = Mathf.Clamp(CurrentMental, 0, settings.maxMental);
                break;
        }
    }

    public void TakeDamage(int hitPower)
    {
        AdjustStatus(StatusType.Health, -hitPower);
        Debug.Log($"현재 체력 : {CurrentHealth}");
    }
}
