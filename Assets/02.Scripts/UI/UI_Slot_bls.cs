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

    public SlotType slotType;     // ���� Ÿ��
    public Image ItemIcon; // ���Կ� �������� ������� �׸� �̹���


    public Color testColor; // �׽�Ʈ�� �÷��ڵ�
    public int index;

    public GameObject count;

    public Inventory_Manager InventoryMgr;
    public UI_QuickSlot QuickSlot; //�ش� ������ �����ϰ��ִ� ������

    [SerializeField, Header("Item")]
    private BaseItem SlotItem; //��� �ִ� ������

    



    private void Awake()
    {
        // ////////////////////// �ӽ� �ڵ�
        // InventoryMgr = GameObject.Find("Inventory")?.GetComponent<Inventory_Manager>();
        // //////////////////////

        // Test
        InventoryMgr = GameObject.Find("GameManager")?.GetComponent<Inventory_Manager>();


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
        Debug.Log(name + "�� ���");

        if (UI_DragSlot.instance.DragSlot == null)
            return;

        InventoryMgr.swap_Item(this, UI_DragSlot.instance.DragSlot);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log(name + " ���� Ŭ��");

            if(SlotItem != null)
            {
                Debug.Log("����");
                InventoryMgr.Text_ItemName.GetComponent<TMPro.TextMeshProUGUI>().text = SlotItem.itemData.name;
                InventoryMgr.Text_ItemDescription.GetComponent<TMPro.TextMeshProUGUI>().text = SlotItem.itemData.description;
            }
            else
            {
                Debug.Log("����");
                InventoryMgr.Text_ItemName.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                InventoryMgr.Text_ItemDescription.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log(name + " ���� ���");

            if (SlotItem == null)
                return;

            InventoryMgr.Use_Item(this);
        }
            

        if (SlotItem == null)
            return;



        //�κ��丮 �Ŵ������� ������ �̸�, ���� ���
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
        //�׽�Ʈ �ڵ�
        Set_Color(Color.white);

        SlotItem = null;
        ItemIcon.sprite = null;
        count.GetComponent<TextMeshProUGUI>().text = "";
        QuickSlot.Clear();
    }


}
