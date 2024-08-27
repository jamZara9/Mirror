using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot_bls : MonoBehaviour, IItemContainer, IPointerClickHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum SlotType
    {
        Inventory,
        QuickSlot
    }

    public SlotType slotType;     // ���� Ÿ��


    public Color testColor; // �׽�Ʈ�� �÷��ڵ�
    public int index;   // 인벤토리에서 검색&스왑시 빠르게 처리하기 위한 인덱스

    public UI_QuickSlot QuickSlot; //�ش� ������ �����ϰ��ִ� ������

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
            Clear();
        else
        {
            Set_Color(testColor);                   //테스트 코드
            _ItemIcon.sprite = SlotItem.Icon;        //아이콘 업데이트
            _countTXT.text = "" + SlotItem.Count;   //아이템 갯수 업데이트
        }

        QuickSlot?.Update_QuickSlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(name + "에 마우스 입장");

        if (SlotItem == null)
            return;


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log(name + "에 마우스 퇴장");

        if (SlotItem == null)
            return;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(name + "에 드랍");

        if (UI_DragSlot.instance.DragSlot == null)
            return;

        GameManager.Instance.inventoryManager.Add_InventorySlot(this);   //인벤토리에 자기자신(슬롯) 추가.swap_Item(this, UI_DragSlot.instance.DragSlot);
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
            Debug.Log(name + " ���� ���");

            if (SlotItem == null)
                return;
        }


        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(name + "���� �巡�� ����");

        if (SlotItem == null)
            return;

        UI_DragSlot.instance.DragSlot = this;
        UI_DragSlot.instance.DragSetItem(SlotItem);
        UI_DragSlot.instance.transform.position = eventData.position;
        UI_DragSlot.instance.Set_Alpha(1);
        UI_DragSlot.instance.Set_color(testColor);      //�׽�Ʈ�ڵ�
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(name + "�巡�� ��");

        if (SlotItem == null)
            return;

        UI_DragSlot.instance.transform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(name + " �巡�� ����");

        ///////////////////////////////////// �巡�� ���� ��� �Ϸ�
        UI_DragSlot.instance.Set_Alpha(0);
        UI_DragSlot.instance.DragSlot = null;
        ///////////////////////////////////// ������ ��� �Ϸ�

        Update_Slot();
    }

    public IInventoryItem Get_Item() 
    { 
        return SlotItem; 
    }

    public void Set_Color(Color _color)
    {
        testColor = _color;
        _ItemIcon.color = testColor;
    }

    //슬롯 초기화용 함수
    public void Clear()
    {
        Set_Color(Color.white); //테스트 코드

        SlotItem = null;            //슬롯 비우기
        _ItemIcon.sprite = null;     //아이콘 제거
        _countTXT.text = "";        //텍스트 초기화

        //QuickSlot.Clear();
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
