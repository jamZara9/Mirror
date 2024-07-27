using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// SlotItem이 보유한 아이템 정보를 처리하는 클래스
/// </summary>
public class UI_Slot : MonoBehaviour
{
    public Image itemIcon;                  // 아이템 이미지를 표시할 Image 컴포넌트
    private BaseItem item;

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
}
