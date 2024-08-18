using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System;

public class MainGameManager : BasicSingleton<MainGameManager>
{
    private static MainGameManager _mainGameManager;              // 싱글톤 인스턴스

    [Header("Manager")]
    public ItemManager itemManager;
    public CameraController cameraController;         // 카메라 컨트롤러
    public UIController_Test uiController;            // UI 컨트롤러
    public Storage storage;                           // 저장소
    public PlayerStatus playerStatus;                 // 플레이어 상태
    public PlayerInventory playerInventory;           // 플레이어 인벤토리

    void Awake(){
        // itemManager = GetComponent<ItemManager>();
        // uiController = GetComponent<UIController_Test>();
        CheckObject(ref itemManager);
        CheckObject(ref uiController);
        CheckObject(ref cameraController);
        CheckObject(ref storage);
        CheckObject(ref playerStatus);
        CheckObject(ref playerInventory);

    }

    /// <summary>
    /// 해당 매니저가 null인지 확인하고 null이면 해당 manager의 타입을 찾아서 할당
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="manager"></param>
    private void CheckObject<T>(ref T manager) where T : Component{
        // 해당 매니저가 null이면 해당 manager의 타입을 찾아서 할당
        if (manager == null){
            manager = GetComponent<T>();
        }

    }
}
