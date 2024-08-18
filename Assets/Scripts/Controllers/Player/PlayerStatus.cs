using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 플레이어의 상태(체력, 스태미나)를 관리하는 클래스
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    [Serializable]
    public class PlayerBasicSettings
    {
        public readonly float maxHealth = 100.0f;
        public readonly float walkSpeed = 2.0f;         // 걷기 속도
        public readonly float runSpeed = 5.335f;        // 달리기 속도
        public readonly float speedChangeRate = 10.0f;  // 속도 변경 비율
        public readonly float jumpHeight = 1.0f;        // 점프 높이
    }

    public PlayerBasicSettings settings = new();     // 기본 설정

    public float CurrentHealth { get; private set; }

    public int maxHealth = 100;
    public int currentHealth;
    public int maxStamina = 100;
    public int currentStamina;

    void Awake()
    {
        currentHealth = 50;
        currentStamina = maxStamina;
    }
}
