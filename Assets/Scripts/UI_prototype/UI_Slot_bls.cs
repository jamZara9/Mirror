using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot_bls : MonoBehaviour, IPointerClickHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum SlotType
    {
        Inventory,
        QuickSlot
    }

    public SlotType slotType;     // 슬롯 타입
    public Image ItemIcon; // 슬롯에 아이템이 있을경우 그릴 이미지


    public Color testColor; // 테스트용 컬러코드
    public int index;

    public GameObject count;

    public Inventory_Manager InventoryMgr;
    public UI_QuickSlot QuickSlot; //해당 슬롯을 참조하고있는 퀵슬롯

    [SerializeField, Header("Item")]
    private BaseItem SlotItem; //담고 있는 아이템

    



    private void Awake()
    {
        ////////////////////// 임시 코드
        InventoryMgr = GameObject.Find("Inventory")?.GetComponent<Inventory_Manager>();
        //////////////////////

        SlotItem = null;
        ItemIcon.sprite = null;
        QuickSlot = null;

        InventoryMgr.Add_InventorySlot(this);

    }

    // Start is called before the first frame update
    void Start()
    {

        
    }

    public void Update_Slot()
    {
        if (SlotItem == null)
        {
            Set_Color(Color.white);
            count.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            Set_Color(testColor);
            count.GetComponent<TextMeshProUGUI>().text = "" + SlotItem.itemData.count;
        }

        QuickSlot?.Update_QuickSlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(SlotItem == null)
            return;


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SlotItem == null)
            return;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(name + "에 드롭");

        if (UI_DragSlot.instance.DragSlot == null)
            return;

        InventoryMgr.swap_Item(this, UI_DragSlot.instance.DragSlot);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log(name + " 슬롯 클릭");

            if(SlotItem != null)
            {
                Debug.Log("있음");
                InventoryMgr.Text_ItemName.GetComponent<TMPro.TextMeshProUGUI>().text = SlotItem.itemData.name;
                InventoryMgr.Text_ItemDescription.GetComponent<TMPro.TextMeshProUGUI>().text = SlotItem.itemData.description;
            }
            else
            {
                Debug.Log("없음");
                InventoryMgr.Text_ItemName.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                InventoryMgr.Text_ItemDescription.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log(name + " 슬롯 사용");

            if (SlotItem == null)
                return;

            InventoryMgr.Use_Item(this);
        }
            

        if (SlotItem == null)
            return;



        //인벤토리 매니저에서 아이템 이름, 설명 출력
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(name + "에서 드래그 시작");

        if (SlotItem == null)
            return;

        UI_DragSlot.instance.DragSlot = this;
        UI_DragSlot.instance.DragSetItem(SlotItem);
        UI_DragSlot.instance.transform.position = eventData.position;
        UI_DragSlot.instance.Set_Alpha(1);
        UI_DragSlot.instance.Set_color(testColor);      //테스트코드
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(name + "드래그 중");

        if (SlotItem == null)
            return;

        UI_DragSlot.instance.transform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(name + " 드래그 끝남");

        ///////////////////////////////////// 드래그 슬롯 사용 완료
        UI_DragSlot.instance.Set_Alpha(0);
        UI_DragSlot.instance.DragSlot = null;
        ///////////////////////////////////// 퀵슬롯 등록 완료

        Update_Slot();
    }

    public void Set_Item(BaseItem _Item)
    {
        SlotItem = _Item;

        Update_Slot();
    }

    public BaseItem Get_Item() 
    { 
        return SlotItem; 
    }

    public void Set_Color(Color _color)
    {
        testColor = _color;
        ItemIcon.color = testColor;
    }

    public void Clear()
    {
        //테스트 코드
        Set_Color(Color.white);

        SlotItem = null;
        ItemIcon.sprite = null;
        count.GetComponent<TextMeshProUGUI>().text = "";
        QuickSlot.Clear();
    }


}
