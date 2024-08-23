using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, IItemContainer
{
    public List<BaseItem> items;    // 창고 아이템 리스트

    void Start(){
        // 아이템 리스트 초기화
        items = new List<BaseItem>();
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
