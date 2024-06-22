using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, IItemContainer
{
    private static Storage _storage;    // 싱글톤 인스턴스

    public List<BaseItem> items = new List<BaseItem>();    // 창고 아이템 리스트

    public static Storage Instance{
        get{
            if(_storage == null){
                _storage = FindObjectOfType<Storage>();

                if(_storage == null){
                    GameObject singleton = new GameObject("Storage");
                    _storage = singleton.AddComponent<Storage>();
                }
            }
            return _storage;
        }
    }

    void Awake(){
        if(_storage == null){
            _storage = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    // 아이템 추가
    public void AddItem(BaseItem item){
        item.itemData.count += 1;

        if(!items.Contains(item)){
            items.Add(item);
        }
    }

    // 아이템 제거
    public void RemoveItem(BaseItem item){
        item.itemData.count -= 1;

        if(item.itemData.count <= 0){
            items.Remove(item);
        }
    }
}
