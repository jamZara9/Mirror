using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_QuickSlot : MonoBehaviour, IDropHandler
{
    public enum SlotType
    {
        Inventory,
        QuickSlot
    }

    public BaseItem Item;

    public SlotType slotType;     // ���� Ÿ��
    public Image ItemIcon;

    public InventoryManager InventoryMgr;

    public Color testcolor;
    public int index;

    private void Awake()
    {
        // InventoryMgr = GameObject.Find("Inventory")?.GetComponent<Inventory_Manager>();

        // Test
        InventoryMgr = GameObject.Find("GameManager")?.GetComponent<InventoryManager>();

        InventoryMgr.Add_QuickSlot(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (UI_DragSlot.instance.DragSlot == null)
            return;

        ///////////// �׽�Ʈ �ڵ�
        testcolor = UI_DragSlot.instance.testcolor;
        /////////////

        Item = UI_DragSlot.instance.Item;
        UI_DragSlot.instance.DragSlot.QuickSlot = this;

        Update_QuickSlot();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update_QuickSlot()
    {
        ItemIcon.color = testcolor;
    }

    public void Clear()
    {
        testcolor = Color.white;
        ItemIcon.color = testcolor;
        Item = null;
    }
}
