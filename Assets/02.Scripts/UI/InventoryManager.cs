using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryManager : MonoBehaviour, IItemContainer
{
    private float Inventory_MaxSize = 12;
    private float QuickSlot_MaxSize = 4;

    static private int Inventory_Index = 0;


    [SerializeField] public List<UI_Slot_bls> Inventory = new List<UI_Slot_bls>(12);
    [SerializeField] public List<UI_QuickSlot> QuickSlot = new List<UI_QuickSlot>(4);

    //ui매니저 업데이트 하면서 이동예정
    public GameObject Text_ItemName;
    public GameObject Text_ItemDescription;
    //

    public BaseItem[] testitems;

    private UIManager _UIManager;

    public bool Use_Inventory = false;
    public bool Use_QuickSlot = false;

    private void Awake()
    {
        Debug.Log("inventory awake");
        _UIManager = GameManager.Instance.uiManager;
    }

    private void Start()
    {
        //인벤토리에 빈 슬롯 채우기
        for (int i = 0; i < Inventory_MaxSize; i++)
        {
                                                                           //   HUD_CANVAS           BackGround  InvenSlots   Slot
            UI_Slot_bls slot = GameManager.Instance.uiManager.GetCanvas("Inventory_Canvas").transform.GetChild(0).GetChild(0).GetChild(i).gameObject.GetComponent<UI_Slot_bls>();      // 이게 무슨.. 나중에 줄일게요
            Add_InventorySlot(slot);
        }

        Inventory.Sort(delegate (UI_Slot_bls a, UI_Slot_bls b) { return a.index.CompareTo(b.index); });
        QuickSlot.Sort(delegate (UI_QuickSlot a, UI_QuickSlot b) { return a.index.CompareTo(b.index); });
    }

    // Update is called once per frame
    void Update()
    {
        foreach (UI_Slot_bls slot in Inventory)
        {
            if (slot.Get_Item()?.Count == 0)
            {
                Remove_Item(slot);
            }
        }
    }

        /// <summary>
    /// 아이템을 획득했을 때 호출되는 함수
    /// </summary>
    //public void OnPickUp()
    //{
    //    testitems = FindObjectsOfType<BaseItem>();

    //    for (int i = 0; i < testitems.Length; i++)
    //    {
    //        UI_Slot_bls slot = Inventory.Find(x => x.Get_Item()?.ItemData.Name == testitems[i].itemData.name);

    //        if (slot != null)
    //        {
    //            slot.Get_Item().Count++;
    //        }
    //        else
    //        {
    //            AddItem(testitems[i]);

    //        }

    //    }
    //}

    /// <summary>
    /// 인벤토리를 열었을 때 호출되는 함수
    /// </summary>
    public void OnShowInventory()
    {
        Use_Inventory = !Use_Inventory;
        _UIManager.Inventory_Active(Use_Inventory);
    }

    /// <summary>
    /// 퀵슬롯을 열었을 때 호출되는 함수
    /// </summary>
    public void OnShowQuickSlot()
    {
        Use_QuickSlot = !Use_QuickSlot;
        _UIManager.QuickSlot_Active(Use_QuickSlot);
    }

    public void swap_Item(UI_Slot_bls _from, UI_Slot_bls _to)
    {
        Debug.Log("������ ����");

        UI_QuickSlot tempQslot = _to.QuickSlot;
        _to.QuickSlot = _from.QuickSlot;
        _from.QuickSlot = tempQslot;

        IInventoryItem tempItem = _to.Get_Item();
        _to.ChangeItem(_from.Get_Item());
        _from.ChangeItem(tempItem);
    }

    public void Use_Item(UI_Slot_bls _Slot)
    {
        _Slot.Get_Item().Count--;
        _Slot.Update_Slot();
    }

    public void Remove_Item(UI_Slot_bls _Slot)
    {
        _Slot.Clear();
    }

    public void Add_InventorySlot(UI_Slot_bls _Slot)
    {
        if(Inventory.Count < Inventory_MaxSize)
            Inventory.Add(_Slot);
    }

    public void Add_QuickSlot(UI_QuickSlot _Slot)
    {
        if (QuickSlot.Count < QuickSlot_MaxSize)
            QuickSlot.Add(_Slot);
    }

    /// <summary>
    /// 아이템을 획득했을 때 호출되는 함수
    /// </summary>
    public void AddItem(IInventoryItem item)
    {
        // UI_Slot_bls Slot = Inventory.Find(x => x.Get_Item()?.ItemData.Name == item.ItemData.Name);        //인벤토리에 동일한 이름의 아이템이 있는지 먼저 검색
        // if( null != Slot )                                                                  
        // {
        //     Slot.Get_Item().Count++;                                                        //인벤토리에 같은 이름의 아이템이 있다면 갯수만 늘려주고 종료
        //     Slot.Update_Slot();
        //     return;
        // }
        // else                                                                                
        // {
        //     foreach (UI_Slot_bls slot in Inventory)                                         //인벤토리에 같은 이름의 아이템이 없다면 리스트 순회하면서 빈 슬롯을 검색
        //     {
        //         if (null == slot.Get_Item())                                                //빈 슬롯 발견시 아이템 넣어주고 종료
        //         {
        //             item.Count++;
        //             slot.AddItem(item);     //빈슬롯에 아이템 입력
        //             Slot.Update_Slot();     //업데이트된 정보 반영
                    
        //             return;
        //         }
        //     }

        //     //빈 슬롯 조차 없는 경우
        //     Debug.Log("인벤토리 빈공간 부족");
        // }

        // 인벤토리에 동일한 이름의 아이템이 있는지 먼저 검색
        UI_Slot_bls slotWithSameItem  = Inventory.Find(x => x.Get_Item()?.ItemData.Name == item.ItemData.Name);
       
       if (slotWithSameItem != null)
        {
            // 인벤토리에 같은 이름의 아이템이 있다면 갯수만 늘려주고 종료
            slotWithSameItem.Get_Item().Count++;
            slotWithSameItem.Update_Slot();
            return;
        }

        // 인벤토리에 동일한 아이템이 없을 경우 

        // 인벤토리에 빈 슬롯이 있는지 검색
        UI_Slot_bls emptySlot = Inventory.Find(x => x.Get_Item() == null);

        if (emptySlot != null)
        {
            // 빈 슬롯 발견시 아이템 넣어주고 종료
            item.Count = 1;  // 아이템의 초기 수량을 1로 설정
            emptySlot.AddItem(item);
            emptySlot.Update_Slot();  // 업데이트된 정보 반영
            return;
        }

        // 빈 슬롯 조차 없는 경우
        Debug.Log("인벤토리 빈공간 부족");
    }

    public void RemoveItem(IInventoryItem item)
    {
        
    }
}
