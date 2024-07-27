using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController_Test : MonoBehaviour
{
    public GameObject uiSlotPrefab;         // SlotItem UI 프리팹
    public Transform itemSlotContainer;     // 아이템 슬롯들을 담을 컨테이너
    public GameObject hpText;      // HP 텍스트 UI

    private PlayerInventory playerInventory;
    private PlayerStatus playerStatus;

    [SerializeField]
    private GameObject[] slots;             // 슬롯 배열 (인벤토리 슬롯 개수만큼 생성)

    void Start()
    {
        playerStatus = GameObject.Find("Player").GetComponent<PlayerStatus>();
        playerInventory = GameObject.Find("Player").GetComponent<PlayerInventory>();

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
        if (itemSlotContainer != null){
            slots = new GameObject[playerInventory.maxSlots];             // 슬롯 인벤터리 최대 개수만큼 생성

            // 슬롯 생성
            for(int i = 0; i < itemSlotContainer.childCount; i++){        // 인벤토리 슬롯 개수만큼 반복
                if(i < playerInventory.maxSlots){
                    slots[i] = itemSlotContainer.GetChild(i).gameObject;  // 슬롯 배열에 UI 슬롯 추가
                    ClearSlot(slots[i]);                                  // 슬롯 초기화
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
    public void UpdateInventoryUI(){
        // UI 슬롯 프리팹과 슬롯 컨테이너가 설정되었는지 확인
        if (uiSlotPrefab == null){
            Debug.LogError("UI Slot Prefab이 설정되지 않았습니다.");
            return;
        }

        if (itemSlotContainer == null){
            Debug.LogError("Item Slot Container가 설정되지 않았습니다.");
            return;
        }

        for (int i = 0; i < playerInventory.items.Count; i++){
            if(i < slots.Length){
                GameObject slot = slots[i]; // 슬롯(아이템을 보유할 Slot)을 가져옴
                ClearSlot(slot);

                GameObject slotItem = Instantiate(uiSlotPrefab, slot.transform);
                UI_Slot uiSlot = slotItem.GetComponent<UI_Slot>();

                if(uiSlot != null){
                    uiSlot.SetItem(playerInventory.items[i]);
                }

                // slotItem을 slot의 자식으로 설정
                slotItem.transform.SetParent(slot.transform);
            }
        }
    }
}
