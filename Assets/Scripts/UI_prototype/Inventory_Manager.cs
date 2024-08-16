using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using TMPro;
using UHFPS.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Inventory_Manager : MonoBehaviour
{
    private float Inventory_MaxSize = 12;
    private float QuickSlot_MaxSize = 4;

    //[SerializeField]
    public List<UI_Slot_bls> Inventory = new List<UI_Slot_bls>(12);
    public List<UI_QuickSlot> QuickSlot = new List<UI_QuickSlot>(4);

    public GameObject Text_ItemName;
    public GameObject Text_ItemDescription;

    public BaseItem[] testitems;

    public GameObject HUD_Canvas;
    public GameObject Inventory_Canvas;
    public GameObject QuickSlot_Canvas;

    public bool Use_Inventory = false;
    public bool Use_QuickSlot = false;

    private void Awake()
    {

    }

    private void Start()
    {
        Inventory.Sort(delegate (UI_Slot_bls a, UI_Slot_bls b) { return a.index.CompareTo(b.index); });
        QuickSlot.Sort(delegate (UI_QuickSlot a, UI_QuickSlot b) { return a.index.CompareTo(b.index); });

        Inventory_Canvas.SetActive(false);
        QuickSlot_Canvas.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && Use_Inventory)
        {
            testitems = FindObjectsOfType<BaseItem>();

            for (int i = 0; i < testitems.Length; i++)
            {
                UI_Slot_bls slot = Inventory.Find(x => x.Get_Item()?.itemData.name == testitems[i].itemData.name);

                if (slot != null)
                {
                    slot.Get_Item().itemData.count++;
                }
                else
                {
                    Add_Item(testitems[i]);

                }

            }
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            Use_Inventory = !Use_Inventory;
            Inventory_Canvas.SetActive(Use_Inventory);
        }

        if (Input.GetMouseButtonDown((int)MouseButton.Middle))
            Use_QuickSlot = true;

        if (Input.GetMouseButtonUp((int)MouseButton.Middle))
            Use_QuickSlot = false;

        if(!Use_Inventory)
            QuickSlot_Canvas.SetActive(Use_QuickSlot);

        foreach(UI_Slot_bls slot in Inventory)
        {
            if(slot.Get_Item()?.itemData.count == 0)
            {
                Remove_Item(slot);
            }
        }
    }

    public void swap_Item(UI_Slot_bls _from, UI_Slot_bls _to)
    {
        Debug.Log("아이템 스왑");

        /////////////////////// 등록된 퀵슬롯 교환
        UI_QuickSlot tempQSlot = _to.QuickSlot;
        _to.QuickSlot = _from.QuickSlot;
        _from.QuickSlot = tempQSlot;
        ///////////////////////
 
        ////////////////////////// 테스트 코드
        Color tempColor = _from.testColor;
        _from.testColor = _to.testColor;
        _to.testColor = tempColor;
        //////////////////////////

        /////////////////////// 아이템 교환
        BaseItem temp = _to.Get_Item();
        _to.Set_Item(_from.Get_Item());
        _from.Set_Item(temp);
        ///////////////////////
    }

    public void Add_Item(BaseItem _Item)
    {
        foreach (UI_Slot_bls slot in Inventory)
        {
            if(null == slot.Get_Item())
            {
                _Item.itemData.count++;
                slot.Set_Item(_Item);
                slot.Set_Color(new Color(Random.RandomRange(0f, 1f), Random.RandomRange(0f, 1f), Random.RandomRange(0f, 1f)));
                slot.Update_Slot();
                return;
            }
        }

        
    }

    public void Use_Item(UI_Slot_bls _Slot)
    {
        _Slot.Get_Item().itemData.count--;
        _Slot.Update_Slot();
    }

    public void Remove_Item(UI_Slot_bls _Slot)
    {
        _Slot.Clear();
    }

    public void Add_InventorySlot(UI_Slot_bls _Slot)
    {
        if(Inventory.Count < Inventory_MaxSize)
        {
            Inventory.Add(_Slot);
        }
    }

    public void Add_QuickSlot(UI_QuickSlot _Slot)
    {
        if (QuickSlot.Count < QuickSlot_MaxSize)
            QuickSlot.Add(_Slot);
    }

}
