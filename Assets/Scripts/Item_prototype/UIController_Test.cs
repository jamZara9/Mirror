using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController_Test : MonoBehaviour
{
    public GameObject uiSlotPrefab;             // SlotItem UI 프리팹
    public Transform itemSlotContainer;         // 아이템 슬롯들을 담을 컨테이너
    public Transform itemQuickSlotContainer;    // 퀵슬롯 아이템 슬롯들을 담을 컨테이너
    public GameObject hpText;                   // HP 텍스트 UI

    private PlayerInventory playerInventory;    // 플레이어 인벤토리
    private PlayerStatus playerStatus;          // 플레이어 상태

    public GameObject[] slots;                  // 슬롯 배열 (인벤토리 슬롯 개수만큼 생성)

    public GameObject[] quickSlots;             // 퀵슬롯(Slot) 배열

    void Start()
    {
        playerStatus = MainGameManager.Instance.playerStatus;
        playerInventory = MainGameManager.Instance.playerInventory;

        if (playerInventory == null){
            Debug.LogError("PlayerInventory를 찾을 수 없습니다.");
        }

        InitializeInventoryUI();
    }

    void Update()
    {
        if(hpText != null)
            hpText.GetComponent<TMPro.TextMeshProUGUI>().text = "HP : " + playerStatus.currentHealth;
    }

    private void HandleShowInventory(GameObject inventoryUI){
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    /// <summary>
    /// 인벤토리 UI 초기화
    /// </summary>
    void InitializeInventoryUI(){
        // 인벤토리 UI 초기화
        if (itemSlotContainer != null && itemQuickSlotContainer != null){
            slots = new GameObject[playerInventory.maxSlots];               // 슬롯 인벤터리 최대 개수만큼 생성
            quickSlots = new GameObject[playerInventory.maxQuickSlots];     // 퀵슬롯 인벤터리 최대 개수만큼 생성

            // 슬롯 생성
            for(int i = 0; i < itemSlotContainer.childCount; i++){          // 인벤토리 슬롯 개수만큼 반복
                if(i < playerInventory.maxSlots){
                    slots[i] = itemSlotContainer.GetChild(i).gameObject;    // 슬롯 배열에 UI 슬롯 추가
                    ClearSlot(slots[i]);                                    // 슬롯 초기화
                }
            }

            for(int i = 0; i < itemQuickSlotContainer.childCount; i++){     // 퀵슬롯 개수만큼 반복
                if(i < playerInventory.maxQuickSlots){
                    quickSlots[i] = itemQuickSlotContainer.GetChild(i).gameObject;  // 퀵슬롯 배열에 UI 슬롯 추가
                    ClearSlot(quickSlots[i]);                                  // 퀵슬롯 초기화
                }
            }
        }
    }

    /// <summary>
    /// 인벤토리의 Slot들을 비움
    /// </summary>
    /// <param name="slot"></param>
    private void ClearSlot(GameObject slot){
        foreach(Transform child in slot.transform){
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 인벤토리 UI 업데이트
    /// </summary>
    public void UpdateInventoryUI(int index){
        UpdateSlotUI(playerInventory.items, slots, index);
    }

    /// <summary>
    /// 퀵슬롯 UI 업데이트
    /// </summary>
    /// <param name="index">아이템을 배치할 Slot index</param>
    public void UpdateQuickSlotUI(int index){
        UpdateSlotUI(playerInventory.quickSlots, quickSlots, index);
    }

    /// <summary>
    /// 슬롯 UI 업데이트
    /// </summary>
    /// <param name="items">아이템을 담고 있는 List</param>
    /// <param name="slots">업데이트할 Slot 배열</param>
    /// <param name="index">업데이트할 Slot Index</param>
    private void UpdateSlotUI(List<BaseItem> items, GameObject[] slots, int index){
        // UI 슬롯 프리팹과 슬롯 컨테이너가 설정되었는지 확인
        if (uiSlotPrefab == null){
            Debug.LogError("UI Slot Prefab이 설정되지 않았습니다.");
            return;
        }

        if (itemSlotContainer == null){
            Debug.LogError("Item Slot Container가 설정되지 않았습니다.");
            return;
        }

        GameObject slot = slots[index]; // 슬롯(아이템을 보유할 Slot)을 가져옴
        ClearSlot(slot);

        GameObject slotItem = Instantiate(uiSlotPrefab, slot.transform);
        UI_Slot uiSlot = slotItem.GetComponent<UI_Slot>();

        if(uiSlot != null){
            uiSlot.SetItem(items[index]);
        }

        // slotItem을 slot의 자식으로 설정
        slotItem.transform.SetParent(slot.transform);
    }
}
