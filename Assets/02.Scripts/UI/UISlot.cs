using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot_bls : MonoBehaviour, IItemContainer, IPointerClickHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int index;   // 인벤토리에서 검색&스왑시 빠르게 처리하기 위한 인덱스

    public UI_QuickSlot QuickSlot; //

    [SerializeField, Header("Item")]
    private IInventoryItem SlotItem; //��� �ִ� ������

    [SerializeField] private TextMeshProUGUI _countTXT; //아이템 갯수를 표시할 text
    [SerializeField] private Image _ItemIcon; //아이템 아이콘




    private void Awake()
    {
        _countTXT = transform.Find("Amount")?.GetComponent<TextMeshProUGUI>();
        _ItemIcon = transform.Find("Icon")?.GetComponent<Image>();

        Clear();    //슬롯 초기화
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Update_Slot()   //슬롯의 아이템 종류에 따라(없는경우 포함) UI 업데이트
    {
        if (SlotItem == null)   //슬롯에 아이템이 없을경우 초기화
        {
            Clear();
            QuickSlot?.Clear();
        }
        else
        {
            _ItemIcon.sprite = SlotItem.Icon;        //아이콘 업데이트
            _countTXT.text = "" + SlotItem.Count;   //아이템 갯수 업데이트
            QuickSlot?.Update_QuickSlot(SlotItem);

            // Test
            GameManager.Instance.playerInventory.quickSlots[index-1] = SlotItem.ItemGameObject;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SlotItem == null)
            return;


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SlotItem == null)
            return;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (UI_DragSlot.instance.IsClear)   //드래그 슬롯이 비어진 상태 == 드롭할 정보 없음
            return;

        Debug.Log(name + "에 드랍");

        GameManager.Instance.inventoryManager.swap_Item(UI_DragSlot.instance.Get_Slot(), this);

        UI_DragSlot.instance.EndDrag();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log(name + " 슬롯 클릭");

            if(SlotItem != null)
            {
                GameManager.Instance.inventoryManager.Text_ItemName.GetComponent<TextMeshProUGUI>().text = SlotItem.ItemData.Name;
                GameManager.Instance.inventoryManager.Text_ItemDescription.GetComponent<TextMeshProUGUI>().text = SlotItem.ItemData.Description;
            }
            else
            {
                GameManager.Instance.inventoryManager.Text_ItemName.GetComponent<TextMeshProUGUI>().text = "";
                GameManager.Instance.inventoryManager.Text_ItemDescription.GetComponent<TextMeshProUGUI>().text = "";
            }
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if (SlotItem == null)
                return;

            Debug.Log(name + SlotItem.ItemData.Name + " 아이템 사용");

            GameManager.Instance.inventoryManager.Use_Item(this);
        }


        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (SlotItem == null)
            return;

        Debug.Log(name + "에서 드래그 시작");

        UI_DragSlot.instance.StartDrag(eventData, this);   //퀵슬롯에 필요한 정보를 처리하기 위해 드래그를 시작한 슬롯의 정보를 넘겨줌
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (SlotItem == null)
            return;

        UI_DragSlot.instance.transform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (SlotItem == null)
            return;

        Debug.Log(name + "End drag");

        UI_DragSlot.instance.EndDrag();

        Update_Slot();
    }

    public IInventoryItem Get_Item() 
    { 
        return SlotItem; 
    }

    //슬롯 초기화용 함수
    public void Clear()
    {
        SlotItem = null;            //슬롯 비우기
        _ItemIcon.sprite = null;    //아이콘 제거
        _countTXT.text = "";        //텍스트 초기화

        QuickSlot?.Clear();
        QuickSlot = null;           //참조중인 퀵슬롯 제거
    }

    public void AddItem(IInventoryItem item)
    {
        SlotItem = item;

        Update_Slot();
    }

    //AddItem과 같은 동작을 하지만 외부에서 함수 호출시 혼란을 방지하기 위해 사용
    public void ChangeItem(IInventoryItem item)
    {
        SlotItem = item;

        Update_Slot();
    }

    public void RemoveItem(IInventoryItem item)
    {
        Debug.Log("아이템 제거 코드 실행");  //임시 코드
    }
}
