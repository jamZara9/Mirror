using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private static PlayerInventory _playerInventory;    // 싱글톤 인스턴스

    public static PlayerInventory Instance{
        get{
            if(_playerInventory == null){
                _playerInventory = FindObjectOfType<PlayerInventory>();

                if(_playerInventory == null){
                    GameObject singleton = new GameObject("PlayerInventory");
                    _playerInventory = singleton.AddComponent<PlayerInventory>();
                }
            }
            return _playerInventory;
        }
    }

    public List<BaseItem> items = new List<BaseItem>();    // 플레이어의 인벤토리 아이템 리스트

    void Awake(){
        if(_playerInventory == null){
            _playerInventory = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    // 아이템 추가
    public void AddItem(BaseItem item){
        items.Add(item);
    }

    // 아이템 제거
    public void RemoveItem(BaseItem item){
        items.Remove(item);
    }
}
