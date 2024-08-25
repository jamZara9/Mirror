using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System;

/// <summary>
/// 게임의 전반적인 관리를 담당하는 클래스
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [Header("Manager")]
    #region Manager
    public ItemManager itemManager;
    public CameraController cameraController;         // 카메라 컨트롤러
    public UIController_Test uiController;            // UI 컨트롤러
    public Storage storage;                           // 저장소
    public PlayerStatus playerStatus;                 // 플레이어 상태
    public PlayerInventory playerInventory;           // 플레이어 인벤토리
    public WeaponManager weaponManager;               // 무기 매니저
    public DialogueManager dialogueManager;           // 대화 매니저
    public UIManager uiManager;                       // UI 매니저
    public InventoryManager inventoryManager;         // 인벤토리 매니저

    public InputManager inputManager;                 // 입력 매니저
    #endregion

    void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // GameManager가 씬 변경 시에도 파괴되지 않도록 설정

        playerStatus = FindAnyObjectByType<PlayerStatus>(); // 플레이어 상태 찾기

        CheckObject(ref itemManager);
        CheckObject(ref uiController);
        CheckObject(ref cameraController);
        CheckObject(ref storage);
        CheckObject(ref playerInventory);
        CheckObject(ref inventoryManager);
        CheckObject(ref weaponManager);
        CheckObject(ref uiManager);
        CheckObject(ref dialogueManager);
        CheckObject(ref inputManager);

    }

    /// <summary>
    /// 해당 매니저가 null인지 확인하고 null이면 해당 manager의 타입을 찾아서 할당
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="manager"></param>
    private void CheckObject<T>(ref T manager) where T : Component
    {
        // 해당 매니저가 null이면 해당 manager의 타입을 찾아서 할당
        if (manager == null)
        {
            manager = GetComponent<T>();
        }

    }
}
