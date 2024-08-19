using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IItemContainer
{
    public List<BaseItem> items = new List<BaseItem>();         // 플레이어의 인벤토리 아이템 리스트
    public List<BaseItem> quickSlots = new List<BaseItem>();    // 퀵슬롯 아이템 리스트
    public GameObject selectedItem = null;      // 선택된 아이템

    public GameObject ItemNameTxt;
    public GameObject ItemSubTxt;

    public int maxSlots = 9;                    // 인벤토리 슬롯 최대 개수
    public int maxQuickSlots = 5;               // 퀵슬롯 최대 개수

    public int currentSlots = 0;                // 현재 사용되는 인벤토리 슬롯

    private UIController_Test uiController;     // UI 컨트롤러

    void Start(){
        uiController = GameManager.Instance.uiController;

        // 인벤토리 슬롯 초기화
        for (int i = 0; i < maxSlots; i++){
            items.Add(null);
        }

        // 퀵슬롯 초기화
        for (int i = 0; i < maxQuickSlots; i++){
            quickSlots.Add(null);
        }
    }

    // 아이템 추가
    public void AddItem(BaseItem item) {
        item.itemData.count += 1;

        if (currentSlots < maxSlots) {
            int index = items.IndexOf(null);  // 빈 슬롯을 찾습니다.
            if (index != -1) {
                item.itemData.inventoryIndex = index;
                items[index] = item;
                uiController.UpdateInventoryUI(index);
                currentSlots += 1;
            } else {
                Debug.Log("인벤토리가 가득 찼습니다.");
            }
        } else {
            Debug.Log("인벤토리가 가득 찼습니다.");
        }
    }

    public void RemoveItem(BaseItem item) {
        if (items.Contains(item)) {
            items[item.itemData.inventoryIndex] = null;
            currentSlots -= 1;
            uiController.UpdateInventoryUI(item.itemData.inventoryIndex);
        } else {
            Debug.Log("인벤토리에 아이템이 존재하지 않습니다.");
        }
    }


    public void ChangeItemCount(BaseItem item) {
        int itemCount = item.itemData.count;

        if(itemCount > 0){
            item.itemData.count -= 1;
            
            //@TOD: 현재는 itemCount만 조절 추후 인벤토리 / 퀵슬롯 제거 여부에 따라 수정 필요
        }else{
            Debug.Log("아이템이 없습니다.");
        }
    }

    public BaseItem GetItemAt(int index){
        if (index >= 0 && index < items.Count){
            return items[index];
        }
        return null;
    }

    public void SetQuickSlotItem(GameObject item, int quickSlotIndex){
        BaseItem baseItem = item.GetComponent<BaseItem>();

        if (items.Contains(baseItem)) {
            if (quickSlotIndex >= 0 && quickSlotIndex < quickSlots.Count){
                quickSlots[quickSlotIndex] = baseItem;
                baseItem.itemData.quickSlotIndex = quickSlotIndex;

                Debug.Log($"아이템 '{baseItem.itemID}'이(가) 퀵슬롯 {quickSlotIndex}에 추가되었습니다.");
            }else{
                Debug.LogError("유효하지 않은 퀵슬롯 인덱스입니다.");
            }
        }else{
            Debug.LogError("인벤토리에 아이템이 존재하지 않습니다.");
        }
    }

    public void SwapInventoryItem(int originalIndex, int newIndex){

        Debug.Log($"인벤토리 아이템 스왑: {originalIndex} -> {newIndex}");
        BaseItem originalItem = GetItemAt(originalIndex);   // 옮길 아이템
        BaseItem newItem = GetItemAt(newIndex);             // 옮길 위치에 있는 아이템
        
        // 옮긴 위치에 슬롯이 비어있지 않은 경우
        if(newItem != null){
            items[newIndex] = originalItem; 
            items[originalIndex] = newItem;     
        }else{
            items[newIndex] = originalItem;
            items[originalIndex] = null;
        }
        
        originalItem.itemData.inventoryIndex = newIndex;
        if(items[originalIndex]) newItem.itemData.inventoryIndex = originalIndex;

        Debug.Log($"옮긴 아이템의 인벤토리 인덱스 : {items[newIndex].itemData.inventoryIndex}");
    }
}
