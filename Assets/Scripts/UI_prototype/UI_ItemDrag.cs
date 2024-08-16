using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Slot Data")]
    private UI_Slot Originalslot;
    private Canvas InventoryCanvas;
    private GraphicRaycaster raycaster;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(Originalslot.slotItem != null)
        {
            Originalslot = transform.parent.GetComponent<UI_Slot>();
            transform.SetParent(InventoryCanvas.transform);
            raycaster.enabled = false; // 드래그 중에 레이캐스트 비활성화
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        Originalslot = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
