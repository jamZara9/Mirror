using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Security.Cryptography;

public class UI_SlotItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler

{
    public Image itemIcon;                  // 아이템 이미지를 표시할 Image 컴포넌트
    public BaseItem item;


    [Header("Slot Data")]
    private Canvas canvas;                  // 캔버스[UI]
    private GraphicRaycaster raycaster;     // 레이캐스터
    private UI_Slot originalSlot;           // 기존 슬롯

    private void Start(){
        canvas = MainGameManager.Instance.uiController.canvas.GetComponent<Canvas>();
        raycaster = canvas.GetComponent<GraphicRaycaster>();
    }

    /// <summary>
    /// SlotItem에 아이템을 설정
    /// </summary>
    /// <param name="newItem">설정할 대상</param>
    public void SetItem(BaseItem newItem){
        item = newItem; 
        UpdateSlot();
    }

    public void ClearSlot(){
        item = null;
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

    // 드래그 시작 시 호출되는 함수
    public void OnBeginDrag(PointerEventData eventData){
        if (item != null){
            originalSlot = transform.parent.GetComponent<UI_Slot>(); // 드래그 시작 시점의 슬롯을 저장
            transform.SetParent(canvas.transform);
            raycaster.enabled = false; // 드래그 중에 레이캐스트 비활성화
        }
    }

    // e드래그 중일때 호출되는 함수
    public void OnDrag(PointerEventData eventData){
        if (item != null){
            transform.position = eventData.position;
        }
    }
    
    // 드래그가 끝날 때 호출되는 함수
    public void OnEndDrag(PointerEventData eventData){
        if(item == null){
            return;
        }

        // 포인터 아래에 있는 슬롯의 Transform
        Transform newSlotTransform = GetSlotUnderPointer(eventData);

        if(newSlotTransform == null){
            ReturnToOriginalSlot();
            return;
        }

        UI_Slot newSlot = newSlotTransform.GetComponent<UI_Slot>();

        if(newSlot == null){
            ReturnToOriginalSlot();
            return;
        }

        int originalIndex = originalSlot.transform.GetSiblingIndex();
        int newIndex = newSlot.transform.GetSiblingIndex();

        // 동일한 슬롯 타입 내에서 이동한 경우
        if(newSlot.slotType == originalSlot.slotType){
            HandleSameSlotTypeMove(newSlot);
        }else{  // 서로 다른 슬롯 타입으로 이동한 경우
            if(newSlot.slotType == UI_Slot.SlotType.QuickSlot){
                MainGameManager.Instance.playerInventory.SetQuickSlotItem(item.gameObject, newIndex);

                // 퀵슬롯에 이미 아이템이 있는 경우 해당 아이템을 비우고 새로운 아이템을 추가
                if(newSlot.transform.childCount > 0){
                    newSlot.ClearSlot();
                    Destroy(newSlot.transform.GetChild(0).gameObject);
                }
                newSlot.SetSlotItem(this);

                // 드래그가 끝난 후, SlotItem을 복사하여 새로운 슬롯에 추가
                MainGameManager.Instance.uiController.UpdateInventoryUI(originalIndex);
                transform.SetParent(newSlot.transform);
            }else{
                ReturnToOriginalSlot();
                return;
            }
        }

        transform.SetParent(newSlotTransform);
        transform.localPosition = Vector3.zero;
        raycaster.enabled = true;  // 드래그 종료 후 레이캐스트 활성화
    }
    
    /// <summary>
    /// 원래 슬롯으로 돌아가는 함수
    /// </summary>
    private void ReturnToOriginalSlot(){
        if(originalSlot != null){
            transform.SetParent(originalSlot.transform);
            transform.localPosition = Vector3.zero;
            raycaster.enabled = true;  // 드래그 종료 후 레이캐스트 활성화
        }else{
            Debug.LogError("원래 슬롯이 존재하지 않습니다.");
        }
    }


    /// <summary>
    /// 동일한 슬롯 타입 내에서 이동할 때 호출되는 함수
    /// </summary>
    /// <param name="newSlot"></param>
    private void HandleSameSlotTypeMove(UI_Slot newSlot){
        // newSlot의 게임오브젝트에 자식 객체로 SlotItem이 존재하는 경우 -> 스왑
        if(newSlot.transform.childCount > 0){
            UI_SlotItem existingItem = newSlot.transform.GetChild(0).GetComponent<UI_SlotItem>();
            existingItem.originalSlot = originalSlot;
            existingItem.transform.SetParent(originalSlot.transform);
            existingItem.transform.localPosition = Vector3.zero;
        }
        newSlot.SetSlotItem(this);
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
        // MainGameManager.Instance.playerInventory.MoveItemToQuickSlot(item.gameObject, 0);
    }

    /// <summary>
    /// 포인터 아래에 있는 슬롯을 찾는 함수
    /// </summary>
    /// <param name="eventData">포인터의 위치 데이터<param>
    /// <returns></returns>
    private Transform GetSlotUnderPointer(PointerEventData eventData){
        
        // 드래그 중인 SlotItem의 위치를 eventData.position으로 설정
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = eventData.position; 

        // 포인터 위치에서 레이캐스트를 수행한 후, 히트된 UI 요소들의 정보 저장하기 위한 리스트
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, raycastResults);    // GraphicRaycaster를 사용하여 레이캐스트 수행

        // 레이케스트 결과를 순회하면서 UI_Slot을 가지는 부모의 Transform을 찾아 반환
        foreach (RaycastResult result in raycastResults){
           
            Transform hitTransform = result.gameObject.transform;
            // Debug.Log($"Hit: {result.gameObject.name}");

            while (hitTransform != null){
                if (hitTransform.GetComponent<UI_Slot>() != null){
                    return hitTransform;
                }
                hitTransform = hitTransform.parent;
            }
        }

        // 슬롯이 발견되지 않으면 null
        return null;
    }
}
