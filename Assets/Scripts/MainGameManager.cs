using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class MainGameManager : MonoBehaviour
{
    private static MainGameManager _mainGameManager;              // 싱글톤 인스턴스


    public GameObject player;            
    private ThirdPersonController playerController;    // 플레이어 컨트롤러
    private PlayerStatus playerStatus;                 // 플레이어 상태
    private PlayerInventory playerInventory;           // 플레이어 인벤토리


    public GameObject detectedItem;                    // 감지된 아이템

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
            DontDestroyOnLoad(gameObject);                  // 씬 전환 시에도 파괴되지 않도록 설정
        }
        else{
            Destroy(gameObject);
        }

        if(player == null){
            // 플레이어 컨트롤러 설정
            playerController = player.GetComponentInChildren<ThirdPersonController>();
            playerStatus = player.GetComponent<PlayerStatus>();
            playerInventory = player.GetComponent<PlayerInventory>();
        }

    }

    public void PickupItem(){
        
    }

}
