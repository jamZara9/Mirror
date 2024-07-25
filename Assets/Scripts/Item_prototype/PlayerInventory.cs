using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IItemContainer
{
    public List<BaseItem> items = new List<BaseItem>();    // 플레이어의 인벤토리 아이템 리스트
    public GameObject[] quickSlots = new GameObject[5];    // 퀵슬롯
    public GameObject selectedItem = null;    // 선택된 아이템
    
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
