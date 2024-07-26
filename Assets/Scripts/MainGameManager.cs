using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class MainGameManager : MonoBehaviour
{
    private static MainGameManager _mainGameManager;              // 싱글톤 인스턴스

    [Header("Manager")]
    public ItemManager itemManager;

    public GameObject player;            
    public ThirdPersonController playerController;    // 플레이어 컨트롤러
    public PlayerStatus playerStatus;                 // 플레이어 상태
    public PlayerInventory playerInventory;           // 플레이어 인벤토리

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
            DontDestroyOnLoad(gameObject);                      // 씬 전환 시에도 파괴되지 않도록 설정

            if(itemManager == null){
                GameObject itemManagerObject = new GameObject("ItemManager");
                itemManager = itemManagerObject.AddComponent<ItemManager>();
                DontDestroyOnLoad(itemManagerObject);
            }
        }
        else{
            Destroy(gameObject);
        }

    }


    // 아이템 픽업 함수
    public void PickupItem(){
        if(detectedItem != null){
            EventManager.ItemPickup(detectedItem, playerInventory);  // 아이템을 인벤토리에 추가
            detectedItem = null;
        }
    }


    // 아이템 사용 함수
    public void UseItem(){
        if(playerInventory.selectedItem != null){
            EventManager.ItemUse(playerInventory.selectedItem, playerInventory);  // 아이템 사용
        }
    }

}
