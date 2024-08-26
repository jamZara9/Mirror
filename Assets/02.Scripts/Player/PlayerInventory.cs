using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IItemContainer
{
    public List<GameObject> inventoryItems = new();     // 플레이어의 인벤토리 아이템 리스트
    public List<GameObject> quickSlots = new();         // 퀵슬롯 아이템 리스트

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
            inventoryItems.Add(null);
        }

        // 퀵슬롯 초기화
        for (int i = 0; i < maxQuickSlots; i++){
            quickSlots.Add(null);
        }
    }

    /// <summary>
    /// 아이템 추가
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(IInventoryItem item) {
        // 아이템 개수 증가
        item.Count += 1;

        if (currentSlots < maxSlots) {
            int index = inventoryItems.IndexOf(null);  // 빈 슬롯을 찾습니다.
            if (index != -1) {
                item.InventoryIndex = index;
                inventoryItems[index] = item.ItemGameObject;
                // uiController.UpdateInventoryUI(index);            // UI 업데이트 [ 현재 UIController 미완성]
                currentSlots += 1;
            } else {
                Debug.Log("인벤토리가 가득 찼습니다.");
            }
        } else {
            Debug.Log("인벤토리가 가득 찼습니다.");
        }
    }

    public void RemoveItem(IInventoryItem item) {
        if (inventoryItems.Contains(item.ItemGameObject)) {
            inventoryItems[item.InventoryIndex] = null;
            currentSlots -= 1;
            uiController.UpdateInventoryUI(item.InventoryIndex);
        } else {
            Debug.Log("인벤토리에 아이템이 존재하지 않습니다.");
        }
    }

    /// <summary>
    /// 아이템 선택
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GetItemAt(int index)
    {
        if (index >= 0 && index < inventoryItems.Count)
        {
            return inventoryItems[index];
        }
        return null;
    }

    /// <summary>
    /// 인벤토리 속 아이템을 퀵슬롯으로 이동
    /// </summary>
    /// <param name="item">옮기고자 하는 Item</param>
    /// <param name="quickSlotIndex">퀵슬롯 위치</param>
    public void SetQuickSlotItem(GameObject item, int quickSlotIndex)
    {
        IInventoryItem toQuickSlotItem = item.GetComponent<IInventoryItem>();

        if (toQuickSlotItem != null && inventoryItems.Contains(toQuickSlotItem.ItemGameObject))
        {
            if (quickSlotIndex >= 0 && quickSlotIndex < quickSlots.Count)
            {
                quickSlots[quickSlotIndex] = toQuickSlotItem.ItemGameObject;
                toQuickSlotItem.InventoryIndex = quickSlotIndex;

                Debug.Log($"아이템 '{toQuickSlotItem.ItemID}'이(가) 퀵슬롯 {quickSlotIndex}에 추가되었습니다.");
            }
            else
            {
                Debug.LogError("유효하지 않은 퀵슬롯 인덱스입니다.");
            }
        }
        else
        {
            Debug.LogError("인벤토리에 아이템이 존재하지 않습니다.");
        }
    }

    /// <summary>
    /// 인벤토리 아이템 스왑
    /// </summary>
    /// <param name="originalIndex">기존 아이템의 인벤토리 위치(인덱스)</param>
    /// <param name="newIndex">아이템을 이동시킬 인벤토리 위치(인덱스)</param>
    public void SwapInventoryItem(int originalIndex, int newIndex)
    {
        Debug.Log($"인벤토리 아이템 스왑: {originalIndex} -> {newIndex}");
        IInventoryItem originalItem = GetItemAt(originalIndex).GetComponent<IInventoryItem>();
        IInventoryItem newItem = GetItemAt(newIndex).GetComponent<IInventoryItem>();

        if (newItem != null)
        {
            inventoryItems[newIndex] = originalItem.ItemGameObject;
            inventoryItems[originalIndex] = newItem.ItemGameObject;
        }
        else
        {
            inventoryItems[newIndex] = originalItem.ItemGameObject;
            inventoryItems[originalIndex] = null;
        }

        originalItem.InventoryIndex = newIndex;
        if (inventoryItems[originalIndex] != null) newItem.InventoryIndex = originalIndex;

        Debug.Log($"옮긴 아이템의 인벤토리 인덱스 : {newIndex}");
    }
}
