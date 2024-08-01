using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System;

public class MainGameManager : MonoBehaviour
{
    private static MainGameManager _mainGameManager;              // 싱글톤 인스턴스

    [Header("Manager")]
    public ItemManager itemManager;
    public CameraController cameraController;        // 카메라 컨트롤러
    public UIController_Test uiController;            // UI 컨트롤러

    public GameObject player;            
    public ThirdPersonController playerController;    // 플레이어 컨트롤러
    public PlayerStatus playerStatus;                 // 플레이어 상태
    public PlayerInventory playerInventory;           // 플레이어 인벤토리

    public static MainGameManager Instance {
        get{
            if(_mainGameManager == null){
                // 씬에서 MainGameManager를 찾음
                _mainGameManager = FindObjectOfType<MainGameManager>();

                // 씬에 MainGameManager가 없는 경우 새로운 GameObject를 생성하여 추가
                if (_mainGameManager == null)
                {
                    GameObject singleton = new GameObject("MainGameManager");
                    _mainGameManager = singleton.AddComponent<MainGameManager>();
                }
            }
            return _mainGameManager;
        }
    }

    void Awake(){
        // 싱글톤 인스턴스 설정
        if(_mainGameManager == null){
            _mainGameManager = this;                            // 인스턴스 할당
            DontDestroyOnLoad(gameObject);                      // 씬 전환 시에도 파괴되지 않도록 설정
        }

        // itemManager = GetComponent<ItemManager>();
        // uiController = GetComponent<UIController_Test>();
        CheckObject(ref itemManager);
        CheckObject(ref uiController);
        CheckObject(ref cameraController);

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
