using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DragSlot : MonoBehaviour
{
    static public UI_DragSlot instance;
    public bool IsClear = true;         //슬롯에서 OnDrop호출시 유효한 액션인지 판단하기 위해 사용
    private UI_Slot_bls DragSlot;       //드래그 시작시 참조중인 슬롯
    private IInventoryItem Item;        //드래그 시작시 참소하는 슬롯의 아이템

    [SerializeField] private Image ItemIcon;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Clear()
    {
        Set_Alpha(0f);      //투명상태

        DragSlot = null;
        Item = null;
        ItemIcon.sprite = null;

        IsClear = true;
    }

    public void StartDrag(PointerEventData _eventData, UI_Slot_bls _slot)   //슬롯에서 드래그 시작시 호출
    {
        DragSlot = _slot;                                                   //슬롯 정보 업데이트
        Item = _slot.Get_Item();                                            //아이템 정보 업데이트
        transform.position = _eventData.position;                           //드래그 시작장소로 포지션 변경
        IsClear = false;                                                    //OnDrop시 유효한 상태체크용

        Set_Alpha(1f);                                                      //alpha를 1로 바꿔줘 투명상태 해제

        Update_DragSlot();
    }

    public void EndDrag()   //드래그가 끝난 시점에 호출
    {
        Clear();            //드래그 슬롯 초기화
    }

    private void Update_DragSlot()
    {
        if(Item != null)
            ItemIcon.sprite = Item.Icon;
    }

    private void Set_Alpha(float _alpha)
    {
        Color color = ItemIcon.color;
        color.a = _alpha;
        ItemIcon.color = color;
    }

    public UI_Slot_bls Get_Slot()
    {
        return DragSlot;
    }

}
