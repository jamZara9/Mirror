using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DragSlot : MonoBehaviour
{
    static public UI_DragSlot instance;
    public UI_Slot_bls DragSlot;
    public IInventoryItem Item;

    [SerializeField]
    private Image ItemIcon;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DragSetItem(IInventoryItem _slot)
    {
        Item = _slot;
        Update_DragSlot();

    }

    public void Update_DragSlot()
    {
        if(Item != null)
            ItemIcon.sprite = Item.Icon;


    }

    public void ActiveDragSlot(bool Active)
    {
        if(Active)
        {
            Set_Alpha(1f);
            ItemIcon.sprite = Item.Icon;
        }
        else
            Set_Alpha(0f);
    }

    public void Set_Alpha(float _alpha)
    {
        Color color = ItemIcon.color;
        color.a = _alpha;
        ItemIcon.color = color;
    }

}
