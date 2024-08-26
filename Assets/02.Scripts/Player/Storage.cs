using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, IItemContainer
{
    public List<IInventoryItem> items;    // 창고 아이템 리스트

    void Start(){
        // 아이템 리스트 초기화
        items = new List<IInventoryItem>();
    }

    // 아이템 추가
    public void AddItem(IInventoryItem item){
        item.Count += 1;

        if(!items.Contains(item)){
            items.Add(item);
        }
    }

    // 아이템 제거
    public void RemoveItem(IInventoryItem item){
        
        item.Count -= 1;

        if(item.Count <= 0){
            items.Remove(item);
        }
    }
}
