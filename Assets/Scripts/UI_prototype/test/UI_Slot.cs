using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 슬롯의 UI 업데이트
/// </summary>
public class UI_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum SlotType
    {
        Inventory,
        QuickSlot
    }

    public SlotType slotType;     // 슬롯 타입
    public Image border;            // 슬롯 테두리 이미지
    public UI_SlotItem slotItem;  // 슬롯에 할당된 아이템

    public void OnPointerEnter(PointerEventData eventData){
        if(border != null){ 
            border.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        if(border != null){
            border.enabled = false;
        }
    }

    // 슬롯에 아이템을 설정
    public void SetSlotItem(UI_SlotItem item){
        slotItem = item;
        UpdateSlot();
    }

    // 슬롯에 있는 아이템을 제거
    public void ClearSlot(){
        slotItem = null;
        UpdateSlot();
    }

    // 슬롯 업데이트
    private void UpdateSlot(){
        if (slotItem != null){
            slotItem.UpdateSlot();
        }else{
            // 슬롯 아이콘을 지웁니다.
            slotItem?.ClearSlot();
        }
    }

}
