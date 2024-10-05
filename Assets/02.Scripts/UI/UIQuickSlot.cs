using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_QuickSlot : MonoBehaviour, IDropHandler
{
    public int index;

    private InventoryManager _InventoryMgr = null;
    private Image _ItemIcon = null;
    private TextMeshProUGUI _countTXT = null;
    private Image HUD_QuickSlot = null;

    private void Awake()
    {
        _InventoryMgr = GameObject.Find("GameManager")?.GetComponent<InventoryManager>();
        _InventoryMgr.Add_QuickSlot(this);

        _countTXT = transform.Find("Count")?.GetComponent<TextMeshProUGUI>();
        _ItemIcon = transform.Find("Icon")?.GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform QuickSlotUI_BackGround = GameManager.uiManager.GetCanvas("QuickSlot_Canvas").transform.GetChild(0).GetChild(0);
        GameObject QuickSlot = QuickSlotUI_BackGround.Find("QuickSlot" + (index + 1)).gameObject;
        HUD_QuickSlot = QuickSlot.transform.Find("Icon")?.GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (UI_DragSlot.instance.IsClear)
            return;

        UI_DragSlot.instance.Get_Slot().QuickSlot = this;
    }



    public void Update_QuickSlot(IInventoryItem _item)
    {
        Debug.Log("퀵슬롯 업데이트");
        _ItemIcon.sprite = _item.Icon;        //아이콘 업데이트
        _countTXT.text = "" + _item.Count;   //아이템 갯수 업데이트


        //      마우스 가운데 클릭하면 나오는 퀵슬롯 UI업데이트     //
        //------------------------------------------------------//
        HUD_QuickSlot.sprite = _item.Icon;
    }

    public void Clear()
    {
        _ItemIcon.sprite = null;
        _countTXT.text = "";
        HUD_QuickSlot.sprite = null;
    }


}
