using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IItemContainer
{
    public List<BaseItem> items = new List<BaseItem>();         // 플레이어의 인벤토리 아이템 리스트
    public List<BaseItem> quickSlots = new List<BaseItem>();    // 퀵슬롯 아이템 리스트
    public GameObject selectedItem = null;      // 선택된 아이템

    public int maxSlots = 9;                    // 인벤토리 슬롯 최대 개수

    public UIController_Test uiController;      // UI 컨트롤러

    // 아이템 추가
    public void AddItem(BaseItem item){
        item.itemData.count += 1;

        if(!items.Contains(item)){
            items.Add(item);
            uiController.UpdateInventoryUI();
        }else{
            Debug.Log("인벤토리가 가득 찼습니다.");
        }
    }

    // 아이템 제거
    public void RemoveItem(BaseItem item){
        item.itemData.count -= 1;

        if(item.itemData.count <= 0){
            items.Remove(item);
        }
    }
    public BaseItem GetItemAt(int index){
        if (index >= 0 && index < items.Count){
            return items[index];
        }
        return null;
    }

    public void MoveItemToQuickSlot(BaseItem item, int quickSlotIndex){
        if (items.Contains(item)){
            if (quickSlotIndex >= 0 && quickSlotIndex < quickSlots.Count){
                quickSlots[quickSlotIndex] = item;
                Debug.Log($"아이템 '{item.itemID}'이(가) 퀵슬롯 {quickSlotIndex}에 추가되었습니다.");
            }else{
                Debug.LogError("유효하지 않은 퀵슬롯 인덱스입니다.");
            }
        }else{
            Debug.LogError("인벤토리에 아이템이 존재하지 않습니다.");
        }
    }
}
