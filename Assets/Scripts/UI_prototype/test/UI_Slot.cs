using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// SlotItem이 보유한 아이템 정보를 처리하는 클래스
/// </summary>
public class UI_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image itemIcon;                  // 아이템 이미지를 표시할 Image 컴포넌트
    private BaseItem item;
    public Image border;                    // 슬롯 테두리 이미지

    /// <summary>
    /// SlotItem에 아이템을 설정
    /// </summary>
    /// <param name="newItem">설정할 대상</param>
    public void SetItem(BaseItem newItem){
        item = newItem; 
        UpdateSlot();
    }


    // 슬롯 업데이트
    public void UpdateSlot(){
        if (item != null){
            if (itemIcon != null){
                itemIcon.sprite = Resources.Load<Sprite>(item.itemData.iconPath);
            }
        }else{
            if (itemIcon != null){
                itemIcon.sprite = null;
            }
        }
    }


    /// <summary>
    /// 마우스가 슬롯 위에 있을 때 호출되는 함수 [아직 x]
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData){
        if(border != null){ 
            border.enabled = true;
        }
    }
    
    /// <summary>
    /// 마우스가 슬롯 밖으로 나갈 때 호출되는 함수 [아직 x]
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData){
        if(border != null){
            border.enabled = false;
        }
    }

    /// <summary>
    /// 마우스 클릭 시 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    /// @TODO: 아이템을 선택 후 퀵슬롯을 지정해야 해당 퀵 슬롯에 아이템이 추가되도록 수정필요
    public void OnPointerClick(PointerEventData eventData){
        MainGameManager.Instance.playerInventory.selectedItem = item.gameObject;
        Debug.Log($"아이템 '{item.itemData.name}'이(가) 선택되었습니다.");

        // 임시로 자동으로 퀵슬롯에 추가
        MainGameManager.Instance.playerInventory.MoveItemToQuickSlot(item.gameObject, 0);
    }

}
